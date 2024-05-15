using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusRwdBubbleUI : MonoBehaviour {
    [field: SerializeField] public Sprite PointMarkBlueSpr {get; set;}
    [field: SerializeField] public Sprite PointMarkYellowSpr {get; set;}
    [field: SerializeField] public Color HalfTransparentClr {get; set;}
    [field: SerializeField] public Color originClr {get; set;}
    [field: SerializeField] public Color BubbleBlueClr {get; set;}
    [field: SerializeField] public Color UnlockGrayFrameClr {get; set;}
    [field: SerializeField] public Color UnlockOrangeFrameClr {get; set;}

    [field: SerializeField] public string Name {get; set;}
    [field: SerializeField] public int Id {get; set;}
    [field: SerializeField] public int Quantity {get; set;}
    [field: SerializeField] public int UnlockCnt {get; set;}
    [field: SerializeField] public Image PointMarkImg {get; set;}
    [field: SerializeField] public Image UnlockCntFrameImg {get; set;}
    [field: SerializeField] public Image BubbleFrameImg {get; set;}
    [field: SerializeField] public Image BubbleArrowImg {get; set;}
    [field: SerializeField] public Image ItemIconImg {get; set;}
    [field: SerializeField] public TMP_Text ItemNameTxt {get; set;}
    [field: SerializeField] public TMP_Text UnlockCntTxt {get; set;}
    [field: SerializeField] public Image CheckMark {get; set;}
    [field: SerializeField] public Image LockMark {get; set;}

    private void SetUIStyle(Color bubbleClr, Color txtClr, Sprite pointMarkSpr, Color unlockFrameClr) {
        BubbleFrameImg.color = bubbleClr;
        BubbleArrowImg.color = bubbleClr;
        ItemIconImg.color = txtClr;
        ItemNameTxt.color = txtClr;
        PointMarkImg.sprite = pointMarkSpr;
        UnlockCntFrameImg.color = unlockFrameClr;
    }

    public void SetData(int id, string name, int quantity, int unlockCnt) {
        Id = id;
        Name = name;
        Quantity = quantity;
        UnlockCnt = unlockCnt;
    }

    public void SetUI() {
        ItemNameTxt.text = $"{Name}";
        UnlockCntTxt.text = $"{UnlockCnt}";

        //* Locked
        SetUIStyle(BubbleBlueClr, HalfTransparentClr, PointMarkBlueSpr, UnlockGrayFrameClr);

        //* Unlocked
        SetUIStyle(originClr, originClr, PointMarkYellowSpr, UnlockOrangeFrameClr);

        //* Accepted
    }
}
