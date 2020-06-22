// Project:         Races Redone mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2020 Ralzar
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Ralzar

using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using UnityEngine;
using DaggerfallWorkshop.Game.Utility;
using System;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop;

namespace RacesRedone
{

    public class RacesRedone : MonoBehaviour
    {
        static Mod mod;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            go.AddComponent<RacesRedone>();
            StartGameBehaviour.OnStartGame += RacesRedone_OnStartGame;
        }

        static bool classic = false;
        static bool modern = false;

        static int agility;
        static int endurance;
        static int intelligence;
        static int luck;
        static int personality;
        static int speed;
        static int strength;
        static int willpower;
        static bool male;
        static bool dumbPlayer;

        PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

        void Awake()
        {
            ModSettings settings = mod.GetSettings();
            int rules = settings.GetValue<int>("Features", "Rules");
            classic = (rules == 0);
            modern = (rules == 1);
            mod.IsReady = true;
            Debug.Log("[Races Redone] Classic Rules = " + classic.ToString());
            Debug.Log("[Races Redone] Modern Rules = " + modern.ToString());
            Debug.Log("[Races Redone] Ready");
            Debug.Log("[Races Redone] Awake dumbPlayer = " + dumbPlayer.ToString());
        }

        void Start()
        {
            if (modern)
            {
                FormulaHelper.RegisterOverride<Func<DaggerfallEntity, DaggerfallUnityItem, PlayerEntity, FormulaHelper.ToHitAndDamageMods>>(mod, "CalculateRacialModifiers", (DaggerfallEntity attacker, DaggerfallUnityItem weapon, PlayerEntity player) =>
                {
                    FormulaHelper.ToHitAndDamageMods mods = new FormulaHelper.ToHitAndDamageMods();
                    if (weapon == null)
                    {
                        if (player.RaceTemplate.ID == (int)Races.Khajiit)
                        {
                            mods.damageMod = attacker.Level / 2;
                            mods.toHitMod = attacker.Level / 2;
                        }
                    }
                    else if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.ShortBlade)
                    {
                        if (player.RaceTemplate.ID == (int)Races.Argonian)
                        {
                            mods.damageMod = attacker.Level / 2;
                            mods.toHitMod = attacker.Level / 2;
                        }
                        else if (player.RaceTemplate.ID == (int)Races.DarkElf)
                        {
                            mods.damageMod = attacker.Level / 3;
                            mods.toHitMod = attacker.Level / 3;
                        }
                        else if (player.RaceTemplate.ID == (int)Races.Khajiit)
                        {
                            mods.damageMod = attacker.Level / 4;
                            mods.toHitMod = attacker.Level / 4;
                        }
                    }
                    else if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.LongBlade)
                    {
                        if (player.RaceTemplate.ID == (int)Races.DarkElf)
                        {
                            mods.damageMod = attacker.Level / 4;
                            mods.toHitMod = attacker.Level / 4;
                        }
                        else if (player.RaceTemplate.ID == (int)Races.Redguard)
                        {
                            mods.damageMod = attacker.Level / 3;
                            mods.toHitMod = attacker.Level / 3;
                        }
                    }
                    else if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.Axe)
                    {
                        if (player.RaceTemplate.ID == (int)Races.Nord)
                        {
                            mods.damageMod = attacker.Level / 3;
                            mods.toHitMod = attacker.Level / 3;
                        }
                    }
                    else if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.BluntWeapon)
                    {
                        if (player.RaceTemplate.ID == (int)Races.Nord)
                        {
                            mods.damageMod = attacker.Level / 3;
                            mods.toHitMod = attacker.Level / 3;
                        }
                    }
                    else if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.Archery)
                    {
                        if (player.RaceTemplate.ID == (int)Races.WoodElf)
                        {
                            mods.damageMod = attacker.Level / 3;
                            mods.toHitMod = attacker.Level / 3;
                        }
                        else if (player.RaceTemplate.ID == (int)Races.DarkElf)
                        {
                            mods.damageMod = attacker.Level / 4;
                            mods.toHitMod = attacker.Level / 4;
                        }
                    }
                    Debug.Log("[Races Redone] Racial Modifier Damage = " + mods.damageMod.ToString());
                    Debug.Log("[Races Redone] Racial Modifier ToHit = " + mods.toHitMod.ToString());
                    return mods;
                });
            }
        }        

        void Update()
        {
            if (!DaggerfallUnity.Instance.IsReady || GameManager.IsGamePaused)
                return;
            if (modern && playerEntity.CurrentBreath < playerEntity.MaxBreath && playerEntity.BirthRaceTemplate.ID == (int)Races.Argonian)
                {
                    playerEntity.SetBreath(playerEntity.MaxBreath);
                }
        }

        private static void RacesRedone_OnStartGame(object sender, EventArgs e)
        {
            Debug.Log("[Races Redone] Starting");
            GetAttributes();

            if (classic)
            {
                ClassicRules();
                Debug.Log("[Races Redone] Character Adjusted by Classic Rules");
            }
            else if (modern)
            {
                ModernRules();
                Debug.Log("[Races Redone] Character Adjusted by Modern Rules");
            }
            else { Debug.Log("[Races Redone] No Rules. Character NOT adjusted."); }                     
        }


        private static void ClassicRules()
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            RaceTemplate playerRace = playerEntity.RaceTemplate;
            male = playerEntity.Gender == Genders.Male;

            if (playerRace.ID == (int)Races.Argonian)
            {
                Debug.Log("[Races Redone] Is Argonian");
                if (male)
                {
                    agility += 10;
                    speed += 10;
                    endurance -= 10;
                }
                else
                {
                    strength += 10;
                    endurance -= 10;
                }
            }
            else if (playerRace.ID == (int)Races.Breton)
            {
                Debug.Log("[Races Redone] Is Breton");
                if (male)
                {
                    intelligence += 10;
                    willpower += 10;
                    strength -= 10;
                    endurance -= 10;
                }
                else
                {
                    intelligence += 10;
                    willpower += 10;
                    strength -= 10;
                    endurance -= 10;
                }
            }
            else if (playerRace.ID == (int)Races.DarkElf)
            {
                Debug.Log("[Races Redone] Is DarkElf");
                if (male)
                {
                    strength += 10;
                    intelligence += 10;
                    willpower -= 10;
                    personality -= 10;
                }
                else
                {
                    intelligence += 10;
                    willpower -= 10;
                }
            }
            else if (playerRace.ID == (int)Races.HighElf)
            {
                Debug.Log("[Races Redone] Is HighElf");
                if (male)
                {
                    intelligence += 10;
                    willpower += 10;
                    strength -= 10;
                    endurance -= 10;
                }
                else
                {
                    intelligence += 10;
                    personality += 10;
                    strength -= 10;
                    endurance -= 10;
                }
            }
            else if (playerRace.ID == (int)Races.Khajiit)
            {
                Debug.Log("[Races Redone] Is Khajiit");
                if (male)
                {
                    endurance += 10;
                    willpower -= 10;
                    agility -= 10;
                }
                else
                {
                    luck += 10;
                }
            }
            else if (playerRace.ID == (int)Races.Nord)
            {
                Debug.Log("[Races Redone] Is Nord");
                if (male)
                {
                    strength += 10;
                    endurance += 10;
                    intelligence -= 10;
                    willpower -= 10;
                    agility -= 10;
                }
                else
                {
                    luck += 10;
                    intelligence -= 10;
                }
            }
            else if (playerRace.ID == (int)Races.Redguard)
            {
                Debug.Log("[Races Redone] Is Redguard");
                if (male)
                {
                    agility += 10;
                    luck += 10;
                    intelligence -= 10;
                    willpower -= 10;
                    endurance -= 10;
                }
                else
                {
                    speed += 10;
                    luck += 10;
                    strength -= 10;
                    intelligence -= 10;
                    willpower -= 10;
                    endurance -= 10;
                }
            }
            else if (playerRace.ID == (int)Races.WoodElf)
            {
                Debug.Log("[Races Redone] Is WoodElf");
                if (male)
                {
                    agility += 10;
                    speed += 10;
                    endurance -= 10;
                    luck -= 10;
                }
                else
                {
                }
            }


            SetAttributes();


            if (dumbPlayer)
            {
                KillDumbPlayer();
            }
        }

        private static void ModernRules()
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            RaceTemplate playerRace = playerEntity.RaceTemplate;


            if (playerRace.ID == (int)Races.Argonian)
            {
                Debug.Log("[Races Redone] Is Argonian");
                playerEntity.RaceTemplate.ImmunityFlags = DFCareer.EffectFlags.Poison;
                playerEntity.RaceTemplate.ResistanceFlags = DFCareer.EffectFlags.Disease;
                intelligence += 10;
                agility += 10;
                speed += 10;
                personality -= 20;
                endurance -= 10;
            }
            else if (playerRace.ID == (int)Races.Breton)
            {
                Debug.Log("[Races Redone] Is Breton");

                playerEntity.Career.SpellPointMultiplierValue += 0.5f;
                playerEntity.CurrentMagicka = playerEntity.MaxMagicka;
                intelligence += 10;
                willpower += 10;
                strength -= 10;
                endurance -= 10;
            }
            else if (playerRace.ID == (int)Races.DarkElf)
            {
                Debug.Log("[Races Redone] Is DarkElf");
                playerEntity.RaceTemplate.ResistanceFlags = DFCareer.EffectFlags.Fire;
                strength += 10;
                intelligence += 10;
                speed += 10;
                willpower -= 20;
                personality -= 10;
            }
            else if (playerRace.ID == (int)Races.HighElf)
            {
                Debug.Log("[Races Redone] Is HighElf");
                playerEntity.RaceTemplate.ResistanceFlags = DFCareer.EffectFlags.Disease;
                playerEntity.RaceTemplate.LowToleranceFlags = (byte)DFCareer.EffectFlags.Shock+(byte)DFCareer.EffectFlags.Magic+(byte)DFCareer.EffectFlags.Fire+DFCareer.EffectFlags.Frost;
                playerEntity.Career.SpellPointMultiplierValue += 1f;
                intelligence += 10;
                willpower += 10;
                agility += 10;
                strength -= 20;
                endurance -= 10;
            }
            else if (playerRace.ID == (int)Races.Khajiit)
            {
                Debug.Log("[Races Redone] Is Khajiit");
                playerEntity.Career.AcuteHearing = true;
                playerEntity.Career.Athleticism = true;
                endurance += 10;
                intelligence += 10;
                agility += 10;
                willpower -= 20;
                personality -= 10;
            }
            else if (playerRace.ID == (int)Races.Nord)
            {
                Debug.Log("[Races Redone] Is Nord");
                endurance += 20;
                strength += 10;
                intelligence -= 20;
                willpower -= 10;
            }
            else if (playerRace.ID == (int)Races.Redguard)
            {
                Debug.Log("[Races Redone] Is Redguard");
                playerEntity.RaceTemplate.ResistanceFlags = (byte)DFCareer.EffectFlags.Disease+ DFCareer.EffectFlags.Poison;
                playerEntity.Career.AdrenalineRush = true;
                endurance += 10;
                agility += 10;
                speed += 10;
                intelligence -= 20;
                willpower -= 10;
            }
            else if (playerRace.ID == (int)Races.WoodElf)
            {
                Debug.Log("[Races Redone] Is WoodElf");
                playerEntity.RaceTemplate.ResistanceFlags = DFCareer.EffectFlags.Disease;
                agility += 20;
                speed += 10;
                endurance -= 10;
                strength -= 20;
            }


            SetAttributes();


            if (dumbPlayer)
            {
                KillDumbPlayer();
            }
        }


        private static void GetAttributes()
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            RaceTemplate playerRace = playerEntity.RaceTemplate;
            DaggerfallStats stats = playerEntity.Stats;

            agility = playerEntity.Stats.PermanentAgility;
            endurance = playerEntity.Stats.PermanentEndurance;
            intelligence = playerEntity.Stats.PermanentIntelligence;
            luck = playerEntity.Stats.PermanentLuck;
            personality = playerEntity.Stats.PermanentPersonality;
            speed = playerEntity.Stats.PermanentSpeed;
            strength = playerEntity.Stats.PermanentStrength;
            willpower = playerEntity.Stats.PermanentWillpower;
            
        }

        private static void SetAttributes()
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            DaggerfallStats stats = playerEntity.Stats;
            RaceTemplate playerRace = playerEntity.RaceTemplate;
            int maxStat = FormulaHelper.MaxStatValue();

            if (agility > maxStat || endurance > maxStat || intelligence > maxStat || luck > maxStat || personality > maxStat || speed > maxStat || strength > maxStat || willpower > maxStat)
            {
                string[] messages = new string[] { "Your racial modifiers have increased one or", "more Attribute over the maximum limit. It will be reduced." };
                StatusPopup(messages);
            }

            if (agility > maxStat) { agility = maxStat; }
            if (endurance > maxStat) { endurance = maxStat; }
            if (intelligence > maxStat) { intelligence = maxStat; }
            if (luck > maxStat) { luck = maxStat; }
            if (personality > maxStat) { personality = maxStat; }
            if (speed > maxStat) { speed = maxStat; }
            if (strength > maxStat) { strength = maxStat; }
            if (willpower > maxStat) { willpower = maxStat; }

            if (agility <= 0) { dumbPlayer = true; }
            if (endurance <= 0) { dumbPlayer = true; }
            if (intelligence <= 0) { dumbPlayer = true; }
            if (luck <= 0) { dumbPlayer = true; }
            if (personality <= 0) { dumbPlayer = true; }
            if (speed <= 0) { dumbPlayer = true; }
            if (strength <= 0) { dumbPlayer = true; }
            if (willpower <= 0) { dumbPlayer = true; }

            stats.SetPermanentStatValue(DFCareer.Stats.Agility, agility);
            stats.SetPermanentStatValue(DFCareer.Stats.Endurance, endurance);
            stats.SetPermanentStatValue(DFCareer.Stats.Intelligence, intelligence);
            stats.SetPermanentStatValue(DFCareer.Stats.Luck, luck);
            stats.SetPermanentStatValue(DFCareer.Stats.Personality, personality);
            stats.SetPermanentStatValue(DFCareer.Stats.Speed, speed);
            stats.SetPermanentStatValue(DFCareer.Stats.Strength, strength);
            stats.SetPermanentStatValue(DFCareer.Stats.Willpower, willpower);

        }

        private static void KillDumbPlayer()
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            RaceTemplate playerRace = playerEntity.RaceTemplate;
            DaggerfallStats stats = playerEntity.Stats;
            Debug.Log("[Races Redone] KillDumbPlayer dumbPlayer = " + dumbPlayer.ToString());
            dumbPlayer = false;
            string[] messages = new string[] { "Your racial modifiers have reduced an Attribute to 0, which kills you." };
            StatusPopup(messages);
        }

        static DaggerfallMessageBox tempInfoBox;

        public static void StatusPopup(string[] message)
        {
            if (tempInfoBox == null)
            {
                tempInfoBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
                tempInfoBox.AllowCancel = true;
                tempInfoBox.ClickAnywhereToClose = true;
                tempInfoBox.ParentPanel.BackgroundColor = Color.clear;
            }

            tempInfoBox.SetText(message);
            DaggerfallUI.UIManager.PushWindow(tempInfoBox);
        }

    }
}