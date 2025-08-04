using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using CardMaker.Common;
using CardMaker.MAI;
using HarmonyLib;

namespace cm_mai_resource_fix
{
    public class MaiResourcePatch
    {
        private static CardMaker.MAI.MaiStudio.RomConfig romConfig;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MAIDataManager.Loader), "mount")]
        public static void mount_Postfix(MAIDataManager.Loader __instance, ref bool __result)
        {
            var romConfigField = AccessTools.Field(typeof(MAIDataManager.Loader), "romConfig_");
            romConfig = (CardMaker.MAI.MaiStudio.RomConfig)romConfigField.GetValue(__instance);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MAIContext), "Awake")]
        public static void Awake_Postfix(MAIContext __instance)
        {
            var list = new List<Entry>();
            Find(list, romConfig.optDir, "/MAI");
            for (int index = 0; index < list.Count; ++index)
            { 
                if (list[index].type_ == Entry.Type_OS)
                {
                    AssetBundleDB.getInstance(AssetBundleDB.Title.Maimai).appendAssetBundleSet(list[index].path_);
                }
            }
        }

        private static void Find(List<Entry> entries, string directory, string optionPath)
        {
            int count = entries.Count;
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            DirectoryInfo[] directories = directoryInfo.GetDirectories("A???", SearchOption.TopDirectoryOnly);
            for (int index = 0; index < directories.Length; ++index)
            {
                Entry entry = new Entry();
                if (int.TryParse(directories[index].Name.Substring(1, 3), out entry.version_))
                {
                    entry.type_ = 0;
                    if (Directory.Exists(directories[index].FullName + optionPath))
                    {
                        entry.path_ = directories[index].FullName + optionPath;
                        entries.Add(entry);
                    }
                }
            }
            FileInfo[] files = directoryInfo.GetFiles("A???.pac");
            for (int index = 0; index < files.Length; ++index)
            {
                Entry entry = new Entry();
                if (int.TryParse(files[index].Name.Substring(1, 3), out entry.version_))
                {
                    entry.type_ = 1;
                    entry.path_ = files[index].FullName;
                    entries.Add(entry);
                }
            }
            if (count >= entries.Count)
                return;
            entries.Sort(count, entries.Count - count, new EntryComparer());
        }
        
        private struct Entry
        {
            public const int Type_OS = 0;
            public const int Type_Pack = 1;
            public int version_;
            public int type_;
            public string path_;
        }
        
        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct EntryComparer : IComparer<Entry>
        {
            public int Compare(Entry x0, Entry x1)
            {
                return x0.version_ == x1.version_ ? x0.type_ - x1.type_ : x0.version_ - x1.version_;
            }
        }
    }
}
