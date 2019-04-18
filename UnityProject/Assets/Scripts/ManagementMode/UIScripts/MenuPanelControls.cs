using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Class for controlling the UI elements contained in the information menu panel at the bottom of the screen
/// Controls button elements for menu state changes, and information on currently selected objects.
/// </summary>
public class MenuPanelControls : MonoBehaviour
{
    // Reference all UI elements to control
    public Sprite MineBuildingSprite;
    public Sprite AltarBuildingSprite;
    public Sprite UpgradeBuildingSprite;
    public Sprite VillageBuildingSprite;
    public Sprite HousingBuildingSprite;

    public GameObject SelectedBuildingPanel;
    public GameObject SelectedBuildingSpriteObject;
    public GameObject SelectedBuildingTypeTextObject;
    public GameObject SelectedBuildingUpgradeLevelTextObject;
    public GameObject SelectedBuildingOwnerTextObject;
    public GameObject SelectedBuildingMinersTextObject;
    public GameObject BuyMinersTextAmount;

    public GameObject PauseButton;
    public GameObject MoveBuildingButton;
    public GameObject UpgradeBuildingButton;
    public GameObject BuyMinersButton;
    public GameObject UpgradeUIButton;
    public GameObject EnemyBattleButton;
    public GameObject TierRewardButton;

    public GameObject AltarButton;
    public GameObject HousingButton;
    public GameObject MineButton;
    public GameObject BlacksmithButton;

    private Image SelectedBuildingImage;
    private TextMeshProUGUI SelectedBuildingTypeText;
    private TextMeshProUGUI SelectedBuildingUpgradeLevelText;
    private TextMeshProUGUI SelectedBuildingOwnerText;
    private TextMeshProUGUI SelectedBuildingMinersText;

    public GameObject DefaultMenuPanel;
    public GameObject BuildMenuPanel;
    public GameObject SelectedBuildingMenu;

    private HotKeyManager mmushotKeyManager = new HotKeyManager();


    /// <summary>
    /// Get references from scene to needed components
    /// Allows placement to be done in scene, and ignored in code.
    /// </summary>
    private void Start()
    {
        SelectedBuildingImage = SelectedBuildingSpriteObject.GetComponent<Image>();
        SelectedBuildingTypeText = SelectedBuildingTypeTextObject.GetComponent<TextMeshProUGUI>();
        SelectedBuildingUpgradeLevelText = SelectedBuildingUpgradeLevelTextObject.GetComponent<TextMeshProUGUI>();
        SelectedBuildingOwnerText = SelectedBuildingOwnerTextObject.GetComponent<TextMeshProUGUI>();
        SelectedBuildingMinersText = SelectedBuildingMinersTextObject.GetComponent<TextMeshProUGUI>();
        mmushotKeyManager.LoadHotkeyProfile();
        SetButtonText();
        GoToDefaultMenu();
    }

    /// <summary>
    /// Dynamically set the text displayed on each button based on the current hotkeys of the player.
    /// Allows for information on which keys can be pressed to be viewable by the player
    /// </summary>
    public void SetButtonText()
    {
        Text[] arrButtonText;
        mmushotKeyManager.LoadHotkeyProfile();
        // Set hotkey text to max 3 characters (e.g. escape -> esc)
        arrButtonText = MoveBuildingButton.GetComponentsInChildren<Text>(); 
        arrButtonText[0].text = mmushotKeyManager.GetUserFriendlyKeyCode("BuildingMoveKeyCode");

        arrButtonText = UpgradeBuildingButton.GetComponentsInChildren<Text>();
        arrButtonText[0].text = mmushotKeyManager.GetUserFriendlyKeyCode("BuildingUpgradeKeyCode");

        arrButtonText = BuyMinersButton.GetComponentsInChildren<Text>();
        arrButtonText[0].text = mmushotKeyManager.GetUserFriendlyKeyCode("BuyMinersKeyCode");

        arrButtonText = UpgradeUIButton.GetComponentsInChildren<Text>();
        arrButtonText[0].text = mmushotKeyManager.GetUserFriendlyKeyCode("BlackSmithUIKeyCode");

        arrButtonText = EnemyBattleButton.GetComponentsInChildren<Text>();
        arrButtonText[0].text = mmushotKeyManager.GetUserFriendlyKeyCode("StartBattleKeyCode");

        arrButtonText = AltarButton.GetComponentsInChildren<Text>();
        arrButtonText[0].text = mmushotKeyManager.GetUserFriendlyKeyCode("AltarKeyCode");
        arrButtonText[1].text = Building.CalculateBuildingCost(Building.BUILDING_TYPE.ALTAR).ToString();

        arrButtonText = HousingButton.GetComponentsInChildren<Text>();
        arrButtonText[0].text = mmushotKeyManager.GetUserFriendlyKeyCode("HouseKeyCode");
        arrButtonText[1].text = Building.CalculateBuildingCost(Building.BUILDING_TYPE.HOUSING).ToString();

        arrButtonText = MineButton.GetComponentsInChildren<Text>();
        arrButtonText[0].text = mmushotKeyManager.GetUserFriendlyKeyCode("MineKeyCode");
        arrButtonText[1].text = Building.CalculateBuildingCost(Building.BUILDING_TYPE.MATERIAL).ToString();

        arrButtonText = BlacksmithButton.GetComponentsInChildren<Text>();
        arrButtonText[0].text = mmushotKeyManager.GetUserFriendlyKeyCode("BlacksmithKeyCode");
        arrButtonText[1].text = Building.CalculateBuildingCost(Building.BUILDING_TYPE.UPGRADE).ToString();

        arrButtonText = PauseButton.GetComponentsInChildren<Text>();
        arrButtonText[0].text = mmushotKeyManager.GetUserFriendlyKeyCode("EscapeKeyCode");

        arrButtonText = TierRewardButton.GetComponentsInChildren<Text>();
        arrButtonText[0].text = mmushotKeyManager.GetUserFriendlyKeyCode("TierRewardKeyCode");
    }

