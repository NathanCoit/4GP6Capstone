using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;

public class CombatModeTests
{

    [Test]
    public void CombatModeTestsSimplePasses() {
        // Use the Assert class to test conditions.
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator TestCreatingPlayerUnit() {

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        int currentPlayerUnits = BoardMan.playerUnits.Count;

        SetupMan.CreatePlayerUnit(new Vector2(0, 0), MapMan.tiles, 10, 100);

        //Checks to see if we added one unit
        Assert.True(currentPlayerUnits == BoardMan.playerUnits.Count - 1);
    }

    [UnityTest]
    public IEnumerator TestCreatingEnemyUnit()
    {

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        int currentEnemyUnits = BoardMan.enemyUnits.Count;

        SetupMan.CreateEnemyUnit(new Vector2(0, 0), MapMan.tiles, 10, 100);

        //Checks to see if we added one unit
        Assert.True(currentEnemyUnits == BoardMan.enemyUnits.Count - 1);
    }
}
