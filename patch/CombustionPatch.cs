using Assets.Scripts.Atmospherics;
using Assets.Scripts.Objects.Items;
using Assets.Scripts.Objects.Pipes;
using HarmonyLib;
using JetBrains.Annotations;
using Reagents;
using sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.patch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using static Assets.Scripts.Objects.Slot;

namespace sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.Scripts
{

    [HarmonyPatch(typeof(Atmosphere), "Combust")]
    public class AtmosphereCombustPatch
    {
        [UsedImplicitly]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by mod loader")]
        private static void Prefix(Atmosphere __instance, ref float rateOverride,
                                   ref bool ____inflamed,
                                   out Dictionary<string, object> __state)
        {

            __state = new Dictionary<string, object>();

            GasMixture copygasMixture = new GasMixture(__instance.GasMixture);


            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.GasMixture.Oxygen.Quantity + __instance.GasMixture.LiquidOxygen.Quantity + " TotalOxygen  " + __instance.ReferenceId, SOGS.Logs.DEBUG);
            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.GasMixture.TotalFuel + " TotalFuel  " + __instance.ReferenceId, SOGS.Logs.DEBUG);
            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.BurnedPropaneRatio + " BurnedPropaneRatio  " + __instance.ReferenceId, SOGS.Logs.DEBUG);
            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.CleanBurnRate + " CleanBurnRate  " + __instance.ReferenceId, SOGS.Logs.DEBUG);

            // copy of Combust -> Atmosphere
            float totalFuel = __instance.GasMixture.TotalFuel;
            float totalno = __instance.GasMixture.NitrousOxide.Quantity + __instance.GasMixture.LiquidNitrousOxide.Quantity;
            float Totalo = __instance.GasMixture.Oxygen.Quantity + __instance.GasMixture.LiquidOxygen.Quantity;

            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.ReferenceId + " TotalFuel  " + __instance.GasMixture.TotalFuel, SOGS.Logs.DEBUG);
            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.ReferenceId + " totalno  " + totalno, SOGS.Logs.DEBUG);
            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.ReferenceId + " Totalo  " + Totalo, SOGS.Logs.DEBUG);



            bool flag = totalFuel < AtmosphereHelper.MinimumMolesForProcessing || (Totalo < AtmosphereHelper.MinimumMolesForProcessing && totalno < AtmosphereHelper.MinimumMolesForProcessing);

            float num3 = Totalo * 2f + totalno;
            float num4 = Totalo * 2f / num3;
            float num5 = totalno / num3;
            float num6 = __instance.GasMixture.LiquidVolatiles.Quantity / totalFuel;
            float num7 = __instance.GasMixture.LiquidOxygen.Quantity / Totalo;
            float num8 = __instance.GasMixture.LiquidNitrousOxide.Quantity / totalno;
            float num9 = Mathf.Min(totalFuel, num3);
            float num10 = num9 * (num4 / 2f);
            float num11 = num9 * num5;
            float num12;

            if (rateOverride != 0f)
            {
                num12 = Mathf.Clamp(rateOverride, 0f, 1f);
            }
            else if (flag)
            {
                num12 = 1f;
            }
            else if (num5 >= 0.1f)
            {
                num12 = 1f / MathF.Pow(0.0025f * (__instance.GasMixture.Temperature + 273f), 1.01f) + 0.05f;
                num12 = Mathf.Clamp(num12, 0f, 1f) / 5f;
            }
            else
            {
                num12 = 1f / MathF.Pow(0.002f * (__instance.GasMixture.Temperature + 273f), 1.6f) + 0.05f;
                num12 = Mathf.Clamp(num12, 0f, 1f) / 5f;
            }
            float num13 = num9 * num12 * num4;
            float num14 = num9 * num12 * num5;
            float num15 = num10 * num12;
            float num16 = num11 * num12;

            GasMixture o2comb = new GasMixture(copygasMixture.Volatiles.Remove(num13 * (1f - num6)));
            o2comb.Add(copygasMixture.LiquidVolatiles.Remove(num13 * num6));
            o2comb.Add(copygasMixture.LiquidOxygen.Remove(num15 * num7));
            o2comb.Add(copygasMixture.Oxygen.Remove(num15 * (1f - num7)));
            GasMixture no2comb = new GasMixture(copygasMixture.Volatiles.Remove(num14 * (1f - num6)));
            no2comb.Add(copygasMixture.LiquidVolatiles.Remove(num14 * num6));
            no2comb.Add(copygasMixture.LiquidNitrousOxide.Remove(num16 * num8));
            no2comb.Add(copygasMixture.NitrousOxide.Remove(num16 * (1f - num8)));

            float remove = no2comb.TotalMolesGassesAndLiquids + o2comb.TotalMolesGassesAndLiquids * (2f / 3f);

            ///////////////////////////
            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.ReferenceId + " no2comb  " + no2comb.TotalMolesGassesAndLiquids, SOGS.Logs.DEBUG);
            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.ReferenceId + " o2comb  " + o2comb.TotalMolesGassesAndLiquids, SOGS.Logs.DEBUG);

            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.ReferenceId + " remove  " + remove, SOGS.Logs.DEBUG);
            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.ReferenceId + " qualif  " + Mathf.Clamp(Convert.ToSingle(StaticAttributes.Combustonconfigs["FCW"].ToString()), 0, 1), SOGS.Logs.DEBUG);
            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.ReferenceId + " totaladd  " + (remove * Mathf.Clamp(Convert.ToSingle(StaticAttributes.Combustonconfigs["FCW"].ToString()), 0, 1)), SOGS.Logs.DEBUG);

            //// --- new add stream

            __instance.GasMixture.Steam.Add(new Mole(Chemistry.GasType.Steam, remove * Mathf.Clamp(Convert.ToSingle(StaticAttributes.Combustonconfigs["FCW"].ToString()), 0, 1), 0f)
            {
                Energy = __instance.CombustionEnergy
            });

            SOGS.log("AtmosphereCombustPatch :: Prefix --> " + __instance.ReferenceId + " Steam  " + totalno, SOGS.Logs.DEBUG);
        }


        [UsedImplicitly]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by mod loader")]
        private static void Posfix(Atmosphere __instance,
                                   ref bool ____inflamed,
                                   Dictionary<string, object> __state)
        {

        }
    }

    [HarmonyPatch]
    public class water
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Atmosphere), "StateChange")]
        private static void Posfix(Atmosphere __instance)
        {
            try {
                    float num = 0f;
                    float waterqty = __instance.GasMixture.Water.Quantity;
                    num += __instance.GasMixture.PollutedWater.Quantity;
                    num += __instance.GasMixture.LiquidNitrogen.Quantity;
                    num += __instance.GasMixture.LiquidOxygen.Quantity;
                    num += __instance.GasMixture.LiquidVolatiles.Quantity;
                    num += __instance.GasMixture.LiquidCarbonDioxide.Quantity;
                    num += __instance.GasMixture.LiquidPollutant.Quantity;
                    num += __instance.GasMixture.LiquidNitrousOxide.Quantity;
                    num += __instance.GasMixture.LiquidHydrogen.Quantity;

                    if (num > 0f)
                    {

                        SOGS.log("AtmosphereCondensationTypePatch :: Posfix " + __instance.ReferenceId + " ---> qty " + num, SOGS.Logs.DEBUG);
                        SOGS.log("AtmosphereCondensationTypePatch :: Posfix " + __instance.ReferenceId + " ---> water qty " + waterqty, SOGS.Logs.DEBUG);
                        if (waterqty > 0f) {
                            __instance.GasMixture.Water.Clear();
                            SOGS.log("AtmosphereCondensationTypePatch :: Posfix " + __instance.ReferenceId + " ---> af water qty " + __instance.GasMixture.Water.Quantity, SOGS.Logs.DEBUG);
                            __instance.GasMixture.PollutedWater.Add(new Mole(Chemistry.GasType.PollutedWater, waterqty, 0f));
                            SOGS.log("AtmosphereCondensationTypePatch :: Posfix " + __instance.ReferenceId + " ---> add water pol qty " + waterqty, SOGS.Logs.DEBUG);
                        }
                    }
            }catch(Exception ex)
            {
                SOGS.log("AtmosphereCondensationTypePatch :: Posfix error" + ex, SOGS.Logs.DEBUG);
            }
        }
    }

}