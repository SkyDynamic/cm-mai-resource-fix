using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace cm_mai_resource_fix
{
    [BepInPlugin("CmMaiResourceFix", "CmMaiResourceFix", "1.0.0.0")]
    public class CmMaiResourceFix : BaseUnityPlugin
    {
        public static ManualLogSource logger;
        
        private ConfigEntry<bool> EnableCustomUserInfo { get; set; }
        private ConfigEntry<string> UserName { get; set; }
        private ConfigEntry<int> PlayerRating { get; set; }
        private ConfigEntry<bool> EnableMaiRatingSpacingFix { get; set; }
        private ConfigEntry<bool> EnableIgnoreLoginVersion { get; set; }
        
        void Awake()
        {
            EnableCustomUserInfo = Config.Bind("CustomUserData", "Enable", false, "Enable Custom UserInfo");
            UserName = Config.Bind("CustomUserData", "UserName", "", "Custom UserName");
            PlayerRating = Config.Bind("CustomUserData", "PlayerRating", -1, "Custom PlayerRating, -1 means disable");
            EnableMaiRatingSpacingFix = Config.Bind("MaiRatingSpacingFix", "Enable", false, "Enable Fix MaimaiDX Print Rating Spacing");
            EnableIgnoreLoginVersion = Config.Bind("IgnoreLoginVersion", "Enable", false, "Enable Ignore Login Version");
            
            logger = Logger;
            Logger.LogInfo("Load CmKiraMod Success");
            Patch(typeof(MaiResourcePatch));
            Patch(typeof(MaiRatingSpacingFix));
            Patch(typeof(CustomUserInfo));
        }

        private void Update()
        {
            CustomUserInfo.enable = EnableCustomUserInfo.Value;
            CustomUserInfo.userName = UserName.Value;
            CustomUserInfo.playerRating = PlayerRating.Value;
            
            MaiRatingSpacingFix.Enable = EnableMaiRatingSpacingFix.Value;
            IgnoreLoginVersion.Enable = EnableIgnoreLoginVersion.Value;
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