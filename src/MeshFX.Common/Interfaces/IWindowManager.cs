using MeshFX.Common.Callbacks;
using MeshFX.Common.Native;

namespace MeshFX.Common.Interfaces;

public interface IWindowManager
{
    public event EventHandler<WindowResumeEventRequest> OnResume;
    public event EventHandler<WindowRedrawRequest> OnRedraw; 
    public void RunLoop(IWindowEventHandler windowEventHandler);
}