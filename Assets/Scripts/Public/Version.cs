using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Version : MonoBehaviour {
    public static Version _;
    public int Major = 1;
    public int Minor = 1;
    public int Revision = 6;
    public string Date = "2024.05.23";

    void Awake() {
        _ = this;
    }
}
