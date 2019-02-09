using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ConfirmationBoxController : MonoBehaviour
{
    public GameObject ConfirmationBoxGameObject;
    public TextMeshProUGUI ConfirmationBoxText;
    public Button ConfirmationBoxYesButtonComponent;
    public Button ConfirmationBoxNoButtonComponent;
    public ExecuteSound SoundManager;

    public bool BoxIsActive { get; private set; }

    private Animator muniAnimator;

    // Start is called before the first frame update
    void Awake()
    {
        // Add Click noises to no button. Click sound to yes button must be readded on every call.
        ConfirmationBoxGameObject.SetActive(true);
        BoxIsActive = false;
    }

    private void Start()
    {
        ConfirmationBoxNoButtonComponent.onClick.AddListener(() => SoundManager.PlaySound("MouseClick"));

        // Add Pointer Enter noises
        SoundManager.AttachOnHoverSoundToObject("MouseHover", ConfirmationBoxNoButtonComponent.gameObject);
        SoundManager.AttachOnHoverSoundToObject("MouseHover", ConfirmationBoxYesButtonComponent.gameObject);
        muniAnimator = ConfirmationBoxGameObject.GetComponent<Animator>();
        ConfirmationBoxNoButtonComponent.onClick.AddListener(() => muniAnimator.SetBool("Open", false));
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
        ConfirmationBoxYesButtonComponent.onClick.AddListener(() => SoundManager.PlaySound("MouseClick"));
        ConfirmationBoxText.text = pstrConfirmationDialog;
        ConfirmationBoxYesButtonComponent.GetComponentInChildren<Text>().text = pstrConfirmButtonDialog;
        ConfirmationBoxNoButtonComponent.GetComponentInChildren<Text>().text = pstrCancelButtonDialog;
        muniAnimator.SetBool("Open", true);
        BoxIsActive = true;
    }

    public void HideConfirmationBox()
    {
        muniAnimator.SetBool("Open", false);
        BoxIsActive = false;
    }
}
