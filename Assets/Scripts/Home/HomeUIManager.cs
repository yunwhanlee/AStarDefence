using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Lobby
/// </summary>
public class HomeUIManager : MonoBehaviour {
    public Button PlayBtn {get; set;}

#region EVENT/*  */
    public void OnClickPlayBtn() {
        HM._.stgm.StageGroup.SetActive(true);
        int i = 0;
        Array.ForEach(HM._.stgm.StagePopUps, popUp => popUp.SetActive(HM._.SelectedStage == i++));
    }
#endregion
}
