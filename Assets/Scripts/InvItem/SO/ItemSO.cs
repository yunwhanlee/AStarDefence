using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
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
    }
}



