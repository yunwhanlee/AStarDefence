using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TowerCardUI {
    [field:SerializeField] public Button[] Buttons {get; set;}
    [field:SerializeField] public TextMeshProUGUI[] LvTxts {get; set;}
    [field:SerializeField] public TextMeshProUGUI[] PriceTxts {get; set;}
}
