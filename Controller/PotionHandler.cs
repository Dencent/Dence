using LeagueSharp;
using LeagueSharp.Common;
using LeeSin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeSin.Controller
{
    internal class PotionHandler : Utilitario
    {
        private const string HpPotName = "RegenerationPotion";
        private const string MpPotName = "FlaskOfCrystalWater";
        private const int Hpid = 2003;
        private const int Mpid = 2004;
        private static Menu _menu;

        public override void Load(Menu config)
        {
            config.AddItem(new MenuItem("useHP", "Usar pot de vida").SetValue(true));
            config.AddItem(new MenuItem("useHPPercent", "Vida %").SetValue(new Slider(35, 1, 100)));
            config.AddItem(new MenuItem("sseperator", "       "));
            config.AddItem(new MenuItem("useMP", "Usar pot de mana").SetValue(true));
            config.AddItem(new MenuItem("useMPPercent", "Mana %").SetValue(new Slider(35, 1, 100)));

            _menu = config;

            Game.OnUpdate += GameOnOnGameUpdate;
        }

        private static void GameOnOnGameUpdate(EventArgs args)
        {
            var useHp = _menu.Item("useHP").GetValue<bool>();
            var useMp = _menu.Item("useMP").GetValue<bool>();

            if (useHp && ObjectManager.Player.HealthPercentage() <= _menu.Item("useHPPercent").GetValue<Slider>().Value &&
                !HasHealthPotBuff())
            {
                if (Items.CanUseItem(Hpid) && Items.HasItem(Hpid))
                {
                    Items.UseItem(Hpid);
                }
            }

            if (!useMp ||
                !(ObjectManager.Player.ManaPercentage() <= _menu.Item("useMPPercent").GetValue<Slider>().Value) ||
                HasMannaPutBuff()) return;

            if (Items.CanUseItem(Mpid) && Items.HasItem(Mpid))
            {
                Items.UseItem(Mpid);
            }
        }

        private static bool HasHealthPotBuff()
        {
            return ObjectManager.Player.HasBuff(HpPotName, true);
        }

        private static bool HasMannaPutBuff()
        {
            return ObjectManager.Player.HasBuff(MpPotName, true);
        }
    }
}