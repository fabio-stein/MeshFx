using GlyphFX.Common.Input;
using GlyphFX.Common.Scenes;

namespace GlyphFX.Common.Cameras;

public interface ICameraController
{
    void SetCamera(Camera camera);
    void Update(IInputManager inputManager);
}