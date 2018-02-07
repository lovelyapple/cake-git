using System;
using System.Linq;
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
    uint defaultFriendLeftCount;//救えるフレンドの数
    public uint savedFriendCount { get; private set; }//救ったスライムの数

    public Action<uint> OnUpdateFriendCount;
    public Action OnSaveOnFriend;
    uint friendIdx = 0;//やらかした..くそ(挽回はできるけど、FieldObjectまで改造しまうのでとりあえずこれで)


    //
    // マップロード関連
    //

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
                    mainCameraCtrl.SetupCamera(g.m_CentralPoint.GameObject, cameraOffset);
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
        friendIdx = 0;

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
                fAI.friendId = friendIdx;
            });
            friendList.Add(fAI);
            friendIdx++;
        }
        defaultFriendLeftCount = (uint)friendList.Count;
        savedFriendCount = 0;
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

    //
    // スライム操作と情報の更新
    //

    /// マップの情報を更新する
    public void RequestUpdateFieldInfo()
    {
        if (OnUpdateFriendCount != null)
        {
            OnUpdateFriendCount((uint)friendList.Count);
        }

        if (mainChara == null) { return; }
        var mainJellyMeshCtrl = mainChara.GetCharaMeshController();
        if (mainCameraCtrl == null) { return; }
        mainJellyMeshCtrl.ChangeCharacterStatusLevel(0);
    }
    /// メインキャラにスライムを与える
    public void RequestGiveMainCharaFriendSLime(int diff)
    {
        RequestUpdateFieldFriendCount(-diff);
        RequestUpdateMainCharaSlimeCount(diff);
    }
    /// スライムを一個解放する
    public void RequestReleaseOneSlime()
    {
        RequestUpdateFieldFriendCount(-1);
        savedFriendCount += 1;

        if (OnSaveOnFriend != null)
        {
            OnSaveOnFriend();
        }
    }
    /// フィール上のスライム数更新
    public void RequestUpdateFieldFriendCount(int diff)
    {
        var countAfter = friendList.Count + diff;

        if (countAfter < 0 || countAfter > defaultFriendLeftCount)
        {
            Debug.LogWarning("out of friend count range !");
            return;
        }

        if (OnUpdateFriendCount != null)
        {
            OnUpdateFriendCount((uint)friendList.Count);
        }
    }
    /// メインキャラのスライム数更新
    public void RequestUpdateMainCharaSlimeCount(int diff)
    {
        if (mainChara == null) { return; }
        mainChara.GetCharaMeshController().ChangeCharacterStatusLevel(diff);
        mainChara.GetCharaMeshController().UpdateCharacterStatus();
    }
    /// フィールドデータ更新イベント登録
    public void SetUpOnUpdateMainCharaStatusLevel(Action<CharacterData> onUpdate)
    {
        if (mainChara == null) { return; }

        var mainJellyMeshCtrl = mainChara.GetCharaMeshController();

        if (mainCameraCtrl == null) { return; }

        mainJellyMeshCtrl.OnStatusChanged = onUpdate;
    }
    ///現在のフレンドがデフォルト値以上かどうか
    public bool IsReachingMaxFriendAmount()
    {
        return friendList.Count > defaultFriendLeftCount;
    }
    /// フレンドスライムを一個作成
    public AIFriendSlime CreateOneFriendSlime(Vector3 position, Action<AIFriendSlime> onResult)
    {
        var fObj = ResourcesManager.Get().CreateInstance(FieldObjectIndex.SlimeFriend, characterRoot.transform);
        fObj.transform.position = position;

        if (fObj == null) { return null; }

        friendIdx++;
        var fc = fObj.GetComponent<AIFriendSlime>();

        fc.CreateJellyMesh((j) =>
        {
            fc.friendId = friendIdx;
            j.SetPosition(position, true);
            friendList.Add(fc);
            RequestUpdateFieldInfo();
            if (onResult != null)
            {
                onResult(fc);
            }
        });

        return fc;
    }

    public void RemoveOneFriendSlime(uint id)
    {
        var f = friendList.First(friend => friend.friendId == id);

        if (f != null)
        {
            friendList.Remove(f);
            Destroy(f.gameObject);
            RequestUpdateFieldInfo();
        }
    }

    //
    //ヘルパー
    //

    public FieldObjectBase GetCurrentFieldEndArea()
    {
        if (currentFieldInto == null) { return null; }
        return currentFieldInto.GetEndArea();
    }



}
