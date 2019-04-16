using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Contains properties for a resource multiplier to be unlocked. Inherits from TierAbility
/// </summary>
class ResourceMultiplierTierReward : TierReward
{
    public float Multiplier = 0;
    public RESOURCETYPE ResourceType;

    /// <summary>
    /// Constructor for creating a resource multiplier reward.
    /// </summary>
    /// <param name="pstrName">The name of the reward displayed to the player.</param>
    /// <param name="pstrDesc">A descrition of the reward. Will be displayed on the reward tree.</param>
    /// <param name="penumType">The resource type to be rewarded</param>
    /// <param name="pfMultiplier">The multipler of resource to be added.</param>
    /// <param name="pmusPreviousRequiredTierReward">The previous reward required to unlock this reward. Optional</param>
    public ResourceMultiplierTierReward(string pstrName, string pstrDesc, RESOURCETYPE penumType, float pfMultiplier, TierReward pmusPreviousRequiredTierReward = null)
    {
        RewardType = REWARDTYPE.ResourceMultiplier;
        RewardName = pstrName;
        RewardDescription = pstrDesc;
        ResourceType = penumType;
        Multiplier = pfMultiplier;
        PreviousRequiredReward = pmusPreviousRequiredTierReward;
        ChildRewards = new List<TierReward>();
    }

    public override string GetRewardDescription()
    {
        string strDescription =
string.Format(
@"{0}
Material Type: {1}
Amount: {2}", RewardDescription, RewardType, Multiplier);


        return strDescription;
    }
}