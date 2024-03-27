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
    Coroutine CorTimerID;

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

    void Awake() {
        Array.ForEach(GoblinCards, card => card.InitOutline()); 
        Array.ForEach(OreCards, card => card.InitOutline()); 

        GoblinCards[0].Cnt = 14;
        GoblinCards[1].Cnt = 4;

        OreCards[0].Cnt = 11;
        OreCards[1].Cnt = 4;

        if(WindowObj.activeSelf)
            WindowObj.SetActive(false);
    }

    #region EVENT
        /// <summary>
        /// ホームの場所⊕ボタン
        /// </summary>
        public void OnClickGoblinLeftSpotBtn() {
            WindowObj.SetActive(true);
            SetUI((int)MineCate.Goblin);
        }
        public void OnClickOreSpotBtn() {
            WindowObj.SetActive(true);
            SetUI((int)MineCate.Ore);
        }

        /// <summary>
        /// カテゴリボタン
        /// </summary>
        /// <param name="idx">0: Goblin, 1: Ore</param>
        public void OnClickCategoryBtn(int idx) => SetUI(idx);
        public void OnClickBackBtn() {
            WindowObj.SetActive(false);
        }

        /// <summary>
        /// カードボタン
        /// </summary>
        /// <param name="idx">0: Goblin, 1: Ore</param>
        public void OnClickGoblinCard(int idx) => SelectCard(MineCate.Goblin, idx);
        public void OnClickOreCard(int idx) => SelectCard(MineCate.Ore, idx);

        /// <summary>
        /// アイテム配置ボタン
        /// </summary>
        public void OnClickArrangeBtn() {
            if(CurCategory == MineCate.Ore) {
                if(HM._.wsm.CurWorkSpace.OreSpotDt.IsActive) {
                    HM._.hui.ShowAgainAskMsg("(주의)\n현재 배치된 광석이 사라집니다.");
                    HM._.hui.OnClickConfirmAction = () => Arrange(MineCate.Ore);
                    return;
                }
            }

            Arrange(CurCategory);

            //* 採掘開始
            bool isSuccess = HM._.wsm.CurWorkSpace.StartMining(HM._.wsm.CurIdx);

            if(isSuccess) {
                Debug.Log("OnClickArrangeBtn():: isSuccess= " + isSuccess);
                int oreLvIdx = HM._.wsm.CurWorkSpace.OreSpotDt.LvIdx;
                int timeSec = OreDataSO.Datas[oreLvIdx].TimeSec;

                CorTimerID = StartCoroutine(HM._.wsm.CurWorkSpace.CoTimerStart(timeSec));
            }

        }
        public void OnClickArrangeCancelBtn() {
            if(CurCategory == MineCate.Goblin)
                Remove(MineCate.Goblin);
            else {
                HM._.hui.ShowAgainAskMsg("(주의)\n현재 배치된 광석이 사라집니다.");
                HM._.hui.OnClickConfirmAction = () => Remove(MineCate.Ore);
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
        private void SetArrangeBtn(bool isActive) {
            const int ON = 0, CANCEL = 1;
            ArrangeBtns[ON].SetActive(!isActive);
            ArrangeBtns[CANCEL].SetActive(isActive);
        }
        /// <summary>
        /// カテゴリによるUI表示
        /// </summary>
        /// <param name="idx">0: Goblin, 1: Ore</param>
        private void SetUI(int idx) {
            PopUpTitleTxt.text = $"작업장 {HM._.wsm.CurIdx + 1}";

            //* 配置ボタン 表示
            if(idx == (int)MineCate.Goblin) {
                CurCategory = MineCate.Goblin;
                SetArrangeBtn(HM._.wsm.CurWorkSpace.GoblinSpotDt.IsActive);
            }
            else {
                CurCategory = MineCate.Ore;
                SetArrangeBtn(HM._.wsm.CurWorkSpace.OreSpotDt.IsActive);
            }

            //* データ アップデート
            Array.ForEach(GoblinCards, card => card.Update());
            Array.ForEach(OreCards, card => card.Update());

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
        private void SelectCard(MineCate cate, int idx) {
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
                HM._.wsm.CurWorkSpace.GoblinSpotDt.IsActive = true;
                HM._.wsm.CurWorkSpace.GoblinSpotDt.LvIdx = lvIdx;
                //* 配置
                HM._.wsm.ActiveSpot(MineCate.Goblin, HM._.wsm.CurWorkSpace.GoblinSpotDt);
                HM._.wsm.GoblinSpot.BodySprLib.spriteLibraryAsset = GoblinDataSO.Datas[lvIdx].SprLibAst;
            }
            else {
                //* UIチェック
                OreCards[lvIdx].Check();
                OreCards[lvIdx].Cnt--; // カウント減る
                //* データ
                HM._.wsm.CurWorkSpace.OreSpotDt.IsActive = true;
                HM._.wsm.CurWorkSpace.OreSpotDt.LvIdx = lvIdx;
                //* 配置
                HM._.wsm.ActiveSpot(MineCate.Ore, HM._.wsm.CurWorkSpace.OreSpotDt);
                HM._.wsm.OreSpot.OreImg.sprite = OreDataSO.Datas[lvIdx].Sprs[(int)ORE_SPRS.DEF]; // OreSprs[lvIdx];
            }

            WindowObj.SetActive(false);
        }

        private void Remove(MineCate cate) {
            //* Timer Off
            if(CorTimerID != null) StopCoroutine(CorTimerID);
            HM._.wsm.SetTimerSlider("0", 0);
            
            //* Goblin Anim Off
            // HM._.wsm.GoblinChrCtrl.GoblinStopMiningAnim();
            // HM._.wsm.GoblinChrCtrl.SpawnAnim();

            if(cate == MineCate.Goblin) {
                GoblinCards[HM._.wsm.CurWorkSpace.GoblinSpotDt.LvIdx].InitCheck();
                HM._.wsm.CurWorkSpace.GoblinSpotDt.IsActive = false;
                HM._.wsm.CurWorkSpace.GoblinSpotDt.LvIdx = -1;
                WindowObj.SetActive(false);
                HM._.wsm.ActiveSpot(MineCate.Goblin, HM._.wsm.CurWorkSpace.GoblinSpotDt);
            }
            else {
                OreCards[HM._.wsm.CurWorkSpace.OreSpotDt.LvIdx].InitCheck();
                HM._.wsm.CurWorkSpace.OreSpotDt.IsActive = false;
                HM._.wsm.CurWorkSpace.OreSpotDt.LvIdx = -1;
                WindowObj.SetActive(false);
                HM._.wsm.ActiveSpot(MineCate.Ore, HM._.wsm.CurWorkSpace.OreSpotDt);
            }
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
            }
            HM._.hui.ShowMsgNotice("자동합성 완료!");
        }
    #endregion
}
