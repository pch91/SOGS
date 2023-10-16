using Assets.Scripts.Atmospherics;
using HarmonyLib;
using JetBrains.Annotations;
using sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.patch;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.Scripts
{

    [HarmonyPatch(typeof(Atmosphere), "Combust")]
    public class AtmosphereCombustPatch
    {
        [UsedImplicitly]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by mod loader")]
        private static void Prefix(Atmosphere __instance,ref float rateOverride,
                                   ref bool ____inflamed,
                                   out Dictionary<string, object> __state)
        {

            __state = new Dictionary<string, object>();
            /* try old
             {
                 float molesOfH2 = Mathf.Min(__instance.GasMixture.TotalFuel, __instance.GasMixture.Oxygen.Quantity * 2f);
                 float molesOfO2 = molesOfH2 / 2f;
                 float burnPercentage = 0.95f;
                 float removedMolesH2 = molesOfH2 * burnPercentage;
                 float removedMolesO2 = molesOfO2 * burnPercentage;
                 Mole burnt = __instance.GasMixture.Volatiles.Remove(removedMolesH2);
                 burnt.Add(__instance.GasMixture.Oxygen.Remove(removedMolesO2));

                 __instance.GasMixture.Water.Add(new Mole(Chemistry.GasType.Water, burnt.Quantity * Mathf.Clamp(Convert.ToSingle(StaticAttributes.Combustonconfigs["FCW"].ToString()),0,1) ), 0f)
                 {
                     Energy = burnt.Energy
                 });

                 SOGS.log("AtmosphereCombustPatch :: Prefix --> combustion create a wather ", SOGS.Logs.DEBUG);

                 __instance.CombustionEnergy = __instance.GasMixture.Volatiles.Enthalpy * removedMolesH2;
                 __instance.GasMixture.AddEnergy(__instance.CombustionEnergy);
                 __instance.BurnedPropaneRatio = removedMolesH2;
                 __instance.CleanBurnRate = float.Parse(typeof(Atmosphere).GetMethod("CombustableMix", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, null).ToString());
                 ____inflamed = true;
             }
             catch (Exception e)
             {
                 Debug.LogException(e);
             }*/
            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.GasMixture.Oxygen.Quantity + __instance.GasMixture.LiquidOxygen.Quantity + " TotalOxygen  " + __instance.ReferenceId, SOGS.Logs.DEBUG);
            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.GasMixture.TotalFuel + " TotalFuel  " + __instance.ReferenceId, SOGS.Logs.DEBUG);
            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.BurnedPropaneRatio + " BurnedPropaneRatio  " + __instance.ReferenceId, SOGS.Logs.DEBUG);
            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.CleanBurnRate + " CleanBurnRate  " + __instance.ReferenceId, SOGS.Logs.DEBUG);

            // copy of Combust -> Atmosphere
            float totalFuel = __instance.GasMixture.TotalFuel;
            float quantity = __instance.GasMixture.NitrousOxide.Quantity;
            float num = __instance.GasMixture.Oxygen.Quantity + __instance.GasMixture.LiquidOxygen.Quantity;
            bool flag = totalFuel < AtmosphereHelper.MinimumMolesForProcessing || (num < AtmosphereHelper.MinimumMolesForProcessing && quantity < AtmosphereHelper.MinimumMolesForProcessing);
            float num2 = num * 2f + quantity;
            float num3 = num * 2f / num2;
            float num4 = quantity / num2;
            float num5 = __instance.GasMixture.LiquidVolatiles.Quantity / totalFuel;
            float num6 = __instance.GasMixture.LiquidOxygen.Quantity / num;
            float num7 = Mathf.Min(totalFuel, num2);
            float num8 = num7 * (num3 / 2f);
            float num9 = num7 * num4;
            float num10;
            if (rateOverride != 0f)
            {
                num10 = Mathf.Clamp(rateOverride, 0f, 1f);
            }
            else if (flag)
            {
                num10 = 1f;
            }
            else if (num4 >= 0.1f)
            {
                num10 = 1f / MathF.Pow(0.0025f * (__instance.GasMixture.Temperature + 273f), 1.01f) + 0.05f;
                num10 = Mathf.Clamp(num10, 0f, 1f) / 5f;
            }
            else
            {
                num10 = 1f / MathF.Pow(0.002f * (__instance.GasMixture.Temperature + 273f), 1.6f) + 0.05f;
                num10 = Mathf.Clamp(num10, 0f, 1f) / 5f;
            }
            float num11 = num7 * num10 * num3;
            float num12 = num7 * num10 * num4;
            float num13 = num8 * num10;
            float removedMoles = num9 * num10;

            // sum values of h2 and 02 .... 
            float removedOxigen = (num13 * num6)+(num13 * (1f - num6))+ removedMoles;
            float removedh2 = ((num11 * (1f - num5))/2) + ((num11 * num5)/2) + ((num12 * (1f - num5))/2) + ((num12 * num5)/2);

            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + removedOxigen + " removedOxigen  " + __instance.ReferenceId, SOGS.Logs.DEBUG);
            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + removedh2 + " removedh2  " + __instance.ReferenceId, SOGS.Logs.DEBUG);
            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + MathF.Min(removedOxigen, removedh2) * Mathf.Clamp(Convert.ToSingle(StaticAttributes.Combustonconfigs["FCW"].ToString()), 0, 1) + " add agua " + __instance.ReferenceId, SOGS.Logs.DEBUG);
            ///////////////////////////
            
            //// --- new add stream

            __instance.GasMixture.Steam.Add(new Mole(Chemistry.GasType.Steam, MathF.Min(removedOxigen, removedh2) * Mathf.Clamp(Convert.ToSingle(StaticAttributes.Combustonconfigs["FCW"].ToString()),0,1), 0f)
            {
                Energy = __instance.CombustionEnergy
            });
        }


        [UsedImplicitly]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by mod loader")]
        private static void Posfix(Atmosphere __instance,
                                   ref bool ____inflamed,
                                   Dictionary<string, object> __state)
        {

        }

    }
}
