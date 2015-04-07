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

namespace LeeSin.Plugin
{
    internal class LeeSin : PluginModel
    {

        private bool qInstance = true;
        private bool wInstance = true;
        private bool eInstance = true;
        private bool flashed = true;
        private bool insec = false;
        private bool starCombo = true;
        private bool comboStarted = false;
        private int buffCount = 0;

        protected int LastPlaced;
        protected Vector3 LastWardPos;
        protected Vector3 insecPos;

        public Spell Q;
        public Spell W;
        public Spell E;
        public Spell R;
        private const int flashRange = 425;
        private SpellSlot _flashSlot;

        public LeeSin()
        {
            Q = new Spell(SpellSlot.Q, 975);

            Q.SetSkillshot(Q.Instance.SData.SpellCastTime, Q.Instance.SData.LineWidth, Q.Instance.SData.MissileSpeed, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 450);
            R = new Spell(SpellSlot.R, 375);

            Game.OnUpdate += GameOnOnGameUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            Drawing.OnDraw += DrawingOnOnDraw;

            _flashSlot = ObjectManager.Player.GetSpellSlot("SummonerFlash");
            MiscControl.PrintChat(MiscControl.stringColor(LanguageDic["load"], MiscControl.TableColor.Red));
        }

        private void DrawingOnOnDraw(EventArgs args)
        {
            var drawQ = GetBool("drawQ");
            var drawW = GetBool("drawW");
            var drawE = GetBool("drawE");
            var drawR = GetBool("drawR");


            var p = Player.Position;

            if (GetBool("disableAll"))
                return;

            if (drawQ)
                Render.Circle.DrawCircle(p, Q.Range, Q.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);

            if (drawW)
                Render.Circle.DrawCircle(p, W.Range, W.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);

            if (drawE)
                Render.Circle.DrawCircle(p, E.Range, E.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);

            if (drawR)
                Render.Circle.DrawCircle(p, R.Range, R.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);

            if (GetBool("drawInsecPos"))
            {
                Render.Circle.DrawCircle(insecPosition(), 150, System.Drawing.Color.White);
            }

            var wts = Drawing.WorldToScreen(Player.Position);

            Drawing.DrawText(wts[0] - 35, wts[1] + 10, System.Drawing.Color.White, insec ? "Insec Combo" : starCombo ? "Star Combo" : "Normal Combo");
            Drawing.DrawText(wts[0] - 35, wts[1] + 25, System.Drawing.Color.White, "Buff Count: "+ buffCount);

        }

        private void GameOnOnGameUpdate(EventArgs args)
        {

            if (Player.IsDead) return;

            UpdateInstance();
            switch (OrbwalkerMode)
            {
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;

                case Orbwalking.OrbwalkingMode.Combo:
                    Combar();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    break;
            }

        }

        private void UpdateInstance()
        {
            starCombo = GetValue<KeyBind>("starCombo").Active;
            insec = GetValue<KeyBind>("insecCombo").Active;

            if (Player.Spellbook.GetSpell(SpellSlot.Q).Name.ToString().Equals("BlindMonkQOne"))
            {
                qInstance = true;
            }
            else
            {
                qInstance = false;
            }

            if (Player.Spellbook.GetSpell(SpellSlot.W).Name.ToString().Equals("BlindMonkWOne"))
            {
                wInstance = true;
            }
            else
            {
                wInstance = false;
            }

            if (Player.Spellbook.GetSpell(SpellSlot.E).Name.ToString().Equals("BlindMonkEOne"))
            {
                eInstance = true;
            }
            else
            {
                eInstance = false;
            }

            var found = false;
            foreach (var buff in Player.Buffs)
            {
                if (buff.DisplayName.Equals("BlindMonkFlurry"))
                {
                    buffCount = buff.Count;
                    found = true;
                }
            }

            if (!found)
            {
                buffCount = 0;
            }
        }

        private void Combar()
        {
            var target = TargetSelector.GetTarget(1300, TargetSelector.DamageType.Physical);

            var nearAlly =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => !x.IsEnemy)
                        .Where(x => !x.IsDead)
                        .Where(x => !x.IsMe)
                        .Where(x => x.Distance(Player.Position) <= 1200).FirstOrDefault();

