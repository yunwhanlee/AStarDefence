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

    public void OpenPresent(RewardContentSO rwdPresentSO) {
        List<RewardItem> rewardList = new List<RewardItem>();

        //* ランダムリワード
        int randMax = rwdPresentSO.ItemPerTb.GetTotal();
        if(randMax != RAND_MAX) {
            Debug.LogError("全てアイテム確率テーブルの合計が１０００にならないです。RewardContentSOのInspectorビューの値を確認してください。");
            return;
        }

        //* ランダム確率でアイテムや容量選択
        int coinAmount = rwdPresentSO.GetRandomCoin();
        int diaAmount = rwdPresentSO.GetRandomDiamond();
        var equipItem = rwdPresentSO.GetRandomEquipDatas(); //* 装置種類
        int equipGradeIdx = rwdPresentSO.GetRandomEquipGrade(); //* 装置等級
        int relicGradeIdx = rwdPresentSO.GetRandomRelicGrade(); //* 異物等級
        int goblinGradeIdx = (int)rwdPresentSO.GetRandomGoblinGrade(); //* ゴブリン等級
        int oreGradeIdx = (int)rwdPresentSO.GetRandomOreGrade(); //* 鉱石等
        int randConsumeIdx = (int)rwdPresentSO.GetRandomConsumeItem(); //* 消費アイテム

        //* 夫々アイテム確率テーブルリスト生成 (Tuple方式)
        RewardPercentTable  itemPerTb = rwdPresentSO.ItemPerTb;
        List<(ItemSO item, int percent, int quantity)> itemPerTableList = new List<(ItemSO, int, int)> {
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], itemPerTb.COIN, coinAmount),
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], itemPerTb.DIAMOND, diaAmount),
            (equipItem[equipGradeIdx], itemPerTb.EQUIP, 1),
            (RelicDatas[relicGradeIdx], itemPerTb.RELIC, 1),
            (EtcNoShowInvDatas[goblinGradeIdx], itemPerTb.GOBLIN, 1),
            (EtcNoShowInvDatas[oreGradeIdx], itemPerTb.ORE, 1),
            (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.GoldKey], itemPerTb.GOLD_KEY, 1),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.Clover], itemPerTb.CLOVER, 1),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.GoldClover], itemPerTb.GOLD_CLOVER, 1),
            (EtcConsumableDatas[randConsumeIdx], itemPerTb.CONSUME_ITEM, 1),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.SoulStone], itemPerTb.SOUL_STONE, 1),
            (EtcConsumableDatas[(int)Etc.ConsumableItem.MagicStone], itemPerTb.MAGIC_STONE, 1)
        };

        //* 確率テーブルによる、ランダムアイテム選択
        var copyItemTableList = itemPerTableList.ToList();
        for(int i = 0; i < rwdPresentSO.Cnt; i++) {
            int rand = Random.Range(0, randMax);
            int startRange = 0;
            foreach (var (item, percent, quantity) in copyItemTableList) {  //* ToList()를 사용하여 원본 리스트를 수정하지 않고 복사본을 만듭니다.
                int endRange = startRange + percent;
                if (rand < endRange) {
                    Debug.Log($"<color=yellow>OpenPresent0():: {i}: {item.name} {item.Name}, rand= {rand} / {randMax}, Range: {startRange} - {endRange - 1}</color>");
                    rewardList.Add(new RewardItem(item, quantity));
                    copyItemTableList.Remove((item, percent, quantity));  //* 이미 선택된 아이템 제거
                    randMax -= percent;
                    break;
                }
                startRange = endRange;
            }
        }

        //* UI表示
        HM._.rwlm.ShowReward(rewardList);
        HM._.rwm.UpdateInventory(rewardList);
    }
}
#endregion