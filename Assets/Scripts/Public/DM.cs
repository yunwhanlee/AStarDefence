
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Inventory.Model;
using System.Text;
using System.Linq;
using System.IO;

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
    [field:SerializeField] public int Crack {get; set;}
    [field:SerializeField] public int Fame {get; set;}
    [field:SerializeField] public int Mileage {get; set;}

    public StatusDB() {
        Lv = 1;
        Exp = 1;
        GoldKey = 1;
        Coin = 0;
        Diamond = 0;
        SkillPoint = 0;
        Crack = 0;
        Fame = 0;
        Mileage = 0;
    }

    public float GetUserLvExtraDmgPercent()
        => Config.USER_LV_EXTRA_DMG_PER * (Lv - 1);
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
    [field:SerializeField] public bool IsHiddenDungeon {get; set;}
    [field:SerializeField] public bool IsUnlockAlert {get; set;}
    [field:SerializeField] public StageReward[] StageRewards {get; set;}
    [field:SerializeField] public bool IsLockStage1_1 {get; set;}
    [field:SerializeField] public bool IsLockStage1_2 {get; set;}
    [field:SerializeField] public bool IsLockStage1_3 {get; set;}
    
    public StageLockedDB(string name, bool isHiddenDungeon, bool islockStage1_1, bool islockStage1_2, bool islockStage1_3, StageReward[] stageRewards) {
        Name = name;
        IsHiddenDungeon = isHiddenDungeon;
        IsLockStage1_1 = islockStage1_1;
        IsLockStage1_2 = islockStage1_2;
        IsLockStage1_3 = islockStage1_3;
        StageRewards = stageRewards;
    }

    public bool CheckIsUnlockStage() {
        Debug.Log($"{this.Name} CheckIsUnlockStage():: lockStgs => {IsLockStage1_1}, {IsLockStage1_2}, {IsLockStage1_3}");
        if(IsLockStage1_1 == false) 
            return true;
        if(IsLockStage1_2 == false) 
            return true;
        if(IsLockStage1_3 == false) 
            return true;
        else 
            return false;
    }
}

[Serializable]
public class DungeonLockedDB {
    [field:SerializeField] public bool IsLockGoblinNormal {get; set;}
    [field:SerializeField] public bool IsLockGoblinHard {get; set;}
    [field:SerializeField] public bool IsLockInfinite {get; set;}

    public DungeonLockedDB() {
        IsLockGoblinNormal = false;
        IsLockGoblinHard = false;
        IsLockInfinite = false;
    }
}

/// <summary>
/// 採掘システムの保存データ
/// </summary>
[Serializable]
public class MiningDB {
    [field:SerializeField] public int[] GoblinCardCnts {get; set;} = new int[7];
    [field:SerializeField] public int[] OreCardCnts {get; set;} = new int[9];
    [field:SerializeField] public WorkSpace[] WorkSpaces {get; set;} = new WorkSpace[3];
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
    [field:SerializeField] public bool IsOnetimeFree {get; set;}
    [field:SerializeField] public bool IsAccept {get; set;}
}

/// <summary>
/// スキルツリー Lockリストデータ
/// </summary>
[Serializable]
public class ShopDB {
    public static readonly int FREE_COMMON = 0, 
        DIAMOND_CHEST = 1,
        FREE_TINYDIAMOND = 2,
        FREE_TINYCOIN = 3;

    [field:SerializeField] public bool[] IsPruchasedPackages {get; set;} = new bool[8];
    [field:SerializeField] public ShopDailyItem[] DailyItems {get; set;} = new ShopDailyItem[4];

    public ShopDB() {
        for(int i = 0; i < IsPruchasedPackages.Length; i++)
            IsPruchasedPackages[i] = false;
        
        //* 一日制限がある アイテム
        InitDaily(FREE_COMMON);
        InitDaily(DIAMOND_CHEST);
        InitDaily(FREE_TINYDIAMOND);
        InitDaily(FREE_TINYCOIN);
    }

    private void InitDaily(int idx) {
        DailyItems[idx].IsOnetimeFree = false;
        DailyItems[idx].IsAccept = false;
    }

    public void SetOneTimeFreeTriggerOn(int idx)
        => DailyItems[idx].IsOnetimeFree = true;
    public void SetAcceptTriggerOn(int idx)
        => DailyItems[idx].IsAccept = true;

