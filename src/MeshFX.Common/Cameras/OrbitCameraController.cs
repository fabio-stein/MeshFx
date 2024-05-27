using System.Numerics;
using MeshFX.Common.Input;
using MeshFX.Common.Scenes;

namespace MeshFX.Common.Cameras;

public class OrbitCameraController : ICameraController
{
    private Camera _camera;
    public void SetCamera(Camera camera)
    {
        _camera = camera;
    }

    public void Update(IInputManager inputManager)
    {
        var rotationSpeed = 0.01f;

        if (inputManager.IsKeyDown(KeyCode.KeyA))
            _camera.RotateAroundTarget(Vector3.UnitY, rotationSpeed);
        else if (inputManager.IsKeyDown(KeyCode.KeyD))
            _camera.RotateAroundTarget(-Vector3.UnitY, rotationSpeed);

        if (inputManager.IsKeyDown(KeyCode.KeyW))
            _camera.RotateAroundTarget(_camera.Right, rotationSpeed);
        else if (inputManager.IsKeyDown(KeyCode.KeyS))
            _camera.RotateAroundTarget(-_camera.Right, rotationSpeed);

        _camera.UpdateProjection();
    }

}