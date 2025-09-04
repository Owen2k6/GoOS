use std::ffi::CString;
use std::os::unix::process::CommandExt;
use std::path::PathBuf;
use std::ptr;

fn main() -> Result<(), Box<dyn std::error::Error>> {
    print!("\x1B[2J\x1b[H"); // Clear screen and reset caret pos

    unsafe { init_mount(); } // Mount the essential "drives"

    println!("Welcome to\x1B[36m GoOS!\x1B[39;49m"); // Welcome message!

    // If .Services is empty...
    let is_empty = PathBuf::from("/.Services").read_dir().map(|mut i| i.next().is_none()).unwrap_or(false);

    // ... run the emergency shell, there are no services which means nothing will happen otherwise.
    if is_empty {
        let _ = std::process::Command::new("/bin/busybox")
            .args(&["sh"])
            .status()?;

        println!("Shutting Down...");
        unsafe { libc::reboot(libc::RB_POWER_OFF); }
    }

    loop {} // Infinite loop (wow)
}

unsafe fn init_mount() {
    mount_fs("proc", "/proc", "proc", 0);
    mount_fs("sysfs", "/sys", "sysfs", 0);
    mount_fs("devtmpfs", "/dev", "devtmpfs", 0);
    mount_fs("tmpfs", "/tmp", "tmpfs", 0);
}

fn mount_fs(src: &str, target: &str, fstype: &str, flags: libc::c_ulong) -> i32 {
    let src = CString::new(src).unwrap();
    let target = CString::new(target).unwrap();
    let fstype = CString::new(fstype).unwrap();
    unsafe { libc::mount(src.as_ptr(), target.as_ptr(), fstype.as_ptr(), flags, ptr::null()) }
}
