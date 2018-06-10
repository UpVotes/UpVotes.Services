using System;

namespace UpVotes.BusinessServices
{
    public class Helper
    {
        public static string BasicDecryptString(string str)
        {
            if (str.IndexOf("%lthash%") != -1)
            {
                str = str.Replace("%lthash%", "&#");
            }

            return System.Web.HttpUtility.HtmlDecode(Uri.UnescapeDataString(str));
        }

        public static string BasicEncryptString(string str)
        {
            if (str.IndexOf("&#") != -1)
            {
                str = str.Replace("&#", "%lthash%");
            }

            var index = str.IndexOf("+"); //Added for QA Correction as on 10-Oct-2014 for Sales_PL_Unit_0452 by Ashok Kumar M

            while (index != -1)
            {

                str = str.Replace("+", "%Plus%");

                index = str.IndexOf("+");

            }
            return System.Web.HttpUtility.HtmlEncode(Uri.EscapeDataString((str)));
        }
    }
}
