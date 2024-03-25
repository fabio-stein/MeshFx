use std::panic;
use std::sync::{Arc, Mutex};
use log::info;
use winit::event::{Event, WindowEvent};
use winit::event_loop::EventLoop;
use winit::keyboard::{KeyCode, PhysicalKey};
use winit::window::Window;
use crate::bridge::glyphfx_native::*;
use crate::graphics::renderer::init_renderer;

pub fn run_main_loop(request: RunMainLoopRequest) -> RunMainLoopResponse {
    info!("Received request for main loop: {:?}", request);
    run_loop();
    RunMainLoopResponse {}
}

fn run_loop() {
    let event_loop = EventLoop::new().unwrap();
    let mut builder = winit::window::WindowBuilder::new();

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

    event_loop.run(move |event, target| {
        match event {
            Event::Resumed => {
                init_renderer();
            },
            Event::WindowEvent { event, .. } => {
                match event {
                    WindowEvent::KeyboardInput { event, .. } => {
                        info!("Key pressed: {:?}", event.physical_key);
                        //init_renderer();

                        let keycode_str = format!("{:?}", event.physical_key);

                        let request = AppEventRequest {
                            name: keycode_str,
                        };
                        crate::bridge::handle_native::<AppEventRequest, AppEventResponse>(NativeRequestCode::AppEvent, request);
                    },
                    WindowEvent::MouseInput { state, button, .. } => {
                    },
                    WindowEvent::CursorMoved { position, .. } => {
                    },
                    WindowEvent::RedrawRequested => {
                    },
                    _ => {},
                };
            },
            _ => {}
        }
    }).unwrap();
}