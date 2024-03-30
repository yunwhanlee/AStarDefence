using System.Collections;
using System.Collections.Generic;
// using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// ステータス
/// </summary>
[Serializable]
public class StatusDB {
    [field:SerializeField] public int Lv {get; set;}
    [field:SerializeField] public int Exp {get; set;}
    [field:SerializeField] public int GoblinKey {get; set;}
    [field:SerializeField] public int Coin {get; set;}
    [field:SerializeField] public int Diamond {get; set;}
    [field:SerializeField] public int SkillPoint {get; set;}

    public StatusDB() {
        Lv = 1;
        Exp = 8;
        GoblinKey = 1;
        Coin = 0;
        Diamond = 0; 
        SkillPoint = 0; 
    }
}

/// <summary>
/// ステージロック状態のデータ
/// </summary>
[Serializable]
public class StageLockedDB {
    [field:SerializeField] public string Name {get; private set;}
    [field:SerializeField] public bool IsLockEasy {get; set;}
    [field:SerializeField] public bool IsLockNormal {get; set;}
    [field:SerializeField] public bool IsLockHard {get; set;}
    
    public StageLockedDB(string name, bool isLockEasy) {
        Name = name;
        IsLockEasy = isLockEasy;
        IsLockNormal = false;
        IsLockHard = false;
    }
}

/// <summary>
/// 採掘システムの保存データ
/// </summary>
[Serializable]
public class MiningDB {
    [field:SerializeField] public int[] GoblinCardCnts {get; set;} = new int[7];
    [field:SerializeField] public int[] OreCardCnts {get; set;} = new int[9];
    [field:SerializeField] public WorkSpace[] WorkSpaces {get; set;} = new WorkSpace[5];
}

/// <summary>
/// スキルツリー Lockリストデータ
/// </summary>
[Serializable]
public class SkillTreeDB {
    [field:SerializeField] public bool[] IsLockWarriorSTs {get; set;} = new bool[5];
    [field:SerializeField] public bool[] IsLockArcherSTs {get; set;} = new bool[5];
    [field:SerializeField] public bool[] IsLockMagicianSTs {get; set;} = new bool[5];
    [field:SerializeField] public bool[] IsLockUtilitySTs {get; set;} = new bool[5];

    public SkillTreeDB() {
        for(int i = 0; i < IsLockWarriorSTs.Length; i++)
            IsLockWarriorSTs[i] = true;
        for(int i = 0; i < IsLockArcherSTs.Length; i++)
            IsLockArcherSTs[i] = true;
        for(int i = 0; i < IsLockMagicianSTs.Length; i++)
            IsLockMagicianSTs[i] = true;
        for(int i = 0; i < IsLockUtilitySTs.Length; i++)
            IsLockUtilitySTs[i] = true;
    }

    /// <summary>
    /// アンロックしたスキルツリーの効果を返す(float返しですが、場合によってIntに変換する必要がある)
    /// </summary>
    /// <param name="lvIdx">レベルIndex</param>
    public float GetWarriorVal(int lvIdx) {
        return !IsLockWarriorSTs[lvIdx]? DM._.WarriorSkillTreeSO.Datas[lvIdx].Val : 0;
    }
    public float GetArcherVal(int lvIdx) {
        return !IsLockArcherSTs[lvIdx]? DM._.ArcherSkillTreeSO.Datas[lvIdx].Val : 0;
    }
    public float GetMagicianVal(int lvIdx) {
        return !IsLockMagicianSTs[lvIdx]? DM._.MagicianSkillTreeSO.Datas[lvIdx].Val : 0;
    }
    public float GetUtilityVal(int lvIdx) {
        return !IsLockUtilitySTs[lvIdx]? DM._.UtilitySkillTreeSO.Datas[lvIdx].Val : 0;
    }
}

/// <summary>
///* 保存・読込のデータベース ★データはPlayerPrefsに保存するので、String、Float、Intのみ！！！★
/// </summary>
[Serializable]
public class DB {
    [field:SerializeField] public StatusDB StatusDB {get; set;}
    [field:SerializeField] public StageLockedDB[] StageLockedDBs {get; set;}
    [field:SerializeField] public MiningDB MiningDB {get; set;}
    [field:SerializeField] public SkillTreeDB SkillTreeDB {get; set;}
}

/// <summary>
///* データマネジャー
/// </summary>
public class DM : MonoBehaviour {
    public static DM _ {get; private set;}
    const string DB_KEY = "DB";
    const string PASSEDTIME_KEY = "PASSED_TIME";
    [field: SerializeField] public bool IsReset {get; set;}

    //* スキルツリーデータ
    [field:Header("SKILL TREE DATA")]
    [field:SerializeField] public SettingSkillTreeData WarriorSkillTreeSO {get; private set;}
    [field:SerializeField] public SettingSkillTreeData ArcherSkillTreeSO {get; private set;}
    [field:SerializeField] public SettingSkillTreeData MagicianSkillTreeSO {get; private set;}
    [field:SerializeField] public SettingSkillTreeData UtilitySkillTreeSO {get; private set;}

    //* ★データベース
    [field: SerializeField] public DB DB {get; private set;}
    [field: SerializeField] public int PassedSec {get; set;}

