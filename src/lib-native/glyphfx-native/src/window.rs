use winit::event::{Event, WindowEvent};
use winit::event_loop::EventLoop;
use winit::window::Window;

// #[no_mangle]
// pub extern "C" fn run_loop() {
//     let event_loop = EventLoop::new().unwrap();
//     let _window = Window::new(&event_loop).unwrap();
//
//     let _ = event_loop.run(move |event, target| {
//         match event {
//             Event::Resumed => {
//             },
//             Event::WindowEvent { event, .. } => {
//                 match event {
//                     WindowEvent::KeyboardInput { event, .. } => {
//                     },
//                     WindowEvent::MouseInput { state, button, .. } => {
//                     },
//                     WindowEvent::CursorMoved { position, .. } => {
//                     },
//                     WindowEvent::RedrawRequested => {
//                     },
//                     _ => {},
//                 };
//             },
//             _ => {}
//         }
//     });
// }