using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Linq;

#region リワードアイテム
[Serializable]
public class RewardItem {
    [field: SerializeField] public ItemSO Data {get; private set;}
    [field: SerializeField] public int Quantity {get; set;} = 1;
    [field: SerializeField] public AbilityType[] RelicAbilities {get; set;}

    public RewardItem(ItemSO data, int quantity = 1, AbilityType[] relicAblities = null) {
        Data = data;
        Quantity = quantity;
        RelicAbilities = relicAblities;
    }

    /// <summary>
    /// インベントリへ表示されないアイテム 最新化
    /// </summary>
    public void UpdateNoShowItemData(Etc.NoshowInvItem noShowInvItem, int quantity) {
        switch (noShowInvItem) {
            case Etc.NoshowInvItem.GoldKey:
                if(HM._)    HM._.GoldKey += quantity;
                else        DM._.DB.StatusDB.GoldKey += quantity;
                break;
            case Etc.NoshowInvItem.Coin:
                if(HM._)    HM._.Coin += quantity;
                else        DM._.DB.StatusDB.Coin += quantity;
                break;
            case Etc.NoshowInvItem.Diamond:
                if(HM._)    HM._.Diamond += quantity;
                else        DM._.DB.StatusDB.Diamond += quantity;
                break;
            case Etc.NoshowInvItem.Exp:
                HM._.lvm.Exp += quantity;
                break;
            case Etc.NoshowInvItem.Goblin0: case Etc.NoshowInvItem.Goblin1: case Etc.NoshowInvItem.Goblin2:
            case Etc.NoshowInvItem.Goblin3: case Etc.NoshowInvItem.Goblin4: case Etc.NoshowInvItem.Goblin5: case Etc.NoshowInvItem.Goblin6:
                int goblinLvIdx = int.Parse($"{noShowInvItem}".Split("n")[1]);
                DM._.DB.MiningDB.GoblinCardCnts[goblinLvIdx] += quantity;
                break;
            case Etc.NoshowInvItem.Ore0: case Etc.NoshowInvItem.Ore1: case Etc.NoshowInvItem.Ore2:
            case Etc.NoshowInvItem.Ore3: case Etc.NoshowInvItem.Ore4: case Etc.NoshowInvItem.Ore5:
            case Etc.NoshowInvItem.Ore6: case Etc.NoshowInvItem.Ore7: case Etc.NoshowInvItem.Ore8:
                int oreLvIdx = int.Parse($"{noShowInvItem}".Split("e")[1]);
                DM._.DB.MiningDB.OreCardCnts[oreLvIdx] += quantity;
                break;
            case Etc.NoshowInvItem.SkillPoint:
                if(HM._)    HM._.SkillPoint += quantity;
                else        DM._.DB.StatusDB.SkillPoint += quantity;
                break;
            case Etc.NoshowInvItem.Crack:
                if(HM._)    HM._.Crack += quantity;
                else        DM._.DB.StatusDB.Crack += quantity;
                break;
            case Etc.NoshowInvItem.RemoveAd:
                DM._.DB.IsRemoveAd = true;
                HM._.hui.RemoveAdIcon.SetActive(true);
                HM._.shopMg.RemoveAdDim.SetActive(true);
                break;
            case Etc.NoshowInvItem.Fame:
                DM._.DB.StatusDB.Fame += quantity;
                if(HM._)    HM._.hui.FameTxt.text = DM._.DB.StatusDB.Fame.ToString();
                GPGSManager._.UpdateFameToLeaderBoard();
                break;
        }
    }
}
#endregion

#region リワードアイテムのデータベース
[CreateAssetMenu]
public class RewardItemSO : ScriptableObject {
    public const int RAND_MAX = 1000;

    [field: Header("ItemSO LIST")]
    [field: SerializeField] public ItemSO[] EtcConsumableDatas {get; private set;}
    [field: SerializeField] public ItemSO[] EtcNoShowInvDatas {get; private set;}
    [field: SerializeField] public ItemSO[] WeaponDatas {get; private set;}
    [field: SerializeField] public ItemSO[] ShoesDatas {get; private set;}
    [field: SerializeField] public ItemSO[] RingDatas {get; private set;}
    [field: SerializeField] public ItemSO[] RelicDatas {get; private set;}

