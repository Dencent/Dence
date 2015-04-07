using LeagueSharp;
using LeagueSharp.Common;
using LeeSin.Model;
using LeeSin.Util;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeSin.Controller
{
    internal class FlashHandler : Utilitario
    {
        private const int IgniteRange = 600;
        private SpellSlot _flashSlot;
        private Menu _menu;

        public override void Load(Menu config)
        {
            config.AddItem(new MenuItem("useFlash", "Use Flash on Insec").SetValue(true));

            _menu = config;
            _flashSlot = ObjectManager.Player.GetSpellSlot("SummonerFlash");

        }

        public bool hasFlash()
        {
            return _flashSlot.IsReady();
        }

        public void fireFlash(Vector3 pos)
        {
            if (!hasFlash() || _menu.Item("useFlash").GetValue<bool>())
                return;

            ObjectManager.Player.Spellbook.CastSpell(_flashSlot, pos);

        }
    }
}