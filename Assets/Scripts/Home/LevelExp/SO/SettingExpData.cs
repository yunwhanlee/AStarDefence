using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ExpData {
    [field:SerializeField] public int Max {get; set;}
}

[CreateAssetMenu(fileName = "ExpSO", menuName = "Scriptable Object/Setting ExpData")]
public class SettingExpData : ScriptableObject {
    [field:SerializeField] public int Origin {get; private set;}
    [field:SerializeField] public int Deviation {get; private set;}
    [field:SerializeField] public float StandardDeviationIncRate {get; private set;}

    [field:SerializeField] public ExpData[] Datas {get; private set;}
}
