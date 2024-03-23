use wasm_bindgen::{JsCast, JsValue};
use wasm_bindgen::prelude::wasm_bindgen;
use web_sys::js_sys::Function;
use web_sys::window;
use wgpu::RenderPass;
use winit::event::{Event, WindowEvent};
use winit::event_loop::EventLoop;
use winit::keyboard::PhysicalKey;
use winit::platform::web::WindowBuilderExtWebSys;
use winit::raw_window_handle::{HasDisplayHandle, HasWindowHandle, RawDisplayHandle, RawWindowHandle};
use winit::window::Window;

#[wasm_bindgen(js_namespace = netapp)]
extern "C" {
    fn SimpleLog();
    fn RenderNet();
}

#[wasm_bindgen]
pub fn simple_log() {
    SimpleLog();
}

// #[wasm_bindgen]
// pub fn test_callback(callback: &Function)
// {
//     web_sys::console::log_1(&"USING CALLBACK".to_string().into());
//     callback.call1(&JsValue::NULL, &JsValue::from_str("Hello callback!")).unwrap();
//     web_sys::console::log_1(&"FINISHED CALLBACK".to_string().into());
//
// }

#[wasm_bindgen(start)]
fn main() -> Result<(), JsValue> {
    web_sys::console::log_1(&"Hello from Rust!".into());
    Ok(())
}

#[repr(C)]
pub struct DemoData{
    pub val1: i32,
    pub val2: i32,
}

#[wasm_bindgen]
pub fn simsum(a: i32, b: i32) -> i32 {
    let data = DemoData {
        val1: a,
        val2: b,
    };
    let ptr_box = Box::new(data);
    let ptr = Box::into_raw(ptr_box);
    ptr as i32
}

#[wasm_bindgen]
pub fn show_sum(ptr: i32) -> i32 {
    let data = unsafe { &*(ptr as *const DemoData) };
    data.val1 + data.val2
}

#[wasm_bindgen]
pub fn simple_string() -> String {
    "Hello from Rust!".to_string()
}

static mut WGPU_STATE: Option<*mut wgpu_wrapper::State> = None;
static mut WGPU_MATERIAL: Option<*mut wgpu_wrapper::material::Material> = None;
static mut WGPU_MESH: Option<*mut wgpu_wrapper::model::Mesh> = None;
static mut WGPU_RENDERPASS: Option<*mut wgpu::RenderPass> = None;

#[wasm_bindgen]
pub fn start_winit() {
    let document = web_sys::window().unwrap().document().unwrap();
    let canvas = document.get_element_by_id("canvas").unwrap();
    let canvas = canvas
        .dyn_into::<web_sys::HtmlCanvasElement>()
        .map_err(|_| ())
        .unwrap();

    let event_loop = winit::event_loop::EventLoop::new().unwrap();
    let window = winit::window::WindowBuilder::new()
        .with_canvas(Some(canvas))
        .build(&event_loop).unwrap();

    wasm_bindgen_futures::spawn_local(run_loop(event_loop, window));
}

async fn run_loop(event_loop: EventLoop<()>, window: Window){
    let display_handle = window.display_handle().unwrap().as_raw();
    let window_handle = window.window_handle().unwrap().as_raw();

    // let display_handle_ptr = Box::into_raw(Box::new(display_handle));
    // let window_handle_ptr = Box::into_raw(Box::new(window_handle));
    // web_sys::console::log_1(&format!("Display Handle: {}", display_handle_ptr as i32).into());
    // web_sys::console::log_1(&format!("Window Handle: {}", window_handle_ptr as i32).into());

    unsafe {
        WGPU_STATE = Some(wgpu_wrapper::init_async(display_handle, window_handle).await);
    }


    let _ = event_loop.run(move |event, target| {
        match event {
            Event::WindowEvent { event, .. } => {
                match event {
                    WindowEvent::CloseRequested => {
                    },
                    WindowEvent::KeyboardInput { event, .. } => {
                        let keycode_int = match event.physical_key {
                            PhysicalKey::Code(keycode) => keycode as i32,
                            PhysicalKey::Unidentified(_) => -1,
                        };

                        web_sys::console::log_1(&format!("Keycode: {}", keycode_int).into());

                        let state = unsafe{ &mut *WGPU_STATE.unwrap() };
                        wgpu_wrapper::render(state, render_callback);
                    },
                    WindowEvent::MouseInput { state, button, .. } => {

                    },
                    WindowEvent::CursorMoved { position, .. } => {

                    },
                    WindowEvent::RedrawRequested => {
                        let state = unsafe{ &mut *WGPU_STATE.unwrap() };
                        wgpu_wrapper::render(state, render_callback);

                        window.request_redraw();
                    },
                    _ => {},
                };
            },
            Event::Resumed => {
                web_sys::console::log_1(&format!("Resumed").into());
                //
                // let callback:wgpu_wrapper::RenderCallback = |d| {
                //     web_sys::console::log_1(&format!("Render Callback RUN!").into());
                // };
            },
            _ => {}
        }
    });
}

