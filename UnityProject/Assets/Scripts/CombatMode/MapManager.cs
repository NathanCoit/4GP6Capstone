using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Concerned with the actual map (tiles) of combat mode.
 * Involves:
 *      - connections between tiles
 *      - which tiles a Unit can move onto
 *      - who is selected? in terms of units/worshippers/gods (helpful when trying to decide who is attacking or whose menu to bring up)
 */
public class MapManager : MonoBehaviour
{
    public Tile[,] tiles;

    private List<GameObject> Movable;
    private GameObject SelectionIndicator;

    public GameObject Unit;
    public GameObject MovableTile;
    public GameObject Selected;
    private GameObject previousSelected;
    private BoardManager BoardMan;
    public bool newSelected = false;
    public string mapName;
    public float godFloatHeight;

    public GameObject unitPanel;
    public GameObject unitButton;
    public GameObject abilityPanel;
    public GameObject abilityButton;

    private int uiPadding;

    public GameObject selectedMenu;

    public Canvas screenCanvas;

    // Use this for initialization. We're loading our maps from a txt and building them here so we can easily add more maps without the need for more scenes.
    void Start ()
    {
        //If we don't have a specified map, it's testmap
        if (mapName == "")
            mapName = "testMap";

        TextAsset map = Resources.Load("CMaps/" + mapName) as TextAsset;

        //Splitting on newline from https://stackoverflow.com/questions/1547476/easiest-way-to-split-a-string-on-newlines-in-net
        string[] lines = map.text.Split(
        new[] { "\r\n", "\r", "\n" },
        System.StringSplitOptions.None);

        tiles = new Tile[lines[0].Split('-').Length, lines.Length];

        //Initiliaze Grid
        for (int y = 0; y < lines.Length; y++)
        {
            //Read back to front to match how it looks in the text file
            string[] chars = lines[(lines.Length - 1) - y].Split('-');

            for(int x = 0; x < chars.Length; x++)
            {
                tiles[x, y] = new Tile(new Vector3(x, 0, y), chars[x]);
            }
        }

        //Made as a seperate function as we need to do it a lot
        DefineConnections();

        Movable = new List<GameObject>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        //How high gods float above the map
        godFloatHeight = 3.5f;

        //Padding for ui
        uiPadding = 10;


    }

	// Update is called once per frame
	void Update ()
    {
        if (screenCanvas.transform.childCount != 0 && Selected != null)
        {
            selectedMenu = screenCanvas.transform.GetChild(0).gameObject;

            RectTransform selectedMenuPanelRect = selectedMenu.GetComponent<RectTransform>();
            selectedMenuPanelRect.anchoredPosition = worldToScreenSpace(selectedMenu);
        }
        //For Selected Unit
        if (Selected != null && newSelected)
        {
            //Clean up Previous selection (the tiles)
            ClearSelection();

            //Clear previous menus
            removeMenu();

            makePanel(unitPanel);
            

            if (Selected.GetComponent<UnitObjectScript>().getUnit() is God)
            {
                makeGodButtons();

            }
            else if (Selected.GetComponent<UnitObjectScript>().getUnit() is Unit)
            {

                //Add all the buttons
                makeUnitButtons();

            }


            makeEndTurnButton();
            

            //TODO FIX GOD MENUS ONCE WE ACTUALL MAKE THE GOD CLASS

            /*
            if (!Selected.CheckIfGod())
                Selected.unitGameObject().GetComponent<Canvas>().gameObject.SetActive(true);
            else if(Selected.GetComponent<Gods>().isInBattle())
                Selected.transform.GetChild(0).GetComponent<Canvas>().gameObject.SetActive(true);
            else if(!Selected.GetComponent<Gods>().isInBattle())
                Selected.transform.GetChild(1).GetComponent<Canvas>().gameObject.SetActive(true);
            */

            previousSelected = Selected;

            //So we don't do this every frame
            newSelected = false;
            
        }


    }

    //For making the abilities menu
    private void showAbilities(string[] Abilities)
    {
        //Get rid of old buttons and panels
        removeMenu();

        //Replace with the larger panel
        makePanel(abilityPanel);

        //Make the return button
        GameObject endTurnButton = Instantiate(abilityButton);
        endTurnButton.transform.SetParent(selectedMenu.transform);
        endTurnButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 
            endTurnButton.GetComponent<RectTransform>().rect.height / (2 * selectedMenu.GetComponent<RectTransform>().localScale.y) + uiPadding);
        endTurnButton.GetComponentInChildren<Text>().text = "Return";
        endTurnButton.GetComponent<Button>().onClick.AddListener(delegate { removeMenu(); makePanel(unitPanel); makeGodButtons(); makeEndTurnButton(); });

