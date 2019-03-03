using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject unitPanel;
    public GameObject unitButton;
    public GameObject abilityPanel;
    public GameObject abilityButton;

    private int uiPadding;

    public GameObject selectedMenu;

    public Canvas screenCanvas;

    private MapManager MapMan;
    private BoardManager BoardMan;

    // Start is called before the first frame update
    void Start()
    {
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        //Padding for ui
        uiPadding = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (screenCanvas.transform.childCount != 0 && MapMan.Selected != null)
        {
            selectedMenu = screenCanvas.transform.GetChild(0).gameObject;

            RectTransform selectedMenuPanelRect = selectedMenu.GetComponent<RectTransform>();
            selectedMenuPanelRect.anchoredPosition = worldToScreenSpace(selectedMenu);
        }
        //For Selected Unit
        if (MapMan.Selected != null && MapMan.newSelected)
        {
            //Clean up Previous selection (the tiles)
            MapMan.ClearSelection();

            //Clear previous menus
            removeMenu();

            makePanel(unitPanel);


            if (MapMan.Selected.GetComponent<UnitObjectScript>().getUnit() is God)
            {
                makeGodButtons();

            }
            else if (MapMan.Selected.GetComponent<UnitObjectScript>().getUnit() is Unit)
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

            MapMan.previousSelected = MapMan.Selected;

            //So we don't do this every frame
            MapMan.newSelected = false;

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
        attackButton.GetComponent<Button>().onClick.AddListener(delegate { BoardMan.showAttackable(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()); });

        GameObject moveButton = Instantiate(unitButton);
        moveButton.transform.SetParent(selectedMenu.transform);

        //Lalala dumb scaling (this one actually isn't that dumb)
        moveButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 1 / 2f * (selectedMenu.GetComponent<RectTransform>().rect.height));

        moveButton.GetComponentInChildren<Text>().text = "Move";
        moveButton.GetComponent<Button>().onClick.AddListener(delegate { BoardMan.showMovable(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()); });
    }

    //Make the god button (when out of battle). Similar to above.
    private void makeGodButtons()
    {
        GameObject abilitiesButton = Instantiate(unitButton);
        abilitiesButton.transform.SetParent(selectedMenu.transform);
        abilitiesButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,
            selectedMenu.GetComponent<RectTransform>().rect.height - abilitiesButton.GetComponent<RectTransform>().rect.height / (2 * selectedMenu.GetComponent<RectTransform>().localScale.y) - uiPadding);
        abilitiesButton.GetComponentInChildren<Text>().text = "Abilites";
        abilitiesButton.GetComponent<Button>().onClick.AddListener(delegate { showAbilities((MapMan.Selected.GetComponent<UnitObjectScript>().getUnit() as God).getAbilites()); });

        GameObject enterBattleButton = Instantiate(unitButton);
        enterBattleButton.transform.SetParent(selectedMenu.transform);
        enterBattleButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 1 / 2f * (selectedMenu.GetComponent<RectTransform>().rect.height));
        enterBattleButton.GetComponentInChildren<Text>().text = "Enter Battle";
        enterBattleButton.GetComponent<Button>().onClick.AddListener(delegate { BoardMan.showAttackable(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()); });
    }

    //Make the end turn button. Not specific to god or unit, so it gets it's own function. What a special snowflake.
    private void makeEndTurnButton()
    {
        GameObject endTurnButton = Instantiate(unitButton);
        endTurnButton.transform.SetParent(selectedMenu.transform);
        endTurnButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,
            endTurnButton.GetComponent<RectTransform>().rect.height / (2 * selectedMenu.GetComponent<RectTransform>().localScale.y) + uiPadding);
        endTurnButton.GetComponentInChildren<Text>().text = "End";
        endTurnButton.GetComponent<Button>().onClick.AddListener(delegate { MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().EndTurnButton(); });
    }

    //World to screen space postitioning from https://answers.unity.com/questions/799616/unity-46-beta-19-how-to-convert-from-world-space-t.html
    private Vector2 worldToScreenSpace(GameObject Go)
    {


        GameObject selectedMenu = screenCanvas.transform.GetChild(0).gameObject;

        RectTransform CanvasRect = screenCanvas.GetComponent<RectTransform>();

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(MapMan.Selected.transform.position);

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
        if (MapMan.previousSelected != null)
            foreach (Transform ui in screenCanvas.transform)
                Destroy(ui.gameObject);
    }
}
