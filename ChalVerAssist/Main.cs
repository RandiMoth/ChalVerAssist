using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RWCustom;
using BepInEx.Logging;

namespace ChalVerAssist
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class ChalVerAssist : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "randi_moth.chalassist"; // This should be the same as the id in modinfo.json!
        public const string PLUGIN_NAME = "Server Challenge Tracker"; // This should be a human-readable version of your mod's name. This is used for log files and also displaying which mods get loaded. In general, it's a good idea to match this with your modinfo.json as well.
        public const string PLUGIN_VERSION = "1.1.1"; // This follows semantic versioning. For more information, see https://semver.org/ - again, match what you have in modinfo.json
                                                      // It should go without saying, but for this to benefit other modders, the class *and* these const strings must be public.


        public static ChalVerAssist Instance { get; private set; }
        public ChalVerOptionInterface options;

        public ChalVerAssist()
        {
            options = new ChalVerOptionInterface(this);
            Instance = this;
        }

        private void Awake()
        {
            Hooks.Initialise();
        }
        public static new ManualLogSource Logger { get; private set; }

        public void OnEnable()
        {
            Logger = base.Logger;
        }
    }
}
