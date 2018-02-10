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
public class JellyMeshController : MonoBehaviour
{
    [SerializeField] protected CharacterData slimeDataPrefab;
    [SerializeField] protected JellyMesh jellyMesh;
    [SerializeField] protected CharacterColliderController colliderController;
    [SerializeField] protected GameObject jellyMeshReferenceParentObj;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected uint hp;
    [SerializeField] protected float jumpPower;
    [SerializeField] protected float weight;
    [Range(5f, 100f)]
    public float moveSpeedAdd = 50f;
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
        jumpPower = (1 - charaData.GetWeight()) * 1000;

        if (OnStatusChanged != null)
        {
            OnStatusChanged(charaData);
        }
    }
    /// キャラクタのステータスレベルを変更
    /// targetState変更したいステータスレベル
    public void ChangeCharacterStatusLevel(int levelDiff)
    {
        if (charaData == null) { return; }

        charaData.ChangeStatusLevelDiff(levelDiff);
        UpdateCharacterStatus();
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
    //
    //操作関連(todo 1.3で分ける)
    //

    public void UpdateCharacterInput()
    {
        if (jellyMesh == null || !jellyMesh.IsMeshCreated) { return; }
        if (JellyMeshIsGrounded(LayerUtility.FieldEnvObjectMask, 1))
        {
            if (Input.GetKey(KeyCode.D) && GetJellyMeshVelocity().x < moveSpeed)//これ速度だぜ！
            {
                JellyMeshAddForce(Vector3.right * moveSpeedAdd, false);
                facingDir = Vector3.right;
            }
            else if (Input.GetKey(KeyCode.A) && GetJellyMeshVelocity().x > -moveSpeed)
            {
                JellyMeshAddForce(Vector3.left * moveSpeedAdd, false);
                facingDir = Vector3.left;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                JellyMeshAddForce(Vector3.up * jumpPower, false);
                SoundManager.Get().PlayOneShotSe_Jump();
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.D))
            {
                facingDir = Vector3.right;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                facingDir = Vector3.left;
            }
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            var pos = colliderController.transform.position;
            pos.y += 1.0f;//test todo なんか距離とった方がいいな

            var vel = GetJellyMeshVelocity();
            var isparaNull = parasitismingEnemy == null;
            var isCurrentStatusabove = charaData.GetCurrentStatusLevel() > 1;
            var isReachMax = FieldManager.Get().IsReachingMaxFriendAmount();
            if (parasitismingEnemy == null && charaData.GetCurrentStatusLevel() > 1 && !FieldManager.Get().IsReachingMaxFriendAmount())
            {
                var ai = FieldManager.Get().CreateOneFriendSlime(pos, (i) =>
                 {
                     FieldManager.Get().RequestCatchSLimeFromField(-1);
                     i.PushOutThisSlime(Vector3.zero, facingDir, vel);
                 });
            }
            else if (parasitismingEnemy != null)
            {
                FieldManager.Get().RemoveOneEnemySlime(parasitismingEnemy.enemyId);
                parasitismingEnemy = null;

                facingDir.y += 1.0f;
                facingDir.Normalize();

                var setPos = GetMeshPosition() + facingDir * 1.2f;
                FieldManager.Get().CreateOneEnemySlime(setPos, (i) =>
                 {
                     i.PushOutThisSlime(facingDir, vel);
                 });


            }
        }
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
