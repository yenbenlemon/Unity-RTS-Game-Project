using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventManager : MonoBehaviour
{
    // Handle events where the resources are updated (currency)
    public delegate void ResourceHandler(int resourceAmount);
    public delegate void CostsIncurredHandler(int costs);

	// Handle when a unit gets selected to train
    public delegate void TrainUnitHandler(int unitDataIndex);

	// Handle events where the building is being placed
    public delegate void PlacingBuildingHandler(bool isPlacingBuilding);
	
    // Handle when a unit gets selected
    public delegate void UnitSelectHandler(GameObject unit);
    public delegate void UnitDeselectHandler();
    public delegate void DragSelectionHandler(Rect selectRect);
    public delegate void DraggingHandler(bool isDragging);

    // Handle when a unit is focused
    public delegate void FocusSelectHandler(GameObject unit);

    // Handle when a unit is created & destroyed
    public delegate void UnitCreatedHandler(GameObject unit);
    public delegate void UnitDestroyedHandler(GameObject unit);
    public delegate void UnitDisableddHandler(GameObject unit);

    // Game State change event
    public delegate void GameStateChangeHandler(GameManager.GameState state);

    // Handle when unit attacks
    public delegate void UnitAttackHandler(GameObject unit, bool isAttacking);
}