        Debug.Log(selectedMenu.GetComponent<RectTransform>().rect.height);

        //Make ability buttons
        for (int i = 0; i < Abilities.Length; i++)
        {
            //Instantiate new button and make it the child of our panel
            GameObject newAbilityButton = Instantiate(abilityButton);
            newAbilityButton.transform.SetParent(selectedMenu.transform);

            //Screw unity ui scaling. This is very strange and long but it works
            newAbilityButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,
                (abilityButton.GetComponent<RectTransform>().rect.height / (2 * selectedMenu.GetComponent<RectTransform>().localScale.y) + uiPadding)
                + (i + 1) * (selectedMenu.GetComponent<RectTransform>().rect.height - Abilities.Length * uiPadding) / (Abilities.Length + 1));

            //Set appopritate text
            newAbilityButton.GetComponentInChildren<Text>().text = Abilities[i];

            //Add listener to acutally use stuff
            newAbilityButton.GetComponent<Button>().onClick.AddListener(delegate { BoardMan.useAbility(newAbilityButton.GetComponentInChildren<Text>().text); });
        }
    }

    //Makes the regular sized unit panel
    private void makePanel(GameObject panel)
    {
        RectTransform CanvasRect = screenCanvas.GetComponent<RectTransform>();
        GameObject newUnitPanel = Instantiate(panel);
        newUnitPanel.transform.SetParent(CanvasRect);

        RectTransform newUnitPanelRect = newUnitPanel.GetComponent<RectTransform>();
        newUnitPanelRect.anchoredPosition = worldToScreenSpace(newUnitPanel);

        selectedMenu = newUnitPanel;
    }

    //Makes the attack and move buttons
    private void makeUnitButtons()
    {
        GameObject attackButton = Instantiate(unitButton);
        attackButton.transform.SetParent(selectedMenu.transform);

        //Don't mind the dumb scaling. Blame unity.
        attackButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 
            selectedMenu.GetComponent<RectTransform>().rect.height - attackButton.GetComponent<RectTransform>().rect.height / (2 * selectedMenu.GetComponent<RectTransform>().localScale.y) - uiPadding);

        attackButton.GetComponentInChildren<Text>().text = "Attack";
        attackButton.GetComponent<Button>().onClick.AddListener(delegate { BoardMan.showAttackable(Selected.GetComponent<UnitObjectScript>().getUnit()); });

        GameObject moveButton = Instantiate(unitButton);
        moveButton.transform.SetParent(selectedMenu.transform);

        //Lalala dumb scaling (this one actually isn't that dumb)
        moveButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 1/2f * (selectedMenu.GetComponent<RectTransform>().rect.height));

        moveButton.GetComponentInChildren<Text>().text = "Move";
        moveButton.GetComponent<Button>().onClick.AddListener(delegate { BoardMan.showMovable(Selected.GetComponent<UnitObjectScript>().getUnit()); });
    }

    //Make the god button (when out of battle). Similar to above.
    private void makeGodButtons()
    {
        GameObject abilitiesButton = Instantiate(unitButton);
        abilitiesButton.transform.SetParent(selectedMenu.transform);
        abilitiesButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 
            selectedMenu.GetComponent<RectTransform>().rect.height - abilitiesButton.GetComponent<RectTransform>().rect.height / (2 * selectedMenu.GetComponent<RectTransform>().localScale.y) - uiPadding);
        abilitiesButton.GetComponentInChildren<Text>().text = "Abilites";
        abilitiesButton.GetComponent<Button>().onClick.AddListener(delegate { showAbilities((Selected.GetComponent<UnitObjectScript>().getUnit() as God).getAbilites()); });

        GameObject enterBattleButton = Instantiate(unitButton);
        enterBattleButton.transform.SetParent(selectedMenu.transform);
        enterBattleButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 1 / 2f * (selectedMenu.GetComponent<RectTransform>().rect.height));
        enterBattleButton.GetComponentInChildren<Text>().text = "Enter Battle";
        enterBattleButton.GetComponent<Button>().onClick.AddListener(delegate { BoardMan.showAttackable(Selected.GetComponent<UnitObjectScript>().getUnit()); });
    }

    //Make the end turn button. Not specific to god or unit, so it gets it's own function. What a special snowflake.
    private void makeEndTurnButton()
    {
        GameObject endTurnButton = Instantiate(unitButton);
        endTurnButton.transform.SetParent(selectedMenu.transform);
        endTurnButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 
            endTurnButton.GetComponent<RectTransform>().rect.height / (2 * selectedMenu.GetComponent<RectTransform>().localScale.y) + uiPadding);
        endTurnButton.GetComponentInChildren<Text>().text = "End";
        endTurnButton.GetComponent<Button>().onClick.AddListener(delegate { Selected.GetComponent<UnitObjectScript>().getUnit().EndTurnButton(); });
    }

    //World to screen space postitioning from https://answers.unity.com/questions/799616/unity-46-beta-19-how-to-convert-from-world-space-t.html
    private Vector2 worldToScreenSpace(GameObject Go)
    {
        

        GameObject selectedMenu = screenCanvas.transform.GetChild(0).gameObject;

        RectTransform CanvasRect = screenCanvas.GetComponent<RectTransform>();

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(Selected.transform.position);

        //Dont really know what this is doing, but it works
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        return WorldObject_ScreenPosition;
    }

    //Clear menu without killing the panel. Currently not using, but could be useful later
    private void clearMenu()
    {
        foreach (Transform button in selectedMenu.transform)
            Destroy(button.gameObject);
    }

    //Kill all menus
    public void removeMenu()
    {
        //Kill all children code from https://answers.unity.com/questions/611850/destroy-all-children-of-object.html
        if (previousSelected != null)
            foreach (Transform ui in screenCanvas.transform)
                Destroy(ui.gameObject);
    }

    //Define which tiles are connected to which tiles
    public void DefineConnections()
    {
        //Define Connections
        for (int x = 0; x < tiles.GetLength(0); x++)
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                Tile temp = tiles[x, y];
                List<Tile> tempConnections = new List<Tile>();
                if (y < tiles.GetLength(1) - 1 && tiles[x, y + 1].isTraversable())
                {
                    tempConnections.Add(tiles[x, y + 1]);
                }
                if (x < tiles.GetLength(0) - 1 && tiles[x + 1, y].isTraversable())
                {
                    tempConnections.Add(tiles[x + 1, y]);
                }
                if (y > 0 && tiles[x, y - 1].isTraversable())
                {
                    tempConnections.Add(tiles[x, y - 1]);
                }
                if (x > 0 && tiles[x - 1, y].isTraversable())
                {
                    tempConnections.Add(tiles[x - 1, y]);
                }
                temp.updateConnections(tempConnections);
                tiles[x, y] = temp;
            }
    }

    //Cleans all the tiles (the ones that go on top of the actual tiles (the interactable ones))
    public void ClearSelection()
    {
        //Clean up tiles
        GameObject[] movableTiles = GameObject.FindGameObjectsWithTag("MoveableTile");
        for (int i = 0; i < movableTiles.GetLength(0); i++)
            Destroy(movableTiles[i]);

        GameObject[] attackableTiles = GameObject.FindGameObjectsWithTag("AttackableTile");
        for (int i = 0; i < attackableTiles.GetLength(0); i++)
            Destroy(attackableTiles[i]);

        GameObject[] TargetableTiles = GameObject.FindGameObjectsWithTag("TargetableTile");
        for (int i = 0; i < TargetableTiles.GetLength(0); i++)
            Destroy(TargetableTiles[i]);
    }

    //For making the gameObject of a tile (the real ones)
    public void InstantiateTile(string typeID, Vector3 pos)
    {
        GameObject tileGameObject = Instantiate(Resources.Load("Tiles/" + typeID) as GameObject);

        //Cenetering
        tileGameObject.transform.position 
            = new Vector3(pos.x + ((1 - tileGameObject.transform.lossyScale.x) / 2) + tileGameObject.transform.lossyScale.x / 2,
            0,
            pos.z + ((1 - tileGameObject.transform.lossyScale.z) / 2) + tileGameObject.transform.lossyScale.x / 2);
    }
}