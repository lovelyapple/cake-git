using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AIFriendSlime : AIBase
{
    [Range(1.0f, 3.0f)] public float _disperaTime = 2.0f;
    [Range(0.8f, 1f)] public float _scaleDiff = 0.95f;
    [Range(10f, 50f)] public float _forceSpeed = 20f;
    [SerializeField] float? disperaTime;
    bool isDead = false;
    CharacterControllerJellyMesh mainCharaCtrlJellyMesh;
    //CharacterController mainCharaColliderCtrl;
    void Update()
    {
        if (!disperaTime.HasValue) { return; }

        disperaTime -= Time.deltaTime;
        charaMeshController.SetJellyMeshScale(_scaleDiff);

        if (mainCharaCtrlJellyMesh == null)
        {
            mainCharaCtrlJellyMesh = FieldManager.Get().GetMainChara().GetCharaMeshController();
        }

        var dir = mainCharaCtrlJellyMesh.GetMeshPosition() - colliderController.transform.position;
        dir.Normalize();
        charaMeshController.JellyMeshAddForce(dir * _forceSpeed, false);

        if (disperaTime.Value <= 0)
        {
            charaMeshController.SetMeshActive(false);
            disperaTime = null;
        }
    }
    protected override void OnTriggerEnterCheckFix(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (!isDead)
            {
                FieldManager.Get().RequestInserFriendSLime(1);
                disperaTime = _disperaTime;
                isDead = true;
            }
        }
    }
}