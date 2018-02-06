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
    Vector3? freeDumpDir;
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
            if (freeDumpDir.HasValue && actionBuffTime.HasValue)
            {
                //charaMeshController.JellyMeshAddForce(freeDumpDir.Value * _forceSpeed, false);
            }
        }

        if (actionBuffTime.HasValue)
        {
            actionBuffTime -= Time.deltaTime;

            if (actionBuffTime <= 0f)
            {
                actionBuffTime = null;
                freeDumpDir = null;
            }
        }
    }
    public void PushOutThisSlime(Vector3 startPos, Vector3 dir,Vector3 vel)
    {
        if (StateConfig.IsPausing) { return; }

        startPos.y += 0.5f;
        dir.y += 0.5f;
        dir.Normalize();
        actionBuffTime = _actionBuffTime;
        actionState = ActionStats.Free;
        //charaMeshController.SetJellyMeshPosition(startPos, true);
        charaMeshController.SetMeshActive(true);
        charaMeshController.RestJellyMeshScale();
        
        //todo これ使えばいいけど
        charaMeshController.JellyMeshAddForce(vel * 1000f, true);
        freeDumpDir = dir;
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
                }
                break;
            case "CatchArea":
                if (actionState == ActionStats.Free)
                {
                    //フィールドのスライムカウントを一個戻す
                    FieldManager.Get().RequestUpdateFieldFriendCount(-1);
                    scaleTime = _scaleTime;
                    actionState = ActionStats.Release;
                    charaMeshController.SetJellyMeshScale(50);
                    var endArea = FieldManager.Get().GetCurrentFieldEndArea();

                    if (endArea != null)
                    {
                        moveTargetPos = endArea.gameObject.transform.position;
                    }
                }
                break;
        }
    }
}