using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.Networking.NetworkUpdateType;

namespace sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.patch
{
    [HarmonyPatch]
    class BackpackPatch
    {

        [HarmonyPatch(typeof(Assets.Scripts.Objects.Thing), "CanEnter")]
        [HarmonyPostfix]
        private static void CanEnterpatch(Assets.Scripts.Objects.Thing __instance, Slot destinationSlot, ref bool __result)
        {
            SOGS.log("BackpackPatch :: CanEnterpatch --> " + __instance.ReferenceId + " destinationSlot " + destinationSlot.Parent.CustomName, SOGS.Logs.DEBUG);
            SOGS.log("BackpackPatch :: CanEnterpatch --> " + __instance.ReferenceId + " destinationSlot type " + destinationSlot.Parent.GetType().Name, SOGS.Logs.DEBUG);
            SOGS.log("BackpackPatch :: CanEnterpatch --> " + __instance.ReferenceId + " thing type " + __instance.GetType().Name, SOGS.Logs.DEBUG);
            SOGS.log("BackpackPatch :: CanEnterpatch --> " + __instance.ReferenceId + " thing is Backpack " + (__instance is Backpack), SOGS.Logs.DEBUG);
            SOGS.log("BackpackPatch :: CanEnterpatch --> " + __instance.ReferenceId + " destinationSlot.Parent is Backpack " + (destinationSlot.Parent is Backpack), SOGS.Logs.DEBUG);
            SOGS.log("BackpackPatch :: CanEnterpatch --> " + __instance.ReferenceId + " destinationSlot.Parent SlotType " + (destinationSlot.Parent is DynamicThing ? (destinationSlot.Parent as DynamicThing).SlotType : "not is DynamicThing"), SOGS.Logs.DEBUG);


            if (destinationSlot.Parent is DynamicThing && __instance is DynamicThing){
                Slot.Class destinationslottype = (destinationSlot.Parent as DynamicThing).SlotType;
                Slot.Class instanceslottype = (__instance as DynamicThing).SlotType;

                if (
                    (
                        instanceslottype == Slot.Class.Back
                        && 
                        (
                            destinationslottype == Slot.Class.Back
                            || destinationslottype == Slot.Class.Belt
                            || destinationslottype == Slot.Class.Uniform
                        )
                    )
                    ||
                    (
                        instanceslottype == Slot.Class.Belt
                        &&
                        (
                            destinationslottype == Slot.Class.Belt
                            || destinationslottype == Slot.Class.Uniform
                            || (destinationslottype == Slot.Class.Back && destinationSlot.Type == Slot.Class.None)
                        )
                    )
                    
                ){
                    __result = false;
                }
            }

        }

        [HarmonyPatch(typeof(Assets.Scripts.Objects.Thing), "Awake")]
        [HarmonyPrefix]
        private static void CanEnterpatch(Assets.Scripts.Objects.Thing __instance)
        {
            if (__instance is Backpack) {
                SOGS.log("BackpackPatch :: CanEnterpatch --> " + __instance.ReferenceId + " cratete backpack ", SOGS.Logs.DEBUG);
                __instance.Slots.Last().StringKey = "Belt";
                __instance.Slots.Last().StringHash = Animator.StringToHash(__instance.Slots.Last().StringKey);
                __instance.Slots.Last().Type = Slot.Class.Belt;
            }
        }

    }
}