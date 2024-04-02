using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour {
    [SerializeField] private InventoryUIManager ivm;
    [field:SerializeField] public int InventorySize {get; set;} = 10;

    void Start() {
        ivm.InitInventoryUI(InventorySize);
    }
}
