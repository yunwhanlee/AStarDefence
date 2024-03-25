using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiningUIManager : MonoBehaviour {
    [field: SerializeField] public GameObject WindowObj;
    [field: SerializeField] public GameObject GoblinScrollRect;
    [field: SerializeField] public GameObject OreScrollRect;

    [field: SerializeField] public Button OreSpotBtn;
    [field: SerializeField] public Button GoblinLeftSpotBtn;
    [field: SerializeField] public Button GoblinRightSpotBtn;

    #region EVENT
        public void OnClickOreSpotBtn() {
            WindowObj.SetActive(true);
            OreScrollRect.SetActive(true);
        }
        public void OnClickGoblinLeftSpotBtn() {
            WindowObj.SetActive(true);
            GoblinScrollRect.SetActive(true);
        }
        public void OnClickGoblinRightSpotBtn() {
            WindowObj.SetActive(true);
            GoblinScrollRect.SetActive(true);
        }
        public void OnClickBackBtn() {
            WindowObj.SetActive(false);
        }
    #endregion
}
