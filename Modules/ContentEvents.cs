using System;
using HarmonyLib;

using Logger = DbsContentApi.Modules.Logger;

namespace DbsContentApi.Modules;

public class ContentEvents
{
    public static void RegisterEvent(ContentEvent contentEvent)
    {
        Logger.Log($"Registering content event for {contentEvent.GetName()}");
        DbsContentApi.DbsContentApiPlugin.customContentEvents.Add(contentEvent);
    }

    public static ushort GetEventID(string contentEventName)
    {
        var eventList = DbsContentApiPlugin.customContentEvents;

        Logger.Log(eventList.Count.ToString());

        int foundIndex = eventList.FindIndex(match => match.GetType().Name == contentEventName);
        if (foundIndex == -1)
        {
            for (int index = 0; index < eventList.Count; index++)
            {
                Logger.Log($"{eventList[index].GetType().Name}, {contentEventName}, {eventList[index].GetType().Name == contentEventName}");
            }
            Logger.Log($"GetEventID for {contentEventName} returned -1");
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

        Logger.Log($"GetContentEvent was called: {id} Normalized: {id - 2000} EventList count: {eventList.Count}");
        if (id - 2000 < 0) return true;
        ContentEvent? contentEvent = eventList[id - 2000];
        if (contentEvent == null) return true;
        __result = (ContentEvent)Activator.CreateInstance(contentEvent.GetType());
        return false;
    }
}