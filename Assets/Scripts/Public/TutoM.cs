using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutoM : MonoBehaviour {
    static public TutoM _;

    [field: Header("PUCLIC")]
    [field: SerializeField] public int HowToPlayIdx {get; private set;}
    [field: SerializeField] public int EnemyInfoIdx {get; private set;}

    [field: SerializeField] public GameObject HowToPlayPopUpObj {get; private set;}
    [field: SerializeField] public TMP_Text HowToPlayPageTxt {get; private set;}
    [field: SerializeField] public GameObject[] HowToPlayObjs {get; private set;}
    [field: SerializeField] public GameObject EnemyInfoPopUpObj {get; private set;}
    [field: SerializeField] public TMP_Text EnemyInfoPageTxt {get; private set;}
    [field: SerializeField] public GameObject[] EnemyInfoObjs {get; private set;}

    [field: Header("HOME")]
    [field: SerializeField] public GameObject H_TutoGameStartBubble {get; private set;}
    [field: SerializeField] public GameObject G_TutoPathFindBubble {get; private set;}
    [field: SerializeField] public GameObject G_TutoWaveStartBubble {get; private set;}

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
        HowToPlayPageTxt.text = $"(1/{HowToPlayObjs.Length})";
    }

#region EVENT
    public void OnClickCloseBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        if(GM._) {
            Debug.Log($"OnClickCloseBtn():: GM:: Play");
            GM._.gui.Play();
        }

        HowToPlayPopUpObj.SetActive(false);
        EnemyInfoPopUpObj.SetActive(false);
    }
    public void OnClickPreviousBtn() {
        Debug.Log("TutoM:: OnClickPreviousBtn()::");
        SM._.SfxPlay(SM.SFX.ClickSFX);
        HowToPlayIdx--;
        if(HowToPlayIdx < 0) {
            HowToPlayIdx = 0;
        }
        HowToPlayPageTxt.text = $"({HowToPlayIdx + 1}/{HowToPlayObjs.Length})";

        SetHowToPlayPage();
    }
    public void OnClickNextBtn() {
        Debug.Log("TutoM:: OnClickNextBtn()::");
        SM._.SfxPlay(SM.SFX.ClickSFX);
        int lastIdx = HowToPlayObjs.Length - 1;
        HowToPlayIdx++;
        if(HowToPlayIdx >= lastIdx) {
            HowToPlayIdx = lastIdx;
        }
        HowToPlayPageTxt.text = $"({HowToPlayIdx + 1}/{HowToPlayObjs.Length})";

        for(int i = 0; i < HowToPlayObjs.Length; i++)
            HowToPlayObjs[i].SetActive(i == HowToPlayIdx);
        SetHowToPlayPage();
    }
    public void OnClickEnemyInfoPreviousBtn() {
        Debug.Log("TutoM:: OnClickEnemyInfoPreviousBtn()::");
        SM._.SfxPlay(SM.SFX.ClickSFX);
        EnemyInfoIdx--;
        if(EnemyInfoIdx < 0) {
            EnemyInfoIdx = 0;
        }
        EnemyInfoPageTxt.text = $"({EnemyInfoIdx + 1}/{EnemyInfoObjs.Length})";
        SetEnemyInfoPage();
    }
    public void OnClickEnemyInfoNextBtn() {
        Debug.Log("TutoM:: OnClickEnemyInfoNextBtn()::");
        SM._.SfxPlay(SM.SFX.ClickSFX);
        int lastIdx = EnemyInfoObjs.Length - 1;
        EnemyInfoIdx++;
        if(EnemyInfoIdx >= lastIdx) {
            EnemyInfoIdx = lastIdx;
        }
        EnemyInfoPageTxt.text = $"({EnemyInfoIdx + 1}/{EnemyInfoObjs.Length})";

        for(int i = 0; i < EnemyInfoObjs.Length; i++)
            EnemyInfoObjs[i].SetActive(i == EnemyInfoIdx);
        SetEnemyInfoPage();
    }
#endregion

#region FUNC
    public void InitHomeBubbleElements() {
        H_TutoGameStartBubble = GameObject.Find("H_TutoGameStartBubble");
    }
    public void InitGameBubbleElements() {
        G_TutoPathFindBubble = GameObject.Find("G_TutoPathFindBubble");
        G_TutoWaveStartBubble = GameObject.Find("G_TutoWaveStartBubble");
    }

    public void ShowHowToPlayPopUp(float delay) {
        SM._.SfxPlay(SM.SFX.ItemPickSFX, delay);
        HowToPlayPopUpObj.SetActive(true);

        HowToPlayIdx = 0;
        SetHowToPlayPage();
    }
    public void ShowEnemyInfoPopUp(int page) {
        if(GM._) {
            Debug.Log($"ShowEnemyInfoPopUp():: GM:: Pause");
            GM._.gui.Pause();
        }

        SM._.SfxPlay(SM.SFX.ItemPickSFX);
        EnemyInfoPopUpObj.SetActive(true);

        EnemyInfoIdx = page;
        EnemyInfoPageTxt.text = $"({EnemyInfoIdx + 1}/{EnemyInfoObjs.Length})";
        SetEnemyInfoPage();
    }
    public void SetHowToPlayPage() {
        for(int i = 0; i < HowToPlayObjs.Length; i++)
            HowToPlayObjs[i].SetActive(i == HowToPlayIdx);
    }
    public void SetEnemyInfoPage() {
        for(int i = 0; i < EnemyInfoObjs.Length; i++)
            EnemyInfoObjs[i].SetActive(i == EnemyInfoIdx);
    }
#endregion
}
