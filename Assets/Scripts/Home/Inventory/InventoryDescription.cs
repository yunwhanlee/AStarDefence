using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Inventory.Model;

namespace Inventory.UI {
    public class InventoryDescription : MonoBehaviour {
        [field:SerializeField] private Image TopBg {get; set;}
        [field:SerializeField] private Image ItemBg {get; set;}
        [field:SerializeField] private Image ItemImg {get; set;}
        [field:SerializeField] private Image TypeImg {get; set;}
        [field:SerializeField] private TMP_Text TypeTxt {get; set;}
        [field:SerializeField] private TMP_Text NameTxt {get; set;}
        [field:SerializeField] private TMP_Text GradeTxt {get; set;}
        [field:SerializeField] private TMP_Text Description {get; set;}

        void Awake() {
            ResetDescription();
        }

        public void ResetDescription() {
            ItemImg.gameObject.SetActive(false);
            TypeTxt.text = "";
            NameTxt.text = "";
            GradeTxt.text = "";
            Description.text = "";
        }

        public void SetDescription(ItemSO item) {
            Debug.Log("item.Name= " + item.Name + ",item.Image= " + item.ItemImg);
            ItemImg.gameObject.SetActive(true);
            //* スタイル
            TopBg.color = HM._.ivm.GradeClrs[(int)item.Grade];
            ItemBg.sprite = HM._.ivm.GradeBgSprs[(int)item.Grade];
            ItemImg.sprite = item.ItemImg;
            TypeImg.sprite = HM._.ivm.TypeSprs[(int)item.Type];
            TypeTxt.text = (item.Type == Enum.ItemType.Weapon)? "무기"
                : (item.Type == Enum.ItemType.Shoes)? "신발"
                : (item.Type == Enum.ItemType.Accessories)? "악세서리"
                : (item.Type == Enum.ItemType.Relic)? "유물"
                : "기타";
            NameTxt.text = item.Name;
            GradeTxt.text = item.Grade.ToString();
            GradeTxt.color = HM._.ivm.GradeClrs[(int)item.Grade];
            Description.text = item.Description;
        }
    }
}