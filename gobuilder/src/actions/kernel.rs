use std::fs;

pub(crate) fn kernel_actions(build_arch: &str, _build_debug: bool) {
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