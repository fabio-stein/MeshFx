use wgpu::util::DeviceExt;

use crate::graphics::leg_renderer::State;

#[repr(C)]
#[derive(Copy, Clone, Debug, bytemuck::Pod, bytemuck::Zeroable)]
pub struct Vertex {
    pub position: [f32; 3],
    pub tex_coords: [f32; 2],
    pub normal: [f32; 3],
}

pub struct Mesh {
    pub vertex_buffer: wgpu::Buffer,
    pub index_buffer: wgpu::Buffer,
    pub num_indices: u32,
}

pub fn load_mesh(state: &State, vertices: Vec<u8>, indices: Vec<u32>) -> Mesh {
    let index_count = indices.len() as u32;
    let indices = indices.as_slice();

    let vertex_buffer = state.device.create_buffer_init(
        &wgpu::util::BufferInitDescriptor {
            label: None,
            usage: wgpu::BufferUsages::VERTEX | wgpu::BufferUsages::COPY_DST,
            contents: &vertices,
        }
    );

    let index_buffer = state.device.create_buffer_init(
        &wgpu::util::BufferInitDescriptor {
            label: None,
            usage: wgpu::BufferUsages::INDEX | wgpu::BufferUsages::COPY_DST,
            contents: bytemuck::cast_slice(indices),
        }
    );

    Mesh {
        vertex_buffer,
        index_buffer,
        num_indices: index_count,
    }
}

