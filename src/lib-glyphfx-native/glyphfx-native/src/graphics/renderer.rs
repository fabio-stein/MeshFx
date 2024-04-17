use std::collections::HashMap;
use std::ptr::null;
use log::info;
use wgpu::RenderPass;
use winit::window::Window;
use crate::graphics::{leg_renderer, material, model};
use crate::window;
use crate::bridge::glyphfx_native::*;
use crate::bridge::handle_native;
use crate::graphics::material::load_texture;

static mut GLOBAL_STATE: Option<leg_renderer::State> = None;
static mut GLOBAL_RENDERPASS: Option<&mut RenderPass> = None;
static mut GLOBAL_MESHES: Option<HashMap<u32, model::Mesh>> = None;
static mut GLOBAL_MATERIALS: Option<HashMap<u32, material::Material>> = None;

pub fn init_renderer(_request: InitRendererRequest) -> InitRendererResponse {
    unsafe { GLOBAL_MESHES = Some(HashMap::new()) }
    unsafe { GLOBAL_MATERIALS = Some(HashMap::new()) }
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
    let indices = mesh.num_indices;
    unsafe {
        GLOBAL_MESHES.as_mut().unwrap().insert(request.mesh_id, mesh);
    }
    info!("Loaded mesh indices: {}", indices);
    LoadMeshResponse {}
}

pub fn load_material(request: LoadMaterialRequest) -> LoadMaterialResponse {
    info!("Received request to load material");
    let state = unsafe { GLOBAL_STATE.as_ref().unwrap() };
    let material = load_texture(state, request.texture_data);
    unsafe {
        GLOBAL_MATERIALS.as_mut().unwrap().insert(request.material_id, material);
    }
    LoadMaterialResponse {}
}

pub fn resize_renderer(width: u32, height: u32) {
    let state = unsafe { GLOBAL_STATE.as_mut() };
    if state.is_none() {
        return;
    }
    leg_renderer::resize(state.unwrap(), width, height);

}

pub fn begin_render(request: BeginRenderRequest) -> BeginRenderResponse {
    let state = unsafe { GLOBAL_STATE.as_ref().unwrap() };
    leg_renderer::render(state, |rpass|{
        unsafe {
            GLOBAL_RENDERPASS = Some(rpass);
        }
        handle_native::<RenderWaitingRequest, RenderWaitingResponse>(NativeRequestCode::RenderWaiting, RenderWaitingRequest {});
        unsafe {
            GLOBAL_RENDERPASS = None;
        }
    }, request.instance_buffer);
    BeginRenderResponse {}
}

pub fn render_draw(request: RenderDrawRequest) -> RenderDrawResponse {
    let state = unsafe { GLOBAL_STATE.as_ref().unwrap() };
    let rpass = unsafe { GLOBAL_RENDERPASS.as_mut().unwrap() };
    let mesh = unsafe { GLOBAL_MESHES.as_ref().unwrap().get(&request.mesh_id).unwrap() };
    let material = unsafe { GLOBAL_MATERIALS.as_ref().unwrap().get(&request.material_id).unwrap() };
    leg_renderer::draw(state, rpass, request.camera_view_projection, request.instance_count, mesh, material);
    RenderDrawResponse {}
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
