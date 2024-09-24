using UnityEngine;
using UnityEngine.Events;

public class SubmarineCameraScreen : MonoBehaviour
{
    [SerializeField] private GameObject screen;
    [SerializeField] private Button buttonL;
    [SerializeField] private Button buttonR;
    [SerializeField] private Material[] cameraViewports;
    [SerializeField] private int cameraIndex;

    private Renderer _screenRenderer;
    
    public UnityEvent buttonClicked;

    private void Start()
    {
        _screenRenderer = screen.GetComponent<MeshRenderer>();
        SetScreenCameraView();

        buttonL.onGrabbed.AddListener(() =>
        {
            DecreaseCameraIndex();
            SetScreenCameraView();
        });

        buttonR.onGrabbed.AddListener(() =>
        {
            IncreaseCameraIndex();
            SetScreenCameraView();
        });
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
        if (cameraIndex < 0) cameraIndex = cameraViewports.Length - 1;
        SetScreenCameraView();
    }
    
    private void IncreaseCameraIndex()
    {
        cameraIndex = (cameraIndex + 1) % cameraViewports.Length;
        SetScreenCameraView();
    }

    private void SetScreenCameraView()
    {
        _screenRenderer.material = cameraViewports[cameraIndex];
    }
}