    //* Global Values
    [field: SerializeField] public int SelectedStage {get; set;}
    [field: SerializeField] public Enum.Difficulty SelectedDiff {get; set;}

    void Awake() {
        //* SINGLETON
        if(_ == null) {
            _ = this;
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this);
            return;
        }

        //* アプリを初めてスタートした場合のみ、一回
        if(Load() == null) {
            DB = new DB();
            HM._.hui.ShowMsgNotice("AStar디펜스에 오신걸 환영합니다!");
            Reset();
        }
        else
            DB = Load();
    }

/// -----------------------------------------------------------------------------------------------------------------
#region QUIT APP EVENT
/// -----------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
    void OnApplicationQuit() {
        Debug.Log("<color=yellow>QUIT APP(PC)::OnApplicationQuit():: SAVE</color>");
        Save();
    }
#elif UNITY_ANDROID
    void OnApplicationPause(bool paused){
        //* ゲームが開くとき（paused == true）にも起動されるので注意が必要。
        if(paused == true) {
            Debug.Log("<color=yellow>QUIT APP(Mobile)::OnApplicationPause( "+paused+" ):: Scene= " + SceneManager.GetActiveScene().name);
            Save();
        }
    }
#endif
#endregion
/// -----------------------------------------------------------------------------------------------------------------
#region SAVE
/// -----------------------------------------------------------------------------------------------------------------
    public void Save() {
        //* 経過時間 保存
        // 현재 시간을 UTC 기준으로 가져와서 1970년 1월 1일 0시 0분 0초와의 시간 차이를 구합니다.
        TimeSpan timestamp = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0);
        // 시간 차이를 정수형으로 변환하여 PlayerPrefs에 저장합니다.
        PlayerPrefs.SetInt(PASSEDTIME_KEY, (int)timestamp.TotalSeconds);

        //* Serialize To Json
        PlayerPrefs.SetString(DB_KEY, JsonUtility.ToJson(DB, true)); 
        //* Print
        string json = PlayerPrefs.GetString(DB_KEY);
        Debug.Log($"★SAVE:: The Key: {DB_KEY} Exists? {PlayerPrefs.HasKey(DB_KEY)}, Data ={json}");
    }
#endregion
/// -----------------------------------------------------------------------------------------------------------------
#region LOAD
/// -----------------------------------------------------------------------------------------------------------------
    public DB Load() {
        //* 経過時間 ロード
        // 현재 시간을 UTC 기준으로 가져와서 1970년 1월 1일 0시 0분 0초와의 시간 차이를 구합니다.
        TimeSpan timestamp = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0);
        // 앱을 최초로 시작했을 때와의 시간 차이를 계산하여 PassedSec에 저장합니다.
        int past = PlayerPrefs.GetInt(PASSEDTIME_KEY, defaultValue: (int)timestamp.TotalSeconds);
        PassedSec = (int)timestamp.TotalSeconds - past;

        //* (BUG)最初の実行だったら、ロードデータがないから、リセットして初期化。
        if(!PlayerPrefs.HasKey(DB_KEY)){
            return null;
        }
        //* Json 読み込み
        string json = PlayerPrefs.GetString(DB_KEY);
        Debug.Log($"★LOAD:: (json == null)? {json == null}, \ndata= {json}");
        //* Json クラス化
        DB db = JsonUtility.FromJson<DB>(json);
        return db;
    }
#endregion
/// -----------------------------------------------------------------------------------------------------------------
#region RESET
/// -----------------------------------------------------------------------------------------------------------------
    public void OnClickResetBtn() {
        Reset();
        SceneManager.LoadScene($"{Enum.Scene.Home}");
    }
    private void Reset() {
        Debug.Log($"★RESET:: The Key: {DB_KEY} Exists? {PlayerPrefs.HasKey(DB_KEY)}");
        PlayerPrefs.DeleteAll();
        Init();
    }
#endregion
/// -----------------------------------------------------------------------------------------------------------------
#region FUNC
/// -----------------------------------------------------------------------------------------------------------------
    public void Init() {
        DB = new DB();

        DB.StatusDB = new StatusDB();

        //* Stages
        DB.StageLockedDBs = new StageLockedDB[5] {
            new ("스테이지1. 초원", false),
            new ("스테이지2. 황량한 사막", true),
            new ("스테이지3. 침묵의 바다", true),
            new ("스테이지4. 죽음의 던젼", true),
            new ("스테이지5. 불타는 지옥", true),
        };

        //* Mining
        DB.MiningDB = new MiningDB();
        DB.MiningDB.GoblinCardCnts = new int[7] {
            5, 4, 3, 2, 1, 0, 0
        };

        DB.MiningDB.OreCardCnts = new int[9] {
            5, 4, 3, 2, 4, 2, 0, 0, 0
        };

        DB.MiningDB.WorkSpaces = new WorkSpace[5];
        for(int i = 0; i < DB.MiningDB.WorkSpaces.Length; i++) {
            DB.MiningDB.WorkSpaces[i] = new WorkSpace(i, i > 0, -1); // WorkSpace의 각 요소를 새로운 인스턴스로 만듭니다.
        }

        DB.SkillTreeDB = new SkillTreeDB();
    }
#endregion
}