    public bool TogglePassedDay(int dailyItemIdx) {
        return DailyItems[dailyItemIdx].IsAccept;
    }

    public void ResetDailyItemData() {
        for(int i = 0; i < DailyItems.Length; i++) {
            DailyItems[i].IsOnetimeFree = false;
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
    [field:SerializeField] public float BgmVolume {get; set;}
    [field:SerializeField] public float SfxVolume {get; set;}

    public SettingDB() {
        BgmVolume = 1;
        SfxVolume = 1;
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
    [field:SerializeField] public bool IsActiveConsumeBag {get; set;}

    public TutorialDB() {
        IsActiveGameStart = true;
        IsActiveEnemyInfo = true;
        IsActiveMiningInfo = true;
        IsActiveConsumeBag = true;
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
                    new (HM._.rwlm.RwdItemDt.WeaponDatas[0], 1),
                    new (HM._.rwlm.RwdItemDt.ShoesDatas[0], 1),
                    new (HM._.rwlm.RwdItemDt.RingDatas[1], 1),
                };
                HM._.rwlm.ShowRewardMiningTuto(rewardList);

                //* アクション 初期化
                TutoM._.OnClickCloseTutorial = null;
            };
        }
    }
}

/// <summary>
/// きれつダンジョンのアップグレード DB
/// </summary>
[Serializable]
public class InfiniteUpgradeDB {
    [field:SerializeField] public int MyBestInfiniteFloor {get; set;}
    [field:SerializeField] public int CurInfiniteFloor {get; set;}

    public const float DmgUpgUnit = 0.02f;
    [field:SerializeField] public int DmgUpgLv {get; set;}
    public const float CritDmgUpgUnit = 0.03f;
    [field:SerializeField] public int CritDmgUpgLv {get; set;}
    public const float BossDmgUpgUnit = 0.04f;
    [field:SerializeField] public int BossDmgUpgLv {get; set;}

    public InfiniteUpgradeDB() {
        MyBestInfiniteFloor = 0;
        DmgUpgLv = 0;
        CritDmgUpgLv = 0;
        BossDmgUpgLv = 0;
    }

    public float GetExtraDmgPercent() => DmgUpgLv * DmgUpgUnit; // -> Warrior, Archer, Magician Tower
    public float GetExtraCritDmgPercent() => CritDmgUpgLv * CritDmgUpgUnit; // -> Warrior, Archer, Magician Tower
    public float GetExtraBossDmgPercent() => BossDmgUpgLv * BossDmgUpgUnit; // -> Enemy

    public void UpdateBestScore(int curInfiniteFloor) {
        if(curInfiniteFloor > MyBestInfiniteFloor) {
            MyBestInfiniteFloor = curInfiniteFloor + 1;
            GM._.gui.ShowMsgNotice("축하합니다. 최고기록 달성!", 240);
            GPGSManager._.UpdateCrackDungeonBestWaveToLeaderBoard();
        }
    }
}

/// <summary>
/// マイレージリワードステータスDB
/// </summary>
[Serializable]
public class MileageRewardDB {
    const int MAXCNT = 16;
    [field:SerializeField] public RwdBubbleStatus[] Statuses {get; set;} = new RwdBubbleStatus[MAXCNT];

    public MileageRewardDB() {
        for(int i = 0; i < Statuses.Length; i++)
            Statuses[i] = RwdBubbleStatus.Locked;
    }
}

/// <summary>
/// 名声リワードステータスDB
/// </summary>
[Serializable]
public class FameRewardDB {
    const int MAXCNT = 50;
    [field:SerializeField] public RwdBubbleStatus[] Statuses {get; set;} = new RwdBubbleStatus[MAXCNT];

