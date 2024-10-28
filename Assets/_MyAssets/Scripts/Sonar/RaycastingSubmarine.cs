using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class RaycastingSubmarine : ElectricalDevice
{

    [SerializeField] private GameObject _submarine;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _minDistance;
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _angleStep;
    [SerializeField, Range(0, 1)] private float zoom;
    [SerializeField] private PhysicalControlSurface pcs;
    [SerializeField] private TMP_Text distanceLabel;

    private float _distance;

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
	private Vector3 _artiPos3D;
    private Vector2 _calculatedArtiPos;
	private Vector2 _currentArtiPos;
    private Vector2 _artiArrowL;
    private Vector2 _artiArrowR;

    private bool _arrow;
    private float _angleArt;
    void Start()
    {
        ArtifactLocation(new Vector3(10000, 10000, 10000));
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
        if(pcs) zoom = pcs.Get01FloatValue();
        _distance = Mathf.Lerp(_minDistance, _maxDistance, zoom);

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
        if(_artiPos != new Vector2(10000, 10000))
        {
            _currentArtiPos = CalculateArtiPoint();
            blitMat.SetVector("_PointArtifact", _currentArtiPos);
            if (_arrow)
            {
                _arrow = false;
                blitMat.SetVector("_PointArrowL", CalculateArrowL());
                blitMat.SetVector("_PointArrowR", CalculateArrowR());
            }
            else
            {
                blitMat.SetVector("_PointArrowL", _currentArtiPos);
                blitMat.SetVector("_PointArrowR", _currentArtiPos);

            }
            if(powered)
                distanceLabel.text = $"Dist:\n{Mathf.RoundToInt((_artiPos3D - _submarine.transform.position).magnitude)} m";
            else
                distanceLabel.text = "";

        }


        blitMat.SetVector("_Point", _hitpoint);
        blitMat.SetVector("_PointB", _linePoint);
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

    bool powered = true;

    protected override void OnPowerGained()
    {
        renderer.material.SetInt("_On", 1);
        powered = true;
    }

    protected override void OnPowerLost()
    {
        renderer.material.SetInt("_On", 0);
        powered = false;

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
        if (pos == new Vector3(10000, 10000, 10000))
        {
            blitMat.SetInt("_ArtefactActivate", 0);
            _artiPos = new Vector2(10000, 10000);
            distanceLabel.text = "";
        }
        else
        {
            blitMat.SetInt("_ArtefactActivate", 1);
            _artiPos = new Vector2(pos.x,  pos.z);
        }
        _artiPos3D = pos;
        
	}

	private Vector2 CalculateArtiPoint()
    {
        Vector3 subPos = _submarine.transform.position;
        Vector2 subWithoutY = new Vector2(subPos.x, subPos.z);
        float dis = Vector2.Distance(subWithoutY, _artiPos);
        Vector2 dirVec = _artiPos - subWithoutY;
        Vector2 subForward = new Vector2(_submarine.transform.forward.x, _submarine.transform.forward.z);
        _angleArt = Vector2.SignedAngle(subForward, dirVec);
        float angleRad = _angleArt * Mathf.Deg2Rad;
        Vector2 rotVec;
        if (Mathf.Abs(dis) > (_distance - (_distance * 0.08f) ))
        {
            rotVec = new Vector2(0, 0.92f);
            _arrow = true;
        }
        else
        {
            float scaledDis = (dis / (_distance));
            rotVec = (Vector2.up * scaledDis);
        }
        return new Vector2(rotVec.x * Mathf.Cos(angleRad) - rotVec.y * Mathf.Sin(angleRad), rotVec.x * Mathf.Sin(angleRad) + rotVec.y * Mathf.Cos(angleRad));
    }

    private Vector2 CalculateArrowL()
    {
        Vector2 rotVec = new Vector2(0, 0.88f);
        float angleRad = (_angleArt - 2.5f) * Mathf.Deg2Rad;
        return new Vector2(rotVec.x * Mathf.Cos(angleRad) - rotVec.y * Mathf.Sin(angleRad), rotVec.x * Mathf.Sin(angleRad) + rotVec.y * Mathf.Cos(angleRad));
    }
    private Vector2 CalculateArrowR()
    {
        Vector2 rotVec = new Vector2(0, 0.88f);
        float angleRad = (_angleArt + 2.5f) * Mathf.Deg2Rad;
        return new Vector2(rotVec.x * Mathf.Cos(angleRad) - rotVec.y * Mathf.Sin(angleRad), rotVec.x * Mathf.Sin(angleRad) + rotVec.y * Mathf.Cos(angleRad));
    }
}