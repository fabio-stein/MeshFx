using MeshFX.Common.Scenes;

namespace MeshFX.Common.Interfaces;

public interface IRenderer
{
    void Initialize();
    void RenderScene(Scene scene, Camera camera);
}