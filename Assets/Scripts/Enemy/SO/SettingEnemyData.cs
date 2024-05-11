using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

[System.Serializable]
public class EnemyData {
    [field:SerializeField] public string Name {get; set;}
    [field:SerializeField] public EnemyType Type {get; set;}
    [field:SerializeField] public Sprite Spr {get; set;}
    [field:SerializeField] public SpriteLibraryAsset SprLibAst {get; set;}
    [field:SerializeField] public int Lv {get; set;}
    [field:SerializeField] public int Hp {get; set;}
    [field:SerializeField] public float Speed {get; set;}
}

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "Scriptable Object/Setting EnemyData")]
public class SettingEnemyData : ScriptableObject {
    [field:Header("無限ダンジョン専用の登録する敵リスト")]
    [field:SerializeField] public SettingEnemyData[] ApplyInfiniteEnemyDts {get; set;}

    [field:SerializeField] public EnemyData[] Waves {get; set;}
    [field:SerializeField] public float HpRatio {get; private set;} = 1.0f;
}