using System.Collections;
using System.Collections.Generic;
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

    void Start() {
        MileagePointTxt.text = $"{MileagePoint}";

        //* リワードBubbleUI初期化
        UpdateUI();
    }

#region FUNC
    private void UpdateUI() {
        //* リワードBubbleUI初期化
        int i = 0;
        foreach (RwdBubbleDt bubbleDt in RwdBubbleDts) {
            BonusRwdBubbleUI  rwdBubbleUI = Instantiate(RwdItemBubblePf, BubbleGroupTf).GetComponent<BonusRwdBubbleUI>();
            rwdBubbleUI.ItemIconImg.sprite = bubbleDt.ItemDt.ItemImg;

            //* タイプ名
            string typeName = (bubbleDt.ItemDt.Type == Enum.ItemType.Weapon)? "무기"
                : (bubbleDt.ItemDt.Type == Enum.ItemType.Shoes)? "신발"
                : (bubbleDt.ItemDt.Type == Enum.ItemType.Ring)? "반지"
                :  "유물";

            //* 等級色
            Enum.Grade itemGrade = bubbleDt.ItemDt.Grade;
            Color[] gradeClrs = HM._.ivm.GradeClrs;
            Color gradeClr = (itemGrade == Enum.Grade.Unique)? gradeClrs[(int)Enum.Grade.Unique]
                : (itemGrade == Enum.Grade.Legend)? gradeClrs[(int)Enum.Grade.Legend]
                : (itemGrade == Enum.Grade.Myth)? gradeClrs[(int)Enum.Grade.Myth]
                : (itemGrade == Enum.Grade.Prime)? gradeClrs[(int)Enum.Grade.Prime]
                : Color.white; // null

            //* アンロック
            RwdBubbleStatus status = (MileagePoint >= bubbleDt.UnlockCnt)? RwdBubbleStatus.Unlocked : RwdBubbleStatus.Locked;

            //* 設定
            rwdBubbleUI.SetData(i, typeName, 1, bubbleDt.UnlockCnt, status);
            rwdBubbleUI.SetUI(gradeClr);
            i++;
        }
    }
#endregion

#region EVENT
    public void OnClickMileageIconAtShop() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(true);
        UpdateUI();
    }
    public void OnClickCloseBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(false);
    }
#endregion
}