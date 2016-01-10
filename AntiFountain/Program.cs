using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Drawing;

namespace AntiFountain
{
    internal class Program
    {
        private const float FountainRange = 1420f;
        private static Obj_AI_Turret Fountain;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            try
            {
                foreach (
                    var fountain in
                        ObjectManager.Get<Obj_AI_Turret>()
                            .Where(fountain => fountain.Team != ObjectManager.Player.Team &&
                                               fountain.Name.ToLower().Contains("shrine")))
                {
                    Fountain = fountain;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Obj_AI_Hero.OnNewPath += Obj_AI_Hero_OnNewPath;
        }

        private static void Obj_AI_Hero_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.Path.Any())
            {
                var first = args.Path.FirstOrDefault(p => p.Distance(Fountain.Position) < FountainRange);
                if (!first.Equals(default(Vector3)))
                {
                    ObjectManager.Player.IssueOrder(
                        GameObjectOrder.MoveTo, Fountain.Position.Extend(first, FountainRange));
                }
            }
        }
    }
}
