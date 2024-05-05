using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class TutoPopUp {
    [field: SerializeField] public int Idx {get; set;}
    [field: SerializeField] public GameObject WindowObj {get; private set;}
    [field: SerializeField] public TMP_Text PageTxt {get; private set;}
    [field: SerializeField] public GameObject[] Contents {get; private set;}
}

public class TutoM : MonoBehaviour {
    static public TutoM _;

    public const int HOWTIPLAY_INFO = 0;
    public const int ENEMY_IFNO = 1;
    public const int MINING_INFO = 2;

    [field: Header("PUCLIC")]
    [field: SerializeField] public int HowToPlayIdx {get; private set;}
    [field: SerializeField] public int EnemyInfoIdx {get; private set;}
    [field: SerializeField] public int MiningInfoIdx {get; private set;}

    [field: SerializeField] public GameObject HowToPlayPopUpObj {get; private set;}
    [field: SerializeField] public TMP_Text HowToPlayPageTxt {get; private set;}
    [field: SerializeField] public GameObject[] HowToPlayObjs {get; private set;}

    [field: SerializeField] public GameObject EnemyInfoPopUpObj {get; private set;}
    [field: SerializeField] public TMP_Text EnemyInfoPageTxt {get; private set;}
    [field: SerializeField] public GameObject[] EnemyInfoObjs {get; private set;}

    [field: SerializeField] public GameObject MiningInfoPopUpObj {get; private set;}
    [field: SerializeField] public TMP_Text MiningInfoPageTxt {get; private set;}
    [field: SerializeField] public GameObject[] MiningInfoObjs {get; private set;}

    [field: SerializeField] public TutoPopUp[] TutoPopUps {get; private set;}

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
        TutoPopUps[HOWTIPLAY_INFO].PageTxt.text = $"(1/{TutoPopUps[HOWTIPLAY_INFO].Contents.Length})";
        TutoPopUps[ENEMY_IFNO].PageTxt.text = $"(1/{TutoPopUps[ENEMY_IFNO].Contents.Length})";
        TutoPopUps[MINING_INFO].PageTxt.text = $"(1/{TutoPopUps[MINING_INFO].Contents.Length})";
    }

#region EVENT
    public void OnClickCloseBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        if(GM._) {
            Debug.Log($"OnClickCloseBtn():: GM:: Play");
            GM._.gui.Play();
        }

        //* 全てのPopUpを共通で非表示にする
        TutoPopUps[HOWTIPLAY_INFO].WindowObj.SetActive(false);
        TutoPopUps[ENEMY_IFNO].WindowObj.SetActive(false);
        TutoPopUps[MINING_INFO].WindowObj.SetActive(false);
    }
    /// <summary>
    /// チュートリアル ← ボタン
    /// </summary>
    /// <param name="tutoIdx">0: HOWTIPLAY_INFO, 1:ENEMY_IFNO, 2: MINING_INFO</param>
    public void OnClickPreviousBtn(int tutoIdx) {
        Debug.Log("TutoM:: OnClickPreviousBtn()::");
        SM._.SfxPlay(SM.SFX.ClickSFX);
        int cttLen = TutoPopUps[tutoIdx].Contents.Length;

        TutoPopUps[tutoIdx].Idx--;
        if(TutoPopUps[tutoIdx].Idx < 0)
            TutoPopUps[tutoIdx].Idx = 0;

        TutoPopUps[tutoIdx].PageTxt.text = $"({TutoPopUps[tutoIdx].Idx + 1}/{cttLen})";

        //SetHowToPlayPage();
        //* Show Page
        for(int i = 0; i < cttLen; i++)
            TutoPopUps[tutoIdx].Contents[i].SetActive(i == TutoPopUps[tutoIdx].Idx);
    }
    /// <summary>
    /// チュートリアル → ボタン
    /// </summary>
    /// <param name="tutoIdx">0: HOWTIPLAY_INFO, 1:ENEMY_IFNO, 2: MINING_INFO</param>
    public void OnClickNextBtn(int tutoIdx) {
        Debug.Log("TutoM:: OnClickNextBtn()::");
        SM._.SfxPlay(SM.SFX.ClickSFX);
        int cttLen = TutoPopUps[tutoIdx].Contents.Length;
        int lastIdx = cttLen - 1;

        TutoPopUps[tutoIdx].Idx++;
        if(TutoPopUps[tutoIdx].Idx >= lastIdx)
            TutoPopUps[tutoIdx].Idx = lastIdx;

        TutoPopUps[tutoIdx].PageTxt.text = $"({TutoPopUps[tutoIdx].Idx + 1}/{cttLen})";

        //* Show Page
        for(int i = 0; i < cttLen; i++)
            TutoPopUps[tutoIdx].Contents[i].SetActive(i == TutoPopUps[tutoIdx].Idx);
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

    /// <summary>
    /// チュートリアルPOPUP 表示
    /// </summary>
    /// <param name="tutoIdx"></param>
    /// <param name="delay"></param> <summary>
    public void ShowTutoPopUp(int tutoIdx, int pageIdx, float delay = 0) {
        if(GM._) {
            Debug.Log($"ShowTutoPopUp():: GM:: Pause");
            GM._.gui.Pause();
        }

        int cttLen = TutoPopUps[tutoIdx].Contents.Length;

        SM._.SfxPlay(SM.SFX.ItemPickSFX, delay);
        TutoPopUps[tutoIdx].WindowObj.SetActive(true);

        TutoPopUps[tutoIdx].Idx = pageIdx;
        TutoPopUps[tutoIdx].PageTxt.text = $"({pageIdx + 1}/{cttLen})";

        //* Show Page
        for(int i = 0; i < cttLen; i++)
            TutoPopUps[tutoIdx].Contents[i].SetActive(i == pageIdx);
    }
#endregion
}
