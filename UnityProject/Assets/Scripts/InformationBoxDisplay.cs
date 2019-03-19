using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Class for displaying infomation boxes to the player
/// Used to display tutorial information for the player.
/// See Tutorial flags list for list of tutorial flags.
/// </summary>
public class InformationBoxDisplay : MonoBehaviour
{
    public enum TutorialFlag
    {
        NewGame,
        FirstMine,
        FirstMiners,
        FirstAltar,
        FirstAbility
    };
    private HotKeyManager musHotkeyManager;

    public static List<string> TutorialInformationText = new List<string>
    {
        "Welcome to UnderGods! Start by opening the build menu with 'B' and building a Mine with the 'S' key.",
        "You've built your first mine! Select the mine by clicking it. Spend worshippers to buy miners with the 'K' key to start mining resources!",
        "Now that you have some miners, materials will be mined! Use these materials to build an Altar from the build menu.",
        "Altars will generate worshippers over time. Worshippers will help you in combat and can be used as miners at mines. " +
        "Worshippers will also generate God Bucks which can be spend at the God Shop. Open the God Shop with the 'V' key.",
        "Now that you have an ability, it's time to challenge an enemy god. Find the village of an enemy god and click 'C' to challenge."
    };

    public GameObject InformationBoxGameObject;
    public TextMeshProUGUI InformationBoxTextObject;
    public Button InformationOkButton;
    public ExecuteSound SoundManager;
    private Animator muniAnimator;

    void Awake()
    {
        // Hide Information box 
        InformationBoxGameObject.SetActive(true);
    }

    private void Start()
    {
        musHotkeyManager = new HotKeyManager();
        musHotkeyManager.LoadHotkeyProfile();
        muniAnimator = InformationBoxGameObject.GetComponent<Animator>();
        SoundManager.AttachOnHoverSoundToObject("MouseHover", InformationOkButton.gameObject);
    }


    /// <summary>
    /// Public method for displaying an information box to the user.
    /// Optional parameters to add callbacks to ok button.
    /// </summary>
    /// <param name="pstrInformation"></param>
    /// <param name="puniCallback"></param>
    /// <param name="pstrOkButtonText"></param>
    public void DisplayInformationBox(string pstrInformation, UnityAction puniCallback = null, string pstrOkButtonText = "Ok")
    {
        InformationBoxTextObject.text = pstrInformation;
        InformationOkButton.onClick.RemoveAllListeners();
        if(puniCallback != null)
        {
            
            InformationOkButton.onClick.AddListener(puniCallback);
        }
        InformationOkButton.onClick.AddListener(DefaultOkCallback);
        InformationOkButton.onClick.AddListener(() => SoundManager.PlaySound("MouseClick"));
        InformationOkButton.GetComponentInChildren<Text>().text = pstrOkButtonText;
        muniAnimator.SetBool("Open", true);
    }

    /// <summary>
    /// Use tutorial information text list to display a tutorial message
    /// Used to keep a counter for current tutorial progress to eliminate
    /// chance of showing the same text box again and tracking progress for next tutorial box
    /// </summary>
    /// <param name="penumTutorialFlag"></param>
    public void DisplayTutorialBox(TutorialFlag penumTutorialFlag, UnityAction puniCallback = null)
    {
        if((int)penumTutorialFlag <= TutorialInformationText.Count)
        {
            DisplayInformationBox(TutorialInformationText[(int)penumTutorialFlag], puniCallback);
        }
    }

    /// <summary>
    /// Default callback when Ok button is clicked
    /// </summary>
    public void DefaultOkCallback()
    {
        muniAnimator.SetBool("Open", false);
    }

    
}
