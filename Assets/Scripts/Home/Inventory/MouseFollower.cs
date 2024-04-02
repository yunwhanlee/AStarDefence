using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseFollower : MonoBehaviour {
    [field:SerializeField] private Canvas canvas;
    [field:SerializeField] private InventoryItem Item;

    void Awake() {
        canvas = transform.root.GetComponent<Canvas>();
        Item = GetComponentInChildren<InventoryItem>();
    }

    public void SetData(Sprite spr, int Val) {
        Item.SetData(spr, Val);
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
