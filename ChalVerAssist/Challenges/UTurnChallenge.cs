using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MoreSlugcats;
using RWCustom;

namespace ChalVerAssist.Challenges
{
    public class UTurnChallenge : TimerChallenge
    {
        public UTurnChallenge(RainWorldGame game) : 
        base(
            ChallengeID.UTurn,
            game, 
            8.32f, 
            "U-Turn", 
            "Using extended slide instant jumps, travel from the northeast Outskirts shelter around the pre-defined loop and return to the same shelter within 1 minute.", 
            60, 
            new List<string> { "SU_A37", "SU_A63", "SU_B12", "SU_A17", "SU_A40", "SU_B07", "SU_A02", "SU_A53", "SU_A39", "SU_A37", "SU_S04" },
            0
        )
        {
            Instance = this;
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
    }
}
