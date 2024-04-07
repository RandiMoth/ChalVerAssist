using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ChalVerAssist
{
    public class ChallengeSaveData
    {
        public ChallengeSaveData(SaveState saveState)
        {
            this.saveState = saveState;
            Instance = this;
        }

        public SaveState saveState;

        public static ChallengeSaveData Instance;

        public double UTurnBestTime = -1;

        public string SaveToString()
        {
            string text = "";
            text += string.Format(CultureInfo.InvariantCulture, "UTURNTIME<svB>{0}<svA>", UTurnBestTime);
            //ChalVerAssist.Logger.LogMessage("Saved!" + text);
            return text;
        }
        public void LoadGame(List<string> saveStrings)
        {
            if (saveStrings.Count == 0)
                return;
            string[] array;
            for (int i = 0; i < saveStrings.Count; i++)
            {
                //ChalVerAssist.Logger.LogMessage("Loading!" + saveStrings[i]);
                array = Regex.Split(saveStrings[i], "<svB>");
                switch (array[0])
                {
                    case "UTURNTIME":
                        UTurnBestTime = Convert.ToDouble(array[1]);
                        break;
                }
            }
            //ChalVerAssist.Logger.LogMessage("Loading finished! U-Turn time: " + UTurnBestTime.ToString());
        }
    }
}
