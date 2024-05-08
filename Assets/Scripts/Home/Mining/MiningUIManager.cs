using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.U2D.Animation;

public enum MineCate {
    Goblin, Ore
};

public class MiningUIManager : MonoBehaviour {
    const int MERGE_CNT = 5;
    public Coroutine[] CorTimerIDs = new Coroutine[5];

    [field: SerializeField] public MineCate CurCategory {get; set;}

    [field: SerializeField] public MiningCard[] GoblinCards {get; private set;}
    [field: SerializeField] public MiningCard[] OreCards {get; private set;}

    [field: Header("Resource")]
    [field: SerializeField] public SettingGoblinData GoblinDataSO {get; private set;}
    [field: SerializeField] public SettingOreData OreDataSO {get; private set;}

    [field: Header("PopUp")]
    [field: SerializeField] public TextMeshProUGUI PopUpTitleTxt {get; set;}
    [field: SerializeField] public GameObject[] CategoryUnderLines {get; set;}

    [field: SerializeField] public GameObject WindowObj {get; set;}
    [field: SerializeField] public GameObject GoblinScrollRect {get; set;}
    [field: SerializeField] public GameObject OreScrollRect {get; set;}
    [field: SerializeField] public GameObject[] ArrangeBtns {get; set;}
    [field: SerializeField] public GameObject LockedDim {get; set;}
    [field: SerializeField] public GameObject GreenAlertDot {get; set;}

    void Awake() {
        Array.ForEach(GoblinCards, card => card.InitOutline()); 
        Array.ForEach(OreCards, card => card.InitOutline()); 

         //* データ初期化
        for(int i = 0; i < GoblinCards.Length; i++)
            GoblinCards[i].InitData(MineCate.Goblin, i, $"{GoblinDataSO.Datas[i].SpeedPer * 100}%");
        for(int i = 0; i < OreCards.Length; i++) 
            OreCards[i].InitData(MineCate.Ore, i, $"{OreDataSO.Datas[i].TimeSec}초");

        if(WindowObj.activeSelf)
            WindowObj.SetActive(false);

        GreenAlertDot.SetActive(false);
    }

#region EVENT
    /// <summary>
    /// カテゴリボタン
    /// </summary>
    /// <param name="idx">0: Goblin, 1: Ore</param>
    public void OnClickCategoryBtn(int idx) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        SetUI(idx);
    }
    public void OnClickBackBtn() {
        HM._.hui.IsActivePopUp = false;
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(false);
    }
    /// <summary>
    /// MINING POPUPでの作業場の切り替え矢印ボタン
    /// </summary>
    /// <param name="dir">-1: ←(左)、+1: →(右)</param> <summary>
    public void OnClickWrokSpaceArrowPageBtn(int dir) {
        HM._.wsm.OnClickWorkSpacePageBtn(dir);
        if(CurCategory == MineCate.Goblin)
            HM._.wsm.OnClickGoblinLeftSpotBtn();
        else
            HM._.wsm.OnClickOreSpotBtn();
    }
    /// <summary>
    /// カードボタン
    /// </summary>
    /// <param name="idx">0: Goblin, 1: Ore</param>
    public void OnClickGoblinCard(int idx) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        SelectCard(MineCate.Goblin, idx);
        OnClickArrangeBtn();
    } 
    public void OnClickOreCard(int idx) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        SelectCard(MineCate.Ore, idx);
        OnClickArrangeBtn();
    } 

    /// <summary>
    /// アイテム配置ボタン
    /// </summary>
    public void OnClickArrangeBtn() {
        //* 鉱石カテゴリの上、鉱石が既に有ったら
        if(CurCategory == MineCate.Ore) {
            if(HM._.wsm.CurWorkSpace.OreSpotDt.IsActive) {
                SM._.SfxPlay(SM.SFX.ClickSFX);
                HM._.hui.ShowAgainAskMsg("배치된 광석을 교체하시겠습니까?\n<color=red>(주의)현재 광석은 제거됩니다.</color>");
                HM._.hui.OnClickAskConfirmAction = () => {
                    Arrange(CurCategory);
                    CanStartMining(isSwitchOre: true);
                };
                return;
            }
        }
        //* ゴブリンカテゴリの上、ゴブリンが既に有ったら
        else if(CurCategory == MineCate.Goblin) {
            if(HM._.wsm.CurWorkSpace.GoblinSpotDt.IsActive) {
                //* 鉱石も既に有ったら
                if(HM._.wsm.CurWorkSpace.OreSpotDt.IsActive) {
                    SM._.SfxPlay(SM.SFX.ClickSFX);
                    HM._.hui.ShowAgainAskMsg("고블린을 교체하시겠습니까?");
                    HM._.hui.OnClickAskConfirmAction = () => {
                        Remove(CurCategory);
                        Arrange(CurCategory);
                        CanStartMining();
                    };
                    return;
                }
                else {
                    HM._.mnm.GoblinCards[HM._.wsm.CurWorkSpace.GoblinSpotDt.LvIdx].Cnt++;
                }
            }
        }

        //* 空の場合
        Arrange(CurCategory);
        CanStartMining();
    }
    public void OnClickArrangeCancelBtn() {
        if(CurCategory == MineCate.Goblin) {
            HM._.hui.ShowAgainAskMsg("배치된 고블린을 회수합니까?");
            HM._.hui.OnClickAskConfirmAction = () => Remove(MineCate.Goblin);
        }
        else {
            HM._.hui.ShowAgainAskMsg("배치된 광석을 취소하시겠습니까?\n<color=red>(주의)현재 광석은 제거됩니다.</color>");
            HM._.hui.OnClickAskConfirmAction = () => Remove(MineCate.Ore);
        }
    }

    public void OnClickMergeBtn() {
        if(CurCategory == MineCate.Goblin)
            Merge(GoblinCards);
        else
            Merge(OreCards);
    }

    public void OnClickAutoMergeBtn() {
        if(CurCategory == MineCate.Goblin) {
            AutoMerge(GoblinCards);
        }
        else {
            AutoMerge(OreCards);
        }
    }
