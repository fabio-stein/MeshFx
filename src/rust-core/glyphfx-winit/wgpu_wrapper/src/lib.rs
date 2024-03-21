mod texture;
pub mod model;
pub mod material;

use std::borrow::Cow;
use std::ffi::c_void;
use std::mem;
use wgpu::{Adapter, BindGroup, BindGroupLayout, Buffer, Device, Instance, PipelineLayout, Queue, RenderPass, RenderPipeline, ShaderModule, Surface, SurfaceConfiguration, SurfaceTargetUnsafe, VertexBufferLayout, web_sys};
use wgpu::rwh::{RawDisplayHandle, RawWindowHandle};
use wgpu::util::DeviceExt;
use crate::material::Material;
use crate::model::{Mesh, Vertex};
use crate::texture::Texture;

#[repr(C)]
#[derive(Copy, Clone, bytemuck::Pod, bytemuck::Zeroable)]
struct InstanceRaw {
    model_matrix: [[f32; 4]; 4],
}

#[repr(C)]
#[derive(Debug, Copy, Clone, bytemuck::Pod, bytemuck::Zeroable)]
struct CameraUniform {
    view_proj: [[f32; 4]; 4],
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
    texture_bind_group_layout: BindGroupLayout,
    camera_buffer: Buffer,
    camera_bind_group: BindGroup,
    instance_buffer: Buffer,
    instance_buffer2: Buffer,
    depth_texture: Texture,
    counter: u32,
}

#[no_mangle]
pub extern "C" fn init_state(display_handle: RawDisplayHandle, window_handle: RawWindowHandle) -> *mut State{
    futures::executor::block_on(init_async(display_handle, window_handle))
}

