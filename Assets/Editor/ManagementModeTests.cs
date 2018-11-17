using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class ManagementModeTests {

    // Edit mode tests for simple scenarios not needing unity physics and multi frame data
	[Test]
	public void FactionOwnsBuildingTest() {
        Faction TestFaction = new Faction("TestGod", Faction.GodType.Duck, 0);
        Building TestBuilding = new Building(Building.BUILDING_TYPE.ALTAR, TestFaction);
        Assert.True(TestFaction.OwnedBuildings.Contains(TestBuilding));
	}

	
    // Play mode tests for scenarios that require multi frame data or unity physics
	[UnityTest]
	public IEnumerator ManagementModeTestsWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
        Assert.True(true);
    }
}
