mod actions;
mod project;
mod setup;

use std::{env, fs, thread};
use std::process::Command;
use std::time::Instant;
use crate::actions::busybox::{busybox_actions, busybox_pre_actions};
use crate::actions::init::init_actions;
use crate::actions::kernel::{kernel_actions, kernel_pre_actions};
use crate::project::Project;
use crate::setup::setup;

fn main() -> Result<(), Box<dyn std::error::Error>> {
    // Set up variables
    let mut build_kernel = true;
    let mut build_image = true;
    let mut build_run = false;
    let mut build_debug = true;
    let mut build_clean = false;
    let cores = thread::available_parallelism().expect("REASON").get();
    let version = "2.0-internal-test";

    #[cfg(target_arch = "x86")]
    let mut build_arch = "i386";
    #[cfg(target_arch = "x86_64")]
    let mut build_arch = "x86_64";
    #[cfg(target_arch = "aarch64")]
    let mut build_arch = "aarch64";
    #[cfg(target_arch = "powerpc")]
    let mut build_arch = "powerpc";

    // Command line arguments
    let args: Vec<String> = env::args().collect();

    for string in args {
        match string.as_str() { // Basically a rust "switch" case
            "--no-kernel" => build_kernel = false, // Disables compiling the kernel
            "--x86" => build_arch = "i386", // Compiles the OS for x86 systems
            "--amd64" => build_arch = "x86_64", // Compiles the OS for amd64/x86_64 systems
            "--arm64" => build_arch = "aarch64", // Compiles the OS for arm64 systems
            "--powerpc" => build_arch = "powerpc", // Compiles the OS for 32 bit PPC systems
            "--no-image" => build_image = false, // Disables building an image
            "--run" => build_run = true,
            "--release" => build_debug = false, // Compiles the OS with release
            "--clean" => build_clean = true, // Just cleans
            "--setup" => setup(build_arch),
            _ => {} // "default"
        }
    }
 
    match build_arch {
        "i386"|"x86_64"|"aarch64"|"powerpc" => {},
        _ => {
            eprintln!("Error: Unsupported architecture! Must be x86, amd64, arm64, powerpc, or all.");
            eprintln!("You may report your architecture to the GoOS devs over at Owen2k6 network, and we may consider support for your architecture.");
            std::process::exit(1);
        }
    }

    let rust_target_table = match build_arch {
        "x86_64" => "x86_64-unknown-linux-",
        "aarch64" => "aarch64-unknown-linux-",
        "i386" => "i686-unknown-linux-",
        "powerpc" => "powerpc-unknown-linux-",
        _ => "fucking hell mate"
    };

    println!("Building GoOS for {} using {} threads", build_arch, cores);//
    let total_timer = Instant::now();

    let mut dir = env::current_dir()?;

    loop {
        if let Some(name) = dir.file_name() {
            if name == "GoOS" {
                // Change current working directory
                env::set_current_dir(&dir)?;
                println!("Changed working directory to {:?}", dir);
                break;
            }
        }

        // Move up one level
        if let Some(parent) = dir.parent() {
            dir = parent.to_path_buf();
        } else {
            // Reached root and didn't find GoOS
            eprintln!("Error: GoOS root not found in parent directories.");
            std::process::exit(1);
        }
    }

    if fs::exists("out")? {
        // Delete out directory
        println!("Deleting ./out");

        if let Err(e) = fs::remove_dir_all("out") {
            eprintln!("Failed to delete {:?}: {}", "./out", e);
        }
    }

    if build_clean {
        let root = "./";

        for entry in fs::read_dir(root)? {
            let entry = entry?;
            let path = entry.path();

            // Only look at directories
            if !path.is_dir() {
                continue;
            }

            // Skip the gobuilder folder
            if path.file_name().unwrap() == "gobuilder" {
                continue;
            }

            // Look for "bin" and "target" inside this directory
            for sub in ["bin", "target"] {
                let sub_path = path.join(sub);
                if sub_path.exists() {
                    println!("Deleting {:?}", sub_path);
                    if let Err(e) = fs::remove_dir_all(&sub_path) {
                        eprintln!("Failed to delete {:?}: {}", sub_path, e);
                    }
                }
            }
        }

        std::process::exit(0);
    }

    // Remake out, the arch folder, and the ISO Image before being iso-ed folder in one go
    println!("Creating ./out/{}/image", build_arch);
    if let Err(e) = fs::create_dir_all(format!("out/{}/image", build_arch)) {
        eprintln!("Failed to create {:?}: {}", format!("./out/{}/image", build_arch), e);
        std::process::exit(1);
    }

    do_dirs(build_arch);

    // Add new projects in add_c_projects and add_rust_projects (these are found at the bottom of the file)
    let c_projects = add_c_projects(); // Array for C based projects to compile
    let rust_projects = add_rust_projects();

    // List of completed projects
    let mut completed_builds: Vec<String> = Vec::new();

    while completed_builds.len() < c_projects.len() + rust_projects.len() {
        'c_projects: for project in &c_projects {
            if !build_kernel && project.name == "kernel" {
                continue 'c_projects;
            }

            for dependency in &project.dependencies {
                if !completed_builds.contains(dependency) {
                    continue 'c_projects;
                }
            }

            if !completed_builds.contains(&project.name)
            {
                let timer = Instant::now();
                println!("Building {}...", project.name);

                match project.pre_build {
                    Some(f) => {
                        if let Err(e) = f(build_arch, "", build_debug) {
                            eprintln!("Pre-build action failed for project {}: {}", &project.name, e);
                            std::process::exit(1);
                        }
                        println!("Ran pre-build action for project {}", &project.name);
                    },
                    None => println!("No pre-build action for project {}, continuing", &project.name)
                }

                let status = Command::new("make")
                    .args(&[
                        &format!("ARCH={}", if build_arch == "aarch64" { "arm64" } else { build_arch }),
                        "LLVM=1",
                        &format!("-j{}", cores)
                    ])
                    .current_dir(&project.name)
                    .status()?;

                if !status.success() {
                    eprintln!("Build failed for {}!", project.name);
                    std::process::exit(1);
                }

                match project.post_build {
                    Some(f) => { f(build_arch, "", build_debug); println!("Ran post-build action for project {} ", &project.name); },
                    None => println!("No post-build action for project {}, continuing", &project.name)
                }

                println!("Done building {}, took {:?}", project.name, timer.elapsed());

                completed_builds.push(project.name.clone())
            }
        }

        'rust_projects: for project in &rust_projects {
            for dependency in &project.dependencies {
                if !completed_builds.contains(dependency) {
                    continue 'rust_projects;
                }

                //let rust_target = &(rust_target_table.to_owned() + if project.name == "init" { "musl" } else { "gnu" });
                let rust_target = &(rust_target_table.to_owned() + "musl");

                if !completed_builds.contains(&project.name)
                {
                    let timer = Instant::now();
                    println!("Building {}...", project.name);

                    match project.pre_build {
                        Some(f) => {
                            if let Err(e) = f(build_arch, rust_target, build_debug) {
                                eprintln!("Pre-build action failed for project {}: {}", &project.name, e);
                                std::process::exit(1);
                            }
                            println!("Ran pre-build action for project {}", &project.name);
                        },
                        None => println!("No pre-build action for project {}, continuing", &project.name)
                    }

                    if build_debug {
                        let status = Command::new("cargo")
                            .args(&[
                                "build",
                                "--target", rust_target
                            ])
                            .current_dir(&project.name)
                            .status()?;

                        if !status.success() {
                            eprintln!("Build failed for {}!", project.name);
                            std::process::exit(1);
                        }
                    }
                    else {
                        let status = Command::new("cargo")
                            .args(&[
                                "build",
                                "--release",
                                "--target", rust_target
                            ])
                            .current_dir(&project.name)
                            .status()?;

                        if !status.success() {
                            eprintln!("Build failed for {}!", project.name);
                            std::process::exit(1);
                        }
                    }

                    match project.post_build {
                        Some(f) => { f(build_arch, rust_target, build_debug); println!("Ran post-build action for project {} ", &project.name); },
                        None => println!("No post-build action for project {}, continuing", &project.name)
                    }

                    println!("Done building {}, took {:?}", project.name, timer.elapsed());

                    completed_builds.push(project.name.clone())
                }
            }
        }
    }

    do_initramfs(build_arch)?;

    if build_image {
        do_image(build_arch, version, build_debug)?;
    }

    println!("Done! Total time elapsed: {:?}", total_timer.elapsed());

    if build_run {
        println!("Running via QEMU...");

        let status = Command::new(format!("qemu-system-{}", build_arch))
            .args(&[
                "-cdrom",
                &format!("./out/{}/GoOS-{}-{}{}.iso", build_arch, version, build_arch, if build_debug { "-debug" } else { "" }),
                "-m",
                "512M", // increase as needed
                if build_debug { "-serial" } else { "" },
                if build_debug { "stdio" } else { "" }
            ])
            .status()?;

        if !status.success() {
            eprintln!("Failed while running!");
            std::process::exit(1);
        }
    }

    Ok(())
}

