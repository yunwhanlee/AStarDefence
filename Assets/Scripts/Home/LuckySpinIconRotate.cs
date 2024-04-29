using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckySpinIconRotate : MonoBehaviour {
    private const int SpinPower = 100;

    void Update() {
        //* 回転
        transform.Rotate(new Vector3(0, 0, SpinPower * Time.deltaTime));
    }
}
