using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;
    public TextMeshProUGUI LoadingScreenText;
    public GameObject LoadingScreenSpinner;
    public Image LoadingScreenImage;
    public Animator LoadingScreenAnimator;
    public Sprite[] LoadingImages;
    public GameObject LoadingScreenBackground;

    private bool mblnActive = false;

    public bool ScreenActive { get { return mblnActive; } }

    private List<string> marrLoadingScreenLines = new List<string>
    {
        "Thing",
        "Stuff"
    };


    private void Awake()
    {
        // Singleton pattern to ensure only 1 loading screen exists at a time
        // Also makes loading screen manager reference easier
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // Don't destroy canvas loading screen is attached too.
            DontDestroyOnLoad(gameObject.transform.parent);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadingScreenBackground.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(mblnActive)
        {
            LoadingScreenSpinner.transform.localEulerAngles -= new Vector3(0, 0, 1);
        }
    }

    public void ResetLoadingScreen()
    {
        mblnActive = false;
        LoadingScreenSpinner.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public void FadeIn()
    {
        mblnActive = true;
        LoadingScreenAnimator.SetTrigger("FadeIn");
        StartCoroutine(ReplaceTextAndImageAfterDelay(5));
    }

    public void FadeOut()
    {
        mblnActive = false;
        LoadingScreenAnimator.SetTrigger("FadeOut");
    }

    public IEnumerator ReplaceTextAndImageAfterDelay(float pfDelay)
    {
        yield return new WaitForSeconds(pfDelay);
        if(mblnActive)
        {
            LoadingScreenAnimator.SetTrigger("SwapUI");
            StartCoroutine(ReplaceTextAndImageAfterDelay(pfDelay));
        }
    }

    private string GetrandomText()
    {
        int intRand = UnityEngine.Random.Range(0, marrLoadingScreenLines.Count);
        return marrLoadingScreenLines[intRand];
    }

    private Sprite GetRandomSprite()
    {
        int intRand = UnityEngine.Random.Range(0, LoadingImages.Length);
        return LoadingImages[intRand];
    }

    public void ReplaceLoadingUI()
    {
        LoadingScreenImage.sprite = GetRandomSprite();
        LoadingScreenText.text = GetrandomText();
    }
}
