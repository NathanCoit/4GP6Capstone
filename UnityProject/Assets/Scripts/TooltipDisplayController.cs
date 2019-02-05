using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Script for displaying text as a tooltip for the player.
/// </summary>
public class TooltipDisplayController : MonoBehaviour
{
    public GameObject TooltipTextGameObject;
    public TextMeshProUGUI TooltipTextComponent;
    // Offset of tooltip from mouse
    private Vector3 muniOffsetVector = new Vector3(50, -20, 0);
    // Start is called before the first frame update
    void Awake()
    {
        // Find the tooltip text object
        TooltipTextComponent.text = string.Empty;
        TooltipTextGameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!string.IsNullOrEmpty(TooltipTextComponent.text))
        {
            // tooltip text exists, ensure tooltip follows mouse
            TooltipTextGameObject.transform.position = Input.mousePosition + muniOffsetVector;
        }
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
        musTooltip.OffsetVector = muniOffsetVector;
    }
}
