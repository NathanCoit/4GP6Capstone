using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script to control UI elements that display current resource counts for the player
/// </summary>
public class ResourceUIScript : MonoBehaviour
{
    public GameObject MaterialTextObject = null;
    public GameObject WorshipperTextObject = null;
    public GameObject MoraleTextObject = null;
    public GameObject RewardsTextObject = null;
    public GameObject RewardsDisplayObject = null;
    // Made public for test cases
    public TextMeshProUGUI MaterialText { get; private set; }
    public TextMeshProUGUI MoraleText { get; private set; }
    public TextMeshProUGUI WorshipperText { get; private set; }
    public TextMeshProUGUI RewardsText { get; private set; }
    public TooltipDisplayController TooltipControllerScript;
    // Use this for initialization
    void Start()
    {
        MaterialText = MaterialTextObject.GetComponent<TextMeshProUGUI>();
        MoraleText = MoraleTextObject.GetComponent<TextMeshProUGUI>();
        WorshipperText = WorshipperTextObject.GetComponent<TextMeshProUGUI>();
        RewardsText = RewardsTextObject.GetComponent<TextMeshProUGUI>();
        // Attach tooltips to alleviate confusion
        TooltipControllerScript.AttachTooltipToObject(MaterialTextObject.transform.parent.gameObject, "Materials");
        TooltipControllerScript.AttachTooltipToObject(WorshipperTextObject.transform.parent.gameObject, "Worshippers");
        TooltipControllerScript.AttachTooltipToObject(MoraleTextObject.transform.parent.gameObject, "Morale");
        TooltipControllerScript.AttachTooltipToObject(RewardsDisplayObject, "God Bucks");
    }

    /// <summary>
    /// Method called to update text on UI elements
    /// Called by game manager whenever resource counts change
    /// </summary>
    /// <param name="pintMaterialCount"></param>
    /// <param name="pintWorshipperCount"></param>
    /// <param name="pfMorale"></param>
    /// <param name="pintRewardPoints"></param>
    public void UpdateResourceUIElements(int pintMaterialCount, int pintWorshipperCount, float pfMorale, int pintRewardPoints)
    {
        MaterialText.text = pintMaterialCount.ToString();
        MoraleText.text = pfMorale.ToString("0.00");
        WorshipperText.text = pintWorshipperCount.ToString();
        if (pintRewardPoints > 0 && !RewardsDisplayObject.activeInHierarchy)
        {
            RewardsDisplayObject.SetActive(true);
        }
        else if (pintRewardPoints == 0 && RewardsDisplayObject.activeInHierarchy)
        {
            RewardsDisplayObject.SetActive(false);
        }
        RewardsText.text = pintRewardPoints.ToString();
    }
}
