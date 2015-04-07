using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeSin.Util
{
    class Events
    {
        public class Game
        {
            public delegate void OnGameStartet(EventArgs args);

            static Game()
            {
            }

            public static event OnGameStartet OnGameStart;

            private static void OnGameProcessPacket(GamePacketEventArgs args)
            {
                if (LeagueSharp.Game.Mode != GameMode.Running || OnGameStart == null)
                    return;
                OnGameStart(new EventArgs());
            }
        }
    }
}
