using UnityEngine;
using UnityEngine.Serialization;

public class SubmarineCameraScreen : MonoBehaviour
{
    [SerializeField] private GameObject screen;
    [SerializeField] private GameObject buttonL;
    [SerializeField] private GameObject buttonR;
    [SerializeField] private Material[] cameraViewports;
    [SerializeField] private int cameraIndex = 0;

    private Renderer m_screenRenderer;

    private void Start()
    {
        m_screenRenderer = screen.GetComponent<MeshRenderer>();
        SetScreenCameraView();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform == buttonL.transform)
                {
                    DecreaseCameraIndex();
                    SetScreenCameraView();
                }
                
                if (hit.transform == buttonR.transform)
                {
                    IncreaseCameraIndex();
                    SetScreenCameraView();
                }
            }
        }
    }

    private void DecreaseCameraIndex()
    {
        cameraIndex--;
        if (cameraIndex < 0) cameraIndex = cameraViewports.Length - 1;
    }
    
    private void IncreaseCameraIndex()
    {
        cameraIndex = (cameraIndex + 1) % cameraViewports.Length;
    }

    private void SetScreenCameraView()
    {
        m_screenRenderer.material = cameraViewports[cameraIndex];
    }
}
