using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {
    [Header("POPUP")]
    public GameObject topMsgError;
    public TextMeshProUGUI topMsgErrorTxt;

    void Start() {
        topMsgError.SetActive(false);
    }

#region FUNC
    public IEnumerator CoShowMsgError(string msg) {
        topMsgError.SetActive(true);
        topMsgErrorTxt.text = msg;
        yield return Util.time1;
        topMsgError.SetActive(false);
    }
#endregion
}
