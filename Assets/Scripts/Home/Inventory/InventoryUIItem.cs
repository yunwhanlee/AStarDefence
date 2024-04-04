using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Inventory.UI
{
    public class InventoryUIItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler {
        private Coroutine CorPushTimeID = null;
        private bool IsShortPush = false;

        [field: Header("ELEMENT")]
        [field:SerializeField] public Enum.ItemType Type {get; set;}
        [field:SerializeField] public Image TypeBgImg {get; set;}
        [field:SerializeField] public Image TypeIconImg {get; set;}
        [field:SerializeField] public Image BgImg {get; set;}
        [field:SerializeField] public Image ItemImg {get; set;}
        [field:SerializeField] public TMP_Text ValTxt {get; set;}
        [field:SerializeField] public Image BorderImg {get; set;}
        public event Action<InventoryUIItem> OnItemClicked, 
            OnItemDroppedOn, 
            OnItemBeginDrag, 
            OnItemEndDrag,
            OnItemClickShortly;

        bool IsEmpty = false;

        void Awake() {
            ResetData();
            Deselect();
        }
        public void ResetData() {
            ItemImg.gameObject.SetActive(false);
            IsEmpty = true;
        }
        public void Deselect() {
            BorderImg.enabled = false;
        }

        /// <summary>
        /// アイテムのデータ設定
        /// </summary>
        public void SetData(Enum.ItemType type, Enum.Grade grade, Sprite spr, int val) {
            if(grade == Enum.Grade.None) {
                TypeBgImg.enabled = false;
                TypeIconImg.sprite = HM._.ivm.NoneSpr;
                BgImg.sprite = HM._.ivm.NoneBgSpr;
            }
            else {
                TypeBgImg.enabled = true;
                TypeBgImg.color = HM._.ivm.GradeClrs[(int)grade];
                TypeIconImg.sprite = HM._.ivm.TypeSprs[(int)type];
                BgImg.sprite = HM._.ivm.GradeBgSprs[(int)grade];
            }
            ItemImg.gameObject.SetActive(true);
            ItemImg.sprite = spr;
            ValTxt.text = (type == Enum.ItemType.Etc)? $"{val}" : $"Lv.{val}";
            IsEmpty = false;
        }

        public void Select() {
            if(!IsShortPush) return;
            Debug.Log("Select():: BorderImg.Color= <color=red>Red</color>");
            BorderImg.color = Color.red;
            BorderImg.enabled = true;
        }

        public void Draggable() => BorderImg.color = Color.green;

        private IEnumerator CoCheckPushTime() {
            IsShortPush = true;
            yield return Util.Time0_3;
            IsShortPush = false;
            Draggable();
        }

        public void OnPointerDown(PointerEventData eventData) { // マウス押した
            if(IsEmpty) return;
            CorPushTimeID = StartCoroutine(CoCheckPushTime());
            OnItemClicked?.Invoke(this);
        }

        public void OnPointerUp(PointerEventData eventData) { // マウス離れた
            Debug.Log($"OnPointerUp():: IsShortPush= {IsShortPush}");
            if(IsShortPush) {
                StopCoroutine(CorPushTimeID);
                OnItemClickShortly?.Invoke(this);
            }
        }

        public void OnBeginDrag(PointerEventData eventData) {
            Debug.Log("OnBeginDrag():: IsShortPush= " + IsShortPush);
            if(IsEmpty) return;
            if(IsShortPush) return;
            OnItemBeginDrag?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData) {
            Debug.Log("OnEndDrag():: IsShortPush= " + IsShortPush);
            if(IsShortPush) return;
            OnItemEndDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData) {
            if(IsShortPush) return;
            Debug.Log("OnDrop():: IsShortPush= " + IsShortPush);
            OnItemDroppedOn?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData) {
            Debug.Log("OnDrag():: IsShortPush= " + IsShortPush);
            if(IsShortPush) return;
        }
    }
}
