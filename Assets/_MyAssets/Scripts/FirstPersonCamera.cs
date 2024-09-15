using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private Vector2 sensitivity;

    private SubmarineControls controls;
    private Vector2 input;

    private void Awake()
    {
        controls = new SubmarineControls();

        controls.InGame.FirstPersonCamera.performed += HandleFirstPersonCameraInput;
        controls.InGame.FirstPersonCamera.canceled += HandleFirstPersonCameraInput;

        controls.Enable();
    }

    private void HandleFirstPersonCameraInput(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, input.x * sensitivity.x, 0) * Time.deltaTime, Space.World);
        transform.Rotate(new Vector3(-input.y * sensitivity.y, 0, 0) * Time.deltaTime, Space.Self);
    }
}
