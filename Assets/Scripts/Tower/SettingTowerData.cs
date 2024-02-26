using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerDataSO", menuName = "Scriptable Object/Setting TowerData")]
public class SettingTowerData : ScriptableObject {
    public TowerType Type;
    public TowerKind Kind;
    public string Name;
    [Range(1, 7)] public int Lv;
    public int Dmg;
    public float AtkSpeed;
    [Range(0, 10)] public float AtkRange;
    [Range(0, 10)] public float SplashRange;
    [Range(0.00f, 1.00f)] public float CritPer;
    public float CritDmgPer;
    //* CC
    [Range(0.00f,1.00f)] public float SlowPer;
    [Range(0.0f, 5.0f)] public float StunSec;
}