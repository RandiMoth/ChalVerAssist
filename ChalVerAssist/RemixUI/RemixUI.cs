using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace ChalVerAssist
{
    public class ChalVerOptionInterface : OptionInterface
    {
        public static Configurable<KeyCode> cfgUTurnTimerReset;
        public override void Initialize()
        {
            base.Initialize();
            Tabs = new OpTab[1]
            {
            new OpTab(this, OptionInterface.Translate("Settings"))
            };
            var opLabel = new OpLabel(new Vector2(100f, 560f), new Vector2(400f, 30f), "Settings", FLabelAlignment.Center, bigText: true);
            Tabs[0].AddItems(opLabel);
            AddKeyBindOption(cfgUTurnTimerReset, Tabs[0], 40);
        }
        private void AddFloatOption(Configurable<float> configurable, OpTab opTab, int num)
        {
            float num2 = 20f;
            float num3 = 550f;
            OpUpdown opUpdown = new OpUpdown(configurable, new Vector2(num2, num3 - num), 550f, 1)
            {
                description = configurable.info.description
            };
            UIfocusable.MutualVerticalFocusableBind(opUpdown, opUpdown);
            OpLabel opLabel2 = new OpLabel(new Vector2(10 + num2, num3 - num), new Vector2(170f, 36f), Custom.ReplaceLineDelimeters(configurable.info.Tags[0] as string), FLabelAlignment.Left, bigText: false)
            {
                bumpBehav = opUpdown.bumpBehav,
                description = opUpdown.description
            };
            opTab.AddItems(opUpdown, opLabel2);
        }
        private void AddKeyBindOption(Configurable<KeyCode> configurable, OpTab opTab, int num)
        {
            float num2 = 20f;
            float num3 = 550f;
            OpKeyBinder opKeyBinder = new OpKeyBinder(configurable, new Vector2(num2, num3 - num), new Vector2(20, 20))
            {
                description = configurable.info.description
            };
            UIfocusable.MutualVerticalFocusableBind(opKeyBinder, opKeyBinder);
            OpLabel opLabel2 = new OpLabel(new Vector2(40 + num2, num3 - num), new Vector2(170f, 36f), Custom.ReplaceLineDelimeters(configurable.info.autoTab), FLabelAlignment.Left, bigText: false)
            {
                bumpBehav = opKeyBinder.bumpBehav,
                description = opKeyBinder.description
            };
            opTab.AddItems(opKeyBinder, opLabel2);
        }

        private ChalVerAssist plugin;

        public ChalVerOptionInterface(ChalVerAssist plugin)
        {
            this.plugin = plugin;
            config.configurables.Clear();
            cfgUTurnTimerReset = config.Bind("cfgUTurnTimerReset", KeyCode.U, new ConfigurableInfo("The button used to reset the best time on U-Turn to nothing.", null, "U-Turn timer reset"));
        }
    }
}
