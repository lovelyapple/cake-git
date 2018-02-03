using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : SingleToneBase<FieldManager>
{
    CharacterControllerJellyMesh mainChara;
    List<AIFriendSlime> friendList;
    [SerializeField] GameObject characterRoot;
    [SerializeField] GameObject dungeonRoot;
    FieldInfo currentFieldInto;
    FieldInfo currentFieldCache;
    [SerializeField] string loadDungeonName;
    uint maxStats = 5;
    string creatingMap = "フィールドデータロード中";
    string createMainChara = "メインキャラロード中";
    string createFreind = "スライム生成中";
    string createEnemy = "敵生成中";
    public void CreateField(Action OnFinished, Action OnError)
    {
        StartCoroutine(CreateFieldEnumerator(OnFinished, OnError));
    }
    IEnumerator CreateFieldEnumerator(Action OnFinished, Action OnError)
    {
        yield return LoadDungeon();
        yield return LoadMainCharacter();
        yield return LoadFriendCharacter();
        yield return LoadEnemy();
        yield return RunLoadFadeOut();

    }
    public IEnumerator LoadDungeon()
    {
        UpdateLoadWindow(1, maxStats, creatingMap);

        if (string.IsNullOrEmpty(loadDungeonName))
        {
            var go = ResourcesManager.Get().CreateInstance(FieldObjectIndex.TestDungeon, dungeonRoot.transform);
            if (go == null)
            {
                ClearField();
                yield break;
            }
            currentFieldInto = go.GetComponent<FieldInfo>();
            currentFieldCache = null;
        }
        else
        {
            var go = ResourcesManager.LoadSourcePrefab(loadDungeonName, SourceType.Dungeon);
            if (go == null)
            {
                ClearField();
                yield break;
            }
            currentFieldCache = go.GetComponent<FieldInfo>();

            currentFieldInto = ResourcesManager.CreateGetComponent<FieldInfo>(currentFieldCache.gameObject);

            if (currentFieldInto != null)
            {
                ClearField();
                yield break;
            }
            currentFieldInto.transform.parent = dungeonRoot.transform;
        }

        UIUtility.SetActive(currentFieldInto.gameObject, true);
        yield return new WaitForSeconds(2);
    }
    public IEnumerator LoadMainCharacter()
    {
        UpdateLoadWindow(2, maxStats, createMainChara);

        if (currentFieldInto == null)
        {
            ClearField();
            yield break;
        }

        var startP = currentFieldInto.GetStartPoint();

        if (startP == null)
        {
            ClearField();
            yield break;
        }

        var charaObj = ResourcesManager.Get().CreateInstance(FieldObjectIndex.SlimeMainChara, characterRoot.transform);
        if (charaObj != null)
        {
            mainChara = charaObj.GetComponent<CharacterControllerJellyMesh>();
            mainChara.CreateCharacter();

            if (mainChara == null)
            {
                ClearField();
            }
        }
        yield return new WaitForSeconds(2);

    }
    public IEnumerator LoadFriendCharacter()
    {
        UpdateLoadWindow(3, maxStats, createFreind);
        yield return new WaitForSeconds(2);
        yield break; //todo とりあえず、パス
        if (currentFieldInto == null)
        {
            ClearField();
            yield break;
        }

        friendList = new List<AIFriendSlime>();
        foreach (var friend in currentFieldInto.friendSlimeList)
        {
            var fObj = ResourcesManager.Get().CreateInstance(FieldObjectIndex.SlimeFriend, characterRoot.transform);
            if (fObj == null)
            {
                ClearField();
                yield break;
            }

            var fAI = fObj.GetComponent<AIFriendSlime>();
            friendList.Add(fAI);
        }
    }
    public IEnumerator LoadEnemy()
    {
        UpdateLoadWindow(4, maxStats, createEnemy);
        yield return new WaitForSeconds(2);
        UpdateLoadWindow(5, maxStats, createEnemy);
        yield return new WaitForSeconds(1);
        yield break;
    }
    public IEnumerator RunLoadFadeOut()
    {
        if (!ResourcesManager.Get().IsWindowActive(WindowIndex.LoadWindow)) { yield break; }

        var loadWnd = ResourcesManager.Get().GetWindow(WindowIndex.LoadWindow) as LoadWindow;

        loadWnd.RunFadeOut();
    }
    void UpdateLoadWindow(uint now, uint max, string description)
    {
        if (!ResourcesManager.Get().IsWindowActive(WindowIndex.LoadWindow)) { return; }

        var loadWnd = ResourcesManager.Get().GetWindow(WindowIndex.LoadWindow) as LoadWindow;

        loadWnd.SetSLiderValue(now, max, description);
    }

    public CharacterControllerJellyMesh GetMainChara()
    {
        if (mainChara == null)
        {
            Debug.LogError(" could not find main chara in FieldMgr");
            return null;
        }

        return mainChara;
    }
    public void ClearField()
    {
        if (currentFieldInto != null)
        {
            UIUtility.SetActive(currentFieldInto.gameObject, false);
            Destroy(currentFieldInto.gameObject);
            currentFieldInto = null;
        }

        if (currentFieldCache != null)
        {
            currentFieldCache = null;
        }

        if (mainChara != null)
        {
            UIUtility.SetActive(mainChara.gameObject, false);
            Destroy(mainChara.gameObject);
            mainChara = null;
        }

        if (friendList != null && friendList.Count > 0)
        {
            foreach (var f in friendList)
            {
                UIUtility.SetActive(f.gameObject, false);
                Destroy(f.gameObject);
            }

            friendList.Clear();
        }
    }
}
