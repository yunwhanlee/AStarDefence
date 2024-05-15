using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;

[System.Serializable]
public class RwdBubbleDt {
    [field:SerializeField] public int UnlockCnt {get; set;}
    [field:SerializeField] public ItemSO ItemDt {get; set;}
}

public class MileageRewardUIManager : MonoBehaviour {
    [field:SerializeField] public RwdBubbleDt[] RwdBubbleDts {get; private set;}

    [field:SerializeField] public Transform BubbleGroupTf {get; private set;}
    [field:SerializeField] public GameObject RwdItemBubblePf {get; private set;}

    void Start() {
        
        int i = 0;
        foreach (var bubbleDt in RwdBubbleDts) {
            var rwdBubbleUI = Instantiate(RwdItemBubblePf, BubbleGroupTf).GetComponent<BonusRwdBubbleUI>();
            rwdBubbleUI.ItemIconImg.sprite = bubbleDt.ItemDt.ItemImg;

            //* タイプ名
            string typeName = (bubbleDt.ItemDt.Type == Enum.ItemType.Weapon)? "무기"
                : (bubbleDt.ItemDt.Type == Enum.ItemType.Shoes)? "신발"
                : (bubbleDt.ItemDt.Type == Enum.ItemType.Ring)? "반지"
                :  "유물";

            //* 設定
            rwdBubbleUI.SetData(i, typeName, 1, bubbleDt.UnlockCnt);
            rwdBubbleUI.SetUI();

            i++;
        }

    }

}
