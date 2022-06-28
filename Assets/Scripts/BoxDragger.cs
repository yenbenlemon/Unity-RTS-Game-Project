using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDragger : EventManager
{
    // Event
    public static event DraggingHandler OnDrag;

    // These create our visual and logical box
    public RectTransform boxTran;
    Rect selectRect;

    // Don't need a vector position yet
    Vector2 start, end = Vector2.zero;

    void Start()
    {
        DrawBox();
    }

    // Update is called once per frame
    void Update()
    {
        // Get starting position of box
        if (Input.GetMouseButtonDown(1))
        {
            start = Input.mousePosition;
            selectRect = new Rect();
        }

        // Draw the box
        if (Input.GetMouseButton(1))
        {
            end = Input.mousePosition;
            DrawBox();
            CreateSelection();
        }

        // Remove box and create our selection for game manager
        if (Input.GetMouseButtonUp(1))
        {
			GameManager.Instance.BoxDragger_OnDragSelection(selectRect);
            start = Vector2.zero;
            end = Vector2.zero;
            DrawBox();
        }
    }

	/*
	 *	Function:	DrawBox
	 *	Purpose:	Draw a box on the player's view port
	 */
    void DrawBox()
    {
        boxTran.position = (start + end) / 2.0f;
        boxTran.sizeDelta = new Vector2(Mathf.Abs(start.x - end.x), Mathf.Abs(start.y - end.y));
    }

	/*
	 *	Function:	CreateSelection
	 *	Purpose:	Define the box dimensions based upon player mouse movement
	 */
    void CreateSelection()
    {
        float offset = 0.0f; // test

        if(Input.mousePosition.x > start.x) // We dragged right
        {
            selectRect.xMin = start.x - offset;
            selectRect.xMax = Input.mousePosition.x + offset;
        }
        else // We dragged left
        {
            selectRect.xMin = Input.mousePosition.x - offset;
            selectRect.xMax = start.x + offset;
        }

        if (Input.mousePosition.y > start.y) // We dragged up
        {
            selectRect.yMin = start.y - offset;
            selectRect.yMax = Input.mousePosition.y + offset;
        }
        else // We dragged down
        {
            selectRect.yMin = Input.mousePosition.y - offset;
            selectRect.yMax = start.y + offset;
        }
    }


}
