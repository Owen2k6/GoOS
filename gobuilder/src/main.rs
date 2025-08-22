use std::{env, fs, thread};
use std::time::Instant;

fn main() -> std::io::Result<()> {
    // Set up variables
    let mut build_kernel = false;
    let mut build_image = false;
    let mut build_debug = false;
    let mut build_clean = false;
    let mut build_arch = "unknown";
    let cores = thread::available_parallelism().expect("REASON").get();

    // Make default build architecture match host CPU.
    // It will only stay "unknown" if it's not listed here which means, for now, it's unsupported.
    #[cfg(target_arch = "x86")]
    let mut build_arch = "x86";
    #[cfg(target_arch = "x86_64")]
    let mut build_arch = "amd64";
    #[cfg(target_arch = "aarch64")]
    let mut build_arch = "arm64";

    // Command line arguments
    let args: Vec<String> = env::args().collect();

    for string in args {
        match string.as_str() { // Basically a rust "switch" case
            "--compile-kernel" => build_kernel = true, // (re)compiles the kernel
            "--x86" => build_arch = "x86", // Compiles the OS for x86 systems
            "--amd64" => build_arch = "amd64", // Compiles the OS for amd64/x86_64 systems
            "--arm64" => build_arch = "arm64", // Compiles the OS for arm64 systems
            "--all" => build_arch = "all", // Compiles the OS for all supported architectures (useful for releases)
            "--image" => build_image = true, // Creates a bootable ISO after building, like cosmos.
            "--debug" => build_debug = true, // Compiles the OS with debug
            "--clean" => build_clean = true, // Just cleans
            _ => {} // "default"
        }
    }

    println!("Building GoOS for {} using {} threads", build_arch, cores);
    let timer = Instant::now();

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

    if fs::exists("out").unwrap() {
        // Delete out directory
        println!("Deleting ./out");

        if let Err(e) = fs::remove_dir_all("out") {
            eprintln!("Failed to delete {:?}: {}", "./out", e);
        }
    }

    if build_clean {
        let root = "./";

        for entry in fs::read_dir(root).unwrap() {
            let entry = entry.unwrap();
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
    }

    // Remake it if not cleaning
    println!("Creating ./out");
    if let Err(e) = fs::create_dir("out") {
        eprintln!("Failed to create {:?}: {}", "./out", e);
    }

    // ISO Image before being iso-ed
    println!("Creating ./out/image");
    if let Err(e) = fs::create_dir("out/image") {
        eprintln!("Failed to create {:?}: {}", "./out/image", e);
    }


    println!("Done! Took {:?}", timer.elapsed());
    Ok(())
}


