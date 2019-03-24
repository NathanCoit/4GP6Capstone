using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MenuPanelControls : MonoBehaviour {

    public Sprite MushroomGodMenuBackground;
    public Sprite PaintGodMenuBackground;
    public Sprite ForkGodMenuBackground;
    public Sprite ShoeGodMenuBackground;
    public Sprite DuckGodMenuBackground;

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
    private HotKeyManager hotKeyManager = new HotKeyManager();
    

    private void Start()
    {
        SelectedBuildingImage = SelectedBuildingSpriteObject.GetComponent<Image>();
        SelectedBuildingTypeText = SelectedBuildingTypeTextObject.GetComponent<TextMeshProUGUI>();
        SelectedBuildingUpgradeLevelText = SelectedBuildingUpgradeLevelTextObject.GetComponent<TextMeshProUGUI>();
        SelectedBuildingOwnerText = SelectedBuildingOwnerTextObject.GetComponent<TextMeshProUGUI>();
        SelectedBuildingMinersText = SelectedBuildingMinersTextObject.GetComponent<TextMeshProUGUI>();
        hotKeyManager.LoadHotkeyProfile();
        SetButtonText();
        GoToDefaultMenu();
    }

    public void SetButtonText()
    {
        hotKeyManager.LoadHotkeyProfile();
        // Set hotkey text to max 3 characters
        MoveBuildingButton.GetComponentInChildren<Text>().text = hotKeyManager.HotKeys["BuildingMoveKeyCode"].
            ToString().Substring(0, Math.Min(3, hotKeyManager.HotKeys["BuildingMoveKeyCode"].ToString().Length));

        UpgradeBuildingButton.GetComponentInChildren<Text>().text = hotKeyManager.HotKeys["BuildingUpgradeKeyCode"].
            ToString().Substring(0, Math.Min(3, hotKeyManager.HotKeys["BuildingUpgradeKeyCode"].ToString().Length));

        BuyMinersButton.GetComponentInChildren<Text>().text = hotKeyManager.HotKeys["BuyMinersKeyCode"].
            ToString().Substring(0, Math.Min(3, hotKeyManager.HotKeys["BuyMinersKeyCode"].ToString().Length));

        UpgradeUIButton.GetComponentInChildren<Text>().text = hotKeyManager.HotKeys["BlackSmithUIKeyCode"].
            ToString().Substring(0, Math.Min(3, hotKeyManager.HotKeys["BlackSmithUIKeyCode"].ToString().Length));

        EnemyBattleButton.GetComponentInChildren<Text>().text = hotKeyManager.HotKeys["StartBattleKeyCode"].
            ToString().Substring(0, Math.Min(3, hotKeyManager.HotKeys["StartBattleKeyCode"].ToString().Length));

        AltarButton.GetComponentInChildren<Text>().text = hotKeyManager.HotKeys["AltarKeyCode"].
            ToString().Substring(0, Math.Min(3, hotKeyManager.HotKeys["AltarKeyCode"].ToString().Length));

        HousingButton.GetComponentInChildren<Text>().text = hotKeyManager.HotKeys["HouseKeyCode"].
            ToString().Substring(0, Math.Min(3, hotKeyManager.HotKeys["HouseKeyCode"].ToString().Length));

        MineButton.GetComponentInChildren<Text>().text = hotKeyManager.HotKeys["MineKeyCode"].
            ToString().Substring(0, Math.Min(3, hotKeyManager.HotKeys["MineKeyCode"].ToString().Length));

        BlacksmithButton.GetComponentInChildren<Text>().text = hotKeyManager.HotKeys["BlacksmithKeyCode"].
            ToString().Substring(0, Math.Min(3, hotKeyManager.HotKeys["BlacksmithKeyCode"].ToString().Length));

        PauseButton.GetComponentInChildren<Text>().text = hotKeyManager.HotKeys["EscapeKeyCode"].
            ToString().Substring(0, Math.Min(3, hotKeyManager.HotKeys["EscapeKeyCode"].ToString().Length));

        TierRewardButton.GetComponentInChildren<Text>().text = hotKeyManager.HotKeys["TierRewardKeyCode"].
            ToString().Substring(0, Math.Min(3, hotKeyManager.HotKeys["TierRewardKeyCode"].ToString().Length));
    }

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
        if(pblnAllowedToBuildUpgrade)
        {
            EnableUpgradeButton();
        }
        else
        {
            DisableUpgradeButton();
        }
    }

    public void GoToDefaultMenu()
    {
        DisableAllMenus();
        DefaultMenuPanel.SetActive(true);
        SelectedBuildingPanel.SetActive(false);
    }

    public void EnterSelectedBuildingMenu(Building SelectedBuilding, bool pblnPlayerOwnedBuilding, bool pblnCanBeUpgraded)
    {
        string strBuildingType = string.Empty;
        DisableAllMenus();
        SelectedBuildingMenu.SetActive(true);
        SelectedBuildingPanel.SetActive(true);
        SelectedBuildingOwnerText.text = string.Format("Owner: {0} God of {1}", SelectedBuilding.OwningFaction.GodName, SelectedBuilding.OwningFaction.Type.ToString());
        switch(SelectedBuilding.BuildingType)
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
        SelectedBuildingUpgradeLevelText.text = string.Format("Level: {0}", SelectedBuilding.UpgradeLevel);
        HideSelectedBuildingButtons();
        if(pblnPlayerOwnedBuilding)
        {
            // enable buttons based on building type
            if(SelectedBuilding.BuildingType == Building.BUILDING_TYPE.MATERIAL)
            {
                SelectedBuildingMinersTextObject.SetActive(true);
                SelectedBuildingMinersText.text = string.Format("Miners: {0}/{1}", ((MineBuilding)SelectedBuilding).Miners, ((MineBuilding)SelectedBuilding).MinerCap);
                // Show upgrade, move, and buy miners buttons
                UpgradeBuildingButton.SetActive(true);
                MoveBuildingButton.SetActive(true);
                BuyMinersButton.SetActive(true);
                BuyMinersTextAmount.GetComponent<Text>().text = Mathf.Pow(10, SelectedBuilding.UpgradeLevel).ToString();
            }
            else if(SelectedBuilding.BuildingType == Building.BUILDING_TYPE.UPGRADE)
            {
                // Enable upgrade UI button, move, and buildingupgrade button
                MoveBuildingButton.SetActive(true);
                UpgradeBuildingButton.SetActive(true);
                UpgradeUIButton.SetActive(true);
                SelectedBuildingMinersTextObject.SetActive(false);
            }
            else if (SelectedBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE)
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
            if(SelectedBuilding.BuildingType == Building.BUILDING_TYPE.VILLAGE)
            {
                // Enable battle button
                EnemyBattleButton.SetActive(true);
            }
        }
    }

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

    private void EnableButton(Button pbtnToEnable, Image pimgToEnable)
    {
        pbtnToEnable.enabled = true;
        pimgToEnable.color = new Color(0.4902f, 0, 0, 255);
    }

    private void DisableButton(Button pbtnToDisable, Image pimgToDisable)
    {
        pbtnToDisable.enabled = false;
        pimgToDisable.color = Color.grey;
    }
}
