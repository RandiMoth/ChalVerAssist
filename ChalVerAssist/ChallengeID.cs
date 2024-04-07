using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChalVerAssist
{
    public class ChallengeID : ExtEnum<ChallengeID>
    {
        public static readonly ChallengeID None = new ChallengeID("None", true);
        public static readonly ChallengeID UTurn = new ChallengeID("U-Turn", true);
        public static readonly ChallengeID Circuit = new ChallengeID("Circuit", true);
        public static readonly ChallengeID Acrobat = new ChallengeID("Acrobat", true);

        public ChallengeID(string value, bool register = false)
            : base(value, register)
        {
        }
    }
}
