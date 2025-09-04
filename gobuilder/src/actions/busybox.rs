use std::fs;

pub(crate) fn busybox_actions(build_arch: &str, _rust_target: &str, _build_debug: bool) {
    println!("Copying ./busybox/busybox to ./out/{}/image/bin/busybox", build_arch);
    if let Err(e) = fs::copy("busybox/busybox", format!("out/{}/image/bin/busybox", build_arch)) {
        eprintln!("Failed to copy {:?}: ./{}", "./busybox/busybox", e);
        std::process::exit(1);
    }
}