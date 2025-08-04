using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace cm_mai_resource_fix
{
    [BepInPlugin("CmMaiResourceFix", "CmMaiResourceFix", "1.0.0.0")]
    public class CmMaiResourceFix : BaseUnityPlugin
    {
        public static ManualLogSource logger;
        
        void Awake()
        {
            logger = Logger;
            Logger.LogInfo("Load Mod");
            Patch(typeof(MaiResourcePatch));
        }

        private bool Patch(Type type, bool noLoggerPrint = false)
        {
            try
            {
                Harmony.CreateAndPatchAll(type);
                Logger.LogInfo($"Patch {type.Name}");
                return true;
            } catch (Exception e)
            {
                if (!noLoggerPrint)
                {
                    Logger.LogError($"Failed to patch {type.Name}: {e}");
                }
                return false;
            }
        }
    }
}