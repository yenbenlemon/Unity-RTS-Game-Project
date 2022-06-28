using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraManager : MonoBehaviour
{
    [Header("Camera Positioning")]
    public Vector3 cameraOffset = new Vector3(5.5f, 5.7f, -5.3f);
    public float lookAtOffset = 2f;

    [Header("Move Controls")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 45f;

    [Header("Movement Bounds")]
    public Vector3 minBounds, maxBounds;

    [Header("Zoom Controls")]
    public float zoomSpeed = 4f;
    public float nearZoomLimit = 2f;
    public float farZoomLimit = 16f;
    public float startingZoom = 5f;

	// Public variables
	public Camera fowCamera;
    // Private variables
    IZoomStrategy zoomStrat;
    IZoomStrategy fowZoomStrat;
    Vector3 frameMove;
    float frameRotate;
    float frameZoom;
    Camera cam;
    bool isFollowing;
    GameObject followTarget;
    float originalSpeed;

    void Awake()
    {
        // Get our camera
        cam = GetComponentInChildren<Camera>();

        // Set position of camera to default
        cam.transform.localPosition = new Vector3(cameraOffset.x, cameraOffset.y, cameraOffset.z);

        // Used for zooming
        zoomStrat = new ZoomStrategy(cam, startingZoom);
        fowZoomStrat = new ZoomStrategy(fowCamera, startingZoom);

        // Look at the map
        cam.transform.LookAt(transform.position + Vector3.up * lookAtOffset);

        // Get our original speed so we can multiply it later
        originalSpeed = moveSpeed;
    }

    void OnEnable()
    {
        // Get keyboard listening
        KeyboardInputManager.OnMoveInput    += KeyboardInputManager_OnMoveInput;
        KeyboardInputManager.OnRotateInput  += KeyboardInputManager_OnRotateInput;
        KeyboardInputManager.OnZoomInput    += KeyboardInputManager_OnZoomInput;

        // Get our mouse listening
        MouseInputManager.OnMoveInput += KeyboardInputManager_OnMoveInput;
        MouseInputManager.OnZoomInput += KeyboardInputManager_OnZoomInput;

        // Check for when a unit is selected
        GameManager.OnFocusSelected += GameManager_OnFocusSelected;
    }

    void OnDisable()
    {
        // Remove keyboard listeners
        KeyboardInputManager.OnMoveInput    -= KeyboardInputManager_OnMoveInput;
        KeyboardInputManager.OnRotateInput  -= KeyboardInputManager_OnRotateInput;
        KeyboardInputManager.OnZoomInput    -= KeyboardInputManager_OnZoomInput;

        // Remove mouse listeners
        MouseInputManager.OnMoveInput -= KeyboardInputManager_OnMoveInput;
        MouseInputManager.OnZoomInput -= KeyboardInputManager_OnZoomInput;

        // Remove GameManager listener
        GameManager.OnFocusSelected -= GameManager_OnFocusSelected;
    }

    void KeyboardInputManager_OnMoveInput(Vector3 moveVector)   { frameMove += moveVector; }
    void KeyboardInputManager_OnRotateInput(float rotateAmount) { frameRotate += rotateAmount; }
    void KeyboardInputManager_OnZoomInput(float zoomAmount)     {  frameZoom += zoomAmount; }
    void GameManager_OnFocusSelected(GameObject unit)
    {
        // Focus has been selected so make sure we are following our target and reset the camera
        isFollowing = true;
        followTarget = unit;
        ResetCamera();
    }

    void LateUpdate()
    {
        // USed for following units
        if(isFollowing)
        {
            this.transform.position = transform.TransformDirection(new Vector3(followTarget.transform.position.x + cameraOffset.x, 
                0, 
                followTarget.transform.position.z + cameraOffset.z));
        }

        // Are we moving?
        if (frameMove != Vector3.zero)
        {
            // We are no longer focused on the selected unit
            isFollowing = false;

            // Perform movement
            Vector3 modFrameMove = new Vector3(frameMove.x, 0, frameMove.z);
            transform.position += transform.TransformDirection(modFrameMove * moveSpeed) * Time.deltaTime;

            // Make sure we stay in bounds
            //LockPosition();

            // Reset to zero to prevent drifting
            frameMove = Vector3.zero;
        }

        // Are we rotating?
        if (frameRotate != 0f) 
        { 
            transform.Rotate(Vector3.up, frameRotate * Time.deltaTime * rotateSpeed);
            frameRotate = 0f;
        }

        // Are we zooming in or out?
        if (frameZoom < 0f)         
        { 
            zoomStrat.ZoomIn(cam, Time.deltaTime * Mathf.Abs(frameZoom) * zoomSpeed, nearZoomLimit);
            zoomStrat.ZoomIn(fowCamera, Time.deltaTime * Mathf.Abs(frameZoom) * zoomSpeed, nearZoomLimit);
            frameZoom = 0f;
        }
        else if (frameZoom > 0f)    
        { 
            zoomStrat.ZoomOut(cam, Time.deltaTime * frameZoom * zoomSpeed, farZoomLimit);
            zoomStrat.ZoomOut(fowCamera, Time.deltaTime * frameZoom * zoomSpeed, farZoomLimit);
            frameZoom = 0f;
        }

        moveSpeed = originalSpeed * (cam.orthographicSize / startingZoom);
    }

	/*
	 *	Function:	LockPosition
	 *	Purpose:	Keep camera within designated boundaries
	 */
    void LockPosition()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x), cameraOffset.y, Mathf.Clamp(transform.position.z, minBounds.z, maxBounds.z)); 
    }

	/*
	 *	Function:	ResetCamera
	 *	Purpose:	Reset camera to original values while focusing on a selected unit
	 */
    void ResetCamera()
    {
        // Defaults
        transform.rotation = Quaternion.identity;

        cam = GetComponentInChildren<Camera>();

        cam.transform.localPosition = new Vector3(cameraOffset.x, cameraOffset.y, cameraOffset.z);

        zoomStrat = new ZoomStrategy(cam, startingZoom);
        fowZoomStrat = new ZoomStrategy(fowCamera, startingZoom);
        cam.transform.LookAt(transform.position + Vector3.up * lookAtOffset);
    }
}
