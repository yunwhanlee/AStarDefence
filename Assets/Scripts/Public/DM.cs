using System.Collections;
using System.Collections.Generic;
// using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ステージロック状態のデータ
/// </summary>
[System.Serializable]
public class StageLockedData {
    [field:SerializeField] public string Name {get; private set;}
    [field:SerializeField] public bool Easy {get; set;}
    [field:SerializeField] public bool Normal {get; set;}
    [field:SerializeField] public bool Hard {get; set;}
}

/// <summary>
/// 保存・読込のデータベース
/// </summary>
[System.Serializable]
public class DataBase {
    [field:SerializeField] public StageLockedData[] StageLockedDatas {get; set;}
}

/// <summary>
/// データマネジャー
/// </summary>
public class DM : MonoBehaviour {
    public static DM _ {get; private set;}
    //* ★データベース
    [field: SerializeField] public DataBase DB {get; set;}

    //* Global Values
    [field: SerializeField] public int SelectedStage {get; set;}
    [field: SerializeField] public Enum.Difficulty SelectedDiff {get; set;}

    void Awake() {
#region SINGLETON
        if(_ == null) {
            _ = this;
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this);
            return;
        }
#endregion
    }
}
