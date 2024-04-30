using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.patch;
using sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace sogs_standing_on_giants_shoulders_a_collection_of_physics_improv
{
    [BepInPlugin("SOGS", "sogs_standing_on_giants_shoulders_a_collection_of_physics_improv", "0.0.0.1")]
    public class SOGS : BaseUnityPlugin
    {
        public static SOGS Instance;

        private static string loglevel = "INFO";

        public enum Logs
        {
            DEBUG = 1,
            ERROR = 2,
            INFO = 0,
        }

        public static void log(string line, Logs level)
        {
            //Debug.Log((int)Enum.Parse(typeof(Logs), loglevel));

            if ((int)Enum.Parse(typeof(Logs), loglevel) - (int)level >= 0)
            {
                Debug.Log("[" + level + "    :   SOGS] " + line);
            }
        }


        public static Dictionary<string, object> fmainconfig = new Dictionary<string, object>();
        public static Dictionary<string, object> fconfigEvents = new Dictionary<string, object>();
        private Dictionary<string, object> mainconfigs = new Dictionary<string, object>();


        private void Awake()
        {
            try
            {
                log("Start - SOGS", Logs.INFO);
                Instance = this;
                //Harmony.DEBUG = true;
                Handleconfig();
                if (bool.Parse(StaticAttributes.configs["EnabledMod"].ToString()))
                {
                    var harmony = new Harmony("net.pch91.stationeers.SOGS.patch");
                    if (bool.Parse(StaticAttributes.configs["EnabledBP"].ToString()))
                    {
                        log("Load - Backpack System", Logs.INFO);

                        harmony.PatchAll(typeof(BackpackPatch));
                    }
                    if (bool.Parse(StaticAttributes.configs["EnabledM"].ToString()))
                    {
                        log("Load - Mine System", Logs.INFO);

                        harmony.PatchAll(typeof(MinePatch));
                    }
                    if (bool.Parse(StaticAttributes.configs["EnabledC"].ToString()))
                    {
                        log("Load - Combuston system", Logs.INFO);

                        harmony.PatchAll(typeof(AtmosphereCombustPatch));
                    }
                    if (bool.Parse(StaticAttributes.configs["EnabledDirtyW"].ToString()))
                    {
                        log("Load -  polluted water contact System", Logs.INFO);

                        harmony.PatchAll(typeof(water));
                    }
                    if (bool.Parse(StaticAttributes.configs["EnabledT"].ToString()))
                    {
                        log("Load - Termal System", Logs.INFO);

                        harmony.PatchAll(typeof(GeothermalAtmospherePatch));
                    }
                    if (bool.Parse(StaticAttributes.configs["EnabledB"].ToString()))
                    {
                        log("Load - Battery Nuke", Logs.INFO);

                        harmony.PatchAll(typeof(AtomicBatteryPatch));
                    }
                }
                log("Finish patch", Logs.INFO);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void Handleconfig()
        { 
            mainconfigs.Add("LogEnabled", Config.Bind("0 - General configuration", "Log Level", "info", "Enable or disable logs. values can be debug , info or error"));
            mainconfigs.Add("EnabledMod", Config.Bind("0 - General configuration", "Eneble mod", true, "Enable or disable mod. values can be false or true"));
            mainconfigs.Add("EnabledB", Config.Bind("0 - General configuration", "Eneble Battery Nuke", true, "Enable or disable part of mod. values can be false or true"));
            mainconfigs.Add("EnabledC", Config.Bind("0 - General configuration", "Eneble Combuston system", true, "Enable or disable part of mod. values can be false or true"));
            mainconfigs.Add("EnabledT", Config.Bind("0 - General configuration", "Eneble Termal System", true, "Enable or disable part of mod. values can be false or true"));
            mainconfigs.Add("EnabledM", Config.Bind("0 - General configuration", "Eneble Mine System", true, "Enable or disable part of mod. values can be false or true"));
            mainconfigs.Add("EnabledBP", Config.Bind("0 - General configuration", "Eneble Backpack System", true, "Enable or disable part of mod. values can be false or true"));
            mainconfigs.Add("EnabledDirtyW", Config.Bind("0 - General configuration", "Eneble polluted water contact System", true, "Enable or disable polluting water when in contact with another liquid. values can be false or true \n true values mean that when purified water is in contact with another liquid it will become polluted and change to polluted water"));


            fconfigEvents.Add("OuchRadius", Config.Bind("1 - Battery", "Damage Radius", 190f, "Minimum battery damage radius to food and people"));
            fconfigEvents.Add("OuchDps", Config.Bind("1 - Battery", "Damage increment", 0.5f, "Damage factor dealt to food and people\n Values between 0 and 1"));
            fconfigEvents.Add("damageRechargeBateryNuke", Config.Bind("1 - Battery", "Damage Recharge Batery", 0.01f, "Damage caused to the battery after self-charging\n Values between 0.1 and 0.01"));
            fconfigEvents.Add("fatRechargeBateryNuke", Config.Bind("1 - Battery", "Factor Recharge", 0.005f, "battery recharge factor\n Values between 0 and 1"));

            fconfigEvents.Add("FCW", Config.Bind("2 - Combustion", "Factor combustion wather", 0.05f, "water generation factor when there is volatile combustion and o2"));

            fconfigEvents.Add("DamageRange", Config.Bind("3 - Thermal", "Action Radios", 10f, "conversation radius near lava"));
            fconfigEvents.Add("MaxPowerPerVolume", Config.Bind("2 - Thermal", "Max Power Per Volume", 100f, "maximum temperature transmission near the lava"));

            mainconfigs.Add("EnabledMDMRegions", Config.Bind("2 - Mine", "Eneble Deep mine regions", true, "Enable or disable DeepMine regions in Mine System. values can be false or true \n true values able DeepMiner only works on places has a mines"));
            fconfigEvents.Add("rangeDP", Config.Bind("2 - Mine", "Distance", 10f, "maximum distance that deepMine picks up ore"));
            fconfigEvents.Add("CentrifugeDirtyOreMetod", Config.Bind("2 - Mine", "return method", (byte)2, "Pre-processing return method, values can be 1 or 2: \n 1 - does not return rare ores like lead,uranium,nikel....\n 2 - returns a reduced percentage for rare ores."));
            fconfigEvents.Add("Ores", Config.Bind("2 - Mine", "Ores aplicate", "LEAD,COBALT,NICKEL", "Ores in which the method will be applied ....\n  COBALT,LEAD,NICKEL,URANIUM They are the ores that can be applied when specifying more than one separated by a comma."));
            fconfigEvents.Add("RetCentPorc", Config.Bind("2 - Mine", "return factor", 0.1f, "ore return factor when processing a dirty ore in method 2 \n Values between 0.1 and 1"));

            fconfigEvents.Add("beltPossitionBP", Config.Bind("3 - Backpack configuration", "Belt Slot Possition", 0, "The position where the belt can be placed needs to change if you modify the start game or use mods that modify it so that the belts are generated. values from 1 to 10 \n 0 defines the last slot"));

            loglevel = (mainconfigs["LogEnabled"] as ConfigEntry<string>).Value.ToUpper();

            StaticConfig();
        }

        private void StaticConfig()
        {
            StaticAttributes.configs.Add("EnabledMod", (mainconfigs["EnabledMod"] as ConfigEntry<bool>).Value);
            StaticAttributes.configs.Add("EnabledB", (mainconfigs["EnabledB"] as ConfigEntry<bool>).Value);
            StaticAttributes.configs.Add("EnabledC", (mainconfigs["EnabledC"] as ConfigEntry<bool>).Value);
            StaticAttributes.configs.Add("EnabledT", (mainconfigs["EnabledT"] as ConfigEntry<bool>).Value);
            StaticAttributes.configs.Add("EnabledM", (mainconfigs["EnabledM"] as ConfigEntry<bool>).Value);
            StaticAttributes.configs.Add("EnabledBP", (mainconfigs["EnabledBP"] as ConfigEntry<bool>).Value);
            StaticAttributes.configs.Add("EnabledDirtyW", (mainconfigs["EnabledDirtyW"] as ConfigEntry<bool>).Value);


            StaticAttributes.Baterryconfigs.Add("OuchRadius", (fconfigEvents["OuchRadius"] as ConfigEntry<float>).Value);
            StaticAttributes.Baterryconfigs.Add("OuchDps", (fconfigEvents["OuchDps"] as ConfigEntry<float>).Value);
            StaticAttributes.Baterryconfigs.Add("damageRechargeBateryNuke", (fconfigEvents["damageRechargeBateryNuke"] as ConfigEntry<float>).Value);
            StaticAttributes.Baterryconfigs.Add("fatRechargeBateryNuke", (fconfigEvents["fatRechargeBateryNuke"] as ConfigEntry<float>).Value);

            StaticAttributes.Combustonconfigs.Add("FCW", (fconfigEvents["FCW"] as ConfigEntry<float>).Value);

            StaticAttributes.Thermalconfigs.Add("DamageRange", (fconfigEvents["DamageRange"] as ConfigEntry<float>).Value);
            StaticAttributes.Thermalconfigs.Add("MaxPowerPerVolume", (fconfigEvents["MaxPowerPerVolume"] as ConfigEntry<float>).Value);

            StaticAttributes.configs.Add("DeepMinerRegions", (mainconfigs["EnabledMDMRegions"] as ConfigEntry<bool>).Value);
            StaticAttributes.mineConfigsFloat.Add("rangeDP", (fconfigEvents["rangeDP"] as ConfigEntry<float>).Value);
            StaticAttributes.CentrifugeDirtyOreMetod = (fconfigEvents["CentrifugeDirtyOreMetod"] as ConfigEntry<byte>).Value;
            StaticAttributes.mineConfigsFloat.Add("RetCentPorc", (fconfigEvents["RetCentPorc"] as ConfigEntry<float>).Value);

            StaticAttributes.mineOreReagentIdDic.Add("COBALT", "Cobalt");
            StaticAttributes.mineOreReagentIdDic.Add("LEAD", "Lead");
            StaticAttributes.mineOreReagentIdDic.Add("NICKEL", "Nickel");
            StaticAttributes.mineOreReagentIdDic.Add("URANIUM", "Uranium");

            String yx = (fconfigEvents["Ores"] as ConfigEntry<string>).Value;

            //StaticAttributes.CentrifugeDirtyOreMetod = 1;
            StaticAttributes.mineOreReagentId = StaticAttributes.mineOreReagentIdDic.Where(x => yx.Contains(x.Key)).Select(x => x.Value).ToArray();

            StaticAttributes.beltPosition = (fconfigEvents["beltPossitionBP"] as ConfigEntry<int>).Value;
        }
    }
}