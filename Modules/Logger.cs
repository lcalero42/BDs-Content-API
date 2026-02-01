using BepInEx.Logging;

namespace DbsContentApi.Modules;

/// <summary>
/// Static logger wrapper for consistent mod logging.
/// </summary>
public static class Logger
{
    private static ManualLogSource? _source;

    /// <summary>
    /// Initializes the logger with a BepInEx ManualLogSource.
    /// </summary>
    /// <param name="source">The log source to use.</param>
    public static void Init(ManualLogSource source)
    {
        _source = source;
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void Log(string message) => _source?.LogInfo($"[DbsContentApi] {message}");

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void LogError(string message) => _source?.LogError($"[DbsContentApi] {message}");

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void LogWarning(string message) => _source?.LogWarning($"[DbsContentApi] {message}");
}

