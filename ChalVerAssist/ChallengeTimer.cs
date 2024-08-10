using ChalVerAssist.GUI;
using HUD;
using JetBrains.Annotations;
using MoreSlugcats;
using RWCustom;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChalVerAssist
{
    public class ChallengeTimer
    {
        public ChallengeTimer(RainWorldGame game, Challenge challenge)
        {
            this.challenge = challenge;
        }
        public void InitTimer(HUD.HUD hud)
        {
            if (challenge.allowed)
            {
                display = new ChallengeTimerDisplay(hud, null, hud.fContainers[1], this);
                hud.AddPart(display);
            }
            bestTimeTracker = new BestTimeTracker(hud, null, hud.fContainers[1], this);
            hud.AddPart(bestTimeTracker);
            bestTimeTracker.BestTime = BestTime;
            if (challenge.allowed)
            {
                prevTimeTracker = new BestTimeTracker(hud, null, hud.fContainers[1], this, "Previous time: ");
                hud.AddPart(prevTimeTracker);
            }
        }
        public Challenge challenge;
        public int entranceNode;
        protected double target = -1;
        protected List<string> rooms = new List<string>();
        private int cooldown = 0;
        public ChallengeTimerDisplay display;
        public BestTimeTracker bestTimeTracker;
        public BestTimeTracker prevTimeTracker;
        public double BestTime = -1;
        public bool ActiveTimer
        {
            get
            {
                return activeTimer;
            }
            set
            {
                activeTimer = value;
            }
        }
        private bool activeTimer;
        public void Update()
        {
            if (display == null)
            {
                return;
            }
            if (cooldown > 0)
            {
                cooldown--;
                return;
            }
            if (!activeTimer && challenge.Available)
                EnableTimer();
        }
        public void Finish()
        {
            ResetTimer();
            double passedTime = display.PassedTime;
            prevTimeTracker.BestTime = passedTime;
            if (BestTime == -1 || passedTime < BestTime)
                BestTime = passedTime;
            bestTimeTracker.BestTime = BestTime;
            if (ChallengeMSD.Instance.ChallengeTimes.ContainsKey(challenge.data.Key))
                ChallengeMSD.Instance.ChallengeTimes[challenge.data.Key] = BestTime;
            else
                ChallengeMSD.Instance.ChallengeTimes.Add(challenge.data.Key, BestTime);
            display.paused = true;
            display.remainVisibleCounter = 120;
        }
        public void ResetTimer()
        {
            activeTimer = false;
            cooldown = 120;
        }
        public void EnableTimer()
        {
            display.CalcOffset();
            activeTimer = true;
            display.paused = false;
        }
        public void ResetBestTime()
        {
            BestTime = -1;
            if (bestTimeTracker == null)
                return;
            bestTimeTracker.BestTime = -1;
            prevTimeTracker.BestTime = -1;
        }
    }
}
