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

    public override string GetRewardDescription()
    {
        string strDescription = "";
        switch (TierAbility.AbiltyType)
        {
            case Ability.ABILITYTYPE.Buff:
                strDescription = string.Format(
@"{0}
{1}
Ability Type: {2}
Buff Type: {3}", TierAbility.AbilityName, RewardDescription, TierAbility.AbiltyType, ((BuffAbility)TierAbility).BuffType);
                break;
            case Ability.ABILITYTYPE.Debuff:
                strDescription = string.Format(
@"{0}
{1}
Ability Type: {2}
Debuff Type: {3}", TierAbility.AbilityName, RewardDescription, TierAbility.AbiltyType, ((DebuffAbility)TierAbility).DebuffType);
                break;
            case Ability.ABILITYTYPE.MultiTarget:
                strDescription = string.Format(
@"{0}
{1}
Ability Type: {2}
Shape: {3}
Base Damage: {4}
Scaling: {5}",
TierAbility.AbilityName,
RewardDescription,
TierAbility.AbiltyType,
((MultiTargetAbility)TierAbility).AbilityShape,
((MultiTargetAbility)TierAbility).AbilityDamage,
((MultiTargetAbility)TierAbility).AbilityScalingDamage);
                break;
            case Ability.ABILITYTYPE.SingleTarget:
                strDescription = string.Format(
@"{0}
{1}
Ability Type: {2}
Base Damage: {3}
Scaling: {4}",
TierAbility.AbilityName,
RewardDescription,
TierAbility.AbiltyType,
((SingleTargetAbility)TierAbility).AbilityDamage,
((SingleTargetAbility)TierAbility).AbilityScalingDamage);
                break;
        }


        return strDescription;
    }
}
