using MoreSlugcats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChalVerAssist
{
    public static class ModTester
    {
        static ModTester()
        {

        }
        private static List<string> allowedMods = new List<string>();
        private static List<string> bannedMods = new List<string>();
        public static void InitModLists()
        {
            allowedMods = new List<string>();
            bannedMods = new List<string>();
            if (File.Exists(AssetManager.ResolveFilePath("allowedmods.txt")))
            {
                string[] array = File.ReadAllLines(AssetManager.ResolveFilePath("allowedmods.txt"));
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].StartsWith("//") || array[i].Length <= 0)
                    {
                        continue;
                    }
                    allowedMods.Add(array[i]);
                }
            }
            if (File.Exists(AssetManager.ResolveFilePath("bannedmods.txt")))
            {
                string[] array = File.ReadAllLines(AssetManager.ResolveFilePath("bannedmods.txt"));
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].StartsWith("//") || array[i].Length <= 0)
                    {
                        continue;
                    }
                    bannedMods.Add(array[i]);
                }
            }
        }
        public static List<string> AllowedMods = new List<string>();
        public static List<string> BannedMods = new List<string>();
        public static List<string> BannedRemixOptions = new List<string>();
        public static List<string> WarnedRemixOptions = new List<string>();
        public static List<string> UnclearMods = new List<string>();
        public static void FetchModLists()
        {
            AllowedMods = new List<string>();
            BannedMods = new List<string>();
            BannedRemixOptions = new List<string>();
            WarnedRemixOptions = new List<string>();
            UnclearMods = new List<string>();
            string text;
            foreach (var x in ModManager.ActiveMods)
            {
                text = x.id;
                if (allowedMods.Contains(text))
                    AllowedMods.Add(x.name);
                else if (bannedMods.Contains(text))
                    BannedMods.Add(x.name);
                else if (text != ChalVerAssist.PLUGIN_GUID)
                    UnclearMods.Add(x.name);
            }
            if (!ModManager.MMF)
                return;
            foreach (var x in MMF.boolPresets)
            {
                if (!x.config.Value)
                    continue;
                switch(x.config.key)
                {
                    case "cfgKeyItemPassaging":
                    case "cfgMonkBreathTime":
                    case "cfgLargeHologramLight":
                    case "cfgFreeSwimBoosts":
                    case "cfgGlobalMonkGates":
                    case "cfgIncreaseStuns":
                    case "cfgDislodgeSpears":
                    case "cfgAlphaRedLizards":
                        BannedRemixOptions.Add(x.config.info.autoTab);
                        break;
                    case "cfgDisableGateKarma":
                    case "cfgNoRandomCycles":
                        WarnedRemixOptions.Add(x.config.info.autoTab);
                        break;
                    default:
                        break;
                }
            }
            if (MMF.cfgSlowTimeFactor.Value != 1)
                BannedRemixOptions.Add(MMF.cfgSlowTimeFactor.info.autoTab);
            if (MMF.cfgRainTimeMultiplier.Value != 1)
                BannedRemixOptions.Add(MMF.cfgRainTimeMultiplier.info.autoTab);
            if (MMF.cfgHunterCycles.Value != 20)
                WarnedRemixOptions.Add(MMF.cfgHunterCycles.info.autoTab);
            if (MMF.cfgHunterBonusCycles.Value != 5)
                WarnedRemixOptions.Add(MMF.cfgHunterBonusCycles.info.autoTab);
        }
    }
}
