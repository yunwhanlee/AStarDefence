using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class SM : MonoBehaviour {
    static public SM _;

    public enum BGM {
        //* HOME
        HomeBGM,
        //* GAME
        ForestBGM,
        DesertBGM,
        SeaBGM,
        UndeadBGM,
        HellBGM,
        GoblinDungeonBGM,
    }

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
        DecreaseLife,
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
        FrostNovaSFX,
        LightningNovaSFX,
        // Hit
        HitSFX,
        HitSFX2,
        HitSFX3,
        MetalHitSFX,
        MetalHitSFX2,
        EnemyDeadSFX,
        EnemyDeadSFX2,
        BossKilledSFX,
        // Skill
        RageSFX,
        CheerUpSFX,
        WheelWindSFX,
        RoarASFX,
        RoarBSFX,
        FireExplosionSFX,
        MagicCircle1SFX,
        MagicCircle2SFX,
        MagicCircle3SFX,
        LaserSFX,
        BigbangSFX,
        PassArrowSFX,
        ArrowRainSFX,

        //* UI
        RewardSFX,
        ClickSFX,
        UpgradeSFX,
        CompleteSFX,
        ErrorSFX,
        StageSelectSFX,
        ItemPickSFX,
        InvEquipSFX,
        InvUnEquipSFX,
        InvStoneSFX,
        LevelUpSFX,
        BossSpawnSFX,
        CountdownSFX,
    }

    //* BGM
    [field: Header("BGM")]
    [field: SerializeField] public AudioSource BgmPlayer {get; set;}
    [field: SerializeField] AudioClip HomeBGM {get; set;}
    [field: SerializeField] AudioClip ForestBGM {get; set;}
    [field: SerializeField] AudioClip DesertBGM {get; set;}
    [field: SerializeField] AudioClip SeaBGM {get; set;}
    [field: SerializeField] AudioClip UndeadBGM {get; set;}
    [field: SerializeField] AudioClip HellBGM {get; set;}
    [field: SerializeField] AudioClip GoblinDungeonBGM {get; set;}

    [field: Header("SFX")]
    [field: SerializeField] public GameObject SfxObj {get; set;}
    [field: SerializeField] public AudioSource[] SfxPlayers {get; set;}
    [field: SerializeField] AudioClip[] SfxClips {get; set;}
    [field: SerializeField] public float SfxVolume {get; set;}
    [field: SerializeField] public int Channels {get; set;}
    [field: SerializeField] public int ChannelIdx {get; set;}

    void Awake() {
        Singleton();
        InitSfx();
    } 

    private void Singleton(){
        if(_ == null) {
            _ = this;
            DontDestroyOnLoad(_);
        }
        else
            Destroy(gameObject);
    }

#region EVENT
        public void OnClickCloseBtnSFX() => SfxPlay(SFX.ClickSFX);
#endregion

