using UnityEngine;

public class SubmarineScreen : MonoBehaviour
{
    [SerializeField] private GameObject screen;
    [SerializeField] private ClickySwitch clickySwitch;
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
        clickySwitch.onValueChanged.AddListener(ToggleScreen);
        
        buttonL.onGrabbed.AddListener(DecreaseCameraIndex);

        buttonR.onGrabbed.AddListener(IncreaseCameraIndex);
    }

    private void OnDisable()
    {
        clickySwitch.onValueChanged.RemoveListener(ToggleScreen);
        
        buttonL.onGrabbed.RemoveListener(DecreaseCameraIndex);

        buttonR.onGrabbed.RemoveListener(IncreaseCameraIndex);
    }

    private void DecreaseCameraIndex()
    {
        cameras[cameraIndex].Unwatch();

        if (--cameraIndex < 0) cameraIndex = cameras.Length - 1;

        SetScreenCameraView();
    }

    private void IncreaseCameraIndex()
    {
        cameras[cameraIndex].Unwatch();

        cameraIndex = (cameraIndex + 1) % cameras.Length;

        SetScreenCameraView();
    }

    private void ToggleScreen()
    {
        if (clickySwitch.GetBoolValue())
        {
            // We should use block instead, but it doesn't work on buttons
            // buttonL.Unblock();
            // buttonR.Unblock();
            buttonL.onGrabbed.AddListener(DecreaseCameraIndex);
            buttonR.onGrabbed.AddListener(IncreaseCameraIndex);
            SetScreenCameraView();
        }
        else
        {
            // We should use block instead, but it doesn't work on buttons
            // buttonL.Block();
            // buttonR.Block();
            buttonL.onGrabbed.RemoveListener(DecreaseCameraIndex);
            buttonR.onGrabbed.RemoveListener(IncreaseCameraIndex);
            RemoveScreenCameraView();
            cameras[cameraIndex].Unwatch();
        }
        
    }

    private void SetScreenCameraView()
    {
        _screenRenderer.material.mainTexture = cameras[cameraIndex].Watch();
    }
    
    private void RemoveScreenCameraView()
    {
        _screenRenderer.material.mainTexture = null;
    }
}