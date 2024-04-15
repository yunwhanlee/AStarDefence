using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Linq;

// public enum RWD_TBIDX {
//     Coin, Diamond, Equip, Relic, 
//     Goblin, Ore, GoldKey, Clover,
//     GoldClover, CONSUME_ITEM, SoulStone, MagicStone
// }

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
    public void UpdateItemData(Etc.NoshowInvItem noShowInvItem, int quantity) {
        switch (noShowInvItem) {
            case Etc.NoshowInvItem.GoldKey:
                DM._.DB.StatusDB.GoldKey += quantity; // HM._.GoldKey += quantity;
                break;
            case Etc.NoshowInvItem.Coin:
                DM._.DB.StatusDB.Coin += quantity; // HM._.Coin += quantity;
                break;
            case Etc.NoshowInvItem.Diamond:
                DM._.DB.StatusDB.Diamond += quantity; // HM._.Diamond += quantity;
                break;
            case Etc.NoshowInvItem.Exp:
                DM._.DB.StatusDB.Exp += quantity; // HM._.Exp += quantity;
                break;
            case Etc.NoshowInvItem.Goblin0: case Etc.NoshowInvItem.Goblin1: case Etc.NoshowInvItem.Goblin2:
            case Etc.NoshowInvItem.Goblin3: case Etc.NoshowInvItem.Goblin4: case Etc.NoshowInvItem.Goblin5: case Etc.NoshowInvItem.Goblin6:
                int goblinLvIdx = int.Parse($"{noShowInvItem}".Split("n")[1]);
                DM._.DB.MiningDB.GoblinCardCnts[goblinLvIdx] += quantity; // HM._.mnm.GoblinCards[goblinLvIdx].Cnt += quantity;
                break;
            case Etc.NoshowInvItem.Ore0: case Etc.NoshowInvItem.Ore1: case Etc.NoshowInvItem.Ore2:
            case Etc.NoshowInvItem.Ore3: case Etc.NoshowInvItem.Ore4: case Etc.NoshowInvItem.Ore5:
            case Etc.NoshowInvItem.Ore6: case Etc.NoshowInvItem.Ore7: case Etc.NoshowInvItem.Ore8:
                int oreLvIdx = int.Parse($"{noShowInvItem}".Split("e")[1]);
                DM._.DB.MiningDB.OreCardCnts[oreLvIdx] += quantity; // HM._.mnm.OreCards[oreLvIdx].Cnt += quantity;
                break;
            case Etc.NoshowInvItem.SkillPoint:
                DM._.DB.StatusDB.SkillPoint += quantity; // HM._.SkillPoint += quantity;
                break;
        }
    }
}
#endregion

#region リワードアイテムのデータベース
[CreateAssetMenu]
public class RewardItemSO : ScriptableObject {
    const int RAND_MAX = 1000;

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

