using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Models;

public class PlayerController : MonoBehaviour
{
    private CharacterController playerController;
    private DefaultInput defaultInput;
    private Vector2 inputMovement;
    private Vector2 inputView;

    private Vector3 newCameraRotation;
    private Vector3 newPlayerRotation;

    [Header("References")] public Transform cameraHolder;
    public Transform feetTransform;
    
    [Header("Settings")] 
    public PlayerSettingsModel playerSettings;
    public float viewClampYMin;
    public float viewClampYMax;
    public LayerMask playerMask;
    
    [Header("Gravity")] public float gravityAmount;
    public float gravityMin;
    private float playerGravity;

    public Vector3 jumpingForce;
    private Vector3 jumpingForceVelocity;

    [Header("Stance")] public PlayerStance playerStance;
    public float playerStanceSmoothing;
    public CharacterStance playerStandStance;
    public CharacterStance playerCrouchStance;
    public CharacterStance playerProneStance;
    public float stanceCheckErrorMargin = 0.05f;
    private float cameraHeight;
    private float cameraHeightVelocity;
    
    
    private Vector3 stanceCapsuleCenterVelocity;
    private float stanceCapsuleHeightVelocity;

    private bool isSprinting;
    private void Awake()
    {
        defaultInput = new DefaultInput();
        defaultInput.Player.Movement.performed += e => inputMovement = e.ReadValue<Vector2>();
        defaultInput.Player.View.performed += e => inputView = e.ReadValue<Vector2>();
        defaultInput.Player.Jump.performed += e => Jump();
        defaultInput.Player.Crouch.performed += e => Crouch();
        defaultInput.Player.Prone.performed += e => Prone();
        defaultInput.Player.Sprint.performed += e => toggleSprint();
        defaultInput.Enable();
        newCameraRotation = cameraHolder.localRotation.eulerAngles;
        newPlayerRotation = transform.localRotation.eulerAngles;
        playerController = GetComponent<CharacterController>();

        cameraHeight = cameraHolder.localPosition.y;
    }

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        CalculateView();
        CalculateMovement();
        CalculateJump();
        CalculateStance();
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
        if (inputMovement.y <= .1f)
        {
            isSprinting = false;
        }
        
        var verticalSpeed = playerSettings.WalkingForwardSpeed;
        var horizontalSpeed = playerSettings.WalkingStrafeSpeed;

        if (isSprinting)
        {
            verticalSpeed = playerSettings.RunningForwardSpeed;
            horizontalSpeed = playerSettings.RunningStrafeSpeed;
        }
        
        var newMovementSpeed = new Vector3(horizontalSpeed*inputMovement.x*Time.deltaTime, 0,verticalSpeed*inputMovement.y*Time.deltaTime );
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

    private void CalculateStance()
    {
        var currStance = playerStandStance;
        if (playerStance == PlayerStance.Crouch)
        {
            currStance = playerCrouchStance;
        }else if (playerStance == PlayerStance.Prone)
        {
            currStance = playerProneStance;
        }
        cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y, currStance.CameraHeight,
            ref cameraHeightVelocity, playerStanceSmoothing);
        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, cameraHeight,cameraHolder.localPosition.z);
        playerController.height =
            Mathf.SmoothDamp(playerController.height, currStance.stanceCollider.height, ref stanceCapsuleHeightVelocity,
                playerStanceSmoothing);
        playerController.center = Vector3.SmoothDamp(playerController.center, currStance.stanceCollider.center,
            ref stanceCapsuleCenterVelocity, playerStanceSmoothing);
    }
    private void Jump()
    {
        if (!playerController.isGrounded || playerStance == PlayerStance.Prone)
        {
            return;
        }

        if (playerStance == PlayerStance.Crouch)
        {
            playerStance = PlayerStance.Stand;
            return;
        }

        jumpingForce = Vector3.up * playerSettings.JumpingHeight;
        playerGravity = 0f;
    }

    private void Crouch()
    {
        if (playerStance == PlayerStance.Crouch)
        {
            if (stanceCheck(playerStandStance.stanceCollider.height))
            {
                return;
            }
            playerStance = PlayerStance.Stand;
            return;
        }
        if (stanceCheck(playerCrouchStance.stanceCollider.height))
        {
            return;
        }
        playerStance = PlayerStance.Crouch;
    }

    private void Prone()
    {
        
        playerStance = PlayerStance.Prone;
    }

    private bool stanceCheck(float stanceCheckHeight)
    {

        var start = new Vector3(feetTransform.position.x, feetTransform.position.y + playerController.radius + stanceCheckErrorMargin,feetTransform.position.z);
        var end = new Vector3(feetTransform.position.x, feetTransform.position.y - playerController.radius - stanceCheckErrorMargin+stanceCheckHeight,feetTransform.position.z);
        return Physics.CheckCapsule(start, end, playerController.radius, playerMask);
    }

    private void toggleSprint()
    {
        if (inputMovement.y <= .1f)
        {
            isSprinting = false;
            return;
        }
        isSprinting = !isSprinting;

    }
}
