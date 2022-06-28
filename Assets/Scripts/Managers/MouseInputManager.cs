using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputManager : InputManager
{
    // TODO: Probaby do the raycasting checks here

    Vector2Int screenDim;

    // Events
    public static event MoveInputHandler OnMoveInput;
    public static event RotateInputHandler OnRotateInput;
    public static event ZoomInputHandler OnZoomInput;
    public static event MoveInputHandler OnBuildInput;

    public static event ClickInputHandler OnLeftClickInput;
    public static event ClickInputHandler OnRightClickInput;
    public static event ClickInputHandler OnMiddleClickInput;

    private void Awake()
    {
        screenDim = new Vector2Int(Screen.width, Screen.height);
    }

    private void Update()
    {
        // Get the mouse position
        Vector3 mousePos = Input.mousePosition;

        // See if the mouse position is in a valid location
        bool isValid = (mousePos.y <= screenDim.y * 1.05f && mousePos.y >= screenDim.y * -0.05f &&
            mousePos.x <= screenDim.x * 1.05f && mousePos.x >= screenDim.x * -0.05f);

        if (!isValid) return; // If not valid, don't use the mouse input

        // Movement if we aren't on a UI object
        if (!IsOverUI())
        {
            if (mousePos.y > screenDim.y * 0.98f) { OnMoveInput?.Invoke(Vector3.forward + Vector3.left); }
            else if (mousePos.y < screenDim.y * 0.02f) { OnMoveInput?.Invoke(Vector3.back + Vector3.right); }

            if (mousePos.x > screenDim.x * 0.98f) { OnMoveInput?.Invoke(Vector3.right + Vector3.forward); }
            else if (mousePos.x < screenDim.x * 0.02f) { OnMoveInput?.Invoke(Vector3.left + Vector3.back); }
        }


        // Zoom
        if (Input.mouseScrollDelta.y > 0) { OnZoomInput?.Invoke(-5.0f); }
        if (Input.mouseScrollDelta.y < 0) { OnZoomInput?.Invoke(5.0f); }

        // Mouse Click
        if (Input.GetMouseButtonDown(0)) { OnLeftClickInput?.Invoke(Input.mousePosition); }
        if (Input.GetMouseButtonDown(1)) { OnRightClickInput?.Invoke(Input.mousePosition); }
        if (Input.GetMouseButtonDown(2)) { OnMiddleClickInput?.Invoke(Input.mousePosition); }

		// Build
		OnBuildInput?.Invoke(Input.mousePosition);
    }

	/*
	 *	Function:	IsOverUI
	 *	Purpose:	Check to see if mouse is over a UI element to prevent raycasting
	 *	Return:		bool (True if over UI false otherwise) 
	 */
    public bool IsOverUI()
    {
        // This returns true if over UI
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }
}
