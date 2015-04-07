using LeagueSharp;
using LeagueSharp.Common;
using LeeSin.Model;
using LeeSin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeSin.Controller
{
    internal class ManaHandler : Utilitario
    {
        private Menu _menu;

        public Orbwalking.Orbwalker Orbwalker { get; internal set; }
        public Orbwalking.OrbwalkingMode OrbwalkerMode
        {
            get { return Orbwalker.ActiveMode; }
        }

        public override void Load(Menu config)
        {
            config.AddItem(new MenuItem("manaSaver", "Controle de mana").SetValue(true));
            config.AddItem(new MenuItem("percentMana", "% de mana para o Combo").SetValue(new Slider(50, 0, 100)));

            _menu = config;
        }

        public bool canUseSpell()
        {
            if (!GetBool("manaSaver")) { return true; }

            if (OrbwalkerMode != Orbwalking.OrbwalkingMode.Combo)
            {
                if (ObjectManager.Player.ManaPercent >= GetValue<Slider>("percentMana").Value || ObjectManager.Player.Mana >= ObjectManager.Player.MaxMana * (GetValue<Slider>("percentMana").Value / 100))
                {
                    return true;
                }
            }

            return false;
        }

        public T GetValue<T>(string name)
        {
            return _menu.Item(name).GetValue<T>();
        }

        public bool GetBool(string name)
        {
            return GetValue<bool>(name);
        }
    }
}
