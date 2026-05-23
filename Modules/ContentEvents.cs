using System;
using HarmonyLib;

namespace DbsContentApi;

/// <summary>
/// API for registering custom <see cref="ContentEvent"/> types that can be referenced by content triggers and filming systems.
/// Custom events are assigned IDs starting at 2000 in registration order.
/// </summary>
public class ContentEvents
{
    /// <summary>
    /// Registers a custom content event type with the API.
    /// The event is instantiated on demand when the game requests it by ID.
    /// </summary>
    /// <param name="contentEvent">A template instance of the content event to register.</param>
    public static void RegisterEvent(ContentEvent contentEvent)
    {
        ApiLog.Log($"Registering content event for {contentEvent.GetName()}");
        DbsContentApiPlugin.customContentEvents.Add(contentEvent);
    }

    /// <summary>
    /// Resolves the runtime event ID for a registered content event by its type name.
    /// IDs are assigned as <c>2000 + registrationIndex</c>.
    /// </summary>
    /// <param name="contentEventName">The class name of the registered content event type.</param>
    /// <returns>The assigned event ID.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no registered event matches <paramref name="contentEventName"/>.</exception>
    public static ushort GetEventID(string contentEventName)
    {
        var eventList = DbsContentApiPlugin.customContentEvents;

        int foundIndex = eventList.FindIndex(match => match.GetType().Name == contentEventName);
        if (foundIndex == -1)
        {
            throw new InvalidOperationException(
                $"Content event '{contentEventName}' is not registered. " +
                "Call ContentEvents.RegisterEvent before resolving its ID.");
        }

        return (ushort)(2000 + foundIndex);
    }
}


[HarmonyPatch(typeof(ContentEventIDMapper))]
internal class ContentEventIDMapperPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ContentEventIDMapper.GetContentEvent))]
    public static bool GetContentEventPrefix(ref ushort id, ref ContentEvent __result)
    {
        var eventList = DbsContentApiPlugin.customContentEvents;

        ApiLog.Log($"GetContentEvent was called: {id} Normalized: {id - 2000} EventList count: {eventList.Count}");
        if (id - 2000 < 0) return true;
        ContentEvent? contentEvent = eventList[id - 2000];
        if (contentEvent == null) return true;
        __result = (ContentEvent)Activator.CreateInstance(contentEvent.GetType());
        return false;
    }
}
