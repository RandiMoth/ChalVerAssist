using System.Collections.Generic;
using UnityEngine;

namespace ChalVerAssist.Challenges
{
    public class UTurnChallenge : TimerChallenge
    {
        public UTurnChallenge(RainWorldGame game) : base(game)
        {
            Instance = this;
            name = "U-Turn";
            description = "Using extended slide instant jumps, travel from the northeast Outskirts shelter around the pre-defined loop and return to the same shelter within 1 minute.";
            target = 60000;
            rooms = new List<string> { "SU_A37", "SU_A63", "SU_B12", "SU_A17", "SU_A40", "SU_B07", "SU_A02", "SU_A53", "SU_A39", "SU_A37", "SU_S04" };
            id = ChallengeID.UTurn;
            entranceNode = 0;
            difficulty = new ChallengeDifficulty(8.31f);
            allowedSlugcats = new List<SlugcatStats.Name>() { SlugcatStats.Name.White };
            Activate();
        }
        public static UTurnChallenge Instance;
        public override void Update()
        {
            base.Update();
            if (Input.GetKey(ChalVerOptionInterface.cfgUTurnTimerReset.Value))
            {
                ResetBestTime();
            }
        }
        public override void Destroy()
        {
            Instance = null;
            base.Destroy();
        }
        protected override void ResetBestTime()
        {
            ChallengeSaveData.Instance.UTurnBestTime = -1;
            base.ResetBestTime();
        }
    }
}
