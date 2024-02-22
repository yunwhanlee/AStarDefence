using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerStateWindowManager : MonoBehaviour {
    [field: SerializeField] GameObject windowObj {get; set;}
    [field: SerializeField] Transform GradeLabelGroup {get; set;}
    [field: SerializeField] TextMeshProUGUI DmgTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI AtkSpeedTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI AtkRangeTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI SplashRangeTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI CritPerTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI CritDmgPerTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI SlowPerTxt {get; set;}
    [field: SerializeField] TextMeshProUGUI StunSecTxt {get; set;}

    void Start() {
        windowObj.SetActive(false);
    }

    void Update() {
        
    }
}
