using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checker
{
    public class CheckerBase
    {
        protected float ConvertPriceString(string pricestring)
        {
            float price = 0;
            if (string.IsNullOrEmpty(pricestring))
            {
                return price;
            }

            string revisedstring = pricestring.Trim().TrimStart('$');
            float.TryParse(revisedstring, out price);

            return price;
        }
    }
}
