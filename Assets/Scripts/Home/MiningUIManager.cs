using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.U2D.Animation;
using UnityEditor.PackageManager.UI;

[Serializable]
public struct SpotData {
    public bool isActive;
    public int lvIdx;
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

    public void InitOutline() {
        Outline.color = Color.black;
    }

    public void InitCheck() {
        IsChecked = false;
        CheckMark.gameObject.SetActive(false);
    }

    public void Update() {
        Outline.color = Color.black;
        Dim.SetActive(Cnt <= 0);
        CntTxt.text = $"보유: {Cnt} / 5";
        CntTxt.color = Cnt > 5? Color.green : Color.white;
    }

    public void Check() {
        IsChecked = true;
        CheckMark.gameObject.SetActive(true);
    }
}

public class MiningUIManager : MonoBehaviour {
    const int ON = 0, OFF = 1;
    const int MERGE_CNT = 5;
    public enum Category {Goblin, Ore};

    [field: SerializeField] public Category CurCategory {get; set;}

    [field: Header("Spot Data")]
    [field: SerializeField] public SpotData GoblinSpotDt;
    [field: SerializeField] public SpotData OreSpotDt;

    [field: SerializeField] public MiningCard[] GoblinCards {get; private set;}
    [field: SerializeField] public MiningCard[] OreCards {get; private set;}

    [field: Header("Resource")]
    [field: SerializeField] public SpriteLibraryAsset[] GoblinSprLibAst {get; set;}
    [field: SerializeField] public Sprite[] OreSprs {get; set;}

    [field: Header("Home")]
    [field: SerializeField] public Button OreSpotBtn {get; set;}
    [field: SerializeField] public Button GoblinLeftSpotBtn {get; set;}
    [field: SerializeField] public Image OreImg {get; set;}
    [field: SerializeField] public SpriteLibrary GoblinBodySprLib {get; set;}

