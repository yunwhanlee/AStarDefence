using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusRwdBubbleUI : MonoBehaviour {
    [field: SerializeField] public Sprite PointMarkBlueSpr {get; set;}
    [field: SerializeField] public Sprite PointMarkSprYellow {get; set;}


    [field: SerializeField] public string Name {get; set;}
    [field: SerializeField] public int Id {get; set;}
    [field: SerializeField] public int Quantity {get; set;}
    [field: SerializeField] public string UnlockCnt {get; set;}
    [field: SerializeField] public Image PointMarkImg {get; set;}
    [field: SerializeField] public Image NeedCntFrameImg {get; set;}
    [field: SerializeField] public TMP_Text NeedCntTxt {get; set;}
    [field: SerializeField] public Image BubbleFrameImg {get; set;}
    [field: SerializeField] public Image BubbleArrowImg {get; set;}
    [field: SerializeField] public Image ItemIconImg {get; set;}
    [field: SerializeField] public TMP_Text ItemNameTxt {get; set;}
    [field: SerializeField] public Image CheckMark {get; set;}
    [field: SerializeField] public Image LockMark {get; set;}

}
