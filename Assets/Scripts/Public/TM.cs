using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TM : MonoBehaviour {
    static public TM _;

    const int SKIP_TXT = 0, OK_LETSGO_TXT = 1;

    [field: Header("PUCLIC")]
    [field: SerializeField] public int HowToPlayIdx {get; private set;}
    [field: SerializeField] public GameObject TutoWindowObj {get; private set;}
    [field: SerializeField] public TMP_Text PageTxt {get; private set;}
    [field: SerializeField] public GameObject[] SkipBtnTxtObjs {get; private set;}
    [field: SerializeField] public GameObject[] HowToPlayObjs {get; private set;}

    [field: Header("HOME")]
    [field: SerializeField] public GameObject H_TutoGameStartBubble {get; private set;}


    void Awake() => singleton();

    private void singleton(){
        if(_ == null) {
            _ = this;
            DontDestroyOnLoad(_);
        }
        else
            Destroy(gameObject);
    }    

    void Start() {
        PageTxt.text = $"( 1 / {HowToPlayObjs.Length} )";
    }

#region EVENT
    public void OnClickSkipBtn() {
        TutoWindowObj.SetActive(false);
    }
    public void OnClickPreviousBtn() {
        SkipBtnTxtObjs[SKIP_TXT].SetActive(true);
        SkipBtnTxtObjs[OK_LETSGO_TXT].SetActive(false);
        HowToPlayIdx--;
        if(HowToPlayIdx < 0) {
            HowToPlayIdx = 0;
        }
        PageTxt.text = $"( {HowToPlayIdx + 1} / {HowToPlayObjs.Length} )";

        SetPage();
    }
    public void OnClickNextBtn() {
        int lastIdx = HowToPlayObjs.Length - 1;
        SkipBtnTxtObjs[SKIP_TXT].SetActive(true);
        SkipBtnTxtObjs[OK_LETSGO_TXT].SetActive(false);
        HowToPlayIdx++;
        if(HowToPlayIdx >= lastIdx) {
            HowToPlayIdx = lastIdx;
            SkipBtnTxtObjs[SKIP_TXT].SetActive(false);
            SkipBtnTxtObjs[OK_LETSGO_TXT].SetActive(true);
        }
        PageTxt.text = $"( {HowToPlayIdx + 1} / {HowToPlayObjs.Length} )";

        for(int i = 0; i < HowToPlayObjs.Length; i++)
            HowToPlayObjs[i].SetActive(i == HowToPlayIdx);
        SetPage();
    }
#endregion

#region FUNC
    public void ShowHowToPlayPopUp() {
        SM._.SfxPlay(SM.SFX.ItemPickSFX, 0.5f);
        TutoWindowObj.SetActive(true);
        SkipBtnTxtObjs[SKIP_TXT].SetActive(true);
        SkipBtnTxtObjs[OK_LETSGO_TXT].SetActive(false);

        HowToPlayIdx = 0;
        SetPage();
    }
    public void SetPage() {
        for(int i = 0; i < HowToPlayObjs.Length; i++)
            HowToPlayObjs[i].SetActive(i == HowToPlayIdx);
    }
#endregion
}
