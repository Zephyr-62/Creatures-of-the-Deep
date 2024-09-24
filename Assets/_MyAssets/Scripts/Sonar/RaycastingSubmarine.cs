using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastingSubmarine : MonoBehaviour
{

    [SerializeField] private GameObject _submarine;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _distance;
    [SerializeField] private int _angle;

	//public ComputeShader computeShader;
	public RenderTexture sonar1;
    public RenderTexture sonar2;

    private List<Vector3> _hitPoints;
    private Vector3 _dir;
    public Material blitMat;
    public Material outMat;
    public Texture startingTex;
    public Texture startingTex2;
    public Renderer renderer;
    
    public float delay = 0f;
    
    void Start()
    {
        _hitPoints = new List<Vector3>();
        Graphics.Blit(startingTex,sonar1);
        Graphics.Blit(startingTex2,sonar2);
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

        //if (Time.time >= delay + 0.5f)
        //{
            delay = Time.time;
            blitMat.SetVector("_Point", new Vector2(Random.Range(0f,1f), Random.Range(0f,1f)));
            RenderTexture tempRend = RenderTexture.GetTemporary(sonar1.width, sonar1.height, 0, sonar1.format);
            Graphics.Blit(sonar1, tempRend, blitMat, 0);
            Graphics.Blit(tempRend, sonar2);
            (sonar1, sonar2) = (sonar2, sonar1);
            renderer.material.SetTexture("_BaseMap", sonar1);
        //}
        
    }
/*
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
*/
}
