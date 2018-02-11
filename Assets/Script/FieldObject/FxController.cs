using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FxType
{
    Damage,
}
public class FxController : MonoBehaviour
{
    [SerializeField] ParticleSystem pSys;
    public bool IsPlaying { get; private set; }
    public FxType fxRype;
    public void Setup(FxType type, ParticleSystem fxInstance)
    {
        fxRype = type;
        pSys = fxInstance;
        StartCoroutine(IePlay());
    }
    public void Play(Vector3 pos)
    {
        transform.position = pos;
        StartCoroutine(IePlay());
    }
    IEnumerator IePlay()
    {
        if (pSys == null) { yield break; }

        IsPlaying = true;
        UIUtility.SetActive(pSys.gameObject, true);
        yield return new WaitWhile(() => pSys.IsAlive(true));
        UIUtility.SetActive(pSys.gameObject, false);
        IsPlaying = false;
    }

}
