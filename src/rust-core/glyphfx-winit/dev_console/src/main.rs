use std::env;
use winit_wrapper::{CursorMovedEventData, get_display_handle, get_window_handle, run_loop};
use wgpu_wrapper::model::*;

static mut APP_STATE: *const winit_wrapper::EventLoopState = std::ptr::null();
static mut WGPU_STATE: *const wgpu_wrapper::State = std::ptr::null();

fn main() {
    let file = ObjFile::load("cube/cube.obj").unwrap();
    //simple debug data:
    println!("models: {:?}", file);
    //run_loop(keyboard_event_callback, init_state_callback, cursor_moved_callback, mouse_input_callback, redraw_requested_callback, close_requested_callback);
}

extern "C" fn init_state_callback(state: *const winit_wrapper::EventLoopState) {
    unsafe {
        //let state = &*state;
        println!("Event loop state");
        APP_STATE = state;

        let window_handle = get_window_handle(APP_STATE);
        let display_handle = get_display_handle(APP_STATE);

        let display_handle_ref = unsafe { &*display_handle };
        let window_handle_ref = unsafe { &*window_handle };

        let state_ptr = wgpu_wrapper::init_state(*display_handle_ref, *window_handle_ref);
        WGPU_STATE = state_ptr;
        //exit_target(state);
    }
}

static mut INDICES: &[u16] = &[
    0, 1, 2,
    0, 2, 3,
];

static mut CURSOR:CursorMovedEventData = CursorMovedEventData { x: 0.0, y: 0.0 };

extern "C" fn mouse_input_callback(event_data: winit_wrapper::MouseInputEventData) {
    unsafe {
        let event_data = event_data;
        println!("Mouse input: {:?}", event_data);

        if event_data.is_down == 0{
            return;
        }

        let state_ref = unsafe { &*WGPU_STATE };

        //wgpu_wrapper::render(state_ref, &VERTICES, &INDICES);
    }
}

extern "C" fn cursor_moved_callback(event_data: winit_wrapper::CursorMovedEventData) {
    unsafe {
        //let event_data = event_data;
        //println!("Cursor moved: ({}, {})", event_data.x, event_data.y);
        CURSOR = event_data;
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


