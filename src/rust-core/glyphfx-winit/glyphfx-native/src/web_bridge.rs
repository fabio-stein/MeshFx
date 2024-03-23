use std::ffi::c_void;
use std::panic;
use wasm_bindgen::prelude::wasm_bindgen;
use crate::bridge;
use crate::bridge::NativeBuffer;

#[wasm_bindgen(js_namespace = glyphfx_net)]
extern "C" {
    fn HandleNative(code: i32, data: &[u8]) -> Vec<u8>;
}

#[wasm_bindgen(start)]
pub fn initialize() {
    bridge::set_native_handler(send_message);
    panic::set_hook(Box::new(console_error_panic_hook::hook));
}

#[wasm_bindgen]
pub fn process_web_message(code: i32, data: &[u8]) -> Vec<u8> {
    let ptr = data.as_ptr();
    let response = bridge::process_message(code, ptr, data.len());

    //TODO check if need to free memory
    let response_bytes = unsafe { std::slice::from_raw_parts(response.ptr as *const u8, response.size as usize) };
    response_bytes.to_vec()
}

extern "C" fn send_message(code: i32, message_bytes: *const u8, message_size: usize) -> NativeBuffer{
    let encoded_buf = unsafe { std::slice::from_raw_parts(message_bytes, message_size) };
    let result = HandleNative(code, encoded_buf);

    let size = result.len() as i32;
    //TODO free memory to avoid memory leak
    let ptr = Box::into_raw(result.into_boxed_slice()) as *mut c_void;

    NativeBuffer {
        ptr,
        size,
    }
}

#[wasm_bindgen]
pub struct NativeWebBuffer {
    pub ptr: *mut c_void,
    pub size: i32,
}