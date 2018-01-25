using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameMainObject : SingleToneBase<GameMainObject>
{
    Coroutine _LoadSceneCoroutine = null;
    public enum SceneName
    {
        UITest,
        Load,
        Title,
        Game,
        Result,
    }
    static Dictionary<SceneName, string> SceneNameDict = new Dictionary<SceneName, string>()
    {
        {SceneName.Title,"_TitleScene"},
        {SceneName.Load, "_LoadScene"},
        {SceneName.Game, "_GameScene"},
        {SceneName.Result, "_ResultScene"},
    };
    public void LoadTitleScene()
    {
        if (_LoadSceneCoroutine != null)
        {
            Debug.LogError("could not change Scene load process is running");
            return;
        }

        _LoadSceneCoroutine = StartCoroutine(ChangeScene(SceneName.Title, SceneName.Title));
    }
    public void LoadGameScene()
    {
        if (_LoadSceneCoroutine != null)
        {
            Debug.LogError("could not change Scene load process is running");
            return;
        }
        _LoadSceneCoroutine = StartCoroutine(ChangeScene(SceneName.Title, SceneName.Title));
    }
    public void LoadResultScene()
    {
        if (_LoadSceneCoroutine != null)
        {
            Debug.LogError("could not change Scene load process is running");
            return;
        }
        _LoadSceneCoroutine = StartCoroutine(ChangeScene(SceneName.Title, SceneName.Title));
    }
    public void ReturnToTitleScene()
    {
        if (_LoadSceneCoroutine != null)
        {
            Debug.LogError("could not change Scene load process is running");
            return;
        }
        _LoadSceneCoroutine = StartCoroutine(ChangeScene(SceneName.Title, SceneName.Title));
    }
    IEnumerator ChangeScene(SceneName loadTarget, SceneName unloadTarget)
    {
        Debug.LogWarning("request change scene " + loadTarget.ToString() + " to " + unloadTarget.ToString());
        //todo シーンの遷移処理
        yield return null;
        _LoadSceneCoroutine = null;
    }
}
