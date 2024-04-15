using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Inventory.Model;

/// <summary>
/// ステータス
/// </summary>
[Serializable]
public class StatusDB {
    [field:SerializeField] public int Lv {get; set;}
    [field:SerializeField] public int Exp {get; set;}
    [field:SerializeField] public int GoldKey {get; set;}
    [field:SerializeField] public int Coin {get; set;}
    [field:SerializeField] public int Diamond {get; set;}
    [field:SerializeField] public int SkillPoint {get; set;}

    public StatusDB() {
        Lv = 1;
        Exp = 1;
        GoldKey = 1;
        Coin = 0;
        Diamond = 0; 
        SkillPoint = 0; 
    }
}

/// <summary>
/// 装置アイテムのデータ
/// </summary>
[Serializable]
public class EquipDB {
    [field:SerializeField] public float AttackPer {get; set;} //
    [field:SerializeField] public float SpeedPer {get; set;} //
    [field:SerializeField] public float RangePer {get; set;} //
    [field:SerializeField] public float CritPer {get; set;} //
    [field:SerializeField] public float CritDmgPer {get; set;} //
    [field:SerializeField] public float ClearEtcPer {get; set;}
    [field:SerializeField] public float ClearCoinPer {get; set;}
    [field:SerializeField] public int StartMoney {get; set;} //
    [field:SerializeField] public int StartLife {get; set;} //
    [field:SerializeField] public float ItemDropPer {get; set;}
    // [field:SerializeField] public int SkillPoint {get; set;}
    [field:SerializeField] public int BonusCoinBy10Kill {get; set;} //
    [field:SerializeField] public float WarriorAttackPer {get; set;} //
    [field:SerializeField] public float ArcherAttackPer {get; set;} //
    [field:SerializeField] public float MagicianAttackPer {get; set;} //
    [field:SerializeField] public float WarriorUpgCostPer {get; set;} //
    [field:SerializeField] public float ArcherUpgCostPer {get; set;} //
    [field:SerializeField] public float MagicianUpgCostPer {get; set;} //
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
    [field:SerializeField] public EquipDB EquipDB {get; set;}
    [field:SerializeField] public StageLockedDB[] StageLockedDBs {get; set;}
    [field:SerializeField] public MiningDB MiningDB {get; set;}
    [field:SerializeField] public SkillTreeDB SkillTreeDB {get; set;}
    [field:SerializeField] public List<InventoryItem> InvItemDBs {get; set;}
    [field:SerializeField] public bool IsCloverActive {get; set;}
    [field:SerializeField] public bool IsGoldCloverActive {get; set;}
}

/// <summary>
///* データマネジャー
/// </summary>
public class DM : MonoBehaviour {
    public static DM _ {get; private set;}
    const string DB_KEY = "DB";
    const string PASSEDTIME_KEY = "PASSED_TIME";
    [field: SerializeField] public bool IsReset {get; set;}
    [field: SerializeField] public bool IsInit {get; set;}

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
            DontDestroyOnLoad(_);
        }
        else {
            Destroy(gameObject);
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

    void LateUpdate() {
        IsInit = true;
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

        //* 空に初期化してから、InventorySOデータを上書き
        DB.InvItemDBs = new List<InventoryItem>();
        for(int i = 0; i < HM._.ivCtrl.InventoryData.ItemList.Count; i++) {
            var invItem = HM._.ivCtrl.InventoryData.ItemList[i];
            DB.InvItemDBs.Add(InventoryItem.GetEmptyItem());
            DB.InvItemDBs[i] = DB.InvItemDBs[i].ChangeQuantity(invItem.Quantity);
            DB.InvItemDBs[i] = DB.InvItemDBs[i].ChangeLevel(invItem.Lv);
            DB.InvItemDBs[i] = DB.InvItemDBs[i].ChangeItemData(invItem.Data);
            DB.InvItemDBs[i] = DB.InvItemDBs[i].ChangeItemRelicAbilities(invItem.RelicAbilities);
            DB.InvItemDBs[i] = DB.InvItemDBs[i].ChangeIsEquip(invItem.IsEquip);
        }

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
    public void Reset() {
        Debug.Log($"★RESET:: The Key: {DB_KEY} Exists? {PlayerPrefs.HasKey(DB_KEY)}");
        IsReset = true; //* リセットしたら、InventoryControllerのStart()からLoadDt()が呼び出して、InvItemDBsがNullになるエラー防止
        PlayerPrefs.DeleteAll();
        Init();
    }
#endregion
/// -----------------------------------------------------------------------------------------------------------------
#region FUNC
/// -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// アプリ起動：データロード
    /// アプリ終了：データセーブ
    /// ホーム ➡ ゲーム：データセーブ
    /// ホームシーン：データロード (※リセットかけたら、これスキップ：InvItemDBsがNULLになるエラーあるため)
    /// </summary>
    public void LoadDt() {
        if(IsReset)
            IsReset = false;
        else 
            DB = Load();
    }

    public void Init() {
        DB = new DB();
        
        DB.InvItemDBs = new List<InventoryItem>();
        for(int i = 0; i < InventorySO.Size; i++) {
            DB.InvItemDBs.Add(InventoryItem.GetEmptyItem());
        }
        Debug.Log("DB.InvItemDBs.Count= " + DB.InvItemDBs.Count);

        DB.StatusDB = new StatusDB();

        DB.EquipDB = new EquipDB() {
            AttackPer = 0,
            SpeedPer = 0,
            RangePer = 0,
            CritPer = 0,
            CritDmgPer = 0,
            ClearEtcPer = 0,
            ClearCoinPer = 0,
            StartMoney = 0,
            StartLife = 0,
            ItemDropPer = 0,
            BonusCoinBy10Kill = 0,
            WarriorAttackPer = 0,
            ArcherAttackPer = 0,
            MagicianAttackPer = 0,
            WarriorUpgCostPer = 0,
            ArcherUpgCostPer = 0,
            MagicianUpgCostPer = 0,
            // SkillPoint = 0
        };

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

        DB.IsCloverActive = false;
        DB.IsGoldCloverActive = false;
    }
#endregion
}
