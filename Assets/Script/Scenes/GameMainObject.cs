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
public class StateConfig
{
    static bool isPausing = false;
    public static bool IsPausing
    {
        get { return isPausing; }
        set
        {
            if (isPausing == value)
            {
                Debug.LogWarning("game pause is already in " + value);
            }
            else
            {
                isPausing = value;
                Time.timeScale = isPausing ? 0 : 1;
                if (OnPauseChanged != null)
                {
                    OnPauseChanged(IsPausing);
                }
            }
        }
    }

    public static event Action<bool> OnPauseChanged;
}
public class GameMainObject : SingleToneBase<GameMainObject>
{
    [SerializeField] GameMode _gameMode;
    public GameMode gameMode { get { return _gameMode; } }
    public bool IsDebugMode { get { return _gameMode == GameMode.Debug; } }
    public Action OnFadeInOver;
    GameState gameState = GameState.Title;
    void Start()
    {
        WindowManager.Get().ChecktInitWindowList();
        WindowManager.CreateOpenWindow(WindowIndex.TitleWindow, (w) =>
         {
             gameState = GameState.Title;
         });
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            WindowManager.CreateOpenWindow(WindowIndex.PauseWindow, (w) =>
             {
                 StateConfig.IsPausing = true;
             });
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            FieldManager.Get().ReSetMap(null, null);
        }
    }
    public bool IsGamePlaying { get { return gameState == GameState.Game; } }
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

        gameState = targetState;

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
        WindowManager.CreateOpenWindow(WindowIndex.LoadWindow, (w) =>
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
