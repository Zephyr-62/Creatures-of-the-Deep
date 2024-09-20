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

    private SubmarineControls controls;
    private Vector2 input;
    private Camera attachedCamera;
    private PhysicalControlSurface pcs;

    private void Awake()
    {
        controls = new SubmarineControls();

        controls.InGame.FirstPersonCamera.performed += HandleFirstPersonCameraInput;
        controls.InGame.FirstPersonCamera.canceled += HandleFirstPersonCameraInput;

        controls.InGame.Grab.performed += HandleGrabInput;
        controls.InGame.Grab.canceled += HandleGrabInput;

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
                pcs.Grab(this);   
            }
        } else if (pcs)
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
            } else
            {
                reticle.Set(Reticle.Mode.Hover);
            }
        } else
        {
            reticle.Set(Reticle.Mode.Normal);
        }

        transform.Rotate(new Vector3(0, input.x * sensitivity.x, 0) * Time.deltaTime, Space.World);
        transform.Rotate(new Vector3(-input.y * sensitivity.y, 0, 0) * Time.deltaTime, Space.Self);
    }

    public bool CheckForPCS()
    {
        if (pcs && pcs.grabbed) return true;

        Ray ray = GetRay();

        if (Physics.Raycast(ray, out RaycastHit hit, 200f, mask))
        {
            pcs = hit.transform.GetComponentInParent<PhysicalControlSurface>();
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