using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * characterの動作を制御するメインクラス
 * キャラクタのデータはCreateCharacter(jellyMesh)が作成されるときに
 * slimeDataPrefabで読み込まれ、
 * charaDataに保存される。
 * データの操作はCharacterDataで管理する。
 */
public  partial class JellyMeshController : MonoBehaviour
{
    [SerializeField] protected CharacterData slimeDataPrefab;
    [SerializeField] protected JellyMesh jellyMesh;
    [SerializeField] protected CharacterColliderController colliderController;
    [SerializeField] protected GameObject jellyMeshReferenceParentObj;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected uint hp;
    [SerializeField] protected float jumpPower;
    [SerializeField] protected float weight;

    float? damageCoolingTime;
    public Action<CharacterData> OnStatusChanged = null;
    public CharacterData charaData { get; private set; }
    Rigidbody jellyMeshRigidbody;
    [SerializeField] Vector3 facingDir = Vector3.up;
    public AIEnemy parasitismingEnemy;

    void OnDisable()
    {
        OnStatusChanged = null;
    }
    public void CreateCharacter(Action<JellyMesh> onFinished)
    {
        if (jellyMesh == null)
        {
            jellyMesh = GetComponent<JellyMesh>();

            if (jellyMesh == null)
            {
                Debug.LogError("could not fine JeelyMesh");
                return;
            }
        }

        if (slimeDataPrefab == null)
        {
            Debug.Log("could not find slimeData");
            jellyMesh = null;
            return;
        }
        else
        {
            var go = (GameObject)(GameObject.Instantiate(slimeDataPrefab.gameObject));
            go.transform.parent = this.transform;
            go.transform.position = this.transform.position;
            charaData = go.GetComponent<CharacterData>();
        }

        if (colliderController == null)
        {
            Debug.LogWarning("could not fine CollderController ad new one");
            colliderController = ResourcesManager.Get().CreateInstance(FieldObjectIndex.SlimeCharacterCollderController).GetComponent<CharacterColliderController>();
        }

        jellyMesh.CreateJelleMeshReferenceObj((res) =>
        {
            colliderController.SetUpController(jellyMesh.m_CentralPoint.transform);
            charaData.ResetStatusLevel();
            UpdateCharacterStatus();

            if (onFinished != null)
            {
                onFinished(jellyMesh);
            }
        });
    }
    //
    //キャラステータス関連
    //
    public void ResetCharaStatus()
    {
        if (charaData == null) { return; }

        charaData.ResetStatusLevel();
        UpdateCharacterStatus();
    }
    public void UpdateCharacterStatus(uint? targetStatusLevel = null)
    {
        if (charaData == null) { return; }

        if (targetStatusLevel.HasValue)
        {
            //今何もしない
        }

        weight = charaData.GetWeight();
        hp = charaData.GetHp();
        moveSpeed = charaData.GetMoveSpeed();
        jumpPower = (1 - charaData.GetWeight()) * GameSettingTable.CharacStatusWeightMax;

        if (OnStatusChanged != null)
        {
            OnStatusChanged(charaData);
        }
    }
    public bool IsDead()
    {
        if (charaData == null) { return false; }
        return charaData.IsDead;
    }
    // キャラにダメーじを加える
    public void RequestDamageCharacter(int damage)
    {
        if (damageCoolingTime.HasValue) { return; }

        StartCoroutine(IeDamageChara(damage));
    }
    IEnumerator IeDamageChara(int damage)
    {
        if (!ChangeCharacterStatusLevel(-damage))
        {
            yield break;
        }

        var jumpX = UnityEngine.Random.Range(-1, 1) > 0 ? 1 : -1;
        var jumpDir = new Vector3(jumpX, 1f, 0);
        jumpDir = jumpDir.normalized * GameSettingTable.CharaDamagedDumpPower;
        JellyMeshAddForce(jumpDir, false);

        FxManager.Get().CreateFx_Damage(GetMeshPosition());
        damageCoolingTime = GameSettingTable.CharaDamageCoolingTime;
        SoundManager.Get().PlayOneShotSe_Damage();

        while (damageCoolingTime.HasValue)
        {
            damageCoolingTime -= Time.deltaTime;
            if (damageCoolingTime.Value <= 0)
            {
                damageCoolingTime = null;
            }
            yield return null;
        }
    }
    /// キャラクタのステータスレベルを変更
    /// targetState変更したいステータスレベル
    public bool ChangeCharacterStatusLevel(int levelDiff)
    {
        if (charaData == null || IsDead()) { return false; }

        charaData.ChangeStatusLevelDiff(levelDiff);

        if (!IsDead())
        {
            UpdateCharacterStatus();
        }
        else
        {
            WindowManager.CreateOpenWindow(WindowIndex.ResultWindow, (w) =>
             {
                 var wnd = w as ResultWindow;
                 wnd.SetUp(FieldManager.Get().savedFriendCount, false);
             });
        }
        return true;
    }
    public bool IsCharaReachingMaxLevel()
    {
        if (charaData == null) { return true; }

        return charaData.GetCurrentStatusLevel() == charaData.maxLevel;
    }
    public bool IsCharaReachingMinLevel()
    {
        if (charaData == null) { return true; }
        return charaData.GetCurrentStatusLevel() == 1;//最小
    }
    
