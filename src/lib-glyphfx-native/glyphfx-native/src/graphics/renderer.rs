use log::info;
use winit::window::Window;
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
    let instance = wgpu::Instance::default();
    let surface = instance.create_surface(get_global_window().unwrap()).unwrap();
    let adapter = instance.request_adapter(&wgpu::RequestAdapterOptions {
        power_preference: wgpu::PowerPreference::default(),
        force_fallback_adapter: false,
        compatible_surface: Some(&surface),
    }).await.unwrap();

    info!("Adapter: {:?}", adapter.get_info());

}

pub fn get_global_window() -> Option<&'static Window> {
    unsafe {
        window::GLOBAL_WINDOW.as_ref()
    }
}
