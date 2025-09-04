use std::fs;

pub(crate) fn init_actions(build_arch: &str, rust_target: &str, build_debug: bool) {
    println!("Copying ./init/target/{}/{}/init to ./out/{}/image/boot/init", rust_target, if build_debug { "debug" } else { "release" }, build_arch);
    if let Err(e) = fs::copy(format!("init/target/{}/{}/init", rust_target, if build_debug { "debug" } else { "release" }), format!("out/{}/image/boot/init", build_arch)) {
        eprintln!("Failed to copy {:?}: {}", format!("./init/target/{}/{}/init", rust_target, if build_debug { "debug" } else { "release" }), e);
        std::process::exit(1);
    }
}