using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiningTimeUIManager : MonoBehaviour {
    [field: SerializeField] public Slider Slider {get; set;}
    [field: SerializeField] public TextMeshProUGUI SliderTxt {get; set;}
}
