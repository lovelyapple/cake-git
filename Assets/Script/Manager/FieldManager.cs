using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : SingleToneBase<FieldManager>
{
    MainSlimeController mainChara;
    Dictionary<uint, AIFriendSlime> friendList;
    Dictionary<uint, AIEnemy> enemyList;
    [SerializeField] GameObject characterRoot;
    [SerializeField] GameObject dungeonRoot;
    [SerializeField] CameraController mainCameraCtrl;
    FieldInfo currentFieldInto;
    FieldInfo currentFieldCache;
    [SerializeField] string loadDungeonName;
    [SerializeField] int debugWaitSec = 1;
    public Vector3 cameraOffset = new Vector3(0, 0, -10f);
    uint maxLoadStats = 5;
    uint defaultFriendLeftCount;//救えるフレンドの数
    public uint savedFriendCount { get; private set; }//救ったスライムの数

    public Action<uint> OnUpdateFriendCount;
    public Action OnSaveOneFriend;
    uint friendIdx = 0;//やらかした..くそ(挽回はできるけど、FieldObjectまで改造しまうのでとりあえずこれで)
    uint enemyIdx = 0;

    void OnDisable()
    {
        OnUpdateFriendCount = null;
    }
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
        LoadFieldUI();
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
        LoadFieldUI();
        if (OnFinished != null)
        {
            OnFinished();
        }
        mainChara.ResetCharaStatus();
        RequestUpdateFieldInfo();
        StateConfig.IsPausing = false;
    }
    public void LoadFieldUI()
    {
        WindowManager.CreateOpenWindow(WindowIndex.FieldMenu, (w) =>
        {
            var fieldMenu = w as FieldMenu;
            fieldMenu.SetupFieldData();
            FieldManager.Get().RequestUpdateFieldInfo();

            var loadWnd = WindowManager.GetWindow(WindowIndex.LoadWindow) as LoadWindow;
            if (loadWnd != null && WindowManager.IsWindowActive(WindowIndex.LoadWindow))
            {
                loadWnd.MoveToTop();
            }
        });
    }
    //
    //ダンジョンリソースのロード
    //

    //ダンジョンインスタンス作成
    public IEnumerator LoadDungeon()
    {
        UpdateLoadWindow(1, maxLoadStats, StringTable.LoadPhaseCreateMap);

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
    //メインキャラのインスタンス作成
    public IEnumerator LoadMainCharacter(bool isReset = false)
    {
        if (!isReset)
        {
            UpdateLoadWindow(2, maxLoadStats, StringTable.LoadPhaseCreateMainChara);
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
            mainChara.CreateCharacter((g) =>
            {
                if (mainChara != null)
                {
                    mainCameraCtrl.SetupCamera(g.m_CentralPoint.GameObject, cameraOffset);
                    var wnd = WindowManager.GetWindow(WindowIndex.FieldMenu) as FieldMenu;
                    if (wnd != null)
                    {
                        wnd.SetupFieldData();
                    }
                }
            });
        }
        yield return new WaitForSeconds(debugWaitSec);
    }
    //フレンドスライムの作成
    public IEnumerator LoadFriendCharacter(bool isReset = false)
    {
        if (!isReset)
        {
            UpdateLoadWindow(3, maxLoadStats, StringTable.LoadPhaseCreateFriend);
            yield return new WaitForSeconds(debugWaitSec);
        }

        if (currentFieldInto == null)
        {
            ClearFieldAll();
            yield break;
        }

        friendList = new Dictionary<uint, AIFriendSlime>();
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
            friendList.Add(friendIdx, fAI);
            friendIdx++;
        }

        //所有している分をたす
        var earned = Mathf.Max(mainChara.charaData.GetCurrentStatusLevel() - 1, 0);
        defaultFriendLeftCount = (uint)friendList.Count + (uint)earned;
        savedFriendCount = 0;
    }
    //敵キャラの作成
    public IEnumerator LoadEnemy(bool isReset = false)
    {

        if (!isReset)
        {
            UpdateLoadWindow(5, maxLoadStats, StringTable.LoadPhaseCreateEnemy);
            yield return new WaitForSeconds(debugWaitSec);
        }

        if (currentFieldInto == null)
        {
            ClearFieldAll();
            yield break;
        }

        enemyList = new Dictionary<uint, AIEnemy>();
        enemyIdx = 0;

        foreach (var enemy in currentFieldInto.enemySlimeList)
        {
            var fObj = ResourcesManager.Get().CreateInstance(FieldObjectIndex.SlimeEnemy, characterRoot.transform);
            if (fObj == null)
            {
                ClearFieldAll();
                yield break;
            }

            fObj.transform.position = enemy.gameObject.transform.position;
            var fE = fObj.GetComponent<AIEnemy>();
            fE.CreateJellyMesh((g) =>
            {
                fE.enemyId = enemyIdx;
            });
            enemyList.Add(enemyIdx, fE);
            enemyIdx++;
        }

    }
    public IEnumerator RunLoadFadeOut()
    {
        if (!WindowManager.IsWindowActive(WindowIndex.LoadWindow)) { yield break; }

        var loadWnd = WindowManager.GetWindow(WindowIndex.LoadWindow) as LoadWindow;

        loadWnd.RunFadeOut();
    }
    void UpdateLoadWindow(uint now, uint max, string description)
    {
        if (!WindowManager.IsWindowActive(WindowIndex.LoadWindow)) { return; }

        var loadWnd = WindowManager.GetWindow(WindowIndex.LoadWindow) as LoadWindow;

        loadWnd.SetSLiderValue(now, max, description);
    }

    //
    //ヘルパー
    //

    public MainSlimeController GetMainChara()
    {
        if (mainChara == null)
        {
            Debug.LogError(" could not find main chara in FieldMgr");
            return null;
        }

        return mainChara;
    }
    public uint GetCurrentFriendCountOnField()
    {
        return (uint)friendList.Values.Count(x => x != null && x.IsFree);
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
            foreach (var k in friendList.Keys)
            {
                var f = friendList[k];
                if (f != null)
                {
                    UIUtility.SetActive(f.gameObject, false);
                    Destroy(f.gameObject);
                }
            }

            friendList.Clear();
        }

        if (enemyList != null && enemyList.Count > 0)
        {
            foreach (var k in enemyList.Keys)
            {
                var e = enemyList[k];
                if (e != null)
                {
                    UIUtility.SetActive(e.gameObject, false);
                    Destroy(e.gameObject);
                }
            }

            enemyList.Clear();
        }
    }

    //
    // スライム操作と情報の更新
    //

    /// マップの情報を更新する
    public void RequestUpdateFieldInfo()
    {
        if (OnUpdateFriendCount != null)
        {
            OnUpdateFriendCount(GetCurrentFriendCountOnField());
        }

        if (mainChara != null)
        {
            mainChara.UpdateCharacterStatus();
        }

    }
    /// フィールド上メインキャラがフレンドスライムを吸収する
    public bool RequestCatchSLimeFromField(int diff)
    {
        if (diff > 0 && mainChara.IsCharaReachingMaxLevel())
        {
            return false;
        }
        else if (diff < 0 && mainChara.IsCharaReachingMinLevel())
        {
            return false;
        }

        RequestUpdateFieldFriendCount();
        RequestUpdateMainCharaSlimeCount(diff);
        return true;
    }
    /// フィール上のスライム数更新
    public void RequestUpdateFieldFriendCount()
    {
        if (OnUpdateFriendCount != null)
        {
            OnUpdateFriendCount(GetCurrentFriendCountOnField());
        }
    }
    /// メインキャラのスライム数更新
    public void RequestUpdateMainCharaSlimeCount(int diff)
    {
        if (mainChara == null) { return; }

        mainChara.ChangeCharacterStatusLevel(diff);
    }
    /// メインキャラにダメージを与える
    public void RequestDamageMainCharaSLime(int damage)
    {
        if (mainChara == null) { return; }
        mainChara.RequestDamageCharacter(damage);
    }
    /// スライムを一個解放する
    /// 削除はAIの消失まで待つので、各自実行
    public void RequestPutOneSlimeToCatchArea(AIFriendSlime ai)
    {
        if (friendList.ContainsKey(ai.friendId))
        {
            RequestUpdateFieldFriendCount();
            savedFriendCount += 1;

            if (OnSaveOneFriend != null)
            {
                OnSaveOneFriend();
            }
        }
        else
        {
            Debug.Log("こんな友達持っていないよ");
        }

    }


    /// フィールドデータ更新イベント登録
    public void SetUpOnUpdateMainCharaStatusLevel(Action<CharacterData> onUpdate)
    {
        if (mainChara == null) { return; }

        var mainJellyMeshCtrl = mainChara;

        if (mainCameraCtrl == null) { return; }

        mainJellyMeshCtrl.OnStatusChanged = onUpdate;
    }
    ///現在のフレンドがデフォルト値以上かどうか
    public bool IsReachingMaxFriendAmount()
    {
        return GetCurrentFriendCountOnField() >= defaultFriendLeftCount;
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
            friendList.Add(friendIdx, fc);
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
        var f = friendList[id];

        if (f != null)
        {
            Destroy(f.gameObject);
            friendList[id] = null;
            RequestUpdateFieldInfo();
        }
    }
    /// 敵スライムを一個作成
    public AIEnemy CreateOneEnemySlime(Vector3 position, Action<AIEnemy> onResult)
    {
        var fObj = ResourcesManager.Get().CreateInstance(FieldObjectIndex.SlimeEnemy, characterRoot.transform);
        fObj.transform.position = position;

        if (fObj == null) { return null; }

        enemyIdx++;
        var ec = fObj.GetComponent<AIEnemy>();

        ec.CreateJellyMesh((j) =>
        {
            ec.enemyId = enemyIdx;
            j.SetPosition(position, true);
            enemyList.Add(enemyIdx, ec);
            if (onResult != null)
            {
                onResult(ec);
            }
        });

        return ec;
    }
    public void RemoveOneEnemySlime(uint id)
    {
        var e = enemyList[id];

        if (e != null)
        {
            Destroy(e.gameObject);
            enemyList[id] = null;
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
