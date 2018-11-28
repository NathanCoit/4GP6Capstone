using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public GameObject MoveBuildingButton;
    public GameObject UpgradeBuildingButton;
    public GameObject BuyMinersButton;
    public GameObject UpgradeUIButton;
    public GameObject EnemyBattleButton;

    private Image SelectedBuildingImage;
    private TextMeshProUGUI SelectedBuildingTypeText;
    private TextMeshProUGUI SelectedBuildingUpgradeLevelText;
    private TextMeshProUGUI SelectedBuildingOwnerText;
    private TextMeshProUGUI SelectedBuildingMinersText;

    public GameObject DefaultMenuPanel;
    public GameObject BuildMenuPanel;
    public GameObject SelectedBuildingMenu;

    public GameObject MineButton;
    public GameObject UpgradeButton;

    private void Start()
    {
        SelectedBuildingImage = SelectedBuildingSpriteObject.GetComponent<Image>();
        SelectedBuildingTypeText = SelectedBuildingTypeTextObject.GetComponent<TextMeshProUGUI>();
        SelectedBuildingUpgradeLevelText = SelectedBuildingUpgradeLevelTextObject.GetComponent<TextMeshProUGUI>();
        SelectedBuildingOwnerText = SelectedBuildingOwnerTextObject.GetComponent<TextMeshProUGUI>();
        SelectedBuildingMinersText = SelectedBuildingMinersTextObject.GetComponent<TextMeshProUGUI>();

        GoToDefaultMenu();
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
        UpgradeButton.GetComponent<Button>().enabled = false;
        UpgradeButton.GetComponent<Image>().color = Color.gray;
    }

    private void EnableUpgradeButton()
    {
        UpgradeButton.GetComponent<Button>().enabled = true;
        UpgradeButton.GetComponent<Image>().color = new Color(0.4902f, 0, 0, 255);
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
