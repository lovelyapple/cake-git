using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : WindowBase
{
    // Use this for initialization

    //private int Mode = 0;
    /*
	void Update(){
		
	}*/
    [SerializeField] GameObject HowGame;
    [SerializeField] GameObject HowPlay;

    // ゲーム説明
    public void OnClickHowGame()
    {
        HowGame.SetActive(true);
        HowPlay.SetActive(false);
    }

    // 操作説明
    public void OnClickHowPlay()
    {
        HowGame.SetActive(false);
        HowPlay.SetActive(true);
    }

    // ゲームに戻る
    public void OnClickBack()
    {
        StateConfig.IsPausing = false;
        Close();
    }

    public void OnClickReset()
    {
        FieldManager.Get().ReSetMap(() => { Close(); }, null);
    }
}
