using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enum
{
    public class Layer { //* Switch文で使えるため、整数化
        public const int Default = 0, Wall = 6, Board = 7, CCTower = 8;
    }
    public enum Grade {
        None, 
        //* 노말, 레어, 에픽, 유니크, 레전드, 신화, 태초
        Common, Rare, Epic, Unique, Legend, Myth, Prime
        
    }
}
