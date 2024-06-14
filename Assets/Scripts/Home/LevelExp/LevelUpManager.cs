using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.UI;
using UnityEngine.UI;
using TMPro;
using Inventory.Model;
using DG.Tweening;

public class LevelUpManager : MonoBehaviour {
    [field: SerializeField] public int Lv {
        get => DM._.DB.StatusDB.Lv;
        set {
            DM._.DB.StatusDB.Lv = value;
            HM._.hui.LvTxt.text = $"{value}";
            LevelMarkTxt.text = $"{value}";
            string extraDmgTxt = $"<size=80%><color=#3cff00>+{Config.USER_LV_EXTRA_DMG_PER * 100}%</color></size>";
            ExraDmgValTxt.text = $"{DM._.DB.StatusDB.GetUserLvExtraDmgPercent() * 100}% {extraDmgTxt}";
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
    public TMP_Text LevelUpTitleTxt;
    public TMP_Text LevelMarkTxt;
    public TMP_Text ExraDmgValTxt;
    public DOTweenAnimation ArrowUpDOTAnim;
    public GameObject ContentGroupObj;
    public GameObject UIEFGroupObj;
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

        //* UI 初期化
        HM._.hui.LvTxt.text = $"{Lv}";
        LevelMarkTxt.text = $"{Lv}";
        ExraDmgValTxt.text = $"{DM._.DB.StatusDB.GetUserLvExtraDmgPercent() * 100}%";
    }

    // void Update() {
    //     if(Input.GetKeyDown(KeyCode.S)) {
    //         Exp += 10;
    //         CheckLevelUp();
    //     }
    // }

#region EVENT
    public void OnClickLevelUpIconAtHome() {
        SM._.SfxPlay(SM.SFX.StageSelectSFX);
        WindowObj.SetActive(true);
        LevelUpTitleTxt.text = "레벨";
        ExraDmgValTxt.text = $"{DM._.DB.StatusDB.GetUserLvExtraDmgPercent() * 100}%";
        UIEFGroupObj.SetActive(false);
        ContentGroupObj.SetActive(false);
        ArrowUpDOTAnim.gameObject.SetActive(false);
    }
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
        RewardItemSO rwDt = HM._.rwlm.RwdItemDt;

        SM._.SfxPlay(SM.SFX.LevelUpSFX);
        LevelUpTitleTxt.text = "레벨 UP!";
        UIEFGroupObj.SetActive(true);
        ContentGroupObj.SetActive(true);
        ArrowUpDOTAnim.gameObject.SetActive(true);

        Lv++;

        var rewardList = new List<RewardItem> {
            new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], Lv * 100),
            new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], 10),
            new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.SkillPoint]),
            new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Fame], Lv / 5 + 1),
        };

        //* ボナース Chestリワード
        ItemSO chestRwd = null;
        if(Lv % 4 == 0) {
            if(Lv <= 10)
                chestRwd = rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestCommon];
            else if(Lv <= 25)
                chestRwd = rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present0];
            else if(Lv <= 40)
                chestRwd = rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present1];
            else if(Lv <= 55)
                chestRwd = rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present2];
            else if(Lv <= 70)
                chestRwd = rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestGold];
            else 
                chestRwd = rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestPremium];
        }
        if(chestRwd)
            rewardList.Add(new(chestRwd));

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
                    rwdItem.UpdateNoShowItemData(enumVal, rwdItem.Quantity);
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
