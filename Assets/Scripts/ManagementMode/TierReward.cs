using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TierReward{
    public enum REWARDTYPE
    {
        Ability,
        Buff,
        Resource
    }

    public REWARDTYPE RewardType;
    public int TierLevel;
    public string RewardName;
    public string RewardDescription;
    public TierReward PreviousRequiredReward;
    public List<TierReward> ChildRewards;
    public bool Unlocked = false;
    public GameObject ButtonObject;

    public Ability TierAbility;
    //public Buff TierBuff
    //public Resource TierResource

    // Create an ability tier reward
    public TierReward(string pstrName, string pstrDesc, TierReward pTierRewardPreviousRequired = null)
    {
        RewardType = REWARDTYPE.Ability;
        RewardName = pstrName;
        RewardDescription = pstrDesc;
        PreviousRequiredReward = pTierRewardPreviousRequired;
        ChildRewards = new List<TierReward>();
        TierAbility = new Ability(pstrName, pstrDesc);
    }

    // Create a Buff tier reward

    // Create a Resource Tier reward

    public Ability UnlockAbility()
    {
        return TierAbility;
    }
}
