using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceUIScript : MonoBehaviour {
    public GameObject MaterialTextObject = null;
    public GameObject WorshipperTextObject = null;
    public GameObject MoraleTextObject = null;
    public GameObject RewardsTextObject = null;
    public GameObject RewardsDisplayObject = null;
    public TextMeshProUGUI MaterialText { get; private set; }
    public TextMeshProUGUI MoraleText { get; private set; }
    public TextMeshProUGUI WorshipperText { get; private set; }
    public TextMeshProUGUI RewardsText { get; private set; }
    // Use this for initialization
    void Start () {
        MaterialText = MaterialTextObject.GetComponent<TextMeshProUGUI>();
        MoraleText = MoraleTextObject.GetComponent<TextMeshProUGUI>();
        WorshipperText = WorshipperTextObject.GetComponent<TextMeshProUGUI>();
        RewardsText = RewardsTextObject.GetComponent<TextMeshProUGUI>();
    }

	// Update is called once per frame
	void Update () {
	}

	public void UpdateResourceUIElements(int pintMaterialCount, int pintWorshipperCount, float pfMorale, int pintRewardPoints){
		MaterialText.text = pintMaterialCount.ToString();
        MoraleText.text = pfMorale.ToString("0.00");
        WorshipperText.text = pintWorshipperCount.ToString();
        if(pintRewardPoints > 0 && !RewardsDisplayObject.activeInHierarchy)
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