            if (!insec)
            {
                if (starCombo)
                {
                    if (!qInstance && Q.IsReady())
                    {
                        if (R.IsReady())
                        {
                            if (R.IsInRange(target))
                            {
                                R.Cast(target); //EEEEK !
                            }
                            else if(W.IsReady())
                            {
                                WardJump();
                            }
                        }
                        else
                        {
                            Utility.DelayAction.Add(200, () => Q.Cast());
                        }
                    }
                    else if (qInstance && Q.IsReady() && Q.IsInRange(target))
                    {
                        Q.CastIfHitchanceEquals(target, HitChance.High);
                    }
                }
                else
                {
                    if (GetBool("comboR") && R.IsReady() && target.Health + 50 <= Player.GetSpellDamage(target, SpellSlot.R))
                    {
                        if (R.IsInRange(target))
                        {
                            R.Cast(target);
                        }
                        else
                        {
                            if (GetBool("comboQ") && Q.IsReady() && Q.IsInRange(target))
                            {
                                Q.CastIfHitchanceEquals(target, HitChance.High);
                            }
                            else if (GetBool("comboW") && W.IsReady() && W.IsInRange(target))
                            {
                                WardJump();
                            }
                        }
                    }
                }
            }
            else
            {
                if (nearAlly == null || !R.IsReady()) return;

                var pos = insecPosition();

                if (Q.IsReady() && !qInstance)
                {
                    Q.Cast();
                }
                else if (Q.IsReady() && qInstance)
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High);
                }
                
                if (!hasFlash()) return;

                if (hasFlash() && Player.Distance(pos) < 200)
                {
                    fireFlash(pos);
                    flashed = true;
                }

