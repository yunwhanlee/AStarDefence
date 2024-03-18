using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class StageLockedData {
    [field:SerializeField] public string Name {get; private set;}
    [field:SerializeField] public bool Easy {get; set;}
    [field:SerializeField] public bool Normal {get; set;}
    [field:SerializeField] public bool Hard {get; set;}
}

[System.Serializable]
public class DataBase { //* Saveデータ
    //* ステージクリアデータ（EASY、NORMAL、HARD、NIGHT）
    [field:SerializeField] public StageLockedData[] StageLockedDatas {get; set;}
}
public class DM : MonoBehaviour {
    public static DM _ {get; private set;}
    //* Save Data
    [field: SerializeField] public DataBase DB {get; set;}

    //* Grobal Datas
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
