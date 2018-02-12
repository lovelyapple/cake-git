using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//todo resourceManagerでキャッシュ化
/* /*
// 				/l__/l           食料くれにゃー
			   /  _  _l           
		      /     w /__
		     /    ____C __C 
	_______/       /
	~~~~~~       /
	     |  / | / 
		 | /  |/
	 
	*/
public class FxManager : SingleToneBase<FxManager>
{
    [SerializeField] GameObject fxRoot;
    [SerializeField] FxController fxControlerPrefab;
    [SerializeField] ParticleSystem damage;
    List<FxController> fxCtrlList = new List<FxController>();
    public void CreateFx_Damage(Vector3 pos)
    {
        CreateFx(pos, FxType.Damage, damage);
    }
    void CreateFx(Vector3 pos, FxType type, ParticleSystem pS)
    {
        foreach (var p in fxCtrlList)
        {
            if (!p.IsPlaying && p.fxRype == type)
            {
                p.Play(pos);
                return;
            }
        }

        var goC = GameObject.Instantiate(fxControlerPrefab, fxRoot.transform);
        goC.transform.position = pos;
        var fxC = goC.GetComponent<FxController>();
        var goP = GameObject.Instantiate(damage, goC.transform);
        goP.transform.position = pos;
        var pc = goP.GetComponent<ParticleSystem>();

        goC.Setup(type, pc);
        fxCtrlList.Add(fxC);
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            CreateFx_Damage(Vector3.zero);
        }
    }
}
