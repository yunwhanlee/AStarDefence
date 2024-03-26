using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.U2D.Animation;

[Serializable]
public class WorkSpace {
    public bool IsLock;
    public SpotData GoblinSpotDt;
    public SpotData OreSpotDt;
}

[Serializable]
public struct SpotData {
    public bool IsActive;
    public int LvIdx;
}

[Serializable]
public class MiningCard {
    public string Name;
    public int Cnt;
    public bool IsChecked;
    public Button Button;
    public TextMeshProUGUI CntTxt;
    public Image Outline;
    public Image CheckMark;
    public GameObject Dim;
    public void InitOutline() => Outline.color = Color.black;
    public void Select() => Outline.color = Color.red;
    public bool Selected() => Outline.color == Color.red;
    public void InitCheck() {
        IsChecked = false;
        CheckMark.gameObject.SetActive(false);
    }
    public void Check() {
        IsChecked = true;
        CheckMark.gameObject.SetActive(true);
    }
    public void Update() {
        Dim.SetActive(Cnt <= 0);
        CntTxt.text = $"보유: {Cnt} / 5";
        CntTxt.color = Cnt > 5? Color.green : Color.white;
    }
}

public class MiningUIManager : MonoBehaviour {
    const int MERGE_CNT = 5;
    public enum Cate {Goblin, Ore};

    [field: SerializeField] public Cate CurCategory {get; set;}
    [field: SerializeField] public int CurWorkSpaceIdx {get; set;} = 0;

    [field: Header("Work Space Datas")]
    [field: SerializeField] public WorkSpace[] WorkSpaces = new WorkSpace[4];

    [field: SerializeField] public MiningCard[] GoblinCards {get; private set;}
    [field: SerializeField] public MiningCard[] OreCards {get; private set;}

    [field: Header("Resource")]
    [field: SerializeField] public SpriteLibraryAsset[] GoblinSprLibAst {get; set;}
    [field: SerializeField] public Sprite[] OreSprs {get; set;}

    [field: Header("Home")]
    [field: SerializeField] public Slider WorkTimeSlider {get; set;}
    [field: SerializeField] public TextMeshProUGUI TitleTxt {get; set;}
    [field: SerializeField] public Button OreSpotBtn {get; set;}
    [field: SerializeField] public Button GoblinLeftSpotBtn {get; set;}
    [field: SerializeField] public Image OreImg {get; set;}
    [field: SerializeField] public SpriteLibrary GoblinBodySprLib {get; set;}

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
        /// ワークスペース移動
        /// </summary>
        /// <param name="dir">-1：左、1：右</param>
        public void OnClickSwitchWorkSpace(int dir) {
            CurWorkSpaceIdx += dir;

            //* インデックス超える、下げる 防止
            if(CurWorkSpaceIdx >= WorkSpaces.Length)
                CurWorkSpaceIdx = 0;
            if(CurWorkSpaceIdx < 0)
                CurWorkSpaceIdx = WorkSpaces.Length - 1;
            
            TitleTxt.text = $"작업장 {CurWorkSpaceIdx + 1}";

            var curWorkSpace = WorkSpaces[CurWorkSpaceIdx];
            ActiveSpot(Cate.Goblin, curWorkSpace.GoblinSpotDt);
            ActiveSpot(Cate.Ore, curWorkSpace.OreSpotDt);
        }

