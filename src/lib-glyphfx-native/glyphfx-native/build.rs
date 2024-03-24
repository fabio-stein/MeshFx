use std::io::Result;
fn main() -> Result<()> {
    prost_build::compile_protos(&["src/glyphfx_native.proto"], &["src/"])?;
    Ok(())
}