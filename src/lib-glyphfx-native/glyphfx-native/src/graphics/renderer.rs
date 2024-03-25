use log::info;
use winit::window::Window;
use crate::graphics::leg_renderer;
use crate::window;
use crate::bridge::glyphfx_native::*;
use crate::bridge::handle_native;

pub fn init_renderer(request: InitRendererRequest) -> InitRendererResponse {
    #[cfg(target_arch = "wasm32")]
    {
        wasm_bindgen_futures::spawn_local(init_renderer_async());
    }
    #[cfg(not(target_arch = "wasm32"))]
    {
        pollster::block_on(init_renderer_async());
    }
    InitRendererResponse {}
}

pub fn load_mesh(request: LoadMeshRequest) -> LoadMeshResponse {
    info!("Received request to load mesh");
    LoadMeshResponse {}
}

async fn init_renderer_async(){
    leg_renderer::init_async(get_global_window().unwrap()).await;
    handle_native::<RendererReadyRequest, RendererReadyResponse>(NativeRequestCode::RendererReady, RendererReadyRequest {});
}

pub fn get_global_window() -> Option<&'static Window> {
    unsafe {
        window::GLOBAL_WINDOW.as_ref()
    }
}
