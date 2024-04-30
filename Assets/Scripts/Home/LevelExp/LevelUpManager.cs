using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.UI;
using UnityEngine.UI;
using TMPro;

public class LevelUpManager : MonoBehaviour {
    [field: SerializeField] public int Lv {
        get => DM._.DB.StatusDB.Lv;
        set {
            DM._.DB.StatusDB.Lv = value;
            HM._.hui.LvTxt.text = $"{value}";
            LevelMarkTxt.text = $"{value}";
        } 
    }
    [field: SerializeField] public int Exp {
        get => DM._.DB.StatusDB.Exp;
        set => UpdateData(value);
    }
    [field: SerializeField] public int MaxExp {
        get => ExpDt.Datas[Lv - 1].Max;
    }
    public GameObject WindowObj;
    public TMP_Text LevelMarkTxt;
    public Transform Content;
    public bool IsFinishSlotsSpawn = false;

    [Header("REWARD DATA")]
    public InventoryUIItem rwdItemPf;
    [field: SerializeField] public SettingExpData ExpDt {get; private set;}

    IEnumerator Start() {
        //* HM.Start()でのDM.LoadDt()してから、下の処理を実行するために、子ルチンで少し待機
        yield return Util.Time0_1;
        UpdateData(Exp);
        CheckLevelUp();
        HM._.hui.LvTxt.text = $"{Lv}";
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.S)) {
            Exp += 10;
            CheckLevelUp();
        }
    }

#region EVENT
    public void OnClickCloseLevelUpPopUp() {
        Debug.Log("OnClickCloseLevelUpPopUp()::");
        //* リワードスロットのアニメーションが全部終わるまで待つ
        if(IsFinishSlotsSpawn)
            return;

        SM._.SfxPlay(SM.SFX.ClickSFX);
        HM._.hui.IsActivePopUp = false;
        WindowObj.SetActive(false);
        StartCoroutine(CoReCheckLevelUp());
    }
#endregion

#region FUNC
    private void UpdateData(int value) {
        DM._.DB.StatusDB.Exp = value;
        HM._.hui.ExpTxt.text = $"{value} / {MaxExp}";
        HM._.hui.ExpSlider.value = (float)Exp / MaxExp;
    }
    private void LevelUp() {
        Debug.Log($"LevelUp():: Lv= {Lv}, DB.Lv= {DM._.DB.StatusDB.Lv}");
        SM._.SfxPlay(SM.SFX.LevelUpSFX);
        Lv++;
        var rewardList = new List<RewardItem> {
            new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 1000),
            new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], 5),
            new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestCommon]),
            new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.SkillPoint]),
        };
        UpdateData(Exp);
        ShowReward(rewardList);
        UpdateInventory(rewardList);
        HM._.stm.UpdateAlertIcon();
    }
    
    IEnumerator CoReCheckLevelUp() {
        yield return Util.Time0_2;
        CheckLevelUp();
    }
    public void CheckLevelUp() {
        if(Exp >= MaxExp) {
            Exp -= MaxExp;
            Debug.Log($"CheckLevelUp():: AfterLevel -> Exp={Exp}, MaxExp={MaxExp}");
            LevelUp();
        }
    }

    private void DeleteAll() {
        foreach (Transform child in Content)
            Destroy(child.gameObject);
    }

    IEnumerator CoPlayRewardSlotSpawnSFX(int cnt) {
        IsFinishSlotsSpawn = true;
        yield return Util.Time0_5;
        for(int i = 0; i < cnt; i++) {
            SM._.SfxPlay(SM.SFX.InvUnEquipSFX);
            yield return Util.Time0_1;
        }
        IsFinishSlotsSpawn = false;
    }

    /// <summary>
    /// リワードリスト表示
    /// </summary>
    private void DisplayRewardList(List<RewardItem> rewardList) {
        StartCoroutine(CoPlayRewardSlotSpawnSFX(rewardList.Count));
        //* リワードリストへオブジェクト生成・追加
        for(int i = 0; i < rewardList.Count; i++) {
            RewardItem rewardItem = rewardList[i];
            InventoryUIItem rwdItemUI = Instantiate(rwdItemPf.gameObject, Content).GetComponent<InventoryUIItem>();
            rwdItemUI.SetUI(rewardItem.Data.Type, rewardItem.Data.Grade, rewardItem.Data.ItemImg, rewardItem.Quantity, lv: 1);
            // rwdItemUI.IsNewAlert = true;
            //* Particle UI Effect 1
            rwdItemUI.PlayScaleUIEF(rwdItemUI, rewardItem.Data.ItemImg);
            rwdItemUI.ItemImgScaleUIEF.startDelay = 0.5f + i * 0.1f;
            //* Particle UI Effect 2
            rwdItemUI.WhiteDimScaleUIEF.lifetime = 0.5f + i * 0.1f;
            rwdItemUI.WhiteDimScaleUIEF.Play();

        }
    }
    public void ShowReward(List<RewardItem> itemList) {
        HM._.hui.IsActivePopUp = true;
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
                        rwdItem.RelicAbilities,
                        isEquip: false,
                        isNewAlert: true
                    );
                    rwdItem.Quantity = reminder;
                }
            }
        }
    }
#endregion
}