pub async fn init_async(display_handle: RawDisplayHandle, window_handle: RawWindowHandle) -> *mut State {
    //env_logger::init();
    web_sys::console::log_1(&"STEP 1".to_string().into());
    let width = 1600;
    let height = 1200;
    let predefined_buffer_size = 5000;

    web_sys::console::log_1(&"STEP 2".to_string().into());

    let instance = Instance::default();

    web_sys::console::log_1(&"STEP 3".to_string().into());

    let surface = unsafe {
        let raw_handle = SurfaceTargetUnsafe::RawHandle {
            raw_display_handle: display_handle,
            raw_window_handle: window_handle,
        };

        instance.create_surface_unsafe(raw_handle)
            .expect("Failed to create surface")
    };

    web_sys::console::log_1(&"STEP 4".to_string().into());

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

    let swapchain_capabilities = surface.get_capabilities(&adapter);
    let swapchain_format = swapchain_capabilities.formats[0];

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
                shader_location: 1, //V Tex Coords
                format: wgpu::VertexFormat::Float32x2,
            },
            wgpu::VertexAttribute {
                offset: std::mem::size_of::<[f32; 5]>() as wgpu::BufferAddress,
                shader_location: 2, //V Normal
                format: wgpu::VertexFormat::Float32x3,
            },
        ]
    };

    let index_buffer = device.create_buffer(
        &wgpu::BufferDescriptor {
            label: None,
            size: (predefined_buffer_size * std::mem::size_of::<u32>()) as wgpu::BufferAddress,
            usage: wgpu::BufferUsages::INDEX | wgpu::BufferUsages::COPY_DST,
            mapped_at_creation: false,
        }
    );

    let instance_buffer = device.create_buffer(
        &wgpu::BufferDescriptor {
            label: None,
            size: (predefined_buffer_size * std::mem::size_of::<InstanceRaw>()) as wgpu::BufferAddress,
            usage: wgpu::BufferUsages::VERTEX | wgpu::BufferUsages::COPY_DST,
            mapped_at_creation: false,
        }
    );

    let instance_vertex_buffer_layout = VertexBufferLayout {
        array_stride: mem::size_of::<InstanceRaw>() as wgpu::BufferAddress,
        // We need to switch from using a step mode of Vertex to Instance
        // This means that our shaders will only change to use the next
        // instance when the shader starts processing a new instance
        step_mode: wgpu::VertexStepMode::Instance,
        attributes: &[
            // A mat4 takes up 4 vertex slots as it is technically 4 vec4s. We need to define a slot
            // for each vec4. We'll have to reassemble the mat4 in the shader.
            wgpu::VertexAttribute {
                offset: 0,
                // While our vertex shader only uses locations 0, and 1 now, in later tutorials, we'll
                // be using 2, 3, and 4, for Vertex. We'll start at slot 5, not conflict with them later
                shader_location: 5,
                format: wgpu::VertexFormat::Float32x4,
            },
            wgpu::VertexAttribute {
                offset: mem::size_of::<[f32; 4]>() as wgpu::BufferAddress,
                shader_location: 6,
                format: wgpu::VertexFormat::Float32x4,
            },
            wgpu::VertexAttribute {
                offset: mem::size_of::<[f32; 8]>() as wgpu::BufferAddress,
                shader_location: 7,
                format: wgpu::VertexFormat::Float32x4,
            },
            wgpu::VertexAttribute {
                offset: mem::size_of::<[f32; 12]>() as wgpu::BufferAddress,
                shader_location: 8,
                format: wgpu::VertexFormat::Float32x4,
            },
        ],
    };


    let mut config = surface
        .get_default_config(&adapter, width, height)
        .unwrap();
    surface.configure(&device, &config);


    let texture_bind_group_layout =
        device.create_bind_group_layout(&wgpu::BindGroupLayoutDescriptor {
            entries: &[
                wgpu::BindGroupLayoutEntry {
                    binding: 0,
                    visibility: wgpu::ShaderStages::FRAGMENT,
                    ty: wgpu::BindingType::Texture {
                        multisampled: false,
                        view_dimension: wgpu::TextureViewDimension::D2,
                        sample_type: wgpu::TextureSampleType::Float { filterable: true },
                    },
                    count: None,
                },
                wgpu::BindGroupLayoutEntry {
                    binding: 1,
                    visibility: wgpu::ShaderStages::FRAGMENT,
                    // This should match the filterable field of the
                    // corresponding Texture entry above.
                    ty: wgpu::BindingType::Sampler(wgpu::SamplerBindingType::Filtering),
                    count: None,
                },
            ],
            label: Some("texture_bind_group_layout"),
        });

    let depth_texture = texture::Texture::create_depth_texture(&device, &config, "depth_texture");

    let camera_buffer = device.create_buffer(
        &wgpu::BufferDescriptor {
            label: Some("Camera Buffer"),
            usage: wgpu::BufferUsages::UNIFORM | wgpu::BufferUsages::COPY_DST,
            mapped_at_creation: false,
            size: std::mem::size_of::<CameraUniform>() as wgpu::BufferAddress,
        }
    );

    let camera_bind_group_layout = device.create_bind_group_layout(&wgpu::BindGroupLayoutDescriptor {
        entries: &[
            wgpu::BindGroupLayoutEntry {
                binding: 0,
                visibility: wgpu::ShaderStages::VERTEX,
                ty: wgpu::BindingType::Buffer {
                    ty: wgpu::BufferBindingType::Uniform,
                    has_dynamic_offset: false,
                    min_binding_size: None,
                },
                count: None,
            }
        ],
        label: Some("camera_bind_group_layout"),
    });

    let camera_bind_group = device.create_bind_group(&wgpu::BindGroupDescriptor {
        layout: &camera_bind_group_layout,
        entries: &[
            wgpu::BindGroupEntry {
                binding: 0,
                resource: camera_buffer.as_entire_binding(),
            }
        ],
        label: Some("camera_bind_group"),
    });

    let pipeline_layout = device.create_pipeline_layout(&wgpu::PipelineLayoutDescriptor {
        label: None,
        bind_group_layouts: &[
            &texture_bind_group_layout,
            &camera_bind_group_layout,
        ],
        push_constant_ranges: &[],
    });

    let render_pipeline = device.create_render_pipeline(&wgpu::RenderPipelineDescriptor {
        label: None,
        layout: Some(&pipeline_layout),
        vertex: wgpu::VertexState {
            module: &shader,
            entry_point: "vs_main",
            buffers: &[vertex_buffer_layout, instance_vertex_buffer_layout],
        },
        fragment: Some(wgpu::FragmentState {
            module: &shader,
            entry_point: "fs_main",
            targets: &[Some(swapchain_format.into())],
        }),
        primitive: wgpu::PrimitiveState{
            //cull_mode: Some(wgpu::Face::Back),
            ..Default::default()
        },
        depth_stencil: Some(wgpu::DepthStencilState {
            format: texture::Texture::DEPTH_FORMAT,
            depth_write_enabled: true,
            depth_compare: wgpu::CompareFunction::Less,
            stencil: wgpu::StencilState::default(),
            bias: wgpu::DepthBiasState::default(),
        }),
        multisample: wgpu::MultisampleState::default(),
        multiview: None,
    });

    let instance_buffer2 = device.create_buffer(
        &wgpu::BufferDescriptor {
            label: None,
            size: (predefined_buffer_size * std::mem::size_of::<InstanceRaw>()) as wgpu::BufferAddress,
            usage: wgpu::BufferUsages::VERTEX | wgpu::BufferUsages::COPY_DST,
            mapped_at_creation: false,
        }
    );

    let state = Box::new(State {
        width,
        height,
        instance,
        surface,
        adapter,
        device,
        queue,
        shader,
        pipeline_layout,
        render_pipeline,
        config,
        texture_bind_group_layout,
        camera_buffer,
        camera_bind_group,
        instance_buffer,
        instance_buffer2,
        depth_texture,
        counter: 0,
    });

    Box::into_raw(state)
}

