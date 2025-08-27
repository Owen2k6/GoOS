#[cfg(any(target_arch = "x86", target_arch = "x86_64", target_arch = "arm"))]
const SYS_SYSLOG: libc::c_long = 103;

#[cfg(any(target_arch = "powerpc", target_arch = "powerpc64", target_arch = "aarch64"))]
const SYS_SYSLOG: libc::c_long = 116;

fn main() {
    unsafe {
        libc::syscall(SYS_SYSLOG, 8, 1 as *const u8, 1); // Disable kernel logs (FUCK OFF KERNEL)
    }

    print!("\x1B[2J\x1b[H");
    println!("Welcome to\x1B[36m GoOS!\x1B[39;49m");
    loop {}
}
