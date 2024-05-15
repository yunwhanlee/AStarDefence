using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Inventory.Model;
using TMPro;
using UnityEngine;

[System.Serializable]
public class RwdBubbleDt {
    [field:SerializeField] public int UnlockCnt {get; set;}
    [field:SerializeField] public ItemSO ItemDt {get; set;}
}

public class MileageRewardUIManager : MonoBehaviour {
    [field:SerializeField] public int MileagePoint {
        get => DM._.DB.StatusDB.Mileage;
        set {
            DM._.DB.StatusDB.Mileage = value;
            MileagePointTxt.text = $"{value}";
        }
    }

    [field:SerializeField] public RwdBubbleDt[] RwdBubbleDts {get; private set;}

    [field:SerializeField] public GameObject WindowObj {get; private set;}
    [field:SerializeField] public TMP_Text MileagePointTxt {get; private set;}
    [field:SerializeField] public Transform BubbleGroupTf {get; private set;}
    [field:SerializeField] public GameObject RwdItemBubblePf {get; private set;}

    private BonusRwdBubbleUI[] RwdBubbleUIs;

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
            //* 生成
            RwdBubbleDt bubbleDt = RwdBubbleDts[i];
            RwdBubbleUIs[i] = Instantiate(RwdItemBubblePf, BubbleGroupTf).GetComponent<BonusRwdBubbleUI>();

            //* 初期化
            // アイコン 画像
            RwdBubbleUIs[i].ItemIconImg.sprite = bubbleDt.ItemDt.ItemImg;
            
            // タイプ名
            string typeName = (bubbleDt.ItemDt.Type == Enum.ItemType.Weapon)? "무기"
                : (bubbleDt.ItemDt.Type == Enum.ItemType.Shoes)? "신발"
                : (bubbleDt.ItemDt.Type == Enum.ItemType.Ring)? "반지"
                :  "유물";

            // 等級色
            Enum.Grade itemGrade = bubbleDt.ItemDt.Grade;
            Color[] gradeClrs = HM._.ivm.GradeClrs;
            Color gradeClr = (itemGrade == Enum.Grade.Unique)? gradeClrs[(int)Enum.Grade.Unique]
                : (itemGrade == Enum.Grade.Legend)? gradeClrs[(int)Enum.Grade.Legend]
                : (itemGrade == Enum.Grade.Myth)? gradeClrs[(int)Enum.Grade.Myth]
                : (itemGrade == Enum.Grade.Prime)? gradeClrs[(int)Enum.Grade.Prime]
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
            RwdBubbleUIs[i].SetStatusUI();
        }
    }
#endregion

#region EVENT
    public void OnClickRewardBubbleBtn(int idx) {
        Debug.Log($"OnClickRewardBubbleBtn({idx}):: Status= {RwdBubbleUIs[idx].Status}");

        if(RwdBubbleUIs[idx].Status == RwdBubbleStatus.Accepted) {
            HM._.hui.ShowMsgError("이미 보상수령을 완료했습니다.");
            return;
        }

        if(MileagePoint < RwdBubbleUIs[idx].UnlockCnt) {
            HM._.hui.ShowMsgError("천장 마일리지가 부족합니다!");
            return;
        }

        //* リワード
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
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(true);
        UpdateBubbleStatusUI();
    }
    public void OnClickCloseBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(false);
    }
#endregion
}