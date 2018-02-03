using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject lookAtTarget;
    [SerializeField] GameObject offsetTarget;
    [SerializeField] float distacneDump = 10.0f;
    [SerializeField] Camera targetCamera;
	Vector3 offseTargetPos;
    Vector3 velocity = Vector3.one;
    public bool lookAtTaretFlag = true;

    public void SetupCamera(GameObject targetObj, GameObject offsetObj)
    {
        this.lookAtTarget = targetObj;
        this.offsetTarget = offsetObj;
    }
    void Update()
    {
        if (targetCamera == null)
        {
            Debug.LogError("could not find camera");
            return;
        }
        LookAtTarget();
        FollowTheOffseTarget();
    }
    void FollowTheOffseTarget()
    {
        if (offsetTarget == null) { return; }
        var curPos = Vector3.SmoothDamp(targetCamera.transform.position, offsetTarget.transform.position, ref velocity, distacneDump);
        targetCamera.transform.position = curPos;
    }
    void LookAtTarget()
    {
        if (lookAtTaretFlag)
            targetCamera.transform.LookAt(lookAtTarget.transform.position, Vector3.up);
    }
}
