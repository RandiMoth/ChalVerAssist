using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public ChallengeData(string path)
        {
            Dictionary<string, object> data = null;
            if (File.Exists(path))
            {
                //ChalVerAssist.Logger.LogMessage(File.ReadAllText(path));
                data = File.ReadAllText(path).dictionaryFromJson();
            }
            if (data == null)
            {
                return;
            }
            foreach (KeyValuePair<string, object> kvp in data)
            {
                try
                {
                    switch (kvp.Key.ToLower())
                    {
                        // Essential
                        case "key":
                            Key = kvp.Value.ToString(); break;
                        case "difficulty":
                            Difficulty = new ChallengeDifficulty(Convert.ToSingle(kvp.Value)); break;

                        // Allowed 
                        case "msc":
                            IsMSC = Convert.ToBoolean(kvp.Value); break;
                        case "slugcats":
                            try
                            {
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
                            FirstCycle = Convert.ToBoolean(kvp.Value); break;

                        // Availability/Completion
                        case "multiplecompletions":
                            MultipleCompletions = Convert.ToBoolean(kvp.Value); break;
                        case "cycleend":
                            CycleEnd = Convert.ToBoolean(kvp.Value); break;

                        // Room crossings
                        case "rooms":
                            Rooms = ((List<object>)kvp.Value).ConvertAll(x => x.ToString()).ToArray(); break;
                        case "pathtype":
                            try
                            {
                                PathType = new PathTypeID(kvp.Value.ToString());
                            }
                            catch (Exception ex)
                            {
                                ChalVerAssist.Logger.LogError(ex);
                                PathType = PathTypeID.Ordered;
                            }
                            break;
                        case "startnode":
                            StartNode = Convert.ToInt32(kvp.Value); break;
                        case "avoidrooms":
                            AvoidRooms = ((List<object>)kvp.Value).ConvertAll(x => x.ToString()).ToArray(); break;

                        // Room-specific conditions
                        case "meetecho":
                            MeetEcho = Convert.ToBoolean(kvp.Value); break;
                        case "crossgates":
                            CrossGates = Convert.ToBoolean(kvp.Value); break;

                        // Timer
                        case "hastimer":
                            HasTimer = Convert.ToBoolean(kvp.Value); break;
                        case "targettime":
                            TargetTime = Convert.ToDouble(kvp.Value); break;

                        // Default 
                        default:
                            unrecognisedFields.Add(kvp); break;
                    }
                }
                catch (Exception ex)
                {
                    ChalVerAssist.Logger.LogError($"Error when reading {kvp.Key}:");
                    ChalVerAssist.Logger.LogError(ex);
                }
            }
            if (Rooms?.Length == 0)
                Rooms = null;
            if (Instances.Any(x => x.Key == Key))
            {
                ChalVerAssist.Logger.LogError($"Several challenges have the same key: {Key}. One is removed from the list.");
                return;
            }
            Instances.Add(this);
        }

        public static List<ChallengeData> Instances = new List<ChallengeData>();
        public static void ReadChallenges()
        {
            string prevChalKey = Challenge.SelectedChallenge?.Key;
            ClearChallenges();
            string[] files;
            try
            {
                files = AssetManager.ListDirectory("serverchallenges", moddedOnly: true);
            }
            catch (Exception ex)
            {
                ChalVerAssist.Logger.LogMessage(RWCustom.Custom.RootFolderDirectory() ?? "no root folder is set??? wtf");
                ChalVerAssist.Logger.LogError(ex);
                return;
            }
            if (files == null || files.Length == 0)
                return;
            //ChalVerAssist.Logger.LogMessage("Starting reading challenges!");
            foreach (string file in files)
            {
                if (Path.GetExtension(file) != ".json")
                    continue;
                _ = new ChallengeData(file);
            }
            if (prevChalKey != null && Instances.Any(x => x.Key == prevChalKey))
                Challenge.SelectedChallenge = Instances.First(x => x.Key == prevChalKey);
        }
        public static void ClearChallenges()
        {
            Instances = new List<ChallengeData>();
            Challenge.SelectedChallenge = null;
            Challenge.ActiveChallenge = null;
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
