using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Inventory.UI {
    public class InventoryDescription : MonoBehaviour {
        [field:SerializeField] private Image ItemImg {get; set;}
        [field:SerializeField] private TMP_Text Title {get; set;}
        [field:SerializeField] private TMP_Text Description {get; set;}

        void Awake() {
            ResetDescription();
        }

        public void ResetDescription() {
            ItemImg.gameObject.SetActive(false);
            Title.text = "";
            Description.text = "";
        }

        public void SetDescription(Sprite spr, string itemName, string itemDescription) {
            ItemImg.gameObject.SetActive(true);
            ItemImg.sprite = spr;
            Title.text = itemName;
            Description.text = itemDescription;
        }
    }
}