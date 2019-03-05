﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject unitPanel;
    public GameObject unitButton;
    public GameObject abilityPanel;
    public GameObject abilityButton;
    public Camera mainCamera;

    private int uiPadding;

    public GameObject selectedMenu;

    public Canvas UICanvas;

    public bool godEnteringBattle;

    private MapManager MapMan;
    private BoardManager BoardMan;
    private SoundManager SoundMan;

    // Start is called before the first frame update
    void Start()
    {
        MapMan = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        BoardMan = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        //Dropping some phat beats right in here
        SoundMan = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();

        //Padding for ui
        uiPadding = 10;
    }

    // Update is called once per frame
    void Update()
    {
        //Unselecting unit by clicking off
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && !MapMan.newSelected && !MapMan.tilesPresent())
        {
            MapMan.ClearSelection();
            MapMan.Selected = null;
            removeMenu();
        }

        //Position menu if one is made (runs every frame if a menu is visible)
        if (UICanvas.transform.childCount != 0 && MapMan.Selected != null && MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().isPlayer)
        {
            selectedMenu = UICanvas.transform.GetChild(0).gameObject;

            RectTransform selectedMenuPanelRect = selectedMenu.GetComponent<RectTransform>();
            
            //Postioning
            selectedMenuPanelRect.anchoredPosition = worldToScreenSpace(UICanvas, selectedMenu, 
                new Vector3(MapMan.Selected.transform.position.x - 1, MapMan.Selected.transform.position.y + 1, MapMan.Selected.transform.position.z));

            //Scale based on distance to camera
            scaleOnCameraDistance(selectedMenu);
        }

        //When we select a new unit
        if (MapMan.Selected != null && MapMan.newSelected && MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().isPlayer)
        {
            //Clean up Previous selection (the tiles)
            MapMan.ClearSelection();

            //Clear previous menus
            removeMenu();

            //Hide menu while we make it
            hideMenu();

            makePanel(unitPanel);

            if (MapMan.Selected.GetComponent<UnitObjectScript>().getUnit() is God && !(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit() as God).isInBattle)
            {
                makeGodButtons();

                //Play god select sound
                SoundMan.playGodSelect();

            }
            else if (MapMan.Selected.GetComponent<UnitObjectScript>().getUnit() is Unit)
            {

                //Add all the buttons
                makeUnitButtons();

                //Play unit select sound
                SoundMan.playUnitSelect();

            }


            makeEndTurnButton();

            selectedMenu = UICanvas.transform.GetChild(0).gameObject;

            //Scale based on distance to camera
            scaleOnCameraDistance(selectedMenu);

            //Menu is done, now we show
            showMenu();


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
    public void makePanel(GameObject panel)
    {
        RectTransform CanvasRect = UICanvas.GetComponent<RectTransform>();
        GameObject newUnitPanel = Instantiate(panel);
        newUnitPanel.transform.SetParent(CanvasRect);

        RectTransform newUnitPanelRect = newUnitPanel.GetComponent<RectTransform>();
        newUnitPanelRect.anchoredPosition = worldToScreenSpace(UICanvas, newUnitPanel, 
            new Vector3 (MapMan.Selected.transform.position.x - 1, MapMan.Selected.transform.position.y + 1, MapMan.Selected.transform.position.z));

        selectedMenu = newUnitPanel;
    }

    //Makes the attack and move buttons
    public void makeUnitButtons()
    {
        //Attack button is at the top
        GameObject attackButton = Instantiate(unitButton);
        attackButton.transform.SetParent(selectedMenu.transform);

        //Don't mind the dumb scaling. Blame unity.
        attackButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,
            selectedMenu.GetComponent<RectTransform>().rect.height - attackButton.GetComponent<RectTransform>().rect.height / (2 * selectedMenu.GetComponent<RectTransform>().localScale.y) - uiPadding);

        attackButton.GetComponentInChildren<Text>().text = "Attack";
        attackButton.GetComponent<Button>().onClick.AddListener(delegate { BoardMan.showAttackable(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()); SoundMan.playUiSelect(); });

        //Event Tigger for showing preview
        EventTrigger attackButtonTrigger = attackButton.GetComponent<EventTrigger>();
        AddEventTrigger(attackButtonTrigger, onAttackButtonEnter, EventTriggerType.PointerEnter);
        AddEventTrigger(attackButtonTrigger, MapMan.ClearPreview, EventTriggerType.PointerExit);
        attackButton.tag = "attackButton";

        if (!BoardMan.canAttack(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()))
            attackButton.GetComponent<Button>().interactable = false;


        //Followed by the move button
        GameObject moveButton = Instantiate(unitButton);
        moveButton.transform.SetParent(selectedMenu.transform);

        //Lalala dumb scaling (this one actually isn't that dumb)
        moveButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 1 / 2f * (selectedMenu.GetComponent<RectTransform>().rect.height));

        moveButton.GetComponentInChildren<Text>().text = "Move";
        moveButton.GetComponent<Button>().onClick.AddListener(delegate { BoardMan.showMovable(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()); SoundMan.playUiSelect(); });

        //Event Tigger for showing preview
        EventTrigger moveButtonTrigger = moveButton.GetComponent<EventTrigger>();
        AddEventTrigger(moveButtonTrigger, onMoveButtonEneter, EventTriggerType.PointerEnter);
        AddEventTrigger(moveButtonTrigger, MapMan.ClearPreview, EventTriggerType.PointerExit);
        moveButton.tag = "moveButton";
    }

    private void onAttackButtonEnter()
    {
        BoardMan.previewAttackable();
        if(UICanvas.transform.GetChild(0).transform.GetChild(0).GetComponent<Button>().interactable)
            SoundMan.playUiHover();
    }

    private void onMoveButtonEneter()
    {
        BoardMan.previewMoveable();
        SoundMan.playUiHover();
    }

    //Make the god button (when out of battle). Similar to above.
    private void makeGodButtons()
    {
        GameObject abilitiesButton = Instantiate(unitButton);
        abilitiesButton.transform.SetParent(selectedMenu.transform);
        abilitiesButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,
            selectedMenu.GetComponent<RectTransform>().rect.height - abilitiesButton.GetComponent<RectTransform>().rect.height / (2 * selectedMenu.GetComponent<RectTransform>().localScale.y) - uiPadding);
        abilitiesButton.GetComponentInChildren<Text>().text = "Abilites";
        abilitiesButton.GetComponent<Button>().onClick.AddListener(delegate { showAbilities((MapMan.Selected.GetComponent<UnitObjectScript>().getUnit() as God).getAbilites()); SoundMan.playUiSelect(); });

        EventTrigger abilityButtonTrigger = abilitiesButton.GetComponent<EventTrigger>();
        AddEventTrigger(abilityButtonTrigger, SoundMan.playUiHover, EventTriggerType.PointerEnter);

        GameObject enterBattleButton = Instantiate(unitButton);
        enterBattleButton.transform.SetParent(selectedMenu.transform);
        enterBattleButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 1 / 2f * (selectedMenu.GetComponent<RectTransform>().rect.height));
        enterBattleButton.GetComponentInChildren<Text>().text = "Enter Battle";
        enterBattleButton.GetComponent<Button>().onClick.AddListener(delegate { (MapMan.Selected.GetComponent<UnitObjectScript>().getUnit() as God).enterBattle(); SoundMan.playUiSelect(); });

        EventTrigger enterBattleButtonTrigger = enterBattleButton.GetComponent<EventTrigger>();
        AddEventTrigger(enterBattleButtonTrigger, SoundMan.playUiHover, EventTriggerType.PointerEnter);
    }

    //Make the end turn button. Not specific to god or unit, so it gets it's own function. What a special snowflake.
    public void makeEndTurnButton()
    {
        GameObject endTurnButton = Instantiate(unitButton);
        endTurnButton.transform.SetParent(selectedMenu.transform);
        endTurnButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,
            endTurnButton.GetComponent<RectTransform>().rect.height / (2 * selectedMenu.GetComponent<RectTransform>().localScale.y) + uiPadding);
        endTurnButton.GetComponentInChildren<Text>().text = "End";
        endTurnButton.GetComponent<Button>().onClick.AddListener(delegate { MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().EndTurnButton(); });

        EventTrigger endTurnButtonTrigger = endTurnButton.GetComponent<EventTrigger>();
        AddEventTrigger(endTurnButtonTrigger, SoundMan.playUiHover, EventTriggerType.PointerEnter);
    }

    //World to screen space postitioning from https://answers.unity.com/questions/799616/unity-46-beta-19-how-to-convert-from-world-space-t.html
    public Vector2 worldToScreenSpace(Canvas canvas,GameObject uiElement, Vector3 center)
    {
        RectTransform CanvasRect = canvas.GetComponent<RectTransform>();


        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(center);

        //Dont really know what this is doing, but it works
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        return WorldObject_ScreenPosition;
    }

    //Scales a ui element based on the distance between selected and the camera (only z and y, it looks stupid if you do x as well)
    private void scaleOnCameraDistance(GameObject ui)
    {
        ui.transform.localScale = new Vector3(0.5f, 1, 1) * 1 / Vector3.Distance(new Vector3(0, MapMan.Selected.transform.position.y, MapMan.Selected.transform.position.z),
                        new Vector3(0, Camera.main.transform.position.y, Camera.main.transform.position.z));
    }

    //Show menu if a unit can still act, hides it otherwise. Used after an action.
    public void showMenuIfCanAct()
    {
        if (BoardMan.canMove(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()) || BoardMan.canAttack(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()))
        {
            if (BoardMan.canAttack(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()))
                UICanvas.transform.GetChild(0).transform.GetChild(0).GetComponent<Button>().interactable = true;
            else
                UICanvas.transform.GetChild(0).transform.GetChild(0).GetComponent<Button>().interactable = false;

            if (BoardMan.canMove(MapMan.Selected.GetComponent<UnitObjectScript>().getUnit()))
                UICanvas.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>().interactable = true;
            else
                UICanvas.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>().interactable = false;

            showMenu();
        }
        else
        {
            //AUTO END TURN IF WE CAN ACT WOOOOOOOO WE DID IT BOIS
            MapMan.Selected.GetComponent<UnitObjectScript>().getUnit().EndTurnButton();

            //Unselect, as we can act anymore
            MapMan.Selected = null;

            //eh hem, clean up the menus
            removeMenu();
        }
    }

    //Add event tigger code from https://answers.unity.com/questions/781726/how-do-i-add-a-listener-to-onpointerenter-ugui.html
    private void AddEventTrigger(EventTrigger eventTrigger, UnityAction action, EventTriggerType triggerType)
    {
        // Create a nee TriggerEvent and add a listener
        EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
        trigger.AddListener((eventData) => action()); // you can capture and pass the event data to the listener

        // Create and initialise EventTrigger.Entry using the created TriggerEvent
        EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };

        // Add the EventTrigger.Entry to delegates list on the EventTrigger
        eventTrigger.triggers.Add(entry);
    }

    public void hideMenu()
    {
        UICanvas.enabled = false;
    }

    public void showMenu()
    {
        UICanvas.enabled = true;
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
        //In case ui canvas is disable (by previous menu)
        showMenu();

        //Kill all children code from https://answers.unity.com/questions/611850/destroy-all-children-of-object.html
        if (MapMan.previousSelected != null)
            foreach (Transform ui in UICanvas.transform)
                Destroy(ui.gameObject);
    }
}
