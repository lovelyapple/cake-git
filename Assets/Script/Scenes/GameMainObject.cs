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
    public Action OnFadeInOver;
    bool isPausing;

    GameState gameState = GameState.Title;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        ResourcesManager.Get().ChecktInitWindowList();
        ResourcesManager.Get().CreateOpenWindow(WindowIndex.TitleWindow, (w) =>
         {
             gameState = GameState.Title;
         });
    }
    public void PauseGame()
    {
        if (isPausing) return;
        //todo pause
    }
    public void UnPauseGame()
    {
        if (!isPausing) return;
        //todo unPause
    }
    public bool IsPaussing { get { return isPausing; } }
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
    public void RequestChangeStateWithoutFade(GameState targetState, Action OnChanged)
    {
        if (gameState == targetState)
        {
            Debug.LogWarning("already in the state " + targetState.ToString());
            return;
        }

        if (OnChanged != null)
        {
            OnChanged();
        }

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

        if (OnFadeInOver != null)
        {
            OnFadeInOver();
        }

        loadWnd.RunLoading();

        while (!loadWnd.IsFadeOutFin())
        {
            yield return null;
        }

        loadWnd.Close();
        gameState = targetState;
    }
}
