using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Small script for moving text down on clicked buttons
/// to match changed sprite.
/// Gives buttons a 'click down' appearance
/// </summary>
public class ButtonTextShift: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int offsetX = 2, offsetY = 2;
    public RectTransform textRect;
    Vector3 pos;

    void Start()
    {
        pos = textRect.localPosition;
    }

    public void Down()
    {
        textRect.localPosition = new Vector3(pos.x + (float)offsetX, pos.y - (float)offsetY, pos.z);
    }

    public void Up()
    {
        textRect.localPosition = pos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Down();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Up();
    }
}