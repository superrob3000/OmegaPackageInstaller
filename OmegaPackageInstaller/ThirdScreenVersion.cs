using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OmegaPackageInstaller
{
    internal class ThirdScreenVersion
    {
        public static int Compare(String version1, String version2)
        {
            //return  1 if version1 >  version2
            //return  0 if version1 == version2
            //return -1 if version1 <  version2
            //return -2 for any errors

            //Extract versions in the form of xx.xx.xx (ignore any trailing characters)
            MatchCollection match1 = Regex.Matches(version1, "[0-9]+");
            MatchCollection match2 = Regex.Matches(version2, "[0-9]+");

            if ((match1.Count < 3) ||
                (match2.Count < 3))
            {
                return -2;
            }

            //First check major
            if (Convert.ToInt32(match1[0].Captures[0].Value) > Convert.ToInt32(match2[0].Captures[0].Value))
                return 1;
            else if (Convert.ToInt32(match1[0].Captures[0].Value) < Convert.ToInt32(match2[0].Captures[0].Value))
                return -1;

            //Major was equal, now check middle
            if (Convert.ToInt32(match1[1].Captures[0].Value) > Convert.ToInt32(match2[1].Captures[0].Value))
                return 1;
            else if (Convert.ToInt32(match1[1].Captures[0].Value) < Convert.ToInt32(match2[1].Captures[0].Value))
                return -1;

            //Middle was equal, now check minor
            if (Convert.ToInt32(match1[2].Captures[0].Value) > Convert.ToInt32(match2[2].Captures[0].Value))
                return 1;
            else if (Convert.ToInt32(match1[2].Captures[0].Value) < Convert.ToInt32(match2[2].Captures[0].Value))
                return -1;

            //Versions are equal
            return 0;
        }
    }
}
