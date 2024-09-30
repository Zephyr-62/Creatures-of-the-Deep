using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private Vector2 sensitivity;
    [SerializeField] private LayerMask mask;
    [SerializeField] private Reticle reticle;
    [SerializeField] private float defaultFOV, zoomFOV;

    private SubmarineControls controls;
    private Vector2 input;
    private Camera attachedCamera;
    private PhysicalControlSurface pcs;
    private Vector3 point;
    private float topAngle;
    private float downAngle;
    private Vector2 rotation;

    private void Awake()
    {
        controls = new SubmarineControls();

        controls.InGame.FirstPersonCamera.performed += HandleFirstPersonCameraInput;
        controls.InGame.FirstPersonCamera.canceled += HandleFirstPersonCameraInput;

        controls.InGame.Grab.performed += HandleGrabInput;
        controls.InGame.Grab.canceled += HandleGrabInput;

        controls.InGame.Zoom.performed += HandleZoomInput;
        controls.InGame.Zoom.canceled += HandleZoomInput;

        controls.Enable();

        attachedCamera = GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void HandleFirstPersonCameraInput(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }

    private void HandleGrabInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (pcs)
            {
                pcs.Grab(this, point);   
            }
        } else if (pcs)
        {
            pcs.Release();
            pcs = null;
        }
    }

    private void HandleZoomInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            attachedCamera.DOFieldOfView(zoomFOV, 0.1f);
        }
        else
        {
            attachedCamera.DOFieldOfView(defaultFOV, 0.1f);
        }
    }

    public void ForceRelease()
    {
        if (pcs != null)
        {
            pcs.Release();
            pcs = null;
        }
    }

    private void Update()
    {
        CheckForPCS();

        if (pcs)
        {
            if(pcs.grabbed)
            {
                reticle.Set(Reticle.Mode.Grabbed);
                input = transform.InverseTransformDirection(pcs.UpdateSurface(transform.TransformDirection(input)));
            } else if(reticle)
            {
                reticle.Set(Reticle.Mode.Hover);
            }
        } else if (reticle)
        {
            reticle.Set(Reticle.Mode.Normal);
        }

        //transform.localRotation = Quaternion.A

        //transform.Rotate(new Vector3(0, input.x * sensitivity.x, 0) * Time.deltaTime, Space.Self);
        //transform.Rotate(new Vector3(-input.y * sensitivity.y, 0, 0) * Time.deltaTime, Space.Self);

        rotation.x += input.x * sensitivity.x * Time.deltaTime;
        rotation.y += input.y * sensitivity.y * Time.deltaTime;
        rotation.y = Mathf.Clamp(rotation.y, -80, 80);
        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        transform.localRotation = xQuat * yQuat;
    }

    public bool CheckForPCS()
    {
        if (pcs && pcs.grabbed) return true;

        Ray ray = GetRay();

        if (Physics.Raycast(ray, out RaycastHit hit, 200f, mask))
        {
            pcs = hit.collider.GetComponentInParent<PhysicalControlSurface>();
            point = hit.point;
            return true;
        }
        pcs = null;
        return false;
    }

    public Ray GetRay()
    {
        return new Ray(attachedCamera.transform.position, attachedCamera.transform.forward);
    }
}
