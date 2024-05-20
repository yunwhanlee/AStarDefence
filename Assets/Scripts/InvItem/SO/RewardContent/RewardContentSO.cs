using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;
using Random = UnityEngine.Random;

#region リワードアイテム％テーブル 構造体
    [Serializable]
    public struct RewardPercentTable {
        [field: Header("-1は固定（必ず出る）")]
        [field: SerializeField] public int Exp {get; private set;}
        [field: SerializeField] public int Coin {get; private set;}
        [field: SerializeField] public int Diamond {get; private set;}
        [field: SerializeField] public int Equip {get; private set;}
        [field: SerializeField] public int Relic {get; private set;}
        [field: SerializeField] public int Goblin {get; private set;}
        [field: SerializeField] public int Ore {get; private set;}
        [field: SerializeField] public int GoldKey {get; private set;}
        [field: SerializeField] public int Clover {get; private set;}
        [field: SerializeField] public int GoldClover {get; private set;}
        [field: SerializeField] public int ConsumeItem {get; private set;}
        [field: SerializeField] public int SoulStone {get; private set;}
        [field: SerializeField] public int MagicStone {get; private set;}
        [field: SerializeField] public int Fame {get; private set;}

        [field: Header("★ステージクリアー専用CHEST")]
        [field: SerializeField] public int ChestCommon {get; private set;}
        [field: SerializeField] public int ChestDiamond {get; private set;}
        [field: SerializeField] public int ChestEquipment {get; private set;}
        [field: SerializeField] public int ChestGold {get; private set;}
        [field: SerializeField] public int ChestPremium {get; private set;}


        /// <summary>
        /// 全ての合計が１０００なのかを検査。 ただし、-1値は固定(必ず出る)なので、-1を0にして切り替えて計算
        /// </summary>
        public readonly int GetTotal() {
            return (Exp == -1? 0 :Exp)
                + (Coin == -1? 0 :Coin) 
                + (Diamond == -1? 0 :Diamond) 
                + (Equip == -1? 0 :Equip) 
                + (Relic == -1? 0 :Relic) 
                + (Goblin == -1? 0 :Goblin) 
                + (Ore == -1? 0 :Ore) 
                + (GoldKey == -1? 0 :GoldKey) 
                + (Clover == -1? 0 :Clover) 
                + (GoldClover == -1? 0 :GoldClover) 
                + (ConsumeItem == -1? 0 :ConsumeItem) 
                + (SoulStone == -1? 0 :SoulStone) 
                + (MagicStone == -1? 0 : MagicStone)
                + (Fame == -1? 0 : Fame)
                + (ChestCommon == -1? 0 : ChestCommon)
                + (ChestDiamond == -1? 0 : ChestDiamond)
                + (ChestEquipment == -1? 0 : ChestEquipment)
                + (ChestGold == -1? 0 : ChestGold)
                + (ChestPremium == -1? 0 : ChestPremium);
        }
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

    [field: Header("Exp 範囲")]
    [field: SerializeField] public int ExpMin {get; private set;}
    [field: SerializeField] public int ExpMax {get; private set;}

    [field: Header("コイン範囲")]
    [field: SerializeField] public int CoinMin {get; private set;}
    [field: SerializeField] public int CoinMax {get; private set;}

    [field: Header("ダイアモンド範囲")]
    [field: SerializeField] public int DiamondMin {get; private set;}
    [field: SerializeField] public int DiamondMax {get; private set;}

    [field: Header("Ore 範囲")]
    [field: SerializeField] public int OreMin {get; private set;}
    [field: SerializeField] public int OreMax {get; private set;}

    [field: Header("GoldKey")]
    [field: SerializeField] public int GoldKeyMin {get; private set;}
    [field: SerializeField] public int GoldKeyMax {get; private set;}
    [field: Header("消費アイテム")]
    [field: SerializeField] public int ConsumeItemMin {get; private set;}
    [field: SerializeField] public int ConsumeItemMax {get; private set;}
    [field: Header("SoulStone")]
    [field: SerializeField] public int SoulStoneMin {get; private set;}
    [field: SerializeField] public int SoulStoneMax {get; private set;}
    [field: Header("MagicStone")]
    [field: SerializeField] public int MagicStoneMin {get; private set;}
    [field: SerializeField] public int MagicStoneMax {get; private set;}
    [field: Header("Chest Common")]
    [field: SerializeField] public int ChestCommonMin {get; private set;}
    [field: SerializeField] public int ChestCommonMax {get; private set;}
    [field: Header("Chest Equipment")]
    [field: SerializeField] public int ChestEquipmentMin {get; private set;}
    [field: SerializeField] public int ChestEquipmentMax {get; private set;}

    [field: Header("Fame 範囲")]
    [field: SerializeField] public int FameMin {get; private set;}
    [field: SerializeField] public int FameMax {get; private set;}

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

    public string GetChestCommonQuantityTxt() 
        => (ChestCommonMin == ChestCommonMax)? $"{ChestCommonMax}" : $"{ChestCommonMin}~{ChestCommonMax}";
    public string GetConsumeItemQuantityTxt()
        => (ConsumeItemMin == ConsumeItemMax)? $"{ConsumeItemMax}" : $"{ConsumeItemMin}~{ConsumeItemMax}";
    public string GetChestEquipmentQuantityTxt()
        => (ChestEquipmentMin == ChestEquipmentMax)? $"{ChestEquipmentMax}" : $"{ChestEquipmentMin}~{ChestEquipmentMax}";
    public string GetGoldKeyQuantityTxt()
        => (GoldKeyMin == GoldKeyMax)? $"{GoldKeyMax}" : $"{GoldKeyMin}~{GoldKeyMax}";
    public string GetMagicStoneQuantityTxt()
        => (MagicStoneMin == MagicStoneMax)? $"{MagicStoneMax}" : $"{MagicStoneMin}~{MagicStoneMax}";
    public string GetSoulStoneQuantityTxt()
        => (SoulStoneMin == SoulStoneMax)? $"{SoulStoneMax}" : $"{SoulStoneMin}~{SoulStoneMax}";

    #region RANDOM ITEM SELECT
    public int GetRandomExp() => Mathf.RoundToInt(Random.Range(ExpMin, ExpMax + 1));
    public int GetRandomCoin() => Mathf.RoundToInt(Random.Range(CoinMin, CoinMax + 1) / 5) * 5; //* ５倍数
    public int GetRandomDiamond() => Mathf.RoundToInt(Random.Range(DiamondMin, DiamondMax + 1) / 5) * 5; //* ５倍数
    public int GetRandomOre() => Mathf.RoundToInt(Random.Range(OreMin, OreMax + 1));
    public int GetRandomFame() => Random.Range(FameMin, FameMax + 1);
    public int GetRandomGoldKeyCnt() => Random.Range(GoldKeyMin, GoldKeyMax + 1);
    public int GetRandomConsumeItemCnt() => Random.Range(ConsumeItemMin, ConsumeItemMax + 1);
    public int GetRandomSoulStoneMaxCnt() => Random.Range(SoulStoneMin, SoulStoneMax + 1);
    public int GetRandomMagicStoneCnt() => Random.Range(MagicStoneMin, MagicStoneMax + 1);
    public int GetRandomChestCommonCnt() => Random.Range(ChestCommonMin, ChestCommonMax + 1);
    public int GetRandomChestEquipmentCnt() => Random.Range(ChestEquipmentMin, ChestEquipmentMax + 1);

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