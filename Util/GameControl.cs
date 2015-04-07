using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeSin.Util
{
    class GameControl
    {
        public static Obj_AI_Hero MyHero = ObjectManager.Player;

        public static void LoadPlugin()
        {
            new LeeSin.Plugin.LeeSin();
            Game.PrintChat(MiscControl.stringColor(ObjectManager.Player.ChampionName, MiscControl.TableColor.RoyalBlue) + " loaded, thanks for using LeeSin.");

        }

        private static void CurrentDomainOnUnhandledException(object sender,
            UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Console.WriteLine(((Exception)unhandledExceptionEventArgs.ExceptionObject).Message);
            Game.PrintChat("Fatal Error please report on forum / Erro critico por favor avise no fórum");
        }

        public class EnemyInfo
        {
            public Obj_AI_Hero Player;
            public int LastSeen;
            public int LastPinged;

            public EnemyInfo(Obj_AI_Hero player)
            {
                Player = player;
            }
        }

    }
}
