using Assets.Scripts.Atmospherics;
using HarmonyLib;
using JetBrains.Annotations;
using sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.patch;
using System;
using UnityEngine;

namespace sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.Scripts
{
    [HarmonyPatch(typeof(Atmosphere), "ReactWithCell")]
    public class GeothermalAtmospherePatch
    {
        const float DamageRange = 10;
        const float MaxPowerPerVolume = 100;

        [UsedImplicitly]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by mod loader")]
        private static void Prefix(Atmosphere __instance)
        {
            try
            {
                if (bool.Parse(StaticAttributes.configs["EnabledT"].ToString())) {
                    if (!WorldManager.HasLava)
                        // le shrug i guess you do not have le lava 
                        return;

                    var distanceFromLavaSigned = __instance.WorldPosition.y - WorldManager.LavaLevel;


                    if (distanceFromLavaSigned < DamageRange)
                    {
                        var maxPower = AtmosphericsManager.Instance.TickSpeedMs * MaxPowerPerVolume * __instance.Volume;
                        var power = Mathf.Clamp01(distanceFromLavaSigned / DamageRange) * maxPower;
                        __instance.GasMixture.AddEnergy(power); // no idea how much heat you should get.
                    } 
                }
            }catch(Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
