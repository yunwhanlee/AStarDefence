using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Inventory.Model;
using TMPro;
using UnityEngine;

/// <summary>
//* ボーナスリワードのアイテム表示💭(吹き出し)データ : 「マイレージ」と「名声」システムに使う
/// </summary>
[Serializable]
public class RwdBubbleDt {
    [field:SerializeField] public int UnlockCnt {get; set;}
    [field:SerializeField] public ItemSO ItemDt {get; set;}
    [field:SerializeField] public int Quantity {get; set;}
}