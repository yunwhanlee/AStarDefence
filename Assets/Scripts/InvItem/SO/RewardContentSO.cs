using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;
using Random = UnityEngine.Random;

#region リワードアイテム％テーブル 構造体
[Serializable]
public struct RewardPercentTable {
    public int COIN, DIAMOND, EQUIP, RELIC, GOBLIN, ORE, GOLD_KEY, CLOVER, GOLD_CLOVER, CONSUME_ITEM, SOUL_STONE, MAGIC_STONE;

    public RewardPercentTable (
        int COIN = 0, 
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
            this.COIN = COIN;
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
    
    public int GetTotal() 
        => COIN + DIAMOND + EQUIP + RELIC + GOBLIN + ORE + GOLD_KEY + CLOVER + GOLD_CLOVER + CONSUME_ITEM + SOUL_STONE + MAGIC_STONE;
}
#endregion

[Serializable]
public struct RewardGradeTable {
    [field: SerializeField] public List<int> EquipPerList {get; private set;}
    [field: SerializeField] public List<int> RelicPerList {get; private set;}
    [field: SerializeField] public List<int> GoblinPerList {get; private set;}
    [field: SerializeField] public List<int> OrePerList {get; private set;}
}

/// <summary>
/// ステージクリアやボックスを開くと出るリワードリストデータ
/// </summary>
[CreateAssetMenu]
public class RewardContentSO : ScriptableObject {
    [field: SerializeField] public int Cnt {get; private set;}

    [field: Header("アイテム確率テーブル設定")]
    [field: SerializeField] public RewardPercentTable ItemPerTb {get; private set;}
    [field: Header("コイン範囲")]
    [field: SerializeField] public int CoinMin {get; private set;}
    [field: SerializeField] public int CoinMax {get; private set;}
    [field: Header("ダイアモンド範囲")]
    [field: SerializeField] public int DiamondMin {get; private set;}
    [field: SerializeField] public int DiamondMax {get; private set;}
    [field: Header("アイテム等級テーブル")]
    [field: SerializeField] public RewardGradeTable RwdGradeTb {get; private set;}

#region FUNC
    /// <summary>
    /// 装置・異物アイテムの確率テーブルから、ランダム選択処理のフォーマット
    /// </summary>
    private int GetEquipItemRandomGradePer(List<int> equipItemPerList) {
        int rand = Random.Range(0, 1000);
        int gradeIdx = 0;
        int i = 0;
        foreach(int per in equipItemPerList) {
            if(rand < per) {
                gradeIdx = i;
                break;
            }
            rand -= per;
            i++;
        }
        return gradeIdx;
    }
    /// <summary>
    /// ゴブリン・鉱石アイテムの確率テーブルから、ランダム選択処理のフォーマット
    /// </summary>
    private Etc.NoshowInvItem GetMiningItemRandomGradePer(Etc.NoshowInvItem type, List<int> miningItemPerList) {
        int rand = Random.Range(0, 1000);
        int idx = 0;
        foreach(int per in miningItemPerList) {
            if(rand < per) {
                return (type == Etc.NoshowInvItem.Goblin)? Etc.GetGoblinInvItem(idx)
                    : Etc.GetOreInvItem(idx); // (type == Etc.NoshowInvItem.Ore)?
            }
            rand -= per;
            idx++;
        }
        return 0;
    }

    #region RANDOM ITEM SELECT
    public int GetRandomCoin()
        => Mathf.RoundToInt(Random.Range(CoinMin, CoinMax + 1) / 5) * 5; //* ５倍数
    public int GetRandomDiamond() 
        => Mathf.RoundToInt(Random.Range(DiamondMin, DiamondMax + 1) / 5) * 5; //* ５倍数
    public ItemSO[] GetRandomEquipDatas() {
        var rwdDt = HM._.rwlm.RwdItemDt;
        int rand = Random.Range(0, 2 + 1);
        return rand == 0? rwdDt.WeaponDatas : rand == 1? rwdDt.ShoesDatas: rwdDt.RingDatas;
    }
    public int GetRandomEquipGrade()
        => GetEquipItemRandomGradePer(RwdGradeTb.EquipPerList);
    public int GetRandomRelicGrade()
        => GetEquipItemRandomGradePer(RwdGradeTb.RelicPerList);
    public Etc.NoshowInvItem GetRandomGoblinGrade()
        => GetMiningItemRandomGradePer(Etc.NoshowInvItem.Goblin, RwdGradeTb.GoblinPerList);
    public Etc.NoshowInvItem GetRandomOreGrade()
        => GetMiningItemRandomGradePer(Etc.NoshowInvItem.Ore, RwdGradeTb.OrePerList);
    public Etc.ConsumableItem GetRandomConsumeItem()
        => Etc.GetConsumableItem(Random.Range(0, 4));
    #endregion
#endregion
}