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
    public bool Unlocked = false;

    public Ability TierAbility;
    //public Buff TierBuff
    //public Resource TierResource

    // Create an ability tier reward
    public TierReward(int pintTierLevel, string pstrName, string pstrDesc, TierReward pTierRewardPreviousRequired = null)
    {
        RewardType = REWARDTYPE.Ability;
        TierLevel = pintTierLevel;
        RewardName = pstrName;
        RewardDescription = pstrDesc;
        PreviousRequiredReward = pTierRewardPreviousRequired;
        TierAbility = new Ability(pstrName, pstrDesc);
    }

    // Create a Buff tier reward

    // Create a Resource Tier reward

    public Ability UnlockAbility()
    {
        // If you haven't unlocked the previous tier or this isnt an ability reward, return nothing
        if ((PreviousRequiredReward != null && !PreviousRequiredReward.Unlocked) || RewardType != REWARDTYPE.Ability)
        {
            return null;
        }
        return TierAbility;
    }
}
