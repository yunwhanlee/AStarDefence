using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum RwdBubbleStatus {
    Locked, Unlocked, Accepted, NULL
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
    [field: SerializeField] public int Id {get; set;}
    [field: SerializeField] public int Quantity {get; set;}
    [field: SerializeField] public int UnlockCnt {get; set;}
    [field: SerializeField] public Button Btn {get; set;}
    [field: SerializeField] public RwdBubbleStatus Status {
        get => DM._.DB.MileageRewardDB.Statuses[Id];
        set {
            DM._.DB.MileageRewardDB.Statuses[Id] = value;
        }
    }
    [field: SerializeField] public Image PointMarkImg {get; set;}
    [field: SerializeField] public Image UnlockCntFrameImg {get; set;}
    [field: SerializeField] public Image BubbleFrameImg {get; set;}
    [field: SerializeField] public Image BubbleArrowImg {get; set;}
    [field: SerializeField] public Image ItemIconImg {get; set;}
    [field: SerializeField] public DOTweenAnimation ItemIconDOTAnim {get; set;}
    [field: SerializeField] public TMP_Text ItemNameTxt {get; set;}
    [field: SerializeField] public TMP_Text UnlockCntTxt {get; set;}
    [field: SerializeField] public GameObject CheckMark {get; set;}
    [field: SerializeField] public GameObject LockMark {get; set;}

    public void SetData(int id, string name, int quantity, int unlockCnt) {
        Id = id;
        Name = name;
        Quantity = quantity;
        UnlockCnt = unlockCnt;
    }

    private void SetUIStyle(Color txtClr, Sprite pointMarkSpr, Color unlockFrameClr) {
        ItemIconImg.color = txtClr;
        ItemNameTxt.color = txtClr;
        PointMarkImg.sprite = pointMarkSpr;
        UnlockCntFrameImg.color = unlockFrameClr;
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
            ItemIconDOTAnim.DOPlay();
        }
        else if(Status == RwdBubbleStatus.Accepted) {
            SetUIStyle(HalfTransparentClr, PointMarkYellowSpr, UnlockOrangeFrameClr);
            CheckMark.SetActive(true);
            LockMark.SetActive(false);
            ItemIconDOTAnim.DOKill();
        }
    }
}
