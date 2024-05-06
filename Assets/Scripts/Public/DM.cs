using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.SceneManagement;
using System;
using Inventory.Model;
// using System.Data;

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
    [field:SerializeField] public float ClearCoinPer {get; set;} //
    [field:SerializeField] public float ClearExpPer {get; set;} //
    [field:SerializeField] public int StartMoney {get; set;} //
    [field:SerializeField] public int StartLife {get; set;} //
    [field:SerializeField] public float ItemDropPer {get; set;} //! 未
    // [field:SerializeField] public int SkillPoint {get; set;}
    [field:SerializeField] public int BonusCoinBy10Kill {get; set;} //
    [field:SerializeField] public float WarriorAttackPer {get; set;} //
    [field:SerializeField] public float ArcherAttackPer {get; set;} //
    [field:SerializeField] public float MagicianAttackPer {get; set;} //
    [field:SerializeField] public float WarriorUpgCostPer {get; set;} //
    [field:SerializeField] public float ArcherUpgCostPer {get; set;} //
    [field:SerializeField] public float MagicianUpgCostPer {get; set;} //
}

[Serializable]
public struct StageReward {
    [field:SerializeField] public bool IsUnlockAlert {get; set;}
    [field:SerializeField] public bool IsActiveBonusReward {get; set;}

    public StageReward(bool isUnlockAlert, bool isActiveBonusReward) {
        IsUnlockAlert = isUnlockAlert;
        IsActiveBonusReward = isActiveBonusReward;
    }
}

/// <summary>
/// ステージロック状態のデータ
/// </summary>
[Serializable]
public class StageLockedDB {
    [field:SerializeField] public string Name {get; private set;}
    [field:SerializeField] public bool IsUnlockAlert {get; set;}
    [field:SerializeField] public StageReward[] StageRewards {get; set;}
    [field:SerializeField] public bool IsLockStage1_1 {get; set;}
    [field:SerializeField] public bool IsLockStage1_2 {get; set;}
    [field:SerializeField] public bool IsLockStage1_3 {get; set;}
    
    public StageLockedDB(string name, bool islockStage1_1, bool islockStage1_2, bool islockStage1_3, StageReward[] stageRewards) {
        Name = name;
        IsLockStage1_1 = islockStage1_1;
        IsLockStage1_2 = islockStage1_2;
        IsLockStage1_3 = islockStage1_3;
        StageRewards = stageRewards;
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
        return !IsLockWarriorSTs[lvIdx]? DM._.WarriorSkillTreeDts[lvIdx].Val : 0;
    }
    public float GetArcherVal(int lvIdx) {
        return !IsLockArcherSTs[lvIdx]? DM._.ArcherSkillTreeDts[lvIdx].Val : 0;
    }
    public float GetMagicianVal(int lvIdx) {
        return !IsLockMagicianSTs[lvIdx]? DM._.MagicianSkillTreeDts[lvIdx].Val : 0;
    }
    public float GetUtilityVal(int lvIdx) {
        return !IsLockUtilitySTs[lvIdx]? DM._.UtilitySkillTreeDts[lvIdx].Val : 0;
    }
}

[Serializable]
public struct ShopDailyItem {
    [field:SerializeField] public bool IsAccept {get; set;}
}

/// <summary>
/// スキルツリー Lockリストデータ
/// </summary>
[Serializable]
public class ShopDB {
    public static readonly int FREE_COMMON = 0, DIAMOND_CHEST = 1, FREE_TINY = 2;
    [field:SerializeField] public bool[] IsPruchasedPackages {get; set;} = new bool[6];
    [field:SerializeField] public ShopDailyItem[] DailyItems {get; set;} = new ShopDailyItem[3];

    public ShopDB() {
        for(int i = 0; i < IsPruchasedPackages.Length; i++)
            IsPruchasedPackages[i] = false;
        
        //* 一日制限がある アイテム
        InitDaily(FREE_COMMON);
        InitDaily(DIAMOND_CHEST);
        InitDaily(FREE_TINY);
    }

