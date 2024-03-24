use std::ffi::c_void;
use std::io::Cursor;
use prost::Message;
use std::fmt::Debug;
use log::info;
use glyphfx_native::*;
use crate::window::run_main_loop;

pub mod glyphfx_native {
    include!(concat!(env!("OUT_DIR"), "/glyphfx_native.rs"));
}

pub type NativeHandle = extern "C" fn(code: i32, message_bytes: *const u8, message_size: usize) -> NativeBuffer;
static mut NATIVE_HANDLER: Option<NativeHandle> = None;

#[repr(C)]
pub struct NativeBuffer {
    pub ptr: *mut c_void,
    pub size: i32,
}

#[cfg(not(target_arch = "wasm32"))]
#[no_mangle]
pub extern "C" fn init_desktop() {
    use env_logger::Env;
    use env_logger::Builder;
    use env_logger::Target;
    let mut builder = Builder::from_env(Env::default().default_filter_or("info"));
    builder.init();
}

#[no_mangle]
pub extern "C" fn set_native_handler(handler: NativeHandle) {
    unsafe { NATIVE_HANDLER = Some(handler); }
}

#[no_mangle]
pub extern "C" fn process_message(code: i32, message_bytes: *const u8, message_size: usize) -> NativeBuffer {
    let encoded_buf = unsafe { std::slice::from_raw_parts(message_bytes, message_size) };

    let response_bytes = match code {
        1 => execute::<GetRustRequest, GetRustResponse, _>(encoded_buf, handle_request),
        2 => execute::<RunMainLoopRequest, RunMainLoopResponse, _>(encoded_buf, run_main_loop),
        _ => panic!("Unknown message codex: {}", code),
    };

    let size = response_bytes.len() as i32;
    let ptr = Box::into_raw(response_bytes.into_boxed_slice()) as *mut c_void;

    NativeBuffer {
        ptr,
        size,
    }
}

fn execute<Request, Response, F>(encoded_buf: &[u8], handler: F) -> Vec<u8>
    where
        Request: Message + Default,
        Response: Message + Default,
        F: Fn(Request) -> Response,
{
    let request = decode::<Request>(encoded_buf);
    let response = handler(request);
    encode(response)
}


fn decode<T: Message + Debug + Default>(encoded_buf: &[u8]) -> T {
    let mut cursor = Cursor::new(encoded_buf);
    T::decode(&mut cursor).unwrap()
}

fn encode<T: Message>(message: T) -> Vec<u8> {
    let mut buf = Vec::new();
    message.encode(&mut buf).unwrap();
    buf
}

fn handle_request(request: GetRustRequest) -> GetRustResponse {
    let dotnet = get_dotnet(request.name.as_str());
    GetRustResponse {
        email: request.name + "@rust" + &dotnet,
    }
}

pub(crate) fn handle_native<TRequest: Message + Debug + Default, TResponse: Message + Debug + Default>(code: NativeRequestCode, request: TRequest) -> TResponse {
    let encoded = encode(request).into_boxed_slice();
    let len = encoded.len();
    let ptr = encoded.as_ptr();
    let response = unsafe { NATIVE_HANDLER.unwrap()(code as i32, ptr, len) };

    let response_bytes = unsafe { std::slice::from_raw_parts(response.ptr as *const u8, response.size as usize) };
    decode::<TResponse>(response_bytes)
}

fn get_dotnet(name: &str) -> String{
    info!("Requesting dotnet email for: {}", name);
    let request = GetDotnetRequest {
        name: name.to_string(),
    };
    let response = handle_native::<GetDotnetRequest, GetDotnetResponse>(NativeRequestCode::GetDotnet, request);
    response.email
}