    /// <summary>
    /// Enter the submenu for selecting buildings to buffer
    /// Enabled/disable buttons depending on gamestate
    /// </summary>
    /// <param name="pblnAllowedToBuildMine"></param>
    /// <param name="pblnAllowedToBuildUpgrade"></param>
    public void EnterBuildMenu(bool pblnAllowedToBuildMine, bool pblnAllowedToBuildUpgrade)
    {
        DisableAllMenus();
        BuildMenuPanel.SetActive(true);
        if (pblnAllowedToBuildMine)
        {
            EnableMineButton();
        }
        else
        {
            DisableMineButton();
        }
        if (pblnAllowedToBuildUpgrade)
        {
            EnableUpgradeButton();
        }
        else
        {
            DisableUpgradeButton();
        }
    }

    /// <summary>
    /// Return to default menu.
    /// </summary>
    public void GoToDefaultMenu()
    {
        DisableAllMenus();
        DefaultMenuPanel.SetActive(true);
        SelectedBuildingPanel.SetActive(false);
    }

    /// <summary>
    /// Update information panel with information on the currently selected building.
    /// Allows player to see information on the building they have selected in case of confusion.
    /// </summary>
    /// <param name="pmusSelectedBuilding"></param>
    /// <param name="pblnPlayerOwnedBuilding"></param>
    /// <param name="pblnCanBeUpgraded"></param>
    public void EnterSelectedBuildingMenu(Building pmusSelectedBuilding, bool pblnPlayerOwnedBuilding, bool pblnCanBeUpgraded)
    {
        string strBuildingType = string.Empty;
        DisableAllMenus();
        SelectedBuildingMenu.SetActive(true);
        SelectedBuildingPanel.SetActive(true);
        SelectedBuildingOwnerText.text = string.Format("Owner: {0} God of {1}", pmusSelectedBuilding.OwningFaction.GodName, pmusSelectedBuilding.OwningFaction.Type.ToString());
        switch (pmusSelectedBuilding.BuildingType)
        {
            case Building.BUILDING_TYPE.ALTAR:
                SelectedBuildingImage.sprite = AltarBuildingSprite;
                strBuildingType = "Altar";
                break;
            case Building.BUILDING_TYPE.HOUSING:
                SelectedBuildingImage.sprite = HousingBuildingSprite;
                strBuildingType = "House";
                break;
            case Building.BUILDING_TYPE.MATERIAL:
                SelectedBuildingImage.sprite = MineBuildingSprite;
                strBuildingType = "Mine";
                break;
            case Building.BUILDING_TYPE.UPGRADE:
                SelectedBuildingImage.sprite = UpgradeBuildingSprite;
                strBuildingType = "Blacksmith";
                break;
            case Building.BUILDING_TYPE.VILLAGE:
                SelectedBuildingImage.sprite = VillageBuildingSprite;
                strBuildingType = "Village";
                break;
        }
        SelectedBuildingTypeText.text = strBuildingType;
        SelectedBuildingUpgradeLevelText.text = string.Format("Level: {0}", pmusSelectedBuilding.UpgradeLevel);
        HideSelectedBuildingButtons();
        if (pblnPlayerOwnedBuilding)
        {
            // enable buttons based on building type
            if (pmusSelectedBuilding.BuildingType == Building.BUILDING_TYPE.MATERIAL)
            {
                SelectedBuildingMinersTextObject.SetActive(true);
                SelectedBuildingMinersText.text = string.Format("Miners: {0}/{1}", ((MineBuilding)pmusSelectedBuilding).Miners, ((MineBuilding)pmusSelectedBuilding).MinerCap);
                // Show upgrade, move, and buy miners buttons
                UpgradeBuildingButton.SetActive(true);
                MoveBuildingButton.SetActive(true);
                BuyMinersButton.SetActive(true);
                BuyMinersTextAmount.GetComponent<Text>().text = Mathf.Pow(10, pmusSelectedBuilding.UpgradeLevel).ToString();
            }
            else if (pmusSelectedBuilding.BuildingType == Building.BUILDING_TYPE.UPGRADE)
            {
                // Enable upgrade UI button, move, and buildingupgrade button
                MoveBuildingButton.SetActive(true);
                UpgradeBuildingButton.SetActive(true);
                UpgradeUIButton.SetActive(true);
                SelectedBuildingMinersTextObject.SetActive(false);
            }
            else if (pmusSelectedBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE)
            {
                SelectedBuildingMinersTextObject.SetActive(false);
                // show nothing
            }
            else
            {
                SelectedBuildingMinersTextObject.SetActive(false);
                // show upgrade,move buttons
                UpgradeBuildingButton.SetActive(true);
                MoveBuildingButton.SetActive(true);
            }
            if (pblnCanBeUpgraded)
            {
                // Enable buildingupgrade button
                EnableButton(UpgradeBuildingButton.GetComponent<Button>(), UpgradeBuildingButton.GetComponent<Image>());
            }
            else
            {
                // Disable building Upgrade button
                DisableButton(UpgradeBuildingButton.GetComponent<Button>(), UpgradeBuildingButton.GetComponent<Image>());
            }
        }
        else
        {
            SelectedBuildingMinersTextObject.SetActive(false);
            if (pmusSelectedBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE)
            {
                // Enable battle button
                EnemyBattleButton.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Method to display information on the currently selected god.
    /// Allows player to visually know they have a god selected if he is not on screen.
    /// </summary>
    /// <param name="pmusFaction"></param>
    /// <param name="pmusGameInfo"></param>
    public void EnterGodSelectedState(Faction pmusFaction, GameInfo pmusGameInfo)
    {
        DisableAllMenus();
        SelectedBuildingPanel.SetActive(true);
        SelectedBuildingMinersTextObject.SetActive(false);
        SelectedBuildingImage.sprite = AltarBuildingSprite;
        SelectedBuildingTypeText.text = "God";
        SelectedBuildingUpgradeLevelText.text = string.Format("Level: {0}", pmusFaction.GodTier + 1);
        SelectedBuildingOwnerText.text
            = string.Format(
@"{0} god of {1}
Health: {2}, Attack: {3}, Defense: {4}",
            pmusFaction.GodName,
            pmusFaction.Type,
            pmusGameInfo.GodHealthMultiplier,
            pmusGameInfo.GodAttackMultiplier,
            pmusGameInfo.GodDefenseMultiplier);
    }

    // Methods for enabling/disabling sub menu panels or buttons

    private void DisableAllMenus()
    {
        DefaultMenuPanel.SetActive(false);
        BuildMenuPanel.SetActive(false);
        SelectedBuildingMenu.SetActive(false);
    }

    private void DisableMineButton()
    {
        MineButton.GetComponent<Button>().enabled = false;
        MineButton.GetComponent<Image>().color = Color.gray;
    }

    private void EnableMineButton()
    {
        MineButton.GetComponent<Button>().enabled = true;
        MineButton.GetComponent<Image>().color = new Color(0.4902f, 0, 0, 255);
    }

    private void DisableUpgradeButton()
    {
        BlacksmithButton.GetComponent<Button>().enabled = false;
        BlacksmithButton.GetComponent<Image>().color = Color.gray;
    }

    private void EnableUpgradeButton()
    {
        BlacksmithButton.GetComponent<Button>().enabled = true;
        BlacksmithButton.GetComponent<Image>().color = new Color(0.4902f, 0, 0, 255);
    }

    private void HideSelectedBuildingButtons()
    {
        UpgradeBuildingButton.SetActive(false);
        UpgradeUIButton.SetActive(false);
        MoveBuildingButton.SetActive(false);
        BuyMinersButton.SetActive(false);
        EnemyBattleButton.SetActive(false);
    }

    private void EnableButton(Button puniButtonToEnable, Image puniImageToEnable)
    {
        puniButtonToEnable.enabled = true;
        puniImageToEnable.color = new Color(0.4902f, 0, 0, 255);
    }

    private void DisableButton(Button puniButtonToDisable, Image puniImageToDisable)
    {
        puniButtonToDisable.enabled = false;
        puniImageToDisable.color = Color.grey;
    }
}
