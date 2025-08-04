using System.Linq;
using BepInEx;
using HarmonyLib;

namespace cm_mai_resource_fix
{

    [BepInPlugin("CmMaiResourceFix", "CmMaiResourceFix", "1.0.0.0")]
    public class CmMaiResourceFix : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("CmMaiResourceFix");

        void Awake()
        {
            Logger.LogInfo("Load Mod");
            harmony.PatchAll();

            var patchedMethods = harmony.GetPatchedMethods().ToList();
            Logger.LogInfo($"Number of patched methods: {patchedMethods.Count}");

            foreach (var method in patchedMethods)
            {
                Logger.LogInfo($"Patched method: {method.DeclaringType?.Name}.{method.Name}");
            }
        }

        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}