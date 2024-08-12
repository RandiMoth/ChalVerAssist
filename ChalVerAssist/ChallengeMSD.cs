using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChalVerAssist
{
    public class ChallengeMSD
    {
        public ChallengeMSD()
        {
            Instance = this;
        }

        public static ChallengeMSD Instance;

        public Dictionary<string,double> ChallengeTimes = new Dictionary<string, double>();

        public List<string> CompletedChallenges = new List<string>();

        public static void WipeRecord(string key)
        {
            if (Instance?.ChallengeTimes.ContainsKey(key) ?? false)
                Instance.ChallengeTimes.Remove(key);
            if (Instance?.CompletedChallenges.Contains(key) ?? false)
                Instance.CompletedChallenges.Remove(key);
        }

        public string SaveToString()
        {
            string text = "SERVERCHALLENGE<mpdB>";
            text += "CHALTIMES<srchB>";
            foreach (KeyValuePair<string,double> kvp in ChallengeTimes)
            {
                text += kvp.Key;
                text += string.Format("<srchD>{0}", kvp.Value);
                if (kvp.Key != ChallengeTimes.Last().Key)
                    text += "<srchC>";
            }
            text += "<srchA>COMPCHALS<srchB>";
            foreach(var str in CompletedChallenges)
            {
                text += str;
                if (str != CompletedChallenges.Last())
                    text += "<srchC>";
            }
            text += "<mpdA>";
            return text;
        }
        public void FromString(ref List<string> saveStrings)
        {
            if (saveStrings.Count == 0)
                return;
            string[] array, fields, variables, varvalues;
            string chalData = null;
            bool foundData = false;
            for (int i = 0; i < saveStrings.Count; i++)
            {
                //ChalVerAssist.Logger.LogMessage("Loading!" + saveStrings[i]);
                array = Regex.Split(saveStrings[i], "<mpdB>");
                try
                {
                    switch (array[0])
                    {
                        case "SERVERCHALLENGE":
                            chalData = array[1];
                            saveStrings.RemoveAt(i);
                            foundData = true;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ChalVerAssist.Logger.LogError(ex);
                }
                if (foundData)
                    break;
            }
            if (string.IsNullOrEmpty(chalData))
                return;
            //ChalVerAssist.Logger.LogMessage("Found data: " + chalData);
            array = Regex.Split(chalData, "<srchA>");
            for (int i = 0; i < array.Length;i++)
            {
                //ChalVerAssist.Logger.LogMessage("Data field: " + array[i]);
                try
                {
                    fields = Regex.Split(array[i], "<srchB>");
                    switch (fields[0])
                    {
                        case "CHALTIMES":
                            variables = Regex.Split(fields[1], "<srchC>");
                            for (int j = 0; j < variables.Length; j++)
                            {
                                //ChalVerAssist.Logger.LogMessage("Variable: " + variables[j]);
                                varvalues = Regex.Split(variables[j], "<srchD>");
                                if (ChallengeTimes.ContainsKey(varvalues[0]))
                                    ChallengeTimes[varvalues[0]] = float.Parse(varvalues[1]);
                                else
                                    ChallengeTimes.Add(varvalues[0], float.Parse(varvalues[1]));
                            }
                            break;
                        case "COMPCHALS":
                            CompletedChallenges = Regex.Split(fields[1], "<srchC>").ToList();
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
