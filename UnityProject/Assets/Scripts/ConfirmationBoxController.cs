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

    // Start is called before the first frame update
    void Awake()
    {
        ConfirmationBoxGameObject.SetActive(false);
    }

    public void AttachCallbackToConfirmationBox(UnityAction puniCallbackFunction, string pstrConfirmationDialog, string pstrConfirmButtonDialog = "Yes")
    {
        ConfirmationBoxYesButtonComponent.onClick.RemoveAllListeners();
        ConfirmationBoxYesButtonComponent.onClick.AddListener(puniCallbackFunction);
        ConfirmationBoxYesButtonComponent.onClick.AddListener(HideConfirmationBox);
        ConfirmationBoxText.text = pstrConfirmationDialog;
        ConfirmationBoxYesButtonComponent.GetComponentInChildren<Text>().text = pstrConfirmButtonDialog;
        ConfirmationBoxGameObject.SetActive(true);
    }

    public void HideConfirmationBox()
    {
        ConfirmationBoxGameObject.SetActive(false);
    }
}
