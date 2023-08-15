using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using Assets.Scripts.Objects.Entities;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.patch
{
    [HarmonyPatch]
    public class AtomicBatteryPatch
    {
        static bool descharged = false;

        static float OuchDps = float.Parse(StaticAttributes.Baterryconfigs["OuchDps"].ToString()) / 10f;
        static float OuchRadius = float.Parse(StaticAttributes.Baterryconfigs["OuchRadius"].ToString());
        static float fatRechargeBateryNuke = float.Parse(StaticAttributes.Baterryconfigs["fatRechargeBateryNuke"].ToString());
        static float damageRechargeBateryNuke = float.Parse(StaticAttributes.Baterryconfigs["damageRechargeBateryNuke"].ToString());

        [HarmonyPatch(typeof(BatteryCell), "OnPowerTick")]
        [UsedImplicitly]
        private static void Prefix(BatteryCell __instance)
        {
            try
            {
                if (bool.Parse(StaticAttributes.configs["EnabledB"].ToString())) {

                    if (__instance.PrefabName != "ItemBatteryCellNuclear")
                        return;

                    if (!__instance.IsCharged && descharged)
                    {
                        __instance.AddPowerSafe(__instance.PowerMaximum * fatRechargeBateryNuke); //almost reasonable

                    }
                    else if (__instance.IsEmpty)
                    {
                        SOGS.log("AtomicBatteryPatch :: Prefix --> Baterry sufer damage "+ MathF.Round(__instance.DamageState.MaxDamage * damageRechargeBateryNuke, 2), SOGS.Logs.DEBUG);
                        __instance.DamageState.Damage(ChangeDamageType.Increment, MathF.Round(__instance.DamageState.MaxDamage * damageRechargeBateryNuke,2), DamageUpdateType.Radiation);
                        descharged = true;
                    }
                    else if (__instance.IsCharged)
                    {
                        descharged = false;
                    }

                    //__instance.AddPowerSafe(100);

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
                        SOGS.log("AtomicBatteryPatch :: Prefix --> "+ thing.DisplayName + "pos in " + pos, SOGS.Logs.DEBUG);
                        SOGS.log("AtomicBatteryPatch :: Prefix --> "+ thing.DisplayName + "thing.Position in " + thing.Position, SOGS.Logs.DEBUG);
                        SOGS.log("AtomicBatteryPatch :: Prefix --> "+ thing.DisplayName + "(pos - thing.Position) in " + (pos - thing.Position), SOGS.Logs.DEBUG);
                        SOGS.log("AtomicBatteryPatch :: Prefix --> "+ thing.DisplayName + "Distancia da bateria in " + distanceSqr, SOGS.Logs.DEBUG);
                        SOGS.log("AtomicBatteryPatch :: Prefix --> " + thing.DisplayName + "Raio de dano in " + OuchRadius, SOGS.Logs.DEBUG);
                        
                        if (distanceSqr > 0 && distanceSqr <= OuchRadius && stuff != __instance)
                        {
                            if (p)
                            {
                                if (!humanconditiondamage)
                                {
                                    float damage = (MathF.Pow(distanceSqr, (-0.000001f / 1)) / 1) * OuchDps;
                                    SOGS.log("AtomicBatteryPatch :: Prefix --> Baterry cause damage in "+ thing.DisplayName+" damage: "+ damage, SOGS.Logs.DEBUG);
                                    thing.DamageState.Damage(ChangeDamageType.Increment, damage, DamageUpdateType.Radiation);
                                }
                            }
                            else if (o)
                            {
                                float damage = (MathF.Pow(distanceSqr, (-0.000001f / 1)) / 1) * OuchDps;
                                SOGS.log("AtomicBatteryPatch :: Prefix --> Baterry cause damage in " + thing.DisplayName + " damage: " + damage, SOGS.Logs.DEBUG);
                                thing.DamageState.Damage(ChangeDamageType.Increment, damage, DamageUpdateType.Radiation);
                            }
                        }
                    }
                }
            }catch(Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}