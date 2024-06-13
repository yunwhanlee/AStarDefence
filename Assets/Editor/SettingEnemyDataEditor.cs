using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
/// <summary>
/// 敵SOデータのInspectorViewカスタマイズ
/// </summary>
[CustomEditor(typeof(SettingEnemyData))]
public class SettingEnemyDataEditor : Editor {
    #region HP_TEMPLATE
    readonly int[] tempEnemyHpDts = {
        10,
        15,
        23,
        31,
        43 / 2,
        53,
        68,
        2040, // 8 Wave (1)
        88,
        108,
        128,
        149,
        172 / 2,
        195,
        223,
        8920, // 16 Wave (2)
        254,
        288,
        329,
        369,
        410 / 2,
        439,
        477,
        23850, // 24 Wave (3)
        515,
        557,
        598,
        643,
        689 / 2,
        725,
        776,
        46560, // 32 Wave (4)
        823,
        871,
        919,
        970,
        1022 / 2,
        1063,
        1120,
        78400, // 40 Wave (5)
        1181,
        1242,
        1304,
        1359,
        1421 / 2,
        1521,
        1595,
        127600, // 48 Wave (6)
        1661,
        1731,
        1804,
        1878,
        1955 / 2,
        2026,
        2112,
        190080, // 56 Wave (7)
        2201,
        2290,
        2382,
        2478,
        2583 / 2,
        2664,
        2790,
        279000, // 56 Wave (8)

    };
    #endregion

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        SettingEnemyData dt = (SettingEnemyData)target;

        //* レベル昇順適用ボタン
        if(GUILayout.Button("レベル昇順適用")) {
            int i = 0;
            foreach (var enemyDt in dt.Waves) {
                enemyDt.Lv = ++i;
                EditorUtility.SetDirty(dt); // 변경 내용을 저장
            }
        }

        //* 체력 비율 적용 버튼
        if (GUILayout.Button("HP 比率適用")) {
            int i = 0;
            foreach (var enemyDt in dt.Waves) {
                // 입력된 값에 따라 체력을 비례적으로 증가시킴
                enemyDt.Hp = Mathf.RoundToInt(tempEnemyHpDts[i++] * dt.HpRatio);
                EditorUtility.SetDirty(dt); // 변경 내용 저장
            }
        }

