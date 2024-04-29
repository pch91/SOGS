using Assets.Scripts;
using Assets.Scripts.Localization2;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.patch
{
    [HarmonyPatch]
    class BackpackPatch
    {

        static Assets.Scripts.Localization2.GameString eMsgSogsStorageNSupport = Assets.Scripts.Localization2.GameString.Create(new string[]
                                                    {
                                                        "sogsstoragensupport",
                                                        "It storage does not support this item"
                                                    });

        [HarmonyPatch(typeof(DynamicSpawnData), "GetSlotIn")]
        [HarmonyPostfix]
        private static void Spawnpatchnew(ref DynamicSpawnData __instance, Assets.Scripts.Objects.Thing parent, ref Slot __result)
        {


            if ((parent is Backpack || parent is Jetpack) && StaticAttributes.beltPosition == 0)
            {
                SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId + " parent SlotId " + __instance.SlotId + " "+ __instance.SlotIndex, SOGS.Logs.DEBUG);

                if (__instance.SlotIndex == parent.Slots.Count - 1 )
                {
                    for (int i = 0; i < parent.Slots.Count; i++)
                    {
                        SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId + " Slots  Type" + parent.Slots[i].Type, SOGS.Logs.DEBUG);
                        SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId + " SourcePrefab  Type" + __instance.SlotClass, SOGS.Logs.DEBUG);

                        if (parent.Slots[i].Get() == null && (parent.Slots[i].Type == __instance.SlotClass || parent.Slots[i].Type == Slot.Class.None))
                        {
                            __result = parent.GetSlot(i);
                            SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId + " i " + i, SOGS.Logs.DEBUG);
                        }
                    }

                }

                SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId + " parent " + parent.PrefabName, SOGS.Logs.DEBUG);
                SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId + " parent if " + parent.GetType(), SOGS.Logs.DEBUG);
                SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId + " PrefabName " + __instance.PrefabName, SOGS.Logs.DEBUG);
                SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId + " Name " + __instance.Name, SOGS.Logs.DEBUG);
                SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId + " DynamicThings " + __instance.DynamicThings.ToString(), SOGS.Logs.DEBUG);
                SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId + " Items " + __instance.Items.ToString(), SOGS.Logs.DEBUG);
                SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId + " Slots  Type " + __instance.SlotClass, SOGS.Logs.DEBUG);

                if (__instance.SlotClass != Slot.Class.None)
                {

                    Slot slot = parent.GetNextFreeSlot(__instance.SlotClass);

                    if (slot != null)
                    {
                        SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId + " Slots " + slot.StringHash, SOGS.Logs.DEBUG);
                        __result = slot;
                        return;
                    }

                }
                else
                {
                    try
                    {
                        Thing t = Prefab.Find(__instance.PrefabName);
                        SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId + " Slots type d " + ((DynamicThing)t).SlotType, SOGS.Logs.DEBUG);

                        if (t is DynamicThing)
                        {
                            Slot slot = parent.GetNextFreeSlot(((DynamicThing)t).SlotType);
                            if (slot != null)
                            {
                                __result = slot;
                                return;
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId + " erro tratado " + e, SOGS.Logs.DEBUG);
                    }
                }


                SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId + " SlotType " + Slot.Class.Belt, SOGS.Logs.DEBUG);

                if (__instance.SlotClass == Slot.Class.Belt && __instance.SlotIndex != parent.Slots.Count - 1)
                {
                    __result = parent.GetSlot(parent.Slots.Count - 1);
                    SOGS.log("BackpackPatch :: Spawnpatchnew --> " + __instance.SlotId, SOGS.Logs.DEBUG);

                }
            }
        }


        [HarmonyPatch(typeof(Assets.Scripts.Objects.Thing), "CanEnter")]
        [HarmonyPostfix]
        private static void CanEnterpatch(Assets.Scripts.Objects.Thing __instance, Slot destinationSlot, ref CanEnterResult __result)
        {

            if (destinationSlot.Parent is DynamicThing && __instance is DynamicThing)
            {

                SOGS.log("BackpackPatch :: CanEnterpatch --> " + __instance.ReferenceId + " destinationSlot " + destinationSlot.Parent.CustomName, SOGS.Logs.DEBUG);
                SOGS.log("BackpackPatch :: CanEnterpatch --> " + __instance.ReferenceId + " destinationSlot type " + destinationSlot.Parent.GetType().Name, SOGS.Logs.DEBUG);
                SOGS.log("BackpackPatch :: CanEnterpatch --> " + __instance.ReferenceId + " thing type " + __instance.GetType().Name, SOGS.Logs.DEBUG);
                SOGS.log("BackpackPatch :: CanEnterpatch --> " + __instance.ReferenceId + " thing is Backpack " + (__instance is Backpack || __instance is Backpack), SOGS.Logs.DEBUG);
                SOGS.log("BackpackPatch :: CanEnterpatch --> " + __instance.ReferenceId + " destinationSlot.Parent is Backpack " + (destinationSlot.Parent is Backpack || destinationSlot.Parent is Jetpack), SOGS.Logs.DEBUG);
                SOGS.log("BackpackPatch :: CanEnterpatch --> " + __instance.ReferenceId + " destinationSlot.Parent SlotType " + (destinationSlot.Parent is DynamicThing ? (destinationSlot.Parent as DynamicThing).SlotType : "not is DynamicThing"), SOGS.Logs.DEBUG);


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
                            || destinationSlot.Parent is CardboardBox
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
                    ||
                    (
                        instanceslottype == Slot.Class.Uniform
                        &&
                        destinationslottype == Slot.Class.Uniform
                    )
                    || 
                    (
                        __instance is CardboardBox
                        && (
                            destinationslottype == Slot.Class.Back
                            || destinationslottype == Slot.Class.Belt
                            || destinationslottype == Slot.Class.Uniform
                            || destinationSlot.Parent is CardboardBox
                        )
                    )
                )
                {
                    __result = CanEnterResult.Fail(eMsgSogsStorageNSupport);
                }
            }

        }

        [HarmonyPatch(typeof(Assets.Scripts.Objects.Thing), "Awake")]
        [HarmonyPrefix]
        private static void Awakepatch(Assets.Scripts.Objects.Thing __instance)
        {
            if (__instance is Backpack || __instance is Jetpack)
            {
                SOGS.log("BackpackPatch :: Awakepatch --> " + __instance.ReferenceId + " cratete backpack ", SOGS.Logs.DEBUG);
                if (StaticAttributes.beltPosition > 0 && StaticAttributes.beltPosition < 9)
                {
                    __instance.Slots[StaticAttributes.beltPosition - 1].StringKey = "Belt";
                    __instance.Slots[StaticAttributes.beltPosition - 1].StringHash = Animator.StringToHash(__instance.Slots[StaticAttributes.beltPosition - 1].StringKey);
                    __instance.Slots[StaticAttributes.beltPosition - 1].Type = Slot.Class.Belt;
                }
                else
                {
                    __instance.Slots.Last().StringKey = "Belt";
                    __instance.Slots.Last().StringHash = Animator.StringToHash(__instance.Slots.Last().StringKey);
                    __instance.Slots.Last().Type = Slot.Class.Belt;
                }
            }
        }

    }
}