use winit::event::{Event, WindowEvent};
use winit::event_loop::EventLoop;
use winit::window::Window;
use crate::bridge::glyphfx_native::*;

pub fn run_main_loop(request: RunMainLoopRequest) -> RunMainLoopResponse {
    println!("Received request for main loop: {:?}", request);
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

    let _window = builder.build(&event_loop).unwrap();

    let _ = event_loop.run(move |event, target| {
        match event {
            Event::Resumed => {
            },
            Event::WindowEvent { event, .. } => {
                match event {
                    WindowEvent::KeyboardInput { event, .. } => {
                        println!("Key pressed: {:?}", event.physical_key);

                        #[cfg(target_arch = "wasm32")]{
                            web_sys::console::log_1(&format!("Key pressed: {:?}", event.physical_key).into());
                        }

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
    });
}