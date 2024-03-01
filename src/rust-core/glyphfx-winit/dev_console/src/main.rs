use winit_wrapper::{run_loop};




fn main() {
    run_loop(event_callback);
}

extern "C" fn event_callback(event_type: i32, _event_data: *mut std::ffi::c_void) -> bool {
    match event_type {
        0 => {
            println!("Close requested");
            false
        }
        1 => {
            println!("Other window event");
            true
        }
        2 => {
            println!("Other event");
            true
        }
        _ => {
            println!("Unknown event");
            true
        }
    }
}

