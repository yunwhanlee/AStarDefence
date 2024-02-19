using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour {
    public static GM _; //* Global

    public PathFindManager pfm;
    public EnemyManager emm;

    void Awake() {
        _ = this; //* Global化 値 代入
    }
}
