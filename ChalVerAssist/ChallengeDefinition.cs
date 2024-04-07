using System.Collections.Generic;
using UnityEngine;

namespace ChalVerAssist
{
    public class ChallengeDefinition : UpdatableAndDeletable
    {
        public bool enabled;
        public ChallengeDifficulty difficulty;
        public ChallengeDefinition(RainWorldGame game)
        {
            //ChalVerAssist.Logger.LogMessage("Challenge created: " + name);
            player = game.Players[0].realizedCreature as Player;
            this.game = game;
            Instances.Add(this);
        }
        public RainWorldGame game;
        public ChallengeID id = ChallengeID.None;
        public string name;
        public string description;
        public Player player;
        public bool active;
        public static List<ChallengeDefinition> Instances = new List<ChallengeDefinition>();
        public virtual void Update()
        {
            if (!active)
                return;
            if (game.Players[0].realizedCreature as Player != player)
                player = game.Players[0].realizedCreature as Player;
        }
        public virtual bool Allowed
        {
            get
            {
                if (IsMSC != ModManager.MSC)
                    return false;
                return true;
            }
        }
        public virtual void Activate()
        {
            //if(Allowed)
            active = true;
        }
        public virtual void Complete()
        {

        }
        public virtual void Fail()
        {

        }
        public override void Destroy()
        {
            //ChalVerAssist.Logger.LogMessage("Challenge destroyed: " + name);
            base.Destroy();
        }
        public List<SlugcatStats.Name> allowedSlugcats = new List<SlugcatStats.Name>();
        public bool IsMSC;

        public class ChallengeDifficulty
        {
            public Color color;
            public string name;
            public string description;
            public ChallengeDifficulty(float difficulty)
            {
                if (difficulty < 3)
                {
                    color = Color.green;
                    name = "Beginner";
                    description = "Introductory challenges to help you get accustomed with the map and movement. These have a rated difficulty of less than 3.";
                    return;
                }
                if (difficulty < 5)
                {
                    color = Color.yellow;
                    name = "Intermediate";
                    description = "More difficult challenges to test your knowledge of the world. These have a rated difficulty between 3 and 5.";
                    return;
                }
                if (difficulty < 7)
                {
                    color = Color.HSVToRGB(1 / 12, 1, 1);
                    name = "Advanced";
                    description = "Challenges that require skill and patience to complete. These have a rated difficulty between 5 and 7.";
                    return;
                }
                color = Color.red;
                name = "Expert";
                description = "Only Slugcats that have endured many cycles and mastered their environment will be able to complete these. These have a rated difficulty above 7.";
            }
        }
    }
}
