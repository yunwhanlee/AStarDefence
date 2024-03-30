using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SkillTreeCate { Warrior, Archer, Magician, Utility }

/// <summary>
/// スキルツリーアイコンのデータ
/// </summary>
[System.Serializable]
public class SkillTree {
    [field:SerializeField] public int Id {get; private set;}
    [field:SerializeField] public SkillTreeCate Cate {get; private set;}
    [field:SerializeField] public Image IconImg {get; private set;}
    [field:SerializeField] public Image Border {get; private set;}
    [field:SerializeField] public GameObject Dim {get; private set;}
    [field:SerializeField] public bool IsLock {
        get {
            switch(Cate) {
                case SkillTreeCate.Warrior:  return DM._.DB.SkillTreeDB.IsLockWarriorSTs[Id];
                case SkillTreeCate.Archer:   return DM._.DB.SkillTreeDB.IsLockArcherSTs[Id];
                case SkillTreeCate.Magician: return DM._.DB.SkillTreeDB.IsLockMagicianSTs[Id];
                case SkillTreeCate.Utility:  return DM._.DB.SkillTreeDB.IsLockUtilitySTs[Id];
            }
            return false;
        } 
        set {
            switch(Cate) {
                case SkillTreeCate.Warrior:  DM._.DB.SkillTreeDB.IsLockWarriorSTs[Id] = value; break;
                case SkillTreeCate.Archer:   DM._.DB.SkillTreeDB.IsLockArcherSTs[Id] = value; break;
                case SkillTreeCate.Magician: DM._.DB.SkillTreeDB.IsLockMagicianSTs[Id] = value; break;
                case SkillTreeCate.Utility:  DM._.DB.SkillTreeDB.IsLockUtilitySTs[Id] = value; break;
            }
        } 
    }

    public void InitBorderUI() => Border.color = Color.white;
    public void UpdateDimUI() => Dim.SetActive(IsLock);
}

public class SkillTreeUIManager : MonoBehaviour {
    [field:SerializeField] public SkillTreeCate Cate {get; private set;}
    [field:SerializeField] public int CurIdx {get; private set;}

    [field: SerializeField] public Color[] SkillIconBgColors {get; private set;}

    //* Data
    [field:SerializeField] public SettingSkillTreeData WarriorSkillTreeSO {get; private set;}
    [field:SerializeField] public SettingSkillTreeData ArcherSkillTreeSO {get; private set;}
    [field:SerializeField] public SettingSkillTreeData MagicianSkillTreeSO {get; private set;}
    [field:SerializeField] public SettingSkillTreeData UtilitySkillTreeSO {get; private set;}

    //* Element
    [field:SerializeField] public BgPatternController BgPatternCtrl {get; private set;}
    [field:SerializeField] public SkillTree[] WarriorSkillTrees {get; private set;}
    [field:SerializeField] public SkillTree[] ArcherSkillTrees {get; private set;}
    [field:SerializeField] public SkillTree[] MagicianSkillTrees {get; private set;}
    [field:SerializeField] public SkillTree[] UtilitySkillTrees {get; private set;}

    //* UI
    [field:SerializeField] public GameObject WindowObj {get; private set;}
    [field:SerializeField] public TextMeshProUGUI MySkillPointTxt {get; private set;}
    [field:SerializeField] public Image SkillIconBgImg {get; private set;}
    [field:SerializeField] public Image CurSkillIconImg {get; private set;}
    [field:SerializeField] public TextMeshProUGUI SkillNameTxt {get; private set;}
    [field:SerializeField] public TextMeshProUGUI SkillInfoTxt {get; private set;}
    [field:SerializeField] public TextMeshProUGUI NeededSkillPointTxt {get; private set;}
    [field:SerializeField] public GameObject BurstStarEF {get; private set;}

