using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class LoadScene : SingleToneBase<LoadScene>
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
    LoadPhase loadPhase = LoadPhase.StandBy;
    [SerializeField] Slider loadingSlider;
    [SerializeField] GameObject loadingIconObj;
    [SerializeField] Animator fadeSysAnimator;
    [SerializeField] Object standByAnim;
    [SerializeField] Object endAnim;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeSysAnimator == null) { return; }

        var animInfo = fadeSysAnimator.GetCurrentAnimatorStateInfo(0);

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
                    loadPhase = LoadPhase.FadeInFin;
                }
                break;
            case LoadPhase.FadeOutFin:
                break;
        }

        UpdateLoadingIcon();
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
    }
    public bool IsFadeOutFin()
    {
        return loadPhase == LoadPhase.FadeOutFin;
    }
    public void SetSLiderValue(uint v, uint max)
    {
        if (loadingSlider == null) { return; }
        loadingSlider.value = v;
        loadingSlider.maxValue = max;
    }
    void UpdateLoadingIcon()
    {
        UIUtility.SetActive(loadingIconObj, loadPhase != LoadPhase.Loading);
    }
}
