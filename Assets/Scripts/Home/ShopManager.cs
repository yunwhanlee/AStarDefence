using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShopManager : MonoBehaviour {
    const int TAPBTN_PACKAGE = 0, TAPBTN_CHEST = 1, TAPBTN_RSC = 2;
    [SerializeField] Color TapBtnActiveClr;
    [SerializeField] Color TapBtnInActiveClr;
    [field:SerializeField] public bool[] IsActiveTapBtnCates {get; private set;}
    [field:SerializeField] public GameObject WindowObj {get; private set;}
    [field:SerializeField] public GameObject[] CateTitleLineObjs {get; private set;}
    [field:SerializeField] public GameObject[] CateGroupObjs {get; private set;}
    [field:SerializeField] public Button[] TapBtns {get; private set;}

    void Start() {
        InitUI();
    }

#region EVENT
    public void OnClickShopIconBtnAtHome() => ShowShopPanel(true);
    public void OnClickBackBtn() => ShowShopPanel(false);
    public void OnClickTapBtn(int btnIdx) {
        if(TapBtns[btnIdx].GetComponent<Image>().color == TapBtnActiveClr) {
            InitUI();
        }

        //* TapBtnの色 初期化
        Array.ForEach(TapBtns, btn => btn.GetComponent<Image>().color = TapBtnInActiveClr);

        //* 適用
        IsActiveTapBtnCates[btnIdx] = true;
        TapBtns[btnIdx].GetComponent<Image>().color = TapBtnActiveClr;
        ActiveCateGroup(btnIdx);
    }

#endregion
#region FUNC
    private void ShowShopPanel(bool isShow) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(isShow);
        HM._.hui.TopNavCanvas.sortingOrder = isShow? 101 : 99;
    }
    private void InitUI() {
        Array.ForEach(IsActiveTapBtnCates, isActive => isActive = false);
        Array.ForEach(TapBtns, btn => btn.GetComponent<Image>().color = TapBtnInActiveClr);
        Array.ForEach(CateTitleLineObjs, titleLine => titleLine.SetActive(true));
        Array.ForEach(CateGroupObjs, groupObj => groupObj.SetActive(true));
    }
    public void ActiveCateGroup(int btnIdx) {
        for(int i = 0; i < CateGroupObjs.Length; i++) {
            CateTitleLineObjs[i].SetActive(btnIdx == i);
            CateGroupObjs[i].SetActive(btnIdx == i);
        }

        //* TAPがリーソースであれば、DiamondとCoin両方を表示する
        if(btnIdx == TAPBTN_RSC) {
            const int COIN_GROUP = TAPBTN_RSC + 1;
            CateTitleLineObjs[COIN_GROUP].SetActive(true);
            CateGroupObjs[COIN_GROUP].SetActive(true);
        }
    }
#endregion
}