#[no_mangle]
pub extern "C" fn render_callback(rpass: *const RenderPass) {
    web_sys::console::log_1(&"Render Callback!".into());
    let state = unsafe{ &mut *WGPU_STATE.unwrap() };
    //cast rpass to &mut RenderPass
    let rpass = unsafe { &mut *(rpass as *mut RenderPass) };

    unsafe {
        WGPU_RENDERPASS = Some(rpass);
    }
    RenderNet();
}

#[wasm_bindgen]
pub fn rust_draw(camera_uniform: &[u8], instance_uniform: &[u8]) {
    web_sys::console::log_1(&"Draw Rust!".into());
    let state = unsafe{ &mut *WGPU_STATE.unwrap() };
    let material = unsafe{ &mut *WGPU_MATERIAL.unwrap() };
    let mesh = unsafe{ &mut *WGPU_MESH.unwrap() };
    let rpass = unsafe{ &mut *WGPU_RENDERPASS.unwrap() };
    let camera_uniform = camera_uniform.as_ptr() as *const f32;
    let instance_uniform = instance_uniform.as_ptr() as *const f32;

    //log everything

    wgpu_wrapper::draw(state, rpass, camera_uniform, instance_uniform, mesh, material);
}

#[wasm_bindgen]
pub fn renderrust(data: &[u8]) {
    web_sys::console::log_1(&"Render Rust!".into());
    //log data length
    web_sys::console::log_1(&format!("Data Length: {}", data.len()).into());
}

#[wasm_bindgen]
pub fn rust_load_texture(data: &[u8]) {
    web_sys::console::log_1(&"LoadTexture Rust!".into());

    //map data to *const u8
    let texture_ptr = data.as_ptr();
    let state = unsafe{ &mut *WGPU_STATE.unwrap() };

    let material = wgpu_wrapper::material::load_texture(state, texture_ptr, data.len() as u32);
    unsafe {
        WGPU_MATERIAL = Some(material);
    }
    web_sys::console::log_1(&format!("Texture loaded, Data Length: {}", data.len()).into());
}

#[wasm_bindgen]
pub fn rust_load_mesh(vertex_data: &[u8], index_data: &[u8]) {
    web_sys::console::log_1(&"LoadMesh Rust!".into());

    let vertex_count = vertex_data.len() / 32;
    let index_count = index_data.len() / 4;

    let vertex_ptr = vertex_data.as_ptr() as *mut std::ffi::c_void;
    let index_ptr = index_data.as_ptr() as *const u32;
    let state = unsafe{ &mut *WGPU_STATE.unwrap() };

    let mesh = wgpu_wrapper::model::load_mesh(state, vertex_ptr, vertex_count as u32, index_ptr, index_count as u32);
    unsafe {
        WGPU_MESH = Some(mesh);
    }
    web_sys::console::log_1(&format!("Mesh loaded, Data Length: {}", vertex_data.len()).into());
}


// #[wasm_bindgen]
// pub fn start_wgpu (display_handle: *const RawDisplayHandle, window_handle: *const RawWindowHandle) {
//     web_sys::console::log_1(&format!("Display Handle: {}", display_handle as i32).into());
//     let display_handle = unsafe { &*display_handle };
//     let window_handle = unsafe { &*window_handle };
//
//     let state = wgpu_wrapper::init_state(*display_handle, *window_handle);
//
//     web_sys::console::log_1(&format!("State: {}", state as i32).into());
// }