        //* 무한던전 데이터 설정
        if (GUILayout.Button("無限ダンジョンWaveList設定")) {
            List<EnemyData> infiniteEnemyWaveDtList = new List<EnemyData>();

            //* 全て登録したSOのWavesデータをまとめてリストにする
            foreach(var applyEnemyDt in dt.ApplyInfiniteEnemyDts) {
                foreach(var waveDt in applyEnemyDt.Waves) {
                    infiniteEnemyWaveDtList.Add(waveDt);
                }
            }

            Debug.Log("infiniteEnemyWaveDtList.Count= " + infiniteEnemyWaveDtList.Count);
            
            //* InfiniteWaveへまとめたリストを元に設定
            const int WaveCnt = 1000;
            const int StartHp = 7;
            const int StartBossHpTime = 30;
            int hp = StartHp;
            float hpRatio = 1.5f;

            //* 配列初期化
            dt.Waves = new EnemyData[WaveCnt];

            for(int i = 0; i < WaveCnt; i++) {
                EnemyData enemyDt = infiniteEnemyWaveDtList[i % infiniteEnemyWaveDtList.Count];
                Debug.Log($"enemyDt[{i}]= " + enemyDt.Name);
                dt.Waves[i] = new EnemyData(); // 각 요소에 EnemyData 객체를 생성하여 할당
                string simbolTxt = "";
                if(i >= infiniteEnemyWaveDtList.Count) {
                    Debug.Log($"{i} Infinite Set HP Wave Cycle={i / infiniteEnemyWaveDtList.Count}");
                    switch(i / infiniteEnemyWaveDtList.Count) {
                        case 1: simbolTxt = "(정예)"; break;
                        case 2: simbolTxt = "(유니크)"; break;
                        case 3: simbolTxt = "(전설)"; break;
                        case 4: simbolTxt = "(신화)"; break;
                    }
                }
                dt.Waves[i].Name = enemyDt.Name + "\n" + simbolTxt;
                dt.Waves[i].Spr = enemyDt.Spr;
                dt.Waves[i].Lv = i + 1;

                //* Hp 比率
                if(i == 0) hpRatio = 1.5f;
                if(i == 1) hpRatio = 1.45f;
                if(i == 2) hpRatio = 1.41f;
                if(i == 3) hpRatio = 1.375f;
                if(i == 4) hpRatio = 1.35f;
                if(i == 5) hpRatio = 1.33f;
                if(i == 6) hpRatio = 1.31f;
                if(i == 7) hpRatio = 1.29675f;
                if(i == 8) hpRatio = 1.252f;
                if(i == 9) hpRatio = 1.23f;
                if(i == 10) hpRatio = 1.22f;
                if(i == 11) hpRatio = 1.21f;
                if(i == 12) hpRatio = 1.2f;
                if(i == 13) hpRatio = 1.19f;
                if(i == 14) hpRatio = 1.17f;
                if(i == 15) hpRatio = 1.15f;
                if(i == 16) hpRatio = 1.14f;
                if(i == 17) hpRatio = 1.12f;
                if(i == 18) hpRatio = 1.11f;
                if(i == 19) hpRatio = 1.08f;
                if(i == 20) hpRatio = 1.06f;
                if(i == 21) hpRatio = 1.04f;
                if(i == 22) hpRatio = 1.02f;
                if(i == 23) hpRatio = 1.1f;
                if(i == 24) hpRatio = 1.098f;
                if(i == 25) hpRatio = 1.095f;
                if(i == 26) hpRatio = 1.092f;
                if(i == 27) hpRatio = 1.09f;
                if(i == 28) hpRatio = 1.0895f;
                if(i == 29) hpRatio = 1.08875f;
                if(i == 39) hpRatio = 1.08687f;
                if(i > 39 && i <= 49) hpRatio   = 1.07875f;
                if(i > 49 && i <= 59) hpRatio   = 1.0725f;
                if(i > 59 && i <= 69) hpRatio   = 1.06875f;
                if(i > 69 && i <= 79) hpRatio   = 1.06f;
                if(i > 79 && i <= 89) hpRatio   = 1.055f;
                if(i > 89 && i <= 99) hpRatio   = 1.045f;
                if(i > 99 && i <= 139) hpRatio  = 1.04f;
                if(i > 139 && i <= 179) hpRatio = 1.025f;
                if(i > 179 && i <= 209) hpRatio = 1.01f;
                if(i > 209 && i <= 309) hpRatio = 1.0095f;
                if(i > 309 && i <= 409) hpRatio = 1.00825f;
                if(i > 409 && i <= 509) hpRatio = 1.007f;
                if(i > 509 && i <= 609) hpRatio = 1.0055f;
                if(i > 609 && i <= 709) hpRatio = 1.00425f;
                if(i > 709 && i <= 809) hpRatio = 1.003f;
                else if(i > 209 && i <= WaveCnt) hpRatio = 1.00215f;

                hp = Mathf.RoundToInt(hp * hpRatio);

                int enemyIdx = i + 1;
                //* Type & Speed
                const int OneCycle = Config.BOSS_SPAWN_CNT;
                if(enemyIdx % OneCycle == Config.FLIGHT_SPAWN_CNT) {
                    dt.Waves[i].Type = EnemyType.Flight;
                    dt.Waves[i].Hp = hp / 2;
                    dt.Waves[i].Speed = 1.5f;
                }
                else if(enemyIdx % OneCycle == 0) {
                    dt.Waves[i].Type = EnemyType.Boss;
                    dt.Waves[i].Hp = hp * (StartBossHpTime + (enemyIdx % Config.BOSS_SPAWN_CNT * 9));
                    dt.Waves[i].Speed = 1;
                }
                else {
                    dt.Waves[i].Type = EnemyType.Land;
                    dt.Waves[i].Hp = hp;
                    dt.Waves[i].Speed = 1.5f;
                }
            }
        }
    }
}
#endif