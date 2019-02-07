using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class ConfirmationBoxController : MonoBehaviour
{
    public GameObject ConfirmationBoxGameObject;
    public TextMeshProUGUI ConfirmationBoxText;
    public Button ConfirmationBoxYesButtonComponent;
    public Button ConfirmationBoxNoButtonComponent;

    // Start is called before the first frame update
    void Awake()
    {
        ConfirmationBoxGameObject.SetActive(false);
    }

    /// <summary>
    /// Method for creating confirmation boxes with callback functions attached.
    /// </summary>
    /// <param name="puniCallbackFunction"></param>
    /// <param name="pstrConfirmationDialog"></param>
    /// <param name="pstrConfirmButtonDialog"></param>
    /// <param name="pstrCancelButtonDialog"></param>
    public void AttachCallbackToConfirmationBox(
        UnityAction puniCallbackFunction,
        string pstrConfirmationDialog,
        string pstrConfirmButtonDialog = "Yes",
        string pstrCancelButtonDialog = "No")
    {
        ConfirmationBoxYesButtonComponent.onClick.RemoveAllListeners();
        ConfirmationBoxYesButtonComponent.onClick.AddListener(puniCallbackFunction);
        ConfirmationBoxYesButtonComponent.onClick.AddListener(HideConfirmationBox);
        ConfirmationBoxText.text = pstrConfirmationDialog;
        ConfirmationBoxYesButtonComponent.GetComponentInChildren<Text>().text = pstrConfirmButtonDialog;
        ConfirmationBoxNoButtonComponent.GetComponentInChildren<Text>().text = pstrCancelButtonDialog;
        ConfirmationBoxGameObject.SetActive(true);
    }

    public void HideConfirmationBox()
    {
        ConfirmationBoxGameObject.SetActive(false);
    }
}
