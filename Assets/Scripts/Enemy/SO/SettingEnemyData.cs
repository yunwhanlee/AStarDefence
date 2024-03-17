using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData {
    [field:SerializeField] public string Name {get; private set;}
    [field:SerializeField] public EnemyType Type {get; private set;}
    [field:SerializeField] public Sprite Spr {get; private set;}
    [field:SerializeField] public int Lv {get; set;}
    [field:SerializeField] public int Hp {get; private set;}
    [field:SerializeField] public float Speed {get; private set;}
}

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "Scriptable Object/Setting EnemyData")]
public class SettingEnemyData : ScriptableObject {
    [field:SerializeField] public int WaveCount {get; private set;}
    [field:SerializeField] public EnemyData[] Waves {get; private set;}
}
