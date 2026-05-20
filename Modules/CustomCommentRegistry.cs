using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace DbsContentApi;

/// <summary>
/// Registry for custom localized comment strings that are injected into the game's comment UI at runtime.
/// </summary>
public static class CustomCommentRegistry
{
    private static Dictionary<string, List<CustomComment>> _registeredComments = new();

    /// <summary>
    /// Registers one or more localized translations for a comment key.
    /// If a translation for the same language already exists, its text is updated.
    /// </summary>
    /// <param name="key">The localization key used to look up the comment in string tables.</param>
    /// <param name="translations">One or more <see cref="CustomComment"/> entries for different languages.</param>
    public static void Register(string key, params CustomComment[] translations)
    {
        if (!_registeredComments.TryGetValue(key, out var list))
        {
            list = new List<CustomComment>();
            _registeredComments[key] = list;
        }

        foreach (var translation in translations)
        {
            var existing = list.FirstOrDefault(c => c.Language == translation.Language);
            if (existing != null)
            {
                existing.Comment = translation.Comment;
                continue;
            }

            list.Add(translation);
        }
    }

    internal static void ApplyToStringTables(Dictionary<LocaleIdentifier, StringTable> tables)
    {
        if (tables == null)
        {
            ApiLog.LogError("[CustomCommentRegistry] tables dictionary is null.");
            return;
        }

        foreach (var kvp in _registeredComments)
        {
            string key = kvp.Key;
            var translations = kvp.Value;

            var englishFallback = translations.FirstOrDefault(t => t.Language == "en");

            foreach (var tableKvp in tables)
            {
                var localeId = tableKvp.Key;
                var stringTable = tableKvp.Value;

                if (stringTable.GetEntry(key) != null)
                {
                    ApiLog.LogError($"[CustomCommentRegistry] Key collision for '{key}' in table '{localeId.Code}'. Skipping.");
                    continue;
                }

                var translation = translations.FirstOrDefault(t => t.Language == localeId.Code);
                if (translation != null)
                {
                    stringTable.AddEntry(key, translation.Comment);
                    continue;
                }

                if (englishFallback != null)
                {
                    stringTable.AddEntry(key, englishFallback.Comment);
                    continue;
                }

                ApiLog.LogError($"[CustomCommentRegistry] No translation or English fallback for key '{key}' in locale '{localeId.Code}'.");
            }
        }

        ApiLog.Log($"[CustomCommentRegistry] Applied {_registeredComments.Count} custom comment keys to {tables.Count} string tables.");
    }
}
