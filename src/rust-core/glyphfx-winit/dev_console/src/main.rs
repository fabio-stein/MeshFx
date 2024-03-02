use winit_wrapper::{exit_target, run_loop};




fn main() {
    run_loop(keyboard_event_callback, init_state_callback, cursor_moved_callback, mouse_input_callback, redraw_requested_callback, close_requested_callback);
}

extern "C" fn init_state_callback(state: *const winit_wrapper::EventLoopState) {
    unsafe {
        let state = &*state;
        println!("Event loop state");

        //exit_target(state);
    }
}

extern "C" fn mouse_input_callback(event_data: winit_wrapper::MouseInputEventData) {
    unsafe {
        let event_data = event_data;
        println!("Mouse input: {:?}", event_data);
    }
}

extern "C" fn cursor_moved_callback(event_data: winit_wrapper::CursorMovedEventData) {
    unsafe {
        let event_data = event_data;
        //println!("Cursor moved: ({}, {})", event_data.x, event_data.y);
    }
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
            //print num
            println!("Unknown event type: {}", event_type);
            true
        }
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


