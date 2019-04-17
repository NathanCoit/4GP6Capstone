using System;
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
    [System.Serializable]
    public struct NamedSprite
    {
        public string Name;
        public Sprite sprite;
    }
    public Color UnlockedColor;
    public Color AvailableColor;
    public Color UnavailableColor;

    public List<NamedSprite> ButtonSprites;

    private Dictionary<string, Sprite> mdictButtonSprites;
    private int mintMaxRewardDepth = 0;
    public Dictionary<string, GameObject> arrTierRewardButtons;
    public TooltipDisplayController RewardTooltipController;
    public GameObject HorizontalContainerPrefab;
    public GameObject VerticalContainerPrefab;
    public GameObject RewardButtonPrefab;
    public GameObject LinePrefab;
    private List<GameObject> marrLines;
    private bool mblnDrawLines;
    private List<TierReward> marrTierRewards;

    private void Awake()
    {
        mdictButtonSprites = new Dictionary<string, Sprite>();
        foreach (NamedSprite musSprite in ButtonSprites)
        {
            mdictButtonSprites.Add(musSprite.Name, musSprite.sprite);
        }
    }

    private void Update()
    {
        // Forced to put this 1 frame after awakes/starts are called
        // as buttons have not had their positions calculated until reward panel is first
        // activated.
        if(mblnDrawLines)
        {
            DrawAllUILines();
        }
    }

    /// <summary>
    /// Called from game manager at the start of a game.
    /// Creates and places the tier reward buttons based on the given reward tree.
    /// </summary>
    /// <param name="parrTierRewards"></param>
    public void InitializeButtons(List<TierReward> parrTierRewards)
    {
        arrTierRewardButtons = new Dictionary<string, GameObject>();
        mintMaxRewardDepth = GetTreeMaxDepth(parrTierRewards);
        // Place buttons
        PlaceButtons(parrTierRewards, gameObject);
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
    private void PlaceButtons(List<TierReward> parrTierRewards, GameObject puniParentObject)
    {
        // Place button between bounds
        GameObject uniRewardPrefab;
        GameObject uniHorizontalContainer;
        GameObject uniVerticalContainer;
        Button uniBtn;
        GameObject uniButtonObject;
        // Add horizontal layout group to parent
        uniHorizontalContainer = Instantiate(HorizontalContainerPrefab);
        uniHorizontalContainer.transform.SetParent(puniParentObject.transform, false);
        uniHorizontalContainer.GetComponent<RectTransform>().SetAsFirstSibling();

        foreach (TierReward musReward in parrTierRewards)
        {
            // Add a vertical layout group to the horizontal
            uniVerticalContainer = Instantiate(VerticalContainerPrefab);
            uniVerticalContainer.transform.SetParent(uniHorizontalContainer.transform, false);
            uniVerticalContainer.GetComponent<RectTransform>().SetAsFirstSibling();

            // Place button in vertical layout group
            uniRewardPrefab = Instantiate(RewardButtonPrefab);
            uniRewardPrefab.transform.SetParent(uniVerticalContainer.transform, false);

            // Set button properties
            uniBtn = uniRewardPrefab.GetComponentInChildren<Button>();
            uniBtn.onClick.AddListener(() => RewardClicked(musReward));
            uniButtonObject = uniRewardPrefab.transform.GetChild(0).gameObject;
            uniButtonObject.transform.GetChild(0).GetComponentInChildren<Image>().sprite = GetSpriteForReward(musReward);
            RewardTooltipController.AttachTooltipToObject(uniButtonObject, musReward.GetRewardDescription());
            musReward.ButtonObject = uniButtonObject;
            arrTierRewardButtons.Add(musReward.RewardName, uniButtonObject);

            // Disable rewards that can't be unlocked yet or have already been unlocked
            if ((musReward.PreviousRequiredReward != null && !musReward.PreviousRequiredReward.Unlocked))
            {
                uniButtonObject.GetComponent<Image>().color = UnavailableColor;
            }
            else if (musReward.Unlocked)
            {
                uniButtonObject.GetComponent<Image>().color = UnlockedColor;
            }
            else
            {
                uniButtonObject.GetComponent<Image>().color = AvailableColor;
            }
            // Place child buttons with vertical as parent
            PlaceButtons(musReward.ChildRewards, uniVerticalContainer);
        }
    }

    private Sprite GetSpriteForReward(TierReward pmusReward)
    {
        Sprite uniButtonSprite = null;
        switch (pmusReward.RewardType)
        {
            case TierReward.REWARDTYPE.Ability:
                switch (((AbilityTierReward)pmusReward).TierAbility.AbiltyType)
                {
                    case Ability.ABILITYTYPE.Buff:
                        uniButtonSprite = mdictButtonSprites["Buff"];
                        break;
                    case Ability.ABILITYTYPE.Debuff:
                        uniButtonSprite = mdictButtonSprites["Debuff"];
                        break;
                    case Ability.ABILITYTYPE.MultiTarget:
                        uniButtonSprite = mdictButtonSprites["Multi"];
                        break;
                    case Ability.ABILITYTYPE.SingleTarget:
                        uniButtonSprite = mdictButtonSprites["Single"];
                        break;
                }
                break;
            case TierReward.REWARDTYPE.Resource:
                switch (((ResourceTierReward)pmusReward).ResourceType)
                {
                    case TierReward.RESOURCETYPE.Material:
                        uniButtonSprite = mdictButtonSprites["Mat"];
                        break;
                    case TierReward.RESOURCETYPE.Worshipper:
                        uniButtonSprite = mdictButtonSprites["Wor"];
                        break;
                }
                break;
            case TierReward.REWARDTYPE.ResourceMultiplier:
                switch (((ResourceMultiplierTierReward)pmusReward).ResourceType)
                {
                    case TierReward.RESOURCETYPE.Material:
                        uniButtonSprite = mdictButtonSprites["Matx"];
                        break;
                    case TierReward.RESOURCETYPE.Worshipper:
                        uniButtonSprite = mdictButtonSprites["Worx"];
                        break;
                }
                break;
        }
        if (uniButtonSprite == null)
        {
            uniButtonSprite = mdictButtonSprites["Default"];
        }
        return uniButtonSprite;
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
        if (parrTierRewards == null || parrTierRewards.Count == 0)
        {
            return 0;
        }
        foreach (TierReward Reward in parrTierRewards)
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
            pmusTierReward.ButtonObject.GetComponent<Image>().color = UnlockedColor;
            foreach (TierReward childReward in pmusTierReward.ChildRewards)
            {
                childReward.ButtonObject.GetComponent<Image>().color = AvailableColor;
            }
        }
    }
    public void DrawAllUILines(List<TierReward> parrTierRewards)
    {
        mblnDrawLines = true;
        marrTierRewards = parrTierRewards;
    }
    private void DrawAllUILines()
    {
        if (marrLines != null)
        {
            DestroyAllLines(marrLines);
        }
        marrLines = new List<GameObject>();
        DrawLinesRecursive(marrTierRewards);
        mblnDrawLines = false;
    }

    private void DestroyAllLines(List<GameObject> parrLines)
    {
        foreach(GameObject uniLine in parrLines)
        {
            Destroy(uniLine);
        }
    }

    private void DrawLinesRecursive(List<TierReward> parrTierRewards)
    {
        foreach (TierReward musReward in parrTierRewards)
        {
            if (musReward.PreviousRequiredReward != null)
            {
                Vector3 a = musReward.ButtonObject.transform.parent.localPosition;
                DrawUILine(musReward.ButtonObject, musReward.PreviousRequiredReward.ButtonObject);
            }
            if (musReward.ChildRewards != null && musReward.ChildRewards.Count > 0)
            {
                DrawLinesRecursive(musReward.ChildRewards);
            }
        }
    }

    private void DrawUILine(GameObject puniObjectA, GameObject puniObjectB)
    {
        RectTransform uniRectA = puniObjectA.GetComponent<RectTransform>();
        RectTransform uniRectB = puniObjectB.GetComponent<RectTransform>();
        // Get centre point

        Vector3 pointA = uniRectA.position;
        Vector3 pointB = uniRectB.position;

        RectTransform lineRect = Instantiate(LinePrefab).GetComponent<RectTransform>();
        lineRect.SetParent(transform, false);
        lineRect.SetAsFirstSibling();
        //PointA and PointB determined before this
        Vector3 midpoint = (pointA + pointB) / 2; //used to position line
        float pointDistance = Vector3.Distance(pointA, pointB); //used for height of line

        //really need to figure out what all this does one of these days. Not even my first game using it...
        float angle = Mathf.Atan2(pointB.x - pointA.x, pointA.y - pointB.y);
        if (angle < 0.0) { angle += Mathf.PI * 2; }
        angle *= Mathf.Rad2Deg;

        float lineWidth = 5;
        lineRect.sizeDelta = new Vector2(lineWidth, pointDistance);
        lineRect.rotation = Quaternion.Euler(0, 0, angle); //rotate around the mid point
        lineRect.transform.position = midpoint;
        lineRect.transform.localScale = new Vector2(1 / uniRectA.lossyScale.x, 1 / uniRectA.lossyScale.y);
        marrLines.Add(lineRect.gameObject);
    }
}
