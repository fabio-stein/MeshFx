use std::io::Result;
fn main() -> Result<()> {
    prost_build::compile_protos(&["src/meshfx_native.proto"], &["src/"])?;
    Ok(())
}