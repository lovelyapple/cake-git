using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : SingleToneBase<FieldManager>
{
    MainSlimeController mainChara;
    List<AIFriendSlime> friendList;
    [SerializeField] GameObject characterRoot;
    [SerializeField] GameObject dungeonRoot;
    [SerializeField] CameraController mainCameraCtrl;
    FieldInfo currentFieldInto;
    FieldInfo currentFieldCache;
    [SerializeField] string loadDungeonName;
    [SerializeField] int debugWaitSec = 1;
    public Vector3 cameraOffset = new Vector3(0, 0, -10f);
    uint maxLoadStats = 5;
    string creatingMap = "フィールドデータロード中";
    string createMainChara = "メインキャラロード中";
    string createFreind = "スライム生成中";
    string createEnemy = "敵生成中";
    uint friendLeftCount;
    public Action<uint> OnUpdateFriendCount;
    public void CreateField(Action OnFinished, Action OnError)
    {
        StartCoroutine(CreateFieldAsync(OnFinished, OnError));
    }
    IEnumerator CreateFieldAsync(Action OnFinished, Action OnError)
    {
        yield return LoadDungeon();
        yield return LoadMainCharacter();
        yield return LoadFriendCharacter();
        yield return LoadEnemy();
        yield return RunLoadFadeOut();
        if (OnFinished != null)
        {
            OnFinished();
        }
        RequestUpdateFieldInfo();
        StateConfig.IsPausing = false;
    }
    public void ReSetMap(Action OnFinished, Action OnError)
    {
        StartCoroutine(ReSetMapAsync(OnFinished, OnError));
    }
    IEnumerator ReSetMapAsync(Action OnFinished, Action OnError)
    {
        ClearFieldAll(true);
        yield return LoadMainCharacter(true);
        yield return LoadFriendCharacter(true);
        yield return LoadEnemy(true);
        if (OnFinished != null)
        {
            OnFinished();
        }
        RequestUpdateFieldInfo();
        StateConfig.IsPausing = false;
    }
    public IEnumerator LoadDungeon()
    {
        UpdateLoadWindow(1, maxLoadStats, creatingMap);

        if (string.IsNullOrEmpty(loadDungeonName))
        {
            var go = ResourcesManager.Get().CreateInstance(FieldObjectIndex.TestDungeon, dungeonRoot.transform);
            if (go == null)
            {
                ClearFieldAll();
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
                ClearFieldAll();
                yield break;
            }
            currentFieldCache = go.GetComponent<FieldInfo>();

            currentFieldInto = ResourcesManager.CreateGetComponent<FieldInfo>(currentFieldCache.gameObject);

            if (currentFieldInto != null)
            {
                ClearFieldAll();
                yield break;
            }
            currentFieldInto.transform.parent = dungeonRoot.transform;
        }

        UIUtility.SetActive(currentFieldInto.gameObject, true);
        yield return new WaitForSeconds(debugWaitSec);
    }
    public IEnumerator LoadMainCharacter(bool isReset = false)
    {
        if (!isReset)
        {
            UpdateLoadWindow(2, maxLoadStats, createMainChara);
        }

        if (currentFieldInto == null)
        {
            ClearFieldAll();
            yield break;
        }

        var startP = currentFieldInto.GetStartPoint();

        if (startP == null)
        {
            ClearFieldAll();
            yield break;
        }

        var charaObj = ResourcesManager.Get().CreateInstance(FieldObjectIndex.SlimeMainChara, characterRoot.transform);
        if (charaObj != null)
        {
            mainChara = charaObj.GetComponent<MainSlimeController>();
            charaObj.transform.position = currentFieldInto.GetStartPoint().gameObject.transform.position;
            mainChara.GetCharaMeshController().CreateCharacter((g) =>
            {
                if (mainChara != null)
                {
                    mainCameraCtrl.SetupCamera(g, cameraOffset);
                }
            });


        }
        yield return new WaitForSeconds(debugWaitSec);
    }
    public IEnumerator LoadFriendCharacter(bool isReset = false)
    {
        if (!isReset)
        {
            UpdateLoadWindow(3, maxLoadStats, createFreind);
            yield return new WaitForSeconds(debugWaitSec);
        }

        if (currentFieldInto == null)
        {
            ClearFieldAll();
            yield break;
        }

        friendList = new List<AIFriendSlime>();
        foreach (var friend in currentFieldInto.friendSlimeList)
        {
            var fObj = ResourcesManager.Get().CreateInstance(FieldObjectIndex.SlimeFriend, characterRoot.transform);
            if (fObj == null)
            {
                ClearFieldAll();
                yield break;
            }

            fObj.transform.position = friend.gameObject.transform.position;
            var fAI = fObj.GetComponent<AIFriendSlime>();
            fAI.CreateJellyMesh((g) =>
            {
                //do noth
            });
            friendList.Add(fAI);
        }
        friendLeftCount = (uint)friendList.Count;
    }
    public IEnumerator LoadEnemy(bool isReset = false)
    {
        if (!isReset)
        {
            UpdateLoadWindow(4, maxLoadStats, createEnemy);
            yield return new WaitForSeconds(debugWaitSec);
            UpdateLoadWindow(5, maxLoadStats, createEnemy);
            yield return new WaitForSeconds(debugWaitSec);
        }
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

    public MainSlimeController GetMainChara()
    {
        if (mainChara == null)
        {
            Debug.LogError(" could not find main chara in FieldMgr");
            return null;
        }

        return mainChara;
    }
    public void ClearFieldAll(bool isReset = false)
    {
        if (currentFieldInto != null && !isReset)
        {
            UIUtility.SetActive(currentFieldInto.gameObject, false);
            Destroy(currentFieldInto.gameObject);
            currentFieldInto = null;
        }

        if (currentFieldCache != null && !isReset)
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

        //EnemyList Reset
    }
    public void RequestUpdateFieldInfo()
    {
        if (OnUpdateFriendCount != null)
        {
            OnUpdateFriendCount(friendLeftCount);
        }

        if (mainChara == null) { return; }
        var mainJellyMeshCtrl = mainChara.GetCharaMeshController();
        if (mainCameraCtrl == null) { return; }
        mainJellyMeshCtrl.ChangeCharacterStatusLevel(0);
    }
    public void RequestInserFriendSLime(int diff)
    {
        RequestUpdateFriendCount(-diff);

        if (mainChara == null) { return; }
        mainChara.GetCharaMeshController().ChangeCharacterStatusLevel(diff);
    }
    public void RequestUpdateFriendCount(int diff)
    {
        var countAfter = friendLeftCount + diff;

        if (countAfter < 0 || countAfter > friendList.Count)
        {
            Debug.LogWarning("out of friend count range !");
            return;
        }

        friendLeftCount = (uint)countAfter;

        if (OnUpdateFriendCount != null)
        {
            OnUpdateFriendCount(friendLeftCount);
        }
    }
    public void SetUpOnUpdateMainCharaStatusLevel(Action<CharacterData> onUpdate)
    {
        if (mainChara == null) { return; }
        var mainJellyMeshCtrl = mainChara.GetCharaMeshController();
        if (mainCameraCtrl == null) { return; }
        mainJellyMeshCtrl.OnStatusChanged = onUpdate;
    }
}
