use std::ffi::c_void;
use std::io::Cursor;
use prost::Message;
use std::fmt::Debug;
use GlyphFX::Common::Native::*;

pub mod GlyphFX {
    pub mod Common {
        pub mod Native {
            include!(concat!(env!("OUT_DIR"), "/glyph_fx.common.native.rs"));
        }
    }
}

pub type NativeHandle = extern "C" fn(code: i32, message_bytes: *const u8, message_size: usize) -> NativeBuffer;
static mut NATIVE_HANDLER: Option<NativeHandle> = None;

#[repr(C)]
pub struct NativeBuffer {
    pub ptr: *mut c_void,
    pub size: i32,
}

#[no_mangle]
pub extern "C" fn set_native_handler(handler: NativeHandle) {
    unsafe { NATIVE_HANDLER = Some(handler); }
}

#[no_mangle]
pub extern "C" fn process_message(code: i32, message_bytes: *const u8, message_size: usize) -> NativeBuffer {
    let encoded_buf = unsafe { std::slice::from_raw_parts(message_bytes, message_size) };

    let response_bytes = match code {
        1 => execute::<GetPersonRequest, GetPersonResponse, _>(encoded_buf, handle_request),
        _ => panic!("Unknown message code: {}", code),
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

fn handle_request(request: GetPersonRequest) -> GetPersonResponse {
    let dotnet = get_dotnet(request.name.as_str());
    GetPersonResponse {
        email: request.name + "@rust" + &dotnet,
    }
}

fn handle_native<TRequest: Message + Debug + Default, TResponse: Message + Debug + Default>(request: TRequest) -> TResponse {
    let encoded = encode(request).into_boxed_slice();
    let len = encoded.len();
    let ptr = encoded.as_ptr();
    let response = unsafe { NATIVE_HANDLER.unwrap()(1000, ptr, len) };

    let response_bytes = unsafe { std::slice::from_raw_parts(response.ptr as *const u8, response.size as usize) };
    decode::<TResponse>(response_bytes)
}

fn get_dotnet(name: &str) -> String{
    let request = GetDotnetRequest {
        name: name.to_string(),
    };
    let response = handle_native::<GetDotnetRequest, GetDotnetResponse>(request);
    response.email
}

