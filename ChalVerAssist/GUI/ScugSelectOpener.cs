using IL.Menu;
using Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChalVerAssist.GUI
{
    class ScugSelectOpener : Menu.PositionedMenuObject
    {
        public Menu.SlugcatSelectMenu scugMenu;
        public ScugSelectOpener(Menu.SlugcatSelectMenu menu, Menu.MenuObject owner, Vector2 pos) : base(menu, owner, pos) {
            scugMenu = menu;
        }
    }
}