        /// <summary>
        /// ホームの場所⊕ボタン
        /// </summary>
        public void OnClickGoblinLeftSpotBtn() {
            WindowObj.SetActive(true);
            SetUI((int)Cate.Goblin);
        }
        public void OnClickOreSpotBtn() {
            WindowObj.SetActive(true);
            SetUI((int)Cate.Ore);
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
        public void OnClickGoblinCard(int idx) => SelectCard(Cate.Goblin, idx);
        public void OnClickOreCard(int idx) => SelectCard(Cate.Ore, idx);

        /// <summary>
        /// アイテム配置ボタン
        /// </summary>
        public void OnClickArrangeBtn() {
            if(CurCategory == Cate.Ore) {
                if(WorkSpaces[CurWorkSpaceIdx].OreSpotDt.IsActive) {
                    HM._.hui.ShowAgainAskMsg("(주의)\n현재 배치된 광석이 사라집니다.");
                    HM._.hui.OnClickConfirmAction = () => Arrange(Cate.Ore);
                    return;
                }
            }
            Arrange(CurCategory);
        }
        public void OnClickArrangeCancelBtn() {
            if(CurCategory == Cate.Goblin) {
                Remove(Cate.Goblin);
            }
            else {
                HM._.hui.ShowAgainAskMsg("(주의)\n현재 배치된 광석이 사라집니다.");
                HM._.hui.OnClickConfirmAction = () => Remove(Cate.Ore);
            }
        }

        public void OnClickMergeBtn() {
            if(CurCategory == Cate.Goblin) {
                Merge(GoblinCards);
            }
            else {
                Merge(OreCards);
            }
        }

        public void OnClickAutoMergeBtn() {
            if(CurCategory == Cate.Goblin) {
                AutoMerge(GoblinCards);
            }
            else {
                AutoMerge(OreCards);
            }
        }
    #endregion

    #region FUNC
        private void ActiveSpot(Cate cate, SpotData spotDt) {
            const int OFF = 0, ON = 1;
            if(cate == Cate.Goblin) {
                GoblinLeftSpotBtn.transform.GetChild(OFF).gameObject.SetActive(!spotDt.IsActive);
                GoblinLeftSpotBtn.transform.GetChild(ON).gameObject.SetActive(spotDt.IsActive);

                //* ✓非表示
                Array.ForEach(GoblinCards, card => card.InitCheck());

                //* ✓と画像 アップデート
                if(spotDt.LvIdx != -1) {
                    GoblinCards[spotDt.LvIdx].Check();
                    GoblinBodySprLib.spriteLibraryAsset = GoblinSprLibAst[spotDt.LvIdx];
                }
            }
            else {
                OreSpotBtn.transform.GetChild(OFF).gameObject.SetActive(!spotDt.IsActive);
                OreSpotBtn.transform.GetChild(ON).gameObject.SetActive(spotDt.IsActive);
                
                //* ✓非表示
                Array.ForEach(OreCards, card => card.InitCheck());

                //* ✓と画像 アップデート
                if(spotDt.LvIdx != -1) {
                    OreCards[spotDt.LvIdx].Check();
                    OreImg.sprite = OreSprs[spotDt.LvIdx];
                }

            }
        }
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
            PopUpTitleTxt.text = $"작업장 {CurWorkSpaceIdx + 1}";

            //* 配置ボタン 表示
            if(idx == (int)Cate.Goblin) {
                CurCategory = Cate.Goblin;
                SetArrangeBtn(WorkSpaces[CurWorkSpaceIdx].GoblinSpotDt.IsActive);
            }
            else {
                CurCategory = Cate.Ore;
                SetArrangeBtn(WorkSpaces[CurWorkSpaceIdx].OreSpotDt.IsActive);
            }

            //* データ アップデート
            Array.ForEach(GoblinCards, card => card.Update());
            Array.ForEach(OreCards, card => card.Update());

            //* カテゴリ
            Array.ForEach(CategoryUnderLines, underline => underline.SetActive(false));
            CategoryUnderLines[idx].gameObject.SetActive(true);

            //* ScrollRect
            GoblinScrollRect.SetActive(idx == (int)Cate.Goblin);
            OreScrollRect.SetActive(idx == (int)Cate.Ore);
        }
        /// <summary>
        /// カード選択
        /// </summary>
        private void SelectCard(Cate cate, int idx) {
            MiningCard[] cards = (cate == Cate.Goblin)? GoblinCards : OreCards;
            SpotData spotData = (cate == Cate.Goblin)? WorkSpaces[CurWorkSpaceIdx].GoblinSpotDt : WorkSpaces[CurWorkSpaceIdx].OreSpotDt;
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
        private void Arrange(Cate cate) {
            //* エラーメッセージ
            if(!Array.Exists(cate == Cate.Goblin? GoblinCards : OreCards, card => card.Selected())) {
                HM._.hui.ShowMsgError($"배치할 {(cate == Cate.Goblin? "고블린" : "광석")}을 골라주세요!");
                return;
            }

            Array.ForEach ((cate == Cate.Goblin)? GoblinCards : OreCards, card => card.InitCheck()); 
            int lvIdx = Array.FindIndex ((cate == Cate.Goblin)? GoblinCards : OreCards, card => card.Selected());

            if(cate == Cate.Goblin) {
                //* UIチェック
                GoblinCards[lvIdx].Check();
                GoblinCards[lvIdx].Cnt--;
                //* データ
                WorkSpaces[CurWorkSpaceIdx].GoblinSpotDt.IsActive = true;
                WorkSpaces[CurWorkSpaceIdx].GoblinSpotDt.LvIdx = lvIdx;
                //* 配置
                ActiveSpot(Cate.Goblin, WorkSpaces[CurWorkSpaceIdx].GoblinSpotDt);
                GoblinBodySprLib.spriteLibraryAsset = GoblinSprLibAst[lvIdx];
            }
            else {
                //* UIチェック
                OreCards[lvIdx].Check();
                OreCards[lvIdx].Cnt--; // カウント減る
                //* データ
                WorkSpaces[CurWorkSpaceIdx].OreSpotDt.IsActive = true;
                WorkSpaces[CurWorkSpaceIdx].OreSpotDt.LvIdx = lvIdx;
                //* 配置
                ActiveSpot(Cate.Ore, WorkSpaces[CurWorkSpaceIdx].OreSpotDt);
                OreImg.sprite = OreSprs[lvIdx];
            }

            WindowObj.SetActive(false);
        }

        private void Remove(Cate cate) {
            if(cate == Cate.Goblin) {
                ActiveSpot(Cate.Goblin,  WorkSpaces[CurWorkSpaceIdx].GoblinSpotDt);
                GoblinCards[WorkSpaces[CurWorkSpaceIdx].GoblinSpotDt.LvIdx].InitCheck();
                WorkSpaces[CurWorkSpaceIdx].GoblinSpotDt.IsActive = false;
                WorkSpaces[CurWorkSpaceIdx].GoblinSpotDt.LvIdx = -1;
                WindowObj.SetActive(false);
            }
            else {
                ActiveSpot(Cate.Ore, WorkSpaces[CurWorkSpaceIdx].OreSpotDt);
                OreCards[WorkSpaces[CurWorkSpaceIdx].OreSpotDt.LvIdx].InitCheck();
                WorkSpaces[CurWorkSpaceIdx].OreSpotDt.IsActive = false;
                WorkSpaces[CurWorkSpaceIdx].OreSpotDt.LvIdx = -1;
                WindowObj.SetActive(false);
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
