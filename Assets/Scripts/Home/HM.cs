using System.Collections;
using System.Collections.Generic;
using Inventory;
using Inventory.UI;
using TMPro;
using UnityEngine;

public class HM : MonoBehaviour {
    public static HM _; //* Global

    public Color SkyBlueColor;
    public Color RedOrangeColor;

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
    // Inventory
    [HideInInspector] public InventoryUIManager ivm;
    [HideInInspector] public InventoryController ivCtrl;
    [HideInInspector] public InventoryEquipUIManager ivEqu;

    //* Value
    [field: SerializeField] public int SelectedStage {get; set;}

    [field: SerializeField] public int Lv {
        get => DM._.DB.StatusDB.Lv;
        set {
            DM._.DB.StatusDB.Lv = value;
            hui.LvTxt.text = $"{value}";
        } 
    }
    [field: SerializeField] public int Exp {
        get => DM._.DB.StatusDB.Exp;
        set {
            DM._.DB.StatusDB.Exp = value;
            hui.ExpTxt.text = $"{value}";
        } 
    }
    [field: SerializeField] public int GoblinKey {
        get => DM._.DB.StatusDB.GoldKey;
        set {
            DM._.DB.StatusDB.GoldKey = value;
            hui.TopGoldKeyTxt.text = $"{value}/{Config.MAX_GOBLINKEY}";
        }
    }
    [field: SerializeField] public int Coin {
        get => DM._.DB.StatusDB.Coin;
        set {
            DM._.DB.StatusDB.Coin = value;
            hui.TopCoinTxt.text = $"{value}";
        } 
    }
    [field: SerializeField] public int Diamond {
        get => DM._.DB.StatusDB.Diamond;
        set {
            DM._.DB.StatusDB.Diamond = value;
            hui.TopDiamondTxt.text = $"{value}";
        } 
    }
    [field: SerializeField] public int SkillPoint {
        get => DM._.DB.StatusDB.SkillPoint;
        set {
            DM._.DB.StatusDB.SkillPoint = value;
            stm.MySkillPointTxt.text = $"{value}";
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
        ivm = GameObject.Find("InventoryUIManager").GetComponent<InventoryUIManager>();
        ivCtrl = GameObject.Find("InventoryController").GetComponent<InventoryController>();
        ivEqu = GameObject.Find("InventoryEquipUIManager").GetComponent<InventoryEquipUIManager>();

        //* 初期化
        SelectedStage = 0;
    }
}
