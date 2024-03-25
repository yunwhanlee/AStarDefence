using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

[System.Serializable]
public class MiningCard {
    public string Name;
    public Button Button;
    public TextMeshProUGUI CntTxt;
    public Image Outline;
    public int Cnt;
    public bool IsSelected;

    public void Update() {
        CntTxt.text = $"보유: {Cnt} / 5";
        Outline.color = Color.black;
        IsSelected = false;
    }
}

public class MiningUIManager : MonoBehaviour {
    const int CATE_GOBLIN = 0, CATE_ORE = 1;

    //* Home
    [field: SerializeField] public Button OreSpotBtn {get; set;}
    [field: SerializeField] public Button GoblinLeftSpotBtn {get; set;}
    [field: SerializeField] public Button GoblinRightSpotBtn {get; set;}
    //* PopUp
    [field: SerializeField] public GameObject[] CategoryUnderLines {get; set;}
    [field: SerializeField] public GameObject WindowObj {get; set;}
    [field: SerializeField] public GameObject GoblinScrollRect {get; set;}
    [field: SerializeField] public GameObject OreScrollRect {get; set;}
    [field: SerializeField] public MiningCard[] GoblinCards {get; private set;}
    [field: SerializeField] public MiningCard[] OreCards {get; private set;}

    void Start() {
        
    }

    #region EVENT
        public void OnClickGoblinLeftSpotBtn() {
            WindowObj.SetActive(true);
            SetUI(CATE_GOBLIN);
        }
        public void OnClickGoblinRightSpotBtn() {
            WindowObj.SetActive(true);
            SetUI(CATE_GOBLIN);
        }
        public void OnClickOreSpotBtn() {
            WindowObj.SetActive(true);
            SetUI(CATE_ORE);
        }

        public void OnClickCategoryBtn(int idx) { //* GOBLIN = 0, ORE = 1
            SetUI(idx);
        }
        public void OnClickBackBtn() {
            WindowObj.SetActive(false);
        }
        /// <summary>
        /// カードボタン
        /// </summary>
        public void OnClickGoblinCard(int idx) {
            //* 初期化
            Array.ForEach(GoblinCards, card => card.Update());
            //* UI
            GoblinCards[idx].IsSelected = true;
            GoblinCards[idx].Outline.color = Color.red;
        }
        public void OnClickOreCard(int idx) {
            //* 初期化
            Array.ForEach(OreCards, card => card.Update());
            //* UI
            OreCards[idx].IsSelected = true;
            OreCards[idx].Outline.color = Color.red;
        }
    #endregion

    #region FUNC
        /// <summary>
        /// カテゴリによるUI表示（idx= GOBLIN：０、ORE：１）
        /// </summary>
        private void SetUI(int idx) {
            //* データアップデート
            Array.ForEach(GoblinCards, card => card.Update());
            Array.ForEach(OreCards, card => card.Update());

            Array.ForEach(CategoryUnderLines, underline => underline.SetActive(false));
            CategoryUnderLines[idx].gameObject.SetActive(true);

            //* ScrollRect
            GoblinScrollRect.SetActive(idx == CATE_GOBLIN);
            OreScrollRect.SetActive(idx == CATE_ORE);
        }
    #endregion
}
