use std::fs;
use std::process::Command;

pub(crate) fn kernel_pre_actions(build_arch: &str, _rust_target: &str, _build_debug: bool) -> Result<(), Box<dyn std::error::Error>> {
    if !std::path::Path::new("configs/kernel/aarch64.config").exists() {
        println!("Fetching aarch64.config...");
        let status = Command::new("curl")
            .args(&["-o", "configs/kernel/aarch64.config", "https://salsa.debian.org/kernel-team/linux/-/raw/debian/latest/debian/config/arm64/config"])
            .status()?;
        if !status.success() {
            eprintln!("Failed to fetch aarch64.config!");
            std::process::exit(1);
        }
    }

    if !std::path::Path::new("configs/kernel/i386.config").exists() {
        println!("Fetching i386.config...");
        let status = Command::new("curl")
            .args(&["-o", "configs/kernel/i386.config", "https://salsa.debian.org/kernel-team/linux/-/raw/debian/latest/debian/config/config"])
            .status()?;
        if !status.success() {
            eprintln!("Failed to fetch i386.config!");
            std::process::exit(1);
        }
    }

    if !std::path::Path::new("configs/kernel/x86_64.config").exists() {
        println!("Fetching x86_64.config...");
        let status = Command::new("curl")
            .args(&["-o", "configs/kernel/x86_64.config", "https://salsa.debian.org/kernel-team/linux/-/raw/debian/latest/debian/config/amd64/config"])
            .status()?;
        if !status.success() {
            eprintln!("Failed to fetch x86_64.config!");
            std::process::exit(1);
        }
    }

    if !std::path::Path::new("configs/kernel/powerpc.config").exists() {
        println!("Fetching powerpc.config...");
        let status = Command::new("curl")
            .args(&["-o", "configs/kernel/powerpc.config", "https://salsa.debian.org/kernel-team/linux/-/raw/debian/latest/debian/config/powerpc/config.powerpc"])
            .status()?;
        if !status.success() {
            eprintln!("Failed to fetch powerpc.config!");
            std::process::exit(1);
        }
    }

    println!("Copying ./configs/kernel/{}.config to ./kernel/.config", build_arch);
    if let Err(e) = fs::copy(format!("configs/kernel/{}.config", build_arch), "kernel/.config") {
        eprintln!("Failed to copy {:?}: {}", format!("./configs/kernel/{}.config", build_arch), e);
        std::process::exit(1);
    }

    println!("Applying config overrides...");
    let status = Command::new("sh")
        .args(&[
            "scripts/kconfig/merge_config.sh",
            "-m",
            ".config",
            "../configs/kernel/overrides.config"
        ])
        .current_dir("kernel")
        .status()?;

    if !status.success() {
        eprintln!("Failed to apply config overrides!");
        std::process::exit(1);
    }

    println!("Running make olddefconfig...");
    let status = Command::new("make")
        .args(&["olddefconfig"])
        .current_dir("kernel")
        .status()?;

    if !status.success() {
        eprintln!("Failed to run olddefconfig!");
        std::process::exit(1);
    }

    Ok(())
}

pub(crate) fn kernel_actions(build_arch: &str, _rust_target: &str, _build_debug: bool) {
    println!("Creating ./out/{}/image/boot", build_arch);
    if let Err(e) = fs::create_dir_all(format!("out/{}/image/boot", build_arch)) {
        eprintln!("Failed to create {:?}: {}", format!("./out/{}/image/boot", build_arch), e);
        std::process::exit(1);
    }

    let kernel_location = match build_arch {
        "x86_64" | "i386" => "kernel/arch/x86/boot/bzImage",
        "aarch64" => "kernel/arch/arm64/boot/Image",
        _ => "architecture_not_supported"// handle other cases
    };

    println!("Copying ./{} to ./out/{}/image/boot/{}", kernel_location, build_arch, if build_arch == "arm64" { "Image" } else { "vmlinuz" });
    if let Err(e) = fs::copy(kernel_location, format!("out/{}/image/boot/{}", build_arch, if build_arch == "arm64" { "Image" } else { "vmlinuz" })) {
        eprintln!("Failed to copy {:?}: ./{}", kernel_location, e);
        std::process::exit(1);
    }

    println!("Creating ./out/{}/image/boot/grub", build_arch);
    if let Err(e) = fs::create_dir_all(format!("out/{}/image/boot/grub", build_arch)) {
        eprintln!("Failed to create {:?}: {}", format!("./out/{}/image/boot/grub", build_arch), e);
        std::process::exit(1);
    }

    let grub_cfg_location = match build_arch {
        "x86_64" | "i386" => "configs/grub/x86_grub.cfg",
        "arm64" => "configs/grub/arm_grub.cfg",
        _ => "architecture_not_supported"// handle other cases
    };

    println!("Copying ./{} to ./out/{}/image/boot/grub/grub.cfg", grub_cfg_location, build_arch);
    if let Err(e) = fs::copy(grub_cfg_location, format!("out/{}/image/boot/grub/grub.cfg", build_arch)) {
        eprintln!("Failed to copy {:?}: ./{}", grub_cfg_location, e);
        std::process::exit(1);
    }
}