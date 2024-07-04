// using UnityEngine;
// using Unity.Services.Core;
// using System.Diagnostics;

// public static class Debug {
//     // 전처리 기능 Attribute
//     [Conditional("UNITY_EDITOR")]
//     public static void Break() =>
//         UnityEngine.Debug.Break();

//     [Conditional("UNITY_EDITOR")]
//     public static void ClearDeveloperConsole() =>
//         UnityEngine.Debug.ClearDeveloperConsole();

//     [Conditional("UNITY_EDITOR")] 
//     public static void Log(object msg) =>
//         UnityEngine.Debug.Log(msg);

//     [Conditional("UNITY_EDITOR")] 
//     public static void LogError(object msg) =>
//         UnityEngine.Debug.LogError(msg);

//     [Conditional("UNITY_EDITOR")] 
//     public static void LogWarning(object msg) =>
//         UnityEngine.Debug.LogWarning(msg);

//     [Conditional("UNITY_EDITOR")] 
//     public static void LogException(RequestFailedException msg) =>
//         UnityEngine.Debug.LogException(msg);

//     [Conditional("UNITY_EDITOR")]
//     public static void DrawRay(Vector3 start, Vector3 dir) =>
//         UnityEngine.Debug.DrawRay(start, dir);

//     [Conditional("UNITY_EDITOR")]
//     public static void DrawRay(Vector3 start, Vector3 dir, Color color) =>
//         UnityEngine.Debug.DrawRay(start, dir, color);

//     [Conditional("UNITY_EDITOR")]
//     public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration) =>
//         UnityEngine.Debug.DrawRay(start, dir, color, duration);

//     [Conditional("UNITY_EDITOR")]
//     public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest) =>
//         UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
// }
