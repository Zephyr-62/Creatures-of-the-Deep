using UnityEngine;
using UnityEngine.Serialization;

public class SubmarineScreen : MonoBehaviour
{
    [SerializeField] private GameObject screen;
    [SerializeField] private Button buttonL;
    [SerializeField] private Button buttonR;
    [SerializeField] private SubmarineCamera[] cameras;
    [SerializeField] private int cameraIndex;

    private Renderer _screenRenderer;
    
    private void Awake()
    {
        _screenRenderer = screen.GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        SetScreenCameraView();
    }

    private void OnEnable()
    {
        buttonL.onGrabbed.AddListener(DecreaseCameraIndex);

        buttonR.onGrabbed.AddListener(IncreaseCameraIndex);
    }

    private void OnDisable()
    {
        buttonL.onGrabbed.RemoveListener(DecreaseCameraIndex);

        buttonR.onGrabbed.RemoveListener(IncreaseCameraIndex);
    }

    private void DecreaseCameraIndex()
    {
        cameraIndex--;
        if (cameraIndex < 0) cameraIndex = cameras.Length - 1;
        SetScreenCameraView();
    }

    private void IncreaseCameraIndex()
    {
        cameraIndex = (cameraIndex + 1) % cameras.Length;
        SetScreenCameraView();
    }

    private void SetScreenCameraView()
    {
        _screenRenderer.material.mainTexture = cameras[cameraIndex].GetRenderTexture();
    }
}