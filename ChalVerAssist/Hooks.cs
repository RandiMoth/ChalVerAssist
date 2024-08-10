﻿using ChalVerAssist.GUI;
using System;
using System.Linq;
using UnityEngine;

namespace ChalVerAssist
{
    public static class Hooks
    {
        static public void Initialise()
        {
            On.ExtEnumInitializer.InitTypes += InitTypesHook;
            On.RainWorldGame.Update += RainWorldGameUpdateHook;
            On.RainWorldGame.ctor += RainWorldGame_ctor;
            On.HUD.HUD.InitSinglePlayerHud += HUDInitHook;
            On.PlayerProgression.MiscProgressionData.ToString += SaveStringHook;
            On.PlayerProgression.MiscProgressionData.FromString += FromStringHook;
            On.RainWorld.OnModsInit += ModsInitHook;
            On.Menu.SlugcatSelectMenu.ctor += SlugcatSelectMenuConstructorHook;
            On.Ghost.StartConversation += Ghost_StartConversation;
            On.ModManager.RefreshModsLists += ModManager_RefreshModsLists;
        }

        private static void RainWorldGame_ctor(On.RainWorldGame.orig_ctor orig, RainWorldGame self, ProcessManager manager)
        {
            ChallengeData.ReadChallenges();
            Challenge.SelectedChallenge = ChallengeData.Instances.First(x => x.Key == "uturn");
            orig(self, manager);
        }

        private static void ModManager_RefreshModsLists(On.ModManager.orig_RefreshModsLists orig, RainWorld rainWorld)
        {
            bool flag = ModManager.MSC;
            orig(rainWorld);
            if (ModManager.MSC != flag)
                ChallengeData.ReadChallenges();
        }

        private static void Ghost_StartConversation(On.Ghost.orig_StartConversation orig, Ghost self)
        {
            orig(self);
            if (Challenge.ActiveChallenge?.data.MeetEcho ?? false && self.worldGhost.ghostID == GhostWorldPresence.GetGhostID(Challenge.ActiveChallenge?.data.Rooms?.Last().Split('_')[0] ?? "NoGhost"))
                Challenge.ActiveChallenge.MetEcho = true;
        }

        private static void SlugcatSelectMenuConstructorHook(On.Menu.SlugcatSelectMenu.orig_ctor orig, Menu.SlugcatSelectMenu self, ProcessManager manager)
        {
            orig(self, manager);
            self.pages[0].subObjects.Add(new ScugSelectOpener(self, self.pages[0], new Vector2(manager.rainWorld.screenSize.x / 2 - 100f, manager.rainWorld.screenSize.y - 100f)));
        }

        private static void FromStringHook(On.PlayerProgression.MiscProgressionData.orig_FromString orig, PlayerProgression.MiscProgressionData self, string s)
        {
            orig(self, s);
            if (ChallengeMSD.Instance != null)
                return;
            ChallengeMSD save = new ChallengeMSD();
            save.FromString(ref self.unrecognizedSaveStrings);
        }

        private static string SaveStringHook(On.PlayerProgression.MiscProgressionData.orig_ToString orig, PlayerProgression.MiscProgressionData self)
        {
            string text = orig(self);
            if (ChallengeMSD.Instance != null)
                text += ChallengeMSD.Instance.SaveToString();
            //ChalVerAssist.Logger.LogMessage("Total save!" + text);
            return text;
        }

        private static void ModsInitHook(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                MachineConnector.SetRegisteredOI(ChalVerAssist.PLUGIN_GUID, ChalVerAssist.Instance.options);
            }
            catch (Exception ex)
            {
                /* make sure to error-proof your hook, 
                otherwise the game may break 
                in a hard-to-track way
                and other mods may stop working */
            }
        }

        private static void HUDInitHook(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
        {
            orig(self, cam);
            if (Challenge.ActiveChallenge?.data.HasTimer ?? false)
                Challenge.ActiveChallenge.timer.InitTimer(self);
        }

        private static void RainWorldGameUpdateHook(On.RainWorldGame.orig_Update orig, RainWorldGame self)
        {
            orig(self);
            if (Challenge.ActiveChallenge != null && self.IsStorySession && self.Players.Count > 0 && self.Players[0].realizedCreature != null)
            {
                Challenge.ActiveChallenge.Update();
            }
            else if (Challenge.SelectedChallenge != null && self.IsStorySession && self.Players.Count > 0 && self.Players[0].realizedCreature != null)
                _ = new Challenge(Challenge.SelectedChallenge, self);
        }

        private static void InitTypesHook(On.ExtEnumInitializer.orig_InitTypes orig)
        {
            orig();
            _ = ChallengeData.PathTypeID.Ordered;
        }
    }
}
