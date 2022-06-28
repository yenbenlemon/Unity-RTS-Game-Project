using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IZoomStrategy
{
    // Simply an interface for the actual Zoom logic in ZoomStrategy.cs
    void ZoomIn(Camera cam, float delta, float nearZoomLimit);
    void ZoomOut(Camera cam, float delta, float farZoomLimit);
}
