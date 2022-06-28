using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI resourceText;

    public GameObject stateObj;
    public TextMeshProUGUI stateText;

    void OnEnable()
    {
        // Resource listener
        GameManager.OnResourceUpdate += GameManager_OnResourceUpdate;

        // State listener
        GameManager.OnGameStateChange += GameManager_OnGameStateChange;
    }

    void OnDisable()
    {
        // Remove listeners
        GameManager.OnResourceUpdate -= GameManager_OnResourceUpdate;
        GameManager.OnGameStateChange -= GameManager_OnGameStateChange;
    }

	/*
	 *	Function:	GameManager_OnResourceUpdate
	 *	Purpose:	Update onscreen player resources
	 *	In:			resourceAmount (Player's current resource amount)
	 */
    void GameManager_OnResourceUpdate(int resourceAmount)
    {
        // Update the resource UI
        resourceText.text = "Resources: " + resourceAmount.ToString();
    }

	/*
	 *	Function:	GameManager_OnGameStateChange
	 *	Purpose:	Manage on screen output when game state changes
	 *	In:			state (Current game state)
	 */
    void GameManager_OnGameStateChange(GameManager.GameState state)
    {
        // Deprecated. Was originally for a UI menu, but we switch scenes instead.
        switch(state)
        {
            case GameManager.GameState.PLAY:
                stateObj.SetActive(false);
                break;
            case GameManager.GameState.PAUSE:
                stateObj.SetActive(true);
                stateText.text = "PAUSED";
                break;
            case GameManager.GameState.WIN:
                stateObj.SetActive(true);
                stateText.text = "YOU WIN!!";
                break;
            case GameManager.GameState.LOSE:
                stateObj.SetActive(true);
                stateText.text = "GAME OVER...";
                break;
            default:
                break;
        }
    }
}
