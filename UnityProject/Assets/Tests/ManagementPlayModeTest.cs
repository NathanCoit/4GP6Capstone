using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagementPlayModeTest {

    [Test]
    public void ManagementPlayModeTestSimplePasses() {
        // Use the Assert class to test conditions.
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator ManagementPlayModeTestWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }

    //tests camera moving right on d key press
    [UnityTest]
    public IEnumerator MM1MoveCameraRightOnDKey() {

        if (GameObject.Find("GameInfo") != null) {
            GameObject.Destroy(GameObject.Find("GameInfo"));
        }

        SceneManager.LoadScene("UnderGodScene");

        //Always give it a sec
        yield return null;
		yield return null;

        Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

        Vector3 firstPos = testcam.transform.position;

        testcam.rightHeld = true;

        yield return null;
        testcam.rightHeld = false;

        Vector3 secondPos = testcam.transform.position;

        //Checks to see if we added one unit
        Assert.True(firstPos != secondPos);
    }

    // tests camera moving left on 'a' key press
    [UnityTest]
    public IEnumerator MM1MoveCameraLeftOnAKey() {

        if (GameObject.Find("GameInfo") != null) {
            GameObject.Destroy(GameObject.Find("GameInfo"));
        }

        SceneManager.LoadScene("UnderGodScene");

        //Always give it a sec
        yield return null;
		yield return null;

        Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

        Vector3 firstPos = testcam.transform.position;

        testcam.leftHeld = true;

        yield return null;
        testcam.leftHeld = false;

        Vector3 secondPos = testcam.transform.position;

        //Checks to see if we added one unit
        Assert.True(firstPos != secondPos);
    }

    // tests camera moving up o w key press
    [UnityTest]
    public IEnumerator MM1MoveCameraUpOnWKey() {

        if (GameObject.Find("GameInfo") != null) {
            GameObject.Destroy(GameObject.Find("GameInfo"));
        }

        SceneManager.LoadScene("UnderGodScene");

        //Always give it a sec
        yield return null;
		yield return null;

        Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

        Vector3 firstPos = testcam.transform.position;

        testcam.upHeld = true;

        yield return null;
        testcam.upHeld = false;

        Vector3 secondPos = testcam.transform.position;

        //Checks to see if we added one unit
        Assert.True(firstPos != secondPos);
    }

    //tests camera moving down on s key press
    [UnityTest]
    public IEnumerator MM1MoveCameraDownOnSKey() {

        if (GameObject.Find("GameInfo") != null) {
            GameObject.Destroy(GameObject.Find("GameInfo"));
        }

        SceneManager.LoadScene("UnderGodScene");

        //Always give it a sec
        yield return null;
		yield return null;

        Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

        Vector3 firstPos = testcam.transform.position;

        testcam.downHeld = true;

        yield return null;
        testcam.downHeld = false;

        Vector3 secondPos = testcam.transform.position;

        //Checks to see if we added one unit
        Assert.True(firstPos != secondPos);
    }

    // tests camera moving down on mouse in bottom boundary
    [UnityTest]
    public IEnumerator MM1MoveCameraDownOnMouse() {

        if (GameObject.Find("GameInfo") != null) {
            GameObject.Destroy(GameObject.Find("GameInfo"));
        }

        SceneManager.LoadScene("UnderGodScene");

        //Always give it a sec
        yield return null;
		yield return null;

        Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

        Vector3 firstPos = testcam.transform.position;

        testcam.transform.position += new Vector3(0.0f, 0.0f, -(Time.deltaTime * testcam.fov));

        Vector3 secondPos = testcam.transform.position;

        //Checks to see if we added one unit
        Assert.True(firstPos != secondPos);
    }

    //tests camera moving up on mouse in top boundary
    [UnityTest]
    public IEnumerator MM1MoveCameraUpOnMouse() {

        if (GameObject.Find("GameInfo") != null) {
            GameObject.Destroy(GameObject.Find("GameInfo"));
        }

        SceneManager.LoadScene("UnderGodScene");

        //Always give it a sec
        yield return null;
		yield return null;

        Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

        Vector3 firstPos = testcam.transform.position;

        testcam.transform.position += new Vector3(0.0f, 0.0f, Time.deltaTime * testcam.fov);

        Vector3 secondPos = testcam.transform.position;

        //Checks to see if we added one unit
        Assert.True(firstPos != secondPos);
    }

    // tests camera moving left on mouse in left boundary
    [UnityTest]
    public IEnumerator MM1MoveCameraLeftOnMouse() {

        if (GameObject.Find("GameInfo") != null) {
            GameObject.Destroy(GameObject.Find("GameInfo"));
        }

        SceneManager.LoadScene("UnderGodScene");

        //Always give it a sec
        yield return null;
		yield return null;

        Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

        Vector3 firstPos = testcam.transform.position;

        testcam.transform.position += new Vector3(-(Time.deltaTime * testcam.fov), 0.0f, 0.0f);

        Vector3 secondPos = testcam.transform.position;

        //Checks to see if we added one unit
        Assert.True(firstPos != secondPos);
    }


    // tests camera moving right on mouse in right boundary
    [UnityTest]
    public IEnumerator MM1MoveCameraRightOnMouse() {

        if (GameObject.Find("GameInfo") != null) {
            GameObject.Destroy(GameObject.Find("GameInfo"));
        }

        SceneManager.LoadScene("UnderGodScene");

        //Always give it a sec
        yield return null;
		yield return null;

        Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

        Vector3 firstPos = testcam.transform.position;

        testcam.transform.position += new Vector3(Time.deltaTime * testcam.fov,
            0.0f, 0.0f);

        Vector3 secondPos = testcam.transform.position;

        //Checks to see if we added one unit
        Assert.True(firstPos != secondPos);
    }

    // tests that player gains worshippers
    [UnityTest]
    public IEnumerator MM4PlayerGeneratesWorshippers() {

        if (GameObject.Find("GameInfo") != null) {
            GameObject.Destroy(GameObject.Find("GameInfo"));
        }

        SceneManager.LoadScene("UnderGodScene");

		yield return null;
		yield return null;

        Faction faction = GameObject.Find("GameManager").GetComponent<GameManager>().PlayerFaction;

        int tempWorshippers = faction.WorshipperCount;

        //Always give it a sec
        yield return new WaitForSeconds(2);

        int tempWorshippers2 = faction.WorshipperCount;

        Assert.True(tempWorshippers != tempWorshippers2);
    }

    // tests that player gains materials
    [UnityTest]
    public IEnumerator MM4PlayerGeneratesMaterials() {

        if (GameObject.Find("GameInfo") != null) {
            GameObject.Destroy(GameObject.Find("GameInfo"));
        }

        SceneManager.LoadScene("UnderGodScene");

		yield return null;
		yield return null;

        Faction faction = GameObject.Find("GameManager").GetComponent<GameManager>().PlayerFaction;

        int tempMats = faction.MaterialCount;

        //Always give it a sec
        yield return new WaitForSeconds(2);

        int tempMats2 = faction.MaterialCount;

        Assert.True(tempMats != tempMats2);
    }

    // tests that that UI shows the correct amounts of materials that the user has
    [UnityTest]
    public IEnumerator MM10UIShowsCorrectMaterialAmount() {

        if (GameObject.Find("GameInfo") != null) {
            GameObject.Destroy(GameObject.Find("GameInfo"));
        }

        SceneManager.LoadScene("UnderGodScene");

		yield return null;
		yield return null;

        Faction faction = GameObject.Find("GameManager").GetComponent<GameManager>().PlayerFaction;

        //Always give it a sec
        yield return new WaitForSeconds(2);

        int tempMats = faction.MaterialCount;

        Text temptext = GameObject.Find("ResourcesText").GetComponent<resourceScript>().resourceText;

        Assert.True(temptext.text.Split(' ')[1].Trim() == tempMats.ToString());
    }

    // tests that that UI shows the correct amounts of materials that the user has
    [UnityTest]
    public IEnumerator MM10UIShowsCorrectWorshippersAmount() {

        if (GameObject.Find("GameInfo") != null) {
            GameObject.Destroy(GameObject.Find("GameInfo"));
        }

        SceneManager.LoadScene("UnderGodScene");

		yield return null;
		yield return null;

        Faction faction = GameObject.Find("GameManager").GetComponent<GameManager>().PlayerFaction;

        //Always give it a sec
        yield return new WaitForSeconds(2);

        int tempWors = faction.WorshipperCount;

        Text temptext = GameObject.Find("ResourcesText").GetComponent<resourceScript>().resourceText;

        Assert.True(temptext.text.Split(' ')[4].Trim() == tempWors.ToString());
    }

	// tests that that UI shows the correct amounts of materials that the user has
	[UnityTest]
	public IEnumerator MM10UIDisplaysResources() {

		if (GameObject.Find("GameInfo") != null) {
			GameObject.Destroy(GameObject.Find("GameInfo"));
		}

		SceneManager.LoadScene("UnderGodScene");
		yield return null;
		yield return null;

		Text temptext = GameObject.Find("ResourcesText").GetComponent<resourceScript>().resourceText;

		Assert.True(temptext.text != null);
	}

    [UnityTest]
    public IEnumerator MM2BuildingSelectedHighlight()
    {
        GameManager gameManager = null;
        Building playerBuilding = null;
        Faction playerFaction = null;
        int PositionCount = 0;
        InitManagementScene();
        yield return null;
        yield return null;
        // Grab a random building
        gameManager = GetGameManager();
        playerFaction = gameManager.PlayerFaction;
        playerBuilding = playerFaction.OwnedBuildings[0];

        // Select building
        gameManager.SetSelectedBuilding(playerBuilding);
        yield return null;

        // Check that building is highlighted, line renderer will have points when highlighted
        PositionCount = playerBuilding.BuildingObject.GetComponent<LineRenderer>().positionCount;
        Assert.True(PositionCount > 0);
    }

    [UnityTest]
    public IEnumerator MM3VillagesWithinGodsTerritory()
    {
        GameManager gameManager = null;
        Building villageBuilding = null;
        bool blnInArea = false;
        float RadiusOfPlacement = 0;
        float AngleOfPlacement = 0;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();
        foreach(Faction faction in gameManager.CurrentFactions)
        {
            // Find a village building
            villageBuilding = faction.OwnedBuildings.Find(matchingBuilding => matchingBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE);
            blnInArea = false;
            if(villageBuilding != null)
            {
                foreach (float[] playerArea in faction.FactionArea)
                {
                    // Calculation to check if a village is in its own territory
                    RadiusOfPlacement = Vector3.Distance(new Vector3(0, 0.5f, 0), villageBuilding.BuildingPosition);

                    AngleOfPlacement = Vector3.Angle(new Vector3(100f, 0.5f, 0), villageBuilding.BuildingPosition) * Mathf.PI / 180;
                    // In third or fourth quadrant, add Pi as .angle will always return smallest vector
                    if (villageBuilding.BuildingPosition.z < 0)
                    {
                        AngleOfPlacement = 2 * Mathf.PI - AngleOfPlacement;
                    }
                    if ((AngleOfPlacement > playerArea[2] && AngleOfPlacement < playerArea[3])
                && RadiusOfPlacement > playerArea[0] && RadiusOfPlacement < playerArea[1])
                    {
                        blnInArea = true;
                        break;
                    }
                }
            }
            Assert.True(blnInArea);
        }
    }

    [UnityTest]
    public IEnumerator MM3VillagesRandomlyPlaced()
    {
        GameManager gameManager = null;
        Vector3 buildingPosition = new Vector3();
        InitManagementScene();
        yield return null;
        yield return null;

        gameManager = GetGameManager();
        buildingPosition = gameManager.PlayerVillage.BuildingPosition;
        // Check 5 times for different village starting positions
        for(int i = 0; i < 5; i++)
        {
            InitManagementScene();
            yield return null;
            yield return null;

            gameManager = GetGameManager();
            Assert.True(!buildingPosition.Equals(gameManager.PlayerVillage.BuildingPosition));
        }
    }

    [UnityTest]
    public IEnumerator MM4VillageBuildingGeneratesWorshippers()
    {
        GameManager gameManager = null;
        Building villageBuilding = null;
        int worshipperCount = 0;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();
        gameManager.PlayerFaction.MaterialCount = 10000;
        villageBuilding = gameManager.PlayerVillage;

        worshipperCount = gameManager.PlayerFaction.WorshipperCount;
        yield return new WaitUntil(() => gameManager.ResourceTicks == 1);
        Assert.True(gameManager.PlayerFaction.WorshipperCount == worshipperCount + 1);

        worshipperCount = gameManager.PlayerFaction.WorshipperCount;
        villageBuilding.UpgradeBuilding();
        yield return new WaitUntil(() => gameManager.ResourceTicks == 2);
        Assert.True(gameManager.PlayerFaction.WorshipperCount == worshipperCount + 2);

        worshipperCount = gameManager.PlayerFaction.WorshipperCount;
        villageBuilding.UpgradeBuilding();
        yield return new WaitUntil(() => gameManager.ResourceTicks == 3);
        Assert.True(gameManager.PlayerFaction.WorshipperCount == worshipperCount + 3);
    }

    [UnityTest]
    public IEnumerator MM4VillageBuildingGeneratesMaterials()
    {
        GameManager gameManager = null;
        Building villageBuilding = null;
        int materialCount = 0;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();
        gameManager.PlayerFaction.MaterialCount = 10000;
        villageBuilding = gameManager.PlayerVillage;

        materialCount = gameManager.PlayerFaction.MaterialCount;
        yield return new WaitUntil(() => gameManager.ResourceTicks == 1);
        Assert.True(gameManager.PlayerFaction.MaterialCount == materialCount + 1);

        villageBuilding.UpgradeBuilding();
        materialCount = gameManager.PlayerFaction.MaterialCount;
        yield return new WaitUntil(() => gameManager.ResourceTicks == 2);
        Assert.True(gameManager.PlayerFaction.MaterialCount == materialCount + 2,
            string.Format("{0}, {1}", materialCount + 3, gameManager.PlayerFaction.MaterialCount));

        villageBuilding.UpgradeBuilding();
        materialCount = gameManager.PlayerFaction.MaterialCount;
        yield return new WaitUntil(() => gameManager.ResourceTicks == 3);
        Assert.True(gameManager.PlayerFaction.MaterialCount == materialCount + 3, 
            string.Format("{0}, {1}",materialCount + 3, gameManager.PlayerFaction.MaterialCount));
    }

    [UnityTest]
    public IEnumerator MM5PlayerCanSelectEnemyVillage()
    {
        GameManager gameManager = null;
        Building enemyBuilding = null;

        InitManagementScene();
        yield return null;
        yield return null;

        gameManager = GetGameManager();
        enemyBuilding = gameManager.EnemyFactions[0].OwnedBuildings.Find(villageBuilding => villageBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE);

        gameManager.SetSelectedBuilding(enemyBuilding);
        // Check building is highlighted
        Assert.True(enemyBuilding.BuildingObject.GetComponent<LineRenderer>().positionCount > 0);
        // Check that selected building is enemy village
        Assert.True(gameManager.SelectedBuilding == enemyBuilding);
    }

    [UnityTest]
    public IEnumerator MM5PlayerCanEnterCombatMode()
    {
        GameManager gameManager = null;
        Building enemyBuilding = null;

        InitManagementScene();
        yield return null;
        yield return null;

        gameManager = GetGameManager();
        enemyBuilding = gameManager.EnemyFactions[0].OwnedBuildings.Find(villageBuilding => villageBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE);

        gameManager.SetSelectedBuilding(enemyBuilding);

        gameManager.EnterCombatMode();
        yield return null;
        yield return null;

        Assert.True(GameObject.Find("GameManager") == null);
    }

    [UnityTest]
    public IEnumerator MM5EnterCombatModeAnimationPlays()
    {
        InitManagementScene();
        yield return null;
        yield return null;

        throw new System.NotImplementedException();
    }

    [UnityTest]
    public IEnumerator MM6EnemyChallengesPlayerAfterTime()
    {
        GameManager gameManager = null;
        InitManagementScene();
        yield return null;
        yield return null;

        gameManager = GetGameManager();
        gameManager.EnemyChallengeTimer = 2;

        yield return new WaitForSeconds(3);
        // Check that an enemy has challenged
        Assert.True(GetGameManager() == null);
    }

    [UnityTest]
    public IEnumerator MM6EnemyChallengerIsOfCurrentTier()
    {
        GameManager gameManager = null;
        GameInfo gameInfo = null;
        int CurrentTier = 0;
        InitManagementScene();
        yield return null;
        yield return null;

        gameManager = GetGameManager();
        gameManager.EnemyChallengeTimer = 2;
        CurrentTier = gameManager.CurrentTier;
        yield return new WaitForSeconds(3);

        gameInfo = GetGameInfoObject();
        Assert.True(gameInfo.EnemyFaction.GodTier == CurrentTier);
    }

    [UnityTest]
    public IEnumerator MM6WarningAppearsBeforeEnemyChallenge()
    {
        yield return null;
        throw new System.NotImplementedException();
        // Check that enemy challenege UI element is active/visible
    }

    [UnityTest]
    public IEnumerator MM7HigherTierFactionsNotVisible()
    {
        GameManager gameManager = null;
        InitManagementScene();
        yield return null;
        yield return null;

        // First Tier
        gameManager = GetGameManager();
        foreach(Faction faction in gameManager.EnemyFactions.FindAll(matchingFaction => matchingFaction.GodTier > gameManager.CurrentTier))
        {
            foreach(Building building in faction.OwnedBuildings)
            {
                Assert.False(building.BuildingObject.activeInHierarchy);
            }
        }
        yield return null;
        // Second Tier
        gameManager.UnlockNextTier();
        foreach (Faction faction in gameManager.EnemyFactions.FindAll(matchingFaction => matchingFaction.GodTier > gameManager.CurrentTier))
        {
            foreach (Building building in faction.OwnedBuildings)
            {
                Assert.False(building.BuildingObject.activeInHierarchy);
            }
        }
        yield return null;
        // Third Tier
        gameManager.UnlockNextTier();
        foreach (Faction faction in gameManager.EnemyFactions.FindAll(matchingFaction => matchingFaction.GodTier > gameManager.CurrentTier))
        {
            foreach (Building building in faction.OwnedBuildings)
            {
                Assert.False(building.BuildingObject.activeInHierarchy);
            }
        }
    }

    [UnityTest]
    public IEnumerator MM7TierUnlocksAfterEnemiesDefeated()
    {
        GameManager gameManager = null;
        SetupManager setupManager = null;
        Building enemyVillage = null;
        Faction enemyFaction = null;
        InitManagementScene();
        yield return null;
        yield return null;

        gameManager = GetGameManager();
        for(int i = 0; i < gameManager.EnemiesPerTier; i++)
        {
            enemyFaction = gameManager.EnemyFactions.Find(matchingFaction => matchingFaction.GodTier == gameManager.CurrentTier);
            enemyVillage = enemyFaction.OwnedBuildings.Find(villageBuilding => villageBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE);
            gameManager.SetSelectedBuilding(enemyVillage);
            gameManager.EnterCombatMode();
            yield return null;
            yield return null;

            setupManager = GetSetupManager();
            setupManager.battleResult = 0;
            setupManager.finishedBattle = true;
            yield return null;
            yield return null;

            gameManager = GetGameManager();
            // Return from combat mode
        }
        Assert.True(gameManager.CurrentTier == 1);
    }

    [UnityTest]
    public IEnumerator MM7AnimationPlaysAfterTierUnlock()
    {
        GameManager gameManager = null;
        InitManagementScene();
        yield return null;
        yield return null;

        gameManager = GetGameManager();
        gameManager.UnlockNextTier();

        // Check for animation
        throw new System.NotImplementedException();
    }

    [UnityTest]
    public IEnumerator MM8EnemiesUpgradeTheirOwnBuildings()
    {
        GameManager gameManager = null;
        Faction enemyFaction = null;
        bool blnBuildingUpgraded = false;
        InitManagementScene();
        yield return null;
        yield return null;

        gameManager = GetGameManager();
        enemyFaction = gameManager.EnemyFactions[0];
        enemyFaction.MaterialCount = 1000; // Give enemy faction enough materials to upgrade a building

        yield return new WaitUntil(() => gameManager.ResourceTicks == 1);

        foreach(Building building in enemyFaction.OwnedBuildings)
        {
            if(building.UpgradeLevel > 1)
            {
                blnBuildingUpgraded = true;
            }
        }
        Assert.True(blnBuildingUpgraded);
    }

    [UnityTest]
    public IEnumerator MM8EnemiesBuildNewBuildings()
    {
        // Conditions for building a new building requires all current buildings to be max level already
        GameManager gameManager = null;
        Faction enemyFaction = null;
        int enemyBuildingCount = 0;
        InitManagementScene();
        yield return null;
        yield return null;
        
        // Shrink building size to increase odds of two buildings not being built on top of eachother as building placement attempts are random
        Building.BuildingRadiusSize = 1;

        gameManager = GetGameManager();
        enemyFaction = gameManager.EnemyFactions[0];
        enemyFaction.MaterialCount = int.MaxValue - 10000; // Give enemy faction enough materials to upgrade a building
        enemyBuildingCount = enemyFaction.OwnedBuildings.Count;

        foreach(Building building in enemyFaction.OwnedBuildings)
        {
            building.UpgradeBuilding();
            building.UpgradeBuilding();
        }
        yield return new WaitUntil(() => gameManager.ResourceTicks == 1);
        Assert.True(enemyFaction.OwnedBuildings.Count > enemyBuildingCount);
    }

    [UnityTest]
    public IEnumerator MM8EnemiesBuildingsCostMore()
    {
        GameManager gameManager = null;
        Faction enemyFaction = null;
        int enemyBuildingCount = 0;
        int enemyMaterials = 0;
        int buildingCost = 0;
        InitManagementScene();
        yield return null;
        yield return null;

        // Shrink building size to increase odds of two buildings not being built on top of eachother as building placement attempts are random
        Building.BuildingRadiusSize = 0.1f;

        gameManager = GetGameManager();
        enemyFaction = gameManager.EnemyFactions[0];
        enemyFaction.MaterialCount = int.MaxValue - 10000; // Give enemy faction enough materials to upgrade a building
        enemyBuildingCount = enemyFaction.OwnedBuildings.Count;
        enemyMaterials = enemyFaction.MaterialCount;

        foreach (Building building in enemyFaction.OwnedBuildings)
        {
            building.UpgradeBuilding();
            building.UpgradeBuilding();
        }
        yield return new WaitUntil(() => enemyFaction.OwnedBuildings.Count == enemyBuildingCount + 1);
        buildingCost = Building.CalculateBuildingCost(enemyFaction.OwnedBuildings[0].BuildingType);
        Assert.True(enemyFaction.MaterialCount < enemyMaterials - buildingCost);
    }

    [UnityTest]
    public IEnumerator MM9PlaceBuildingInPlayersTerritory()
    {
        GameManager gameManager = null;
        Faction playerFaction = null;
        Building buildingToPlace = null;
        int buildingCount = 0;
        InitManagementScene();
        yield return null;
        yield return null;
        Building.BuildingRadiusSize = 0.1f;
        gameManager = GetGameManager();
        playerFaction = gameManager.PlayerFaction;
        buildingCount = playerFaction.OwnedBuildings.Count;

        buildingToPlace = gameManager.CreateRandomBuilding(playerFaction);
        gameManager.GameMap.PlaceBuilding(buildingToPlace, new Vector3(1, 0.5f, 1));
        yield return null;

        Assert.True(playerFaction.OwnedBuildings.Count > buildingCount);
    }

    [UnityTest]
    public IEnumerator MM9PlaceBuildingOutsidePlayersTerritory()
    {
        GameManager gameManager = null;
        Faction playerFaction = null;
        Building buildingToPlace = null;
        int buildingCount = 0;
        InitManagementScene();
        yield return null;
        yield return null;
        Building.BuildingRadiusSize = 0.1f;
        gameManager = GetGameManager();
        playerFaction = gameManager.PlayerFaction;
        buildingCount = playerFaction.OwnedBuildings.Count;

        buildingToPlace = gameManager.CreateRandomBuilding(playerFaction);
        Assert.False(gameManager.GameMap.PlaceBuilding(buildingToPlace, new Vector3(1000, 0.5f, 1000)));
    }

    [UnityTest]
    public IEnumerator MM9PlaceBuildingOnTopOfAnotherBuilding()
    {
        GameManager gameManager = null;
        Faction playerFaction = null;
        Building buildingToPlace = null;
        Building currentBuilding = null;
        int buildingCount = 0;
        InitManagementScene();
        yield return null;
        yield return null;
        Building.BuildingRadiusSize = 0.1f;
        gameManager = GetGameManager();
        playerFaction = gameManager.PlayerFaction;
        currentBuilding = gameManager.PlayerVillage;
        buildingCount = playerFaction.OwnedBuildings.Count;

        buildingToPlace = gameManager.CreateRandomBuilding(playerFaction);
        Assert.False(gameManager.GameMap.PlaceBuilding(buildingToPlace, currentBuilding.BuildingPosition));
    }

    [UnityTest]
    public IEnumerator MM11NewGameOptionAvailableInMenu()
    {
        yield return null;
        throw new System.NotImplementedException();
        // load main menu, look for new game button
    }

    [UnityTest]
    public IEnumerator MM11PlayCanChooseStartingGod()
    {
        yield return null;
        throw new System.NotImplementedException();
    }

    [UnityTest]
    public IEnumerator MM11AllFactionsAreUnique()
    {
        GameManager gameManager = null;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        foreach(Faction faction in gameManager.CurrentFactions)
        {
            foreach(Faction otherFaction in gameManager.CurrentFactions)
            {
                if(faction != otherFaction)
                {
                    Assert.True(faction.Type != otherFaction.Type);
                }
            }
        }
    }

    [UnityTest]
    public IEnumerator MM11HigherTierFactionsHaveMoreStartingBuildings()
    {
        int Tier1MaxCount = 0;
        int Tier2MaxCount = 0;
        GameManager gameManager = null;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        foreach(Faction faction in gameManager.CurrentFactions.FindAll(matchingFaction => matchingFaction.GodTier == 0))
        {
            if(faction.OwnedBuildings.Count > Tier1MaxCount)
            {
                Tier1MaxCount = faction.OwnedBuildings.Count;
            }
        }

        foreach (Faction faction in gameManager.CurrentFactions.FindAll(matchingFaction => matchingFaction.GodTier == 1))
        {
            Assert.True(faction.OwnedBuildings.Count > Tier1MaxCount);
            if (faction.OwnedBuildings.Count > Tier2MaxCount)
            {
                Tier2MaxCount = faction.OwnedBuildings.Count;
            }
        }

        foreach (Faction faction in gameManager.CurrentFactions.FindAll(matchingFaction => matchingFaction.GodTier == 2))
        {
            Assert.True(faction.OwnedBuildings.Count > Tier2MaxCount && faction.OwnedBuildings.Count > Tier1MaxCount, 
                string.Format("{0} > {1} > {2}", faction.OwnedBuildings.Count, Tier2MaxCount, Tier1MaxCount));
        }
    }

    [UnityTest]
    public IEnumerator MM11PlayerStartsInTierZero()
    {
        GameManager gameManager = null;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        Assert.True(gameManager.PlayerFaction.GodTier == 0);
    }

    [UnityTest]
    public IEnumerator MM11GameMapGeneratedOnStart()
    {
        GameManager gameManager = null;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        Assert.True(gameManager.GameMap != null && gameManager.GameMap.GetMapObject() != null);
    }

    [UnityTest]
    public IEnumerator MM12PlayerCanCreateAltarBuilding()
    {
        Building altarBuilding = null;
        GameManager gameManager = null;
        bool blnBuildingPlaced = false;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        altarBuilding = new Building(Building.BUILDING_TYPE.ALTAR, gameManager.PlayerFaction);
        // 100 attempts to find a valid building placement
        for (int i = 0; i < 100; i++)
        {
            if(gameManager.GameMap.PlaceBuilding(altarBuilding, gameManager.GameMap.CalculateRandomPosition(gameManager.PlayerFaction)))
            {
                blnBuildingPlaced = true;
                break;
            }
        }
        Assert.True(blnBuildingPlaced);
    }

    [UnityTest]
    public IEnumerator MM12PlayerCanCreateMineBuilding()
    {
        Building mineBuilding = null;
        GameManager gameManager = null;
        bool blnBuildingPlaced = false;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        mineBuilding = new Building(Building.BUILDING_TYPE.MATERIAL, gameManager.PlayerFaction);
        // 100 attempts to find a valid building placement
        for (int i = 0; i < 100; i++)
        {
            if (gameManager.GameMap.PlaceBuilding(mineBuilding, gameManager.GameMap.CalculateRandomPosition(gameManager.PlayerFaction)))
            {
                blnBuildingPlaced = true;
                break;
            }
        }
        Assert.True(blnBuildingPlaced);
    }

    [UnityTest]
    public IEnumerator MM12PlayerCanCreateHousingBuilding()
    {
        Building housingBuilding = null;
        GameManager gameManager = null;
        bool blnBuildingPlaced = false;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        housingBuilding = new Building(Building.BUILDING_TYPE.HOUSING, gameManager.PlayerFaction);
        // 100 attempts to find a valid building placement
        for (int i = 0; i < 100; i++)
        {
            if (gameManager.GameMap.PlaceBuilding(housingBuilding, gameManager.GameMap.CalculateRandomPosition(gameManager.PlayerFaction)))
            {
                blnBuildingPlaced = true;
                break;
            }
        }
        Assert.True(blnBuildingPlaced);
    }

    [UnityTest]
    public IEnumerator MM12PlayerCanCreateBlackSmithBuilding()
    {
        Building blackSmithBuilding = null;
        GameManager gameManager = null;
        bool blnBuildingPlaced = false;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        blackSmithBuilding = new Building(Building.BUILDING_TYPE.UPGRADE, gameManager.PlayerFaction);
        // 100 attempts to find a valid building placement
        for (int i = 0; i < 100; i++)
        {
            if (gameManager.GameMap.PlaceBuilding(blackSmithBuilding, gameManager.GameMap.CalculateRandomPosition(gameManager.PlayerFaction)))
            {
                blnBuildingPlaced = true;
                break;
            }
        }
        Assert.True(blnBuildingPlaced);
    }

    [UnityTest]
    public IEnumerator MM12BufferedBuildingVisible()
    {
        GameManager gameManager = null;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        gameManager.BufferBuilding(Building.BUILDING_TYPE.ALTAR);

        Assert.True(gameManager.BufferedBuilding != null);
    }

    [Test]
    public void MM13BlackSmithBuildingExists()
    {
        Faction TestFaction = new Faction("Test", Faction.GodType.Duck, 0);
        Building blackSmithBuilding = new Building(Building.BUILDING_TYPE.UPGRADE, TestFaction);

        Assert.True(blackSmithBuilding.BuildingType == Building.BUILDING_TYPE.UPGRADE);
    }

    [UnityTest]
    public IEnumerator MM13OnlyOneBlackSmithBuildingAllowed()
    {
        Building upgradeBuilding = null;
        GameManager gameManager = null;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        upgradeBuilding = new Building(Building.BUILDING_TYPE.UPGRADE, gameManager.PlayerFaction);
        // The check is done when buffering the building to place
        for (int i = 0; i < 100; i++)
        {
            if (gameManager.GameMap.PlaceBuilding(upgradeBuilding, gameManager.GameMap.CalculateRandomPosition(gameManager.PlayerFaction)))
            {
                break;
            }
        }
        gameManager.BufferBuilding(Building.BUILDING_TYPE.UPGRADE);
        Assert.True(gameManager.BufferedBuilding == null);
    }

    [UnityTest]
    public IEnumerator MM13BlackSmithCanBeUpgraded()
    {
        Building upgradeBuilding = null;
        GameManager gameManager = null;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();
        gameManager.PlayerFaction.MaterialCount = 100000;
        upgradeBuilding = new Building(Building.BUILDING_TYPE.UPGRADE, gameManager.PlayerFaction);
        for (int i = 0; i < 100; i++)
        {
            if (gameManager.GameMap.PlaceBuilding(upgradeBuilding, gameManager.GameMap.CalculateRandomPosition(gameManager.PlayerFaction)))
            {
                break;
            }
        }
        Assert.True(upgradeBuilding.UpgradeLevel == 1);

        upgradeBuilding.UpgradeBuilding();
        Assert.True(upgradeBuilding.UpgradeLevel == 2);

        upgradeBuilding.UpgradeBuilding();
        Assert.True(upgradeBuilding.UpgradeLevel == 3);
    }

    [UnityTest]
    public IEnumerator MM13UpgradeBuildingUIExists()
    {
        GameManager gameManager = null;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        gameManager.SetUpgradeUIActive(true);
        // Check for UI active
        throw new System.NotImplementedException();
    }

    [UnityTest]
    public IEnumerator MM13PlayerCanUnlockUpgrade()
    {
        yield return null;
        throw new System.NotImplementedException();
    }

    [UnityTest]
    public IEnumerator MM13BuyingUpgradeReducesPlayerMaterials()
    {
        yield return null;
        throw new System.NotImplementedException();
    }

    [UnityTest]
    public IEnumerator MM13UpgradeBuildingUICanBeClosed()
    {
        GameManager gameManager = null;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        gameManager.SetUpgradeUIActive(true);
        gameManager.SetUpgradeUIActive(false);
        // Check for UI in active
        throw new System.NotImplementedException();
    }

    [UnityTest]
    public IEnumerator MM14AltarBuildingGeneratesWorshippers()
    {
        GameManager gameManager = null;
        Building altarBuilding = null;
        int worshipperCount = 0;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();
        gameManager.PlayerFaction.MaterialCount = 10000;
        altarBuilding = new Building(Building.BUILDING_TYPE.ALTAR, gameManager.PlayerFaction);
        for (int i = 0; i < 100; i++)
        {
            if (gameManager.GameMap.PlaceBuilding(altarBuilding, gameManager.GameMap.CalculateRandomPosition(gameManager.PlayerFaction)))
            {
                break;
            }
        }
        
        worshipperCount = gameManager.PlayerFaction.WorshipperCount;
        yield return new WaitUntil(() => gameManager.ResourceTicks == 1);
        Assert.True(gameManager.PlayerFaction.WorshipperCount == worshipperCount + 2);

        worshipperCount = gameManager.PlayerFaction.WorshipperCount;
        altarBuilding.UpgradeBuilding();
        yield return new WaitUntil(() => gameManager.ResourceTicks == 2);
        Assert.True(gameManager.PlayerFaction.WorshipperCount == worshipperCount + 3);

        worshipperCount = gameManager.PlayerFaction.WorshipperCount;
        altarBuilding.UpgradeBuilding();
        yield return new WaitUntil(() => gameManager.ResourceTicks == 3);
        Assert.True(gameManager.PlayerFaction.WorshipperCount == worshipperCount + 4);
    }

    [UnityTest]
    public IEnumerator MM14AltarBuildingUIExists()
    {
        yield return null;
        throw new System.NotImplementedException();
    }

    [UnityTest]
    public IEnumerator MM14PlayerCanCreateAltarBuilding()
    {
        Building altarBuilding = null;
        GameManager gameManager = null;
        bool blnBuildingPlaced = false;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        altarBuilding = new Building(Building.BUILDING_TYPE.ALTAR, gameManager.PlayerFaction);
        // 100 attempts to find a valid building placement
        for (int i = 0; i < 100; i++)
        {
            if (gameManager.GameMap.PlaceBuilding(altarBuilding, gameManager.GameMap.CalculateRandomPosition(gameManager.PlayerFaction)))
            {
                blnBuildingPlaced = true;
                break;
            }
        }
        Assert.True(blnBuildingPlaced);
    }

    [UnityTest]
    public IEnumerator MM14AltarCanBeUpgraded()
    {
        Building altarBuilding = null;
        GameManager gameManager = null;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();
        gameManager.PlayerFaction.MaterialCount = 100000;
        altarBuilding = new Building(Building.BUILDING_TYPE.ALTAR, gameManager.PlayerFaction);
        for (int i = 0; i < 100; i++)
        {
            if (gameManager.GameMap.PlaceBuilding(altarBuilding, gameManager.GameMap.CalculateRandomPosition(gameManager.PlayerFaction)))
            {
                break;
            }
        }
        Assert.True(altarBuilding.UpgradeLevel == 1);

        altarBuilding.UpgradeBuilding();
        Assert.True(altarBuilding.UpgradeLevel == 2);

        altarBuilding.UpgradeBuilding();
        Assert.True(altarBuilding.UpgradeLevel == 3);
    }

    [UnityTest]
    public IEnumerator MM15MineBuildingGeneratesMaterials()
    {
        GameManager gameManager = null;
        Building mineBuilding = null;
        int materialCount = 0;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();
        gameManager.PlayerFaction.MaterialCount = 10000;
        mineBuilding = new Building(Building.BUILDING_TYPE.MATERIAL, gameManager.PlayerFaction);
        for (int i = 0; i < 100; i++)
        {
            if (gameManager.GameMap.PlaceBuilding(mineBuilding, gameManager.GameMap.CalculateRandomPosition(gameManager.PlayerFaction)))
            {
                break;
            }
        }

        materialCount = gameManager.PlayerFaction.MaterialCount;
        yield return new WaitUntil(() => gameManager.ResourceTicks == 1);
        Assert.True(gameManager.PlayerFaction.MaterialCount == materialCount + 2);

        mineBuilding.UpgradeBuilding();
        materialCount = gameManager.PlayerFaction.MaterialCount;
        yield return new WaitUntil(() => gameManager.ResourceTicks == 2);
        Assert.True(gameManager.PlayerFaction.MaterialCount == materialCount + 3);

        mineBuilding.UpgradeBuilding();
        materialCount = gameManager.PlayerFaction.MaterialCount;
        yield return new WaitUntil(() => gameManager.ResourceTicks == 3);
        Assert.True(gameManager.PlayerFaction.MaterialCount == materialCount + 4);
    }

    [UnityTest]
    public IEnumerator MM15MineBuildingUIExists()
    {
        yield return null;
        throw new System.NotImplementedException();
    }

    [UnityTest]
    public IEnumerator MM15PlayerCanCreateMineBuilding()
    {
        Building mineBuilding = null;
        GameManager gameManager = null;
        bool blnBuildingPlaced = false;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        mineBuilding = new Building(Building.BUILDING_TYPE.MATERIAL, gameManager.PlayerFaction);
        // 100 attempts to find a valid building placement
        for (int i = 0; i < 100; i++)
        {
            if (gameManager.GameMap.PlaceBuilding(mineBuilding, gameManager.GameMap.CalculateRandomPosition(gameManager.PlayerFaction)))
            {
                blnBuildingPlaced = true;
                break;
            }
        }
        Assert.True(blnBuildingPlaced);
    }

    [UnityTest]
    public IEnumerator MM15MineCanBeUpgraded()
    {
        Building mineBuilding = null;
        GameManager gameManager = null;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();
        gameManager.PlayerFaction.MaterialCount = 100000;
        mineBuilding = new Building(Building.BUILDING_TYPE.MATERIAL, gameManager.PlayerFaction);
        for (int i = 0; i < 100; i++)
        {
            if (gameManager.GameMap.PlaceBuilding(mineBuilding, gameManager.GameMap.CalculateRandomPosition(gameManager.PlayerFaction)))
            {
                break;
            }
        }
        Assert.True(mineBuilding.UpgradeLevel == 1);

        mineBuilding.UpgradeBuilding();
        Assert.True(mineBuilding.UpgradeLevel == 2);

        mineBuilding.UpgradeBuilding();
        Assert.True(mineBuilding.UpgradeLevel == 3);
    }

    [UnityTest]
    public IEnumerator MM16HousingAffectsMorale()
    {
        GameManager gameManager = null;
        float playerMorale = 0;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        // Set worhsipper count higher than housing level, at start housing level is 100
        gameManager.PlayerFaction.WorshipperCount = 10000;

        yield return new WaitUntil(() => gameManager.ResourceTicks == 1);
        Assert.True(gameManager.PlayerFaction.Morale < 1);
        gameManager.PlayerFaction.WorshipperCount = 50;
        playerMorale = gameManager.PlayerFaction.Morale;

        yield return new WaitUntil(() => gameManager.ResourceTicks == 2);
        Assert.True(gameManager.PlayerFaction.Morale > playerMorale);
    }

    [UnityTest]
    public IEnumerator MM16HousingBuildingUIExists()
    {
        yield return null;
        throw new System.NotImplementedException();
    }

    [UnityTest]
    public IEnumerator MM16PlayerCanCreateHousingBuilding()
    {
        Building housingBuilding = null;
        GameManager gameManager = null;
        bool blnBuildingPlaced = false;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();

        housingBuilding = new Building(Building.BUILDING_TYPE.HOUSING, gameManager.PlayerFaction);
        // 100 attempts to find a valid building placement
        for (int i = 0; i < 100; i++)
        {
            if (gameManager.GameMap.PlaceBuilding(housingBuilding, gameManager.GameMap.CalculateRandomPosition(gameManager.PlayerFaction)))
            {
                blnBuildingPlaced = true;
                break;
            }
        }
        Assert.True(blnBuildingPlaced);
    }

    [UnityTest]
    public IEnumerator MM16HouseCanBeUpgraded()
    {
        Building housingBuilding = null;
        GameManager gameManager = null;
        InitManagementScene();
        yield return null;
        yield return null;
        gameManager = GetGameManager();
        gameManager.PlayerFaction.MaterialCount = 100000;
        housingBuilding = new Building(Building.BUILDING_TYPE.HOUSING, gameManager.PlayerFaction);
        for (int i = 0; i < 100; i++)
        {
            if (gameManager.GameMap.PlaceBuilding(housingBuilding, gameManager.GameMap.CalculateRandomPosition(gameManager.PlayerFaction)))
            {
                break;
            }
        }
        Assert.True(housingBuilding.UpgradeLevel == 1);

        housingBuilding.UpgradeBuilding();
        Assert.True(housingBuilding.UpgradeLevel == 2);

        housingBuilding.UpgradeBuilding();
        Assert.True(housingBuilding.UpgradeLevel == 3);
    }

    // Helper method
    private void InitManagementScene()
    {
        if (GameObject.Find("GameInfo") != null)
        {
            GameObject.Destroy(GameObject.Find("GameInfo"));
        }
        SceneManager.LoadScene("UnderGodScene");
    }

    // Helper method
    private GameManager GetGameManager()
    {
        GameManager gameManager = null;
        if(GameObject.Find("GameManager") != null)
        {
            if(GameObject.Find("GameManager").GetComponent<GameManager>() != null)
            {
                gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            }
        }
        return gameManager;
    }

    // Helper method
    private GameInfo GetGameInfoObject()
    {
        GameInfo gameInfo = null;
        if(GameObject.Find("GameInfo")!= null)
        {
            if(GameObject.Find("GameInfo").GetComponent<GameInfo>() != null)
            {
                gameInfo = GameObject.Find("GameInfo").GetComponent<GameInfo>();
            }
        }
        return gameInfo;
    }

    // Helper method
    private SetupManager GetSetupManager()
    {
        SetupManager setupManager = null;
        if(GameObject.FindGameObjectWithTag("SetupManager") != null)
        {
            if(GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>() != null)
            {
                setupManager = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
            }
        }
        return setupManager;
    }
}
