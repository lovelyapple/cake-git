using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * charaDataとステータをほじするクラス
 * index上0スタートだが、count上1スタートだけ注意
 */
public class CharacterData : MonoBehaviour
{
    public int maxLevel { get; private set; }
    [SerializeField] uint currentLevel;
    public bool IsDead { get { return currentLevel == 0; } }

    public List<uint> HpStatusList;
    [Header("Jumpに影響を与える jumpPower = (1 - x) * 1000")]
    public List<float> WeightStatusList;
    public List<float> MoveSpeedStatusList;

    public void ResetStatusLevel()
    {
        maxLevel = HpStatusList.Count;
        currentLevel = (uint)(maxLevel / 2);

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
        currentLevel = (uint)Mathf.Clamp(currentLevel + levelDiff, 1, maxLevel);
    }
    public uint GetCurrentStatusLevel()
    {
        return currentLevel;
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
            outData = HpStatusList[(int)--State];
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
            outData = WeightStatusList[(int)--State];
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
            outData = MoveSpeedStatusList[(int)--State];
            return outData;
        }
        catch
        {
            return 0f;
        }
    }
}
