using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ゲーム内で使用する文字はこれに登録する
 * csvにしたくないから、これで行く
 */
public static class StringTable
{
    public static string MissionClear { get { return "Clear!"; } }
    public static string MissionFailed { get { return "Fialed"; } }
    public static string LoadPhaseCreateMap { get { return "フィールド生成中"; } }
    public static string LoadPhaseCreateMainChara { get { return "メインキャラ生成中"; } }
    public static string LoadPhaseCreateFriend { get { return "仲間を生成中"; } }
    public static string LoadPhaseCreateEnemy { get { return "敵生成中"; } }
}
