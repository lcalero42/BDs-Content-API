using UnityEngine;

namespace DbsContentApi.Modules;

public static class Logger
{
    public static void Log(string message) => Debug.Log($"[DbsContentApi] {message}");
    public static void LogError(string message) => Debug.LogError($"[DbsContentApi] {message}");
    public static void LogWarning(string message) => Debug.LogWarning($"[DbsContentApi] {message}");
}

