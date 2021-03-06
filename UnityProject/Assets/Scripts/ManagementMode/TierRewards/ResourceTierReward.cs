﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Contains properties for a resource amount to be unlocked. Inherits from TierAbility
/// </summary>
class ResourceTierReward : TierReward
{
    public int Amount = 0;
    public RESOURCETYPE ResourceType;

    /// <summary>
    /// Constructor for creating a resource amount reward
    /// </summary>
    /// <param name="pstrName">The name of the reward displayed to the player.</param>
    /// <param name="pstrDesc">A descrition of the reward. Will be displayed on the reward tree.</param>
    /// <param name="penumType">The resource type to be rewarded</param>
    /// <param name="pintAmount">The amount of resource rewarded.</param>
    /// <param name="pmusPreviousRequiredTierReward">The previous reward required to unlock this reward. Optional</param>
    public ResourceTierReward(string pstrName, string pstrDesc, RESOURCETYPE penumType, int pintAmount, TierReward pmusPreviousRequiredTierReward = null) 
        : base()
    {
        RewardType = REWARDTYPE.Resource;
        RewardName = pstrName;
        RewardDescription = pstrDesc;
        ResourceType = penumType;
        Amount = pintAmount;
        PreviousRequiredReward = pmusPreviousRequiredTierReward;
        ChildRewards = new List<TierReward>();
    }

    public override string GetRewardDescription()
    {
        string strDescription = 
string.Format(
@"{0}
Material Type: {1}
Amount: {2}", RewardDescription, RewardType, Amount);


        return strDescription;
    }
}
