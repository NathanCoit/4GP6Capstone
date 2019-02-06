using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Script attached to objects that handles MouseEnter and MouseExit events
/// An event handler script add to each object
/// </summary>
public class TooltipText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Set by TooltipDisplayController
    public string Tooltip = string.Empty;
    public TextMeshProUGUI TooltipTextComponent;
    public GameObject TooltipGameObject;
    public Vector3 OffsetVector;
    private int mintFrameCounter = 0;
    private bool mblnEntered = false;

    void Update()
    {
        if(mblnEntered)
        {
            mintFrameCounter++;
        }
        if (mintFrameCounter > 20)
        {
            TooltipGameObject.SetActive(true);
            TooltipTextComponent.text = Tooltip;
            TooltipGameObject.transform.position = Input.mousePosition + OffsetVector;
            mintFrameCounter = 0;
            mblnEntered = false;
        }
    }

    /// <summary>
    /// Pointer enter event, or MouseOver event
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        mblnEntered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mblnEntered = false;
        mintFrameCounter = 0;
        TooltipTextComponent.text = string.Empty;
        TooltipGameObject.SetActive(false);
    }
}
