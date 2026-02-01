using System;
using HarmonyLib;

using Logger = DbsContentApi.Modules.Logger;

namespace DbsContentApi.Modules;

public class ContentEvents {
    /// <summary>
    /// Registers the given content event
    /// </summary>
    /// <param name="contentEvent"></param>
    public static void RegisterEvent(ContentEvent contentEvent) {
        // Add the passed contentEvent to the list of events
        Logger.Log($"[ContentLib] Registering content event for {contentEvent.GetName()}");
        DbsContentApi.DbsContentApiPlugin.customContentEvents.Add(contentEvent);
    }

    /// <summary>
    /// A custom method for getting the correct content event ID
    /// Referenced from the ContentLibrary mod
    /// </summary>
    /// <param name="contentEventName"></param>
    /// <returns></returns>
    public static ushort GetEventID(string contentEventName) {
        // The base game reserves IDs 1-1999 around, so we start at 2000.

        var eventList = DbsContentApi.DbsContentApiPlugin.customContentEvents;

        Logger.Log(eventList.Count.ToString());

        // Make sure the event has been registered
        int foundIndex = eventList.FindIndex(match => match.GetType().Name == contentEventName);
        if (foundIndex == -1) {
            for (int index = 0; index < eventList.Count; index++) {
                Logger.Log($"[ContentLib_Logger] {eventList[index].GetType().Name}, {contentEventName}, {eventList[index].GetType().Name == contentEventName}");
            }
            Logger.Log($"[ContentLib] GetEventID for {contentEventName} returned -1");
        }

        // Return the sanitized ID
        return (ushort)(2000 + foundIndex);
    }
}


[HarmonyPatch(typeof(ContentEventIDMapper))]
internal class ContentEventIDMapperPatches {
    /// <summary>
    /// Patch for getting the content events to allow custom events to work
    /// Referenced from the ContentLibrary mod
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ContentEventIDMapper.GetContentEvent))]
    public static bool GetContentEventPrefix(ref ushort id, ref ContentEvent __result) {
        var eventList = DbsContentApi.DbsContentApiPlugin.customContentEvents;
        Logger.Log($"[ContentLib_Logger] GetContentEvent was called: {id} Normalized: {id - 2000} EventList count: {eventList.Count}");
        // If the ID is part of the base game, get out and allow the original function to run
        if (id - 2000 < 0) return true;

        // Get the content event from the list
        // If it doesn't exist / hasn't been registered, get out and allow the original function to run
        ContentEvent? contentEvent = eventList[id - 2000];
        if (contentEvent == null) return true;

        // Create a reference instance of the content event type and disallow the original function from running
        __result = (ContentEvent)Activator.CreateInstance(contentEvent.GetType());
        return false;
    }
}