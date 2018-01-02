using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checker
{
    public enum Retailer
    {
        COLES = 1,

    }
    public class Program
    {

        public static void  Main(string[] args)
        {
            MainAsync(args).Wait();
            
        }

        static async Task MainAsync(string[] args)
        {
            PriceCheckerDataEntities db = new PriceCheckerDataEntities();
            Coles coles = new Coles();
            foreach (GoodSearchKeyword kw in db.GoodSearchKeywords.ToList())
            {
                string kwstring = kw.ColesKeyword;
                await coles.GetPricesAsync(kwstring.Trim());
            }
        }
    }
}
