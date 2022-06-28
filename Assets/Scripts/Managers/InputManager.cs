using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputManager : MonoBehaviour
{
    // Handle movement, rotation, and zoom input events
    public delegate void MoveInputHandler(Vector3 moveVector);
    public delegate void RotateInputHandler(float rotateAmount);
    public delegate void ZoomInputHandler(float zoomAmount);
    public delegate void BuildingInputHandler(int buildingIndex);
    public delegate void CancelBuildingInputHandler();

    // TODO: Add listeners for click inpuits, like checking if we clicked an object
    // Example delegate to pass a gameobject during a raycast check so we can see what is being selected
    public delegate void ClickInputHandler (Vector3 mouseVector);
}
