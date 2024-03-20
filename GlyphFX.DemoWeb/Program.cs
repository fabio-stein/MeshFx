using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;

Console.WriteLine("Hello, Browserxxxxxx!");
var sum = MyClass.simsum(1, 2);
Console.WriteLine($".NET sum: {sum}");

int ptrInt = MyClass.GetAddr().ToInt32();
Console.WriteLine($"ptrInt: {ptrInt}");

Console.WriteLine($"SimpleString: {MyClass.SimpleString()}");

public partial class MyClass
{
    static MyClass.DemoData data = new MyClass.DemoData { val1 = 1, val2 = 2 };
    static GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
    static IntPtr addr = handle.AddrOfPinnedObject();
    public struct DemoData
    {
        public int val1 { get; set; }
        public int val2 { get; set; }
    }
    
    [JSExport]
    internal static IntPtr GetAddr()
    {
        return addr;
    }
    
    [JSExport]
    internal static void ShowData(IntPtr addr)
    {
        var data = Marshal.PtrToStructure<DemoData>(addr);
        Console.WriteLine($"val1: {data.val1}, val2: {data.val2}");
    }
    
    [JSImport("globalThis.rust.simsum")]
    internal static partial int simsum(int a, int b);

    [JSImport("globalThis.rust.simple_string")]
    internal static partial String SimpleString();

}