use log::info;
use winit::window::Window;
use crate::graphics::{leg_renderer, model};
use crate::window;
use crate::bridge::glyphfx_native::*;
use crate::bridge::handle_native;
use crate::graphics::material::load_texture;

static mut GLOBAL_STATE: Option<leg_renderer::State> = None;

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
    let state = unsafe { GLOBAL_STATE.as_ref().unwrap() };
    let mesh = model::load_mesh(state, request.vertices, request.indices.to_vec());
    info!("Loaded mesh indices: {}", mesh.num_indices);
    LoadMeshResponse {}
}

pub fn load_material(request: LoadMaterialRequest) -> LoadMaterialResponse {
    info!("Received request to load material");
    let state = unsafe { GLOBAL_STATE.as_ref().unwrap() };
    let material = load_texture(state, request.texture_data);
    LoadMaterialResponse {}
}

async fn init_renderer_async(){
    let state = leg_renderer::init_async(get_global_window().unwrap()).await;
    unsafe {
        GLOBAL_STATE = Some(state);
    }
    handle_native::<RendererReadyRequest, RendererReadyResponse>(NativeRequestCode::RendererReady, RendererReadyRequest {});
}

pub fn get_global_window() -> Option<&'static Window> {
    unsafe {
        window::GLOBAL_WINDOW.as_ref()
    }
}
