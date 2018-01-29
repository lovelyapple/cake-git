using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum GameMode
{
    Debug,
    EnterPrise,
}
public enum GameState
{
    Title,
    Game,
    Result,
}
public class GameMainObject : SingleToneBase<GameMainObject>
{
    [SerializeField] GameMode _gameMode;
    public GameMode gameMode { get { return _gameMode; } }
    public bool IsDebugMode { get { return _gameMode == GameMode.Debug; } }

    GameState gameState = GameState.Title;
    public void ChangeStateToGame()
    {
        StartCoroutine(IeChangePhase(GameState.Game));
    }
    public void ChangeStateToResult()
    {
        StartCoroutine(IeChangePhase(GameState.Result));
    }
    public void ChangeStateToTitile()
    {
        StartCoroutine(IeChangePhase(GameState.Title));
    }
    public void RequestChangeState(GameState targetState)
    {
        StartCoroutine(IeChangePhase(targetState));
    }
    IEnumerator IeChangePhase(GameState targetState)
    {
        if (gameState == targetState)
        {
            Debug.LogWarning("already in the state " + targetState.ToString());
            yield break;
        }
        LoadWindow loadWnd = null;
        ResourcesManager.Get().CreateOpenWindow(WindowIndex.LoadWindow, (w) =>
         {
             w.MoveToTop();
             loadWnd = (LoadWindow)w;
         });

        while (loadWnd == null)
        {
            yield return null;
        }

        while (!loadWnd.IsFadeInFin())
        {
            yield return null;
        }

        loadWnd.RunLoading();

        var waitSec = 5f;

        while (waitSec > 0f)
        {
            waitSec -= Time.deltaTime;
            loadWnd.SetSLiderValue((uint)waitSec * 100, 500);
            yield return null;
        }

        loadWnd.RunFadeOut();

        while (!loadWnd.IsFadeOutFin())
        {
            yield return null;
        }

        loadWnd.Close();
        gameState = targetState;
    }
}
