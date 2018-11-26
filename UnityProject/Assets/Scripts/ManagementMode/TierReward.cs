using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TierReward{
    public enum REWARDTYPE
    {
        Ability,
        Buff,
        Resource,
        ResourceMultiplier
    }

    public enum RESOURCETYPE
    {
        Worshipper,
        Material
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

    public RESOURCETYPE ResourceType;
    public int Amount = 0;
    public float Multiplier = 0;
    //public Buff TierBuff

    // Create an ability tier reward
    public TierReward(string pstrName, TierReward pTierRewardPreviousRequired = null)
    {
        RewardType = REWARDTYPE.Ability;
        RewardName = pstrName;
        PreviousRequiredReward = pTierRewardPreviousRequired;
        ChildRewards = new List<TierReward>();
        TierAbility = GameInfo.LoadAbility(pstrName);
        RewardDescription = TierAbility.AbilityDescription;
    }

    // Create a Buff tier reward

    // Tier reward for flat resource injections
    public TierReward(string pstrName, string pstrDesc, RESOURCETYPE type, int amount, TierReward pTierRewardPreviousRequired = null)
    {
        RewardType = REWARDTYPE.Resource;
        RewardName = pstrName;
        RewardDescription = pstrDesc;
        ResourceType = type;
        Amount = amount;
        PreviousRequiredReward = pTierRewardPreviousRequired;
        ChildRewards = new List<TierReward>();
    }

    // Tier reward for resource multiplier
    public TierReward(string pstrName, string pstrDesc, RESOURCETYPE type, float multiplier, TierReward pTierRewardPreviousRequired = null)
    {
        RewardType = REWARDTYPE.ResourceMultiplier;
        RewardName = pstrName;
        RewardDescription = pstrDesc;
        ResourceType = type;
        Multiplier = multiplier;
        PreviousRequiredReward = pTierRewardPreviousRequired;
        ChildRewards = new List<TierReward>();
    }

    public Ability UnlockAbility()
    {
        return TierAbility;
    }
}
