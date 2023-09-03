using Assets.Scripts.Atmospherics;
using HarmonyLib;
using JetBrains.Annotations;
using sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.patch;
using System;
using System.Reflection;
using UnityEngine;

namespace sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.Scripts
{

    [HarmonyPatch(typeof(Atmosphere), "Combust")]
    public class AtmosphereCombustPatch
    {
        [UsedImplicitly]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by mod loader")]
        private static bool Prefix(Atmosphere __instance,
                                   ref bool ____inflamed)
        {
            try{
                float molesOfH2 = Mathf.Min(__instance.GasMixture.TotalFuel, __instance.GasMixture.Oxygen.Quantity * 2f);
                float molesOfO2 = molesOfH2 / 2f;
                float burnPercentage = 0.95f;
                float removedMolesH2 = molesOfH2 * burnPercentage;
                float removedMolesO2 = molesOfO2 * burnPercentage;
                Mole burnt = __instance.GasMixture.Volatiles.Remove(removedMolesH2);
                burnt.Add(__instance.GasMixture.Oxygen.Remove(removedMolesO2));
                /* --------------------------old----------------------------------
                // what is matterstate all?
                float magicOutputScaler = productType == AtmosphereHelper.MatterState.All ? 0.5f : 1f;
                // we only burn h2
                Water water = new Water(magicOutputScaler * removedMolesH2)
                {
                    Energy = magicOutputScaler * burnt.Energy
                };
                --------------------------------------------------------------------*/

                __instance.GasMixture.Water.Add(new Mole(Chemistry.GasType.Water, burnt.Quantity * 2f / 3f * float.Parse(StaticAttributes.Combustonconfigs["FCW"].ToString()), 0f)
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
            }
            return false;
        }
    }
}
