using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ChalVerAssist
{
    public class ChallengeSaveData
    {
        public ChallengeSaveData()
        {
            Instance = this;
        }

        public static ChallengeSaveData Instance;

        public double UTurnBestTime = -1;

        public string SaveToString()
        {
            string text = "";
            text += string.Format(CultureInfo.InvariantCulture, "UTURNTIME<dpB>{0}<dpA>", UTurnBestTime);
            ChalVerAssist.Logger.LogMessage("Saved!" + text);
            return text;
        }
        public void FromString(List<string> saveStrings)
        {
            if (saveStrings.Count == 0)
                return;
            string[] array;
            for (int i = 0; i < saveStrings.Count; i++)
            {
                ChalVerAssist.Logger.LogMessage("Loading!" + saveStrings[i]);
                array = Regex.Split(saveStrings[i], "<dpB>");
                try
                {
                    switch (array[0])
                    {
                        case "UTURNTIME":
                            UTurnBestTime = Convert.ToDouble(array[1]);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ChalVerAssist.Logger.LogError(ex);
                }
            }
            //ChalVerAssist.Logger.LogMessage("Loading finished! U-Turn time: " + UTurnBestTime.ToString());
        }
    }
}
