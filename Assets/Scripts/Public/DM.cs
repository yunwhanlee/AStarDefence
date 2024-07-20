
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
    [field:SerializeField] public int MyBestWaveScore {get; set;}
    public const float DmgUpgUnit = 0.02f;
    [field:SerializeField] public int DmgUpgLv {get; set;}
    public const float CritDmgUpgUnit = 0.03f;
    [field:SerializeField] public int CritDmgUpgLv {get; set;}
    public const float BossDmgUpgUnit = 0.04f;
    [field:SerializeField] public int BossDmgUpgLv {get; set;}

    public InfiniteUpgradeDB() {
        MyBestWaveScore = 0;
        DmgUpgLv = 0;
        CritDmgUpgLv = 0;
        BossDmgUpgLv = 0;
    }

    public float GetExtraDmgPercent() => DmgUpgLv * DmgUpgUnit; // -> Warrior, Archer, Magician Tower
    public float GetExtraCritDmgPercent() => CritDmgUpgLv * CritDmgUpgUnit; // -> Warrior, Archer, Magician Tower
    public float GetExtraBossDmgPercent() => BossDmgUpgLv * BossDmgUpgUnit; // -> Enemy
    public void UpdateBestScore(int waveCnt) {
        if(waveCnt > MyBestWaveScore) {
            MyBestWaveScore = waveCnt;
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

    //! REWARD COUPON DELETE AT 7/4
    [field:HideInInspector] public string COUPON_A = "dew13GewfDW";
    [field:SerializeField] public bool IsAccept_CouponA {get; set;}

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
    const string PASSEDTIME_KEY = "PASSED_TIME";
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
        invDtBackUpFilePath = Application.persistentDataPath + "/InvItemDtBackUp";

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
            DB = Load();

            if(DB == null) {
                if(HM._) HM._.hui.ShowMsgError("로드된 DB데이터 KEY가 없습니다. -> 인벤토리 리셋 진행");
                return;
            }

            // DB.InvItemDBList = TEST_InvSO.InvArr.ToList(); //! 이전 55개 인벤토리 리스트 경우의 테스트
            // DB.InvItemDBList = null; //! 인벤토리 데이터가 NULL로 사라졌을 경우의 백업 복구테스트

            //* データがなかったら
            if(DB.InvItemDBList == null || DB.InvItemDBList?.Count == 0) {
                Debug.Log("<color=red>ロードしたInvListデータがNULLとかカウントが０です</color>");

                //* BACK-UPインベントリーファイルで復旧
                if(File.Exists(invDtBackUpFilePath)) {
                    string json = File.ReadAllText(invDtBackUpFilePath);
                    InvItemBackUpDB invItemBackUpDB = JsonUtility.FromJson<InvItemBackUpDB>(json);
                    if(HM._ && invItemBackUpDB.InvArr.Length == ALL_INV_CNT) {
                        HM._.hui.ShowMsgError("(에러) 인벤토리 데이터 없음 -> 백업파일 복구 실행");
                        HM._.hui.RecoverInvDataNoticePopUp.SetActive(true);
                        DB.InvItemDBList = invItemBackUpDB.InvArr.ToList();
                        if(HM._) HM._.hui.RecoverInvDataMsgTxt.text = "인벤토리 데이터 백업 성공!";
                    }
                }
                //* BACK-UPファイルないから、リセット
                else {
                    if(HM._) HM._.hui.ShowMsgError("(에러) 인벤토리 데이터 없음 -> 백업파일 없음으로 복구 불가");
                    if(HM._) HM._.hui.RecoverInvDataMsgTxt.text = "인벤토리 데이터가 없음으로 복구가 불가능하여 리셋을 진행합니다.\n기존에 가지고 계셨던 아이템과 목록을 아래 이메일로 남겨주시면 복구 및 사과보상을 지급하겠습니다. 감사합니다.";
                    // インベントリーリセット
                    DB.InvItemDBList = InvSOTemplate.InvArr.ToList();
                    if(HM._) HM._.hui.ShowMsgNotice("인벤토리 리셋 완료");
                }
                

            }
            //* インベントリーアイテム NULLデータ
            else if(DB.InvItemDBList.Exists(item => item.Data == null)) {
                int RIGHT_INVARR_LEN = InvSOTemplate.InvArr.Length;
                bool isRightInvItemCnt = DB.InvItemDBList.Count == RIGHT_INVARR_LEN; // インベントリー数が４２なら

                // 1. インベントリー数は合うのに、データのみ消えたとき、データを再入れる
                if(isRightInvItemCnt) {
                    if(HM._) HM._.hui.ShowMsgError($"(에러) 아이템 NULL발견 -> 인벤토리 수: {DB.InvItemDBList.Count} -> 데이터 재입력");
                    for(int i = 0; i < DB.InvItemDBList.Count; i++) {
                        InventoryItem tempInvItem = DB.InvItemDBList[i];
                        tempInvItem.Data = ItemSOArr[i];
                        DB.InvItemDBList[i] = tempInvItem;
                    }
                }
                // 2. 以前インベントリーリストデータを新しいInvArrとして、アップロード
                else {
                    if(HM._) HM._.hui.ShowMsgError($"(에러) 아이템 NULL발견 -> 인벤토리 수: {DB.InvItemDBList.Count} -> 이전데이터 복구 실행");
                    // テンプレートInvArrコピーして、新しいインベントリー配列生成
                    InventoryItem[] newInvArr = Array.ConvertAll(InvSOTemplate.InvArr, item => item.DeepCopy());

                    // ログ
                    for(int i = 0; i < newInvArr.Length; i++) {
                        Debug.Log($"Fix Load Data:: i({i}): Name= {newInvArr[i].Data.Name}, Data= {newInvArr[i].Data}");
                    }

                    // ShowInvItemのみ検査して、新しいInvArrへ反映
                    for(int i = 0; i < DB.InvItemDBList.Count; i++) {
                        InventoryItem befInvItem = DB.InvItemDBList[i];

                        // Quantityが０なら除外
                        if(befInvItem.IsEmpty)
                            continue;
                        
                        // データがなかったら除外
                        if(befInvItem.Data == null)
                            continue;

                        // NoShowアイテムデータ 除外
                        if(befInvItem.Data.name.Contains($"{Etc.NoshowInvItem.Goblin}")
                        || befInvItem.Data.name.Contains($"{Etc.NoshowInvItem.Ore}")
                        || befInvItem.Data.name.Contains($"{Etc.NoshowInvItem.GoldKey}")
                        || befInvItem.Data.name.Contains($"{Etc.NoshowInvItem.Coin}")
                        || befInvItem.Data.name.Contains($"{Etc.NoshowInvItem.Diamond}")
                        || befInvItem.Data.name.Contains($"{Etc.NoshowInvItem.Crack}")
                        || befInvItem.Data.name.Contains($"{Etc.NoshowInvItem.SkillPoint}")
                        || befInvItem.Data.name.Contains($"{Etc.NoshowInvItem.RemoveAd}")
                        || befInvItem.Data.name.Contains($"{Etc.NoshowInvItem.Fame}")
                        || befInvItem.Data.name.Contains($"{Etc.NoshowInvItem.NULL}"))
                            continue;

                        // アイテム データ 追加
                        Debug.Log($"Fix Load Data:: {befInvItem.Data.Name} 인벤토리 데이터 복구");
                        int id = befInvItem.Data.ID;
                        newInvArr[id].Data = befInvItem.Data;
                        newInvArr[id].Quantity = befInvItem.Quantity;
                        newInvArr[id].Lv = befInvItem.Lv;
                        newInvArr[id].RelicAbilities = befInvItem.RelicAbilities;
                        newInvArr[id].IsEquip = false;
                        newInvArr[id].IsNewAlert = false;

                        if(HM._) HM._.hui.RecoverInvDataMsgTxt.text +=  $"{befInvItem.Data.Name} 인벤토리 데이터 복구\n";
                    }
                    FixedInvArr = newInvArr;

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
        Save();
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
    public void Save() {
        if(DB == null) return;

        //* 経過時間 保存
        // 현재 시간을 UTC 기준으로 가져와서 1970년 1월 1일 0시 0분 0초와의 시간 차이를 구합니다.
        TimeSpan timestamp = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0);
        // 시간 차이를 정수형으로 변환하여 PlayerPrefs에 저장합니다.
        PlayerPrefs.SetInt(PASSEDTIME_KEY, (int)timestamp.TotalSeconds);

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
        Debug.Log($"★SAVE:: The Key: {DB_KEY} Exists? {PlayerPrefs.HasKey(DB_KEY)}, Data ={json}");
    }
#endregion
/// -----------------------------------------------------------------------------------------------------------------
#region LOAD
/// -----------------------------------------------------------------------------------------------------------------
    public DB 
    Load() {
        //* 経過時間 ロード
        // 현재 시간을 UTC 기준으로 가져와서 1970년 1월 1일 0시 0분 0초와의 시간 차이를 구합니다.
        TimeSpan timestamp = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0);
        // 앱을 최초로 시작했을 때와의 시간 차이를 계산하여 PassedSec에 저장합니다.
        int past = PlayerPrefs.GetInt(PASSEDTIME_KEY, defaultValue: (int)timestamp.TotalSeconds);
        PassedSec = (int)timestamp.TotalSeconds - past;

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
