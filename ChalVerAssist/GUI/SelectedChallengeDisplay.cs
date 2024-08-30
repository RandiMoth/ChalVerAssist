using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChalVerAssist.GUI
{
    public class SelectedChallengeDisplay : HUD.HudPart
    {
        private string text;
        public Vector2 pos;
        public FLabel label;
        private float fade;
        private float lastFade;

        public SelectedChallengeDisplay(HUD.HUD hud, FContainer fContainer, Challenge challenge) : base(hud)
        {
            this.challenge = challenge;
            text = hud.rainWorld.inGameTranslator.Translate("rwsc_challenge_name_prefix").Replace("<NAME>", challenge.Name);
            pos = new Vector2(0.2f + 20, (int)(hud.rainWorld.options.ScreenSize.y - 30f) + 0.2f);
            label = new FLabel(RWCustom.Custom.GetDisplayFont(), text);
            fContainer.AddChild(label);
            label.alignment = FLabelAlignment.Left;
            label.alpha = 1.0f;
            label.x = pos.x;
            label.y = pos.y;
        }
        public override void Update()
        {
            lastFade = fade;
            fade = Mathf.Max(Mathf.Min(0.9f, fade + 0.1f), hud.foodMeter.fade);
            if (hud.HideGeneralHud)
            {
                fade = 0f;
            }
        }
        public override void Draw(float timeStacker)
        {
            label.alpha = Mathf.Max(0.2f, Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(lastFade, fade, timeStacker)), 1.5f));
        }
        public Challenge challenge;
    }
}
