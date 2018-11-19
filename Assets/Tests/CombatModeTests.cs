﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

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

    //Test ending turn without doing anything
    [UnityTest]
    public IEnumerator TestUnitEndTurn()
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
            unit.testClick();
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
    public IEnumerator TestUnitsMove()
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
            unit.testClick();
            unit.OnMouseOver();
            yield return null;

            //Show tiles
            unit.showMovable();
            yield return null;

            //Move to Tile
            List<GameObject> MoveableTiles = GameObject.FindGameObjectsWithTag("MoveableTile").ToList();

            //Find tile 1 to the right
            foreach(GameObject g in MoveableTiles)
            {
                if(g.GetComponent<Movable>().pos.x == unit.getPos().x + 1)
                {
                    forwardTile = g;
                    break;
                }
            }
            forwardTile.GetComponent<Movable>().testClick();
            forwardTile.GetComponent<Movable>().OnMouseOver();
            yield return null;

            //End turn
            unit.testClick();
            unit.OnMouseOver();
            yield return null;
            unit.EndTurnButton();
            yield return null;
        }
        yield return null;
        foreach(int i in initalPosX)
        {
            foreach(GameObject g in BoardMan.playerUnits)
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
    public IEnumerator TestUnitsMoveAttack()
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
            unit.testClick();
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
            forwardTile.GetComponent<Movable>().testClick();
            forwardTile.GetComponent<Movable>().OnMouseOver();
            yield return null;

            //Attack
            unit.testClick();
            unit.OnMouseOver();
            yield return null;
            unit.showAttackable();
            yield return null;
            List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
            AttackableTiles[0].GetComponent<Attackable>().testClick();
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
    public IEnumerator TestUnitsMoveAttackorEndTurn()
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
                unit.testClick();
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
                forwardTile.GetComponent<Movable>().testClick();
                forwardTile.GetComponent<Movable>().OnMouseOver();
                yield return null;

                //Attack
                unit.testClick();
                unit.OnMouseOver();
                yield return null;
                unit.showAttackable();
                yield return null;
                List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
                AttackableTiles[0].GetComponent<Attackable>().testClick();
                AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();

                flip = false;
            }
            else
            {
                //End turn
                unit.testClick();
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
    public IEnumerator TestUnitEndingTurn()
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

        unit.testClick();
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
    public IEnumerator TestUnitAttackingEndingTurn()
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
        unit.testClick();
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
        forwardTile.GetComponent<Movable>().testClick();
        forwardTile.GetComponent<Movable>().OnMouseOver();
        yield return null;

        //Attack
        unit.testClick();
        unit.OnMouseOver();
        yield return null;
        unit.showAttackable();
        yield return null;
        List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
        AttackableTiles[0].GetComponent<Attackable>().testClick();
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
            unit.testClick();
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
            forwardTile.GetComponent<Movable>().testClick();
            forwardTile.GetComponent<Movable>().OnMouseOver();
            yield return null;

            //Attack
            unit.testClick();
            unit.OnMouseOver();
            yield return null;
            unit.showAttackable();
            yield return null;
            List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
            AttackableTiles[0].GetComponent<Attackable>().testClick();
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
            unit.testClick();
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
            forwardTile.GetComponent<Movable>().testClick();
            forwardTile.GetComponent<Movable>().OnMouseOver();
            yield return null;

            //Attack
            unit.testClick();
            unit.OnMouseOver();
            yield return null;
            unit.showAttackable();
            yield return null;
            List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
            //AttackableTiles[0].GetComponent<Attackable>().testClick();
            //AttackableTiles[0].GetComponent<Attackable>().OnMouseOver();

            yield return null;
            /* NEED HELP HERE
             * NEED HELP HERE
             * NEED HELP HERE
             * NEED HELP HERE
             * NEED HELP HERE
             * NEED HELP HERE
             * NEED HELP HERE
             */
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
            
            AttackableTiles[0].GetComponent<Attackable>().testClick();
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
            unit.testClick();
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
            forwardTile.GetComponent<Movable>().testClick();
            forwardTile.GetComponent<Movable>().OnMouseOver();
            yield return null;

            //Attack
            unit.testClick();
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

            AttackableTiles[0].GetComponent<Attackable>().testClick();
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
            unit.testClick();
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
            forwardTile.GetComponent<Movable>().testClick();
            forwardTile.GetComponent<Movable>().OnMouseOver();
            yield return null;

            //Attack
            unit.testClick();
            unit.OnMouseOver();
            yield return null;
            unit.showAttackable();
            yield return null;
            List<GameObject> AttackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile").ToList();
            AttackableTiles[0].GetComponent<Attackable>().testClick();
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
            unit.testClick();
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
            forwardTile.GetComponent<Movable>().testClick();
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
            unit.testClick();
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
            unit.testClick();
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
            forwardTile.GetComponent<Movable>().testClick();
            forwardTile.GetComponent<Movable>().OnMouseOver();
            

            //try moving again cause only moved one tile

            //Opening Menu
            unit.testClick();
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
        unit.testClick();
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
        unit.testClick();
        unit.OnMouseOver();
        yield return null;

        //NOT YET IMPLEMENTED: ABILITIES
        passed = false;

        Assert.True(passed);
    }
}
