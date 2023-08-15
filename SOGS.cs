using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.patch;

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
                Debug.Log("["+ level + "    :   SOGS] " + line);
            }
        }


        public static Dictionary<string, object> fmainconfig = new Dictionary<string, object>();
        public static Dictionary<string, object> fconfigEvents = new Dictionary<string, object>();
        private Dictionary<string, object> mainconfigs = new Dictionary<string, object>();


        private void Awake()
        {
            try
            {
                log("Start - RemoveReciples", Logs.INFO);
                Instance = this;
                //Harmony.DEBUG = true;
                Handleconfig();
                if (bool.Parse(StaticAttributes.configs["EnabledMod"].ToString())) {
                    var harmony = new Harmony("net.pch91.stationeers.SOGS.patch");
                    harmony.PatchAll();
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


            fconfigEvents.Add("OuchRadius", Config.Bind("1 - Battery", "Damage Radius", 190f, ""));
            fconfigEvents.Add("OuchDps", Config.Bind("1 - Battery", "Damage increment", 0.5f, ""));
            fconfigEvents.Add("damageRechargeBateryNuke", Config.Bind("1 - Battery", "Damage Recharge Batery", 0.01f, ""));
            fconfigEvents.Add("fatRechargeBateryNuke", Config.Bind("1 - Battery", "Factor Recharge", 0.05f, ""));

            fconfigEvents.Add("FCW", Config.Bind("2 - Combustion", "Factor combustion wather", 0.1f, ""));

            fconfigEvents.Add("DamageRange", Config.Bind("3 - Thermal", "Action Radios", 10f, ""));
            fconfigEvents.Add("MaxPowerPerVolume", Config.Bind("2 - Thermal", "Max Power Per Volume", 100f, ""));

            loglevel = (mainconfigs["LogEnabled"] as ConfigEntry<string>).Value.ToUpper();

            StaticConfig();
        }

        private void StaticConfig()
        {
            StaticAttributes.configs.Add("EnabledMod", (mainconfigs["EnabledMod"] as ConfigEntry<bool>).Value) ;
            StaticAttributes.configs.Add("EnabledB", (mainconfigs["EnabledB"] as ConfigEntry<bool>).Value);
            StaticAttributes.configs.Add("EnabledC", (mainconfigs["EnabledC"] as ConfigEntry<bool>).Value);
            StaticAttributes.configs.Add("EnabledT", (mainconfigs["EnabledT"] as ConfigEntry<bool>).Value);

            StaticAttributes.Baterryconfigs.Add("OuchRadius", (fconfigEvents["OuchRadius"] as ConfigEntry<float>).Value);
            StaticAttributes.Baterryconfigs.Add("OuchDps", (fconfigEvents["OuchDps"] as ConfigEntry<float>).Value);
            StaticAttributes.Baterryconfigs.Add("damageRechargeBateryNuke", (fconfigEvents["damageRechargeBateryNuke"] as ConfigEntry<float>).Value);
            StaticAttributes.Baterryconfigs.Add("fatRechargeBateryNuke", (fconfigEvents["fatRechargeBateryNuke"] as ConfigEntry<float>).Value);
           
            StaticAttributes.Combustonconfigs.Add("FCW", (fconfigEvents["FCW"] as ConfigEntry<float>).Value);

            StaticAttributes.Thermalconfigs.Add("DamageRange", (fconfigEvents["DamageRange"] as ConfigEntry<float>).Value);
            StaticAttributes.Thermalconfigs.Add("MaxPowerPerVolume", (fconfigEvents["MaxPowerPerVolume"] as ConfigEntry<float>).Value);
        }
    }
}