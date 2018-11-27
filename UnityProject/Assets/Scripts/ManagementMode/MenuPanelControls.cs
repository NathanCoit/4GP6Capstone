using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanelControls : MonoBehaviour {

    public Sprite MushroomGodMenuBackground;
    public Sprite PaintGodMenuBackground;
    public Sprite ForkGodMenuBackground;
    public Sprite ShoeGodMenuBackground;
    public Sprite DuckGodMenuBackground;

    public GameObject DefaultMenuPanel;
    public GameObject BuildMenuPanel;
    public GameObject SelectedBuildingMenu;

    public GameObject MineButton;
    public GameObject UpgradeButton;

    private void Start()
    {
        GoToDefaultMenu();
    }

    public void EnterBuildMode(bool pblnAllowedToBuildMine, bool pblnAllowedToBuildUpgrade)
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
    }

    public void EnterSelectedBuildingMenu(Building.BUILDING_TYPE penumSelectedBuildingType)
    {
        DisableAllMenus();
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
}
