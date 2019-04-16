using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Script for displaying text as a tooltip for the player.
/// Controls attaching hover events to UI elements and showing text to the player on hover.
/// Allows information to be displayed to the user without cluttering the UI.
/// </summary>
public class TooltipDisplayController : MonoBehaviour
{
    public GameObject TooltipTextGameObject;
    public TextMeshProUGUI TooltipTextComponent;
    // Offset of tooltip from mouse
    public Vector3 OffsetVector = new Vector3(50, -25, 0);

    public Animator TooltipAnimator;
    // Start is called before the first frame update
    void Awake()
    {
        // Find the tooltip text object
        TooltipTextComponent.text = string.Empty;
        TooltipTextGameObject.SetActive(true);
    }

    /// <summary>
    /// Method for attaching the event system for a tooltip to an object
    /// </summary>
    /// <param name="puniGameObject">Unity game object to attach tooltip to</param>
    /// <param name="pstrTooltip">Tooltip text to attach</param>
    public void AttachTooltipToObject(GameObject puniGameObject, string pstrTooltip)
    {
        TooltipText musTooltip = puniGameObject.AddComponent<TooltipText>();
        musTooltip.Tooltip = pstrTooltip;
        musTooltip.TooltipTextComponent = TooltipTextComponent;
        musTooltip.TooltipGameObject = TooltipTextGameObject;
        musTooltip.OffsetVector = OffsetVector;
        musTooltip.TooltipAnimator = TooltipAnimator;
    }
}
