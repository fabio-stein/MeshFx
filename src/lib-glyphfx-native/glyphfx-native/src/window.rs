use image::imageops::resize;
use log::info;
use winit::event::{Event, WindowEvent};
use winit::event_loop::EventLoop;
use winit::keyboard::PhysicalKey;
use winit::window::Window;
use crate::bridge;
use crate::bridge::glyphfx_native::*;
use crate::bridge::handle_native;
use crate::graphics::renderer::{get_global_window, resize_renderer};

pub fn run_main_loop(request: RunMainLoopRequest) -> RunMainLoopResponse {
    info!("Received request for main loop: {:?}", request);
    run_loop();
    RunMainLoopResponse {}
}

fn run_loop() {
    let event_loop = EventLoop::new().unwrap();
    #[allow(unused_mut)]
    let mut builder = winit::window::WindowBuilder::new()
        .with_title("GlyphFX")
        //.with_maximized(true)
        ;

    #[cfg(target_arch = "wasm32")]
    {
        use wasm_bindgen::JsCast;
        use winit::platform::web::WindowBuilderExtWebSys;

        let canvas = web_sys::window()
            .unwrap()
            .document()
            .unwrap()
            .get_element_by_id("canvas")
            .unwrap()
            .dyn_into::<web_sys::HtmlCanvasElement>()
            .unwrap();
        builder = builder.with_canvas(Some(canvas));
    }

    let window = builder.build(&event_loop).unwrap();

    #[cfg(not(target_arch = "wasm32"))]
    {
        pollster::block_on(run(event_loop, window));
    }
    #[cfg(target_arch = "wasm32")]
    {
        wasm_bindgen_futures::spawn_local(run(event_loop, window));
    }
}

pub static mut GLOBAL_WINDOW: Option<Window> = None;

async fn run(event_loop: EventLoop<()>, window: Window) {
    unsafe {
        GLOBAL_WINDOW = Some(window);
    }

    event_loop.run(move |event, _target| {
        match event {
            Event::Resumed => {
                handle_native::<WindowResumeEventRequest, WindowResumeEventResponse>(NativeRequestCode::WindowEventResume, WindowResumeEventRequest {});
            },
            Event::WindowEvent { event, .. } => {
                match event {
                    WindowEvent::KeyboardInput { event, .. } => {
                        if let Some(code) = KeyCode::from_winit_event(event.physical_key) {
                            let request = WindowKeyboardEventRequest {
                                key_code: code.into(),
                                is_pressed: event.state == winit::event::ElementState::Pressed,
                            };
                            handle_native::<WindowKeyboardEventRequest, WindowKeyboardEventResponse>(NativeRequestCode::WindowKeyboardEvent, request);
                        }
                    },
                    WindowEvent::MouseInput { state: _state, button: _button, .. } => {
                    },
                    WindowEvent::CursorMoved { position: _position, .. } => {
                    },
                    WindowEvent::Resized(_size) => {
                        info!("Window resized: {:?}", _size);
                        resize_renderer(_size.width, _size.height);
                    },
                    WindowEvent::RedrawRequested => {
                        handle_native::<WindowRedrawRequest, WindowRedrawResponse>(NativeRequestCode::WindowRedraw, WindowRedrawRequest {});
                        get_global_window().unwrap().request_redraw();
                    },
                    _ => {},
                };
            },
            _ => {}
        }
    }).unwrap();
}