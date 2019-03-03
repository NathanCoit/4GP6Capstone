using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class CombatModeTests
{
    //Things changed and this is now very borked

    /*
    [Test]
    public void CombatModeTestsSimplePasses()
    {
        // Use the Assert class to test conditions.
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator Req1ACreatingPlayerUnit()
    {

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        int currentPlayerUnits = BoardMan.playerUnits.Count;

        SetupMan.CreatePlayerUnit(new Vector2(0, 0), MapMan.tiles, 10, 2, 100);

        //Checks to see if we added one unit
        Assert.True(currentPlayerUnits == BoardMan.playerUnits.Count - 1);
    }

    [UnityTest]
    public IEnumerator Req1BCreatingEnemyUnit()
    {
        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        int currentEnemyUnits = BoardMan.enemyUnits.Count;

        SetupMan.CreateEnemyUnit(new Vector2(0, 0), MapMan.tiles, 10, 2, 100);

        //Checks to see if we added one unit
        Assert.True(currentEnemyUnits == BoardMan.enemyUnits.Count - 1);
    }

    //Test ending turn without doing anything
    [UnityTest]
    public IEnumerator Req1CEndingAllTurns()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        List<int> initalPosX = new List<int>();

        foreach (GameObject u in BoardMan.playerUnits)
        {
            Units unit = u.GetComponent<Units>();
            initalPosX.Add((int)unit.getPos().x);

            //End turn
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;
            unit.EndTurnButton();
            yield return null;
        }

        yield return null;
        foreach (int i in initalPosX)
        {
            foreach (GameObject g in BoardMan.playerUnits)
            {
                //Make sure nothing moved and player turn ended, otherwise test fails
                if (i != (int)g.GetComponent<Units>().getPos().x || BoardMan.playerTurn)
                    passed = false;
            }
        }
        Assert.True(passed);

    }

    //Testing moving al units right one tile then ending turn
    [UnityTest]
    public IEnumerator Req1DMovingThenEndingTurns()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        GameObject forwardTile = null;
        List<int> initalPosX = new List<int>();

        foreach (GameObject u in BoardMan.playerUnits)
        {
            Units unit = u.GetComponent<Units>();
            initalPosX.Add((int)unit.getPos().x);

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            //Show tiles
            unit.showMovable();
            yield return null;

            //Move to Tile
            List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

            //Find tile 1 to the right
            foreach (GameObject g in MoveableTiles)
            {
                if (g.GetComponent<Movable>().pos.x == unit.getPos().x + 1)
                {
                    forwardTile = g;
                    break;
                }
            }
            forwardTile.GetComponent<Movable>().TestClick();
            forwardTile.GetComponent<Movable>().OnMouseOver();
            yield return null;

            //End turn
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;
            unit.EndTurnButton();
            yield return null;
        }
        yield return null;
        foreach (int i in initalPosX)
        {
            foreach (GameObject g in BoardMan.playerUnits)
            {
                //Make sure everything moved forward and player turn ended, otherwise test fails
                if (i != (int)g.GetComponent<Units>().getPos().x - 1 || BoardMan.playerTurn)
                    passed = false;
            }
        }
        Assert.True(passed);

    }

    //Test moving one forward and attacking
    [UnityTest]
    public IEnumerator Req1EMoivningThenAttacking()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        GameObject forwardTile = null;
        List<int> initalPosX = new List<int>();

        foreach (GameObject u in BoardMan.playerUnits)
        {
            Units unit = u.GetComponent<Units>();
            initalPosX.Add((int)unit.getPos().x);

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            //Show tiles
            unit.showMovable();
            yield return null;

            //Move to Tile
            List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

            //Find tile 1 to the right
            foreach (GameObject g in MoveableTiles)
            {
                if (g.GetComponent<Movable>().pos.x == unit.getPos().x + 1)
                {
                    forwardTile = g;
                    break;
                }
            }
            forwardTile.GetComponent<Movable>().TestClick();
            forwardTile.GetComponent<Movable>().OnMouseOver();
            yield return null;

            //Attack
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;
            unit.showAttackable();
            yield return null;
            List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
            AttackableTiles[0].GetComponent<Attackable>().TestClick();
            AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();

        }
        yield return null;
        foreach (int i in initalPosX)
        {
            foreach (GameObject g in BoardMan.playerUnits)
            {
                //Make sure everything moved forward and player turn ended, otherwise test fails
                if (i != (int)g.GetComponent<Units>().getPos().x - 1 || BoardMan.playerTurn)
                    passed = false;
            }
        }
        Assert.True(passed);

    }

    //Test a mixture of moving and attacking and ending turn
    [UnityTest]
    public IEnumerator Req1FMovingThenAttackingMixed()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        GameObject forwardTile = null;
        List<int> initalPosX = new List<int>();
        bool flip = false;

        foreach (GameObject u in BoardMan.playerUnits)
        {
            Units unit = u.GetComponent<Units>();
            initalPosX.Add((int)unit.getPos().x);

            if (flip)
            {
                //Opening Menu
                unit.TestClick();
                unit.OnMouseOver();
                yield return null;

                //Show tiles
                unit.showMovable();
                yield return null;

                //Move to Tile
                List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

                //Find tile 1 to the right
                foreach (GameObject g in MoveableTiles)
                {
                    if (g.GetComponent<Movable>().pos.x == unit.getPos().x + 1)
                    {
                        forwardTile = g;
                        break;
                    }
                }
                forwardTile.GetComponent<Movable>().TestClick();
                forwardTile.GetComponent<Movable>().OnMouseOver();
                yield return null;

                //Attack
                unit.TestClick();
                unit.OnMouseOver();
                yield return null;
                unit.showAttackable();
                yield return null;
                List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
                AttackableTiles[0].GetComponent<Attackable>().TestClick();
                AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();

                flip = false;
            }
            else
            {
                //End turn
                unit.TestClick();
                unit.OnMouseOver();
                yield return null;
                unit.EndTurnButton();
                yield return null;

                flip = true;
            }

        }
        yield return null;
        foreach (int i in initalPosX)
        {
            foreach (GameObject g in BoardMan.playerUnits)
            {
                //Make sure everything moved forward and player turn ended, otherwise test fails
                if (BoardMan.playerTurn)
                    passed = false;
            }
        }
        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Req2AUnitEndingTurn()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;


        Units unit = BoardMan.playerUnits[0].GetComponent<Units>();

        unit.TestClick();
        unit.OnMouseOver();
        yield return null;
        unit.EndTurnButton();
        yield return null;


        yield return null;

        if (unit.canAct)
            passed = false;

        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Req2BAttackingEndsTurn()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        GameObject forwardTile = null;

        Units unit = BoardMan.playerUnits[0].GetComponent<Units>();

        //Opening Menu
        unit.TestClick();
        unit.OnMouseOver();
        yield return null;

        //Show tiles
        unit.showMovable();
        yield return null;

        //Move to Tile
        List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

        //Find tile 1 to the right
        foreach (GameObject g in MoveableTiles)
        {
            if (g.GetComponent<Movable>().pos.x == unit.getPos().x + 1)
            {
                forwardTile = g;
                break;
            }
        }
        forwardTile.GetComponent<Movable>().TestClick();
        forwardTile.GetComponent<Movable>().OnMouseOver();
        yield return null;

        //Attack
        unit.TestClick();
        unit.OnMouseOver();
        yield return null;
        unit.showAttackable();
        yield return null;
        List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
        AttackableTiles[0].GetComponent<Attackable>().TestClick();
        AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();


        yield return null;

        if (unit.canAct)
            passed = false;

        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Req4ATestUnitAttackDidDamage()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;


        GameObject forwardTile = null;
        List<int> initialWorshiperCount = new List<int>();

        foreach (GameObject u in BoardMan.playerUnits)
        {
            Units unit = u.GetComponent<Units>();
            initialWorshiperCount.Add((int)unit.getWorshiperCount());

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            //Show tiles
            unit.showMovable();
            yield return null;

            //Move to Tile
            List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

            //Find tile 1 to the right
            foreach (GameObject g in MoveableTiles)
            {
                if (g.GetComponent<Movable>().pos.x == unit.getPos().x + 1)
                {
                    forwardTile = g;
                    break;
                }
            }
            forwardTile.GetComponent<Movable>().TestClick();
            forwardTile.GetComponent<Movable>().OnMouseOver();
            yield return null;

            //Attack
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;
            unit.showAttackable();
            yield return null;
            List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
            AttackableTiles[0].GetComponent<Attackable>().TestClick();
            AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();

        }
        yield return null;
        foreach (int i in initialWorshiperCount)
        {
            foreach (GameObject g in BoardMan.enemyUnits)
            {
                //Make sure everything moved forward and player turn ended, otherwise test fails
                if (i == (int)g.GetComponent<Units>().getWorshiperCount())
                    passed = false;
            }
        }

        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Req4BTestUnitCantAttackFriends()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;


        GameObject forwardTile = null;
        //List<Units> pla = new List<int>();

        foreach (GameObject u in BoardMan.playerUnits)
        {
            Units unit = u.GetComponent<Units>();

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            //Show tiles
            unit.showMovable();
            yield return null;

            //Move to Tile
            List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

            //Find tile 1 to the right
            foreach (GameObject g in MoveableTiles)
            {
                if (g.GetComponent<Movable>().pos.x == unit.getPos().x + 1)
                {
                    forwardTile = g;
                    break;
                }
            }
            forwardTile.GetComponent<Movable>().TestClick();
            forwardTile.GetComponent<Movable>().OnMouseOver();
            yield return null;

            //Attack
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;
            unit.showAttackable();
            yield return null;
            List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
            //AttackableTiles[0].GetComponent<Attackable>().testClick();
            //AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();

            yield return null;

            //basically, check if any of the player's units pos is the same as any attackable tile pos and fail if true
            foreach (GameObject tile in AttackableTiles)
            {
                foreach (GameObject g in BoardMan.playerUnits)
                {
                    if (g.GetComponent<Units>().getPos() == tile.GetComponent<Attackable>().pos)
                        passed = false;
                }
            }

            yield return null;
            
            AttackableTiles[0].GetComponent<Attackable>().TestClick();
            AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();

        }
        yield return null;

        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Req4CTestUnitCantAttackSelf()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;


        GameObject forwardTile = null;
        //List<Units> pla = new List<int>();

        foreach (GameObject u in BoardMan.playerUnits)
        {
            Units unit = u.GetComponent<Units>();

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            //Show tiles
            unit.showMovable();
            yield return null;

            //Move to Tile
            List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

            //Find tile 1 to the right
            foreach (GameObject g in MoveableTiles)
            {
                if (g.GetComponent<Movable>().pos.x == unit.getPos().x + 1)
                {
                    forwardTile = g;
                    break;
                }
            }
            forwardTile.GetComponent<Movable>().TestClick();
            forwardTile.GetComponent<Movable>().OnMouseOver();
            yield return null;

            //Attack
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;
            unit.showAttackable();
            yield return null;
            List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();

            yield return null;

            foreach (GameObject tile in AttackableTiles)
            {
                if (unit.getPos() == tile.GetComponent<Attackable>().pos)
                    passed = false;
            }

            yield return null;

            AttackableTiles[0].GetComponent<Attackable>().TestClick();
            AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();

        }

        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Req3ATestUnitAttackingCannotBeSelectedAgain()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;


        GameObject forwardTile = null;

        foreach (GameObject u in BoardMan.playerUnits)
        {
            Units unit = u.GetComponent<Units>();

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            //Show tiles
            unit.showMovable();
            yield return null;

            //Move to Tile
            List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

            //Find tile 1 to the right
            foreach (GameObject g in MoveableTiles)
            {
                if (g.GetComponent<Movable>().pos.x == unit.getPos().x + 1)
                {
                    forwardTile = g;
                    break;
                }
            }
            forwardTile.GetComponent<Movable>().TestClick();
            forwardTile.GetComponent<Movable>().OnMouseOver();
            yield return null;

            //Attack
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;
            unit.showAttackable();
            yield return null;
            List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
            AttackableTiles[0].GetComponent<Attackable>().TestClick();
            AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();

        }
        yield return null;

        foreach (GameObject u in BoardMan.playerUnits)
        {
            if (u.GetComponent<Units>().canAct)
                passed = false;
        }

        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Req3BTestUnitMovedCanStillAttack()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;


        GameObject forwardTile = null;

        foreach (GameObject u in BoardMan.playerUnits)
        {
            Units unit = u.GetComponent<Units>();

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            //Show tiles
            unit.showMovable();
            yield return null;

            //Move to Tile
            List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

            //Find tile 1 to the right
            foreach (GameObject g in MoveableTiles)
            {
                if (g.GetComponent<Movable>().pos.x == unit.getPos().x + 1)
                {
                    forwardTile = g;
                    break;
                }
            }
            forwardTile.GetComponent<Movable>().TestClick();
            forwardTile.GetComponent<Movable>().OnMouseOver();

        }
        yield return null;

        foreach (GameObject u in BoardMan.playerUnits)
        {
            if (!u.GetComponent<Units>().canAct)
                passed = false;
        }

        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Req5ATestUnitCanMove()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        foreach (GameObject u in BoardMan.playerUnits)
        {
            Units unit = u.GetComponent<Units>();

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            //Show tiles
            unit.showMovable();
            yield return null;

            //Move to Tile
            List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

            if (!MoveableTiles.Any())
                passed = false;

        }
        yield return null;

        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Req5BTestUnitCanMoveTwoTiles()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        GameObject forwardTile = null;

        foreach (GameObject u in BoardMan.playerUnits)
        {
            Units unit = u.GetComponent<Units>();

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            //Show tiles
            unit.showMovable();
            yield return null;

            //Move to Tile
            List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

            //Find tile 1 to the right
            foreach (GameObject g in MoveableTiles)
            {
                if (g.GetComponent<Movable>().pos.x == unit.getPos().x + 1)
                {
                    forwardTile = g;
                    break;
                }
            }
            forwardTile.GetComponent<Movable>().TestClick();
            forwardTile.GetComponent<Movable>().OnMouseOver();
            

            //try moving again cause only moved one tile

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            //Show tiles
            unit.showMovable();
            yield return null;

            //Move to Tile
            MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

            if (!MoveableTiles.Any())
                passed = false;

        }
        yield return null;

        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Req6AExpectedFAILTestGodlyUnitExists()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        //ASSUMES FIRST UNIT SPAWNED WILL BE GOD
        Units unit = BoardMan.playerUnits[0].GetComponent<Units>();

        if (!unit.CheckIfGod())
            passed = false;

        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Req6BExpectedFAILTestChooseGodlyAbility()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        //ASSUMES FIRST UNIT SPAWNED WILL BE GOD
        Units unit = BoardMan.playerUnits[0].GetComponent<Units>();

        //Opens Menu
        unit.TestClick();
        unit.OnMouseOver();
        yield return null;
        
        //NOT YET IMPLEMENTED: ABILITIES
        passed = false;

        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Req6CExpectedFAILTestAbilityHappened()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        //ASSUMES FIRST UNIT SPAWNED WILL BE GOD
        Units unit = BoardMan.playerUnits[0].GetComponent<Units>();

        //Opens Menu
        unit.TestClick();
        unit.OnMouseOver();
        yield return null;

        //NOT YET IMPLEMENTED: ABILITIES
        passed = false;

        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Req7AFaithGeneratedatStartofTurn()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        Units unit = null;
        float initalMoral = BoardMan.GetPlayerFaith();

        foreach (GameObject g in BoardMan.playerUnits)
        {
            yield return null;
            unit = g.GetComponent<Units>();

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            unit.EndTurnButton();
            yield return null;
        }

        foreach (GameObject g in BoardMan.enemyUnits)
        {
            yield return null;
            unit = g.GetComponent<Units>();

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            unit.EndTurnButton();
            yield return null;
        }


        yield return null;

        if (initalMoral == BoardMan.GetPlayerFaith())
            passed = false;

        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Req7BFaithNotGeneratedduringEnemyTurn()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        Units unit = null;
        float initalMoral = BoardMan.GetPlayerFaith();

        foreach (GameObject g in BoardMan.playerUnits)
        {
            yield return null;
            unit = g.GetComponent<Units>();

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            unit.EndTurnButton();
            yield return null;
        }

        yield return null;

        if (initalMoral != BoardMan.GetPlayerFaith())
            passed = false;

        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Req7CFaithNotGeneratedduringPlayerTurn()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        Units unit = null;
        float initalMoral = BoardMan.GetPlayerFaith();

        foreach (GameObject g in BoardMan.playerUnits)
        {
            yield return null;
            unit = g.GetComponent<Units>();

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            unit.EndTurnButton();
            yield return null;

            if (initalMoral != BoardMan.GetPlayerFaith())
                passed = false;
            yield return null;
        }

        yield return null;

        

        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Req7CFaithDoesNotExceedCap()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        Units unit = null;

        for (int i = 0; i < 100; i++)
        {

            foreach (GameObject g in BoardMan.playerUnits)
            {
                yield return null;
                unit = g.GetComponent<Units>();

                //Opening Menu
                unit.TestClick();
                unit.OnMouseOver();
                yield return null;

                unit.EndTurnButton();
                yield return null;
            }

            foreach (GameObject g in BoardMan.enemyUnits)
            {
                yield return null;
                unit = g.GetComponent<Units>();

                //Opening Menu
                unit.TestClick();
                unit.OnMouseOver();
                yield return null;

                unit.EndTurnButton();
                yield return null;
            }

            if (BoardMan.GetPlayerFaith() > BoardMan.GetFaithCap())
                passed = false;
        }


        yield return null;

        

        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Req7DFaithDoesNotGoBelowZero()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        Units unit = null;

        for (int i = 0; i < 100; i++)
        {

            foreach (GameObject g in BoardMan.playerUnits)
            {
                yield return null;
                unit = g.GetComponent<Units>();

                //Opening Menu
                unit.TestClick();
                unit.OnMouseOver();
                yield return null;

                unit.useSkill(0);
                yield return null;

                unit.EndTurnButton();
                yield return null;
            }

            foreach (GameObject g in BoardMan.enemyUnits)
            {
                yield return null;
                unit = g.GetComponent<Units>();

                //Opening Menu
                unit.TestClick();
                unit.OnMouseOver();
                yield return null;

                unit.EndTurnButton();
                yield return null;
            }

            if (BoardMan.GetPlayerFaith() <= 0)
                passed = false;
        }


        yield return null;



        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Req9AHigherMoraleEqualsHigherAttackstr()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;
        
        Units unit = BoardMan.playerUnits[0].GetComponent<Units>();

        unit.WorshiperCount = 10;

        BoardMan.PlayerMorale = 0.2f;
        unit.updateAttackStrength();

        yield return null;

        float initalAttackStr = unit.getAttackStrength();

        yield return null;

        BoardMan.PlayerMorale = 1.0f;
        unit.updateAttackStrength();

        yield return null;

        if (initalAttackStr >= unit.getAttackStrength())
            passed = false;

        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Req9BMoraleChangesAttackStr()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        Units unit = BoardMan.playerUnits[0].GetComponent<Units>();

        unit.WorshiperCount = 10;

        BoardMan.PlayerMorale = 0.2f;
        unit.updateAttackStrength();

        yield return null;

        float initalAttackStr = unit.getAttackStrength();

        yield return null;

        BoardMan.PlayerMorale = 1.0f;
        unit.updateAttackStrength();

        yield return null;

        if (initalAttackStr == unit.getAttackStrength())
            passed = false;

        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Req9CAttackStrisnotNegative()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        Units unit = BoardMan.playerUnits[0].GetComponent<Units>();

        unit.WorshiperCount = 10;

        BoardMan.PlayerMorale = 0.2f;
        unit.updateAttackStrength();

        yield return null;
        if (unit.getAttackStrength() > 0)
            passed = false;

        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Requ11ACheckifKillingAnEnemyRemovesitFromtheList()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        int initalLength = BoardMan.enemyUnits.Count;

        GameObject forwardTile = null;
        List<int> initalPosX = new List<int>();
        GameObject u = BoardMan.playerUnits[0];
        Units unit = u.GetComponent<Units>();

        //Opening Menu
        unit.TestClick();
        unit.OnMouseOver();
        yield return null;

        //Show tiles
        unit.showMovable();
        yield return null;

        //Move to Tile
        List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

        //Find tile 1 to the right
        foreach (GameObject g in MoveableTiles)
        {
            if (g.GetComponent<Movable>().pos.x == unit.getPos().x + 1)
            {
                forwardTile = g;
                break;
            }
        }
        forwardTile.GetComponent<Movable>().TestClick();
        forwardTile.GetComponent<Movable>().OnMouseOver();
        yield return null;

        //Attack
        unit.TestClick();
        unit.OnMouseOver();
        yield return null;
        unit.showAttackable();
        yield return null;
        List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
        AttackableTiles[0].GetComponent<Attackable>().TestClick();
        AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();

        yield return null;

        if (initalLength == BoardMan.enemyUnits.Count)
            passed = false;

        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Requ11BExpectedFailKillingGodUnitEndsBattle()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        int initalLength = BoardMan.enemyUnits.Count;

        GameObject forwardTile = null;
        List<int> initalPosX = new List<int>();

        GameObject u = BoardMan.playerUnits[0];
        Units unit = u.GetComponent<Units>();

        BoardMan.enemyUnits[0].GetComponent<Units>().setGod();

        //Opening Menu
        unit.TestClick();
        unit.OnMouseOver();
        yield return null;

        //Show tiles
        unit.showMovable();
        yield return null;

        //Move to Tile
        List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

        //Find tile 1 to the right
        foreach (GameObject g in MoveableTiles)
        {
            if (g.GetComponent<Movable>().pos.x == unit.getPos().x + 1)
            {
                forwardTile = g;
                break;
            }
        }
        forwardTile.GetComponent<Movable>().TestClick();
        forwardTile.GetComponent<Movable>().OnMouseOver();
        yield return null;

        //Attack
        unit.TestClick();
        unit.OnMouseOver();
        yield return null;
        unit.showAttackable();
        yield return null;
        List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
        AttackableTiles[0].GetComponent<Attackable>().TestClick();
        AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();

        yield return null;

        if (!BoardMan.endBattle)
            passed = false;

        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Requ11CAttackingWithoutKillDoesntKill()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        int initalLength = BoardMan.enemyUnits.Count;

        GameObject forwardTile = null;
        List<int> initalPosX = new List<int>();

        GameObject u = BoardMan.playerUnits[0];
        Units unit = u.GetComponent<Units>();

        BoardMan.enemyUnits[0].GetComponent<Units>().WorshiperCount = 10000;

        //Opening Menu
        unit.TestClick();
        unit.OnMouseOver();
        yield return null;

        //Show tiles
        unit.showMovable();
        yield return null;

        //Move to Tile
        List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

        //Find tile 1 to the right
        foreach (GameObject g in MoveableTiles)
        {
            if (g.GetComponent<Movable>().pos.x == unit.getPos().x + 1)
            {
                forwardTile = g;
                break;
            }
        }
        forwardTile.GetComponent<Movable>().TestClick();
        forwardTile.GetComponent<Movable>().OnMouseOver();
        yield return null;

        //Attack
        unit.TestClick();
        unit.OnMouseOver();
        yield return null;
        unit.showAttackable();
        yield return null;
        List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
        AttackableTiles[0].GetComponent<Attackable>().TestClick();
        AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();

        yield return null;

        if (initalLength != BoardMan.enemyUnits.Count)
            passed = false;

        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Requ12EnemyCanKillPlayerUnit()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        int initalLength = BoardMan.playerUnits.Count;

        GameObject forwardTile = null;
        List<int> initalPosX = new List<int>();

        GameObject u = BoardMan.playerUnits[0];
        Units unit = u.GetComponent<Units>();

        BoardMan.enemyUnits[0].GetComponent<Units>().WorshiperCount = 10000;

        foreach (GameObject g in BoardMan.playerUnits)
        {
            yield return null;
            unit = g.GetComponent<Units>();

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            unit.useSkill(0);
            yield return null;

            unit.EndTurnButton();
            yield return null;
        }

        u = BoardMan.enemyUnits[0];
        unit = u.GetComponent<Units>();

        //Opening Menu
        unit.TestClick();
        unit.OnMouseOver();
        yield return null;

        //Show tiles
        unit.showMovable();
        yield return null;

        //Move to Tile
        List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

        //Find tile 1 to the right
        foreach (GameObject g in MoveableTiles)
        {
            if (g.GetComponent<Movable>().pos.x == unit.getPos().x - 1)
            {
                forwardTile = g;
                break;
            }
        }
        forwardTile.GetComponent<Movable>().TestClick();
        forwardTile.GetComponent<Movable>().OnMouseOver();
        yield return null;

        //Attack
        unit.TestClick();
        unit.OnMouseOver();
        yield return null;
        unit.showAttackable();
        yield return null;
        List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
        AttackableTiles[0].GetComponent<Attackable>().TestClick();
        AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();

        yield return null;

        if (initalLength == BoardMan.playerUnits.Count)
            passed = false;

        Assert.True(passed);

    }

    [UnityTest]
    public IEnumerator Req8ExpectedFAILCanSplitWorshipers()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        passed = SetupMan.SplitWorshipers(); //dummy function

        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Requ10AUnitCanDie()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;


        //end player's turn
        foreach (GameObject u in BoardMan.playerUnits)
        {
            Units unit = u.GetComponent<Units>();

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            unit.EndTurnButton();
        }

        yield return null;


        GameObject forwardTile = null;

        Units enemyUnit = BoardMan.enemyUnits[0].GetComponent<Units>();

        enemyUnit.TestClick();
        enemyUnit.OnMouseOver();
        yield return null;


        //Show tiles
        enemyUnit.showMovable();
        yield return null;

        //Move to Tile
        List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

        //Find tile 1 to the left
        foreach (GameObject g in MoveableTiles)
        {
            if (g.GetComponent<Movable>().pos.x == enemyUnit.getPos().x - 1)
            {
                forwardTile = g;
                break;
            }
        }
        forwardTile.GetComponent<Movable>().TestClick();
        forwardTile.GetComponent<Movable>().OnMouseOver();

        int playerUnitsCount = BoardMan.playerUnits.Count;

        //Attack
        enemyUnit.TestClick();
        enemyUnit.OnMouseOver();
        yield return null;
        enemyUnit.showAttackable();
        yield return null;
        List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
        AttackableTiles[0].GetComponent<Attackable>().TestClick();
        AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();

        yield return null;

        if (playerUnitsCount <= BoardMan.playerUnits.Count)
            passed = false;

        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Requ10BDifferentAttackStrengths()
    {

        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        Units unit1 = BoardMan.playerUnits[0].GetComponent<Units>();
        Units unit2 = BoardMan.playerUnits[1].GetComponent<Units>();

        yield return null;

        unit1.setWorshiperCount(100);
        unit2.setWorshiperCount(57);

        unit1.setMorale(1.0f);
        unit2.setMorale(1.0f);

        yield return null;

        unit1.updateAttackStrength();
        unit2.updateAttackStrength();

        yield return null;

        if (unit1.getAttackStrength() == unit2.getAttackStrength())
            passed = false;

        //Checks to see if we added one unit
        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Requ10CSameAttackStrength()
    {

        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        Units unit1 = BoardMan.playerUnits[0].GetComponent<Units>();
        Units unit2 = BoardMan.playerUnits[1].GetComponent<Units>();

        unit1.setWorshiperCount(100);
        unit2.setWorshiperCount(100);

        unit1.setMorale(1.0f);
        unit2.setMorale(1.0f);

        yield return null;

        unit1.updateAttackStrength();
        unit2.updateAttackStrength();

        yield return null;

        if (unit1.getAttackStrength() != unit2.getAttackStrength())
            passed = false;

        //Checks to see if we added one unit
        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Requ13AExpectedFAILSurrender()
    {

        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        passed = BoardMan.surrender(); //not implemented so it's expected to fail; dummy function for now

        //Checks to see if we added one unit
        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Requ13BExpectedFAILReturnToManagementMode()
    {

        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        passed = BoardMan.surrender(); //since the game isn't meant to start in combat mode, this will fail 
        //HOWEVER starting in management mode, going into combat and then back into management mode WORKS

        //Checks to see if we added one unit
        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Requ13CExpectedFAILAdjustMoraleAfterSurrender()
    {

        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;

        passed = BoardMan.surrender(); //since surrender isn't implemented yet, we aren't properly adjusting values
        Assert.True(passed);
    }

    [UnityTest]
    public IEnumerator Requ14ExpectedFAILAdjustWorshiperCountAfterUnitDeath()
    {
        bool passed = true;

        SceneManager.LoadScene("CombatMode");

        //Always give it a sec
        yield return null;

        SetupManager SetupMan = GameObject.FindGameObjectWithTag("SetupManager").GetComponent<SetupManager>();
        MapManager MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardManager BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return null;


        //end player's turn
        foreach (GameObject u in BoardMan.playerUnits)
        {
            Units unit = u.GetComponent<Units>();

            //Opening Menu
            unit.TestClick();
            unit.OnMouseOver();
            yield return null;

            unit.EndTurnButton();
        }

        yield return null;


        GameObject forwardTile = null;

        Units enemyUnit = BoardMan.enemyUnits[0].GetComponent<Units>();

        enemyUnit.TestClick();
        enemyUnit.OnMouseOver();
        yield return null;


        //Show tiles
        enemyUnit.showMovable();
        yield return null;

        //Move to Tile
        List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

        //Find tile 1 to the left
        foreach (GameObject g in MoveableTiles)
        {
            if (g.GetComponent<Movable>().pos.x == enemyUnit.getPos().x - 1)
            {
                forwardTile = g;
                break;
            }
        }
        forwardTile.GetComponent<Movable>().TestClick();
        forwardTile.GetComponent<Movable>().OnMouseOver();

        int initialPlayerWorshiperCount = 999; //FIX HERE, add worshiper count into BoardMan that gets updated during combat

        //Attack
        enemyUnit.TestClick();
        enemyUnit.OnMouseOver();
        yield return null;
        enemyUnit.showAttackable();
        yield return null;
        List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
        AttackableTiles[0].GetComponent<Attackable>().TestClick();
        AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();

        yield return null;

        if (initialPlayerWorshiperCount > 5)
            passed = false;

        Assert.True(passed);
    }
    */
}
