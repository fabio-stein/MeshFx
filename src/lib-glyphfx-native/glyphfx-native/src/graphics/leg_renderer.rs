use std::borrow::Cow;
use std::mem;
use log::info;
use wgpu::{Adapter, BindGroup, BindGroupLayout, Buffer, BufferDescriptor, Device, Instance, PipelineLayout, Queue, RenderPass, RenderPipeline, ShaderModule, Surface, SurfaceConfiguration, VertexBufferLayout};
use wgpu::TextureFormat::Bgra8UnormSrgb;
use wgpu::util::DeviceExt;
use winit::window::Window;
use crate::graphics::material::Material;
use crate::graphics::model::{Mesh};
use crate::graphics::{model, texture};
use crate::graphics::texture::Texture;

#[repr(C)]
#[derive(Copy, Clone, bytemuck::Pod, bytemuck::Zeroable)]
struct InstanceRaw {
    model_matrix: [[f32; 4]; 4],
    normal: [[f32; 3]; 3],
}

#[repr(C)]
#[derive(Debug, Copy, Clone, bytemuck::Pod, bytemuck::Zeroable)]
struct CameraUniform {
    view_proj: [[f32; 4]; 4],
    pos: [f32; 3],
    _padding: u32,
}

#[repr(C)]
#[derive(Debug, Copy, Clone, bytemuck::Pod, bytemuck::Zeroable)]
struct LightUniform {
    position: [f32; 3],
    // Due to uniforms requiring 16 byte (4 float) spacing, we need to use a padding field here
    _padding: u32,
    color: [f32; 3],
    // Due to uniforms requiring 16 byte (4 float) spacing, we need to use a padding field here
    _padding2: u32,
}

pub struct State {
    instance: Instance,
    surface: Surface<'static>,
    adapter: Adapter,
    pub(crate) device: Device,
    pub(crate) queue: Queue,
    shader: ShaderModule,
    pipeline_layout: PipelineLayout,
    render_pipeline: RenderPipeline,
    config: SurfaceConfiguration,
    pub(crate) texture_bind_group_layout: BindGroupLayout,
    camera_buffer: Buffer,
    camera_bind_group: BindGroup,
    instance_buffer: Buffer,
    depth_texture: Texture,
    light_buffer: Buffer,
    light_bind_group: BindGroup,
}

