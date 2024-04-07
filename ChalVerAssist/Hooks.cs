using ChalVerAssist.Challenges;
using HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChalVerAssist
{
    public static class Hooks
    {
        static public void Initialise()
        {

            On.ExtEnumInitializer.InitTypes += InitExtEnumHook;
            On.RainWorldGame.Update += RainWorldGameUpdateHook;
            On.HUD.HUD.InitSinglePlayerHud += HUDInitHook;
            On.Player.Destroy += PlayerDestroyHook;
            On.SaveState.SaveToString += SaveStringHook;
            On.SaveState.LoadGame += LoadGameHook;
            On.RainWorldGame.ctor += GameConstructorHook;
            On.RainWorld.OnModsInit += ModsInitHook;
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

        private static void GameConstructorHook(On.RainWorldGame.orig_ctor orig, RainWorldGame self, ProcessManager manager)
        {
            foreach (var ch in ChallengeDefinition.Instances)
                ch.Destroy();
            orig(self, manager);
        }

        private static void LoadGameHook(On.SaveState.orig_LoadGame orig, SaveState self, string str, RainWorldGame game)
        {
            orig(self, str, game);
            ChallengeSaveData save = new ChallengeSaveData(self);
            save.LoadGame(self.unrecognizedSaveStrings);
        }

        private static string SaveStringHook(On.SaveState.orig_SaveToString orig, SaveState self)
        {
            string text = orig(self);
            text += ChallengeSaveData.Instance.SaveToString();
            //ChalVerAssist.Logger.LogMessage("Total save!" + text);
            return text;
        }

        private static void PlayerDestroyHook(On.Player.orig_Destroy orig, Player self)
        {
            orig(self);
        }

        private static void HUDInitHook(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
        {
            orig(self, cam);
            UTurnChallenge.Instance?.InitTimer(self);
        }

        static private void InitExtEnumHook(On.ExtEnumInitializer.orig_InitTypes orig)
        {
            orig();
            _ = ChallengeID.None;
        }

        private static void RainWorldGameUpdateHook(On.RainWorldGame.orig_Update orig, RainWorldGame self)
        {
            orig(self);
            if (self.IsStorySession && self.Players.Count > 0 && self.Players[0].realizedCreature != null)
            {
                if (UTurnChallenge.Instance == null)
                    new UTurnChallenge(self);
                UTurnChallenge.Instance.Update();
            }
        }
    }
}
