using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerDataSO", menuName = "Scriptable Object/Setting TowerData")]
public class SettingTowerData : ScriptableObject {
    public TowerType Type;
    public string Name;
    public int Lv;
    public int Dmg;
    public int AtkRange;
    public float AtkSpeed;
    public int SplashRange;
    [Range(0.00f, 1.00f)] public float CritPer;
    public float CritDmgPer;
    //* CC
    [Range(0.00f,1.00f)] public float SlowPer;
    [Range(0.0f, 5.0f)] public int StunSec;
}
