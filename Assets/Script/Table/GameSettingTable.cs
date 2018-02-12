using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * マジックナンバーだけは避けたい
 * 同じくcsv面倒いのでそのまま書き込む
 */
public static class GameSettingTable
{
    //
    //変更してはいけないもの
    //
    ///キャラステータス：重さを計算するための係数
    public static float CharacStatusWeightMax { get { return 1000; } }

    //
    //変更していいもの
    //

    ///キャラ:移動加速力
    public static float CharaStatusSpeedAdd { get { return 50f; } }
    ///キャラ:ダメージ受ける時に、ダンプされる力
    public static float CharaDamagedDumpPower { get { return 300f; } }
    ///キャラ:ダメージを受けた後の無敵時間
    public static float CharaDamageCoolingTime { get { return 2f; } }
    ///敵：投げられる時の力
    public static float EnemyPowerThrowOut { get { return 300f; } }
    ///敵：攻撃時にジャンプする方向ベクトル高さオフセット
    public static float EnemyAttackOffesetY { get { return 0.3f; } }
	///敵：メインキャラを捕まった後、くっ付いとく最大距離
    public static float EnemyReleaseRange { get { return 2.5f; } }
}