    public FameRewardDB() {
        for(int i = 0; i < Statuses.Length; i++)
            Statuses[i] = RwdBubbleStatus.Locked;
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
    [field:SerializeField] public DungeonLockedDB DungeonLockedDB {get; set;}
    [field:SerializeField] public MiningDB MiningDB {get; set;}
    [field:SerializeField] public SkillTreeDB SkillTreeDB {get; set;}
    [field:SerializeField] public ShopDB ShopDB {get; set;}
    [field:SerializeField] public DailyMissionDB DailyMissionDB {get; set;}
    [field:SerializeField] public SettingDB SettingDB {get; set;}
    [field:SerializeField] public TutorialDB TutorialDB {get; set;}
    [field:SerializeField] public InfiniteUpgradeDB InfiniteUpgradeDB {get; set;}
    [field:SerializeField] public MileageRewardDB MileageRewardDB {get; set;}
    [field:SerializeField] public FameRewardDB FameRewardDB {get; set;}
    [field:SerializeField] public TileMapSaveDt StageTileMapSaveDt {get; set;}
    [field:SerializeField] public TileMapSaveDt InfiniteTileMapSaveDt {get; set;}

    [field:SerializeField] public List<InventoryItem> InvItemDBList {get; set;}
    [field:SerializeField] public bool IsRemoveAd {get; set;}
    [field:SerializeField] public bool IsCloverActive {get; set;}
    [field:SerializeField] public bool IsGoldCloverActive {get; set;}
    [field:SerializeField] public bool IsThanksForPlaying {get; set;}
    [field:SerializeField] public long LastDateTicks {get; set;}
    [field:SerializeField] public int LuckySpinFreeAdCnt {get; set;}
    [field:SerializeField] public int GoldkeyFreeAdCnt {get; set;}
    [field:SerializeField] public int MiningFreeAdCnt {get; set;}    

    /// <summary>
    /// 最後にクリアーしたステージIdxを返す
    /// </summary>
    public int GetLatestUnlockStageIdx() {
        int stageIdx = 0;
        int lastIdx = StageLockedDBs.Length - 1;
        for(int i = lastIdx; i >= 0; i--) { // 逆順ループ
            if(StageLockedDBs[i].IsHiddenDungeon)
                continue;

            if(StageLockedDBs[i].CheckIsUnlockStage()) {
                stageIdx = i;
                break;
            }
        }

        Debug.Log($"GetLatestUnlockStageIdx():: stageIdx= {stageIdx}");
        return stageIdx;
    }
}

/// <summary>
///* データマネジャー
/// </summary>
public class DM : MonoBehaviour {
    public static DM _ {get; private set;}
    const string DB_KEY = "DB";
    const string INV_DATA_KEY = "INVENTORY";
    public string PASSEDTIME_KEY = "PASSED_TIME";
    const string DAY_KEY = "DAY";
    const int ALL_INV_CNT = 42;
    [field: SerializeField] public bool IsDebugMode {get; set;}

    //* スキルツリーデータ
    [field:Header("SKILL TREE DATA")]
    [field:SerializeField] public SkillTreeData[] WarriorSkillTreeDts {get; private set;}
    [field:SerializeField] public SkillTreeData[] ArcherSkillTreeDts {get; private set;}
    [field:SerializeField] public SkillTreeData[] MagicianSkillTreeDts {get; private set;}
    [field:SerializeField] public SkillTreeData[] UtilitySkillTreeDts {get; private set;}

    //* ★データベース
    [field: SerializeField] public DB DB {get; private set;}
    [field: SerializeField] public int PassedSec {get; set;}
    [field:SerializeField] public InventorySO TEST_InvSO {get; private set;} //! 55個ある 以前バージョンのインベントリー
    [field:SerializeField] public InventorySO InvSOTemplate {get; private set;}
    [field:SerializeField] public ItemSO[] ItemSOArr {get; private set;}
    [field:SerializeField] public InventoryItem[] FixedInvArr {get; private set;}

    [field: SerializeField] public int SelectedStage {get; set;}
    [field: SerializeField] public Enum.StageNum SelectedStageNum {get; set;}
    [field: SerializeField] public bool IsPassedDate {get; private set;}

    public string invDtBackUpFilePath;

