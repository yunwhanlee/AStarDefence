using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;

[CreateAssetMenu]
public class RewardItemSO : ScriptableObject {
    [field: Header("ItemSO LIST")]
    [field: SerializeField] public ItemSO[] EtcConsumableDatas {get; private set;}
    [field: SerializeField] public ItemSO[] EtcNoShowInvDatas {get; private set;}
    [field: SerializeField] public ItemSO[] WeaponDatas {get; private set;}
    [field: SerializeField] public ItemSO[] ShoesDatas {get; private set;}
    [field: SerializeField] public ItemSO[] RingDatas {get; private set;}
    [field: SerializeField] public ItemSO[] RelicDatas {get; private set;}
}
