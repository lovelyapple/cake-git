using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AIFriendSlime : AIBase
{
    [Range(1.0f, 3.0f)] public float _scaleTime = 1.0f;
    [Range(1.0f, 3.0f)] public float _actionBuffTime = 2.0f;
    [Range(0.8f, 1f)] public float _scaleDiff = 0.95f;
    [Range(10f, 50f)] public float _forceSpeed = 20f;
    [SerializeField] float? scaleTime;
    [SerializeField] float? actionBuffTime;
    public bool IsHolding { get { return actionState == ActionStats.Hold; } }
    public float pushForce = 800f;
    enum ActionStats
    {
        Free,
        Hold,
        Release,
        Dead,
    }
    ActionStats actionState = ActionStats.Free;
    ActionStats? actionStatsPrev;

    CharacterControllerJellyMesh mainCharaCtrlJellyMesh;
    Vector3 moveTargetPos = Vector3.zero;
    void Update()
    {
        if (scaleTime.HasValue)
        {
            scaleTime -= Time.deltaTime;
            charaMeshController.SetJellyMeshScale(_scaleDiff);

            Vector3 dir = Vector3.zero;

            switch (actionState)
            {
                case ActionStats.Hold:
                    if (mainCharaCtrlJellyMesh == null)
                    {
                        mainCharaCtrlJellyMesh = FieldManager.Get().GetMainChara().GetCharaMeshController();
                    }

                    dir = mainCharaCtrlJellyMesh.GetMeshPosition() - colliderController.transform.position;
                    break;
                case ActionStats.Release:
                    dir = moveTargetPos - colliderController.transform.position;
                    break;
            }


            dir.Normalize();
            charaMeshController.JellyMeshAddForce(dir * _forceSpeed, false);

            if (scaleTime.Value <= 0)
            {
                charaMeshController.SetMeshActive(false);
                scaleTime = null;
                FieldManager.Get().RemoveOneFriendSlime(friendId);
            }
        }
        else
        {
        }

        if (actionBuffTime.HasValue)
        {
            actionBuffTime -= Time.deltaTime;

            if (actionBuffTime <= 0f)
            {
                actionBuffTime = null;
            }
        }
    }
    public void PushOutThisSlime(Vector3 startPos, Vector3 dir, Vector3 vel)
    {
        if (StateConfig.IsPausing) { return; }

        startPos.y += 0.5f;
        dir.y += 0.5f;
        dir.Normalize();
        dir *= pushForce * 2;
        actionBuffTime = _actionBuffTime;
        actionState = ActionStats.Free;
        //charaMeshController.SetJellyMeshPosition(startPos, true);
        charaMeshController.SetMeshActive(true);
        charaMeshController.RestJellyMeshScale();
        var finDir = vel * pushForce + dir;
        //todo これ使えばいいけど
        charaMeshController.JellyMeshAddForce(finDir, true);
    }
    protected override void OnTriggerEnterCheckFix(Collider col)
    {
        var tag = col.gameObject.tag;
        switch (tag)
        {
            case "Player":
                if (actionState == ActionStats.Free && !actionBuffTime.HasValue)
                {
                    FieldManager.Get().RequestGiveMainCharaFriendSLime(1);
                    scaleTime = _scaleTime;
                    actionState = ActionStats.Hold;
                    SoundManager.Get().PlayOneShotSe_Catch();
                }
                break;
            case "CatchArea":
                if (actionState == ActionStats.Free)
                {
                    //フィールドのスライムカウントを一個戻す
                    FieldManager.Get().RequestReleaseOneSlime();
                    scaleTime = _scaleTime;
                    actionState = ActionStats.Release;

                    moveTargetPos = col.gameObject.transform.position;
                    SoundManager.Get().PlayOneShotSe_Release();
                }
                break;
        }
    }
}