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
static mut GLOBAL_MESH: Option<model::Mesh> = None;
static mut GLOBAL_MATERIAL: Option<material::Material> = None;

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
    let indices = mesh.num_indices;
    unsafe {
        GLOBAL_MESH = Some(mesh);
    }
    info!("Loaded mesh indices: {}", indices);
    LoadMeshResponse {}
}

pub fn load_material(request: LoadMaterialRequest) -> LoadMaterialResponse {
    info!("Received request to load material");
    let state = unsafe { GLOBAL_STATE.as_ref().unwrap() };
    let material = load_texture(state, request.texture_data);
    unsafe {
        GLOBAL_MATERIAL = Some(material);
    }
    LoadMaterialResponse {}
}

pub fn begin_render(request: BeginRenderRequest) -> BeginRenderResponse {
    info!("Received request to begin render");
    let state = unsafe { GLOBAL_STATE.as_ref().unwrap() };
    leg_renderer::render(state, |rpass|{
        unsafe {
            GLOBAL_RENDERPASS = Some(rpass);
        }
        handle_native::<RenderWaitingRequest, RenderWaitingResponse>(NativeRequestCode::RenderWaiting, RenderWaitingRequest {});
        unsafe {
            GLOBAL_RENDERPASS = None;
        }
    });
    BeginRenderResponse {}
}

pub fn render_draw(request: RenderDrawRequest) -> RenderDrawResponse {
    info!("Received request to draw");
    let state = unsafe { GLOBAL_STATE.as_ref().unwrap() };
    let rpass = unsafe { GLOBAL_RENDERPASS.as_mut().unwrap() };
    let mesh = unsafe { GLOBAL_MESH.as_ref().unwrap() };
    let material = unsafe { GLOBAL_MATERIAL.as_ref().unwrap() };
    leg_renderer::draw(state, rpass, request.camera_view_projection, request.instance_matrix, mesh, material);
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