    void Awake() {
        Application.targetFrameRate = 40;
        invDtBackUpFilePath = Application.persistentDataPath + "/InvItemDtBackUp.txt";

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
        if(!PlayerPrefs.HasKey(DB_KEY)) {
            DB = new DB();
            HM._.hui.ShowMsgNotice("AStar디펜스에 오신걸 환영합니다!!!");
            Reset();
        }
        else {
            Debug.Log($"Load To DB:: invDtBackUpFilePath= {invDtBackUpFilePath}");

            DB = Load();

            // DB.InvItemDBList = TEST_InvSO.InvArr.ToList(); //! 이전 55개 인벤토리 리스트 경우의 테스트
            // DB.InvItemDBList = null; //! 인벤토리 데이터가 NULL로 사라졌을 경우의 백업 복구테스트
            
            //* DB가 없는 경우
            if(DB == null) {
                Debug.Log("(ERR)로드된 DB데이터 KEY가 없음 -> 리셋 진행");
                if(HM._) HM._.hui.ShowMsgError("(ERR)로드된 DB데이터가 없음 -> 리셋 진행");
                Reset();
                return;
            }
            //* 인벤토리 데이터가 없거나 리스트가 0인 경우
            else if(DB.InvItemDBList == null || DB.InvItemDBList?.Count == 0) {
                Debug.Log("(ERR)인벤토리 데이터가 없거나 리스트가 0입니다.");
                if(HM._) HM._.hui.ShowMsgError("(ERR)인벤토리 데이터가 없거나 리스트가 0입니다.");

                //* 인벤토리 복구
                // 백업파일 있을 경우
                if(File.Exists(invDtBackUpFilePath)) {
                    string json = File.ReadAllText(invDtBackUpFilePath);

                    if(json == "") {
                        Debug.Log("(ERR)인벤토리 데이터 없음 -> 백업파일 데이터 없음('') -> 리셋");
                        // 인벤토리 리셋
                        DB.InvItemDBList = InvSOTemplate.InvArr.ToList();
                    }

                    InvItemBackUpDB invBackUpDB = JsonUtility.FromJson<InvItemBackUpDB>(json);

                    // 백업파일 옳바른 리스트인지 확인
                    if(invBackUpDB.InvArr.Length == ALL_INV_CNT) {
                        Debug.Log("(ERR)인벤토리 데이터 없음 -> 백업파일 복구 실행");
                        if(HM._) HM._.hui.ShowMsgError("(ERR)인벤토리 데이터 없음 -> 백업파일 복구 실행");
                        // 백업파일로 복구
                        DB.InvItemDBList = invBackUpDB.InvArr.ToList();
                        // 결과 팝업창 표시
                        if(HM._) HM._.hui.RecoverInvDataNoticePopUp.SetActive(true);
                        if(HM._) HM._.hui.RecoverInvDataMsgTxt.text = "인벤토리 데이터 백업 성공!";
                    }
                    else {
                        Debug.Log($"(ERR)인벤토리 데이터 없음 -> 백업파일 리스트 카운트 이상함({invBackUpDB.InvArr.Length}) -> 리셋");
                        if(HM._) HM._.hui.ShowMsgError($"(ERR)인벤토리 데이터 없음 -> 백업파일 리스트 카운트 이상함({invBackUpDB.InvArr.Length}) -> 리셋");
                        // 인벤토리 리셋
                        DB.InvItemDBList = InvSOTemplate.InvArr.ToList();
                    }
                }
                // 백업파일 없을 경우
                else {
                    Debug.Log("(ERR)인벤토리 데이터 없음 -> 백업파일 없음 -> 리셋");
                    if(HM._) HM._.hui.ShowMsgError("(ERR)인벤토리 데이터 없음 -> 백업파일 없음 -> 리셋");
                    // 인벤토리 리셋
                    DB.InvItemDBList = InvSOTemplate.InvArr.ToList();
                }
            }
            //* 위의 두가지 경우에 해당하지 않으면, 인벤토리 데이터 리스트 검사
            else {
                // 아이템 데이터 오류여부 검사
                bool isItemDataNull = DB.InvItemDBList.Exists(item => item.Data == null);
                bool isNotItemSOType = DB.InvItemDBList.Exists(item => item.Data && item.Data.GetType() != typeof(ItemSO));

                Debug.Log($"인벤토리 리스트 검사 -> 1.데이터 NULL= {isItemDataNull}, 2.데이터타입 다름= {isNotItemSOType}");

                // 아이템 데이터중에서 오류가 있는 경우
                if(isItemDataNull || isNotItemSOType) {
                    Debug.Log("인벤토리 리스트 중 오류가 있음 -> 옳바른 데이터만 추출 실행");
                    // 옳바른 아이템 데이터만 추출하여 받을 인벤토리 변수 선언
                    InventoryItem[] filterInvArr = Array.ConvertAll(InvSOTemplate.InvArr, item => item.DeepCopy());
                    
                    // 옯바른 인벤토리 아이템 데이터 추출
                    for(int i = 0; i < DB.InvItemDBList.Count; i++) {
                        var invItem = DB.InvItemDBList[i];
                        try{
                            var data = invItem.Data;

                            //* #1.옳바른 데이터인지 검사
                            // A.인벤토리 데이터가 없는 경우
                            if(invItem.Data == null) {
                                Debug.Log($"(제외A) invItem[{i}/{DB.InvItemDBList.Count}]: Item.Data가 NULL임");
                                continue;
                            }
                            // B.자료형 타입이 옮바르지 않은 경우
                            else if(invItem.Data.GetType() != typeof(ItemSO)) {
                                Debug.Log($"(제외B) invItem[{i}/{DB.InvItemDBList.Count}]: 자료형이 옮바른 ItemSO가 아님 : 데이터타입= {invItem.Data.GetType()}");
                                continue;
                            }
                            // C.아이템 수량이 없는 경우
                            else if(invItem.IsEmpty) {
                                Debug.Log($"(제외C) invItem[{i}/{DB.InvItemDBList.Count}]: IsEmpty 수량이 0임 : name= {invItem.Data.Name}");
                                continue;
                            }

                            //* #2.인벤토리 아이템인지 확인
                            if(invItem.Data.name.Contains($"{Enum.ItemType.Weapon}")
                            || invItem.Data.name.Contains($"{Enum.ItemType.Shoes}")
                            || invItem.Data.name.Contains($"{Enum.ItemType.Ring}")
                            || invItem.Data.name.Contains($"{Enum.ItemType.Relic}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.SteamPack0}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.SteamPack1}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.BizzardScroll}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.LightningScroll}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.ChestCommon}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.ChestDiamond}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.ChestEquipment}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.ChestGold}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.ChestPremium}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.Clover}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.GoldClover}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.Present0}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.Present1}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.Present2}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.SoulStone}")
                            || invItem.Data.name.Contains($"{Etc.ConsumableItem.MagicStone}")
                            ) {
                                // 유물이라면 잠재능력까지 설정되있는지 확인
                                if(invItem.Data.name.Contains($"{Enum.ItemType.Relic}")) {
                                    if(invItem.RelicAbilities.Count() == 0) {
                                        Debug.Log($"(제외D) invItem[{i}/{DB.InvItemDBList.Count}]: name= {invItem.Data.Name} -> 유물인데 잠재능력 리스트가 0임 : 잠재능력 리스트 카운트= {invItem.RelicAbilities.Count()}");
                                        continue;
                                    }
                                }

                                Debug.Log($"(추출) invItem[{i}/{DB.InvItemDBList.Count}]: type= {invItem.Data.Type}, name= {invItem.Data.Name}, qtt= {invItem.Quantity}");
                                InventoryItem prevInvItem = DB.InvItemDBList[i];
                                int id = prevInvItem.Data.ID;

                                // 현재 데이터에서 필요한 것만 추출하여 아이템 데이터 복구
                                filterInvArr[id].Data = prevInvItem.Data;
                                filterInvArr[id].Quantity = prevInvItem.Quantity;
                                filterInvArr[id].Lv = prevInvItem.Lv;
                                filterInvArr[id].RelicAbilities = prevInvItem.RelicAbilities;
                                filterInvArr[id].IsEquip = false;
                                filterInvArr[id].IsNewAlert = false;

                                // 추출된 아이템 목록 텍스트 추가
                                if(HM._) HM._.hui.RecoverInvDataMsgTxt.text +=  $"{prevInvItem.Data.Name} 인벤토리 데이터 복구\n";
                            }
                        }
                        catch(Exception e) {
                            Debug.LogError($"Exception occurred: {e.Message} -> invItem.Data= {(invItem.Data? invItem.Data : "NULL")}");
                        }
                    }

                    // 추출완료된 인벤토리 데이터를 지역변수 FixedInvArr에 대입 (InventorySO에서 데이터 로드시에 활용)
                    FixedInvArr = filterInvArr;

                    // 추출(복구)된 아이템 목록 팝업창 표시
                    if(HM._) HM._.hui.RecoverInvDataNoticePopUp.SetActive(true);
                }
            }
        }

        //* 日にちが過ぎたら、DAILYデータをリセット
        if(CheckPassedDate()) {
            Debug.Log("IsPassedDate -> Reset Daily Data");
            DB.DailyMissionDB.Reset();
            DB.ShopDB.ResetDailyItemData();
            DB.LuckySpinFreeAdCnt = Config.LUCKYSPIN_FREE_AD_CNT;
            DB.GoldkeyFreeAdCnt = Config.GOLDKEY_FREE_AD_CNT;
            DB.MiningFreeAdCnt = Config.MINING_FREE_AD_CNT;
        }

        //* アプリを最初起動したのみ、タイトル画面 表示
        if(HM._)
            HM._.hui.TitlePopUp.SetActive(true);
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
    public void OnApplicationQuit() {
        if(DB == null) return;

        Debug.Log("<color=yellow>QUIT APP(PC)::OnApplicationQuit():: SAVE</color>");
        DB.LastDateTicks = DateTime.UtcNow.Ticks; //* 終了した日にち時間データをTicks(longタイプ)で保存
        Save("DM");
    }
#elif UNITY_ANDROID
    public void OnApplicationPause(bool paused){
        if(DB == null) return;

        //* ゲームが開くとき（paused == true）にも起動されるので注意が必要。
        if(paused == true) {
            DB.LastDateTicks = DateTime.UtcNow.Ticks; //* 終了した日にち時間データをTicks(longタイプ)で保存
            Save();
        }
    }
#endif
#endregion
/// -----------------------------------------------------------------------------------------------------------------
#region SAVE
/// -----------------------------------------------------------------------------------------------------------------
    public void Save(string status) {
        if(DB == null) return;
    
        //* 현재시간을 저장 → DM.PassedSec
        //! 게임시작시에 이미 Save를 통해 현재시간을 최신화함으로, 홈으로 돌아왔을때 최신화하는 것은 제외
        if(status != "GoHome") {
            // 현재 시간을 UTC 기준으로 가져와서 1970년 1월 1일 0시 0분 0초와의 시간 차이를 구합니다.
            TimeSpan curTimeStamp = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0);
            // 시간 차이를 정수형으로 변환하여 PlayerPrefs에 저장합니다.
            PlayerPrefs.SetInt(PASSEDTIME_KEY, (int)curTimeStamp.TotalSeconds);
        }

        //* InventorySOデータ配列 → 保存するリストに変換して保存
        DB.InvItemDBList = HM._.ivCtrl.InventoryData.InvArr.ToList();

        //* BackUp Txt File
        Debug.Log($"path= {invDtBackUpFilePath}");

        //* BackUp用 インベントリーデータ 準備
        
        InvItemBackUpDB invItemBackUp = new InvItemBackUpDB();

        // インベントリー数が正しいであれば、JSONファイルを別に生成
        if(InvSOTemplate.InvArr.Length == ALL_INV_CNT) {
            invItemBackUp.InvArr = HM._.ivCtrl.InventoryData.InvArr;
            invItemBackUp.InvArrCnt = invItemBackUp.InvArr.Length;
            string invBackUpDtJson = JsonUtility.ToJson(invItemBackUp, true);
            File.WriteAllText(invDtBackUpFilePath, invBackUpDtJson);
        }

        //* Serialize To Json
        string json = JsonUtility.ToJson(DB, true);

        //* Save Data
        PlayerPrefs.SetString(DB_KEY, json);

        //* Print
        Debug.Log($"★SAVE({status}):: The Key: {DB_KEY} Exists? {PlayerPrefs.HasKey(DB_KEY)}, Data ={json}");
    }
#endregion
/// -----------------------------------------------------------------------------------------------------------------
#region LOAD
/// -----------------------------------------------------------------------------------------------------------------
    public DB Load() {
        //* 経過時間 ロード => HMから行うので、ここはコメント
        // Util.CalcPassedTimeSec();

        //* Prefabキーがない
        if(!PlayerPrefs.HasKey(DB_KEY)){
            return null;
        }

        //* Json 読み込み
        string json = PlayerPrefs.GetString(DB_KEY);

        //* Jsonデータがあない
        if (string.IsNullOrEmpty(json)) {
            return null;
        }

        //* 저장데이터 JSON 문자열의 메모리 크기 계산
        const int PLAYER_PREFS_MAX_MEMORY = 1024 * 1024;
        byte[] byteArray = Encoding.UTF8.GetBytes(json);
        int jsonSizeInBytes = byteArray.Length;
        Debug.Log($"★LOAD:: JSON MEMORY DATA= , {jsonSizeInBytes} / {PLAYER_PREFS_MAX_MEMORY} 바이트");

        //* Json クラス化
        Debug.Log($"★LOAD:: PlayerPrefs.GetString({DB_KEY}) -> {json}");
        DB db = JsonUtility.FromJson<DB>(json);
        return db;
    }
#endregion
/// -----------------------------------------------------------------------------------------------------------------
#region RESET
/// -----------------------------------------------------------------------------------------------------------------
    public void Reset() {
        // IsReset = true; //* リセットしたら、InventoryControllerのStart()からLoadDt()が呼び出して、InvItemDBsがNullになるエラー防止
        PlayerPrefs.DeleteAll();
        Debug.Log($"★RESET:: PlayerPrefs.DeleteAll():: PlayerPrefs.HasKey({DB_KEY}) -> {PlayerPrefs.HasKey(DB_KEY)}");
        ResetData();
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
    // public void LoadDt() {
    //     if(IsReset)
    //         IsReset = false;
    //     else 
    //         DB = Load();
    // }

    public void ResetData() {
        // 백업파일 초기화
        File.WriteAllText(invDtBackUpFilePath, "");

        DB = new DB();
        DB.InvItemDBList = new List<InventoryItem>();
        DB.InvItemDBList = InvSOTemplate.InvArr.ToList();
        Debug.Log("DB.InvItemDBs.Count= " + DB.InvItemDBList.Count);

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
            new ("스테이지1. 초원", isHiddenDungeon: false, false, true, true, new StageReward[] {new (false, false), new (false, false), new (false, false)}),
            new ("스테이지2. 황량한 사막", isHiddenDungeon: false, true, true, true, new StageReward[] {new (false, false), new (false, false), new (false, false)}),
            new ("스테이지3. 침묵의 바다", isHiddenDungeon: false, true, true, true, new StageReward[] {new (false, false), new (false, false), new (false, false)}),
            new ("스테이지4. 죽음의 던젼", isHiddenDungeon: false, true, true, true, new StageReward[] {new (false, false), new (false, false), new (false, false)}),
            new ("스테이지5. 불타는 지옥", isHiddenDungeon: false, true, true, true, new StageReward[] {new (false, false), new (false, false), new (false, false)}),
            new ("히든스테이지. 고블린 던전", isHiddenDungeon: true, false, false, false, new StageReward[] {new (false, false), new (false, false), new (false, false)}),
        };

        //* Dungeon
        DB.DungeonLockedDB = new DungeonLockedDB();

        //* Mining
        DB.MiningDB = new MiningDB();
        DB.MiningDB.GoblinCardCnts = new int[7] {
            0, 0, 0, 0, 0, 0, 0
        };

        DB.MiningDB.OreCardCnts = new int[9] {
            0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        DB.MiningDB.WorkSpaces = new WorkSpace[3];
        for(int i = 0; i < DB.MiningDB.WorkSpaces.Length; i++) {
            DB.MiningDB.WorkSpaces[i] = new WorkSpace(i, i > 0, -1); // WorkSpace의 각 요소를 새로운 인스턴스로 만듭니다.
        }

        DB.SkillTreeDB = new SkillTreeDB();

        DB.ShopDB = new ShopDB();

        DB.DailyMissionDB = new DailyMissionDB();

        DB.SettingDB = new SettingDB();

        DB.TutorialDB = new TutorialDB();
        
        DB.InfiniteUpgradeDB = new InfiniteUpgradeDB();

        DB.MileageRewardDB = new MileageRewardDB();

        DB.FameRewardDB = new FameRewardDB();

        DB.StageTileMapSaveDt = new TileMapSaveDt();
        DB.InfiniteTileMapSaveDt = new TileMapSaveDt();

        DB.IsRemoveAd = false;
        DB.IsCloverActive = false;
        DB.IsGoldCloverActive = false;
        DB.IsThanksForPlaying = false;
        DB.LuckySpinFreeAdCnt = Config.LUCKYSPIN_FREE_AD_CNT;
        DB.GoldkeyFreeAdCnt = Config.GOLDKEY_FREE_AD_CNT;
        DB.MiningFreeAdCnt = Config.MINING_FREE_AD_CNT;
    }
#endregion
}
