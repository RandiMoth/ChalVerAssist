using UnityEngine;

namespace ChalVerAssist.GUI
{
    class ScugSelectOpener : Menu.PositionedMenuObject
    {
        public Menu.SlugcatSelectMenu scugMenu;
        public ScugSelectOpener(Menu.SlugcatSelectMenu menu, Menu.MenuObject owner, Vector2 pos) : base(menu, owner, pos)
        {
            scugMenu = menu;
        }
    }
}
