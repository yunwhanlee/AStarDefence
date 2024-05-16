using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Inventory.Model;
using TMPro;
using UnityEngine;

/// <summary>
//* マイレージリワードUIマネジャークラス
/// </summary>
public class MileageRewardUIManager : MonoBehaviour {
    [field:Header("ICON AT SHOP")]
    [field:SerializeField] public GameObject IconAlertRedDot {get; private set;}

    [field:Header("POINT FROM DB")]
    [field:SerializeField] public int MileagePoint {
        get => DM._.DB.StatusDB.Mileage;
        set {
            DM._.DB.StatusDB.Mileage = value;
            MileagePointTxt.text = $"{value}";
        }
    }
    
    [field:Header("POPUP ELEMENTS")]
    [field:SerializeField] public RwdBubbleDt[] RwdBubbleDts {get; private set;} // データ
    [field:SerializeField] public GameObject WindowObj {get; private set;}
    [field:SerializeField] public TMP_Text MileagePointTxt {get; private set;}
    [field:SerializeField] public Transform BubbleGroupTf {get; private set;}

    [field:SerializeField] public GameObject RwdItemBubblePf {get; private set;}
    private BonusRwdBubbleUI[] RwdBubbleUIs; //* Prefabで生成した💭(吹き出し)リスト

    void Start() {
        RwdBubbleUIs = new BonusRwdBubbleUI[RwdBubbleDts.Length];
        MileagePointTxt.text = $"{MileagePoint}";
        CreateBubbleUI();
        UpdateBubbleStatusUI();
    }

#region FUNC
    /// <summary>
    /// リワードBubbleUI生成
    /// </summary>
    private void CreateBubbleUI() {
        for(int i = 0; i < RwdBubbleDts.Length; i++) {
            RwdBubbleDt bubbleDt = RwdBubbleDts[i];
            ItemSO itemDt = bubbleDt.ItemDt;

            //* 生成
            RwdBubbleUIs[i] = Instantiate(RwdItemBubblePf, BubbleGroupTf).GetComponent<BonusRwdBubbleUI>();

            //* 初期化 *//
            // アイコン 画像
            RwdBubbleUIs[i].ItemIconImg.sprite = itemDt.ItemImg;

            // タイプ名
            string typeName = (itemDt.Type == Enum.ItemType.Weapon)? "무기"
                : (itemDt.Type == Enum.ItemType.Shoes)? "신발"
                : (itemDt.Type == Enum.ItemType.Ring)? "반지"
                :  "유물";

            // 等級色
            Color gradeClr = (itemDt.Grade == Enum.Grade.Unique)? HM._.ivm.GradeClrs[(int)Enum.Grade.Unique]
                : (itemDt.Grade == Enum.Grade.Legend)? HM._.ivm.GradeClrs[(int)Enum.Grade.Legend]
                : (itemDt.Grade == Enum.Grade.Myth)? HM._.ivm.GradeClrs[(int)Enum.Grade.Myth]
                : (itemDt.Grade == Enum.Grade.Prime)? HM._.ivm.GradeClrs[(int)Enum.Grade.Prime]
                : Color.white; // null

            // アンロック
            RwdBubbleStatus status = (MileagePoint >= bubbleDt.UnlockCnt)? RwdBubbleStatus.Unlocked : RwdBubbleStatus.Locked;

            // 適用
            RwdBubbleUIs[i].SetData(i, typeName, 1, bubbleDt.UnlockCnt);
            RwdBubbleUIs[i].SetUI(gradeClr);

            //! C#의 람다 표현식과 클로저의 작동 방식 때문에 발생하는 전형적인 문제입니다. for 루프 내에서 람다 표현식을 사용하여 이벤트 핸들러를 등록할 때, 람다 표현식이 참조하는 변수 i가 루프가 끝날 때의 최종 값만을 참조
            int copyIdx = i; // 새로운 변수에 현재의 i 값을 저장
            RwdBubbleUIs[i].Btn.onClick.AddListener(() => OnClickRewardBubbleBtn(copyIdx));
        }
    }

    /// <summary>
    /// 状態によって UI 最新化
    /// </summary>
    private void UpdateBubbleStatusUI() {
        for(int i = 0; i < RwdBubbleDts.Length; i++) {
            BonusRwdBubbleUI bubbleUI = RwdBubbleUIs[i];
            if(bubbleUI.Status == RwdBubbleStatus.Locked && MileagePoint >= bubbleUI.UnlockCnt) {
                RwdBubbleUIs[i].SetStatusUI(RwdBubbleStatus.Unlocked);
            }
            else {
                RwdBubbleUIs[i].SetStatusUI();
            }
        }
        UpdateAlertRedDot();
    }
    /// <summary>
    /// アンロックしたリワード項目があったら、お知らせの🔴を表示
    /// </summary>
    private void UpdateAlertRedDot() {
        bool isExistUnlock = Array.Exists(RwdBubbleUIs, bubbleUI => bubbleUI.Status == RwdBubbleStatus.Unlocked);
        IconAlertRedDot.SetActive(isExistUnlock);
    }
    private void SetPopUpUI(bool isShow) {
        HM._.hui.SetTopNavOrderInLayer(isLocateFront: !isShow);
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(isShow);
        UpdateBubbleStatusUI();
    }
#endregion

#region EVENT
    public void OnClickDebugMileagePointUp() { //! DEBUG
        SM._.SfxPlay(SM.SFX.UpgradeSFX);
        MileagePoint += 50;
    }
    public void OnClickRewardBubbleBtn(int idx) {
        Debug.Log($"OnClickRewardBubbleBtn({idx}):: Status= {RwdBubbleUIs[idx].Status}");

        if(RwdBubbleUIs[idx].Status == RwdBubbleStatus.Accepted) {
            HM._.hui.ShowMsgError("이미 보상수령을 완료했습니다.");
            return;
        }
        else if(MileagePoint < RwdBubbleUIs[idx].UnlockCnt) {
            HM._.hui.ShowMsgError("천장 마일리지가 부족합니다!");
            return;
        }

        //* Equipアイテム リワード (異物と分けて処理)
        List<RewardItem> rewardList = new List<RewardItem>();
        if(RwdBubbleDts[idx].ItemDt.Type == Enum.ItemType.Relic) {
            ItemSO relicDt = RwdBubbleDts[idx].ItemDt;
            AbilityType[] relicAbilities = HM._.ivCtrl.InventoryData.CheckRelicAbilitiesData(relicDt);
            rewardList.Add(new (relicDt, quantity: 1, relicAbilities));
        }
        else {
            rewardList.Add(new (RwdBubbleDts[idx].ItemDt));
        }
        HM._.rwlm.ShowReward(rewardList);

        //* Accept状態に変更
        RwdBubbleUIs[idx].SetStatusUI(RwdBubbleStatus.Accepted);
    }
    public void OnClickMileageIconAtShop() {
        SetPopUpUI(isShow: true);
    }
    public void OnClickCloseBtn() {
        SetPopUpUI(isShow: false);
    }
#endregion
}