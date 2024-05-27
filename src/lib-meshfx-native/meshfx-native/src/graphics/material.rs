use crate::graphics::leg_renderer::State;
use crate::graphics::texture;
use crate::graphics::texture::Texture;

#[derive(Debug)]
pub struct Material {
    pub diffuse_texture: Texture,
    pub bind_group: wgpu::BindGroup,
}

pub fn load_texture(state: &State, texture_data: Vec<u8>) -> Material {
    let diffuse_texture = texture::Texture::from_bytes(&state.device, &state.queue, &texture_data, "texture.jpg").unwrap();

    let diffuse_bind_group = state.device.create_bind_group(
        &wgpu::BindGroupDescriptor {
            layout: &state.texture_bind_group_layout,
            entries: &[
                wgpu::BindGroupEntry {
                    binding: 0,
                    resource: wgpu::BindingResource::TextureView(&diffuse_texture.view),
                },
                wgpu::BindGroupEntry {
                    binding: 1,
                    resource: wgpu::BindingResource::Sampler(&diffuse_texture.sampler),
                }
            ],
            label: Some("diffuse_bind_group"),
        }
    );

    Material {
        diffuse_texture,
        bind_group: diffuse_bind_group,
    }
}

