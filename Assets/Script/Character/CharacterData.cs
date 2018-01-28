using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public List<uint> HpStatusList;
    public List<float> WeightStatusList;
    public List<float> MoveSpeedStatusList;

    uint? minHp;
    public uint MinHp
    {
        get
        {
            if (!minHp.HasValue)
            {
                if (HpStatusList == null || HpStatusList.Count == 0)
                {
                    minHp = 0;
                }
                else
                {
                    minHp = HpStatusList[0];
                }
            }

            return minHp.Value;
        }
    }
    public uint GetHp(uint State)
    {
        uint outData = 0;
        try
        {
            outData = HpStatusList[(int)State];
            return outData;
        }
        catch
        {
            return 0;
        }
    }
    public float GetWeight(uint State)
    {
        float outData = 0f;
        try
        {
            outData = WeightStatusList[(int)State];
            return outData;
        }
        catch
        {
            return 0f;
        }
    }
    public float GetMoveSpeed(uint State)
    {
        float outData = 0f;
        try
        {
            outData = MoveSpeedStatusList[(int)State];
            return outData;
        }
        catch
        {
            return 0f;
        }
    }
}
