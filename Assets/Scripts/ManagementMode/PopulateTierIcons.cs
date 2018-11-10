using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateTierIcons : MonoBehaviour
{
    private float Scale = 0;
    private List<GameObject> Buttons;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void InitializeButtons(List<TierReward> TierRewards)
    {
        Buttons = new List<GameObject>();
        UnityEngine.Object btn = Resources.Load("Button");
        int MaxRewardDepth = GetTreeMaxDepth(TierRewards);
        float startingx = 0 - (Screen.width / 2);
        float startingy = 0 - (Screen.height / 2);
        float endingx = Screen.width / 2;
        float endingy = Screen.height / 2;
        endingy = startingy + (endingy - startingy) / MaxRewardDepth;
        // Change scale based on screen size, should be able to fit 
        Scale = ((MaxRewardDepth * TierRewards.Count) / 8) + 1;

        // Place buttons
        PlaceButtons(btn, TierRewards, startingx, endingx, startingy, endingy);

        // Scale UI
        GetComponent<RectTransform>().localScale = new Vector3(Scale, Scale, 1);
    }

    /// <summary>
    /// Recursion Fun
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="TierRewards"></param>
    /// <param name="startingx"></param>
    /// <param name="endingx"></param>
    /// <param name="startingy"></param>
    /// <param name="endingy"></param>
    private void PlaceButtons(UnityEngine.Object btn, List<TierReward> TierRewards, float startingx, float endingx, float startingy, float endingy)
    {
        // Place button between bounds
        GameObject Button;
        float segment;
        float currentx;
        if (TierRewards != null && TierRewards.Count > 0)
        {
            segment = (endingx - startingx) / TierRewards.Count;
            currentx = startingx;
            foreach (TierReward Reward in TierRewards)
            {
                Button = (GameObject)Instantiate(btn);
                Button.transform.SetParent(this.transform);
                Button.GetComponentInChildren<Text>().text = Reward.RewardName;
                Button.GetComponent<RectTransform>().anchoredPosition = new Vector3(currentx + (segment / 2), startingy + (endingy - startingy) / 2, 0);
                Button.GetComponent<RectTransform>().localScale = new Vector3(1 / Scale, 1 / Scale, 1 / Scale);
                Button.GetComponent<Button>().onClick.AddListener(() => RewardClicked(Reward));
                Reward.ButtonObject = Button;
                Buttons.Add(Button);
                if(Reward.PreviousRequiredReward!= null && !Reward.PreviousRequiredReward.Unlocked)
                {
                    Button.SetActive(false);
                }
                // Place child buttons
                PlaceButtons(btn, Reward.ChildRewards, currentx, currentx + segment, endingy, endingy + (endingy - startingy));

                currentx += segment;
            }
        }
    }

    private static int GetTreeMaxDepth(List<TierReward> testRewards)
    {
        int Max = 0;
        int ChildDepth = 0;
        if(testRewards == null || testRewards.Count == 0)
        {
            return 0;
        }
        foreach(TierReward Reward in testRewards)
        {
            ChildDepth = GetTreeMaxDepth(Reward.ChildRewards);
            Max = ChildDepth > Max ? ChildDepth : Max;
        }
        return ++Max;
    }

    void RewardClicked(TierReward reward)
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().UnlockReward(reward))
        {
            // Grey out self, light up child buttons
            reward.ButtonObject.SetActive(false);
            foreach(TierReward childReward in reward.ChildRewards)
            {
                childReward.ButtonObject.SetActive(true);
            }
        }
    }
}
