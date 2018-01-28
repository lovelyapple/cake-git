using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerJellyMesh : MonoBehaviour
{
    [SerializeField] CharacterData slimeData;
    [SerializeField] JellyMesh jellyMesh;
    [SerializeField] CharacterColliderController colliderController;
    [SerializeField] GameObject jellyMeshReferenceParentObj;
    [SerializeField] float moveSpeed;
    [SerializeField] uint hp;
    [SerializeField] float jumpPower;
    [SerializeField] float weight;
    [SerializeField] uint statusLevel = 5;
    [Range(5f, 100f)]
    public float moveSpeedTest = 50f;

    void Start()
    {
        if (jellyMesh == null)
        {
            jellyMesh = GetComponent<JellyMesh>();

            if (jellyMesh == null)
            {
                Debug.LogError("could not fine JeelyMesh");
            }
        }

        if (slimeData == null)
        {
            Debug.Log("could not find slimeData");
        }

        if (colliderController == null)
        {
            Debug.LogWarning("could not fine CollderController ad new one");
            colliderController = ResourcesManager.Get().CreateInstance(FieldObjectIndex.SlimeCharacterCollderController).GetComponent<CharacterColliderController>();
        }
        CreateJellyMesh();
    }
    public void CreateJellyMesh()
    {
        if (jellyMesh == null) { return; }

        jellyMesh.CreateJelleMeshReferenceObj((res) =>
        {
            colliderController.SetUpController(jellyMesh.m_ReferencePointParent.transform);
        });
    }
    public void UpdateCharacterStatus()
    {
        if (slimeData == null)
        {
            return;
        }

        weight = slimeData.GetWeight(statusLevel);
        hp = slimeData.GetHp(statusLevel);
        moveSpeed = slimeData.GetMoveSpeed(statusLevel);

    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        UpdateCharacter();
    }
    void UpdateCharacter()
    {
        if (JellyMeshIsGrounded(LayerUtility.FieldEnvObjectLayerMask(), 1))
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
