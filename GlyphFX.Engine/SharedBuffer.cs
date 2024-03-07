using System.Runtime.InteropServices;

namespace GlyphFX.Engine;

public class SharedBuffer<T>: IDisposable where T : struct
{
    public IntPtr Buffer { get; private set; }
    public int Size { get; private set; }
    public int Count { get; private set; }
    
    public SharedBuffer(int count)
    {
        Size = Marshal.SizeOf<T>() * count;
        Buffer = Marshal.AllocHGlobal(Size);
        Count = count;
    }
    
    public void SetData(T[] data)
    {
        if (data.Length != Count)
            throw new Exception("Data length does not match buffer count");
        
        for (int i = 0; i < data.Length; i++)
        {
            IntPtr ptr = Buffer + i * Marshal.SizeOf<T>();
            Marshal.StructureToPtr(data[i], ptr, false);
        }
    }

    private void ReleaseUnmanagedResources()
    {
        if (Buffer != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(Buffer);
            Buffer = IntPtr.Zero;
        }
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~SharedBuffer()
    {
        ReleaseUnmanagedResources();
    }
}