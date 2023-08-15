using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace sogs_standing_on_giants_shoulders_a_collection_of_physics_improv.patch
{
    public static class StaticAttributes
    {
        public static Dictionary<string, object> configs = new Dictionary<string, object>();

        public static Dictionary<string, object> Baterryconfigs = new Dictionary<string, object>();

        public static Dictionary<string, object> Combustonconfigs = new Dictionary<string, object>();

        public static Dictionary<string, object> Thermalconfigs = new Dictionary<string, object>();

        public static String incMetName { get; set; }
        public static String confcinc { get; set; }
    }
}
