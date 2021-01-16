using System;

namespace Tess4Windows.UserControls {

    /// <summary>
    /// Umwandlungen für UI / UiController
    /// </summary>
    internal static class UiConversion {

        internal static string GetValueStringMiToKm(double? dbVal) {
            if ( !dbVal.HasValue ) return "";
            dbVal   = dbVal * 1.60934; // Meilen
            int val = (int)Math.Round(dbVal.Value, 0);
            return val.ToString();
        }

        internal static string GetValueString(double? dbVal, bool round = true) {
            if ( !dbVal.HasValue ) return "";

            if ( round ) {
                int val = (int)Math.Round(dbVal.Value, 0);
                return val.ToString();
            }
            else return dbVal.ToString();
        }

        internal static string GetValueString(bool? dbVal) {
            if ( !dbVal.HasValue ) return "";

            if ( dbVal.Value )  return "Yes";
            else                return "No";
        }

        internal static int GetValueInt(int? dbVal) {
            if ( !dbVal.HasValue ) return 0;
            return dbVal.Value;
        }
    }
}