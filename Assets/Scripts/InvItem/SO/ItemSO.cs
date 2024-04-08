using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Inventory.Model {
    [Serializable]
    public class AbilityData {
        public AbilityType Type;
        public float Val;
        public float UpgradeVal;

        public AbilityData(AbilityType type, float val, float upgVal) {
            Type = type;
            Val = val;
            UpgradeVal = upgVal;
        }
    }
    [CreateAssetMenu]
    public class ItemSO : ScriptableObject {
        [field: SerializeField] public bool IsStackable { get; set; }
        public int ID => GetInstanceID();
        [field: SerializeField] public int MaxStackSize {get; set;} = 1;
        [field: SerializeField] public Enum.ItemType Type {get; private set;}
        [field: SerializeField] public Enum.Grade Grade {get; private set;}
        [field: SerializeField] public string Name {get; set;}
        [field: SerializeField] [field: TextArea] public string Description {get; set;}
        [field: SerializeField] public Sprite ItemImg {get; set;}

        [field: SerializeField] public AbilityData[] Abilities {get; set;}

        public void SetRelicAbility() {
            if(Type == Enum.ItemType.Relic) {
                Debug.Log($"ItemSO:: SetRelicAbility():: Type= {Type}");
                Abilities = new AbilityData[] {
                    new AbilityData(AbilityType.ClearEtc, 0.1f, 0.1f)
                };
            }
        }
    }
}



