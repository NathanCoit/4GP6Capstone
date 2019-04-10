using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Contains properties for an ability to be unlocked
/// Inherits from TierAbility
/// </summary>
class AbilityTierReward : TierReward
{

    public Ability TierAbility;

    /// <summary>
    /// Constuctor for creating an ability tier reward
    /// Loads the ability reward based on the given ability name
    /// </summary>
    /// <param name="pstrName">The name of the ability to be loaded</param>
    /// <param name="pmusPreviousRequiredTierReward">The previous reward required to unlock this reward. Optional</param>
    public AbilityTierReward(string pstrName, TierReward pmusPreviousRequiredTierReward = null)
    {
        RewardType = REWARDTYPE.Ability;
        RewardName = pstrName;
        PreviousRequiredReward = pmusPreviousRequiredTierReward;
        ChildRewards = new List<TierReward>();
        TierAbility = Ability.LoadAbilityFromName(pstrName);
        RewardDescription = TierAbility.AbilityDescription;
    }
}
