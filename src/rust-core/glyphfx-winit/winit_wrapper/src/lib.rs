use std::os::raw::c_int;
use std::ffi::c_void;
use winit::{
    event::{Event, WindowEvent},
    event_loop::EventLoop,
    window::Window,
};

// Define the type for the callback function
type EventCallback = extern "C" fn(event_type: c_int, event_data: *mut c_void) -> bool;



#[no_mangle]
pub extern "C" fn run_loop(callback: EventCallback) {
    let event_loop = EventLoop::new().unwrap();
    let _window = Window::new(&event_loop).unwrap();

    let mut continue_running = true;
    let _ = event_loop.run(move |event, target| {
        match event {
            Event::WindowEvent { event, .. } => {
                let event_type = match event {
                    WindowEvent::CloseRequested => 0,
                    _ => 1,
                };
                continue_running = callback(event_type, std::ptr::null_mut());
                if !continue_running {
                    target.exit();
                }
            }
            _ => {
                // Other events (not window events)
                //continue_running = call_event_callback(callback, 2, std::ptr::null_mut());
            }
        }
    });
}
