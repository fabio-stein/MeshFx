use winit::{
    event::{Event, WindowEvent},
    event_loop::EventLoop,
    window::Window,
};
use winit::event_loop::EventLoopWindowTarget;
use winit::keyboard::PhysicalKey;

#[repr(C)]
pub struct KeyboardEventData {
    pub event_type: i32,
    pub key_code: i32,
}
type KeyboardEventCallback = extern "C" fn(KeyboardEventData);

#[repr(C)]
pub struct EventLoopState {
    target: Option<*const EventLoopWindowTarget<()>>,
    window: Option<*const Window>,
}

type InitStateCallback = extern "C" fn(*const EventLoopState);

#[repr(C)]
#[derive(Debug)]
pub struct CursorMovedEventData {
    pub x: f32,
    pub y: f32,
}
type CursorMovedCallback = extern "C" fn(CursorMovedEventData);

#[repr(C)]
#[derive(Debug)]
pub struct MouseInputEventData {
    pub is_down: i32,
    pub button: i32,
}


#[derive(Debug)]
pub enum MouseButton {
    Left,
    Right,
    Middle,
}

type MouseInputCallback = extern "C" fn(MouseInputEventData);

type RedrawRequestedCallback = extern "C" fn();
type CloseRequestedCallback = extern "C" fn();

#[no_mangle]
pub extern "C" fn run_loop(keyboard_callback: KeyboardEventCallback, init_state: InitStateCallback, cursor_moved_callback: CursorMovedCallback, mouse_input_callback: MouseInputCallback, redraw_requested_callback: RedrawRequestedCallback, close_requested_callback: CloseRequestedCallback) {
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
                    WindowEvent::CloseRequested => {
                        close_requested_callback();
                    },
                    WindowEvent::KeyboardInput { event, .. } => {
                        let keycode_int = match event.physical_key {
                            PhysicalKey::Code(keycode) => keycode as i32,
                            PhysicalKey::Unidentified(_) => -1,
                        };
                        let event_data = KeyboardEventData {
                            event_type: 1,
                            key_code: keycode_int,
                        };

                        keyboard_callback(event_data);
                    },
                    WindowEvent::MouseInput { state, button, .. } => {
                        let button_code = match button {
                            winit::event::MouseButton::Left => MouseButton::Left,
                            winit::event::MouseButton::Right => MouseButton::Right,
                            winit::event::MouseButton::Middle => MouseButton::Middle,
                            _ => return,
                        };
                        let event_data = MouseInputEventData {
                            is_down: (state == winit::event::ElementState::Pressed) as i32,
                            button: button_code as i32,
                        };
                        mouse_input_callback(event_data);
                    },
                    WindowEvent::CursorMoved { position, .. } => {
                        let position = CursorMovedEventData {
                            x: position.x as f32,
                            y: position.y as f32,
                        };
                        cursor_moved_callback(position);
                    },
                    WindowEvent::RedrawRequested => {
                        redraw_requested_callback();
                    },
                    _ => {},
                };
            },
            Event::Resumed => {
                let target_ptr = target as *const _;
                let window_ptr = &_window as *const _;
                state.target = Some(target_ptr);
                state.window = Some(window_ptr);
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

#[no_mangle]
pub extern "C" fn request_redraw(state: *const EventLoopState) {
    unsafe {
        if let Some(window_ptr) = (*state).window {
            let window = &*window_ptr; // Convert back to reference
            window.request_redraw();
        }
    }
}

#[no_mangle]
pub extern "C" fn get_window(state: *const EventLoopState) -> *const Window {
    unsafe {
        if let Some(window_ptr) = (*state).window {
            window_ptr
        } else {
            std::ptr::null()
        }
    }
}