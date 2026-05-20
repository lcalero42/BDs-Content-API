using UnityEngine;

namespace DbsContentApi;

internal static class ApiLog
{
    internal static void Log(string message) => Debug.Log($"[DbsContentApi] {message}");
    internal static void LogWarning(string message) => Debug.LogWarning($"[DbsContentApi] {message}");
    internal static void LogError(string message) => Debug.LogError($"[DbsContentApi] {message}");
}