    private void InitDaily(int idx) {
        DailyItems[idx].IsAccept = false;
    }

    public void SetAcceptData(int idx) {
        DailyItems[idx].IsAccept = true;
    }

    public bool TogglePassedDay(int dailyItemIdx) {
        return DailyItems[dailyItemIdx].IsAccept;
    }

    public void ResetDailyItemData() {
        for(int i = 0; i < DailyItems.Length; i++) {
            DailyItems[i].IsAccept = false;
        }
    }
}

/// <summary>
/// スキルツリー Lockリストデータ
/// </summary>
[Serializable]
public class DailyMissionDB {
    [field:SerializeField] public bool IsAcceptAllClearSpecialReward {get; set;}

    public const int CollectCoinMax = 5000;
    [field:SerializeField] public int CollectCoinVal {get; set;}
    [field:SerializeField] public bool IsAcceptCollectCoin {get; set;}

    public const int CollectDiamondMax = 100;
    [field:SerializeField] public int CollectDiamondVal {get; set;}
    [field:SerializeField] public bool IsAcceptCollectDiamond {get; set;}

    public const int MonsterKill = 500;
    [field:SerializeField] public int MonsterKillVal {get; set;}
    [field:SerializeField] public bool IsAcceptMonsterKill {get; set;}

    public const int BossKill = 10;
    [field:SerializeField] public int BossKillVal {get; set;}
    [field:SerializeField] public bool IsAcceptBossKill {get; set;}

    public const int ClearGoblinDungyen = 2;
    [field:SerializeField] public int ClearGoblinDungyenVal {get; set;}
    [field:SerializeField] public bool IsAcceptClearGoblinDungyen {get; set;}

    public const int ClearStage = 2;
    [field:SerializeField] public int ClearStageVal {get; set;}
    [field:SerializeField] public bool IsAcceptClearStage {get; set;}

    public const int ClearMining = 2;
    [field:SerializeField] public int ClearMiningVal {get; set;}
    [field:SerializeField] public bool IsAcceptClearMining {get; set;}

    public const int WatchAds = 2; //TODO
    [field:SerializeField] public int WatchAdsVal {get; set;}
    [field:SerializeField] public bool IsAcceptWatchAds {get; set;}

    public const int OpenAnyChest = 5;
    [field:SerializeField] public int OpenAnyChestVal {get; set;}
    [field:SerializeField] public bool IsAcceptOpenAnyChest {get; set;}

    public DailyMissionDB() {
        Reset();
    }

    public void Reset() {
        IsAcceptAllClearSpecialReward = false;
        CollectCoinVal = 0;
        IsAcceptCollectCoin = false;
        CollectDiamondVal = 0;
        IsAcceptCollectDiamond = false;
        MonsterKillVal = 0;
        IsAcceptMonsterKill = false;
        BossKillVal = 0;
        IsAcceptBossKill = false;
        ClearGoblinDungyenVal = 0;
        IsAcceptClearGoblinDungyen = false;
        ClearStageVal = 0;
        IsAcceptClearStage = false;
        ClearMiningVal = 0;
        IsAcceptClearMining = false;
        WatchAdsVal = 0;
        IsAcceptWatchAds = false;
        OpenAnyChestVal = 0;
        IsAcceptOpenAnyChest = false;
    }
}

/// <summary>
/// セッティングデータ
/// </summary>
[Serializable]
public class SettingDB {
    [field:SerializeField] public bool IsActiveBgm {get; set;}
    [field:SerializeField] public bool IsActiveSfx {get; set;}

    public SettingDB() {
        IsActiveBgm = true;
        IsActiveSfx = true;
    }
}

