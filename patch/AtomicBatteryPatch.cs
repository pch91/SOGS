using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using Assets.Scripts.Objects.Entities;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using System;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading;

namespace sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.patch
{

    public static class BateryStatic
    {
        public static float OuchDps = float.Parse(StaticAttributes.Baterryconfigs["OuchDps"].ToString()) / 10f;
        public static float OuchRadius = float.Parse(StaticAttributes.Baterryconfigs["OuchRadius"].ToString());
        public static float fatRechargeBateryNuke = float.Parse(StaticAttributes.Baterryconfigs["fatRechargeBateryNuke"].ToString());
        public static float damageRechargeBateryNuke = float.Parse(StaticAttributes.Baterryconfigs["damageRechargeBateryNuke"].ToString());
    }

    [HarmonyPatch(typeof(BatteryCell), "OnPowerTick")]
    public class AtomicBatteryPatch
    {
        static bool descharged = false;

        [UsedImplicitly]
        private static void Prefix(BatteryCell __instance)
        {
            try
            {
                if (bool.Parse(StaticAttributes.configs["EnabledB"].ToString())) {

                    if (__instance.PrefabName != "ItemBatteryCellNuclear")
                        return;

                    SOGS.log("AtomicBatteryPatch :: Prefix --> " + __instance.ReferenceId + " vida " + __instance.DamageState.Total, SOGS.Logs.DEBUG);
                    SOGS.log("AtomicBatteryPatch :: Prefix --> " + __instance.ReferenceId + " MaxDamage " + __instance.DamageState.MaxDamage, SOGS.Logs.DEBUG);
                    SOGS.log("AtomicBatteryPatch :: Prefix --> " + __instance.ReferenceId + " __instance.IsCharged " + __instance.IsCharged, SOGS.Logs.DEBUG);
                    SOGS.log("AtomicBatteryPatch :: Prefix --> " + __instance.ReferenceId + " descharged " + descharged, SOGS.Logs.DEBUG);
                    SOGS.log("AtomicBatteryPatch :: Prefix --> " + __instance.ReferenceId + " __instance.IsEmpty " + __instance.IsEmpty, SOGS.Logs.DEBUG);
                    // RECHARGE AND DAMEGE BATTERY
                    float fatheal = (__instance.PowerMaximum * BateryStatic.fatRechargeBateryNuke);
                    if (__instance.IsEmpty && (__instance.PowerMaximum - __instance.DamageState.Total > fatheal))
                    {
                        SOGS.log("AtomicBatteryPatch :: Prefix --> stage1" + __instance.ReferenceId, SOGS.Logs.DEBUG);
                        SOGS.log("AtomicBatteryPatch :: Prefix --> Baterry"+ __instance.ReferenceId + " sufer damage "+ MathF.Round(__instance.DamageState.MaxDamage * BateryStatic.damageRechargeBateryNuke, 6), SOGS.Logs.DEBUG);
                        __instance.DamageState.Damage(ChangeDamageType.Increment, MathF.Round(__instance.DamageState.MaxDamage * BateryStatic.damageRechargeBateryNuke, 6), DamageUpdateType.Decay);

                        new Thread(() =>
                        {
                            Thread.CurrentThread.IsBackground = true;
                            SOGS.log("AtomicBatteryPatch :: Prefix --> Tread stage1 recharge " + __instance.ReferenceId, SOGS.Logs.DEBUG);
                            SOGS.log("AtomicBatteryPatch :: Prefix --> Tread " + __instance.ReferenceId + " rounds " + __instance.PowerMaximum / (fatheal), SOGS.Logs.DEBUG);

                            if (__instance.PowerMaximum - __instance.DamageState.Total > fatheal) {
                                for (int i = 0; i < __instance.PowerMaximum / fatheal; i++)
                                {

                                    SOGS.log("AtomicBatteryPatch :: Prefix --> Tread will recharge " + __instance.ReferenceId + " rodadas " + __instance.AddPowerSafe(fatheal), SOGS.Logs.DEBUG);
                                    SOGS.log("AtomicBatteryPatch :: Prefix --> Tread stage1 " + __instance.ReferenceId + " round " + i, SOGS.Logs.DEBUG);

                                    SOGS.log("AtomicBatteryPatch :: Prefix --> Tread stage1 go sleep" + __instance.ReferenceId, SOGS.Logs.DEBUG);

                                    Thread.Sleep(2000);
                                }
                            }
                        }).Start();

                    }

                    // DAMEGE THINGS
                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        var pos = __instance.Position;
                        //var ouchRadiusSqr = MathF.Round((2*Mathf.PI)*OuchRadius,6);
                        foreach (var stuff in Thing._colliderLookup.Values)
                        {
                            var thing = stuff;

                            //this part needs to be run on main thread i gues
                            //var col = stuff.Key;
                            var p = stuff is Human;
                            var o = stuff is Plant || stuff is Food || stuff is StackableFood;

                            bool humanconditiondamage = false;

                            if (p)
                            {
                                var human = stuff as Human;
                                humanconditiondamage = human.HelmetSlot.Occupant != null && !human.HelmetSlot.Occupant.IsOpen && human.SuitSlot.Occupant != null && human.SuitSlot.Occupant.Powered;

                            }
                            //var thingPos = stuff.Key.ClosestPointOnBounds(pos);
                            //var distanceSqr = thing.Bounds.SqrDistance(pos);
                            var distanceSqr = MathF.Round((pos - thing.Position).sqrMagnitude,6);
                            var efetiveRadios = BateryStatic.OuchRadius + (__instance.DamageState.Total / 100);

                            if (distanceSqr > 0 && distanceSqr <= efetiveRadios && stuff != __instance)
                            {
                               SOGS.log("AtomicBatteryPatch :: Prefix --> " + thing.DisplayName + "pos in " + pos, SOGS.Logs.DEBUG);
                               SOGS.log("AtomicBatteryPatch :: Prefix --> " + thing.DisplayName + "thing.Position in " + thing.Position, SOGS.Logs.DEBUG);
                               SOGS.log("AtomicBatteryPatch :: Prefix --> " + thing.DisplayName + "(pos - thing.Position) in " + (pos - thing.Position), SOGS.Logs.DEBUG);
                               SOGS.log("AtomicBatteryPatch :: Prefix --> " + thing.DisplayName + "Distancia da bateria in " + distanceSqr, SOGS.Logs.DEBUG);
                               SOGS.log("AtomicBatteryPatch :: Prefix --> " + thing.DisplayName + "Raio de dano in " + OuchRadius, SOGS.Logs.DEBUG);

                                if (p)
                                {
                                    if (!humanconditiondamage)
                                    {
                                        float damage = (MathF.Pow(distanceSqr, (-0.000001f / 1)) / 1) * BateryStatic.OuchDps;
                                        SOGS.log("AtomicBatteryPatch :: Prefix --> Baterry cause damage in "+ thing.DisplayName+" damage: "+ damage, SOGS.Logs.DEBUG);
                                        thing.DamageState.Damage(ChangeDamageType.Increment, damage, DamageUpdateType.Radiation);
                                    }
                                }
                                else if (o)
                                {
                                    float damage = (MathF.Pow(distanceSqr, (-0.000001f / 1)) / 1) * BateryStatic.OuchDps;
                                    SOGS.log("AtomicBatteryPatch :: Prefix --> Baterry cause damage in " + thing.DisplayName + " damage: " + damage, SOGS.Logs.DEBUG);
                                    thing.DamageState.Damage(ChangeDamageType.Increment, damage, DamageUpdateType.Radiation);
                                }
                            }
                        }
                    }).Start();
                }
            }catch(Exception e)
            {
                Debug.LogException(e);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}