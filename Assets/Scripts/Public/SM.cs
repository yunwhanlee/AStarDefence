using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM : MonoBehaviour {
    static public SM _;

    public enum SFX {
        //* GAME
        GameStartSFX,
        WaveStartSFX,
        GameoverSFX,
        CreateTowerSFX,
        DeleteTowerSFX,
        BuildSFX,
        BreakSFX,
        Merge1SFX,
        Merge2SFX,
        Merge3SFX,
        Merge4SFX,
        Merge5SFX,
        // Attack
        SwordSFX,
        ArrowSFX,
        Magic1SFX,
        Magic2SFX,
        Magic3SFX,
        Magic4SFX,
        Magic5SFX,
        Magic6SFX,
        CCFrostSFX,
        CCLightningSFX,
        // Hit
        HitSFX,
        EnemyDeadSFX,
        BossKilledSFX,
        // Skill
        FireExplosionSFX,
        MagicCircle1SFX,
        MagicCircle2SFX,
        MagicCircle3SFX,
        LaserSFX,
        BigbangSFX,
        PassArrowSFX,
        ArrowRainSFX,

        //* UI
        ClickSFX,
        UpgradeSFX,
        CompleteSFX,
        ErrorSFX,
        StageSelectSFX,
        ItemPickSFX,
    }

    [field: SerializeField] public AudioSource SfxPlayer {get; set;}
    //* GAME
    [field: SerializeField] AudioClip GameStartSFX {get; set;}
    [field: SerializeField] AudioClip WaveStartSFX {get; set;}
    [field: SerializeField] AudioClip GameoverSFX {get; set;}
    [field: SerializeField] AudioClip CreateTowerSFX {get; set;}
    [field: SerializeField] AudioClip DeleteTowerSFX {get; set;}
    [field: SerializeField] AudioClip BuildSFX {get; set;}
    [field: SerializeField] AudioClip BreakSFX {get; set;}
    [field: SerializeField] AudioClip Merge1SFX {get; set;}
    [field: SerializeField] AudioClip Merge2SFX {get; set;}
    [field: SerializeField] AudioClip Merge3SFX {get; set;}
    [field: SerializeField] AudioClip Merge4SFX {get; set;}
    [field: SerializeField] AudioClip Merge5SFX {get; set;}
    // Attack
    [field: SerializeField] AudioClip SwordSFX {get; set;}
    [field: SerializeField] AudioClip ArrowSFX {get; set;}
    [field: SerializeField] AudioClip Magic1SFX {get; set;}
    [field: SerializeField] AudioClip Magic2SFX {get; set;}
    [field: SerializeField] AudioClip Magic3SFX {get; set;}
    [field: SerializeField] AudioClip Magic4SFX {get; set;}
    [field: SerializeField] AudioClip Magic5SFX {get; set;}
    [field: SerializeField] AudioClip Magic6SFX {get; set;}
    [field: SerializeField] AudioClip CCFrostSFX {get; set;}
    [field: SerializeField] AudioClip CCLightningSFX {get; set;}
    // Hit
    [field: SerializeField] AudioClip[] HitSFXs {get; set;}
    [field: SerializeField] AudioClip[] EnemyDeadSFXs {get; set;}
    [field: SerializeField] AudioClip BossKilledSFX {get; set;}
    // Skill
    [field: SerializeField] AudioClip FireExplosionSFX {get; set;}
    [field: SerializeField] AudioClip MagicCircle1SFX {get; set;}
    [field: SerializeField] AudioClip MagicCircle2SFX {get; set;}
    [field: SerializeField] AudioClip MagicCircle3SFX {get; set;}
    [field: SerializeField] AudioClip LaserSFX {get; set;}
    [field: SerializeField] AudioClip BigbangSFX {get; set;}
    [field: SerializeField] AudioClip PassArrowSFX {get; set;}
    [field: SerializeField] AudioClip ArrowRainSFX {get; set;}

    //* UI
    [field: SerializeField] AudioClip ClickSFX {get; set;}
    [field: SerializeField] AudioClip UpgradeSFX {get; set;}
    [field: SerializeField] AudioClip CompleteSFX {get; set;}
    [field: SerializeField] AudioClip ErrorSFX {get; set;}
    [field: SerializeField] AudioClip StageSelectSFX {get; set;}
    [field: SerializeField] AudioClip ItemPickSFX {get; set;}

    void Awake() => singleton();

    private void singleton(){
        if(_ == null) {
            _ = this;
            DontDestroyOnLoad(_);
        }
        else
            Destroy(gameObject);
    }

    public void SfxPlay(SFX sfx, float delay = 0) {
        //* GAME
        if(sfx == SFX.GameStartSFX) SfxPlayer.PlayOneShot(GameStartSFX);
        if(sfx == SFX.WaveStartSFX) SfxPlayer.PlayOneShot(WaveStartSFX);
        if(sfx == SFX.GameoverSFX) SfxPlayer.PlayOneShot(GameoverSFX);
        if(sfx == SFX.UpgradeSFX) SfxPlayer.PlayOneShot(UpgradeSFX);
        if(sfx == SFX.CreateTowerSFX) SfxPlayer.PlayOneShot(CreateTowerSFX);
        if(sfx == SFX.DeleteTowerSFX) SfxPlayer.PlayOneShot(DeleteTowerSFX);
        if(sfx == SFX.BuildSFX) SfxPlayer.PlayOneShot(BuildSFX);
        if(sfx == SFX.BreakSFX) SfxPlayer.PlayOneShot(BreakSFX);
        if(sfx == SFX.Merge1SFX) SfxPlayer.PlayOneShot(Merge1SFX);
        if(sfx == SFX.Merge2SFX) SfxPlayer.PlayOneShot(Merge2SFX);
        if(sfx == SFX.Merge3SFX) SfxPlayer.PlayOneShot(Merge3SFX);
        if(sfx == SFX.Merge4SFX) SfxPlayer.PlayOneShot(Merge4SFX);
        if(sfx == SFX.Merge5SFX) SfxPlayer.PlayOneShot(Merge5SFX);
        // Attack
        if(sfx == SFX.SwordSFX) SfxPlayer.PlayOneShot(SwordSFX);
        if(sfx == SFX.ArrowSFX) SfxPlayer.PlayOneShot(ArrowSFX);
        if(sfx == SFX.Magic1SFX) SfxPlayer.PlayOneShot(Magic1SFX);
        if(sfx == SFX.Magic2SFX) SfxPlayer.PlayOneShot(Magic2SFX);
        if(sfx == SFX.Magic3SFX) SfxPlayer.PlayOneShot(Magic3SFX);
        if(sfx == SFX.Magic4SFX) SfxPlayer.PlayOneShot(Magic4SFX);
        if(sfx == SFX.Magic5SFX) SfxPlayer.PlayOneShot(Magic5SFX);
        if(sfx == SFX.Magic6SFX) SfxPlayer.PlayOneShot(Magic6SFX);
        if(sfx == SFX.CCFrostSFX) SfxPlayer.PlayOneShot(CCFrostSFX);
        if(sfx == SFX.CCLightningSFX) SfxPlayer.PlayOneShot(CCLightningSFX);
        // Hit
        if(sfx == SFX.HitSFX) SfxPlayer.PlayOneShot(HitSFXs[Random.Range(0, HitSFXs.Length)]);
        if(sfx == SFX.EnemyDeadSFX) SfxPlayer.PlayOneShot(EnemyDeadSFXs[Random.Range(0, EnemyDeadSFXs.Length)]);
        if(sfx == SFX.BossKilledSFX) SfxPlayer.PlayOneShot(BossKilledSFX);
        // Skill
        if(sfx == SFX.FireExplosionSFX) SfxPlayer.PlayOneShot(FireExplosionSFX);
        if(sfx == SFX.MagicCircle1SFX) SfxPlayer.PlayOneShot(MagicCircle1SFX);
        if(sfx == SFX.MagicCircle2SFX) SfxPlayer.PlayOneShot(MagicCircle2SFX);
        if(sfx == SFX.MagicCircle3SFX) SfxPlayer.PlayOneShot(MagicCircle3SFX);
        if(sfx == SFX.LaserSFX) SfxPlayer.PlayOneShot(LaserSFX);
        if(sfx == SFX.BigbangSFX) SfxPlayer.PlayOneShot(BigbangSFX);
        if(sfx == SFX.PassArrowSFX) SfxPlayer.PlayOneShot(PassArrowSFX);
        if(sfx == SFX.ArrowRainSFX) SfxPlayer.PlayOneShot(ArrowRainSFX);

        //* UI
        if(sfx == SFX.ClickSFX) SfxPlayer.PlayOneShot(ClickSFX);
        if(sfx == SFX.CompleteSFX) SfxPlayer.PlayOneShot(CompleteSFX);
        if(sfx == SFX.ErrorSFX) SfxPlayer.PlayOneShot(ErrorSFX);
        if(sfx == SFX.StageSelectSFX) SfxPlayer.PlayOneShot(StageSelectSFX);
        if(sfx == SFX.ItemPickSFX) SfxPlayer.PlayOneShot(ItemPickSFX);
    }
}