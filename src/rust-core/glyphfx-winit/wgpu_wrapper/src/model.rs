use std::path::Path;
use wgpu::{BindGroupLayout, Device, Queue};
use crate::{State, texture};
use crate::texture::load_texture;

#[repr(C)]
#[derive(Copy, Clone, Debug, bytemuck::Pod, bytemuck::Zeroable)]
pub struct Vertex {
    pub position: [f32; 3],
    pub tex_coords: [f32; 2],
    pub normal: [f32; 3],
}

#[derive(Debug)]
pub struct Model {
    pub name: String,
    pub vertices: Vec<Vertex>,
    pub indices: Vec<u32>, // Include indices
    pub material_id: Option<usize>,
}

#[derive(Debug)]
pub struct Material {
    pub name: String,
    pub diffuse_texture: texture::Texture,
    pub bind_group: wgpu::BindGroup,
}

#[derive(Debug)]
pub struct ObjFile {
    pub models: Vec<Model>,
    pub materials: Vec<Material>,
}

impl ObjFile {
    pub fn load(file_path: &str, device: &Device, queue: &Queue, texture_bind_group_layout: &BindGroupLayout) -> Result<Self, String> {
        let (models, materials_result) = tobj::load_obj(Path::new(file_path),
                                                        &tobj::LoadOptions {
                                                            triangulate: true,
                                                            single_index: true,
                                                            ..Default::default()
                                                        })
            .map_err(|e| format!("Failed to load OBJ file: {:?}", e))?;

        let obj_models: Vec<Model> = models.into_iter().map(|m| {
            let mesh = m.mesh;

            let mut vertices = Vec::new();
            for i in 0..mesh.positions.len() / 3 {
                let vertex = Vertex {
                    position: [
                        mesh.positions[3 * i],
                        mesh.positions[3 * i + 1],
                        mesh.positions[3 * i + 2],
                    ],
                    tex_coords: [
                        mesh.texcoords[2 * i],
                        mesh.texcoords[2 * i + 1],
                    ],
                    normal: [
                        mesh.normals[3 * i],
                        mesh.normals[3 * i + 1],
                        mesh.normals[3 * i + 2],
                    ],
                };
                vertices.push(vertex);
            }

            Model {
                name: m.name,
                vertices,
                indices: mesh.indices,
                material_id: mesh.material_id,
            }
        }).collect();

        let mut mapped_materials = Vec::new();
        let materials = materials_result.unwrap_or_default();

        for material in materials {
            let diffuse_texture = load_texture(&material.diffuse_texture.unwrap(), device, queue).unwrap();
            let bind_group = texture::create_bind_group(device, &diffuse_texture, &texture_bind_group_layout);

            mapped_materials.push(Material{
                name: material.name,
                diffuse_texture,
                bind_group
            })
        }

        Ok(ObjFile {
            models: obj_models,
            materials: mapped_materials,
        })
    }
}
