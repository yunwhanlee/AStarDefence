using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OreData {
    [field:SerializeField] public string Name {get; private set;}
    [field:SerializeField] public int Lv {get; private set;}
    [field:SerializeField] public Sprite[] Spr {get; private set;}
    [field:SerializeField] public int TimeSec {get; private set;}
    [field:SerializeField] public Reward[] Rewards;
}

[CreateAssetMenu(fileName = "OreDataSO", menuName = "Scriptable Object/Setting OreData")]
public class SettingOreData : ScriptableObject {
    [field:SerializeField] public OreData[] OreDatas {get; private set;}
}
