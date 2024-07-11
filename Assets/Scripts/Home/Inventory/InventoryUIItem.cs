using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AssetKits.ParticleImage;
using DG.Tweening;

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
        public bool IsEmpty = false;
        public bool IsEquipSlot;

        [field:SerializeField] public Enum.ItemType Type {get; set;} = Enum.ItemType.Empty;
        [field:SerializeField] public Image TypeBgImg {get; set;}
        [field:SerializeField] public Image TypeIconImg {get; set;}
        [field:SerializeField] public Image BgImg {get; set;}
        [field:SerializeField] public Image LightImg {get; set;}
        [field:SerializeField] public Image ItemImg {get; set;}
        [field:SerializeField] public TMP_Text QuantityTxt {get; set;}
        [field:SerializeField] public TMP_Text LvTxt {get; set;}
        [field:SerializeField] public Image BorderImg {get; set;}
        [field:SerializeField] public GameObject AlertRedDot {get; set;}
        [field:SerializeField] public GameObject AlertGreenDot {get; set;}
        [field:SerializeField] public GameObject EquipDim {get; set;}
        [field:SerializeField] public GameObject BonusRewardLabel {get; set;}
        [field:SerializeField] public GameObject DoubleRewardLabel {get; set;}
        [field:SerializeField] public DOTweenAnimation DOTAnim {get; set;}
        [field:SerializeField] public ParticleImage ItemImgScaleUIEF {get; set;}
        [field:SerializeField] public ParticleImage WhiteDimScaleUIEF {get; set;}
        [field:SerializeField] public ParticleImage ShinyUIEF {get; set;}
        [field:SerializeField] public ParticleImage HighGradeSpawnUIEF {get; set;}
        [field:SerializeField] public ParticleImage Twincle1UIEF {get; set;}
        [field:SerializeField] public ParticleImage Twincle2UIEF {get; set;}
        [field:SerializeField] public ParticleImage HighGradeRayUIEF {get; set;}
        [field:SerializeField] public ParticleImage HighGradeHandUIEF {get; set;}
        [field:SerializeField] public ParticleImage HighGradeBurstBlueUIEF {get; set;}
        [field:SerializeField] public ParticleImage HighGradeBurstYellowUIEF {get; set;}
        [field:SerializeField] public ParticleSystem LegendSpawnUIEF {get; set;}
        [field:SerializeField] public ParticleSystem MythSpawnUIEF {get; set;}
        [field:SerializeField] public ParticleSystem PrimeSpawnUIEF {get; set;}

        public event Action<InventoryUIItem> OnItemClicked;
        public event Action<InventoryUIItem> OnItemClickShortly;

        void Awake() {
            Debug.Log("AA InventoryUIItem:: Awake():: ResetUI");
            // ResetUI();

            if(HighGradeSpawnUIEF) HighGradeSpawnUIEF.enabled = false;
            if(Twincle1UIEF) Twincle1UIEF.enabled = false;
            if(Twincle2UIEF) Twincle2UIEF.enabled = false;

            // //* Equipスロットは対応しない
            if(AlertRedDot) AlertRedDot.SetActive(false);
            if(AlertGreenDot) AlertGreenDot.SetActive(false);
            if(EquipDim) EquipDim.SetActive(false);
            if(BonusRewardLabel) BonusRewardLabel.SetActive(false);
            if(DoubleRewardLabel) DoubleRewardLabel.SetActive(false);

            Deselect();

            //* HOMEシーンのみ
            if(HM._) {
                // スロットへクリックイベント登録
                GetComponent<Button>().onClick.AddListener(() => {
                    Debug.Log("OnClick Item!");
                    OnItemClicked?.Invoke(this);      // -> HandleItemSelection()
                    OnItemClickShortly?.Invoke(this); // -> HandleShowItemInfoPopUp()
                });

                // EQUIPスロット 最新化
                HM._.ivEqu.UpdateAllEquipSlots();
                HM._.ivEqu.UpdateAllEquipAbilityData();
            }
        }

    #region EVENT
    #endregion
        public void ResetUI() {
            Debug.Log($"<color=white>ResetUI():: ObjName= {name} Type= {Type}</color>");
            Type = Enum.ItemType.Empty; //* 固定したから要らない
            TypeBgImg.enabled = false;
            TypeIconImg.enabled = false;
            LightImg.enabled = false;
            ItemImg.gameObject.SetActive(false);
            if(HM._) BgImg.sprite = HM._.ivm.NoneBgSpr;
            BgImg.color = Color.white;
            if(HM._) HM._.ivm.AutoMergeBtnAlertIcon.SetActive(false);
            QuantityTxt.text = "";
            LvTxt.text = "";

            //* Equipスロットは対応しない（そのそのオブジェクトが付いていない）
            if(AlertRedDot) AlertRedDot.SetActive(false);
            if(AlertGreenDot) AlertGreenDot.SetActive(false);
            if(EquipDim) EquipDim.SetActive(false);
            if(BonusRewardLabel) BonusRewardLabel.SetActive(false);
            if(DoubleRewardLabel) DoubleRewardLabel.SetActive(false);

            IsEmpty = true;
            // IsNewAlert = false;
            
            ItemImgScaleUIEF.Stop();
            WhiteDimScaleUIEF.Stop();
            ShinyUIEF.Stop();

            if(HighGradeSpawnUIEF) HighGradeSpawnUIEF.enabled = false;
            if(Twincle1UIEF) Twincle1UIEF.enabled = false;
            if(Twincle2UIEF) Twincle2UIEF.enabled = false;

            if(HighGradeRayUIEF) HighGradeRayUIEF.Stop();
            if(HighGradeHandUIEF) HighGradeHandUIEF.Stop();
            if(HighGradeBurstBlueUIEF) HighGradeBurstBlueUIEF.Stop();
            if(HighGradeBurstYellowUIEF) HighGradeBurstYellowUIEF.Stop();
            if(LegendSpawnUIEF) LegendSpawnUIEF.Stop();
            if(MythSpawnUIEF) MythSpawnUIEF.Stop();
            if(PrimeSpawnUIEF) PrimeSpawnUIEF.Stop();
        }
        public void Deselect() => BorderImg.enabled = false;

        public void PlayScaleUIEF(InventoryUIItem  invItemUI, Sprite itemSpr) {
            invItemUI.ItemImgScaleUIEF.sprite = itemSpr;
            invItemUI.ItemImgScaleUIEF.Play();
        }
        /// <summary>
        /// インベントリアイテムUIとデータ設定
        /// </summary>
        public void SetUI(Enum.ItemType type, Enum.Grade grade, Sprite spr, int quantity, int lv, AbilityType[] relicAbilities = null, bool isEquip = false, bool isNewAlert = false) {
            Debug.Log($"<color=white>SetUI(ItemImg.name={ItemImg.sprite.name}, type={type}, grade={grade}, quantity= {quantity})::</color>");

            QuantityTxt.text = $"{quantity}";

            //* ETCアイテム
            if(type == Enum.ItemType.Etc) {
                TypeBgImg.enabled = false;
                TypeIconImg.enabled = false;
                BgImg.sprite = HM._.ivm.NoneBgSpr;
                LightImg.enabled = false;
                BgImg.color = (grade == Enum.Grade.None)? Color.white : HM._.ivm.GradeClrs[ (int)grade];
                LvTxt.text = "";
                EquipDim.SetActive(false);
            }
            //* EQUIPアイテム
            else {
                TypeBgImg.enabled = true;
                TypeIconImg.enabled = true;
                BgImg.sprite = HM._.ivm.GradeBgSprs[(int)grade];
                LightImg.enabled = true;
                TypeBgImg.color = HM._.ivm.GradeClrs[(int)grade];
                TypeIconImg.sprite = HM._.ivm.TypeSprs[(int)type];
                ItemImg.gameObject.SetActive(true);

                if(EquipDim)
                    EquipDim.SetActive(isEquip); //* EquipスロットはEquipDimオブジェクトがないため、合うかif文でチェック

                //* マージ可能な物 表示 (PRIME等級はしない)
                if(quantity >= Config.EQUIP_MERGE_CNT && grade < Enum.Grade.Prime) {
                    if(AlertGreenDot) AlertGreenDot.SetActive(true);
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
    }
}
