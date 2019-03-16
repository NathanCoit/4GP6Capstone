using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorshipperUpgradeController : MonoBehaviour
{
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
        // Attach tooltips to buttons
        //TooltipScript.AttachTooltipToObject(AttackButton, "Upgrade the attack of your worshippers by 1!");
        //TooltipScript.AttachTooltipToObject(DefenseButton, "Upgrade the defense of your worshippers by 1!");
        //TooltipScript.AttachTooltipToObject(MovementButton, "Upgrade the movement of your worshippers by 1!");

        // Get current count of upgrades for price calculations
        mintCurrentAttackCount = PlayerFaction.CurrentUpgrades.FindAll(musUpgrade => musUpgrade.UpgradeType == WorshipperUpgrade.UPGRADETYPE.Attack).Count;
        mintCurrentDefenseCount = PlayerFaction.CurrentUpgrades.FindAll(musUpgrade => musUpgrade.UpgradeType == WorshipperUpgrade.UPGRADETYPE.Defense).Count;
        mintCurrentMovementCount = PlayerFaction.CurrentUpgrades.FindAll(musUpgrade => musUpgrade.UpgradeType == WorshipperUpgrade.UPGRADETYPE.Movement).Count;

        mintAttackUpgradeCost = (int)(100 * Mathf.Pow(10,mintCurrentAttackCount));
        mintDefenseUpgradeCost = (int)(100 * Mathf.Pow(10, mintCurrentDefenseCount));
        mintMovementUpgradeCost = (int)(1000 * Mathf.Pow(10, mintCurrentMovementCount));

        AttackText.text = string.Format("Attack +1\n{0} Mat", mintAttackUpgradeCost);
        DefenseText.text = string.Format("Defense +1\n{0} Mat", mintDefenseUpgradeCost);
        MovementText.text = string.Format("Movement +1\n{0} Mat", mintMovementUpgradeCost);

        TotalAttackText.text = string.Format("Attack Buff\n +{0}", mintCurrentAttackCount);
        TotalDefenseText.text = string.Format("Defense Buff\n +{0}", mintCurrentDefenseCount);
        TotalMovementText.text = string.Format("Movement Buff\n +{0}", mintCurrentMovementCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpgradeWorshipperAttack()
    {
        AttackWorshipperUpgrade musAttackUpgrade;
        if(PlayerFaction.MaterialCount >= mintAttackUpgradeCost)
        {
            PlayerFaction.MaterialCount -= mintAttackUpgradeCost;
            musAttackUpgrade = new AttackWorshipperUpgrade("Attack Upgrade", "Increase attack by 1", mintAttackUpgradeCost, 1);
            PlayerFaction.CurrentUpgrades.Add(musAttackUpgrade);
            mintCurrentAttackCount++;
            mintAttackUpgradeCost = (int)(100 * Mathf.Pow(10, mintCurrentAttackCount));
            AttackText.text = string.Format("Attack +1\n{0} Mat", mintAttackUpgradeCost);
            TotalAttackText.text = string.Format("Attack Buff\n +{0}", mintCurrentAttackCount);
            AudioManager.PlaySound("ChaChing");
        }
        else
        {
            AudioManager.PlaySound("NotMaterials");
        }
    }

    public void UpgradeWorshipperDefense()
    {
        DefenseWorshipperUpgrade musDefenseUpgrade;
        if (PlayerFaction.MaterialCount >= mintDefenseUpgradeCost)
        {
            PlayerFaction.MaterialCount -= mintDefenseUpgradeCost;
            musDefenseUpgrade = new DefenseWorshipperUpgrade("Defense Upgrade", "Increase Defense by 1", mintDefenseUpgradeCost, 1);
            PlayerFaction.CurrentUpgrades.Add(musDefenseUpgrade);
            mintCurrentDefenseCount++;
            mintDefenseUpgradeCost = (int)(100 * Mathf.Pow(10, mintCurrentDefenseCount));
            DefenseText.text = string.Format("Defense +1\n{0} Mat", mintDefenseUpgradeCost);
            TotalDefenseText.text = string.Format("Defense Buff\n +{0}", mintCurrentDefenseCount);
            AudioManager.PlaySound("ChaChing");
        }
        else
        {
            AudioManager.PlaySound("NotMaterials");
        }
    }

    public void UpgradeWorshipperMovement()
    {
        MovementWorshipperUpgrade musMovementUpgrade;
        if (PlayerFaction.MaterialCount >= mintMovementUpgradeCost)
        {
            PlayerFaction.MaterialCount -= mintMovementUpgradeCost;
            musMovementUpgrade = new MovementWorshipperUpgrade("Movement Upgrade", "Increase Movement by 1", mintMovementUpgradeCost, 1);
            PlayerFaction.CurrentUpgrades.Add(musMovementUpgrade);
            mintCurrentMovementCount++;
            mintMovementUpgradeCost = (int)(1000 * Mathf.Pow(10, mintCurrentMovementCount));
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
