using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeSin.View
{
    public class Language
    {

        public static Dictionary<string, string> language = new Dictionary<string, string>();

        public static Dictionary<string, string> LanguageDic
        {
            get { return language; }
        }

        protected Language()
        {
            
        }

    }
}