    [field: Header("RewardContentSO LIST")]
    [field: SerializeField] public RewardContentSO Rwd_Present0 {get; private set;}
    [field: SerializeField] public RewardContentSO Rwd_Present1 {get; private set;}
    [field: SerializeField] public RewardContentSO Rwd_Present2 {get; private set;}
    [field: SerializeField] public RewardContentSO Rwd_ChestCommon {get; private set;}
    [field: SerializeField] public RewardContentSO Rwd_ChestDiamond {get; private set;}
    [field: SerializeField] public RewardContentSO Rwd_ChestEquipment {get; private set;}
    [field: SerializeField] public RewardContentSO Rwd_ChestGold {get; private set;}
    [field: SerializeField] public RewardContentSO Rwd_ChestPremium {get; private set;}

    [field: Header("Stage Clear Data LIST")]
    [field: SerializeField] public RewardContentSO[] Rwd_StageClearDts {get; private set;}

    [field: Header("Goblin Dungeon Clear Data LIST")]
    [field: SerializeField] public RewardContentSO[] Rwd_GoblinDungeonClearDts {get; private set;}

    /// <summary>
    /// アイテム確率テーブルリスト
    /// </summary>
    public List<(ItemSO item, int percent, int quantity)> PrepareItemPerTable(RewardContentSO rwdContentDt) {
        //* ランダム確率でアイテムや容量選択
        var equipItem = rwdContentDt.GetRandomEquipDatas(); //* 装置種類
        int relicGradeIdx = rwdContentDt.GetRandomRelicGrade(); //* 異物等級
        int equipGradeIdx = rwdContentDt.GetRandomEquipGrade(); //* 装置等級
        int goblinGradeIdx = (int)rwdContentDt.GetRandomGoblinGrade(); //* ゴブリン等級
        int oreGradeIdx = (int)rwdContentDt.GetRandomOreGrade(); //* 鉱石 等級
        int randConsumeIdx = (int)rwdContentDt.GetRandomConsumeItem(); //* 消費アイテム

        //* 夫々アイテム確率テーブルリスト生成 (Tuple方式)
        RewardPercentTable  itemPerTb = rwdContentDt.ItemPerTb;
        List<(ItemSO item, int percent, int quantity)> itemPerTableList = new List<(ItemSO, int, int)> {
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], itemPerTb.Exp, rwdContentDt.GetRandomExp()),
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], itemPerTb.Coin, rwdContentDt.GetRandomCoin()),
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], itemPerTb.Diamond, rwdContentDt.GetRandomDiamond()),
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Fame], itemPerTb.Fame, rwdContentDt.GetRandomFame()),
            (equipItem[equipGradeIdx], itemPerTb.Equip, 1),
            (RelicDatas[relicGradeIdx], itemPerTb.Relic, 1),
            (EtcNoShowInvDatas[goblinGradeIdx], itemPerTb.Goblin, rwdContentDt.GetRandomGoblinCnt()),
            (EtcNoShowInvDatas[oreGradeIdx], itemPerTb.Ore, rwdContentDt.GetRandomOreCnt()),
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.GoldKey], itemPerTb.GoldKey, rwdContentDt.GetRandomGoldKeyCnt()),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.Clover], itemPerTb.Clover, 1),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.GoldClover], itemPerTb.GoldClover, 1),
            (EtcConsumableDatas[randConsumeIdx], itemPerTb.ConsumeItem, rwdContentDt.GetRandomConsumeItemCnt()),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.SoulStone], itemPerTb.SoulStone, rwdContentDt.GetRandomSoulStoneMaxCnt()),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.MagicStone], itemPerTb.MagicStone, rwdContentDt.GetRandomMagicStoneCnt()),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.ChestCommon], itemPerTb.ChestCommon, rwdContentDt.GetRandomChestCommonCnt()),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.ChestEquipment], itemPerTb.ChestEquipment, rwdContentDt.GetRandomChestEquipmentCnt())
        };

        //* ログ
        itemPerTableList.ForEach(list => {
            string colorTag = list.percent == -1? "<color=white>FIX " : "<color=grey>";
            Debug.Log($"{colorTag}PrepareItemPerTable():: itemPerTableList -> name= {list.item.Name}, per= {list.percent}, quantity= {list.quantity}</color>");
        });

        return itemPerTableList;
    }

    /// 
    /// <summary>
    /// リワードChestとPresentを開く
    /// </summary>
    /// <param name="rwdContentDt">リワードSOデータ</param>
    /// <param name="chestGatherOpenCnt">箱を同時に開くカウント(ex. EQUIP CHSET 5個、10個、20個、40個...)</param>
    public void OpenRewardContent(bool isOpenByInv ,RewardContentSO rwdContentDt, int chestGatherOpenCnt = 0) {
        //* アイテム数 (指定したカウントがあれば、これにする)
        int itemCnt = (chestGatherOpenCnt == 0)? rwdContentDt.Cnt : chestGatherOpenCnt; 
        List<RewardItem> rewardList = new List<RewardItem>();
        Debug.Log($"OpenRewardContent():: rwdContentDt={rwdContentDt.name}, itemCnt= {itemCnt}");

        DM._.DB.DailyMissionDB.OpenAnyChestVal++;

        //* ランダムリワード
        int randMax = rwdContentDt.ItemPerTb.GetTotal();
        if(randMax != RAND_MAX) {
            Debug.LogError($"<RNADMAX= {randMax}>:: 全てアイテム確率テーブルの合計が１０００にならないです。RewardContentSOのInspectorビューの値を確認してください。");
            return;
        }

        //* アイテム確率テーブルリスト
        List<(ItemSO item, int per, int quantity)> itemPerTableList = PrepareItemPerTable(rwdContentDt);
        var copyItemTableList = itemPerTableList.ToList();

        //* お先に固定アイテム項目
        for (int i = 0; i < copyItemTableList.Count; i++) {
            var (item, per, quantity) = copyItemTableList[i];
            if (per == -1) {
                rewardList.Add(new RewardItem(item, quantity));
                copyItemTableList.RemoveAt(i); // テーブルからこのアイテムを除く
                --itemCnt; // アイテム習得カウント減る
            }
        }

        //* ランダムアイテム選択 (From 確率テーブル)
        for(int i = 0; i < itemCnt; i++) {
            int rand = Random.Range(0, randMax);
            int startRange = 0;
            foreach (var (item, per, quantity) in copyItemTableList) {  //* ToList()를 사용하여 원본 리스트를 수정하지 않고 복사본을 만듭니다.
                int endRange = startRange + per;
                if (rand < endRange) {
                    Debug.Log($"fileName= {rwdContentDt.name} <color=yellow>OpenPresent0():: {i}: {item.name} {item.Name}, rand= {rand} / {randMax}, Range: {startRange} - {endRange - 1}</color>");

                    //* ★RELICなら、ランダムで能力
                    var relicAbilities = HM._.ivCtrl.InventoryData.CheckRelicAbilitiesData(item);
                    rewardList.Add(new RewardItem(item, quantity, relicAbilities));

                    //* 一般宝箱の開く処理
                    if(chestGatherOpenCnt == 0) {
                        copyItemTableList.Remove((item, per, quantity));  //* 이미 선택된 아이템 제거
                        randMax -= per;
                    }
                    //* SHOPでEquipChestを購入の場合、複数を開けることの処理
                    else
                        copyItemTableList = PrepareItemPerTable(rwdContentDt);
                    break;
                }
                startRange = endRange;
            }
        }

        //* UI表示
        HM._.rwlm.ShowReward(rewardList);

        //* 複数の箱を開く(インベントリーからCOMMON & EQUIP箱のみ)
        if(isOpenByInv) { //! SHOPで購入するときには、以前に箱がインベントリーにないから、DecreaseItem処理するとだめ
            int decreaseCnt = (chestGatherOpenCnt == 0)? 1 : chestGatherOpenCnt;
            HM._.ivCtrl.InventoryData.DecreaseItem(HM._.ivm.CurItemIdx, decVal: -decreaseCnt);
        }

        HM._.rwlm.UpdateChestPopUpUI();
    }
}
#endregion