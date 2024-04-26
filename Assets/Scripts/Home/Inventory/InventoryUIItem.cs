using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AssetKits.ParticleImage;

namespace Inventory.UI
{
    /// <summary>
    /// インベントリスロットのアイテム
    /// </summary>
    public class InventoryUIItem : MonoBehaviour //, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler 
    {
        // private Coroutine CorPushTimeID = null;
        private bool IsShortPush = false;

        [field: Header("ELEMENT")]
        [field:SerializeField] public Enum.ItemType Type {get; set;}
        [field:SerializeField] public Image TypeBgImg {get; set;}
        [field:SerializeField] public Image TypeIconImg {get; set;}
        [field:SerializeField] public Image BgImg {get; set;}
        [field:SerializeField] public Image LightImg {get; set;}
        [field:SerializeField] public Image ItemImg {get; set;}
        [field:SerializeField] public TMP_Text QuantityTxt {get; set;}
        [field:SerializeField] public TMP_Text LvTxt {get; set;}
        [field:SerializeField] public Image BorderImg {get; set;}
        [field:SerializeField] public GameObject AlertRedDot {get; set;}
        [field:SerializeField] public GameObject EquipDim {get; set;} 
        [field:SerializeField] public ParticleImage ItemImgScaleUIEF {get; set;}
        [field:SerializeField] public ParticleImage WhiteDimScaleUIEF {get; set;}
        [field:SerializeField] public ParticleImage ShinyUIEF {get; set;}
        [field:SerializeField] public ParticleImage HighGradeSpawnUIEF {get; set;}
        [field:SerializeField] public ParticleImage HighGradeNiceUIEF {get; set;}
        public bool IsEmpty = false;
        public event Action<InventoryUIItem> OnItemClicked, 
            OnItemDroppedOn, 
            OnItemBeginDrag, 
            OnItemEndDrag,
            OnItemClickShortly;

        void Awake() {
            ResetUI();
            Deselect();

            //* HOMEシーンのみ
            if(HM._) {
                GetComponent<Button>().onClick.AddListener(() => {
                    Debug.Log("OnClick Item!");
                    OnItemClicked?.Invoke(this);
                    OnItemClickShortly?.Invoke(this);
                });

                HM._.ivEqu.UpdateAllEquipSlots();
                HM._.ivEqu.UpdateAllEquipAbilityData();
            }
        }

    #region EVENT
    #endregion
        public void ResetUI() {
            Debug.Log($"ResetUI():: TypeBgImg= {TypeBgImg}");
            Type = Enum.ItemType.Etc;
            TypeBgImg.enabled = false;
            TypeIconImg.enabled = false;
            LightImg.enabled = false;
            if(HM._) BgImg.sprite = HM._.ivm.NoneBgSpr;
            BgImg.color = Color.white;
            ItemImg.gameObject.SetActive(false);
            if(HM._) HM._.ivm.AutoMergeBtnAlertIcon.SetActive(false);

            //* Equipスロットは対応しない（そのそのオブジェクトが付いていない）
            if(AlertRedDot) AlertRedDot.SetActive(false);
            if(EquipDim) EquipDim.SetActive(false);

            IsEmpty = true;
            // IsNewAlert = false;
            QuantityTxt.text = "";
            LvTxt.text = "";
            ShinyUIEF.Stop();
        }
        public void Deselect() => BorderImg.enabled = false;

