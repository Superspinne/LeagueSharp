using System;
using LeagueSharp.Common;

namespace Walking
{
    class Program
    {
        private static Orbwalking.Orbwalker Orbwalker;
        private static Menu Menu;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Menu = new Menu("Walker", "Walker", true);
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));

            Menu.AddToMainMenu();
        }
    }
}
