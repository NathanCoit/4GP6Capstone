using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class holding information about a tier reward for the player
/// Tier rewards can be unlocked to give the player different rewards
/// Tier rewards will be linked to the reward tree for each god. Rewards must have unique names for lookup/identification.
/// </summary>
public class TierReward
{
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

    /// <summary>
    /// Default Tier Reward constructor, called on creation of any tier reward type
    /// </summary>
    public TierReward()
    {

    }

    /// <summary>
    /// Serialize the given tier reward tree into a list of strings to save to file.
    /// </summary>
    /// <param name="parrRewardsToSave"></param>
    /// <returns></returns>
    public static List<string> SaveRewardTree(List<TierReward> parrRewardsToSave)
    {
        List<string> arrSavedRewards = new List<string>();
        List<string> arrChildRewards = null;
        foreach (TierReward musTierReward in parrRewardsToSave)
        {
            if (musTierReward.Unlocked)
            {
                arrSavedRewards.Add(musTierReward.RewardName);
                arrChildRewards = SaveRewardTree(musTierReward.ChildRewards);
                if (arrChildRewards != null)
                {
                    foreach (string strSavedReward in arrChildRewards)
                    {
                        arrSavedRewards.Add(strSavedReward);
                    }
                }
            }
        }
        return arrSavedRewards;
    }

    /// <summary>
    /// Parse the entire tier reward tree to find a matching reward by reward name
    /// Used to show which abilites are unlocked on return from save state
    /// Recursive tree search
    /// </summary>
    /// <param name="pstrRewardName"></param>
    /// <param name="parrTierRewards"></param>
    /// <returns></returns>
    public static TierReward FindRewardByName(string pstrRewardName, List<TierReward> parrTierRewards)
    {
        TierReward musFoundTierReward = null;
        if (parrTierRewards != null)
        {
            foreach (TierReward musTierReward in parrTierRewards)
            {
                if (musTierReward.RewardName.Equals(pstrRewardName))
                {
                    return musTierReward;
                }
                musFoundTierReward = FindRewardByName(pstrRewardName, musTierReward.ChildRewards);
                if (musFoundTierReward != null)
                {
                    return musFoundTierReward;
                }
            }
        }
        // Reward not found
        return null;
    }

    /// <summary>
    /// Create an initial tier reward tree based on the given god type.
    /// Allows for the same tier reward tree on each play through for balancing.
    /// Can be moved to/loaded from file for more modularity in future.
    /// </summary>
    /// <param name="penumGodType"></param>
    /// <returns></returns>
    public static List<TierReward> CreateTierRewardTree(Faction.GodType penumGodType)
    {
        List<TierReward> arrPlayerTierRewardTree = null;
        switch (penumGodType)
        {
            case Faction.GodType.Mushrooms:
                arrPlayerTierRewardTree = CreateMushroomRewardTree();
                break;
            case Faction.GodType.Ducks:
                arrPlayerTierRewardTree = CreateDuckRewardTree();
                break;
            case Faction.GodType.Shoes:
                arrPlayerTierRewardTree = CreateShoeRewardTree();
                break;
            case Faction.GodType.Forks:
                arrPlayerTierRewardTree = CreateForkRewardTree();
                break;
        }
        return arrPlayerTierRewardTree;
    }

