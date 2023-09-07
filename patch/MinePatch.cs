using Assets.Scripts;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using Assets.Scripts.Objects.Pipes;
using Assets.Scripts.Util;
using HarmonyLib;
using Reagents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.patch
{
    [HarmonyPatch]
    class MinePatch
    {
        static Dictionary<long, float> dps = new Dictionary<long, float>();
        static DirtyOre dro = (Prefab.AllPrefabs.First((Thing x) => x is DirtyOre) as DirtyOre);

        static Assets.Scripts.Localization2.GameString eMsgDeviceNoFindOre = Assets.Scripts.Localization2.GameString.Create(new string[]
                                {
                                    "DeviceNoFindOre",
                                    "Cannot find ores in this position"
                                });


        [HarmonyPostfix]
        [HarmonyPatch(typeof(DeepMiner), "Start")]
        static void DPSStartPatch(DeepMiner __instance)
        {

            SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " Strat ", SOGS.Logs.DEBUG);

            if (dps.ContainsKey(__instance.ReferenceId))
            {
                SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " remove instance dps ", SOGS.Logs.DEBUG);
                dps.Remove(__instance.ReferenceId);
            }

            typeof(DeepMiner).GetField("_oreAmountPerSpawnMin", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, 0);
            typeof(DeepMiner).GetField("_oreAmountPerSpawnMin", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, 0);

            Vector3 vec = __instance.Position.GridCenter(1f, 0f);
            Vector3 mvec = new Vector3(StaticAttributes.mineConfigsFloat["rangeDP"], Math.Abs(vec.y - WorldManager.BedrockLevel - 1), StaticAttributes.mineConfigsFloat["rangeDP"]) / 2;
            Vector3 zero = Vector3.zero;
            DeepMiner __instance1 = __instance;

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                try
                {
                    Color32[] color = (Color32[])typeof(PortableGPR).GetField("OreColors", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

                    SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " created parameter and vectors ", SOGS.Logs.INFO);

                    SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " position " + vec, SOGS.Logs.INFO);


                    SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " tamanho y " + Math.Abs(vec.y - WorldManager.BedrockLevel - 1), SOGS.Logs.INFO);
                    SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " position z " + vec.z, SOGS.Logs.INFO);
                    SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " position y " + vec.y, SOGS.Logs.INFO);
                    SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " position x " + vec.x, SOGS.Logs.INFO);
                    SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " BedrockLevel level " + WorldManager.BedrockLevel, SOGS.Logs.INFO);

                    int n = 0;
                    for (int i = 1; i < StaticAttributes.mineConfigsFloat["rangeDP"] - 1; i++)
                    {
                        SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " i " + i, SOGS.Logs.DEBUG);

                        for (int j = 1; j < Math.Abs(vec.y - WorldManager.BedrockLevel - 1); j++)
                        {
                            SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " j " + j, SOGS.Logs.DEBUG);

                            for (int k = 1; k < StaticAttributes.mineConfigsFloat["rangeDP"] - 1; k++)
                            {
                                SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " k " + k, SOGS.Logs.DEBUG);

                                zero.x = (float)k;
                                zero.y = -(float)j;
                                zero.z = (float)i;
                                Color32 icolor = color[Mathf.Min((int)ChunkController.World.GetVoxelWorld(vec + zero).Type, color.Length - 1)];

                                if (icolor != Color.black)
                                {
                                    SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " not is black ", SOGS.Logs.DEBUG);
                                    n++;
                                }

                                //typeof(ChunkController).GetMethod("DestroyChunk", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { ChunkController.GetWorldChunkCenter(vec - zero) });
                                SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " passou ", SOGS.Logs.DEBUG);

                            }
                        }
                    }

                    if (n < 1)
                    {
                        DeepMiner dp = Thing.Find<DeepMiner>(__instance.ReferenceId);
                        if (dp != null)
                        {
                            SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " update to error ", SOGS.Logs.INFO);
                            dp.Error = 9999;
                        }
                    }
                    SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " nmines " + n, SOGS.Logs.INFO);

                    dps.Add(__instance.ReferenceId, n);
                }
                catch (Exception e)
                {
                    SOGS.log("MinePatch :: DPSStartPatch --> Tread1 error" + __instance.ReferenceId + " ex: " + e.StackTrace, SOGS.Logs.ERROR);
                }
            }).Start();

        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(DeepMiner), "SpawnOre")]
        static bool DPSpawnOrePatch(ref DeepMiner __instance)
        {

            DirtyOre dirtyOre = Thing.Create<DirtyOre>(dro, __instance.ExportSlot.Location);
            SOGS.log("MinePatch :: DPSpawnOrePatch --> " + __instance.ReferenceId, SOGS.Logs.DEBUG);
            float qty = 0;
            if (dps.ContainsKey(__instance.ReferenceId) && dps[__instance.ReferenceId] >= 1f)
            {
                qty = UnityEngine.Random.Range(1, 50 * Mathf.Clamp(dps[__instance.ReferenceId] / 100f, 0.1f, 0.3f));
                SOGS.log("MinePatch :: DPSpawnOrePatch --> " + __instance.ReferenceId + " qty spawn " + qty, SOGS.Logs.DEBUG);
            }
            else
            {
                SOGS.log("MinePatch :: DPSpawnOrePatch --> " + __instance.ReferenceId + " qty spawn " + 0, SOGS.Logs.DEBUG);
            }

            if (qty > 0)
            {
                dirtyOre.SetQuantity(qty);
                dirtyOre.ParentSlot = null;
                SOGS.log("MinePatch :: DPSpawnOrePatch --> " + __instance.ReferenceId + " spawn " + dirtyOre.Quantity, SOGS.Logs.DEBUG);
                OnServer.MoveToSlot(dirtyOre, __instance.ExportSlot);
            }
            else
            {
                SOGS.log("MinePatch :: DPSpawnOrePatch --> " + __instance.ReferenceId + " update to error ", SOGS.Logs.INFO);
                __instance.Error = 9999;
            }

            typeof(DeepMiner).GetField("_timeSinceLastOreSpawn", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, GameManager.GameTime);
            SOGS.log("MinePatch :: DPSpawnOrePatch --> " + __instance.ReferenceId + " _timeSinceLastOreSpawn " + typeof(DeepMiner).GetField("_timeSinceLastOreSpawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance), SOGS.Logs.DEBUG);

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DeepMiner), "IsSpawnTime")]
        static void IsSpawnTimePatch(ref DeepMiner __instance)
        {
            SOGS.log("MinePatch :: IsSpawnTimePatch --> " + __instance.ReferenceId + " update to error " + __instance.Error, SOGS.Logs.INFO);

            if (dps.ContainsKey(__instance.ReferenceId) && dps[__instance.ReferenceId] < 1)
            {
                SOGS.log("MinePatch :: IsSpawnTimePatch --> " + __instance.ReferenceId + " update to error " + __instance.Error, SOGS.Logs.INFO);
                __instance.Error = 9999;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DirtyOre), "CentrifugeProcessUnit")]
        static void CentrifugeProcessUnitPatch(ref DirtyOre __instance, ref ReagentMixture __result)
        {

            foreach (string item in StaticAttributes.mineOreReagentId)
            {
                double qty = (double)((typeof(ReagentMixture).GetField(item).GetValue(__result) as Reagent)?.Quantity);
                if (qty > 0.0)
                {
                    SOGS.log("MinePatch :: CentrifugeProcessUnitPatch --> " + __result.ToString() + " qty " + qty, SOGS.Logs.DEBUG);
                    if (StaticAttributes.CentrifugeDirtyOreMetod == 1)
                    {
                        __result -= __result;
                        break;
                    }
                    if (StaticAttributes.CentrifugeDirtyOreMetod == 2)
                    {
                        SOGS.log("MinePatch :: CentrifugeProcessUnitPatch --> " + __result.ToString() + " remove ", SOGS.Logs.DEBUG);

                        __result = __result * float.Parse(StaticAttributes.mineConfigsFloat["RetCentPorc"].ToString());
                    }
                }
            }
        }
    }
}
