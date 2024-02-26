using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {
    [Header("ERROR MSG POPUP")]
    public GameObject topMsgError;
    public TextMeshProUGUI MsgErrorTxt;

    [Header("INFO MSG POPUP")]
    public GameObject topMsgInfo;
    public TextMeshProUGUI MsgInfoTxt;

    void Start() {
        topMsgError.SetActive(false);
    }

#region FUNC
    public IEnumerator CoShowMsgError(string msg) {
        topMsgError.SetActive(true);
        MsgErrorTxt.text = msg;
        yield return Util.time1;
        topMsgError.SetActive(false);
    }
    /// <summary>
    /// 情報メッセージ表示のポップアップ（ON、OFF形式）
    /// </summary>    
    public void ShowMsgInfo(bool isActive, string msg = "") {
        MsgInfoTxt.text = isActive? msg : "";
        topMsgInfo.SetActive(isActive);
    }
#endregion
}
