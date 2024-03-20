use wasm_bindgen::JsValue;
use wasm_bindgen::prelude::wasm_bindgen;

#[wasm_bindgen(start)]
fn main() -> Result<(), JsValue> {
    // Use `web_sys`'s global `window` function to get a handle on the global
    // window object.
    let window = web_sys::window().expect("no global `window` exists");
    let document = window.document().expect("should have a document on window");
    let body = document.body().expect("document should have a body");

    // Manufacture the element we're gonna append
    let val = document.create_element("p")?;
    val.set_inner_html("Hello from Rust!");

    body.append_child(&val)?;

    Ok(())
}

#[repr(C)]
pub struct DemoData{
    pub val1: i32,
    pub val2: i32,
}

#[wasm_bindgen]
pub fn simsum(a: i32, b: i32) -> i32 {
    let data = DemoData {
        val1: a,
        val2: b,
    };
    let ptr_box = Box::new(data);
    let ptr = Box::into_raw(ptr_box);
    ptr as i32
}

#[wasm_bindgen]
pub fn show_sum(ptr: i32) -> i32 {
    let data = unsafe { &*(ptr as *const DemoData) };
    data.val1 + data.val2
}

#[wasm_bindgen]
pub fn simple_string() -> String {
    "Hello from Rust!".to_string()
}