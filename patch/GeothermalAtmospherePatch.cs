using Assets.Scripts.Atmospherics;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Entities;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Diagnostics;
using UnityEngine;

namespace sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.Scripts
{
    [HarmonyPatch(typeof(Atmosphere), "ReactWithCell")]
    public class GeothermalAtmospherePatch
    {
        const float DamageRange = 10;
        const float MaxPowerPerVolume = 100;

        [UsedImplicitly]
        private static void Prefix(Atmosphere __instance)
        {
            try
            {
                if (!WorldManager.HasLava)
                    // le shrug i guess you do not have le lava 
                    return;

                SOGS.log("GeothermalAtmospherePatch2 :: ConvectionPosfix 01--> "
                    + __instance.ReferenceId,
                    SOGS.Logs.DEBUG);
                //y= 20000./(1+0.1.^(10000./x))-0.5
                var maxPower = MathF.Min(AtmosphericsManager.Instance.TickSpeedSeconds * MaxPowerPerVolume * __instance.Volume, 20000f);

                var distanceFromLavaSigned = __instance.WorldPosition.y - WorldManager.LavaLevel;
                if (__instance.Thing != null)
                {
                    if (__instance.Thing.GetPrefabName() != null && __instance.Thing.GetPrefabName().Contains("ItemGasCanister") && __instance.Thing is DynamicThing)
                    {
                        SOGS.log("GeothermalAtmospherePatch2 :: ConvectionPosfix 011--> " + __instance.ReferenceId, SOGS.Logs.DEBUG);

                        DynamicThing canister = __instance.Thing as DynamicThing;
                        SOGS.log("GeothermalAtmospherePatch2 :: ConvectionPosfix 012--> " + __instance.ReferenceId, SOGS.Logs.DEBUG);

                        Thing father = canister != null ? Thing.Find<Thing>(canister.ParentReferenceId) : null;
                        SOGS.log("GeothermalAtmospherePatch2 :: ConvectionPosfix 013--> " + __instance.ReferenceId, SOGS.Logs.DEBUG);

                        if (father != null
                            && father.PrefabName != null
                            && (father.PrefabName.Equals("ItemHardSuit")
                            || father.PrefabName.Equals("ItemHardJetpack")
                            || father.PrefabName.Equals("ItemHardsuitHelmet")))
                        {
                            return;
                        }
                    }
                    else if (__instance.Thing.PrefabName.Equals("ItemHardsuitHelmet") || __instance.Thing.PrefabName.Equals("ItemHardSuit"))
                    {
                        maxPower = (1f / 1f + (Mathf.Pow(0.1f, (10000f / maxPower)))) - 0.5f;
                        //return;
                    }

                    SOGS.log("GeothermalAtmospherePatch2 :: ConvectionPosfix 02--> " + __instance.ReferenceId, SOGS.Logs.DEBUG);

                    if (__instance.Thing.PrefabName.Equals("OrganLungs"))
                    {
                        SOGS.log("GeothermalAtmospherePatch2 :: ConvectionPosfix thing aqui atm--> " + __instance.ReferenceId + " " + __instance.Thing.HasAtmosphere + " pulou " + __instance.Thing.InternalAtmosphere?.Mode + " " + __instance.Thing.InternalAtmosphere.ReferenceId, SOGS.Logs.DEBUG);
                        SOGS.log("GeothermalAtmospherePatch2 :: ConvectionPosfix thing aqui atm--> " + __instance.ReferenceId + " " + __instance.Thing.RootParent, SOGS.Logs.DEBUG);

                        Human human = __instance.Thing.RootParent as Human;
                        SOGS.log("GeothermalAtmospherePatch2 :: ConvectionPosfix 021--> " + __instance.ReferenceId, SOGS.Logs.DEBUG);

                        if (human.HelmetSlot.Get() != null
                            && !human.HelmetSlot.Get().IsOpen
                            && human.HelmetSlot.Get().PrefabName.Equals("ItemHardsuitHelmet")
                            && human.SuitSlot.Get() != null
                            && human.SuitSlot.Get().PrefabName.Equals("ItemHardSuit")
                            && human.SuitSlot.Get().Powered
                            && human.UniformSlot.Get() != null)
                        {
                            return;
                        }
                    }
                }
                SOGS.log("GeothermalAtmospherePatch2 :: ConvectionPosfix 03--> " + __instance.ReferenceId, SOGS.Logs.DEBUG);

                /*
                if (__instance.Thing != null)
                {
                    if(__instance.Thing.HasAtmosphere)
                        SOGS.log("GeothermalAtmospherePatch2 :: ConvectionPosfix thing aqui atm--> " + __instance.ReferenceId + " " + __instance.Thing.HasAtmosphere + " pulou " + __instance.Thing.InternalAtmosphere?.Mode+" "+ __instance.Thing.InternalAtmosphere.ReferenceId, SOGS.Logs.DEBUG);

                    SOGS.log("GeothermalAtmospherePatch2 :: ConvectionPosfix thing aqui--> " + __instance.ReferenceId + " " + __instance.DisplayName + " pulou "+ __instance.Thing.PrefabName, SOGS.Logs.DEBUG);
                }*/


                if (distanceFromLavaSigned < DamageRange)
                {
                    var power = Mathf.Clamp01(distanceFromLavaSigned / DamageRange) * maxPower;
                    SOGS.log("GeothermalAtmospherePatch2 :: ConvectionPosfix n passou 3--> " + __instance.ReferenceId + " " + __instance.DisplayName + " pulou " + __instance.Mode, SOGS.Logs.DEBUG);
                    __instance.GasMixture.AddEnergy(power); // no idea how much heat you should get.
                }
            }
            catch (Exception e)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(e, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                SOGS.log("GeothermalAtmospherePatch2 :: ConvectionPosfix --> ex " + __instance.Thing.ReferenceId + " " + __instance.Thing.DisplayName + "" + frame + " :: " + line, SOGS.Logs.DEBUG);

                UnityEngine.Debug.LogException(e);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
