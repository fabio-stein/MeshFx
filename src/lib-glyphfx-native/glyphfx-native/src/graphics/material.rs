use crate::graphics::leg_renderer::State;
use crate::graphics::texture;
use crate::graphics::texture::Texture;

#[derive(Debug)]
pub struct Material {
    pub diffuse_texture: Texture,
    pub bind_group: wgpu::BindGroup,
}

#[no_mangle]
pub extern "C" fn load_texture(state: &mut State, texture_ptr: *const u8, data_size: u32) -> *mut Material {
    let diffuse_bytes = unsafe { std::slice::from_raw_parts(texture_ptr, data_size as usize) };
    let diffuse_texture = texture::Texture::from_bytes(&state.device, &state.queue, diffuse_bytes, "texture.jpg").unwrap();

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

    let material = Material {
        diffuse_texture,
        bind_group: diffuse_bind_group,
    };

    Box::into_raw(Box::new(material))
}

