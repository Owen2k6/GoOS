use std::fs;

pub(crate) fn busybox_pre_actions(_build_arch: &str, _rust_target: &str, _build_debug: bool) -> Result<(), Box<dyn std::error::Error>> {
    println!("Copying ./configs/busybox/busybox.config to ./busybox/.config");
    if let Err(e) = fs::copy("configs/busybox/busybox.config".to_string(), "busybox/.config") {
        eprintln!("Failed to copy ./configs/busybox/busybox.config: {}", e);
        std::process::exit(1);
    }

    Ok(())
}


pub(crate) fn busybox_actions(build_arch: &str, _rust_target: &str, _build_debug: bool) {
    println!("Copying ./busybox/busybox to ./out/{}/image/bin/busybox", build_arch);
    if let Err(e) = fs::copy("busybox/busybox", format!("out/{}/image/bin/busybox", build_arch)) {
        eprintln!("Failed to copy {:?}: ./{}", "./busybox/busybox", e);
        std::process::exit(1);
    }
}