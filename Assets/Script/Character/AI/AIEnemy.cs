using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemy : AIBase
{
    public enum ActionState
    {
        StandBy,
        Attacking,
        Cooling,
    }
    public ActionState actionState = ActionState.StandBy;
    MainSlimeController mainChara;
    [SerializeField] float searchRange = 2f;//todo charaData化
    [SerializeField] float releaseRange = 2.5f;
    [SerializeField] float coolingTime = 5f;
    [SerializeField] float attackingTime = 2f;
    [SerializeField] float damageTime = 2f;
    [SerializeField] float jumpPower = 350f;
    public float pushForce = 2200f;
    public uint enemyId;
    public bool catchingMainCara = false;

    float mainCharaRangePrev;
    [SerializeField] float ct;
    [SerializeField] float at;
    [SerializeField] float dt;
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (mainChara == null)
        {
            mainChara = FieldManager.Get().GetMainChara();
            return;
        }

        var vec = mainChara.GetMeshPosition() - colliderController.transform.position;
        var range = vec.magnitude;

        switch (actionState)
        {
            case ActionState.StandBy:
                if (range <= searchRange)
                {
                    vec.Normalize();
                    vec.y += 0.3f;
                    charaMeshController.JellyMeshAddForce(vec * jumpPower, false);
                    actionState = ActionState.Attacking;
                    ct = coolingTime;
                    at = attackingTime;
                }
                break;
            case ActionState.Attacking:
                at -= Time.deltaTime;
                if (mainCharaRangePrev > range && at <= 0)
                {
                    actionState = ActionState.Cooling;
                }
                break;
            case ActionState.Cooling:
                ct -= Time.deltaTime;

                if (ct <= 0)
                {
                    ct = 0;
                    actionState = ActionState.StandBy;
                }
                break;
        }
        mainCharaRangePrev = range;

        if (catchingMainCara)
        {
            if (dt > 0)
            {
                dt -= Time.deltaTime;
                if (releaseRange < range)
                {
                    catchingMainCara = false;
                    mainChara.parasitismingEnemy = null;
                    charaMeshController.SetJellyMeshPosition(charaMeshController.GetMeshPosition(), true);
                }
            }
            else
            {
                dt = damageTime;
                SoundManager.Get().PlayOneShotSe_Damage();

                if (!mainChara.IsDead())
                {
                    FieldManager.Get().RequestDamageMainCharaSLime(-1);
                }
            }
        }

    }

    protected override void OnTriggerEnterCheckFix(Collider col)
    {
        var tag = col.gameObject.tag;
        if (tag == "Player" && actionState == ActionState.Attacking
        && charaMeshController.JellyMeshIsGrounded(LayerUtility.FieldEnvObjectMask, 1))
        {
            actionState = ActionState.Cooling;
            if (mainChara.parasitismingEnemy == null)
            {
                mainChara.parasitismingEnemy = this;
                catchingMainCara = true;
                dt = 0;
            }
        }
    }
    public void PushOutThisSlime(Vector3 dir, Vector3 vel)
    {
        if (StateConfig.IsPausing) { return; }

        actionState = ActionState.Cooling;
        ct = coolingTime;
        var power = vel.magnitude;
        vel.Normalize();

        var finDir = dir * pushForce;
        //todo これ使えばいいけど
        charaMeshController.JellyMeshAddForce(finDir, false);
        catchingMainCara = false;
    }
}
