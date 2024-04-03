using System.Collections;
using System.Collections.Generic;
using Inventory.UI;
using UnityEngine;

public class MouseFollower : MonoBehaviour {
    [field:SerializeField] private Canvas canvas;
    [field:SerializeField] private InventoryUIItem Item;

    void Awake() {
        canvas = transform.root.GetComponent<Canvas>();
        Item = GetComponentInChildren<InventoryUIItem>();
    }

    public void SetData(Enum.ItemType type, Enum.Grade grade, Sprite spr, int Val) {
        Item.SetData(type, grade, spr, Val);
    }

    void Update() {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            Input.mousePosition,
            canvas.worldCamera,
            out pos
        );
        transform.position = canvas.transform.TransformPoint(pos);
    }

    public void Toggle(bool isActive) {
        Debug.Log($"Item Toggled {isActive}");
        gameObject.SetActive(isActive);
    }
}
