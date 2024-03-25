use log::info;
use winit::window::Window;
use crate::graphics::leg_renderer;
use crate::window;

pub fn init_renderer() {
    #[cfg(target_arch = "wasm32")]
    {
        wasm_bindgen_futures::spawn_local(init_renderer_async());
    }
    #[cfg(not(target_arch = "wasm32"))]
    {
        pollster::block_on(init_renderer_async());
    }
}

async fn init_renderer_async(){
    leg_renderer::init_async(get_global_window().unwrap()).await;
}

pub fn get_global_window() -> Option<&'static Window> {
    unsafe {
        window::GLOBAL_WINDOW.as_ref()
    }
}
