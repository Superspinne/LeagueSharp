using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace Twitch
{
    internal class Program
    {
        private static Orbwalking.Orbwalker Orbwalker;
        private static Menu Menu;
        private static Spell W, E;

        private static Items.Item Botrk => ItemData.Blade_of_the_Ruined_King.GetItem();
        private static Items.Item Cutlass => ItemData.Bilgewater_Cutlass.GetItem();
        private static Items.Item Youmuus => ItemData.Youmuus_Ghostblade.GetItem();
        private static Items.Item ScryingOrb => ItemData.Farsight_Alteration.GetItem();

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            W = new Spell(SpellSlot.W, 950);
            W.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotCircle);
            E = new Spell(SpellSlot.E, 1200);

            Menu = new Menu("Twitch", "Twitch", true);

            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalking"));

            Menu.SubMenu("Combo").AddItem(new MenuItem("UseW", "Use W").SetValue(new bool()));

            Menu.SubMenu("Misc").AddItem(new MenuItem("blueTrinket", "Buy Blue Trinket on Level 9").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("EKillsteal", "Killsteal with E").SetValue(true));
            Menu.SubMenu("Misc")
                .AddItem(
                    new MenuItem("Emobs", "Kill mobs with E").SetValue(
                        new StringList(new[] {"Baron + Dragon + Siege Minion", "Baron + Dragon", "None"})));

            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Menu.Item("EKillsteal").GetValue<bool>() && E.IsReady())
            {
                foreach (
                    var unit in HeroManager.Enemies.Where(unit => unit.IsValidTarget(E.Range) && E.IsKillable(unit)))
                {
                    E.Cast();
                }
            }

            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                var possibleMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);

                foreach (var minion in possibleMinions)
                {
                    switch (Menu.Item("Emobs").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            if (new[] {"MinionSiege", "Dragon", "Baron"}.Any(c => minion.BaseSkinName.Contains(c)))
                            {
                                E.Cast();
                            }
                            break;

                        case 1:
                            if (new[] {"Dragon", "Baron"}.Any(c => minion.BaseSkinName.Contains(c)))
                            {
                                E.Cast();
                            }
                            break;

                        case 2:
                            return;
                    }
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

                if (Menu.Item("UseW").GetValue<bool>())
                {
                    if (target.IsValidTarget(W.Range) && W.CanCast(target))
                    {
                        W.Cast(target);
                    }
                }

                if ((Botrk.IsReady() || Cutlass.IsReady() || Youmuus.IsReady()) && target.IsValidTarget(Botrk.Range))
                {
                    var desiredItem = Cutlass.IsOwned() ? Cutlass : Botrk;

                    desiredItem.Cast(target);

                    if (Orbwalker.InAutoAttackRange(target))
                    {
                        Youmuus.Cast();
                    }
                }
            }

            if (Menu.Item("blueTrinket").GetValue<bool>() && ObjectManager.Player.Level >= 9 &&
                ObjectManager.Player.InShop() && !ScryingOrb.IsOwned())
            {
                ScryingOrb.Buy();
            }
        }
    }
}
