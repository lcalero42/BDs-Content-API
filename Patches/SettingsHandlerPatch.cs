using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Zorro.Settings;

namespace DbsContentApi.Patches
{
    [HarmonyPatch(typeof(SettingsHandler))]
    internal static class SettingsHandlerPatch
    {
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ConstructorTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo operand = AccessTools.Field(typeof(SettingsHandler), nameof(SettingsHandler.settings));
            MethodInfo methodInfo = AccessTools.Method(typeof(SettingsHandlerPatch), nameof(AddCustomBinds), null, null);
            CodeMatcher codeMatcher = new CodeMatcher(instructions, null);
            codeMatcher.MatchForward(false, new CodeMatch[]
            {
                new CodeMatch(new OpCode?(OpCodes.Ldc_I4_S), null, null)
            }).SetOperandAndAdvance(0);
            codeMatcher.MatchForward(false, new CodeMatch[]
            {
                new CodeMatch(new OpCode?(OpCodes.Ldarg_0), null, null),
                new CodeMatch(new OpCode?(OpCodes.Ldfld), operand, null),
                new CodeMatch(new OpCode?(OpCodes.Newobj), null, null),
                new CodeMatch(new OpCode?(OpCodes.Callvirt), null, null)
            }).InsertAndAdvance(new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0, null),
                new CodeInstruction(OpCodes.Ldfld, operand),
                new CodeInstruction(OpCodes.Call, methodInfo)
            });
            return codeMatcher.InstructionEnumeration();
        }
        private static void AddCustomBinds(List<Setting> settings)
        {
            foreach (BaseCWInput input in _inputs)
            {
                settings.Add(input);
            }
        }
    }
}