    void Start() {
        UpdateMyPointTxt();
        OnClickWarriorSkillTreeBtn(0);
        UpdateLock();
    }

#region EVENT
    public void OnClickSkillTreeIconBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(true);
        UpdateMyPointTxt();
    }
    public void OnClickClosePopUpBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(false);
    }

    public void OnClickResetSkillPointBtn() {
        int refundSkillPoint = 0;

        //* 全てのスキルを回して返金ポイント確認
        refundSkillPoint += GetRefundSkillPoint(WarriorSkillTrees);
        refundSkillPoint += GetRefundSkillPoint(ArcherSkillTrees);
        refundSkillPoint += GetRefundSkillPoint(MagicianSkillTrees);
        refundSkillPoint += GetRefundSkillPoint(UtilitySkillTrees);        

        //* 実際にポイント戻す
        HM._.SkillPoint += refundSkillPoint;

        //* UI 最新化
        UpdateMyPointTxt();
        InitSelect();
        UpdateLock();
        SM._.SfxPlay(SM.SFX.WaveStartSFX);
        HM._.hui.ShowMsgNotice($"스킬포인트 초기화 완료! {refundSkillPoint}포인트 반환");

    }
    public void OnClickLearnSkillBtn() {
        SkillTree curST = GetCurSkill(Cate, CurIdx);
        SkillTreeData curSTDataSO = GetCurSkillDataSO(Cate, CurIdx);
        Debug.Log($"LEARN SKILL curST.IsLock= {curST.IsLock}");

        //* 以前のスキルをアンロックしなかったら、習得できない
        if(CurIdx > 0) {
            SkillTree befST = GetCurSkill(Cate, CurIdx - 1);
            if(befST.IsLock) {
                HM._.hui.ShowMsgError("이전스킬 획득 필요!");
                return;
            }
        }

        if(curST.IsLock) {
            if(HM._.SkillPoint >= curSTDataSO.Cost) {
                HM._.SkillPoint -= curSTDataSO.Cost;
                curST.IsLock = false;
                curST.Dim.SetActive(false);
                SM._.SfxPlay(SM.SFX.UpgradeSFX);
                ActiveBurstStarEF();
                HM._.hui.ShowMsgNotice("스킬 획득 완료!");
            }
            else {
                HM._.hui.ShowMsgError("스킬 포인트 부족!");
            }
        }
        else {
            HM._.hui.ShowMsgError("이미 획득한 스킬.");
        }

        UpdateMyPointTxt();
    }

    public void OnClickWarriorSkillTreeBtn(int idx) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        SetCurSkill(SkillTreeCate.Warrior, idx);
        SetUI(idx, SkillTreeCate.Warrior);
    }
    public void OnClickArcherSkillTreeBtn(int idx) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        SetCurSkill(SkillTreeCate.Archer, idx);
        SetUI(idx, SkillTreeCate.Archer);
    }
    public void OnClickMagicianSkillTreeBtn(int idx) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        SetCurSkill(SkillTreeCate.Magician, idx);
        SetUI(idx, SkillTreeCate.Magician);
    }
    public void OnClickUtilitySkillTreeBtn(int idx) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        SetCurSkill(SkillTreeCate.Utility, idx);
        SetUI(idx, SkillTreeCate.Utility);
    }
#endregion

#region FUNC
    private void ActiveBurstStarEF() {
        BurstStarEF.GetComponent<ParticleImage>().Stop();
        BurstStarEF.GetComponent<ParticleImage>().Play();
    }
    private SkillTree GetCurSkill(SkillTreeCate cate, int idx) {
        return cate switch {
            SkillTreeCate.Warrior => WarriorSkillTrees[idx],
            SkillTreeCate.Archer => ArcherSkillTrees[idx],
            SkillTreeCate.Magician => MagicianSkillTrees[idx],
            SkillTreeCate.Utility => UtilitySkillTrees[idx],
            _ => null,
        };
    }

    private SkillTreeData GetCurSkillDataSO(SkillTreeCate cate, int idx) {
        switch(cate) {
            case SkillTreeCate.Warrior:
                return WarriorSkillTreeSO.Datas[idx];
            case SkillTreeCate.Archer:
                return ArcherSkillTreeSO.Datas[idx];
            case SkillTreeCate.Magician:
                return MagicianSkillTreeSO.Datas[idx];
            case SkillTreeCate.Utility:
                return UtilitySkillTreeSO.Datas[idx];
        }
        return null;
    }

    private void SetCurSkill(SkillTreeCate cate, int idx) {
        Cate = cate;
        CurIdx = idx;
    }

    private void UpdateMyPointTxt() {
        MySkillPointTxt.text = $"{HM._.SkillPoint}";
    }

    private int GetRefundSkillPoint(SkillTree[] skillTrees) {
        int refund = 0;
        Array.ForEach(skillTrees, skt => {
            if(!skt.IsLock) {
                skt.IsLock = true;
                refund += GetCurSkillDataSO(skt.Cate, skt.Id).Cost;
            }
        });
        return refund;
    }


    private void SetUI(int idx, SkillTreeCate cate) {
        SkillTree curST = GetCurSkill(cate, idx);
        SkillTreeData  curSTDataSO = GetCurSkillDataSO(cate, idx);

        InitSelect();

        //* 背景とアイコン背景色
        BgPatternCtrl.BgImg.color = BgPatternCtrl.SkillCateColors[(int)cate];
        SkillIconBgImg.color = SkillIconBgColors[(int)cate];
        curST.Border.color = SkillIconBgColors[(int)cate];
        //* スキルボックス UI
        CurSkillIconImg.sprite = curST.IconImg.sprite;
        SkillNameTxt.text = curSTDataSO.Name;
        SkillInfoTxt.text = curSTDataSO.Description;
        NeededSkillPointTxt.text = $"{curSTDataSO.Cost}";
    }

    private void InitSelect() {
        Array.ForEach(WarriorSkillTrees, skt => skt.InitBorderUI());
        Array.ForEach(ArcherSkillTrees, skt => skt.InitBorderUI());
        Array.ForEach(MagicianSkillTrees, skt => skt.InitBorderUI());
        Array.ForEach(UtilitySkillTrees, skt => skt.InitBorderUI());
    }

    private void UpdateLock() {
        Array.ForEach(WarriorSkillTrees, skt => skt.UpdateDimUI());
        Array.ForEach(ArcherSkillTrees, skt => skt.UpdateDimUI());
        Array.ForEach(MagicianSkillTrees, skt => skt.UpdateDimUI());
        Array.ForEach(UtilitySkillTrees, skt => skt.UpdateDimUI());
    }
#endregion
}
