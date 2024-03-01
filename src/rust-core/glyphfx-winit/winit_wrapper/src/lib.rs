use std::os::raw::c_int;
use std::ffi::c_void;
use std::os::macos::raw::stat;
use winit::{
    event::{Event, WindowEvent},
    event_loop::EventLoop,
    window::Window,
};
use winit::event_loop::EventLoopWindowTarget;
use winit::keyboard::{KeyCode, PhysicalKey};

// Define the type for the callback function
type EventCallback = extern "C" fn(event_type: c_int, event_data: *mut c_void) -> bool;

#[repr(C)]
pub struct KeyboardEventData {
    pub event_type: i32,
    pub key_code: i32,
}
type KeyboardEventCallback = extern "C" fn(*const KeyboardEventData);

#[repr(C)]
pub struct EventLoopState {
    target: Option<*const EventLoopWindowTarget<()>>,
}

type InitStateCallback = extern "C" fn(*const EventLoopState);

#[no_mangle]
pub extern "C" fn run_loop(keyboard_callback: KeyboardEventCallback, init_state: InitStateCallback) {
    let event_loop = EventLoop::new().unwrap();
    let _window = Window::new(&event_loop).unwrap();

    let mut state = EventLoopState {
        target: None,
        window: None,
    };

    let _ = event_loop.run(move |event, target| {
        match event {
            Event::WindowEvent { event, .. } => {
                match event {
                    WindowEvent::CloseRequested => {},
                    WindowEvent::KeyboardInput { event, .. } => {
                        let keycode_int = match event.physical_key {
                            PhysicalKey::Code(keycode) => keycode as i32,
                            PhysicalKey::Unidentified(_) => -1,
                        };
                        let event_data = KeyboardEventData {
                            event_type: 1,
                            key_code: keycode_int,
                        };

                        keyboard_callback(&event_data);
                    },
                    WindowEvent::RedrawRequested => {},
                    _ => {},
                };
            },
            Event::Resumed => {
                let target_ptr = target as *const _;
                state.target = Some(target_ptr);
                init_state(&state);
            },
            _ => {}
        }
    });
}

#[no_mangle]
pub extern "C" fn exit_target(state: *const EventLoopState) {
    unsafe {
        if let Some(target_ptr) = (*state).target {
            let target = &*target_ptr; // Convert back to reference
            target.exit();
        }
    }
}