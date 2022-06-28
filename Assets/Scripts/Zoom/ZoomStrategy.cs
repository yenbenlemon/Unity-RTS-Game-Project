using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomStrategy : IZoomStrategy
{
	// Constructor
    public ZoomStrategy(Camera cam, float startingZoom)
    {
        // Set default zoom
        cam.orthographicSize = startingZoom;
    }

	/*
	 *	Function:	ZoomIn
	 *	Purpose:	Zoom a given camera in by a given delta if greater than nearest zoom
	 *	In:			cam (Camera with which to zoom)
	 *	In:			delta (Amount to zoom in)
	 *	In:			nearZoomLimit (Nearest zoom depth)
	 */
    public void ZoomIn(Camera cam, float delta, float nearZoomLimit)
    {
        // Check if at limit
        if (cam.orthographicSize == nearZoomLimit) return;

        // Set to new value or limit
        cam.orthographicSize = Mathf.Max(cam.orthographicSize - delta, nearZoomLimit);
    }

	/*
	 *	Function:	ZoomOut
	 *	Purpose:	Zoom a given camera out by a given delta if less than farthest zoom
	 *	In:			cam (Camera with which to zoom)
	 *	In:			delta (Amount to zoom in)
	 *	In:			farZoomLimit (Farthest zoom depth)
	 */
    public void ZoomOut(Camera cam, float delta, float farZoomLimit)
    {
        // Check if at limit
        if (cam.orthographicSize == farZoomLimit) return;

        // Set to new value or limit
        cam.orthographicSize = Mathf.Min(cam.orthographicSize + delta, farZoomLimit);
    }
}
