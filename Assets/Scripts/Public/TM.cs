using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TM : MonoBehaviour {
    static public TM _;

    [field: SerializeField] public int HowToPlayIdx {get; private set;}

    [field: SerializeField] public GameObject TutoWindowObj {get; private set;}
    [field: SerializeField] public TMP_Text PageTxt {get; private set;}
    [field: SerializeField] public TMP_Text SkipBtnTxt {get; private set;}
    [field: SerializeField] public GameObject[] HowToPlayObjs {get; private set;}


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
        SM._.SfxPlay(SM.SFX.ClickSFX);
        TutoWindowObj.SetActive(false);
    }
    public void OnClickPreviousBtn() {
        SkipBtnTxt.text = "스킵하기";
        HowToPlayIdx--;
        if(HowToPlayIdx < 0) {
            HowToPlayIdx = 0;
        }
        PageTxt.text = $"( {HowToPlayIdx + 1} / {HowToPlayObjs.Length} )";

        for(int i = 0; i < HowToPlayObjs.Length; i++)
            HowToPlayObjs[i].SetActive(i == HowToPlayIdx);
    }
    public void OnClickNextBtn() {
        int lastIdx = HowToPlayObjs.Length - 1;
        SkipBtnTxt.text = "스킵하기";
        HowToPlayIdx++;
        if(HowToPlayIdx >= lastIdx) {
            HowToPlayIdx = lastIdx;
            SkipBtnTxt.text = "좋았어. 가봅시다!";
        }
        PageTxt.text = $"( {HowToPlayIdx + 1} / {HowToPlayObjs.Length} )";

        for(int i = 0; i < HowToPlayObjs.Length; i++)
            HowToPlayObjs[i].SetActive(i == HowToPlayIdx);
    }
#endregion

#region FUNC

#endregion
}
