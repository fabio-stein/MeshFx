use std::ffi::c_void;
use wgpu::util::DeviceExt;
use crate::State;

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

#[no_mangle]
pub extern "C" fn load_mesh(state: &State, vertex_vec_ptr: *mut c_void, vertex_count: u32, indices_vec_ptr: *const u32, index_count: u32) -> *mut Mesh {
    let vertices = unsafe { std::slice::from_raw_parts(vertex_vec_ptr as *const Vertex, vertex_count as usize) };
    let indices = unsafe { std::slice::from_raw_parts(indices_vec_ptr, index_count as usize) };

    let vertex_buffer = state.device.create_buffer_init(
        &wgpu::util::BufferInitDescriptor {
            label: None,
            usage: wgpu::BufferUsages::VERTEX | wgpu::BufferUsages::COPY_DST,
            contents: bytemuck::cast_slice(vertices),
        }
    );

    let index_buffer = state.device.create_buffer_init(
        &wgpu::util::BufferInitDescriptor {
            label: None,
            usage: wgpu::BufferUsages::INDEX | wgpu::BufferUsages::COPY_DST,
            contents: bytemuck::cast_slice(&indices),
        }
    );

    let mesh = Mesh {
        vertex_buffer,
        index_buffer,
        num_indices: index_count,
    };

    Box::into_raw(Box::new(mesh))
}