pub async fn init_async(window: &'static Window) -> State {
    //TODO this fixes the instance limit. Should document and restrict
    let predefined_instance_buffer_size = 1000;

    info!("Initializing renderer");

    let backends = wgpu::Backends::all();
    let instance = wgpu::Instance::new(wgpu::InstanceDescriptor {
        backends,
        ..Default::default()
    });

    let surface = instance.create_surface(window)
        .expect("Failed to create surface");

    info!("Surface created");

    let adapter = instance
        .request_adapter(&wgpu::RequestAdapterOptions {
            power_preference: wgpu::PowerPreference::default(),
            force_fallback_adapter: false,
            compatible_surface: Some(&surface),
        })
        .await
        .expect("Failed to find an appropriate adapter");

    info!("Adapter found: {:?}", adapter.get_info());

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

    info!("Device created");

    let shader = device.create_shader_module(wgpu::ShaderModuleDescriptor {
        label: None,
        source: wgpu::ShaderSource::Wgsl(Cow::Borrowed(include_str!("shader.wgsl"))),
    });

    let vertex_buffer_layout = VertexBufferLayout {
        array_stride: mem::size_of::<model::Vertex>() as wgpu::BufferAddress,
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

    let instance_buffer = device.create_buffer(
        &wgpu::BufferDescriptor {
            label: None,
            size: (predefined_instance_buffer_size * mem::size_of::<InstanceRaw>()) as wgpu::BufferAddress,
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
            //Model matrix
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
            //Normal matrix
            wgpu::VertexAttribute {
                offset: mem::size_of::<[f32; 16]>() as wgpu::BufferAddress,
                shader_location: 9,
                format: wgpu::VertexFormat::Float32x3,
            },
            wgpu::VertexAttribute {
                offset: mem::size_of::<[f32; 19]>() as wgpu::BufferAddress,
                shader_location: 10,
                format: wgpu::VertexFormat::Float32x3,
            },
            wgpu::VertexAttribute {
                offset: mem::size_of::<[f32; 22]>() as wgpu::BufferAddress,
                shader_location: 11,
                format: wgpu::VertexFormat::Float32x3,
            },
        ],
    };

    let width = window.inner_size().width.max(1);
    let height = window.inner_size().height.max(1);

    let swapchain_formats = surface.get_capabilities(&adapter).formats;

    let mut config = surface
        .get_default_config(&adapter, width, height)
        .unwrap();
    let is_webgl = adapter.get_info().backend == wgpu::Backend::Gl;
    if is_webgl {
        //We work with srgb textures but the difference is that WebGl expects UnormSrgb as swapchain output, while WebGpu expects Unorm with view format as Srgb instead
        if !swapchain_formats.contains(&wgpu::TextureFormat::Rgba8UnormSrgb) {
            panic!("GL error: Rgba8UnormSrgb is not supported by the adapter");
        }
        config.format = wgpu::TextureFormat::Rgba8UnormSrgb;
    }else {
        if !swapchain_formats.contains(&wgpu::TextureFormat::Bgra8Unorm) {
            panic!("Wgpu error: Bgra8UnormSrgb is not supported by the adapter");
        }
        config.format = wgpu::TextureFormat::Bgra8Unorm;
        config.view_formats = Vec::from(&[Bgra8UnormSrgb]);
    }
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
                visibility: wgpu::ShaderStages::VERTEX | wgpu::ShaderStages::FRAGMENT,
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

    // We'll want to update our lights position, so we use COPY_DST
    let light_buffer = device.create_buffer(
        &BufferDescriptor {
            label: Some("Light VB"),
            mapped_at_creation: false,
            size: mem::size_of::<LightUniform>() as wgpu::BufferAddress,
            usage: wgpu::BufferUsages::UNIFORM | wgpu::BufferUsages::COPY_DST,
        }
    );

    let light_bind_group_layout =
        device.create_bind_group_layout(&wgpu::BindGroupLayoutDescriptor {
            entries: &[wgpu::BindGroupLayoutEntry {
                binding: 0,
                visibility: wgpu::ShaderStages::VERTEX | wgpu::ShaderStages::FRAGMENT,
                ty: wgpu::BindingType::Buffer {
                    ty: wgpu::BufferBindingType::Uniform,
                    has_dynamic_offset: false,
                    min_binding_size: None,
                },
                count: None,
            }],
            label: None,
        });

    let light_bind_group = device.create_bind_group(&wgpu::BindGroupDescriptor {
        layout: &light_bind_group_layout,
        entries: &[wgpu::BindGroupEntry {
            binding: 0,
            resource: light_buffer.as_entire_binding(),
        }],
        label: None,
    });

    let pipeline_layout = device.create_pipeline_layout(&wgpu::PipelineLayoutDescriptor {
        label: None,
        bind_group_layouts: &[
            &texture_bind_group_layout,
            &camera_bind_group_layout,
            &light_bind_group_layout,
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
            targets: &[Some(config.format.add_srgb_suffix().into())],
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

    State {
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
        depth_texture,
        light_buffer,
        light_bind_group,
    }
}

pub fn resize(state: &mut State, width: u32, height: u32) {
    state.config.width = width;
    state.config.height = height;
    state.surface.configure(&state.device, &state.config);

    //TODO should we destroy previous texture?
    state.depth_texture = Texture::create_depth_texture(&state.device, &state.config, "depth_texture");
}

pub type RenderCallback = fn(&'static mut RenderPass<'static>);

pub fn render(state: &State, render_callback: RenderCallback, instances_matrix: Vec<u8>, camera_uniform: Vec<u8>, light_uniform: Vec<u8>){
    let frame = state.surface
        .get_current_texture()
        .expect("Failed to acquire next swap chain texture");
    let view = frame
        .texture
        .create_view(&wgpu::TextureViewDescriptor{
            format: Some(state.config.format.add_srgb_suffix()),
            ..Default::default()
        });
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

        state.queue.write_buffer(&state.instance_buffer, 0, instances_matrix.as_slice());
        rpass.set_vertex_buffer(1, state.instance_buffer.slice(..));

        state.queue.write_buffer(&state.camera_buffer, 0, bytemuck::cast_slice(camera_uniform.as_slice()));
        rpass.set_bind_group(1, &state.camera_bind_group, &[]);

        rpass.set_bind_group(2, &state.light_bind_group, &[]);
        state.queue.write_buffer(&state.light_buffer, 0, bytemuck::cast_slice(light_uniform.as_slice()));

        let rpass_ref = &mut rpass;
        let rpass_shared_ref: &'static mut RenderPass<'static> = unsafe { mem::transmute(rpass_ref) };

        render_callback(rpass_shared_ref);
    }

    state.queue.submit(Some(encoder.finish()));
    frame.present();
}

pub fn draw(state: &'static State, rpass: &mut wgpu::RenderPass<'static>, instance_item_offset: u32, instance_count: u32, mesh: &'static Mesh, material: &'static Material) {
    rpass.set_bind_group(0, &material.bind_group, &[]);
    rpass.set_vertex_buffer(0, mesh.vertex_buffer.slice(..));
    rpass.set_index_buffer(mesh.index_buffer.slice(..), wgpu::IndexFormat::Uint32);

    let max_offset = instance_item_offset + instance_count;
    rpass.draw_indexed(0..mesh.num_indices, 0, instance_item_offset..max_offset);
}