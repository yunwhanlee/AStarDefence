using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Inventory.UI;

public class GameRewardListUIManager : MonoBehaviour {
    [Header("REWARD LIST POPUP")]
    public GameObject VictoryPopUpObj;
    public Transform VictoryContent;
    public Transform GameoverContent;

    public bool IsFinishSlotsSpawn = false;

    [Header("REWARD DATA")]
    public InventoryUIItem rwdItemPf;
    [field: SerializeField] public RewardItemSO RwdItemDt {get; private set;}

#region FUNC
    private void DeleteAll() {
        Transform contentTf = (GM._.State == GameState.Victory)? VictoryContent
            : GameoverContent;
        foreach (Transform child in contentTf)
            Destroy(child.gameObject);
    }

    IEnumerator CoPlayRewardSlotSpawnSFX(int cnt) {
        IsFinishSlotsSpawn = true;
        yield return Util.RealTime0_5;
        for(int i = 0; i < cnt; i++) {
            SM._.SfxPlay(SM.SFX.InvUnEquipSFX);
            yield return Util.RealTime0_1;
        }
        IsFinishSlotsSpawn = false;
    }

    /// <summary>
    /// リワードリスト表示
    /// </summary>
    private void DisplayRewardList(List<RewardItem> rewardList) {
        Transform contentTf = (GM._.State == GameState.Victory)? VictoryContent
            : GameoverContent;

        StartCoroutine(CoPlayRewardSlotSpawnSFX(rewardList.Count));
        //* リワードリストへオブジェクト生成・追加
        for(int i = 0; i < rewardList.Count; i++) {
            RewardItem rwdItem = rewardList[i];
            InventoryUIItem rwdItemUI = Instantiate(rwdItemPf.gameObject, contentTf).GetComponent<InventoryUIItem>();
            rwdItemUI.SetUI(rwdItem.Data.Type, rwdItem.Data.Grade, rwdItem.Data.ItemImg, rwdItem.Quantity, lv: 1);
            if(rwdItem.Data.name.Contains("Chest"))
                rwdItemUI.BonusRewardLabel.SetActive(true);
            //* Particle UI Effect 1
            rwdItemUI.PlayScaleUIEF(rwdItemUI, rwdItem.Data.ItemImg);
            rwdItemUI.ItemImgScaleUIEF.startDelay = 0.5f + i * 0.1f;
            //* Particle UI Effect 2
            rwdItemUI.WhiteDimScaleUIEF.lifetime = 0.5f + i * 0.1f;
            rwdItemUI.WhiteDimScaleUIEF.Play();
        }
    }
    public void ShowReward(List<RewardItem> itemList) {
        DeleteAll();
        DisplayRewardList(itemList);
    }
#endregion
}