    //JellyMeshのヘルパー(todo 1.2で分けたい)
    public Vector3 GetJellyMeshVelocity()
    {
        if (jellyMesh == null)
        {
            return Vector3.zero;
        }

        if (jellyMeshRigidbody == null)
        {
            jellyMeshRigidbody = jellyMesh.m_CentralPoint.GameObject.GetComponent<Rigidbody>();
        }
        return jellyMeshRigidbody.velocity;
    }
    public Vector3 GetMeshPosition()
    {
        return colliderController.transform.parent.position;
    }
    public void SetMeshActive(bool active)
    {
        return;
        if (jellyMesh == null || jellyMesh.m_ReferencePointParent == null) { return; }

        jellyMesh.m_ReferencePointParent.gameObject.SetActive(active);
        this.gameObject.SetActive(active);
    }
    public void SetJellyMeshPosition(Vector3 position, bool resetVelocity)
    {
        if (jellyMesh == null) { return; }
        jellyMesh.SetPosition(position, resetVelocity);
    }
    public void RestJellyMeshScale()
    {
        if (jellyMesh == null) { return; }
        //var s = 1f / jellyMesh.scaleHistory;
        SetJellyMeshScale(1);
    }
    public void SetJellyMeshScale(float scaleDiff)
    {
        if (jellyMesh == null) { return; }
        jellyMesh.Scale(scaleDiff);
    }
    public void SetJellyMeshKinematic(bool isKinematic, bool centralPointOnly)
    {
        if (jellyMesh == null) { return; }
        jellyMesh.SetKinematic(isKinematic, centralPointOnly);
    }
    public void RotateJellyMesh(Vector3 eulerAngleChange)
    {
        if (jellyMesh == null) { return; }
        jellyMesh.Rotate(eulerAngleChange);
    }
    public bool JellyMeshIsGrounded(LayerMask groundLayer, int minGroundedBodies)
    {
        if (jellyMesh == null) { return true; }
        return jellyMesh.IsGrounded(groundLayer, minGroundedBodies);
    }
    public void JellyMeshAddForce(Vector3 force, bool centralPointOnly)
    {
        if (jellyMesh == null) { return; }
        jellyMesh.AddForce(force, centralPointOnly);
    }
    public void JellyMeshAAddTorque(Vector3 torque, bool centralPointOnly)
    {
        if (jellyMesh == null) { return; }
        jellyMesh.AddTorque(torque, centralPointOnly);
    }
    public void JeelyMeshAddForceAtPosition(Vector2 force, Vector2 position)
    {
        if (jellyMesh == null) { return; }
        jellyMesh.AddForceAtPosition(force, position);
    }
}