/// <summary>
/// チュートリアルデータ
/// </summary>
[Serializable]
public class TutorialDB {
    [field:SerializeField] public bool IsActiveGameStart {get; set;}
    [field:SerializeField] public bool IsActiveEnemyInfo {get; set;}
    [field:SerializeField] public bool IsActiveMiningInfo {get; set;}

    public TutorialDB() {
        IsActiveGameStart = true;
        IsActiveEnemyInfo = true;
        IsActiveMiningInfo = true;
    }
    public bool ActiveMiningInfoBubble() {
        return !IsActiveGameStart && !IsActiveEnemyInfo && IsActiveMiningInfo;
    }
    /// <summary>
    /// 「ゲーム開始」と「敵の情報」を見せるのが終わったら、次のホームに戻った時にMining情報を見せる
    /// </summary>
    public void CheckShowMiningInfo() {
        if(!IsActiveGameStart && !IsActiveEnemyInfo && IsActiveMiningInfo) {
            IsActiveMiningInfo = false;
            TutoM._.H_TutoMiningBubble.SetActive(false); //* 吹き出しOFF
            TutoM._.ShowTutoPopUp(TutoM.MINING_INFO, pageIdx: 0);

            //* 採掘を試すために、材料リワードをあげる
            TutoM._.OnClickCloseTutorial = () => {
                //* Goblin0とOre0リワード
                var rewardList = new List<RewardItem> {
                    new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin0]),
                    new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore0]),
                };
                HM._.rwlm.ShowReward(rewardList);
                HM._.rwm.UpdateInventory(rewardList);

                //* リワードデータのUI最新化
                HM._.mnm.SetUI((int)MineCate.Goblin);
                HM._.mnm.SetUI((int)MineCate.Ore);

                //* アクション 初期化
                TutoM._.OnClickCloseTutorial = null;
            };
        }
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
    [field:SerializeField] public ShopDB ShopDB {get; set;}
    [field:SerializeField] public DailyMissionDB DailyMissionDB {get; set;}
    [field:SerializeField] public SettingDB SettingDB {get; set;}
    [field:SerializeField] public TutorialDB TutorialDB {get; set;}
    
    [field:SerializeField] public List<InventoryItem> InvItemDBs {get; set;}
    [field:SerializeField] public bool IsRemoveAd {get; set;}
    [field:SerializeField] public bool IsCloverActive {get; set;}
    [field:SerializeField] public bool IsGoldCloverActive {get; set;}
    [field:SerializeField] public long LastDateTicks {get; set;}
    [field:SerializeField] public int LuckySpinFreeAdCnt {get; set;}
}

/// <summary>
///* データマネジャー
/// </summary>
public class DM : MonoBehaviour {
    public static DM _ {get; private set;}
    const string DB_KEY = "DB";
    const string PASSEDTIME_KEY = "PASSED_TIME";
    const string DAY_KEY = "DAY";
    [field: SerializeField] public bool IsReset {get; set;}
    [field: SerializeField] public bool IsInit {get; set;}

    //* スキルツリーデータ
    [field:Header("SKILL TREE DATA")]
    [field:SerializeField] public SkillTreeData[] WarriorSkillTreeDts {get; private set;}
    [field:SerializeField] public SkillTreeData[] ArcherSkillTreeDts {get; private set;}
    [field:SerializeField] public SkillTreeData[] MagicianSkillTreeDts {get; private set;}
    [field:SerializeField] public SkillTreeData[] UtilitySkillTreeDts {get; private set;}

    //* ★データベース
    [field: SerializeField] public DB DB {get; private set;}
    [field: SerializeField] public int PassedSec {get; set;}

    [field: SerializeField] public int SelectedStage {get; set;}
    [field: SerializeField] public Enum.StageNum SelectedStageNum {get; set;}
    [field: SerializeField] public bool IsPassedDate {get; private set;}

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
        
