using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 鉱石の状態を表すスプライトINDEX
/// </summary>
public enum ORE_SPRS {
    DEF, // 全体鉱石
    HALF, // 半分残る 
    PIECE // 一部分しか残っていない
}

[System.Serializable]
public class OreData {
    [field:SerializeField] public string Name {get; private set;}
    [field:SerializeField] public int Lv {get; private set;}
    [field:SerializeField] public Sprite[] Sprs {get; private set;}
    [field:SerializeField] public int TimeSec {get; private set;}
    [field:SerializeField] public RewardDt[] Rewards;
}

[CreateAssetMenu(fileName = "OreDataSO", menuName = "Scriptable Object/Setting OreData")]
public class SettingOreData : ScriptableObject {
    [field:SerializeField] public OreData[] Datas {get; private set;}
}
