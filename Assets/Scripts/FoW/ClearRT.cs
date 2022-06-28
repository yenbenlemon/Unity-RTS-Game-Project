using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ClearRT : MonoBehaviour
{
	public bool clearAfterStart = false;

    // Start is called before the first frame update
    void Start()
    {
		// Reset rendered texture for fog of war at the beginning of the game
        GetComponent<Camera>().clearFlags = CameraClearFlags.Color;
    }

    // Update is called once per frame
    private void OnPostRender()
    {
		// Set clearAfterStart to true to contstantly reset fog of war
		if (clearAfterStart)
		{
	        GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
		}
    }
}
