using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastingSubmarine : MonoBehaviour
{

    [SerializeField] private GameObject _submarine;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _distance;
    [SerializeField] private int _angle;

	public ComputeShader computeShader;
	public RenderTexture renderTexture;

    private List<Vector3> _hitPoints;
    private Vector3 _dir;

    void Start()
    {
        _hitPoints = new List<Vector3>();
    }
    
    

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 360; i += _angle)
        {
            _dir = Quaternion.AngleAxis(i, _submarine.transform.up) * _submarine.transform.forward;
            if (Physics.Raycast(transform.position, _dir, out RaycastHit hitInfo,
                    _distance, _layerMask))
            {
                if (!_hitPoints.Contains(hitInfo.point))
                {
                    _hitPoints.Add(hitInfo.point);
                }
                Debug.DrawRay(transform.position, _dir * hitInfo.distance, Color.red);
            }
            else
            {
                Debug.DrawRay(transform.position, _dir * _distance, Color.blue);
            }
            
        }
    }

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(256,256,24);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }
        
        
        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.SetFloat("distance", _distance);
        computeShader.SetVector("dir", _dir);
        computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);
        
        Graphics.Blit(renderTexture, dest);
	}

}
