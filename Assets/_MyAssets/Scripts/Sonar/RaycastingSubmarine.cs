using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastingSubmarine : MonoBehaviour
{

    [SerializeField] private GameObject _submarine;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _distance;
    [SerializeField] private float _angleStep;

	//public ComputeShader computeShader;
	public RenderTexture sonar1;
    public RenderTexture sonar2;
	private float _angle;
    private List<Vector3> _hitPoints;
    private Vector3 _dir;
    public Material blitMat;

    public Texture startingTex;
    public Renderer renderer;
    
    public float delay = 0f;

    private Vector2 _hitpoint;
    private Vector2 _currentHitPoint;
    private bool _raycasting = false;
    void Start()
    {
        _hitPoints = new List<Vector3>();
        Graphics.Blit(startingTex,sonar1);
        Graphics.Blit(startingTex,sonar2);
    }
    
    

    // Update is called once per frame
    void Update()
    {
        if (!_raycasting)
        {
            _raycasting = true;
            StartCoroutine(Raycasting());
        }

        if (_currentHitPoint == null || _currentHitPoint != _hitpoint)
        {
            _currentHitPoint = _hitpoint;
            blitMat.SetVector("_Point", _currentHitPoint);
            RenderTexture tempRend = RenderTexture.GetTemporary(sonar1.width, sonar1.height, 0, sonar1.format);
            Graphics.Blit(sonar1, tempRend, blitMat, 0);
            Graphics.Blit(tempRend, sonar2);
            (sonar1, sonar2) = (sonar2, sonar1);
            renderer.material.SetTexture("_BaseMap", sonar1);
        }
        
    }

    IEnumerator Raycasting()
    {
        _angle += _angleStep;
        if(_angle >= 360) _angle = 0;
        _dir = Quaternion.AngleAxis(_angle, _submarine.transform.up) * _submarine.transform.forward;
        if (Physics.Raycast(transform.position, _dir, out RaycastHit hitInfo,
                _distance, _layerMask))
        {
            _hitpoint = transformHitpoint(hitInfo.distance);
			Debug.Log("Point: " +_hitpoint + ", distance: " + hitInfo.distance);
            Debug.DrawRay(transform.position, _dir * hitInfo.distance, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, _dir * _distance, Color.blue);
        }

        yield return new WaitForSeconds(0.35f);
        _raycasting = false;
    }

    private Vector2 transformHitpoint(float dis)
    { 	
		float scaledDis = (dis / (_distance * 1.5f));
		Vector2 rotVec = (Vector2.up * scaledDis);
        float angleRad = Mathf.Deg2Rad * (-_angle);
        
        return new Vector2(rotVec.x * Mathf.Cos(angleRad) - rotVec.y * Mathf.Sin(angleRad), rotVec.x * Mathf.Sin(angleRad) + rotVec.y * Mathf.Cos(angleRad))  + new Vector2(0.5f, 0.5f);

		//Vector2 rotationVec = Quaternion.Euler(_angle,0, 0) * (Vector2.up * scaledDis);
		//return rotationVec + new Vector2(0.5f, 0.5f);
    }
}