    /// <summary>
    /// アイテム確率テーブルリスト
    /// </summary>
    private List<(ItemSO item, int percent, int quantity)> PrepareItemPerTable(RewardContentSO rwdContentDt) {
        //* ランダム確率でアイテムや容量選択
        int coinAmount = rwdContentDt.GetRandomCoin();
        int diaAmount = rwdContentDt.GetRandomDiamond();
        var equipItem = rwdContentDt.GetRandomEquipDatas(); //* 装置種類
        int equipGradeIdx = rwdContentDt.GetRandomEquipGrade(); //* 装置等級
        int relicGradeIdx = rwdContentDt.GetRandomRelicGrade(); //* 異物等級
        int goblinGradeIdx = (int)rwdContentDt.GetRandomGoblinGrade(); //* ゴブリン等級
        int oreGradeIdx = (int)rwdContentDt.GetRandomOreGrade(); //* 鉱石等
        int randConsumeIdx = (int)rwdContentDt.GetRandomConsumeItem(); //* 消費アイテム

        //* 夫々アイテム確率テーブルリスト生成 (Tuple方式)
        RewardPercentTable  itemPerTb = rwdContentDt.ItemPerTb;
        List<(ItemSO item, int percent, int quantity)> itemPerTableList = new List<(ItemSO, int, int)> {
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], itemPerTb.Coin, coinAmount),
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], itemPerTb.Diamond, diaAmount),
            (equipItem[equipGradeIdx], itemPerTb.Equip, 1),
            (RelicDatas[relicGradeIdx], itemPerTb.Relic, 1),
            (EtcNoShowInvDatas[goblinGradeIdx], itemPerTb.Goblin, 1),
            (EtcNoShowInvDatas[oreGradeIdx], itemPerTb.Ore, 1),
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.GoldKey], itemPerTb.GoldKey, rwdContentDt.GetRandomGoldKeyCnt()),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.Clover], itemPerTb.Clover, 1),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.GoldClover], itemPerTb.GoldClover, 1),
            (EtcConsumableDatas[randConsumeIdx], itemPerTb.ConsumeItem, rwdContentDt.GetRandomConsumeItemCnt()),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.SoulStone], itemPerTb.SoulStone, rwdContentDt.GetRandomSoulStoneMaxCnt()),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.MagicStone], itemPerTb.MagicStone, rwdContentDt.GetRandomMagicStoneCnt())
        };

        return itemPerTableList;
    }

    /// <summary>
    /// リワードChestとPresentを開く
    /// </summary>
    public void OpenRewardContent(RewardContentSO rwdContentDt) {
        int itemCnt = rwdContentDt.Cnt;
        List<RewardItem> rewardList = new List<RewardItem>();

        //* ランダムリワード
        int randMax = rwdContentDt.ItemPerTb.GetTotal();
        if(randMax != RAND_MAX) {
            Debug.LogError($"<RNADMAX= {randMax}>:: 全てアイテム確率テーブルの合計が１０００にならないです。RewardContentSOのInspectorビューの値を確認してください。");
            return;
        }

        //* アイテム確率テーブルリスト
        List<(ItemSO item, int per, int quantity)> itemPerTableList = PrepareItemPerTable(rwdContentDt);
        var copyItemTableList = itemPerTableList.ToList();

        //* 固定アイテム項目があったら、取ってリワードリストへ入れる
        for (int i = 0; i < copyItemTableList.Count; i++) {
            var (item, per, quantity) = copyItemTableList[i];
            if (per == -1) {
                rewardList.Add(new RewardItem(item, quantity));
                copyItemTableList.RemoveAt(i); // テーブルからこのアイテムを除く
                --itemCnt; // アイテム習得カウント減る
            }
        }

        //* 確率テーブルによる、ランダムアイテム選択
        for(int i = 0; i < itemCnt; i++) {
            int rand = Random.Range(0, randMax);
            int startRange = 0;
            foreach (var (item, per, quantity) in copyItemTableList) {  //* ToList()를 사용하여 원본 리스트를 수정하지 않고 복사본을 만듭니다.
                int endRange = startRange + per;
                if (rand < endRange) {
                    Debug.Log($"<color=yellow>OpenPresent0():: {i}: {item.name} {item.Name}, rand= {rand} / {randMax}, Range: {startRange} - {endRange - 1}</color>");

                    //* RELICなら、ランダムで能力
                    var relicAbilities = HM._.ivCtrl.InventoryData.CheckRelicAbilitiesData(item);

                    rewardList.Add(new RewardItem(item, quantity, relicAbilities));
                    copyItemTableList.Remove((item, per, quantity));  //* 이미 선택된 아이템 제거
                    randMax -= per;
                    break;
                }
                startRange = endRange;
            }
        }

        //* UI表示
        HM._.rwlm.ShowReward(rewardList);
        HM._.rwm.UpdateInventory(rewardList);
        HM._.ivCtrl.InventoryData.DecreaseItem(HM._.ivm.CurItemIdx, decVal: -1);
        HM._.rwlm.UpdateChestPopUpUI();
    }
}
#endregion