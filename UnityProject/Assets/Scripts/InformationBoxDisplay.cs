using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        FirstMine
    };

    public static List<string> TutorialInformationText = new List<string>
    {
        "Welcome to UnderGods! Start by opening the build menu with 'B' and building a Mine with the 'S' key.",
        "You've built your first mine! Select the mine by clicking it. Spend worshippers to buy miners with the 'K' key to start mining resources!"
    };

    public GameObject InformationBoxGameObject;
    public TextMeshProUGUI InformationBoxTextObject;

    void Awake()
    {
        // Hide Information box 
        InformationBoxGameObject.SetActive(false);
    }


    /// <summary>
    /// Public method for displaying an information box to the user.
    /// </summary>
    /// <param name="pstrInformation"></param>
    public void DisplayInformationBox(string pstrInformation)
    {
        InformationBoxTextObject.text = pstrInformation;
        InformationBoxGameObject.SetActive(true);
    }

    /// <summary>
    /// Use tutorial information text list to display a tutorial message
    /// Used to keep a counter for current tutorial progress to eliminate
    /// chance of showing the same text box again and tracking progress for next tutorial box
    /// </summary>
    /// <param name="penumTutorialFlag"></param>
    public void DisplayTutorialBox(TutorialFlag penumTutorialFlag)
    {
        if((int)penumTutorialFlag <= TutorialInformationText.Count)
        {
            DisplayInformationBox(TutorialInformationText[(int)penumTutorialFlag]);
        }
    }
}