#region FUNC
    private void InitSfx() {
        SfxObj = new GameObject("SfxPlayer");
        SfxObj.transform.parent = transform;
        SfxPlayers = new AudioSource[Channels];

        for(int i = 0; i < SfxPlayers.Length; i++) {
            SfxPlayers[i] = SfxObj.AddComponent<AudioSource>();
            SfxPlayers[i].playOnAwake = false;
            SfxPlayers[i].volume = SfxVolume;
        }
    }

    public void SetVolumeBGM(float volume) {
        BgmPlayer.volume = volume;
        BgmPlayer.gameObject.SetActive(volume > 0);
        BgmPlay(BGM.HomeBGM);
    }
    public void SetVolumeSFX(float volume) {
        SfxVolume = volume;
        for(int i = 0; i < SfxPlayers.Length; i++) {
            SfxPlayers[i].volume = volume;
        }
        SfxObj.SetActive(volume > 0);
    }

    private void SetBGM(AudioClip bgm) {
        BgmPlayer.clip = bgm;
    }

    public void BgmPlay(BGM bgm) {
        //* ON・OFF チェック
        if(!BgmPlayer.gameObject.activeSelf)
            return;

        //* BGM 設定
        if(bgm == BGM.HomeBGM)
            SetBGM(HomeBGM);
        else if(bgm == BGM.ForestBGM)
            SetBGM(ForestBGM);
        else if(bgm == BGM.DesertBGM)
            SetBGM(DesertBGM);
        else if(bgm == BGM.SeaBGM)
            SetBGM(SeaBGM);
        else if(bgm == BGM.UndeadBGM)
            SetBGM(UndeadBGM);
        else if(bgm == BGM.HellBGM)
            SetBGM(HellBGM);
        else if(bgm == BGM.GoblinDungeonBGM)
            SetBGM(GoblinDungeonBGM);

        //* プレイ
        BgmPlayer.Play();
    }

    public void SfxPlay(SFX sfx, float delay = 0) => StartCoroutine(CoSfxPlay(sfx, delay));

    private IEnumerator CoSfxPlay(SFX sfx, float delay) {
        // //* ON・OFF チェック
        // if(!SfxPlayers.gameObject.activeSelf)
            // yield break;

        //* Delay
        yield return new WaitForSeconds(delay);

        for(int i = 0; i < SfxPlayers.Length; i++) {
            int loopIdx = (i + ChannelIdx) % SfxPlayers.Length;

            //* 現在のAudioPlayerが既に再生中なら、次に進む
            if(SfxPlayers[loopIdx].isPlaying)
                continue;

            //* 空のAutioPlayerへClip再生
            ChannelIdx = loopIdx;

            switch(sfx) {
                case SFX.HitSFX:
                case SFX.HitSFX2:
                case SFX.HitSFX3:
                    SfxPlayers[loopIdx].clip = SfxClips[Random.Range((int)SFX.HitSFX, (int)SFX.HitSFX3 + 1)];
                    break;
                case SFX.MetalHitSFX:
                case SFX.MetalHitSFX2:
                    SfxPlayers[loopIdx].clip = SfxClips[Random.Range((int)SFX.MetalHitSFX, (int)SFX.MetalHitSFX2 + 1)];
                    break;
                case SFX.EnemyDeadSFX:
                case SFX.EnemyDeadSFX2:
                    SfxPlayers[loopIdx].clip = SfxClips[Random.Range((int)SFX.EnemyDeadSFX, (int)SFX.EnemyDeadSFX2 + 1)];
                    break;
                default:
                    SfxPlayers[loopIdx].clip = SfxClips[(int)sfx];
                    break;
            }

            
            SfxPlayers[loopIdx].Play();
            Debug.Log($"SfxPlayers[{loopIdx}]= SfxClips[{SfxPlayers[loopIdx].clip.name}]");
            break; // ループ終了
        }



        // //* GAME
        // if(sfx == SFX.GameStartSFX) SfxPlayers.PlayOneShot(GameStartSFX);
        // if(sfx == SFX.WaveStartSFX) SfxPlayers.PlayOneShot(WaveStartSFX);
        // if(sfx == SFX.GameoverSFX) SfxPlayers.PlayOneShot(GameoverSFX);
        // if(sfx == SFX.UpgradeSFX) SfxPlayers.PlayOneShot(UpgradeSFX);
        // if(sfx == SFX.CreateTowerSFX) SfxPlayers.PlayOneShot(CreateTowerSFX);
        // if(sfx == SFX.DeleteTowerSFX) SfxPlayers.PlayOneShot(DeleteTowerSFX);
        // if(sfx == SFX.BuildSFX) SfxPlayers.PlayOneShot(BuildSFX);
        // if(sfx == SFX.BreakSFX) SfxPlayers.PlayOneShot(BreakSFX);
        // if(sfx == SFX.Merge1SFX) SfxPlayers.PlayOneShot(Merge1SFX);
        // if(sfx == SFX.Merge2SFX) SfxPlayers.PlayOneShot(Merge2SFX);
        // if(sfx == SFX.Merge3SFX) SfxPlayers.PlayOneShot(Merge3SFX);
        // if(sfx == SFX.Merge4SFX) SfxPlayers.PlayOneShot(Merge4SFX);
        // if(sfx == SFX.Merge5SFX) SfxPlayers.PlayOneShot(Merge5SFX);
        // if(sfx == SFX.DecreaseLife) SfxPlayers.PlayOneShot(DecreaseLife);
        // // Attack
        // if(sfx == SFX.SwordSFX) SfxPlayers.PlayOneShot(SwordSFX);
        // if(sfx == SFX.ArrowSFX) SfxPlayers.PlayOneShot(ArrowSFX);
        // if(sfx == SFX.Magic1SFX) SfxPlayers.PlayOneShot(Magic1SFX);
        // if(sfx == SFX.Magic2SFX) SfxPlayers.PlayOneShot(Magic2SFX);
        // if(sfx == SFX.Magic3SFX) SfxPlayers.PlayOneShot(Magic3SFX);
        // if(sfx == SFX.Magic4SFX) SfxPlayers.PlayOneShot(Magic4SFX);
        // if(sfx == SFX.Magic5SFX) SfxPlayers.PlayOneShot(Magic5SFX);
        // if(sfx == SFX.Magic6SFX) SfxPlayers.PlayOneShot(Magic6SFX);
        // if(sfx == SFX.CCFrostSFX) SfxPlayers.PlayOneShot(CCFrostSFX);
        // if(sfx == SFX.CCLightningSFX) SfxPlayers.PlayOneShot(CCLightningSFX);
        // if(sfx == SFX.FrostNovaSFX) SfxPlayers.PlayOneShot(FrostNovaSFX);
        // if(sfx == SFX.LightningNovaSFX) SfxPlayers.PlayOneShot(LightningNovaSFX);
        // // Hit
        // if(sfx == SFX.HitSFX) SfxPlayers.PlayOneShot(HitSFXs[Random.Range(0, HitSFXs.Length)]);
        // if(sfx == SFX.MetalHitSFX) SfxPlayers.PlayOneShot(MetalHitSFXs[Random.Range(0, MetalHitSFXs.Length)]);
        // if(sfx == SFX.EnemyDeadSFX) SfxPlayers.PlayOneShot(EnemyDeadSFXs[Random.Range(0, EnemyDeadSFXs.Length)]);
        // if(sfx == SFX.BossKilledSFX) SfxPlayers.PlayOneShot(BossKilledSFX);
        // // Skill
        // if(sfx == SFX.RageSFX) SfxPlayers.PlayOneShot(RageSFX);
        // if(sfx == SFX.CheerUpSFX) SfxPlayers.PlayOneShot(CheerUpSFX);
        // if(sfx == SFX.WheelWindSFX) SfxPlayers.PlayOneShot(WheelWindSFX);
        // if(sfx == SFX.RoarASFX) SfxPlayers.PlayOneShot(RoarASFX);
        // if(sfx == SFX.RoarBSFX) SfxPlayers.PlayOneShot(RoarBSFX);
        // if(sfx == SFX.FireExplosionSFX) SfxPlayers.PlayOneShot(FireExplosionSFX);
        // if(sfx == SFX.MagicCircle1SFX) SfxPlayers.PlayOneShot(MagicCircle1SFX);
        // if(sfx == SFX.MagicCircle2SFX) SfxPlayers.PlayOneShot(MagicCircle2SFX);
        // if(sfx == SFX.MagicCircle3SFX) SfxPlayers.PlayOneShot(MagicCircle3SFX);
        // if(sfx == SFX.LaserSFX) SfxPlayers.PlayOneShot(LaserSFX);
        // if(sfx == SFX.BigbangSFX) SfxPlayers.PlayOneShot(BigbangSFX);
        // if(sfx == SFX.PassArrowSFX) SfxPlayers.PlayOneShot(PassArrowSFX);
        // if(sfx == SFX.ArrowRainSFX) SfxPlayers.PlayOneShot(ArrowRainSFX);

        // //* UI   
        // if(sfx == SFX.RewardSFX) SfxPlayers.PlayOneShot(RewardSFX);
        // if(sfx == SFX.ClickSFX) SfxPlayers.PlayOneShot(ClickSFX);
        // if(sfx == SFX.CompleteSFX) SfxPlayers.PlayOneShot(CompleteSFX);
        // if(sfx == SFX.ErrorSFX) SfxPlayers.PlayOneShot(ErrorSFX);
        // if(sfx == SFX.StageSelectSFX) SfxPlayers.PlayOneShot(StageSelectSFX);
        // if(sfx == SFX.ItemPickSFX) SfxPlayers.PlayOneShot(ItemPickSFX);
        // if(sfx == SFX.InvEquipSFX) SfxPlayers.PlayOneShot(InvEquipSFX);
        // if(sfx == SFX.InvUnEquipSFX) SfxPlayers.PlayOneShot(InvUnEquipSFX);
        // if(sfx == SFX.InvStoneSFX) SfxPlayers.PlayOneShot(InvStoneSFX);
        // if(sfx == SFX.LevelUpSFX) SfxPlayers.PlayOneShot(LevelUpSFX);
        // if(sfx == SFX.BossSpawnSFX) SfxPlayers.PlayOneShot(BossSpawnSFX);
        // if(sfx == SFX.CountdownSFX) SfxPlayers.PlayOneShot(CountdownSFX);
    }
#endregion
}