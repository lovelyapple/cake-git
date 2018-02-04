using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum FollowStlye
    {
        Vecter,
        SoftDump,
        Lerp,
    }
    public FollowStlye followStlye = FollowStlye.Vecter;
    [SerializeField] GameObject lookAtTarget;
    [SerializeField] GameObject offsetTarget;
    [Range(0.1f, 5.0f)] public float softDumprange = 5.0f;
    [Range(0.01f, 5.0f)] public float softMoveEplision = 0.1f;
    [Range(0.1f, 5.0f)] public float simpleFollowSpeedLevel = 0.5f;
    [Range(0.1f, 5.0f)] public float lerpDumpingDistance = 0.5f;
    [Range(0.1f, 5.0f)] public float lerpFarOffsetDistance = 2.0f;
    [Range(0.01f, 1.0f)] public float lerpMoveEplision = 0.1f;
    [SerializeField] Camera targetCamera;
    Vector3 offseTargetPos;
    [SerializeField] Vector3 velocity = Vector3.one;
    public bool lookAtTaretFlag = true;

    public void SetupCamera(GameObject targetObj, GameObject offsetObj)
    {
        this.lookAtTarget = targetObj;
        this.offsetTarget = offsetObj;
    }
    public void SetupCamera(GameObject targetObj, Vector3 offeset)
    {
        this.lookAtTarget = targetObj;
        this.offseTargetPos = offeset;
    }
    void Update()
    {
        if (targetCamera == null || lookAtTarget == null)
        {
            if (targetCamera == null)
                Debug.LogError("could not find camera");
            return;
        }
        LookAtTarget();

        switch (followStlye)
        {
            case FollowStlye.Vecter:
                FollowVector();
                break;
            case FollowStlye.SoftDump:
                FollowSoftDump();
                break;
            case FollowStlye.Lerp:
                FollowLerp();
                break;
        }
    }
    void FollowSoftDump()
    {
        if (offsetTarget != null)
        {
            var curPos = Vector3.SmoothDamp(targetCamera.transform.position, offsetTarget.transform.position, ref velocity, softDumprange);
            targetCamera.transform.position = curPos;
        }
        else
        {
            var ifxPos = lookAtTarget.transform.position + offseTargetPos;
            if ((ifxPos - targetCamera.transform.position).magnitude < softMoveEplision) { return; }
            var curPos = Vector3.SmoothDamp(targetCamera.transform.position, ifxPos, ref velocity, softDumprange);
            targetCamera.transform.position = curPos;
        }
    }
    public void FollowVector()
    {
        if (offsetTarget != null)
        {
            var diff = (offsetTarget.transform.position - targetCamera.transform.position) * simpleFollowSpeedLevel;
            targetCamera.transform.position += diff * Time.deltaTime;
        }
        else
        {
            var ifxPos = lookAtTarget.transform.position + offseTargetPos;
            var diff = (ifxPos - targetCamera.transform.position) * simpleFollowSpeedLevel;
            targetCamera.transform.position += diff * Time.deltaTime;
        }

    }
    public void FollowLerp()
    {
        if (offsetTarget != null)
        {
            var targetVec = offsetTarget.transform.position - targetCamera.transform.position;
            if (targetVec.magnitude < lerpMoveEplision)
            {
                return;
            }
            targetVec.Normalize();
            var targetPosition = offsetTarget.transform.position + targetVec * lerpFarOffsetDistance;
            targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, targetPosition, lerpDumpingDistance * Time.deltaTime);
        }
        else
        {
            var ifxPos = lookAtTarget.transform.position + offseTargetPos;
            var targetVec = ifxPos - targetCamera.transform.position;
            if (targetVec.magnitude < lerpMoveEplision)
            {
                return;
            }
            targetVec.Normalize();
            var targetPosition = ifxPos + targetVec * lerpFarOffsetDistance;
            targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, targetPosition, lerpDumpingDistance * Time.deltaTime);
        }
    }
    void LookAtTarget()
    {
        if (lookAtTaretFlag)
            targetCamera.transform.LookAt(lookAtTarget.transform.position, Vector3.up);
    }
}
