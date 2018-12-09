using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        TierAbility = Ability.LoadAbilityFromName(pstrName);
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

    public static List<string> SaveRewardTree(List<TierReward> rewards)
    {
        List<string> savedRewards = new List<string>();
        List<string> childRewards = null;
        foreach (TierReward reward in rewards)
        {
            if (reward.Unlocked)
            {
                savedRewards.Add(reward.RewardName);
                childRewards = SaveRewardTree(reward.ChildRewards);
                if (childRewards != null)
                {
                    foreach (string savedReward in childRewards)
                    {
                        savedRewards.Add(savedReward);
                    }
                }
            }
        }
        return savedRewards;
    }

    public static TierReward FindRewardByName(string RewardName, List<TierReward> Rewards)
    {
        TierReward Found = null;
        if (Rewards != null)
        {
            foreach (TierReward reward in Rewards)
            {
                if (reward.RewardName.Equals(RewardName))
                {
                    return reward;
                }
                Found = FindRewardByName(RewardName, reward.ChildRewards);
                if (Found != null)
                {
                    return Found;
                }
            }
        }

        return null;
    }

    public static List<TierReward> CreateTierRewardTree(Faction.GodType godType)
    {
        List<TierReward> playerRewards = null;
        switch (godType)
        {
            case Faction.GodType.Mushrooms:
                playerRewards = CreateMushroomRewardTree();
                break;
            case Faction.GodType.Ducks:
                playerRewards = CreateDuckRewardTree();
                break;
            case Faction.GodType.Shoes:
                playerRewards = CreateShoeRewardTree();
                break;
            case Faction.GodType.Forks:
                playerRewards = CreateForkRewardTree();
                break;
        }
        return playerRewards;
    }
    public static List<TierReward> CreateDuckRewardTree()
    {
        TierReward BasePlayerTierReward;
        TierReward NextPlayerTierReward;
        List<TierReward> PlayerRewardTree = new List<TierReward>();

        // First tier, player gets this tier upon game start
        BasePlayerTierReward = new TierReward("Quack");
        PlayerRewardTree.Add(BasePlayerTierReward);

        // Second tier, unlocked at 1 * TierCount (100)
        NextPlayerTierReward = new TierReward("Quack!!", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        BasePlayerTierReward = NextPlayerTierReward;
        
        NextPlayerTierReward = new TierReward("Quack?", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("Quack¿", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        BasePlayerTierReward = new TierReward("Materials", "100 Materials", TierReward.RESOURCETYPE.Material, 100);
        PlayerRewardTree.Add(BasePlayerTierReward);

        NextPlayerTierReward = new TierReward("2xMat", "2x material growth", TierReward.RESOURCETYPE.Material, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("2xWorshipper", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        BasePlayerTierReward = NextPlayerTierReward;

        NextPlayerTierReward = new TierReward("2xWorshipperAgain", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("MOARWorshippers", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        return PlayerRewardTree;
    }
    public static List<TierReward> CreateShoeRewardTree()
    {
        TierReward BasePlayerTierReward;
        TierReward NextPlayerTierReward;
        List<TierReward> PlayerRewardTree = new List<TierReward>();

        // First tier, player gets this tier upon game start
        BasePlayerTierReward = new TierReward("Kick");
        PlayerRewardTree.Add(BasePlayerTierReward);

        // Second tier, unlocked at 1 * TierCount (100)
        NextPlayerTierReward = new TierReward("Yeezys", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        BasePlayerTierReward = NextPlayerTierReward;
        
        NextPlayerTierReward = new TierReward("Leg Sweep", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("Broken Ankles", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        BasePlayerTierReward = new TierReward("Materials", "100 Materials", TierReward.RESOURCETYPE.Material, 100);
        PlayerRewardTree.Add(BasePlayerTierReward);

        NextPlayerTierReward = new TierReward("2xMat", "2x material growth", TierReward.RESOURCETYPE.Material, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("2xWorshipper", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        BasePlayerTierReward = NextPlayerTierReward;

        NextPlayerTierReward = new TierReward("2xWorshipperAgain", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("MOARWorshippers", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        return PlayerRewardTree;
    }
    public static List<TierReward> CreateForkRewardTree()
    {
        TierReward BasePlayerTierReward;
        TierReward NextPlayerTierReward;
        List<TierReward> PlayerRewardTree = new List<TierReward>();

        // First tier, player gets this tier upon game start
        BasePlayerTierReward = new TierReward("Throw Fork");
        PlayerRewardTree.Add(BasePlayerTierReward);

        // Second tier, unlocked at 1 * TierCount (100)
        NextPlayerTierReward = new TierReward("Fork Sweep", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        BasePlayerTierReward = NextPlayerTierReward;
        
        NextPlayerTierReward = new TierReward("Eat Spaghett", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("Fork Flash", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        BasePlayerTierReward = new TierReward("Materials", "100 Materials", TierReward.RESOURCETYPE.Material, 100);
        PlayerRewardTree.Add(BasePlayerTierReward);

        NextPlayerTierReward = new TierReward("2xMat", "2x material growth", TierReward.RESOURCETYPE.Material, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("2xWorshipper", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        BasePlayerTierReward = NextPlayerTierReward;

        NextPlayerTierReward = new TierReward("2xWorshipperAgain", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("MOARWorshippers", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        return PlayerRewardTree;
    }

    public static List<TierReward> CreateMushroomRewardTree()
    {
        TierReward BasePlayerTierReward;
        TierReward NextPlayerTierReward;
        List<TierReward> PlayerRewardTree = new List<TierReward>();

        // First tier, player gets this tier upon game start
        BasePlayerTierReward = new TierReward("Throw Mushroom");
        PlayerRewardTree.Add(BasePlayerTierReward);

        // Second tier, unlocked at 1 * TierCount (100)
        NextPlayerTierReward = new TierReward("Eat Mushroom", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        BasePlayerTierReward = NextPlayerTierReward;
        
        NextPlayerTierReward = new TierReward("Mushroom Laser", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("Spread Spores", BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        BasePlayerTierReward = new TierReward("Materials", "100 Materials", TierReward.RESOURCETYPE.Material, 100);
        PlayerRewardTree.Add(BasePlayerTierReward);

        NextPlayerTierReward = new TierReward("2xMat", "2x material growth", TierReward.RESOURCETYPE.Material, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("2xWorshipper", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        BasePlayerTierReward = NextPlayerTierReward;

        NextPlayerTierReward = new TierReward("2xWorshipperAgain", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);

        NextPlayerTierReward = new TierReward("MOARWorshippers", "2x worshipper growth", TierReward.RESOURCETYPE.Worshipper, 2.0f, BasePlayerTierReward);
        BasePlayerTierReward.ChildRewards.Add(NextPlayerTierReward);
        return PlayerRewardTree;
    }
}
