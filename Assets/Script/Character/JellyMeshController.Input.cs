using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class JellyMeshController
{
    public void OnMove(Vector3 delta)
    {
        if (jellyMesh == null || !jellyMesh.IsMeshCreated) { return; }

        if (JellyMeshIsGrounded(LayerUtility.FieldEnvObjectMask, 1))
        {
            if (delta.x > 0f && GetJellyMeshVelocity().x < moveSpeed)//これ速度だぜ！
            {
                JellyMeshAddForce(Vector3.right * GameSettingTable.CharaStatusSpeedAdd, false);
                facingDir = Vector3.right;
            }
            else if (delta.x < 0f && GetJellyMeshVelocity().x > -moveSpeed)
            {
                JellyMeshAddForce(Vector3.left * GameSettingTable.CharaStatusSpeedAdd, false);
                facingDir = Vector3.left;
            }
        }
        else
        {
            //向きだけ変える
            if (delta.x > 0f)
            {
                facingDir = Vector3.right;
            }
            else if (delta.x < 0f)
            {
                facingDir = Vector3.left;
            }
        }
    }
    public void OnClickJump()
    {
        if (jellyMesh == null || !jellyMesh.IsMeshCreated) { return; }

        if (JellyMeshIsGrounded(LayerUtility.FieldEnvObjectMask, 1))
        {
            JellyMeshAddForce(Vector3.up * jumpPower, false);
            SoundManager.Get().PlayOneShotSe_Jump();
        }
    }
    public void OnClickAction()
    {
        var pos = colliderController.transform.position;
        pos.y += 1.0f;//頭上に持ってくる

        var vel = GetJellyMeshVelocity();
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

            facingDir.y += 1.0f;//斜め上に向く
            facingDir.Normalize();

            var setPos = GetMeshPosition() + facingDir;
            FieldManager.Get().CreateOneEnemySlime(setPos, (i) =>
             {
                 i.PushOutThisSlime(facingDir, vel);
             });
        }
    }
}