fn do_image(build_arch: &str, version: &str, build_debug: bool) -> std::io::Result<()> {
    let timer = Instant::now();
    println!("Building bootable image...");

    let status = Command::new("grub-mkrescue")
        .args(&[
            "-o",
            &format!("./out/{}/GoOS-{}-{}{}.iso", build_arch, version, build_arch, if build_debug { "-debug" } else { "" }),
            &format!("./out/{}/image", build_arch)
        ])
        .status()?;

    if !status.success() {
        eprintln!("Failed to create bootable image!");
        std::process::exit(1);
    }

    println!("Done building bootable image, took {:?}", timer.elapsed());

    Ok(())
}

fn do_initramfs(build_arch: &str) -> std::io::Result<()> {
    let timer = Instant::now();
    println!("Building initramfs...");

    let status = Command::new("sh")
        .args(&["-c", "find . -print0 | cpio -o -H newc -0 | gzip -9 > ../initramfs.img"])
        .current_dir(format!("./out/{}/image", build_arch))
        .status()?;

    if !status.success() {
        eprintln!("Failed to create initramfs!");
        std::process::exit(1);
    }

    println!("Moving ./out/{}/initramfs.img to ./out/{}/image/boot/initramfs.img", build_arch, build_arch);
    if let Err(e) = fs::rename(format!("out/{}/initramfs.img", build_arch), format!("out/{}/image/boot/initramfs.img", build_arch)) {
        eprintln!("Failed to move {:?}: {}", format!("./out/{}/initramfs.img", build_arch), e);
        std::process::exit(1);
    }

    println!("Done building initramfs, took {:?}", timer.elapsed());

    Ok(())
}

