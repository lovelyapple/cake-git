using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class LoadWindow : WindowBase
{
    public enum LoadPhase
    {
        FadeIn,
        FadeInFin,
        Loading,
        FadeOut,
        FadeOutFin,
        StandBy,
    }
    LoadPhase loadPhase = LoadPhase.FadeIn;
    [SerializeField] Slider loadingSlider;
    [SerializeField] GameObject loadSliderBarObj;
    [SerializeField] GameObject loadingIconObj;
    [SerializeField] Animator fadeSysAnimator;
    [SerializeField] Object standByAnim;
    [SerializeField] Object endAnim;
    [SerializeField] Text loadigStatusLabel;
    string animLayer;
    int stateStandBy;
    int stateEnd;
    // Use this for initialization
    void Start()
    {
        if (fadeSysAnimator == null)
        {
            Debug.LogError("there is no animator!");
            return;
        }

        animLayer = fadeSysAnimator.GetLayerName(0);
        stateStandBy = Animator.StringToHash(animLayer + "." + standByAnim.name);
        stateEnd = Animator.StringToHash(animLayer + "." + endAnim.name);

        if (GameMainObject.Get().IsDebugMode)
        {
            Debug.LogError(" stateStandBy " + stateStandBy);
            Debug.LogError(" stateEnd " + stateEnd);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeSysAnimator == null) { return; }

        var animInfo = fadeSysAnimator.GetCurrentAnimatorStateInfo(0);

        if (GameMainObject.Get().IsDebugMode)
        {
            Debug.LogError("animInfo.fullPathHash " + animInfo.fullPathHash);
        }

        switch (loadPhase)
        {
            case LoadPhase.StandBy:
                break;
            case LoadPhase.FadeIn:
                if (animInfo.fullPathHash == stateStandBy)
                {
                    loadPhase = LoadPhase.FadeInFin;
                }
                break;
            case LoadPhase.FadeInFin:
                break;
            case LoadPhase.Loading:
                break;
            case LoadPhase.FadeOut:
                if (animInfo.fullPathHash == stateEnd)
                {
                    loadPhase = LoadPhase.FadeOutFin;
                }
                break;
            case LoadPhase.FadeOutFin:
                break;
        }

        UpdateLoadingIcon();
        UpdateLoadSlider();
        UpdateLoadingStatusLabel();
    }

    public void RunFadeIn()
    {
        loadPhase = LoadPhase.FadeIn;
    }
    public bool IsFadeInFin()
    {
        return loadPhase == LoadPhase.FadeInFin;
    }
    public void RunLoading()
    {
        loadPhase = LoadPhase.Loading;
    }
    public void RunFadeOut()
    {
        loadPhase = LoadPhase.FadeOut;
        fadeSysAnimator.SetTrigger("FadeOutStart");
    }
    public bool IsFadeOutFin()
    {
        return loadPhase == LoadPhase.FadeOutFin;
    }
    public void SetSLiderValue(uint v, uint max, string description = null)
    {
        if (loadingSlider == null) { return; }
        loadingSlider.value = v;
        loadingSlider.maxValue = max;

        if (description != null)
        {
            loadigStatusLabel.text = description;
        }
    }
    void UpdateLoadingIcon()
    {
        UIUtility.SetActive(loadingIconObj, loadPhase == LoadPhase.Loading);
    }
    void UpdateLoadSlider()
    {
        UIUtility.SetActive(loadSliderBarObj, loadPhase == LoadPhase.Loading);
    }
    void UpdateLoadingStatusLabel()
    {
        UIUtility.SetActive(loadigStatusLabel.gameObject, loadPhase == LoadPhase.Loading);
    }
}
