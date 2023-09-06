using Assets.Scripts;
using Assets.Scripts.Localization2;
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
using static Assets.Scripts.Objects.Thing;

namespace sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.patch
{
    [HarmonyPatch]
    class MinePatch
    {
        static Dictionary<long, float> dps = new Dictionary<long, float>();
        static DirtyOre dro = (Prefab.AllPrefabs.First((Thing x) => x is DirtyOre) as DirtyOre);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DeepMiner), "Start")]
        static void DPSStartPatch(ref DeepMiner __instance)
        {

            SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " Strat ", SOGS.Logs.DEBUG);

            if (dps.ContainsKey(__instance.ReferenceId))
            {
                SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " remove instance dps ", SOGS.Logs.DEBUG);
                dps.Remove(__instance.ReferenceId);
            }

            typeof(DeepMiner).GetField("_oreAmountPerSpawnMin", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, 0);
            typeof(DeepMiner).GetField("_oreAmountPerSpawnMin", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, 0);


            SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " Strat defor dps ", SOGS.Logs.DEBUG);

            Vector3 vec = __instance.Position.GridCenter(1f, 0f);
            SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " vec " + vec, SOGS.Logs.DEBUG);

            Vector3 mvec = new Vector3(StaticAttributes.mineConfigsFloat["rangeDP"], StaticAttributes.mineConfigsFloat["rangeDP"], StaticAttributes.mineConfigsFloat["rangeDP"]) / 2;
            SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " mvec " + mvec, SOGS.Logs.DEBUG);

            Vector3 zero = Vector3.zero;
            SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " zero " + zero, SOGS.Logs.DEBUG);
           // DeepMiner __instance1 = __instance;

           // new Thread(() =>
           // {
             //   Thread.CurrentThread.IsBackground = true;
                try
                {
                    Color32[] color = (Color32[])typeof(PortableGPR).GetField("OreColors", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

                    SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " created parameter and vectors ", SOGS.Logs.DEBUG);
                    int n = 0;
                    for (int i = 1; i < StaticAttributes.mineConfigsFloat["rangeDP"] - 1; i++)
                    {
                        for (int j = 1; j < StaticAttributes.mineConfigsFloat["rangeDP"] - 1; j++)
                        {
                            for (int k = 1; k < StaticAttributes.mineConfigsFloat["rangeDP"] - 1; k++)
                            {
                                zero.x = (float)k;
                                zero.y = (float)j;
                                zero.z = (float)i;
                                SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " color "+ color[Mathf.Min((int)ChunkController.World.GetVoxelWorld(vec + zero - mvec).Type, color.Length - 1)], SOGS.Logs.DEBUG);

                                if (color[Mathf.Min((int)ChunkController.World.GetVoxelWorld(vec + zero - mvec).Type, color.Length - 1)] != Color.black)
                                {
                                    SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " not is black ", SOGS.Logs.DEBUG);
                                    n++;
                                }
                            }
                        }
                    }

                    if (n < 1)
                    {
                        __instance.Error = 9999;
                    }
                    SOGS.log("MinePatch :: DPSStartPatch --> " + __instance.ReferenceId + " nmines " + n, SOGS.Logs.DEBUG);

                    dps.Add(__instance.ReferenceId, n);
                }
                catch (Exception e)
                {
                    SOGS.log("MinePatch :: DPSStartPatch --> Tread1 error" + __instance.ReferenceId + " ex: " + e.StackTrace, SOGS.Logs.ERROR);
                }
            //}).Start();

        }


        [HarmonyPrefix]
        [HarmonyDebug]
        [HarmonyPatch(typeof(DeepMiner), "SpawnOre")]
        static bool DPSpawnOrePatch(DeepMiner __instance)
        {

            DirtyOre dirtyOre = Thing.Create<DirtyOre>(dro, __instance.ExportSlot.Location);
            SOGS.log("MinePatch :: DPSpawnOrePatch --> " + __instance.ReferenceId, SOGS.Logs.DEBUG);
            float qty = 0;
            if (dps.ContainsKey(__instance.ReferenceId) && dps[__instance.ReferenceId] >= 1f) {
                qty = UnityEngine.Random.Range(1, 50* Mathf.Clamp(dps[__instance.ReferenceId]/100f,0.1f,0.3f));
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
                __instance.Error = 9999;
            }

            //FieldInfo type = typeof(DeepMiner).GetField("_timeSinceLastOreSpawn", BindingFlags.NonPublic | BindingFlags.Instance);
            //SOGS.log("MinePatch :: DPSpawnOrePatch --> " + __instance.ReferenceId + " _timeSinceLastOreSpawn type" + type, SOGS.Logs.INFO);
            typeof(DeepMiner).GetField("_timeSinceLastOreSpawn", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, GameManager.GameTime);
            SOGS.log("MinePatch :: DPSpawnOrePatch --> " + __instance.ReferenceId + " _timeSinceLastOreSpawn " + typeof(DeepMiner).GetField("_timeSinceLastOreSpawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance), SOGS.Logs.DEBUG);

            return false;
        }

        [HarmonyPrefix]
        [HarmonyDebug]
        [HarmonyPatch(typeof(DeepMiner), "OnServerExportTick")]
        static void OnServerExportTickPatch(ref DeepMiner __instance)
        {
            if(dps.ContainsKey(__instance.ReferenceId) && dps[__instance.ReferenceId]<1)
            {
                __instance.Error = 9999;
            }
        }


        [HarmonyPostfix]
        [HarmonyDebug]
        [HarmonyPatch(typeof(DirtyOre), "CentrifugeProcessUnit")]
        static void CentrifugeProcessUnitPatch(ref DirtyOre __instance,ref ReagentMixture __result)
        {

            foreach(string item in StaticAttributes.mineOreReagentId) {
                double qty = (double)((typeof(ReagentMixture).GetField(item).GetValue(__result) as Reagent)?.Quantity);
                if (qty > 0.0)
                {
                    SOGS.log("MinePatch :: CentrifugeProcessUnitPatch --> " + __result.ToString() + " qty "+ qty, SOGS.Logs.INFO);
                    if (StaticAttributes.CentrifugeDirtyOreMetod == 1)
                    {
                        __result -= __result;
                        break;
                    }
                    if (StaticAttributes.CentrifugeDirtyOreMetod == 2)
                    {
                        SOGS.log("MinePatch :: CentrifugeProcessUnitPatch --> " + __result.ToString() + " remove ", SOGS.Logs.INFO);

                        __result = __result * float.Parse(StaticAttributes.mineConfigsFloat["RetCentPorc"].ToString());
                    }

                    /*SOGS.log("MinePatch :: CentrifugeProcessUnitPatch --> " + __result.ToString() + " de qty " + (100 * float.Parse(StaticAttributes.mineConfigsFloat["RetCentPorc"].ToString())), SOGS.Logs.INFO);
                    if (StaticAttributes.CentrifugeDirtyOreMetod == 2 && __instance.Quantity >=  (100 * float.Parse(StaticAttributes.mineConfigsFloat["RetCentPorc"].ToString())) )
                    {
                        SOGS.log("MinePatch :: CentrifugeProcessUnitPatch --> " + __result.ToString() + " remove ", SOGS.Logs.INFO);

                        __instance.Quantity -= (int)(100 * float.Parse(StaticAttributes.mineConfigsFloat["RetCentPorc"].ToString()));
                    }
                    else
                    {
                        __result -= __result;
                        break;
                    }*/
                }
            }
        }

        /*[HarmonyPrefix]
        [HarmonyDebug]
        [HarmonyPatch(typeof(DeepMiner), "InteractWith")]
        static void InteractWithPatch(ref DeepMiner __instance,ref Thing.DelayedActionInstance __result, Interactable interactable, Interaction interaction, bool doAction = true)
        {
            if (__instance.Error == 9999)
            {
                Thing.DelayedActionInstance delayedActionInstance = __instance.InteractWith(interactable, interaction, doAction);

                __result = delayedActionInstance.Fail(
                                Assets.Scripts.Localization2.GameString.Create(new string[]
                                {
                                    "NoMinerals",
                                    "Not find minerals in area {LOCAL:Thing}",
                                    "Thing"
                                }),
                                (typeof(DeepMiner).GetField("ThingInTheWay", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance)as Thing).ToTooltip());
            }
        }*/

        /*
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(DeepMiner), "SpawnOre")]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);
            //SOGS.log("MinePatch :: Transpiler --> start", SOGS.Logs.INFO);
            int[] p = new int[] { 0, 0 }; 
            for (int i = 0; i < code.Count - 1; i++) // -1 since we will be checking i + 1
            {
                if (code[i].opcode == OpCodes.Ldarg_0)
                {
                    p[0] = i;
                }
                if (code[i].opcode == OpCodes.Ret)
                {
                    p[0] = i; break;
                }
            }

            foreach (int i in p)
            {
                //SOGS.log("MinePatch :: Transpiler --> removendo", SOGS.Logs.INFO);
                code.RemoveAt(i);
            }

            return code;
        }*/
    }
}
