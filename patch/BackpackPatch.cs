using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using HarmonyLib;
using SimpleSpritePacker;
using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.Networking.NetworkUpdateType;

namespace sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.patch
{
    [HarmonyPatch]
    class BackpackPatch
    {
        [HarmonyPatch(typeof(WorldManager.InventoryData), "Spawn")]
        [HarmonyPrefix]
        private static void Spawnpatch(ref WorldManager.InventoryData __instance, Assets.Scripts.Objects.Thing parentEntity, ColorSwatch colorSwatch)
        {

            if (parentEntity is Backpack && StaticAttributes.beltPosition == 0) {
                if (__instance.SlotId == parentEntity.Slots.Count-1)
                {
                    for ( int i = 0; i < parentEntity.Slots.Count; i++ )
                    {
                        if (parentEntity.Slots[i].Occupant == null && (parentEntity.Slots[i].Type == __instance.SourcePrefab.SlotType || parentEntity.Slots[i].Type == Slot.Class.None)) {
                            //OnServer.Create<DynamicThing>(__instance.SourcePrefab, parentEntity.Slots.Last());
                            __instance.SlotId = i;
                        }
                    }

                }

                if (__instance.SourcePrefab.SlotType == Slot.Class.Belt && __instance.SlotId != parentEntity.Slots.Count-1)
                {
                    //OnServer.Create<DynamicThing>(__instance.SourcePrefab, parentEntity.Slots.Last());
                    __instance.SlotId = parentEntity.Slots.Count - 1;
                }
            }
        }


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
        private static void Awakepatch(Assets.Scripts.Objects.Thing __instance)
        {
            if (__instance is Backpack) {
                SOGS.log("BackpackPatch :: CanEnterpatch --> " + __instance.ReferenceId + " cratete backpack ", SOGS.Logs.DEBUG);
                if (StaticAttributes.beltPosition > 0 && StaticAttributes.beltPosition < 9){ 
                    __instance.Slots[StaticAttributes.beltPosition-1].StringKey = "Belt";
                    __instance.Slots[StaticAttributes.beltPosition-1].StringHash = Animator.StringToHash(__instance.Slots[StaticAttributes.beltPosition-1].StringKey);
                    __instance.Slots[StaticAttributes.beltPosition - 1].Type = Slot.Class.Belt;
                } else {
                    __instance.Slots.Last().StringKey = "Belt";
                    __instance.Slots.Last().StringHash = Animator.StringToHash(__instance.Slots.Last().StringKey);
                    __instance.Slots.Last().Type = Slot.Class.Belt;
                }
            }
        }

    }
}