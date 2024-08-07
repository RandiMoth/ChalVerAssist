using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChalVerAssist
{
    public class ChallengeData
    {
        // Mandatory
        public string Key = "NULL";
        public ChallengeDifficulty Difficulty;

        // Allowed
        public bool IsMSC = false;
        public SlugcatStats.Name[] AllowedSlugcats = null;
        public string[] AllowedRegions = null;
        public bool FirstCycle = false;

        // Availability/Completion
        public bool MultipleCompletions = false;
        public bool CycleEnd = false;

        // Room-visiting
        public string[] Rooms = null;
        public PathTypeID PathType = PathTypeID.Ordered;
        public int StartNode = -1;
        public string[] AvoidRooms = null;

        // Room-specific requirements
        public bool MeetEcho = false;
        public bool CrossGates = false;

        // Timer
        public bool HasTimer = false;
        public double TargetTime = -1;

        // Other
        public List<KeyValuePair<string, object>> unrecognisedFields = new List<KeyValuePair<string, object>>();

        public ChallengeData(string path) {
            Dictionary<string, object> data = null;
            if (File.Exists(path))
            {
                data = File.ReadAllText(path).dictionaryFromJson();
            }
            if (data == null)
            {
                return;
            }
            foreach (KeyValuePair<string, object> kvp in data)
            {
                switch (kvp.Key.ToLower())
                {
                    // Essential
                    case "key":
                        Key = kvp.Value.ToString(); break;
                    case "difficulty":
                        Difficulty = new ChallengeDifficulty((float)kvp.Value); break;

                    // Allowed 
                    case "msc":
                        IsMSC = (bool)kvp.Value; break;
                    case "slugcats":
                        try { 
                            AllowedSlugcats = ((List<object>)kvp.Value).ConvertAll(x => new SlugcatStats.Name(x.ToString())).ToArray();
                        }
                        catch (Exception ex)
                        {
                            ChalVerAssist.Logger.LogError(ex);
                            AllowedSlugcats = null; // Assumed to be MSC, shouldn't be displayed.
                        }; 
                        break;
                    case "allowedregions":
                        AllowedRegions = ((List<object>)kvp.Value).ConvertAll((x) => x.ToString()).ToArray(); break;
                    case "firstcycle":
                        FirstCycle = (bool)kvp.Value; break;

                    // Availability/Completion
                    case "multiplecompletions":
                        MultipleCompletions = (bool)kvp.Value; break;
                    case "cycleend":
                        CycleEnd = (bool)kvp.Value; break;

                    // Room crossings
                    case "rooms":
                        Rooms = ((List<object>)kvp.Value).ConvertAll(x => x.ToString()).ToArray(); break;
                    case "pathtype":
                        try {
                            PathType = new PathTypeID(kvp.Value.ToString());
                        }
                        catch (Exception ex)
                        {
                            ChalVerAssist.Logger.LogError(ex);
                            PathType = PathTypeID.Ordered;
                        }
                        break;
                    case "startnode":
                        StartNode = (int)kvp.Value; break;
                    case "avoidrooms":
                        AvoidRooms = ((List<object>)kvp.Value).ConvertAll(x => x.ToString()).ToArray(); break;

                    // Timer
                    case "hastimer":
                        HasTimer = (bool)kvp.Value; break;
                    case "targettime":
                        TargetTime = (double)kvp.Value; break;

                    // Default 
                    default:
                        unrecognisedFields.Add(kvp); break;
                }
            }
            if (Rooms.Length == 0)
                Rooms = null;
        }

        // Assist classes
        public class ChallengeDifficulty
        {
            public Color color;
            public string key;
            public ChallengeDifficulty(float difficulty)
            {
                if (difficulty < 3)
                {
                    color = Color.green;
                    key = "easy";
                    return;
                }
                if (difficulty < 5)
                {
                    color = Color.yellow;
                    key = "normal";
                    return;
                }
                if (difficulty < 7)
                {
                    color = Color.HSVToRGB(1 / 12, 1, 1);
                    key = "hard";
                    return;
                }
                color = Color.red;
                key = "veryhard";
            }
        }

        public class PathTypeID : ExtEnum<PathTypeID>
        {
            public static readonly PathTypeID Ordered = new PathTypeID("Ordered", true);
            public static readonly PathTypeID Unordered = new PathTypeID("Unordered", true);
            public static readonly PathTypeID Strict = new PathTypeID("Strict", true);

            public PathTypeID(string value, bool register = false) : base(value, register) { }
        }
    }
}
