using System.Runtime.InteropServices;

namespace GlyphFX.Engine;

public class SharedBuffer<T>: IDisposable where T : struct
{
    public IntPtr Pointer { get; private set; }
    public int ByteCount { get; private set; }
    public int Count { get; private set; }
    
    public SharedBuffer(T[] data) : this(data.Length)
    {
        SetData(data);
    }
    
    public SharedBuffer(int count)
    {
        ByteCount = Marshal.SizeOf<T>() * count;
        Pointer = Marshal.AllocHGlobal(ByteCount);
        Count = count;
    }
    
    public void SetData(T[] data)
    {
        if (data.Length != Count)
            throw new Exception($"Data length of {data.Length} does not match buffer size: {Count}");
        
        for (int i = 0; i < data.Length; i++)
        {
            IntPtr ptr = Pointer + i * Marshal.SizeOf<T>();
            Marshal.StructureToPtr(data[i], ptr, false);
        }
    }

    private void ReleaseUnmanagedResources()
    {
        if (Pointer != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(Pointer);
            Pointer = IntPtr.Zero;
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