        //* 日にちが過ぎたら、DAILYデータをリセット
        if(CheckPassedDate()) {
            Debug.Log("IsPassedDate -> Reset Daily Data");
            DB.DailyMissionDB.Reset();
            DB.ShopDB.ResetDailyItemData();
            DB.LuckySpinFreeAdCnt = Config.LUCKYSPIN_FREEAD_CNT;
        }
    }

    void LateUpdate() {
        IsInit = true;
    }
/// -----------------------------------------------------------------------------------------------------------------
#region PASSED DATE TIME (DAILY RESET)
/// -----------------------------------------------------------------------------------------------------------------
    private bool CheckPassedDate() {
        DateTime lastDate = new DateTime(DB.LastDateTicks, DateTimeKind.Utc).Date;
        DateTime curDate = DateTime.UtcNow.Date;
        IsPassedDate = curDate > lastDate;
        Debug.Log($"CheckPassedDate():: IsPassedDate= {IsPassedDate} : curDate({curDate}) > LastDay({lastDate})");
        return IsPassedDate;
    }
#endregion
/// -----------------------------------------------------------------------------------------------------------------
#region QUIT APP EVENT
/// -----------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
    private void OnApplicationQuit() {
        Debug.Log("<color=yellow>QUIT APP(PC)::OnApplicationQuit():: SAVE</color>");
        DB.LastDateTicks = DateTime.UtcNow.Ticks; //* 終了した日にち時間データをTicks(longタイプ)で保存
        Save();
    }
#elif UNITY_ANDROID
    private void OnApplicationPause(bool paused){
        //* ゲームが開くとき（paused == true）にも起動されるので注意が必要。
        if(paused == true) {
            // Debug.Log("<color=yellow>QUIT APP(Mobile)::OnApplicationPause( "+paused+" ):: Scene= " + SceneManager.GetActiveScene().name);
            DB.LastDateTicks = DateTime.UtcNow.Ticks; //* 終了した日にち時間データをTicks(longタイプ)で保存
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
            ClearExpPer = 0,
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
        DB.StageLockedDBs = new StageLockedDB[6] {
            new ("스테이지1. 초원", false, true, true, new StageReward[] {new (false, false), new (false, false), new (false, false)}),
            new ("스테이지2. 황량한 사막", true, true, true, new StageReward[] {new (false, false), new (false, false), new (false, false)}),
            new ("스테이지3. 침묵의 바다", true, true, true, new StageReward[] {new (false, false), new (false, false), new (false, false)}),
            new ("스테이지4. 죽음의 던젼", true, true, true, new StageReward[] {new (false, false), new (false, false), new (false, false)}),
            new ("스테이지5. 불타는 지옥", true, true, true, new StageReward[] {new (false, false), new (false, false), new (false, false)}),
            new ("히든스테이지. 고블린 던전", false, false, false, new StageReward[] {new (false, false), new (false, false), new (false, false)}),
        };

        //* Mining
        DB.MiningDB = new MiningDB();
        DB.MiningDB.GoblinCardCnts = new int[7] {
            0, 0, 0, 0, 0, 0, 0
        };

        DB.MiningDB.OreCardCnts = new int[9] {
            0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        DB.MiningDB.WorkSpaces = new WorkSpace[5];
        for(int i = 0; i < DB.MiningDB.WorkSpaces.Length; i++) {
            DB.MiningDB.WorkSpaces[i] = new WorkSpace(i, i > 0, -1); // WorkSpace의 각 요소를 새로운 인스턴스로 만듭니다.
        }

        DB.SkillTreeDB = new SkillTreeDB();

        DB.ShopDB = new ShopDB();

        DB.DailyMissionDB = new DailyMissionDB();

        DB.SettingDB = new SettingDB();

        DB.TutorialDB = new TutorialDB();

        DB.IsRemoveAd = false;
        DB.IsCloverActive = false;
        DB.IsGoldCloverActive = false;
        DB.LuckySpinFreeAdCnt = 3;
    }
#endregion
}