#endregion

#region FUNC
    /// <summary>
    /// 採掘開始
    /// </summary>
    public void CanStartMining(bool isSwitchOre = false) {
        WorkSpace curWS = HM._.wsm.CurWorkSpace;

        //* 採掘開始
        bool isSuccess = curWS.CheckSpotActive();

        if(isSuccess) {
            Debug.Log($"OnClickArrangeBtn():: isSuccess= {isSuccess}");
            CorTimerIDs[curWS.Id] = StartCoroutine(curWS.CoTimerStart(isSwitchOre));
        }
    }

    private void SetArrangeBtn(bool isActive) {
        const int ON = 0, CANCEL = 1;
        // ArrangeBtns[ON].SetActive(!isActive);
        // ArrangeBtns[CANCEL].SetActive(isActive);
    }

    /// <summary>
    /// カテゴリによるUI表示
    /// </summary>
    /// <param name="idx">0: Goblin, 1: Ore</param>
    public void SetUI(int idx) {
        PopUpTitleTxt.text = $"작업장 {HM._.wsm.CurIdx + 1}";

        //* 配置ボタン 表示
        if(idx == (int)MineCate.Goblin) {
            CurCategory = MineCate.Goblin;
            SetArrangeBtn(HM._.wsm.CurWorkSpace.GoblinSpotDt.IsActive);

            //* データ アップデート
            Array.ForEach(GoblinCards, card => card.Update());
            bool isGoblinMergable = Array.Exists(GoblinCards, card => card.Cnt >= 5);
            Debug.Log($"SetUI():: isGoblinMergable= {isGoblinMergable}");
            GreenAlertDot.SetActive(isGoblinMergable);
        }
        else {
            CurCategory = MineCate.Ore;
            SetArrangeBtn(HM._.wsm.CurWorkSpace.OreSpotDt.IsActive);

            //* データ アップデート
            Array.ForEach(OreCards, card => card.Update());
            bool isOreMergable = Array.Exists(OreCards, card => card.Cnt >= 5);
            GreenAlertDot.SetActive(isOreMergable);
            Debug.Log($"SetUI():: isOreMergable= {isOreMergable}");
        }


    


        //* カテゴリ
        Array.ForEach(CategoryUnderLines, underline => underline.SetActive(false));
        CategoryUnderLines[idx].gameObject.SetActive(true);

        //* ScrollRect
        GoblinScrollRect.SetActive(idx == (int)MineCate.Goblin);
        OreScrollRect.SetActive(idx == (int)MineCate.Ore);
    }
    /// <summary>
    /// カード選択
    /// </summary>
    public void SelectCard(MineCate cate, int idx) {
        MiningCard[] cards = (cate == MineCate.Goblin)? GoblinCards : OreCards;
        SpotData spotData = (cate == MineCate.Goblin)? HM._.wsm.CurWorkSpace.GoblinSpotDt : HM._.wsm.CurWorkSpace.OreSpotDt;
        //* 初期化
        Array.ForEach(cards, card => card.InitOutline());
        //* UI
        cards[idx].Select();
        if(spotData.IsActive && spotData.LvIdx == idx)
            SetArrangeBtn(isActive: true);
        else 
            SetArrangeBtn(isActive: false);
    }
    /// <summary>
    /// 配置
    /// </summary>
    private void Arrange(MineCate cate) {
        //* エラーメッセージ
        if(!Array.Exists(cate == MineCate.Goblin? GoblinCards : OreCards, card => card.Selected())) {
            HM._.hui.ShowMsgError($"배치할 {(cate == MineCate.Goblin? "고블린" : "광석")}을 골라주세요!");
            return;
        }

        Array.ForEach ((cate == MineCate.Goblin)? GoblinCards : OreCards, card => card.InitCheck()); 
        int lvIdx = Array.FindIndex ((cate == MineCate.Goblin)? GoblinCards : OreCards, card => card.Selected());

        if(cate == MineCate.Goblin) {
            //* UIチェック
            GoblinCards[lvIdx].Check();
            GoblinCards[lvIdx].Cnt--;
            //* データ 
            HM._.wsm.CurWorkSpace.GoblinSpotDt.SetData(true, lvIdx);
            //* 配置
            HM._.wsm.ActiveSpot(MineCate.Goblin, HM._.wsm.CurWorkSpace.GoblinSpotDt);
            HM._.wsm.GoblinSpot.BodySprLib.spriteLibraryAsset = GoblinDataSO.Datas[lvIdx].SprLibAst;
        }
        else {
            //* UIチェック
            OreCards[lvIdx].Check();
            OreCards[lvIdx].Cnt--; // カウント減る
            //* データ
            HM._.wsm.CurWorkSpace.OreSpotDt.SetData(true, lvIdx);
            //* 配置
            HM._.wsm.ActiveSpot(MineCate.Ore, HM._.wsm.CurWorkSpace.OreSpotDt);
            HM._.wsm.OreSpot.OreImg.sprite = OreDataSO.Datas[lvIdx].Sprs[(int)ORE_SPRS.DEF]; // OreSprs[lvIdx];
        }

        WindowObj.SetActive(false);
        SM._.SfxPlay(SM.SFX.ItemPickSFX);
        StopCorTimerID(HM._.wsm.CurWorkSpace.Id);
        HM._.hui.ShowMsgNotice($"{(cate == MineCate.Goblin? "고블린" : "광석")} 배치완료!");

        //* スライダー UI 初期化
        HM._.mtm.InitSlider();
    }

    private void StopCorTimerID(int idx) {
        if(CorTimerIDs[idx] != null)
            StopCoroutine(CorTimerIDs[idx]);
    }

    private void Remove(MineCate cate) {
        Debug.Log("Remove()::");
        SM._.SfxPlay(SM.SFX.DeleteTowerSFX);

        //* Timer Off
        StopCorTimerID(HM._.wsm.CurWorkSpace.Id);

        //* Goblin Anim
        HM._.wsm.GoblinChrCtrl.StopGoblinAnim();

        if(cate == MineCate.Goblin) {
            ref SpotData goblinSpotDt = ref HM._.wsm.CurWorkSpace.GoblinSpotDt;
            //* 既にゴブリンが有ったら
            if(goblinSpotDt.IsActive) {
                HM._.hui.ShowMsgNotice("고블린 회수완료!");
                HM._.mnm.GoblinCards[goblinSpotDt.LvIdx].Cnt++;
            }
            GoblinCards[goblinSpotDt.LvIdx].InitCheck();
            goblinSpotDt.Init();
            WindowObj.SetActive(false);
            HM._.wsm.ActiveSpot(MineCate.Goblin, goblinSpotDt);
        }
        else {
            ref SpotData oreSpotDt = ref HM._.wsm.CurWorkSpace.OreSpotDt;
            OreCards[oreSpotDt.LvIdx].InitCheck();
            oreSpotDt.Init();
            WindowObj.SetActive(false);
            HM._.wsm.ActiveSpot(MineCate.Ore, oreSpotDt);
        }

        //* スライダー UI 初期化
        HM._.mtm.InitSlider();
    }

    private void Merge(MiningCard[] cards) {
        if(!Array.Exists(cards, card => card.Selected())) {
            HM._.hui.ShowMsgError("합성할 것을 골라주세요!");
            return;
        }

        int lvIdx = Array.FindIndex(cards, card => card.Selected());
        if(cards[lvIdx].Cnt >= MERGE_CNT) {
            int nextIdx = lvIdx + 1;
            cards[lvIdx].Cnt -= MERGE_CNT;
            cards[nextIdx].Cnt++;
            cards[lvIdx].Update();
            cards[nextIdx].Update();

            SM._.SfxPlay(SM.SFX.Merge1SFX);
            HM._.hui.ShowMsgNotice("합성 완료!");
        }
        else {
            HM._.hui.ShowMsgError("합성할 개수가 부족합니다!");
            return;
        }
    }

    /// <summary>
    /// できる全てカードをマージ
    /// </summary>
    private void AutoMerge(MiningCard[] cards) {
        if(!Array.Exists(cards, card => card.Cnt >= 5)) {
            HM._.hui.ShowMsgError("합성할 것이 없습니다.");
            return;
        }

        for(int i = 0; i < cards.Length - 1; i++) {
            int nextIdx = i + 1;
            while(cards[i].Cnt >= MERGE_CNT) {
                cards[i].Cnt -= MERGE_CNT;
                cards[nextIdx].Cnt++;
            }
            cards[i].Update();
            GreenAlertDot.SetActive(false);
        }

        SM._.SfxPlay(SM.SFX.Merge3SFX);
        HM._.hui.ShowMsgNotice("자동합성 완료!");
    }
#endregion
}
