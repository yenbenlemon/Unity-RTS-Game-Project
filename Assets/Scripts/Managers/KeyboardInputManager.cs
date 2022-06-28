using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputManager : InputManager
{
    // Events
    public static event MoveInputHandler OnMoveInput;
    public static event RotateInputHandler OnRotateInput;
    public static event ZoomInputHandler OnZoomInput;
    public static event BuildingInputHandler OnBuildInput;
    public static event CancelBuildingInputHandler OnCancelBuildInput;

    // Update is called once per frame
    void Update()
    {
        // Movement
        if (Input.GetKey(KeyCode.W)) { OnMoveInput?.Invoke(Vector3.forward + Vector3.left);}
        if (Input.GetKey(KeyCode.A)) { OnMoveInput?.Invoke(Vector3.left + Vector3.back); }
        if (Input.GetKey(KeyCode.S)) { OnMoveInput?.Invoke(Vector3.back + Vector3.right); }
        if (Input.GetKey(KeyCode.D)) { OnMoveInput?.Invoke(Vector3.right + Vector3.forward); }

        // Rotation
        if (Input.GetKey(KeyCode.Q)) { OnRotateInput?.Invoke(1.0f); }
        if (Input.GetKey(KeyCode.E)) { OnRotateInput?.Invoke(-1.0f); }

        // Zoom
        if (Input.GetKey(KeyCode.Z)) { OnZoomInput?.Invoke(-1.0f); }
        if (Input.GetKey(KeyCode.X)) { OnZoomInput?.Invoke(1.0f); }

		// Build buildings
		if (Input.GetKey(KeyCode.Alpha1)) { OnBuildInput?.Invoke(1); } // Barracks
		if (Input.GetKey(KeyCode.Alpha2)) { OnBuildInput?.Invoke(2); } // Factory
		if (Input.GetKey(KeyCode.Alpha3)) { OnBuildInput?.Invoke(3); } // Wall
		if (Input.GetKey(KeyCode.Escape)) { OnCancelBuildInput?.Invoke(); }

    }
}
