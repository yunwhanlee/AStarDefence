using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

[System.Serializable]
public class GoblinData {
    [field:SerializeField] public string Name {get; private set;}
    [field:SerializeField] public int Lv {get; private set;}
    [field:SerializeField] public float SpeedPer {get; private set;}
    [field:SerializeField] public SpriteLibraryAsset SprLibAst {get; private set;}
}

[CreateAssetMenu(fileName = "GoblinDataSO", menuName = "Scriptable Object/Setting GoblinData")]
public class SettingGoblinData : ScriptableObject {
    [field:SerializeField] public GoblinData[] Datas {get; private set;}
}