    /// <summary>
    /// Create the initial reward tree for the god of ducks
    /// </summary>
    /// <returns></returns>
    public static List<TierReward> CreateDuckRewardTree()
    {
        TierReward musBasePlayerTierReward;
        TierReward musNextPlayerTierReward;
        List<TierReward> arrPlayerRewardTree = new List<TierReward>();

        // First tier, player gets this tier upon game start
        musBasePlayerTierReward = new AbilityTierReward("Quack");
        arrPlayerRewardTree.Add(musBasePlayerTierReward);

        // Second tier, unlocked at 1 * TierCount (100)
        musNextPlayerTierReward = new AbilityTierReward("Quack!!", musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);
        musBasePlayerTierReward = musNextPlayerTierReward;
        
        musNextPlayerTierReward = new AbilityTierReward("Quack?", musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musNextPlayerTierReward = new AbilityTierReward("Quack¿", musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musBasePlayerTierReward = new ResourceTierReward("Materials", "100 Materials", TierReward.RESOURCETYPE.Material, 100);
        arrPlayerRewardTree.Add(musBasePlayerTierReward);

        musNextPlayerTierReward = new ResourceMultiplierTierReward("2xMat", "2x material growth", TierReward.RESOURCETYPE.Material, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musNextPlayerTierReward = new ResourceMultiplierTierReward("2xWorshipper", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);
        musBasePlayerTierReward = musNextPlayerTierReward;

        musNextPlayerTierReward = new ResourceMultiplierTierReward("2xWorshipperAgain", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musNextPlayerTierReward = new ResourceMultiplierTierReward("MOARWorshippers", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);
        return arrPlayerRewardTree;
    }

    /// <summary>
    /// Create the initial tier reward tree for the god of shoes
    /// </summary>
    /// <returns></returns>
    public static List<TierReward> CreateShoeRewardTree()
    {
        TierReward musBasePlayerTierReward;
        TierReward musNextPlayerTierReward;
        List<TierReward> arrPlayerRewardTree = new List<TierReward>();

        // First tier, player gets this tier upon game start
        musBasePlayerTierReward = new AbilityTierReward("Kick");
        arrPlayerRewardTree.Add(musBasePlayerTierReward);

        // Second tier, unlocked at 1 * TierCount (100)
        musNextPlayerTierReward = new AbilityTierReward("Yeezys", musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);
        musBasePlayerTierReward = musNextPlayerTierReward;
        
        musNextPlayerTierReward = new AbilityTierReward("Leg Sweep", musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musNextPlayerTierReward = new AbilityTierReward("Broken Ankles", musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musBasePlayerTierReward = new ResourceTierReward("Materials", "100 Materials", TierReward.RESOURCETYPE.Material, 100);
        arrPlayerRewardTree.Add(musBasePlayerTierReward);

        musNextPlayerTierReward = new ResourceMultiplierTierReward("2xMat", "2x material growth", TierReward.RESOURCETYPE.Material, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musNextPlayerTierReward = new ResourceMultiplierTierReward("2xWorshipper", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);
        musBasePlayerTierReward = musNextPlayerTierReward;

        musNextPlayerTierReward = new ResourceMultiplierTierReward("2xWorshipperAgain", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musNextPlayerTierReward = new ResourceMultiplierTierReward("MOARWorshippers", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);
        return arrPlayerRewardTree;
    }

    /// <summary>
    /// Create the initial tier reward tree for the god of forks
    /// </summary>
    /// <returns></returns>
    public static List<TierReward> CreateForkRewardTree()
    {
        TierReward musBasePlayerTierReward;
        TierReward musNextPlayerTierReward;
        List<TierReward> arrPlayerRewardTree = new List<TierReward>();

        // First tier, player gets this tier upon game start
        musBasePlayerTierReward = new AbilityTierReward("Throw Fork");
        arrPlayerRewardTree.Add(musBasePlayerTierReward);

        // Second tier, unlocked at 1 * TierCount (100)
        musNextPlayerTierReward = new AbilityTierReward("Fork Sweep", musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);
        musBasePlayerTierReward = musNextPlayerTierReward;
        
        musNextPlayerTierReward = new AbilityTierReward("Eat Spaghett", musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musNextPlayerTierReward = new AbilityTierReward("Fork Flash", musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musBasePlayerTierReward = new ResourceTierReward("Materials", "100 Materials", TierReward.RESOURCETYPE.Material, 100);
        arrPlayerRewardTree.Add(musBasePlayerTierReward);

        musNextPlayerTierReward = new ResourceMultiplierTierReward("2xMat", "2x material growth", TierReward.RESOURCETYPE.Material, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musNextPlayerTierReward = new ResourceMultiplierTierReward("2xWorshipper", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);
        musBasePlayerTierReward = musNextPlayerTierReward;

        musNextPlayerTierReward = new ResourceMultiplierTierReward("2xWorshipperAgain", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musNextPlayerTierReward = new ResourceMultiplierTierReward("MOARWorshippers", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);
        return arrPlayerRewardTree;
    }

    /// <summary>
    /// Create the initial tier reward tree for the god of mushrooms
    /// </summary>
    /// <returns></returns>
    public static List<TierReward> CreateMushroomRewardTree()
    {
        TierReward musBasePlayerTierReward;
        TierReward musNextPlayerTierReward;
        List<TierReward> arrPlayerRewardTree = new List<TierReward>();

        // First tier, player gets this tier upon game start
        musBasePlayerTierReward = new AbilityTierReward("Throw Mushroom");
        arrPlayerRewardTree.Add(musBasePlayerTierReward);

        // Second tier, unlocked at 1 * TierCount (100)
        musNextPlayerTierReward = new AbilityTierReward("Eat Mushroom", musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);
        musBasePlayerTierReward = musNextPlayerTierReward;
        
        musNextPlayerTierReward = new AbilityTierReward("Mushroom Laser", musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musNextPlayerTierReward = new AbilityTierReward("Spread Spores", musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musBasePlayerTierReward = new ResourceTierReward("Materials", "100 Materials", TierReward.RESOURCETYPE.Material, 100);
        arrPlayerRewardTree.Add(musBasePlayerTierReward);

        musNextPlayerTierReward = new ResourceMultiplierTierReward("2xMat", "2x material growth", TierReward.RESOURCETYPE.Material, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musNextPlayerTierReward = new ResourceMultiplierTierReward("2xWorshipper", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);
        musBasePlayerTierReward = musNextPlayerTierReward;

        musNextPlayerTierReward = new ResourceMultiplierTierReward("2xWorshipperAgain", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);

        musNextPlayerTierReward = new ResourceMultiplierTierReward("MOARWorshippers", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, musBasePlayerTierReward);
        musBasePlayerTierReward.ChildRewards.Add(musNextPlayerTierReward);
        return arrPlayerRewardTree;
    }

    public virtual string GetRewardDescription()
    {
        string strDescription = "Default reward. You shouldn't see this...";
        return strDescription;
    }
}
