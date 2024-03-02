use futures::io::Window;
use wgpu_wrapper::run;
use winit_wrapper::{get_display_handle, get_window_handle, run_loop};


static mut APP_STATE: *const winit_wrapper::EventLoopState = std::ptr::null();

fn main() {
    run_loop(keyboard_event_callback, init_state_callback, cursor_moved_callback, mouse_input_callback, redraw_requested_callback, close_requested_callback);
}

extern "C" fn init_state_callback(state: *const winit_wrapper::EventLoopState) {
    unsafe {
        //let state = &*state;
        println!("Event loop state");
        APP_STATE = state;

        //exit_target(state);
    }
}

extern "C" fn mouse_input_callback(event_data: winit_wrapper::MouseInputEventData) {
    unsafe {
        let event_data = event_data;
        println!("Mouse input: {:?}", event_data);

        let window_handle = get_window_handle(APP_STATE);
        let display_handle = get_display_handle(APP_STATE);

        let task = run(display_handle, window_handle);
        futures::executor::block_on(task);
    }
}

extern "C" fn cursor_moved_callback(event_data: winit_wrapper::CursorMovedEventData) {
    unsafe {
        //let event_data = event_data;
        //println!("Cursor moved: ({}, {})", event_data.x, event_data.y);
    }
}


extern "C" fn keyboard_event_callback(event_data: winit_wrapper::KeyboardEventData) {
    unsafe {
        let event_data = event_data;
        println!("Keyboard event: {:?}", event_data.key_code);
    }
}

extern "C" fn redraw_requested_callback() {
    println!("Redraw requested");
}

extern "C" fn close_requested_callback() {
    println!("Close requested");
}


