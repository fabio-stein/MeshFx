using GlyphFX.Common.Scenes;

namespace GlyphFX.Common.Interfaces;

public interface IRenderer
{
    void Initialize();
    void RenderScene(Scene scene, Camera camera);
}