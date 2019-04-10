using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script to generate and place tier reward buttons. Because tier reward trees 
/// can vary in depth, width and size, UI must be scaled and added as needed at runtime.
/// </summary>
public class PopulateTierIcons : MonoBehaviour
{
    private float mfXScale = 0;
    private float mfYScale = 0;
    private int mintMaxRewardDepth = 0;
    public Dictionary<string,GameObject> arrTierRewardButtons;

    /// <summary>
    /// Called from game manager at the start of a game.
    /// Creates and places the tier reward buttons based on the given reward tree.
    /// </summary>
    /// <param name="parrTierRewards"></param>
    public void InitializeButtons(List<TierReward> parrTierRewards)
    {
        arrTierRewardButtons = new Dictionary<string, GameObject>();
        Object uniTierRewardButtonPrefab = Resources.Load("Button");
        mintMaxRewardDepth = GetTreeMaxDepth(parrTierRewards);
        float fStartingx = 0 - (Screen.width / 2);
        float fStartingy = 0 - (Screen.height / 2);
        float fEndingx = Screen.width / 2;
        float fEndingy = Screen.height / 2;
        fEndingy = fStartingy + (fEndingy - fStartingy) / mintMaxRewardDepth;
        // Change scale based on screen size, should be able to fit all rewards without overlap
        mfXScale = ((parrTierRewards.Count + mintMaxRewardDepth) / 3f);
        mfYScale = (mintMaxRewardDepth / 3f);
        // Place buttons
        PlaceButtons(uniTierRewardButtonPrefab, parrTierRewards, fStartingx, fEndingx, fStartingy, fEndingy);

        // Scale UI
        GetComponent<RectTransform>().localScale = new Vector3(mfXScale, mfYScale, 1);
    }

    /// <summary>
    /// Recursion Fun
    /// Recursively traverse the tier reward tree placing a button for each reward
    /// </summary>
    /// <param name="puniRewardButtonPrefab"></param>
    /// <param name="parrTierRewards"></param>
    /// <param name="pfStartingx"></param>
    /// <param name="pfEndingx"></param>
    /// <param name="pfStartingy"></param>
    /// <param name="pfEndingy"></param>
    private void PlaceButtons(UnityEngine.Object puniRewardButtonPrefab, List<TierReward> parrTierRewards, float pfStartingx, float pfEndingx, float pfStartingy, float pfEndingy)
    {
        // Place button between bounds
        GameObject uniButtonGameObject;
        RectTransform uniButtonRectTransform;
        float fScreenSegment;
        float fCurrentx;
        if (parrTierRewards != null && parrTierRewards.Count > 0)
        {
            fScreenSegment = (pfEndingx - pfStartingx) / parrTierRewards.Count;
            fCurrentx = pfStartingx;
            foreach (TierReward Reward in parrTierRewards)
            {
                uniButtonGameObject = (GameObject)Instantiate(puniRewardButtonPrefab);
                uniButtonGameObject.transform.SetParent(this.transform);
                uniButtonGameObject.GetComponentInChildren<Text>().text = Reward.RewardName;
                uniButtonRectTransform = uniButtonGameObject.GetComponent<RectTransform>();
                uniButtonRectTransform.anchoredPosition = new Vector3(fCurrentx + (fScreenSegment / 2), pfStartingy + (pfEndingy - pfStartingy) / 2, 0);
                uniButtonRectTransform.localScale = new Vector3(1 / mfXScale, 1 / mfYScale, 1);
                uniButtonGameObject.GetComponent<Button>().onClick.AddListener(() => RewardClicked(Reward));
                gameObject.transform.parent.gameObject.GetComponent<TooltipDisplayController>().AttachTooltipToObject(uniButtonGameObject, Reward.RewardDescription);
                Reward.ButtonObject = uniButtonGameObject;
                arrTierRewardButtons.Add(Reward.RewardName,uniButtonGameObject);
                if((Reward.PreviousRequiredReward!= null && !Reward.PreviousRequiredReward.Unlocked) || Reward.Unlocked)
                {
                    uniButtonGameObject.GetComponent<Image>().color = Color.grey;
                }
                if(fScreenSegment < 2 * uniButtonRectTransform.rect.width)
                {
                    uniButtonRectTransform.localScale.Scale(new Vector3(0.1f, 0.1f, 1));
                }
                // Place child buttons
                PlaceButtons(puniRewardButtonPrefab, Reward.ChildRewards, fCurrentx, fCurrentx + fScreenSegment, pfEndingy, pfEndingy + (pfEndingy - pfStartingy));

                fCurrentx += fScreenSegment;
            }
        }
    }

    /// <summary>
    /// Method to get the max depth of the given reward tree
    /// Used to calculate scaling factors so all rewards fit on screen
    /// </summary>
    /// <param name="parrTierRewards"></param>
    /// <returns></returns>
    private static int GetTreeMaxDepth(List<TierReward> parrTierRewards)
    {
        int intMax = 0;
        int intChildDepth = 0;
        if(parrTierRewards == null || parrTierRewards.Count == 0)
        {
            return 0;
        }
        foreach(TierReward Reward in parrTierRewards)
        {
            intChildDepth = GetTreeMaxDepth(Reward.ChildRewards);
            intMax = intChildDepth > intMax ? intChildDepth : intMax;
        }
        return ++intMax;
    }

    /// <summary>
    /// Callback method attached to each button to attempt to unlock itself
    /// </summary>
    /// <param name="pmusTierReward"></param>
    public void RewardClicked(TierReward pmusTierReward)
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().UnlockReward(pmusTierReward))
        {
            // Grey out self, light up child buttons
            pmusTierReward.ButtonObject.GetComponent<Image>().color = Color.grey;
            foreach(TierReward childReward in pmusTierReward.ChildRewards)
            {
                childReward.ButtonObject.GetComponent<Image>().color = Color.white;
            }
        }
    }
}
