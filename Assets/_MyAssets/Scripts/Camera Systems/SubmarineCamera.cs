using System.Threading;
using UnityEngine;

public class SubmarineCamera : MonoBehaviour
{
    [SerializeField] private int textureResolution = 256;
    private Camera _cameraComponent;

    private void Awake()
    {
        _cameraComponent = GetComponent<Camera>();
        RenderTexture renderTexture = new RenderTexture(textureResolution, textureResolution, 16, RenderTextureFormat.ARGB32);
        renderTexture.Create();
        _cameraComponent.targetTexture = renderTexture;
    }

    public RenderTexture GetRenderTexture()
    {
        return _cameraComponent.targetTexture;
    }

    public void Toggle()
    {
        _cameraComponent.enabled = !_cameraComponent.enabled;
    }
    
    public void Toggle(bool value)
    {
        _cameraComponent.enabled = value;
    }
}
