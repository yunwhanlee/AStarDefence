using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour {
    [field:SerializeField] public GameObject WindowObj {get; private set;}
    [field:SerializeField] public InventoryItem ItemPf {get; private set;}
    [field:SerializeField] public RectTransform Content {get; set;}
    List<InventoryItem> ItemList = new List<InventoryItem>();

#region EVENT
    public void OnClickInventoryIconBtn() {
        WindowObj.SetActive(true);
    }
    public void OnClickInventoryPopUpBackBtn() {
        WindowObj.SetActive(false);
    }
#endregion

#region FUNC
    public void InitInventoryUI(int invSize) {
        for(int i = 0; i < invSize; i++) {
            InventoryItem item = Instantiate(ItemPf, Content);
            ItemList.Add(item);
        }
    }
#endregion
}
