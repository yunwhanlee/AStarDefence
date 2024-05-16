using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using Inventory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HM : MonoBehaviour {
    public static HM _; //* Global

    //* Outside
    [HideInInspector] public HomeUIManager hui;
    [HideInInspector] public StageUIManager stgm;
    [HideInInspector] public HomeRewardUIManager rwm;
    [HideInInspector] public HomeRewardListUIManager rwlm;
    // Mining
    [HideInInspector] public MiningUIManager mnm;
    [HideInInspector] public MiningTimeUIManager mtm;
    [HideInInspector] public WorkSpaceUIManager wsm;
    // SkillTree
    [HideInInspector] public SkillTreeUIManager stm;
    [HideInInspector] public LevelUpManager lvm;
    // Inventory
    [HideInInspector] public InventoryUIManager ivm;
    [HideInInspector] public InventoryController ivCtrl;
    [HideInInspector] public InventoryEquipUIManager ivEqu;
    // TowerInfo
    [HideInInspector] public TowerInfoUIManager tifm;
    // Shop
    [HideInInspector] public ShopManager shopMg;
    [HideInInspector] public ChestInfoManager cim;
    // DailyMittion
    [HideInInspector] public DailyMissionManager dailyMs;
    // Spin
    [HideInInspector] public LuckySpinManager lspm;
    // InfiniteUpgrade
    [HideInInspector] public InfiniteUpgradeManager ifum;
    // Bonus Reward Systems
    [HideInInspector] public MileageRewardUIManager mlgm;
    [HideInInspector] public FameRewardUIManager frm;


    //* Value
    public Color SkyBlueColor;
    public Color RedOrangeColor;
    [field: SerializeField] public int SelectedStage {get; set;}
    [field: SerializeField] public int CurDay {get; private set;}

    // [field: SerializeField] public int Lv {
    //     get => DM._.DB.StatusDB.Lv;
    //     set {
    //         DM._.DB.StatusDB.Lv = value;
    //         hui.LvTxt.text = $"{value}";
    //         lvm.LevelMarkTxt.text = $"{value}";
    //     } 
    // }
    [field: SerializeField] public int GoldKey {
        get => DM._.DB.StatusDB.GoldKey;
        set {
            DM._.DB.StatusDB.GoldKey = value;
            hui.TopGoldKeyTxt.text = $"{value}/{Config.MAX_GOBLINKEY}";
            lspm.GoldkeyTxt.text = $"{value}/{Config.MAX_GOBLINKEY}";
            stgm.DungeonAlertDot.SetActive(value > 0);
        }
    }
    [field: SerializeField] public int Coin {
        get => DM._.DB.StatusDB.Coin;
        set {
            DM._.DB.StatusDB.Coin = value;
            hui.TopCoinTxt.text = $"{value}";

            //* Daily
            DailyMissionDB dmDB = DM._.DB.DailyMissionDB;
            dmDB.CollectCoinVal = value;
        } 
    }
    [field: SerializeField] public int Diamond {
        get => DM._.DB.StatusDB.Diamond;
        set {
            DM._.DB.StatusDB.Diamond = value;
            hui.TopDiamondTxt.text = $"{value}";

            //* Daily
            DailyMissionDB dmDB = DM._.DB.DailyMissionDB;
            dmDB.CollectDiamondVal = value;
        } 
    }
    [field: SerializeField] public int SkillPoint {
        get => DM._.DB.StatusDB.SkillPoint;
        set {
            DM._.DB.StatusDB.SkillPoint = value;
            stm.MySkillPointTxt.text = $"{value}";
        } 
    }
    [field: SerializeField] public int Crack {
        get => DM._.DB.StatusDB.Crack;
        set {
            DM._.DB.StatusDB.Crack = value;
            if(HM._) ifum.CurCrackTxt.text = $"{value}";
        } 
    }
    [field: SerializeField] public int Fame {
        get => DM._.DB.StatusDB.Fame;
        set {
            DM._.DB.StatusDB.Fame = value;
            if(HM._) frm.FamePointTxt.text = $"{value}";
        } 
    }

    void Awake() {
        //* Global化 値 代入
        _ = this;

        //* 外部のスクリプト 初期化
        hui = GameObject.Find("HomeUIManager").GetComponent<HomeUIManager>();
        stgm = GameObject.Find("StageUIManager").GetComponent<StageUIManager>();
        rwm = GameObject.Find("HomeRewardUIManager").GetComponent<HomeRewardUIManager>();
        rwlm = GameObject.Find("HomeRewardListUIManager").GetComponent<HomeRewardListUIManager>();
        mnm = GameObject.Find("MiningUIManager").GetComponent<MiningUIManager>();
        mtm = GameObject.Find("MiningTimeUIManager").GetComponent<MiningTimeUIManager>();
        wsm = GameObject.Find("WorkSpaceUIManager").GetComponent<WorkSpaceUIManager>();
        stm = GameObject.Find("SkillTreeUIManager").GetComponent<SkillTreeUIManager>();
        lvm = GameObject.Find("LevelUpManager").GetComponent<LevelUpManager>();
        ivm = GameObject.Find("InventoryUIManager").GetComponent<InventoryUIManager>();
        ivCtrl = GameObject.Find("InventoryController").GetComponent<InventoryController>();
        ivEqu = GameObject.Find("InventoryEquipUIManager").GetComponent<InventoryEquipUIManager>();
        tifm = GameObject.Find("TowerInfoUIManager").GetComponent<TowerInfoUIManager>();
        shopMg = GameObject.Find("ShopManager").GetComponent<ShopManager>();
        cim = GameObject.Find("ChestInfoManager").GetComponent<ChestInfoManager>();
        dailyMs = GameObject.Find("DailyMissionManager").GetComponent<DailyMissionManager>();
        lspm = GameObject.Find("LuckySpinManager").GetComponent<LuckySpinManager>();
        ifum = GameObject.Find("InfiniteUpgradeManager").GetComponent<InfiniteUpgradeManager>();
        mlgm = GameObject.Find("MileageRewardUIManager").GetComponent<MileageRewardUIManager>();
        frm = GameObject.Find("FameRewardUIManager").GetComponent<FameRewardUIManager>();

        //* 初期化
        SelectedStage = 0;
        CurDay = DateTime.UtcNow.Day;
    }

    void Start() {
        DM._.LoadDt();
        SM._.BgmPlay(SM.BGM.HomeBGM);
        TutoM._.InitHomeBubbleElements();
    }
}
