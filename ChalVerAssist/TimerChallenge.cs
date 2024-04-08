using HUD;
using MoreSlugcats;
using RWCustom;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChalVerAssist
{
    public class TimerChallenge : ChallengeDefinition
    {
        public TimerChallenge(RainWorldGame game) : base(game)
        {
            camera = game.cameras[0];
            currentRoom = camera.room.abstractRoom.name;
            InitTimer();
        }
        public void InitTimer()
        {
            if (player.room.game.cameras == null || player.room.game.cameras.Length == 0)
                return;
            HUD.HUD hud = player.room.game.cameras[0].hud;
            if (hud == null)
                return;
            InitTimer(hud);
        }
        public void InitTimer(HUD.HUD hud)
        {
            timer = new ChallengeTimer(hud, null, hud.fContainers[1], this);
            hud.AddPart(timer);
            bestTimeTracker = new BestTimeTracker(hud, null, hud.fContainers[1], this);
            hud.AddPart(bestTimeTracker);
            bestTimeTracker.BestTime = BestTime;
            prevTimeTracker = new BestTimeTracker(hud, null, hud.fContainers[1], this, "Previous time: ");
            hud.AddPart(prevTimeTracker);
        }
        public int entranceNode;
        protected double target = -1;
        protected List<string> rooms = new List<string>();
        private string currentRoom;
        private int roomTracker = -1;
        private int cooldown = 0;
        public ChallengeTimer timer;
        public BestTimeTracker bestTimeTracker;
        public BestTimeTracker prevTimeTracker;
        public double BestTime = -1;
        private RoomCamera camera;
        public bool ActiveTimer
        {
            get
            {
                return active && activeTimer;
            }
            set
            {
                activeTimer = value;
            }
        }
        private bool logFlag;
        private bool activeTimer;
        public override void Update()
        {
            base.Update();
            if (rooms.Count <= 1)
            {
                if (!logFlag)
                {
                    ChalVerAssist.Logger.LogWarning("Challenge with " + rooms.Count + " room(s), should never happen!");
                    logFlag = true;
                }
                return;
            }
            if (!active || timer == null)
            {
                return;
            }
            if (game.cameras[0] != camera)
            {
                camera = game.cameras[0];
            }
            if (cooldown > 0)
            {
                cooldown--;
                return;
            }
            string playerRoom = camera.room.abstractRoom.name;
            if (!activeTimer && playerRoom == rooms[0] && player.abstractCreature.pos.abstractNode == entranceNode)
            {
                EnableTimer();
                currentRoom = playerRoom;
                return;
            }
            else if (!activeTimer)
                return;
            if (playerRoom != currentRoom)
            {
                if (rooms[roomTracker + 1] == playerRoom)
                {
                    currentRoom = playerRoom;
                    roomTracker++;
                    if (roomTracker == rooms.Count - 1)
                    {
                        Complete();
                    }
                }
                else if (roomTracker > 0 && rooms[roomTracker - 1] == playerRoom)
                {
                    currentRoom = playerRoom;
                    roomTracker--;
                }
                else
                    ResetTimer();
            }
        }
        public override void Complete()
        {
            ResetTimer();
            double passedTime = timer.PassedTime;
            prevTimeTracker.BestTime = passedTime;
            if (BestTime == -1 || passedTime < BestTime)
                BestTime = passedTime;
            bestTimeTracker.BestTime = BestTime;
            ChallengeSaveData.Instance.UTurnBestTime = BestTime;
            timer.paused = true;
            timer.remainVisibleCounter = 120;
            if (BestTime < target)
                base.Complete();
        }
        private void ResetTimer()
        {
            activeTimer = false;
            roomTracker = -1;
            cooldown = 120;
        }
        private void EnableTimer()
        {
            activeTimer = true;
            timer.paused = false;
            roomTracker = 0;
            timer.CalcOffset();
        }
        protected virtual void ResetBestTime()
        {
            BestTime = -1;
            if (bestTimeTracker == null)
                return;
            bestTimeTracker.BestTime = -1;
            prevTimeTracker.BestTime = -1;
        }
    }
    public class ChallengeTimer : SpeedRunTimer
    {
        public TimerChallenge challenge;

        private double offset;

        public bool paused;

        public ChallengeTimer(HUD.HUD hud, PlayerSpecificMultiplayerHud multiHud, FContainer fcontainer, TimerChallenge challenge) : base(hud, multiHud, fcontainer)
        {
            this.challenge = challenge;
            pos = new Vector2(0.2f + 20, (int)(hud.rainWorld.options.ScreenSize.y - 30f) + 0.2f);
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
            if (challenge.ActiveTimer || remainVisibleCounter > 0)
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
                    timeLabel.text = "U-Turn time: " + TimeSpan.FromMilliseconds(mills).GetIGTFormat(includeMilliseconds: true);
                }
            }
        }
        public override void Draw(float timeStacker)
        {
            if (!challenge.ActiveTimer)
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

        public BestTimeTracker(HUD.HUD hud, PlayerSpecificMultiplayerHud multiHud, FContainer fcontainer, TimerChallenge challenge, string label = "Best time: ") : base(hud, multiHud, fcontainer)
        {
            this.challenge = challenge;
            this.label = label;
            timeLabel.text = label + "None";
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
        public TimerChallenge challenge;

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
                    timeLabel.text = label + "None";
                else
                    timeLabel.text = label + TimeSpan.FromMilliseconds(value).GetIGTFormat(includeMilliseconds: true);
            }
        }
        private string label;

        private bool hadRuns;
    }
}
