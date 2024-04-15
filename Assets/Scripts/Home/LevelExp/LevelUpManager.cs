using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.UI;
using UnityEngine.UI;
using TMPro;

public class LevelUpManager : MonoBehaviour {
    [field: SerializeField] public int Exp {
        get => DM._.DB.StatusDB.Exp;
        set => UpdateData(value);
    }
    [field: SerializeField] public int MaxExp {
        get => ExpDt.Datas[HM._.Lv - 1].Max;
    }
    public GameObject WindowObj;
    public TMP_Text LevelMarkTxt;
    public Transform Content;

    [Header("REWARD DATA")]
    public InventoryUIItem rwdItemPf;
    [field: SerializeField] public SettingExpData ExpDt {get; private set;}


    void Start() {
        UpdateData(Exp);
        CheckLevelUp();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.S)) {
            Exp += 10;
            CheckLevelUp();
        }
    }

#region FUNC
    private void UpdateData(int value) {
        DM._.DB.StatusDB.Exp = value;
        HM._.hui.ExpTxt.text = $"{value} / {MaxExp}";
        HM._.hui.ExpSlider.value = (float)Exp / MaxExp;
    }
    private void LevelUp() {
        Exp = 1;
        LevelMarkTxt.text = $"{++HM._.Lv}";
        var rewardList = new List<RewardItem> {
            new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 1000),
            new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], 5),
            new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestCommon]),
            new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore0]),
        };
        ShowReward(rewardList);
        UpdateInventory(rewardList);
    }

    public void CheckLevelUp() {
        Debug.Log($"CheckLevelUp():: Exp={Exp}, MaxExp={MaxExp}");
        if(Exp >= MaxExp)
            LevelUp();
    }

    private void DeleteAll() {
        foreach (Transform child in Content)
            Destroy(child.gameObject);
    }

    /// <summary>
    /// リワードリスト表示
    /// </summary>
    private void DisplayRewardList(List<RewardItem> rewardList) {
        //* リワードリストへオブジェクト生成・追加
        for(int i = 0; i < rewardList.Count; i++) {
            RewardItem rewardItem = rewardList[i];
            InventoryUIItem rwdItemUI = Instantiate(rwdItemPf.gameObject, Content).GetComponent<InventoryUIItem>();
            rwdItemUI.SetUIData(rewardItem.Data.Type, rewardItem.Data.Grade, rewardItem.Data.ItemImg, rewardItem.Quantity, lv: 1);
            //* Particle UI Effect 1
            rwdItemUI.PlayScaleUIEF(rwdItemUI, rewardItem.Data.ItemImg);
            rwdItemUI.ItemImgScaleUIEF.startDelay = 0.5f + i * 0.1f;
            //* Particle UI Effect 2
            rwdItemUI.WhiteDimScaleUIEF.lifetime = 0.5f + i * 0.1f;
            rwdItemUI.WhiteDimScaleUIEF.Play();

        }
    }
    public void ShowReward(List<RewardItem> itemList) {
        WindowObj.SetActive(true);
        DeleteAll();
        DisplayRewardList(itemList);
    }

    public void UpdateInventory(List<RewardItem> rewardList) {
        if(rewardList.Count > 0) {
            foreach (RewardItem rwdItem in rewardList) {
                //* リワード処理：インベントリーへ表示しないアイテム
                if(rwdItem.Data.IsNoshowInventory) {
                    Etc.NoshowInvItem enumVal = Util.FindEnumVal(rwdItem.Data.name);
                    rwdItem.UpdateItemData(enumVal, rwdItem.Quantity);
                }
                //* リワード処理：インベントリーへ表示する物
                else {
                    int reminder = HM._.ivCtrl.InventoryData.AddItem (
                        rwdItem.Data, 
                        rwdItem.Quantity, 
                        lv: 1, 
                        rwdItem.RelicAbilities
                    );
                    rwdItem.Quantity = reminder;
                }
            }
        }
    }
#endregion
}
