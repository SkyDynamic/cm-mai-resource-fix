using CardMaker.MAI;
using HarmonyLib;

namespace cm_mai_resource_fix
{
    public class CustomUserInfo
    {
        public static bool enable = false;
        public static string userName = "";
        public static int playerRating = 0;
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MAIUserData), "copyFrom")]
        public static bool MAIUserData_copyFrom_Prefix(ref UserData srcUserData)
        {
            if (enable)
            {
                if (playerRating != -1)
                {
                    srcUserData.playerRating = playerRating;
                }
                
                srcUserData.userName = userName;
            }
            return true;
        }
    }
}