using CardMaker.Common;
using HarmonyLib;

namespace cm_mai_resource_fix
{
    public class MaiResourceManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AssetBundleDB), "getCardAssetFileNameWithDetail")]
        public static void GetCardAssetFileNameWithDetail_Postfix(string __result)
        {
            CmMaiResourceFix.logger.LogInfo(__result);
        }
    }
}