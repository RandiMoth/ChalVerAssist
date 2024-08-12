using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MoreSlugcats.SpeedRunTimer;
using UnityEngine;
using MoreSlugcats;
using HUD;

namespace ChalVerAssist.GUI
{
    public class ChallengeTimerDisplay : SpeedRunTimer
    {
        public ChallengeTimer timer;

        private double offset;
        private string prefix;

        public bool paused;

        public ChallengeTimerDisplay(HUD.HUD hud, PlayerSpecificMultiplayerHud multiHud, FContainer fcontainer, ChallengeTimer timer) : base(hud, multiHud, fcontainer)
        {
            this.timer = timer;
            prefix = hud.rainWorld.inGameTranslator.Translate("rwsc_timer_label").Replace("<NAME>", timer.challenge.Name);
            pos = new Vector2(0.2f + 20, (int)(hud.rainWorld.options.ScreenSize.y - 60f) + 0.2f);
            lastPos = pos;
        }

        public void CalcOffset()
        {
            if (hud.owner is Player player && player.abstractCreature.world != null && player.abstractCreature.world.game != null && player.abstractCreature.world.game.IsStorySession)
            {
                CampaignTimeTracker campaignTimeTracker = GetCampaignTimeTracker(player.abstractCreature.world.game.GetStorySession.saveStateNumber);
                offset = campaignTimeTracker.TotalFreeTime;
            }
        }

        public double PassedTime
        {
            get
            {
                if (hud.owner is Player player && player.abstractCreature.world != null && player.abstractCreature.world.game != null && player.abstractCreature.world.game.IsStorySession)
                {
                    CampaignTimeTracker campaignTimeTracker = GetCampaignTimeTracker(player.abstractCreature.world.game.GetStorySession.saveStateNumber);
                    return campaignTimeTracker.TotalFreeTime - offset;
                }
                return -1;
            }
        }

        public override void Update()
        {
            lastFade = fade;
            if (remainVisibleCounter > 0)
            {
                remainVisibleCounter--;
            }
            if (timer.ActiveTimer || remainVisibleCounter > 0)
            {
                fade = Mathf.Max(Mathf.Min(1f, fade + 0.1f), hud.foodMeter.fade);
            }
            else
            {
                fade = Mathf.Max(0f, fade - 0.1f);
            }
            if (hud.HideGeneralHud)
            {
                fade = 0f;
            }
            if (!paused && hud.owner is Player player && player.abstractCreature.world != null && player.abstractCreature.world.game != null && player.abstractCreature.world.game.IsStorySession)
            {
                CampaignTimeTracker campaignTimeTracker = GetCampaignTimeTracker(player.abstractCreature.world.game.GetStorySession.saveStateNumber);
                if (campaignTimeTracker != null && !RainWorld.lockGameTimer)
                {
                    double mills = campaignTimeTracker.TotalFreeTime - offset;
                    timeLabel.text = prefix + TimeSpan.FromMilliseconds(mills).GetIGTFormat(includeMilliseconds: true);
                }
            }
        }
        public override void Draw(float timeStacker)
        {
            if (!timer.ActiveTimer)
            {
                timeLabel.alpha = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(lastFade, fade, timeStacker)), 1.5f);
            }
            else
            {
                timeLabel.alpha = Mathf.Max(0.2f, Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(lastFade, fade, timeStacker)), 1.5f));
            }
            timeLabel.alignment = FLabelAlignment.Left;
            timeLabel.x = DrawPos(timeStacker).x;
            timeLabel.y = DrawPos(timeStacker).y;
        }
    }
    public class BestTimeTracker : SpeedRunTimer
    {

        public BestTimeTracker(HUD.HUD hud, PlayerSpecificMultiplayerHud multiHud, FContainer fcontainer, ChallengeTimer timer, string label = "rwsc_best_time_prefix") : base(hud, multiHud, fcontainer)
        {
            this.timer = timer;
            this.label = hud.rainWorld.inGameTranslator.Translate(label).Replace("<NAME>", this.timer.challenge.Name);
            timeLabel.text = this.label + hud.rainWorld.inGameTranslator.Translate("none");
            instanceNum = InstanceCount++;
            pos = new Vector2((int)hud.rainWorld.options.ScreenSize.x + 0.2f - 20, (int)(hud.rainWorld.options.ScreenSize.y - 30f - 30f * instanceNum) + 0.2f);
            lastPos = pos;
        }
        public override void Update()
        {
            lastFade = fade;
            if (remainVisibleCounter > 0)
            {
                remainVisibleCounter--;
            }
            if (hadRuns)
            {
                fade = Mathf.Max(Mathf.Min(0.4f, fade + 0.1f), hud.foodMeter.fade, 0.7f);
            }
            else
            {
                fade = Mathf.Max(0f, fade - 0.1f);
            }
            if (hud.HideGeneralHud)
            {
                fade = 0f;
            }
        }
        public override void Draw(float timeStacker)
        {
            timeLabel.alpha = Mathf.Max(0.2f, Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(lastFade, fade, timeStacker)), 1.5f));
            timeLabel.alignment = FLabelAlignment.Right;
            timeLabel.x = DrawPos(timeStacker).x;
            timeLabel.y = DrawPos(timeStacker).y;
        }
        public override void ClearSprites()
        {
            InstanceCount--;
            base.ClearSprites();
        }
        private static int InstanceCount;
        private int instanceNum;
        public ChallengeTimer timer;

        private double bestTime = -1;

        public double BestTime
        {
            get
            {
                return bestTime;
            }
            set
            {
                bestTime = value;
                hadRuns = value != -1;
                if (value == -1)
                    timeLabel.text = label + hud.rainWorld.inGameTranslator.Translate("none");
                else
                    timeLabel.text = label + TimeSpan.FromMilliseconds(value).GetIGTFormat(includeMilliseconds: true);
            }
        }
        private string label;

        private bool hadRuns;
    }
}