                if (flashed && R.IsReady())
                {
                    R.Cast(target);
                }

                
            }
        }

        private void Harass()
        {
            var target = TargetSelector.GetTarget(1300, TargetSelector.DamageType.Physical);

            if (GetBool("harassQ") && Q.IsReady() && Q.IsInRange(target))
            {
                if (qInstance)
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High);
                }
                else
                {
                    Q.Cast();
                }
            }

            if (GetBool("harassW") && W.IsReady())
            {
                if (wInstance)
                {
                    W.Cast(Player);
                }
                else
                {
                    W.Cast();
                }
            }

        }

        private void LaneClear()
        {
            var target = Orbwalker.GetTarget();

            if (GetBool("laneQ") && Q.IsReady() && Q.IsInRange(target))
            {
                if (qInstance)
                {
                    Q.Cast(target.Position);
                }
                else
                {
                    Q.Cast();
                }
            }

            if (GetBool("laneW") && W.IsReady())
            {
                if (wInstance)
                {
                    W.Cast(Player);
                }
                else
                {
                    W.Cast();
                }
            }

        }


        public override void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if(GetBool("comboE") && E.IsReady() && E.IsInRange(target))
                {
                    E.Cast();
                }
            }
            else if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (GetBool("harassE") && E.IsReady() && E.IsInRange(target))
                {
                    E.Cast();
                }
            }
            else if (OrbwalkerMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (GetBool("laneE") && E.IsReady())
                {
                    E.Cast();
                }
            }

        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly || gapcloser.End.Distance(Player.Position) <= gapcloser.Sender.AttackRange)
            {
                return;
            }

            if (GetBool("antigapcloserW") && W.IsReady())
            {
                W.Cast(Player);
            }
        }

        public override void OnPossibleToInterrupt(Obj_AI_Hero target, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel < Interrupter2.DangerLevel.High || target.IsAlly)
            {
                return;
            }

            if (GetBool("interruptR") && R.IsReady() && R.IsInRange(target))
            {
                R.Cast(target);
            }

        }

        public bool hasFlash()
        {
            return _flashSlot.IsReady();
        }

        public void fireFlash(Vector3 pos)
        {
            if (!hasFlash() || !GetBool("useFlash"))
                return;

            ObjectManager.Player.Spellbook.CastSpell(_flashSlot, pos);

        }

        /*
            Ward Jump by xSalice all rights reserved kappa
         */

        private void WardJump()
        {

            foreach (Obj_AI_Minion ward in ObjectManager.Get<Obj_AI_Minion>().Where(ward =>
                ward.Name.ToLower().Contains("ward") && ward.Distance(Game.CursorPos) < 250))
            {
                if (W.IsReady())
                {
                    W.CastOnUnit(ward);
                    return;
                }
            }

            foreach (
                Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.Distance(Game.CursorPos) < 250 && !hero.IsDead))
            {
                if (W.IsReady())
                {
                    W.CastOnUnit(hero);
                    return;
                }
            }

            foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion =>
                minion.Distance(Game.CursorPos) < 250))
            {
                if (W.IsReady())
                {
                    W.CastOnUnit(minion);
                    return;
                }
            }

            if (Environment.TickCount <= LastPlaced + 3000 || !W.IsReady()) return;

            Vector3 cursorPos = Game.CursorPos;
            Vector3 myPos = Player.ServerPosition;

            Vector3 delta = cursorPos - myPos;
            delta.Normalize();

            Vector3 wardPosition = myPos + delta * (600 - 5);

            InventorySlot invSlot = FindBestWardItem();
            if (invSlot == null) return;

            Items.UseItem((int)invSlot.Id, wardPosition);
            LastWardPos = wardPosition;
            LastPlaced = Environment.TickCount;
        }

        private static InventorySlot FindBestWardItem()
        {
            InventorySlot slot = Items.GetWardSlot();
            if (slot == default(InventorySlot)) return null;
            return slot;
        }

        /*
            End of Ward Jump
         */

        private void WardJump(Vector3 pos)
        {

            foreach (Obj_AI_Minion ward in ObjectManager.Get<Obj_AI_Minion>().Where(ward =>
                ward.Name.ToLower().Contains("ward") && ward.Distance(Game.CursorPos) < 250))
            {
                if (W.IsReady())
                {
                    W.CastOnUnit(ward);
                    return;
                }
            }

            if (Environment.TickCount <= LastPlaced + 3000 || !W.IsReady()) return;

            InventorySlot invSlot = FindBestWardItem();
            if (invSlot == null) return;

            Items.UseItem((int)invSlot.Id, pos);
            LastWardPos = pos;
            LastPlaced = Environment.TickCount;
        }


        private Vector3 insecPosition()
        {
            var target = TargetSelector.GetTarget(2500, TargetSelector.DamageType.Physical);

            var nearAlly =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => !x.IsEnemy)
                        .Where(x => !x.IsMe)
                        .Where(x => x.Distance(Player.Position) <= 2000)
                        .Where(x => !x.IsDead);

            if (nearAlly.FirstOrDefault() == null || target == null) return Vector3.Zero;

            var cursorPos = nearAlly.FirstOrDefault().Position;
            var myPos = target.Position;

            var delta = myPos - cursorPos;
            delta.Normalize();

            var wardPosition = myPos + delta * (200 - 5);

            return wardPosition;
        }

        public int enemiesInRange(Obj_AI_Hero obj, float range)
        {
            var nearEnemies =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => x.IsEnemy)
                        .Where(x => !x.IsDead)
                        .Where(x => x.Distance(obj.Position) <= range);
            return nearEnemies.Count();
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            if (enemy == null)
                return 0;

            double damage = 0d;

            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);
            }

            damage += Player.GetAutoAttackDamage(enemy) * 3;

            return (float)damage;
        }

        public override void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_AI_Minion))
                return;

            if (Environment.TickCount < LastPlaced + 300)
            {
                var ward = (Obj_AI_Minion)sender;
                if (ward.Name.ToLower().Contains("ward") && ward.Distance(LastWardPos) < 700 && W.IsReady())
                {
                    W.Cast(ward);
                }
            }
        }

        public override void Combo(Menu config)
        {
            config.AddItem(new MenuItem("starCombo", "Star Combo").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Toggle)));
            config.AddItem(new MenuItem("insecCombo", "Insec Combo").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Toggle)));
            config.AddItem(new MenuItem("comboQ", LanguageDic["use"] + " Q").SetValue(true));
            config.AddItem(new MenuItem("comboW", LanguageDic["use"] + " W").SetValue(true));
            config.AddItem(new MenuItem("comboE", LanguageDic["use"] + " E").SetValue(true));
            config.AddItem(new MenuItem("comboR", LanguageDic["use"] + " R").SetValue(true));

        }

        public override void Harass(Menu config)
        {
            //config.AddItem(new MenuItem("buffControlHarass", "Harass Buff Control").SetValue(true));
            config.AddItem(new MenuItem("harassQ", LanguageDic["use"] + " Q").SetValue(true));
            config.AddItem(new MenuItem("harassW", LanguageDic["use"] + " W").SetValue(true));
            config.AddItem(new MenuItem("harassE", LanguageDic["use"] + " E").SetValue(false));
        }

        public override void Laneclear(Menu config)
        {
            //config.AddItem(new MenuItem("buffControlLane", "Harass Buff Control").SetValue(true));
            config.AddItem(new MenuItem("laneQ", LanguageDic["use"] + " Q").SetValue(true));
            config.AddItem(new MenuItem("laneW", LanguageDic["use"] + " W").SetValue(true));
            config.AddItem(new MenuItem("laneE", LanguageDic["use"] + " E").SetValue(false));
        }

        public override void Misc(Menu config)
        {
            config.AddItem(new MenuItem("useFlash", "Use Flash on Insec").SetValue(true));
        }

        public override void Extra(Menu config)
        {
            config.AddItem(new MenuItem("antigapcloserW", LanguageDic["use"] + " W " + LanguageDic["gap"]).SetValue(true));
            config.AddItem(new MenuItem("interruptR", LanguageDic["use"] + " R " + LanguageDic["interrupt"]).SetValue(true));
        }

        public override void Drawings(Menu config)
        {
            config.AddItem(new MenuItem("disableAll", LanguageDic["disable"]).SetValue(false));
            config.AddItem(new MenuItem("drawQ", LanguageDic["show"] + " Q").SetValue(true));
            config.AddItem(new MenuItem("drawW", LanguageDic["show"] + " W").SetValue(true));
            config.AddItem(new MenuItem("drawE", LanguageDic["show"] + " E").SetValue(true));
            config.AddItem(new MenuItem("drawR", LanguageDic["show"] + " R").SetValue(true));
            config.AddItem(new MenuItem("drawInsecPos", LanguageDic["show"] + " Insec Position").SetValue(true));
        }
    }
}
