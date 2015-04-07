using LeagueSharp;
using LeagueSharp.Common;
using LeeSin.Controller;
using LeeSin.Util;
using LeeSin.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeSin.Model
{
    public abstract class PluginModel
    {
        protected PluginModel()
        {
            CriarMenu();

            Utility.HpBarDamageIndicator.DamageToUnit = DamageToUnit;
            Utility.HpBarDamageIndicator.Enabled = true;

            Orbwalking.AfterAttack += OnAfterAttack;
            Orbwalking.BeforeAttack += OnBeforeAttack;

            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnPossibleToInterrupt;

        }

        public Menu Menu { get; internal set; }
        public Orbwalking.Orbwalker Orbwalker { get; internal set; }

        public Orbwalking.OrbwalkingMode OrbwalkerMode
        {
            get { return Orbwalker.ActiveMode; }
        }

        public bool Packets
        {
            get { return false; }
        }

        public static Language Language { get; set; }

        public Dictionary<string,string> LanguageDic { get { return Language.LanguageDic; } }

        public static Obj_AI_Hero Player
        {
            get { return GameControl.MyHero; }
        }

        private float DamageToUnit(Obj_AI_Hero hero)
        {
            return GetComboDamage(hero);
        }

        private void CriarMenu()
        {
            Menu = new Menu("Lee Sin", "LeeSin", true);
            
            var i8nMenu = new Menu("Language (need reload)", "LeeSinLanguage");
            i8nMenu.AddItem(new MenuItem("LeeSinLanguageSel", "Language Chosen").SetValue(new StringList(new[] { "English", "Portugues"}, 1)));
            Menu.AddSubMenu(i8nMenu);

            var languageIndex = Menu.Item("LeeSinLanguageSel").GetValue<StringList>().SelectedIndex;

            
            switch (languageIndex)
            {
                case 0:
                    Language = new English();
                    break;
                case 1:
                    Language = new Portugues();
                    break;
            }


            var tsMenu = new Menu(Language.LanguageDic["select"], "LeeSinTS");
            TargetSelector.AddToMenu(tsMenu);
            Menu.AddSubMenu(tsMenu);

            var orbwalkMenu = new Menu("Orbwalker", "LeeSinOrbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
            Menu.AddSubMenu(orbwalkMenu);

            var comboMenu = new Menu("Combo", "LeeSinCombo");
            Combo(comboMenu);
            Menu.AddSubMenu(comboMenu);

            var harassMenu = new Menu("Harass", "LeeSinHarass");
            Harass(harassMenu);
            Menu.AddSubMenu(harassMenu);

            var laneMenu = new Menu("LaneClear", "LeeSinLane");
            Laneclear(laneMenu);
            Menu.AddSubMenu(laneMenu);

            var miscMenu = new Menu("Misc", "LeeSinMisc");
            miscMenu.AddItem(new MenuItem("packets", "Use packets").SetValue(true));
            Misc(miscMenu);
            Menu.AddSubMenu(miscMenu);

            var extraMenu = new Menu("Extra", "LeeSinExtra");
            Extra(extraMenu);
            Menu.AddSubMenu(extraMenu);


            if (Player.GetSpellSlot("SummonerDot") != SpellSlot.Unknown)
            {
                var igniteMenu = new Menu(Language.LanguageDic["ignite"], "LeeSinIgnite");
                new ItemHandler().Load(igniteMenu);
                Menu.AddSubMenu(igniteMenu);
            }

            var manaManager = new Menu(Language.LanguageDic["mana"], "LeeSinMana");
            new ManaHandler().Load(manaManager);
            Menu.AddSubMenu(manaManager);

            var pmUtilitario = new Menu(Language.LanguageDic["health"], "LeeSinPM");
            new PotionHandler().Load(pmUtilitario);
            Menu.AddSubMenu(pmUtilitario);

            var drawingMenu = new Menu(Language.LanguageDic["draw"], "LeeSinDrawing");
            Drawings(drawingMenu);
            Menu.AddSubMenu(drawingMenu);


            Menu.AddToMainMenu();
        }

        public T GetValue<T>(string name)
        {
            return Menu.Item(name).GetValue<T>();
        }

        public bool GetBool(string name)
        {
            return GetValue<bool>(name);
        }

        public virtual float GetComboDamage(Obj_AI_Hero target)
        {
            return 0;
        }

        public Spell GetSpell(List<Spell> spellList, SpellSlot slot)
        {
            return spellList.First(x => x.Slot == slot);
        }

        #region Virtuals

        public virtual void Combo(Menu config)
        {
        }

        public virtual void Harass(Menu config)
        {
        }

        public virtual void Laneclear(Menu config)
        {
        }

        public virtual void ItemMenu(Menu config)
        {
        }

        public virtual void Misc(Menu config)
        {
        }

        public virtual void Extra(Menu config)
        {
        }

        public virtual void Drawings(Menu config)
        {
        }

        public virtual void UseItem(int id, Obj_AI_Hero target = null)
        {
            if (Items.HasItem(id) && Items.CanUseItem(id))
            {
                Items.UseItem(id, target);
            }
        }

        public virtual bool CanUseItem(int id)
        {
            return Items.HasItem(id) && Items.CanUseItem(id);
        }

        public virtual void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
        }

        public virtual void OnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
        }

        public virtual void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
        }

        public virtual void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
        }

        public virtual void OnPossibleToInterrupt(Obj_AI_Hero target, Interrupter2.InterruptableTargetEventArgs args)
        {
        }

        #endregion Virtuals
    }
}