        public void PlayScaleUIEF(InventoryUIItem  invItemUI, Sprite itemSpr) {
            invItemUI.ItemImgScaleUIEF.sprite = itemSpr;
            invItemUI.ItemImgScaleUIEF.Play();
        }
        /// <summary>
        /// インベントリアイテムUIとデータ設定
        /// </summary>
        public void SetUI (Enum.ItemType type, Enum.Grade grade, Sprite spr, int quantity, int lv, AbilityType[] relicAbilities = null, bool isEquip = false, bool isNewAlert = false) {
            // Debug.Log($"SetUI(ItemImg.name={ItemImg.sprite.name}, type={type}, grade={grade})::");
            Type = type;
            QuantityTxt.text = $"{quantity}";

            //* その他アイテム
            if(type == Enum.ItemType.Etc) {
                Debug.Log($"SetUI(<color=white>type={type}</color>, ItemImg.name={ItemImg.sprite.name}, grade={grade})::");
                TypeBgImg.enabled = false;
                TypeIconImg.enabled = false;
                BgImg.sprite = HM._.ivm.NoneBgSpr;
                LightImg.enabled = false;
                BgImg.color = (grade == Enum.Grade.None)? Color.white : HM._.ivm.GradeClrs[ (int)grade];
                LvTxt.text = "";
                EquipDim.SetActive(false);
                
            }
            //* 装置アイテム
            else {
                Debug.Log($"SetUI(<color=yellow>type={type}</color>, ItemImg.name={ItemImg.sprite.name}, grade={grade})::");
                TypeBgImg.enabled = true;
                TypeIconImg.enabled = true;
                TypeBgImg.color = HM._.ivm.GradeClrs[(int)grade];
                TypeIconImg.sprite = HM._.ivm.TypeSprs[(int)type];
                BgImg.sprite = HM._.ivm.GradeBgSprs[(int)grade];
                LightImg.enabled = true;
                if(EquipDim) EquipDim.SetActive(isEquip); //* EquipスロットはEquipDimオブジェクトがないため、合うかif文でチェック
                if(quantity >= 10) {
                    QuantityTxt.text = $"<color=green>{quantity}</color>";
                    HM._.ivm.AutoMergeBtnAlertIcon.SetActive(true);
                }

                string lvStr = (type == Enum.ItemType.Relic && lv >= Config.RELIC_UPGRADE_MAX)
                    || (type != Enum.ItemType.Relic && lv >= Config.EQUIP_UPGRADE_MAX) ? "MAX" : lv.ToString();
                LvTxt.text = $"Lv.{lvStr}";

                if(grade >= Enum.Grade.Unique)
                    ShinyUIEF.Play();
                
            }

            IsEmpty = false;
            // IsNewAlert = true;
            if(AlertRedDot) AlertRedDot.SetActive(isNewAlert);
            ItemImg.gameObject.SetActive(true);
            ItemImg.sprite = spr;
        }

        public void Select() {
            if(!IsShortPush) return;
            Debug.Log("Select():: BorderImg.Color= <color=red>Red</color>");
            BorderImg.color = Color.red;
            BorderImg.enabled = true;
        }

        // public void Draggable() => BorderImg.color = Color.green;

        // private IEnumerator CoCheckPushTime() {
        //     IsShortPush = true;
        //     yield return Util.Time0_3;
        //     IsShortPush = false;
        //     Draggable();
        // }

        // public void OnPointerDown(PointerEventData eventData) { // マウス押した
        //     if(IsEmpty) return;
        //     CorPushTimeID = StartCoroutine(CoCheckPushTime());
        //     OnItemClicked?.Invoke(this);
        // }

        // public void OnPointerUp(PointerEventData eventData) { // マウス離れた
        //     Debug.Log($"OnPointerUp():: IsShortPush= {IsShortPush}");
        //     if(IsShortPush) {
        //         StopCoroutine(CorPushTimeID);
        //         OnItemClickShortly?.Invoke(this);
        //     }
        // }

        // public void OnBeginDrag(PointerEventData eventData) {
        //     Debug.Log("OnBeginDrag():: IsShortPush= " + IsShortPush);
        //     if(IsEmpty) return;
        //     if(IsShortPush) return;
        //     OnItemBeginDrag?.Invoke(this);
        // }

        // public void OnEndDrag(PointerEventData eventData) {
        //     Debug.Log("OnEndDrag():: IsShortPush= " + IsShortPush);
        //     if(IsShortPush) return;
        //     OnItemEndDrag?.Invoke(this);
        // }

        // public void OnDrop(PointerEventData eventData) {
        //     if(IsShortPush) return;
        //     Debug.Log("OnDrop():: IsShortPush= " + IsShortPush);
        //     OnItemDroppedOn?.Invoke(this);
        // }

        // public void OnDrag(PointerEventData eventData) {
        //     Debug.Log("OnDrag():: IsShortPush= " + IsShortPush);
        //     if(IsShortPush) return;
        // }
    }
}
