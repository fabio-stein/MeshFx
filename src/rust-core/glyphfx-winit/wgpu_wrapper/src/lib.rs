use std::borrow::Cow;
use wgpu::{Adapter, Buffer, Device, Instance, PipelineLayout, Queue, RenderPipeline, ShaderModule, Surface, SurfaceConfiguration, SurfaceTargetUnsafe, VertexBufferLayout};
use wgpu::rwh::{RawDisplayHandle, RawWindowHandle};
use wgpu::util::DeviceExt;

#[repr(C)]
#[derive(Copy, Clone, Debug, bytemuck::Pod, bytemuck::Zeroable)]
pub struct Vertex {
    pub position: [f32; 3],
    pub color: [f32; 3],
}


pub struct State {
    width: u32,
    height: u32,
    instance: Instance,
    surface: Surface<'static>,
    adapter: Adapter,
    device: Device,
    queue: Queue,
    shader: ShaderModule,
    pipeline_layout: PipelineLayout,
    render_pipeline: RenderPipeline,
    config: SurfaceConfiguration,
    vertex_buffer: Buffer,
    index_buffer: Buffer,
}

#[no_mangle]
pub extern "C" fn init_state(display_handle: RawDisplayHandle, window_handle: RawWindowHandle) -> *mut State {
    futures::executor::block_on(init_async(display_handle, window_handle))
}

async fn init_async(display_handle: RawDisplayHandle, window_handle: RawWindowHandle) -> *mut State {
    env_logger::init();

    let width = 1600;
    let height = 1200;
    let predefined_buffer_size = 1000;

    let instance = Instance::default();

    let surface = unsafe {
        let raw_handle = SurfaceTargetUnsafe::RawHandle {
            raw_display_handle: display_handle,
            raw_window_handle: window_handle,
        };

        instance.create_surface_unsafe(raw_handle)
            .expect("Failed to create surface")
    };

    let adapter = instance
        .request_adapter(&wgpu::RequestAdapterOptions {
            power_preference: wgpu::PowerPreference::default(),
            force_fallback_adapter: false,
            // Request an adapter which can render to our surface
            compatible_surface: Some(&surface),
        })
        .await
        .expect("Failed to find an appropriate adapter");

    // Create the logical device and command queue
    let (device, queue) = adapter
        .request_device(
            &wgpu::DeviceDescriptor {
                label: None,
                required_features: wgpu::Features::empty(),
                // Make sure we use the texture resolution limits from the adapter, so we can support images the size of the swapchain.
                required_limits: wgpu::Limits::downlevel_webgl2_defaults()
                    .using_resolution(adapter.limits()),
            },
            None,
        )
        .await
        .expect("Failed to create device");

    // Load the shaders from disk
    let shader = device.create_shader_module(wgpu::ShaderModuleDescriptor {
        label: None,
        source: wgpu::ShaderSource::Wgsl(Cow::Borrowed(include_str!("shader.wgsl"))),
    });

    let pipeline_layout = device.create_pipeline_layout(&wgpu::PipelineLayoutDescriptor {
        label: None,
        bind_group_layouts: &[],
        push_constant_ranges: &[],
    });

    let swapchain_capabilities = surface.get_capabilities(&adapter);
    let swapchain_format = swapchain_capabilities.formats[0];

    let vertex_buffer = device.create_buffer(
        &wgpu::BufferDescriptor {
            label: None,
            size: (predefined_buffer_size * std::mem::size_of::<Vertex>()) as wgpu::BufferAddress,
            usage: wgpu::BufferUsages::VERTEX | wgpu::BufferUsages::COPY_DST,
            mapped_at_creation: false,
        }
    );

    let vertex_buffer_layout = VertexBufferLayout {
        array_stride: std::mem::size_of::<Vertex>() as wgpu::BufferAddress,
        step_mode: wgpu::VertexStepMode::Vertex,
        attributes: &[
            wgpu::VertexAttribute {
                offset: 0,
                shader_location: 0, //V Position
                format: wgpu::VertexFormat::Float32x3,
            },
            wgpu::VertexAttribute {
                offset: std::mem::size_of::<[f32; 3]>() as wgpu::BufferAddress,
                shader_location: 1, //V Color
                format: wgpu::VertexFormat::Float32x3,
            }
        ]
    };

    let index_buffer = device.create_buffer(
        &wgpu::BufferDescriptor {
            label: None,
            size: (predefined_buffer_size * std::mem::size_of::<u16>()) as wgpu::BufferAddress,
            usage: wgpu::BufferUsages::INDEX | wgpu::BufferUsages::COPY_DST,
            mapped_at_creation: false,
        }
    );

    let render_pipeline = device.create_render_pipeline(&wgpu::RenderPipelineDescriptor {
        label: None,
        layout: Some(&pipeline_layout),
        vertex: wgpu::VertexState {
            module: &shader,
            entry_point: "vs_main",
            buffers: &[vertex_buffer_layout],
        },
        fragment: Some(wgpu::FragmentState {
            module: &shader,
            entry_point: "fs_main",
            targets: &[Some(swapchain_format.into())],
        }),
        primitive: wgpu::PrimitiveState::default(),
        depth_stencil: None,
        multisample: wgpu::MultisampleState::default(),
        multiview: None,
    });

    let mut config = surface
        .get_default_config(&adapter, width, height)
        .unwrap();
    surface.configure(&device, &config);


    let state = Box::new(State {
        width: 800,
        height: 600,
        instance,
        surface,
        adapter,
        device,
        queue,
        shader,
        pipeline_layout,
        render_pipeline,
        config,
        vertex_buffer,
        index_buffer,
    });

    Box::into_raw(state)
}

#[no_mangle]
pub extern "C" fn render(state: &State, vertices: &[Vertex], indices: &[u16]) {
    let frame = state.surface
        .get_current_texture()
        .expect("Failed to acquire next swap chain texture");
    let view = frame
        .texture
        .create_view(&wgpu::TextureViewDescriptor::default());
    let mut encoder =
        state.device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
            label: None,
        });
    {
        let mut rpass =
            encoder.begin_render_pass(&wgpu::RenderPassDescriptor {
                label: None,
                color_attachments: &[Some(wgpu::RenderPassColorAttachment {
                    view: &view,
                    resolve_target: None,
                    ops: wgpu::Operations {
                        load: wgpu::LoadOp::Clear(wgpu::Color::GREEN),
                        store: wgpu::StoreOp::Store,
                    },
                })],
                depth_stencil_attachment: None,
                timestamp_writes: None,
                occlusion_query_set: None,
            });

        state.queue.write_buffer(&state.vertex_buffer, 0, bytemuck::cast_slice(vertices));
        state.queue.write_buffer(&state.index_buffer, 0, bytemuck::cast_slice(indices));

        rpass.set_pipeline(&state.render_pipeline);
        rpass.set_vertex_buffer(0, state.vertex_buffer.slice(..));
        rpass.set_index_buffer(state.index_buffer.slice(..), wgpu::IndexFormat::Uint16);
        rpass.draw_indexed(0..indices.len() as u32, 0, 0..1);
    }

    state.queue.submit(Some(encoder.finish()));
    frame.present();
}