pub type RenderCallback = extern "C" fn(*const RenderPass<'_>);

#[no_mangle]
pub extern "C" fn render(state: &mut State, render_callback: RenderCallback){
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
                        load: wgpu::LoadOp::Clear(wgpu::Color::BLACK),
                        store: wgpu::StoreOp::Store,
                    },
                })],
                depth_stencil_attachment: Some(wgpu::RenderPassDepthStencilAttachment {
                    view: &state.depth_texture.view,
                    depth_ops: Some(wgpu::Operations {
                        load: wgpu::LoadOp::Clear(1.0),
                        store: wgpu::StoreOp::Store,
                    }),
                    stencil_ops: None,
                }),
                timestamp_writes: None,
                occlusion_query_set: None,
            });

        rpass.set_pipeline(&state.render_pipeline);

        let rpass_raw_pointer = &mut rpass as *mut wgpu::RenderPass<'_>;
        render_callback(rpass_raw_pointer);
    }

    state.queue.submit(Some(encoder.finish()));
    frame.present();
}

// pub struct RenderCommand<'a>{
//     pub state: &'a mut State,
//     pub rpass: wgpu::RenderPass<'a>,
// }

#[no_mangle]
pub extern "C" fn draw(state: &'static mut State, rpass: &'static mut wgpu::RenderPass<'static>, camera_uniform: *const f32, instances_single_matrix: *const f32, mesh: &'static mut Mesh, material: &'static mut Material) {
    let camera_uniform = unsafe { std::slice::from_raw_parts(camera_uniform, 16) };
    let instance_single_matrix = unsafe { std::slice::from_raw_parts(instances_single_matrix, 16) };

    if(state.counter == 0) {
        state.queue.write_buffer(&state.instance_buffer, 0, bytemuck::cast_slice(instance_single_matrix));
        rpass.set_vertex_buffer(1, state.instance_buffer.slice(..));
        state.counter = 1;
    }else{
        state.queue.write_buffer(&state.instance_buffer2, 0, bytemuck::cast_slice(instance_single_matrix));
        rpass.set_vertex_buffer(1, state.instance_buffer2.slice(..));
        state.counter = 0;
    }

    state.queue.write_buffer(&state.camera_buffer, 0, bytemuck::cast_slice(camera_uniform));

    rpass.set_bind_group(0, &material.bind_group, &[]);
    rpass.set_bind_group(1, &state.camera_bind_group, &[]);
    rpass.set_vertex_buffer(0, mesh.vertex_buffer.slice(..));
    rpass.set_index_buffer(mesh.index_buffer.slice(..), wgpu::IndexFormat::Uint32);

    rpass.draw_indexed(0..mesh.num_indices, 0, 0..1);
}