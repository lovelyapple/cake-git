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
public class CharacterControllerJellyMesh : MonoBehaviour
{
    [SerializeField] CharacterData slimeDataPrefab;
    [SerializeField] JellyMesh jellyMesh;
    [SerializeField] CharacterColliderController colliderController;
    [SerializeField] GameObject jellyMeshReferenceParentObj;
    [SerializeField] float moveSpeed;
    [SerializeField] uint hp;
    [SerializeField] float jumpPower;
    [SerializeField] float weight;
    [Range(5f, 100f)]
    public float moveSpeedTest = 50f;
    public Action<CharacterData> OnStatusChanged = null;
    public CharacterData charaData { get; private set; }
    public void CreateCharacter()
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
            colliderController.SetUpController(jellyMesh.m_ReferencePointParent.transform);
            charaData.ResetStatusLevel();
            UpdateCharacterStatus();
        });
    }
    public void UpdateCharacterStatus(uint? targetStatusLevel = null)
    {
        if (charaData == null)
        {
            return;
        }

        if (targetStatusLevel.HasValue)
        {
            //今何もしない
        }

        weight = charaData.GetWeight();
        hp = charaData.GetHp();
        moveSpeed = charaData.GetMoveSpeed();

    }
    void Update()
    {
        UpdateCharacter();
    }
    //操作関連
    void UpdateCharacter()
    {
        if (jellyMesh == null) { return; }
        if (JellyMeshIsGrounded(LayerUtility.FieldEnvObjectMask, 1))
        {
            if (Input.GetKey(KeyCode.D))
            {
                JellyMeshAddForce(Vector3.right * moveSpeedTest, false);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                JellyMeshAddForce(Vector3.left * moveSpeedTest, false);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                JellyMeshAddForce(Vector3.up * moveSpeedTest * 10, false);
            }
        }
        else
        {

        }
    }
    /// キャラクタのステータスレベルを変更
    /// targetState変更したいステータスレベル
    public void ChangeCharacterStatusLevel(int levelDiff)
    {
        if (charaData == null) { return; }
        charaData.ChangeStatusLevelDiff(levelDiff);

        if (OnStatusChanged != null)
        {
            OnStatusChanged(charaData);
        }
    }
    /// キャラクタのステータスレベルを変更
    /// targetState変更した後のステータスレベル
    /// 非推奨
    public void ChangeCharacterStatusLevelTo(uint targetLevel)
    {
        if (charaData == null) { return; }
        int diff = (int)(targetLevel - charaData.currentLevel);
        charaData.ChangeStatusLevelDiff(diff);

        if (OnStatusChanged != null)
        {
            OnStatusChanged(charaData);
        }
    }

    //JellyMeshのヘルパー
    public void SetJellyMeshPosition(Vector3 position, bool resetVelocity)
    {
        if (jellyMesh == null) { return; }
        jellyMesh.SetPosition(position, resetVelocity);
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
