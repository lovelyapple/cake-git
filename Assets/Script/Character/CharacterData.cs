using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public int maxLevel { get; private set; }
    public uint currentLevel { get; private set; }
    public bool IsDead { get { return currentLevel == 0; } }
    public List<uint> HpStatusList;
    public List<float> WeightStatusList;
    public List<float> MoveSpeedStatusList;

    public void ResetStatusLevel()
    {
        maxLevel = HpStatusList.Count;

        if (maxLevel != WeightStatusList.Count)
        {
            Debug.LogError("WeightStatusList doesnt has right element count");
        }

        if (maxLevel != MoveSpeedStatusList.Count)
        {
            Debug.LogError("MoveSpeedStatusList doesnt has right element count");
        }
    }
    public void ChangeStatusLevelDiff(int levelDiff)
    {
        currentLevel = (uint)Mathf.Clamp(currentLevel + levelDiff, 0, maxLevel);
    }
    public uint GetHp()
    {
        return GetHp(currentLevel);
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
    public float GetWeight()
    {
        return GetWeight(currentLevel);
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
    public float GetMoveSpeed()
    {
        return GetMoveSpeed(currentLevel);
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
