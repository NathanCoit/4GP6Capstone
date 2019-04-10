using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Script to control the UI elements within the Black smith UI.
/// Controls unlocking of worshipper upgrades and displays information to the player
/// </summary>
public class WorshipperUpgradeController : MonoBehaviour
{
    private const int mcintAttackBaseCost = 100;
    private const int mcintDefenseBaseCost = 100;
    private const int mcintMovementBaseCost = 100;

    public GameObject AttackButton;
    public GameObject DefenseButton;
    public GameObject MovementButton;
    public TooltipDisplayController TooltipScript;
    public GameManager GameManagerScript;
    public Faction PlayerFaction;
    public TextMeshProUGUI AttackText;
    public TextMeshProUGUI DefenseText;
    public TextMeshProUGUI MovementText;
    public TextMeshProUGUI TotalAttackText;
    public TextMeshProUGUI TotalDefenseText;
    public TextMeshProUGUI TotalMovementText;
    public ExecuteSound AudioManager;

    private int mintCurrentAttackCount = 0;
    private int mintCurrentDefenseCount = 0;
    private int mintCurrentMovementCount = 0;
    private int mintAttackUpgradeCost = 100;
    private int mintDefenseUpgradeCost = 100;
    private int mintMovementUpgradeCost = 1000;

    // Start is called before the first frame update
    void Start()
    {
        // Get current count of upgrades for price calculations
        mintCurrentAttackCount = PlayerFaction.CurrentUpgrades.FindAll(musUpgrade => musUpgrade.UpgradeType == WorshipperUpgrade.UPGRADETYPE.Attack).Count;
        mintCurrentDefenseCount = PlayerFaction.CurrentUpgrades.FindAll(musUpgrade => musUpgrade.UpgradeType == WorshipperUpgrade.UPGRADETYPE.Defense).Count;
        mintCurrentMovementCount = PlayerFaction.CurrentUpgrades.FindAll(musUpgrade => musUpgrade.UpgradeType == WorshipperUpgrade.UPGRADETYPE.Movement).Count;

        mintAttackUpgradeCost = (int)(mcintAttackBaseCost * Mathf.Pow(10, mintCurrentAttackCount));
        mintDefenseUpgradeCost = (int)(mcintDefenseBaseCost * Mathf.Pow(10, mintCurrentDefenseCount));
        mintMovementUpgradeCost = (int)(mcintMovementBaseCost * Mathf.Pow(10, mintCurrentMovementCount));

        AttackText.text = string.Format("Attack +1\n{0} Mat", mintAttackUpgradeCost);
        DefenseText.text = string.Format("Defense +1\n{0} Mat", mintDefenseUpgradeCost);
        MovementText.text = string.Format("Movement +1\n{0} Mat", mintMovementUpgradeCost);

        TotalAttackText.text = string.Format("Attack Buff\n +{0}", mintCurrentAttackCount);
        TotalDefenseText.text = string.Format("Defense Buff\n +{0}", mintCurrentDefenseCount);
        TotalMovementText.text = string.Format("Movement Buff\n +{0}", mintCurrentMovementCount);
    }

    /// <summary>
    /// Method attached to attack upgrade button
    /// </summary>
    public void UpgradeWorshipperAttack()
    {
        AttackWorshipperUpgrade musAttackUpgrade;
        if (PlayerFaction.MaterialCount >= mintAttackUpgradeCost)
        {
            PlayerFaction.MaterialCount -= mintAttackUpgradeCost;
            musAttackUpgrade = new AttackWorshipperUpgrade("Attack Upgrade", "Increase attack by 1", mintAttackUpgradeCost, 1);
            PlayerFaction.CurrentUpgrades.Add(musAttackUpgrade);
            mintCurrentAttackCount++;
            mintAttackUpgradeCost = (int)(mcintAttackBaseCost * Mathf.Pow(10, mintCurrentAttackCount));
            AttackText.text = string.Format("Attack +1\n{0} Mat", mintAttackUpgradeCost);
            TotalAttackText.text = string.Format("Attack Buff\n +{0}", mintCurrentAttackCount);
            AudioManager.PlaySound("ChaChing");
        }
        else
        {
            AudioManager.PlaySound("NotMaterials");
        }
    }

    /// <summary>
    /// Method attached to defense upgrade button
    /// </summary>
    public void UpgradeWorshipperDefense()
    {
        DefenseWorshipperUpgrade musDefenseUpgrade;
        if (PlayerFaction.MaterialCount >= mintDefenseUpgradeCost)
        {
            PlayerFaction.MaterialCount -= mintDefenseUpgradeCost;
            musDefenseUpgrade = new DefenseWorshipperUpgrade("Defense Upgrade", "Increase Defense by 1", mintDefenseUpgradeCost, 1);
            PlayerFaction.CurrentUpgrades.Add(musDefenseUpgrade);
            mintCurrentDefenseCount++;
            mintDefenseUpgradeCost = (int)(mcintDefenseBaseCost * Mathf.Pow(10, mintCurrentDefenseCount));
            DefenseText.text = string.Format("Defense +1\n{0} Mat", mintDefenseUpgradeCost);
            TotalDefenseText.text = string.Format("Defense Buff\n +{0}", mintCurrentDefenseCount);
            AudioManager.PlaySound("ChaChing");
        }
        else
        {
            AudioManager.PlaySound("NotMaterials");
        }
    }

    /// <summary>
    /// Method attached to movement upgrade button
    /// </summary>
    public void UpgradeWorshipperMovement()
    {
        MovementWorshipperUpgrade musMovementUpgrade;
        if (PlayerFaction.MaterialCount >= mintMovementUpgradeCost)
        {
            PlayerFaction.MaterialCount -= mintMovementUpgradeCost;
            musMovementUpgrade = new MovementWorshipperUpgrade("Movement Upgrade", "Increase Movement by 1", mintMovementUpgradeCost, 1);
            PlayerFaction.CurrentUpgrades.Add(musMovementUpgrade);
            mintCurrentMovementCount++;
            mintMovementUpgradeCost = (int)(mcintMovementBaseCost * Mathf.Pow(10, mintCurrentMovementCount));
            MovementText.text = string.Format("Movement +1\n{0} Mat", mintMovementUpgradeCost);
            TotalMovementText.text = string.Format("Movement Buff\n +{0}", mintCurrentMovementCount);
            AudioManager.PlaySound("ChaChing");
        }
        else
        {
            AudioManager.PlaySound("NotMaterials");
        }
    }
}