fn do_dirs(build_arch: &str) {
    println!("Creating ./out/{}/image/proc", build_arch);
    if let Err(e) = fs::create_dir_all(format!("out/{}/image/proc", build_arch)) {
        eprintln!("Failed to create {:?}: {}", format!("./out/{}/image/proc", build_arch), e);
        std::process::exit(1);
    }

    println!("Creating ./out/{}/image/sys", build_arch);
    if let Err(e) = fs::create_dir_all(format!("out/{}/image/sys", build_arch)) {
        eprintln!("Failed to create {:?}: {}", format!("./out/{}/image/sys", build_arch), e);
        std::process::exit(1);
    }

    println!("Creating ./out/{}/image/tmp", build_arch);
    if let Err(e) = fs::create_dir_all(format!("out/{}/image/tmp", build_arch)) {
        eprintln!("Failed to create {:?}: {}", format!("./out/{}/image/tmp", build_arch), e);
        std::process::exit(1);
    }

    println!("Creating ./out/{}/image/dev", build_arch);
    if let Err(e) = fs::create_dir_all(format!("out/{}/image/dev", build_arch)) {
        eprintln!("Failed to create {:?}: {}", format!("./out/{}/image/dev", build_arch), e);
        std::process::exit(1);
    }

    println!("Creating ./out/{}/image/lib", build_arch);
    if let Err(e) = fs::create_dir_all(format!("out/{}/image/lib", build_arch)) {
        eprintln!("Failed to create {:?}: {}", format!("./out/{}/image/lib", build_arch), e);
        std::process::exit(1);
    }

    println!("Creating ./out/{}/image/bin", build_arch);
    if let Err(e) = fs::create_dir_all(format!("out/{}/image/bin", build_arch)) {
        eprintln!("Failed to create {:?}: {}", format!("./out/{}/image/bin", build_arch), e);
        std::process::exit(1);
    }

    println!("Creating ./out/{}/image/.Services", build_arch);
    if let Err(e) = fs::create_dir_all(format!("out/{}/image/.Services", build_arch)) {
        eprintln!("Failed to create {:?}: {}", format!("./out/{}/image/.Services", build_arch), e);
        std::process::exit(1);
    }

    println!("Creating ./out/{}/image/Applications", build_arch);
    if let Err(e) = fs::create_dir_all(format!("out/{}/image/Applications", build_arch)) {
        eprintln!("Failed to create {:?}: {}", format!("./out/{}/image/Applications", build_arch), e);
        std::process::exit(1);
    }
}

fn add_c_projects() -> Vec<Project> {
    let mut c_proj: Vec<Project> = Vec::new();

    // Add C projects here (folder name, then function with any special actions you need done after compilation)
    // Note: Replace Some(fn), fn being your post compilation actions, with None to do nothing after compilation.
    c_proj.push(Project::new("kernel", Some(kernel_pre_actions), Some(kernel_actions), vec![/* Put project names here to make it build after them */]));
    c_proj.push(Project::new("busybox", Some(busybox_pre_actions), Some(busybox_actions), vec!["kernel"]));

    c_proj // return without return
}

fn add_rust_projects() -> Vec<Project> {
    let mut rust_proj: Vec<Project> = Vec::new();

    rust_proj.push(Project::new("init", None, Some(init_actions), vec!["kernel", "busybox"]));

    rust_proj
}