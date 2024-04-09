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
    public void UpdateItemData(Etc.NoshowInvItem noShowInvItem, int quantity) {
        switch (noShowInvItem) {
            case Etc.NoshowInvItem.GoldKey:
                HM._.GoblinKey += quantity;
                break;
            case Etc.NoshowInvItem.Coin:
                HM._.Coin += quantity;
                break;
            case Etc.NoshowInvItem.Diamond:
                HM._.Diamond += quantity;
                break;
            case Etc.NoshowInvItem.Exp:
                HM._.Exp += quantity;
                break;
            case Etc.NoshowInvItem.Goblin0: case Etc.NoshowInvItem.Goblin1: case Etc.NoshowInvItem.Goblin2:
            case Etc.NoshowInvItem.Goblin3: case Etc.NoshowInvItem.Goblin4: case Etc.NoshowInvItem.Goblin5: case Etc.NoshowInvItem.Goblin6:
                int goblinLvIdx = int.Parse($"{noShowInvItem}".Split("n")[1]);
                HM._.mnm.GoblinCards[goblinLvIdx].Cnt += quantity;
                break;
            case Etc.NoshowInvItem.Ore0: case Etc.NoshowInvItem.Ore1: case Etc.NoshowInvItem.Ore2:
            case Etc.NoshowInvItem.Ore3: case Etc.NoshowInvItem.Ore4: case Etc.NoshowInvItem.Ore5:
            case Etc.NoshowInvItem.Ore6: case Etc.NoshowInvItem.Ore7: case Etc.NoshowInvItem.Ore8:
                int oreLvIdx = int.Parse($"{noShowInvItem}".Split("e")[1]);
                HM._.mnm.OreCards[oreLvIdx].Cnt += quantity;
                break;
            case Etc.NoshowInvItem.SkillPoint:
                HM._.SkillPoint += quantity;
                break;
        }
    }
}
#endregion

public struct RewardPercentTable {
    public int GOLD, DIAMOND, EQUIP, RELIC, GOBLIN, ORE, GOLD_KEY, CLOVER, GOLD_CLOVER, CONSUME_ITEM, SOUL_STONE, MAGIC_STONE;

    public RewardPercentTable(
        int GOLD = 0, 
        int DIAMOND = 0, 
        int EQUIP = 0, 
        int RELIC = 0, 
        int GOBLIN = 0, 
        int ORE = 0, 
        int GOLD_KEY = 0, 
        int CLOVER = 0, 
        int GOLD_CLOVER = 0, 
        int CONSUME_ITEM = 0, 
        int SOUL_STONE = 0, 
        int MAGIC_STONE = 0) {
            this.GOLD = GOLD;
            this.DIAMOND = DIAMOND;
            this.EQUIP = EQUIP;
            this.RELIC = RELIC;
            this.GOBLIN = GOBLIN;
            this.ORE = ORE;
            this.GOLD_KEY = GOLD_KEY;
            this.CLOVER = CLOVER;
            this.GOLD_CLOVER = GOLD_CLOVER;
            this.CONSUME_ITEM = CONSUME_ITEM;
            this.SOUL_STONE = SOUL_STONE;
            this.MAGIC_STONE = MAGIC_STONE;
        }

    // public int GetTocalPercent() => GOLD + DIAMOND + EQUIP + RELIC + GOBLIN + ORE + GOLD_KEY + CLOVER + GOLD_CLOVER + CONSUME_ITEM + SOUL_STONE + MAGIC_STONE;
}

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

    public void OpenPresent0() {
        const int CNT = 4;
        List<RewardItem> rewardList = new List<RewardItem>();

        //* アイテム確率テーブル設定
        var itemPerTb = new RewardPercentTable(
            GOLD: 300, DIAMOND: 50,
            EQUIP: 100, RELIC: 0, 
            GOBLIN: 200, ORE: 200, GOLD_KEY: 50, 
            CLOVER: 50, GOLD_CLOVER: 0, 
            CONSUME_ITEM: 50, 
            SOUL_STONE: 0, MAGIC_STONE: 0
        );

        //* ランダムリワード
        int randMax = RAND_MAX;
        int rand = Random.Range(0, randMax);

        //* 夫々アイテム確率テーブルリスト生成 (Tuple方式)
        int randEquip = Random.Range(0, 2 + 1);
        var equipItem = randEquip == 0? WeaponDatas : randEquip == 1? ShoesDatas: RingDatas;

        int randEquipGrade = Random.Range(0, 1000);
        int equipGradeIdx = randEquipGrade < 750? 0 : randEquipGrade < 750 + 230? 1 : 2;

        int randRelicGrade = Random.Range(0, 1000);
        int relicGradeIdx = randRelicGrade < 95? 0 : 1;

        //TODO Rand Goblin 
        //TODO Rand Ore
        //TODO Rand Goblin ConsumeItem

        List<(ItemSO item, int percent, int quantity)> itemPerTableList = new List<(ItemSO, int, int)> {
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], itemPerTb.GOLD, Random.Range(50, 200 + 1)),
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], itemPerTb.DIAMOND, Random.Range(10, 50 + 1)),
            (equipItem[equipGradeIdx], itemPerTb.EQUIP, 1),
            (RelicDatas[relicGradeIdx], itemPerTb.RELIC, 1),
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin0], itemPerTb.GOBLIN, 1),
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore0], itemPerTb.ORE, 1),
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.GoldKey], itemPerTb.GOLD_KEY, 1),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.Clover], itemPerTb.CLOVER, 1),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.GoldClover], itemPerTb.GOLD_CLOVER, 1),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack0], itemPerTb.CONSUME_ITEM, 1),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.SoulStone], itemPerTb.SOUL_STONE, 1),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.MagicStone], itemPerTb.MAGIC_STONE, 1)
        };

        //* 確率テーブルによる、ランダムアイテム選択
        Debug.Log("OpenPresent0():: Reward Item Select");
        var copyItemTableList = itemPerTableList.ToList();
        for(int i = 0; i < CNT; i++) {
            rand = Random.Range(0, randMax);
            int startRange = 0;
            foreach (var (item, percent, quantity) in copyItemTableList) {  //* ToList()를 사용하여 원본 리스트를 수정하지 않고 복사본을 만듭니다.
                int endRange = startRange + percent;
                if (rand < endRange) {
                    Debug.Log($"<color=yellow>OpenPresent0():: {i}: {item.name} {item.Name}, rand= {rand} / {randMax}, Range: {startRange} - {endRange - 1}</color>");
                    rewardList.Add(new RewardItem(item));
                    copyItemTableList.Remove((item, percent, quantity));  //* 이미 선택된 아이템 제거
                    randMax -= percent;
                    break;
                }
                startRange = endRange;
            }
        }

        HM._.rwlm.ShowReward(rewardList);
        HM._.rwm.UpdateInventory(rewardList);
    }
}
#endregion