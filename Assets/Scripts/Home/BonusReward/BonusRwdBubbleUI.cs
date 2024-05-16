using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum RwdBubbleStatus {
    Locked, Unlocked, Accepted, NULL
}

public enum BubbleType {
    Mileage, Fame
}

public class BonusRwdBubbleUI : MonoBehaviour {
    [field: SerializeField] public Sprite PointMarkBlueSpr {get; set;}
    [field: SerializeField] public Sprite PointMarkYellowSpr {get; set;}
    [field: SerializeField] public Color HalfTransparentClr {get; set;}
    [field: SerializeField] public Color OriginClr {get; set;}
    [field: SerializeField] public Color BubbleBlueClr {get; set;}
    [field: SerializeField] public Color UnlockGrayFrameClr {get; set;}
    [field: SerializeField] public Color UnlockOrangeFrameClr {get; set;}

    [field: SerializeField] public string Name {get; set;}
    [field: SerializeField] public BubbleType Type {get; set;}
    [field: SerializeField] public int Id {get; set;}
    [field: SerializeField] public int Quantity {get; set;}
    [field: SerializeField] public int UnlockCnt {get; set;}
    [field: SerializeField] public Button Btn {get; set;}
    [field: SerializeField] public RwdBubbleStatus Status {
        get {
            if(Type == BubbleType.Mileage)
                return DM._.DB.MileageRewardDB.Statuses[Id];
            else if(Type == BubbleType.Fame)
                return DM._.DB.FameRewardDB.Statuses[Id];
            else
                return RwdBubbleStatus.NULL;
        } 
        set {
            if(Type == BubbleType.Mileage)
                DM._.DB.MileageRewardDB.Statuses[Id] = value;
            else if(Type == BubbleType.Fame)
                DM._.DB.FameRewardDB.Statuses[Id] = value;
        }
    }
    [field: SerializeField] public Image PointMarkImg {get; set;}
    [field: SerializeField] public Image UnlockCntFrameImg {get; set;}
    [field: SerializeField] public Image BubbleFrameImg {get; set;}
    [field: SerializeField] public Image BubbleArrowImg {get; set;}
    [field: SerializeField] public Image ItemIconImg {get; set;}
    [field: SerializeField] public DOTweenAnimation DOTAnim {get; set;}
    [field: SerializeField] public TMP_Text ItemNameTxt {get; set;}
    [field: SerializeField] public TMP_Text UnlockCntTxt {get; set;}
    [field: SerializeField] public GameObject CheckMark {get; set;}
    [field: SerializeField] public GameObject LockMark {get; set;}

    public void SetData(int id, string name, int quantity, int unlockCnt, BubbleType type) {
        Id = id;
        Name = name;
        Quantity = quantity;
        UnlockCnt = unlockCnt;
        Type = type;
    }

    private void SetUIStyle(Color txtClr, Sprite pointMarkSpr, Color unlockFrameClr) {
        ItemIconImg.color = txtClr;
        ItemNameTxt.color = txtClr;
        PointMarkImg.sprite = pointMarkSpr;
        UnlockCntFrameImg.color = unlockFrameClr;
    }

    public void SetUI() {
        SetUI(Color.white);
    }
    public void SetUI(Color gradeClr) {
        BubbleFrameImg.color = gradeClr;
        BubbleArrowImg.color = gradeClr;
        ItemNameTxt.text = $"{Name}";
        UnlockCntTxt.text = $"{UnlockCnt}";
    }

    public void SetStatusUI(RwdBubbleStatus status = RwdBubbleStatus.NULL) {
        Debug.Log($"SetStatusUI(status= {Status}):: ");
        //* パラメーターで状態を渡したら、状態 変更
        if(status != RwdBubbleStatus.NULL)
            Status = status;

        //* 状態によって
        if(Status == RwdBubbleStatus.Locked) {
            SetUIStyle(OriginClr, PointMarkBlueSpr, UnlockGrayFrameClr);
            CheckMark.SetActive(false);
            LockMark.SetActive(true);
        }
        else if(Status == RwdBubbleStatus.Unlocked) {
            SetUIStyle(OriginClr, PointMarkYellowSpr, UnlockOrangeFrameClr);
            CheckMark.SetActive(false);
            LockMark.SetActive(false);
            DOTAnim.DOPlay();
        }
        else if(Status == RwdBubbleStatus.Accepted) {
            SetUIStyle(HalfTransparentClr, PointMarkYellowSpr, UnlockOrangeFrameClr);
            CheckMark.SetActive(true);
            LockMark.SetActive(false);
            DOTAnim.DOComplete(); //* 初期値に戻す(スケール１)
        }
    }
}
