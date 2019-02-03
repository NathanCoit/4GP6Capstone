using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class AbilityTierReward : TierReward
{

    public Ability TierAbility;

    /// <summary>
    /// Constuctor for creating an ability tier reward
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
