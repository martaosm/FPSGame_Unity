using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Models;

public class PlayerController : MonoBehaviour
{
    private CharacterController playerController;
    private DefaultInput defaultInput;
    public Vector2 inputMovement;
    public Vector2 inputView;

    private Vector3 newCameraRotation;
    private Vector3 newPlayerRotation;

    [Header("References")] public Transform cameraHolder;

    [Header("Settings")] 
    public PlayerSettingsModel playerSettings;
    public float viewClampYMin;
    public float viewClampYMax;

    [Header("Gravity")] public float gravityAmount;
    public float gravityMin;
    private float playerGravity;

    public Vector3 jumpingForce;
    private Vector3 jumpingForceVelocity;

    [Header("Stance")] public PlayerStance playerStance;
    private void Awake()
    {
        defaultInput = new DefaultInput();
        defaultInput.Player.Movement.performed += e => inputMovement = e.ReadValue<Vector2>();
        defaultInput.Player.View.performed += e => inputView = e.ReadValue<Vector2>();
        defaultInput.Player.Jump.performed += e => Jump(); 
        defaultInput.Enable();
        newCameraRotation = cameraHolder.localRotation.eulerAngles;
        newPlayerRotation = transform.localRotation.eulerAngles;
        playerController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CalculateView();
        CalculateMovement();
        CalculateJump();
    }

    private void CalculateView()
    {
        newPlayerRotation.y += playerSettings.ViewXSensitivity * (playerSettings.ViewXInverted? -inputView.x : inputView.x) * Time.deltaTime;
        transform.localRotation=Quaternion.Euler(newPlayerRotation);
        
        newCameraRotation.x += playerSettings.ViewYSensitivity * (playerSettings.ViewYInverted? inputView.y : -inputView.y) * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampYMin,viewClampYMax);
        cameraHolder.localRotation=Quaternion.Euler(newCameraRotation);
    }
    private void CalculateMovement()
    {
        var verticalSpeed = playerSettings.WalkingForwardSpeed*inputMovement.y*Time.deltaTime;
        var horizontalSpeed = playerSettings.WalkingStrafeSpeed*inputMovement.x*Time.deltaTime;
        var newMovementSpeed = new Vector3(horizontalSpeed, 0, verticalSpeed);
        newMovementSpeed = transform.TransformDirection(newMovementSpeed);

        if (playerGravity > gravityMin)
        {
            playerGravity -= gravityAmount * Time.deltaTime;
        }
        
        if (playerGravity < -.1f && playerController.isGrounded)
        {
            playerGravity = -.1f;
        }
        newMovementSpeed.y += playerGravity;
        newMovementSpeed += jumpingForce*Time.deltaTime;
        playerController.Move(newMovementSpeed);
    }

    private void CalculateJump()
    {
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpingForceVelocity,
            playerSettings.jumpingFalloff);
    }
    private void Jump()
    {
        if (!playerController.isGrounded)
        {
            return;
        }

        jumpingForce = Vector3.up * playerSettings.JumpingHeight;
        playerGravity = 0f;
    }
}