    [field: Header("PopUp")]
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
            SetUI((int)Category.Goblin);
        }
        public void OnClickOreSpotBtn() {
            WindowObj.SetActive(true);
            SetUI((int)Category.Ore);
        }
        // public void OnClickGoblinRightSpotBtn() {
        //     WindowObj.SetActive(true);
        //     SetUI((int)Category.Goblin);
        // }

        /// <summary>
        /// カテゴリボタン
        /// </summary>
        /// <param name="idx">0: Goblin, 1: Ore</param>
        public void OnClickCategoryBtn(int idx) {
            SetUI(idx);
        }
        public void OnClickBackBtn() {
            WindowObj.SetActive(false);
        }

        /// <summary>
        /// カードボタン
        /// </summary>
        /// <param name="idx">0: Goblin, 1: Ore</param>
        public void OnClickGoblinCard(int idx) {
            //* 初期化
            Array.ForEach(GoblinCards, card => card.Update());
            //* UI
            GoblinCards[idx].Outline.color = Color.red;
            if(GoblinSpotDt.isActive && GoblinSpotDt.lvIdx == idx) {
                ArrangeBtns[ON].SetActive(false);
                ArrangeBtns[OFF].SetActive(true);
            }
            else {
                ArrangeBtns[ON].SetActive(true);
                ArrangeBtns[OFF].SetActive(false);
            }
        }
        public void OnClickOreCard(int idx) {
            //* 初期化
            Array.ForEach(OreCards, card => card.Update());
            //* UI
            OreCards[idx].Outline.color = Color.red;
            if(OreSpotDt.isActive && OreSpotDt.lvIdx == idx) {
                ArrangeBtns[ON].SetActive(false);
                ArrangeBtns[OFF].SetActive(true);
            }
            else {
                ArrangeBtns[ON].SetActive(true);
                ArrangeBtns[OFF].SetActive(false);
            }
        }

        /// <summary>
        /// アイテム配置ボタン
        /// </summary>
        public void OnClickArrangeBtn() {
            if(CurCategory == Category.Goblin) {
                if(!Array.Exists(GoblinCards, card => card.Outline.color == Color.red)) {
                    HM._.hui.ShowMsgError("배치할 고블린을 골라주세요!");
                    return;
                }

                //* UIチェック
                Array.ForEach(GoblinCards, card => card.InitCheck()); // 初期化
                int lvIdx = Array.FindIndex(GoblinCards, card => card.Outline.color == Color.red);
                GoblinCards[lvIdx].Check();

                //* データ
                GoblinSpotDt.isActive = true;
                GoblinSpotDt.lvIdx = lvIdx;

                //* キャラー配置
                GoblinLeftSpotBtn.transform.GetChild(0).gameObject.SetActive(false);
                GoblinLeftSpotBtn.transform.GetChild(1).gameObject.SetActive(true);
                GoblinBodySprLib.spriteLibraryAsset = GoblinSprLibAst[lvIdx];

                WindowObj.SetActive(false);
            }
            else {
                if(!Array.Exists(OreCards, card => card.Outline.color == Color.red)) {
                    HM._.hui.ShowMsgError("배치할 광석을 골라주세요!");
                    return;
                }

                //* UIチェック
                Array.ForEach(OreCards, card => card.InitCheck()); // 初期化
                int lvIdx = Array.FindIndex(OreCards, card => card.Outline.color == Color.red);
                OreCards[lvIdx].Check();

                //* データ
                OreSpotDt.isActive = true;
                OreSpotDt.lvIdx = lvIdx;

                //* 鉱石(こうせき)配置
                OreSpotBtn.transform.GetChild(0).gameObject.SetActive(false);
                OreSpotBtn.transform.GetChild(1).gameObject.SetActive(true);
                OreImg.sprite = OreSprs[lvIdx];

                WindowObj.SetActive(false);
            }
        }

        public void OnClickMergeBtn() {
            if(CurCategory == Category.Goblin) {
                if(!Array.Exists(GoblinCards, card => card.Outline.color == Color.red)) {
                    HM._.hui.ShowMsgError("합성할 고블린을 골라주세요!");
                    return;
                }

                int lvIdx = Array.FindIndex(GoblinCards, card => card.Outline.color == Color.red);
                if(GoblinCards[lvIdx].Cnt >= 5) {
                    int nextIdx = lvIdx + 1;
                    GoblinCards[lvIdx].Cnt -= MERGE_CNT;
                    GoblinCards[nextIdx].Cnt++;
                    GoblinCards[lvIdx].Update();
                    GoblinCards[nextIdx].Update();
                    HM._.hui.ShowMsgNotice("합성 완료!");
                }
                else {
                    HM._.hui.ShowMsgError("합성할 개수가 부족합니다!");
                    return;
                }
            }
            else {
                if(!Array.Exists(OreCards, card => card.Outline.color == Color.red)) {
                    HM._.hui.ShowMsgError("합성할 광석을 골라주세요!");
                    return;
                }

                int lvIdx = Array.FindIndex(OreCards, card => card.Outline.color == Color.red);
                if(OreCards[lvIdx].Cnt >= 5) {
                    int nextIdx = lvIdx + 1;
                    OreCards[lvIdx].Cnt -= MERGE_CNT;
                    OreCards[nextIdx].Cnt++;
                    OreCards[lvIdx].Update();
                    OreCards[nextIdx].Update();
                    HM._.hui.ShowMsgNotice("합성 완료!");
                }
                else {
                    HM._.hui.ShowMsgError("합성할 개수가 부족합니다!");
                    return;
                }
            }
        }
    #endregion

    #region FUNC
        /// <summary>
        /// カテゴリによるUI表示
        /// </summary>
        /// <param name="idx">0: Goblin, 1: Ore</param>
        private void SetUI(int idx) {
            //* カテゴリ設定
            if(idx == (int)Category.Goblin) {
                CurCategory = Category.Goblin;
                ArrangeBtns[ON].SetActive(!GoblinSpotDt.isActive);
                ArrangeBtns[OFF].SetActive(GoblinSpotDt.isActive);
            }
            else {
                CurCategory = Category.Ore;
                ArrangeBtns[ON].SetActive(!OreSpotDt.isActive);
                ArrangeBtns[OFF].SetActive(OreSpotDt.isActive);
            }

            //* データアップデート
            Array.ForEach(GoblinCards, card => card.Update());
            Array.ForEach(OreCards, card => card.Update());

            Array.ForEach(CategoryUnderLines, underline => underline.SetActive(false));
            CategoryUnderLines[idx].gameObject.SetActive(true);

            //* ScrollRect
            GoblinScrollRect.SetActive(idx == (int)Category.Goblin);
            OreScrollRect.SetActive(idx == (int)Category.Ore);
        }
    #endregion
}
