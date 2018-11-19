using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using System.Collections;

public class ManagementModeTests {

    // Edit mode tests for simple scenarios not needing unity physics and multi frame data
	[Test]
	public void FactionOwnsBuildingTest() {
        Faction TestFaction = new Faction("TestGod", Faction.GodType.Duck, 0);
        Building TestBuilding = new Building(Building.BUILDING_TYPE.ALTAR, TestFaction);
        Assert.True(TestFaction.OwnedBuildings.Contains(TestBuilding));
	}

    /// <summary>
    /// MM-2 Upgrade a village from 1 to 2 with enough materials
    /// </summary>
    [Test]
    public void CreateAndUpgradeVillage()
    {
        Faction TestFaction = new Faction("test", Faction.GodType.Fork, 0)
        {
            MaterialCount = 1000
        };
        Building TestBuilding = new Building(Building.BUILDING_TYPE.VILLAGE, TestFaction);
        Assert.True(TestBuilding.UpgradeBuilding());
        Assert.True(TestBuilding.UpgradeLevel == 2);
        Assert.True(TestFaction.MaterialCount == 1000 - Building.CalculateBuildingUpgradeCost(Building.BUILDING_TYPE.VILLAGE));
    }

    /// <summary>
    /// MM-2 upgrade a village from 1 to 2 with no materials
    /// </summary>
    [Test]
    public void CreateAndUpgradeVillageWithNoMaterials()
    {
        Faction TestFaction = new Faction("test", Faction.GodType.Fork, 0);
        Building TestBuilding = new Building(Building.BUILDING_TYPE.VILLAGE, TestFaction);
        Assert.False(TestBuilding.UpgradeBuilding(), string.Format("Materials {0}", TestFaction.MaterialCount));
        Assert.True(TestFaction.MaterialCount == 0);
        Assert.True(TestBuilding.UpgradeLevel == 1);
    }

    /// <summary>
    /// MM-2 Max upgrade level for a village is 3
    /// </summary>
    [Test]
    public void UpgradeVillagePastThree()
    {
        Faction TestFaction = new Faction("test", Faction.GodType.Fork, 0)
        {
            MaterialCount = 1000
        };
        Building TestBuilding = new Building(Building.BUILDING_TYPE.VILLAGE, TestFaction); //1
        Assert.True(TestBuilding.UpgradeBuilding()); //2
        Assert.True(TestBuilding.UpgradeBuilding()); //3
        Assert.False(TestBuilding.UpgradeBuilding());
        Assert.True(TestBuilding.UpgradeLevel == 3);
    }

    [UnityTest]
	public IEnumerator ManagementModeTestsWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
        Assert.True(true);
    }

	[UnityTest]
	public IEnumerator TestStartWithNoMaterials() {
		yield return null;
		Faction TestFaction = new Faction("TestGod", Faction.GodType.Duck, 0);

		//Checks to see if we added one unit
		Assert.True(TestFaction.MaterialCount == 0);
	}

	[UnityTest]
	public IEnumerator TestStartWithNoWorshippers() {
		yield return null;
		Faction TestFaction = new Faction("TestGod", Faction.GodType.Duck, 0);

		//Checks to see if we added one unit
		Assert.True(TestFaction.WorshipperCount == 0);
	}

	[UnityTest]
	public IEnumerator TestStartWithBaseMorale() {
		yield return null;
		Faction TestFaction = new Faction("TestGod", Faction.GodType.Duck, 0);

		//Checks to see if we added one unit
		Assert.True(TestFaction.Morale == 1.0f);
	}
		
	[UnityTest]
	public IEnumerator TestUIShowsResources() {
		yield return null;
		Faction TestFaction = new Faction("TestGod", Faction.GodType.Duck, 0);

		//Checks to see if we added one unit
		Assert.True(TestFaction.Morale == 1.0f);
	}



}
