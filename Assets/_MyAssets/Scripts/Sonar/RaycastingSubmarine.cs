using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastingSubmarine : ElectricalDevice
{

    [SerializeField] private GameObject _submarine;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _distance;
    [SerializeField] private float _angleStep;

    //public ComputeShader computeShader;
    public RenderTexture sonar1;
    public RenderTexture sonar2;
    private float _angle;
    private Vector3 _dir;
    public Material blitMat;

    public Renderer renderer;


    private Vector2 _hitpoint;
    private Vector2 _currentHitPoint;
    private Vector2 _artifactHitpoint;
    private bool _raycasting = false;

    private Vector2 _linePoint;
    private Vector2 _currentLinePoint;

    private Vector3 lastPosition;
    private Quaternion lastRotation;

	private Vector2 _artiPos;
	private Vector3 _currentArtiPos;

    void Start()
    {
        //Debug.Log(LayerMask.LayerToName(_layerMaskArtifact));
    }

    // Update is called once per frame

    void Update()
    {/*
        if (!_raycasting)
        {
            _raycasting = true;
            StartCorountine(Raycasting)
        }*/
       


        _angle += _angleStep * Time.deltaTime;
        if (_angle >= 360) _angle = 0;
        _linePoint = CalculateLinePoint();

        _dir = Quaternion.AngleAxis(_angle, _submarine.transform.up) * _submarine.transform.forward;

        if (Physics.Raycast(_submarine.transform.position, _dir, out RaycastHit hitInfo, _distance, _layerMask))
        {
            //if the hitpoint is an artifact5
            if (hitInfo.transform.gameObject.layer == 8)
            {
                //Debug.Log(hitInfo.transform.gameObject.layer);
                _artifactHitpoint = TransformHitpoint(hitInfo.distance);
            }
            else
            {
                _hitpoint = TransformHitpoint(hitInfo.distance);
            }
            
            
        }
        else
        {
            _hitpoint = TransformHitpoint(_distance);
        }

        



        if (_currentLinePoint == null || _currentLinePoint != _linePoint)
        {
       		 _currentLinePoint = _linePoint;

		}
        if (_currentHitPoint == null || _currentHitPoint != _hitpoint)
        {
            _currentHitPoint = _hitpoint;
        }

        blitMat.SetVector("_Point", _hitpoint);
        blitMat.SetVector("_PointB", _linePoint);
        blitMat.SetVector("_PointArtifact", _artifactHitpoint);
        blitMat.SetFloat("_TimeDeltaTime", Time.deltaTime);

        RenderTexture tempRend = RenderTexture.GetTemporary(sonar1.width, sonar1.height, 0, sonar1.format);
        Graphics.Blit(sonar1, tempRend, blitMat, 0);
        Graphics.Blit(tempRend, sonar2);
        RenderTexture.ReleaseTemporary(tempRend);


        (sonar1, sonar2) = (sonar2, sonar1);
        
        renderer.material.SetTexture("_BaseMap", sonar1);
        

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    IEnumerator Raycasting()
    {
        _angle += _angleStep;
        if (_angle >= 360) _angle = 0;
        _dir = Quaternion.AngleAxis(_angle, _submarine.transform.up) * _submarine.transform.forward;
        if (Physics.Raycast(_submarine.transform.position, _dir, out RaycastHit hitInfo,
                _distance, _layerMask))
        {
            _hitpoint = TransformHitpoint(hitInfo.distance);
        }
        else
        {
            _hitpoint = TransformHitpoint(_distance);
        }
        _linePoint = CalculateLinePoint();

        yield return new WaitForSeconds(0.001f);
        _raycasting = false;
    }

    private Vector2 TransformHitpoint(float dis)
    {
        float scaledDis = (dis / (_distance));
        Vector2 rotVec = (Vector2.up * scaledDis);
        float angleRad = Mathf.Deg2Rad * (-_angle);

        return new Vector2(rotVec.x * Mathf.Cos(angleRad) - rotVec.y * Mathf.Sin(angleRad), rotVec.x * Mathf.Sin(angleRad) + rotVec.y * Mathf.Cos(angleRad));

    }

    private Vector2 CalculateLinePoint()
    {
        Vector2 rotVec = (Vector2.up);
        float angleRad = Mathf.Deg2Rad * (-(_angle));

        return new Vector2(rotVec.x * Mathf.Cos(angleRad) - rotVec.y * Mathf.Sin(angleRad), rotVec.x * Mathf.Sin(angleRad) + rotVec.y * Mathf.Cos(angleRad));

    }

    protected override void OnPowerGained()
    {
        renderer.material.SetInt("_On", 1);
    }

    protected override void OnPowerLost()
    {
        renderer.material.SetInt("_On", 0);

        var wipe = new Texture2D(sonar1.width, sonar1.height);

        Graphics.Blit(wipe, sonar1);
        Graphics.Blit(wipe, sonar2);
    }

    protected override void OnSurge()
    {
        renderer.material.SetFloat("_Interference", surge);
    }
	
	public void ArtifactLocation(Vector3 pos)
	{
		_artiPos = pos;
	}

	private void CalculateDistance()
	{
		
	}
}