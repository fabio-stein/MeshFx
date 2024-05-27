using MeshFX.Common.Input;
using MeshFX.Common.Scenes;

namespace MeshFX.Common.Cameras;

public interface ICameraController
{
    void SetCamera(Camera camera);
    void Update(IInputManager inputManager);
}