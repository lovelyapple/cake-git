using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingleToneBase<SoundManager>
{
    //resourcemanager に入れたいけど、まあいいや
    [SerializeField] GameObject SoundRoot;
    [SerializeField] AudioSource bgm;
    [SerializeField] AudioClip jump;
    [SerializeField] AudioClip result;
    [SerializeField] AudioClip throwOut;
    [SerializeField] AudioClip releaseFriend;
    [SerializeField] AudioClip catchFriend;
    [SerializeField] AudioClip damage;
    [SerializeField] SoundController seCtrlPrefab;
    public List<SoundController> soundCtrlList = new List<SoundController>();
    public void StartBgm(bool active)
    {
        if (bgm != null)
            bgm.mute = active;
    }
    public void PlayOneShotSe_Damage()
    {
        PlayOneShotSe(damage);
    }
    public void PlayOneShotSe_Jump()
    {
        PlayOneShotSe(jump);
    }
    public void PlayOneShotSe_ThrowOut()
    {
        PlayOneShotSe(throwOut);
    }
    public void PlayOneShotSe_Catch()
    {
        PlayOneShotSe(catchFriend);
    }
    public void PlayOneShotSe_Release()
    {
        PlayOneShotSe(releaseFriend);
    }
    public void PlayOneShotSe_Result()
    {
        PlayOneShotSe(result);
    }
    void PlayOneShotSe(AudioClip clip)
    {
        if (clip == null) { return; }

        foreach (var ctrl in soundCtrlList)
        {
            if (!ctrl.isPlaying)
            {
                ctrl.PlayOneShotSe(clip);
                return;
            }
        }

        try
        {
            var go = GameObject.Instantiate(seCtrlPrefab.gameObject, SoundRoot.transform);
            var newCtrl = go.GetComponent<SoundController>();
            newCtrl.PlayOneShotSe(clip);
            soundCtrlList.Add(newCtrl);
        }
        catch
        {
            Debug.Log("could not create sound!");
        }
    }
}
