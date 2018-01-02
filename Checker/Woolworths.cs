using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Checker
{
    public class Woolworths
    {
        private string WoolworthsOnlineURL = string.Empty;
        PriceCheckerDataEntities db = new PriceCheckerDataEntities();
        public Woolworths()
        {
            WoolworthsOnlineURL = "";
        }

        public List<Dictionary<string, float>> GetPrices(string goodkeyword)
        {
            List<Dictionary<string, float>> prices = new List<Dictionary<string, float>>();
            
            HttpClient http = new HttpClient();
            //var response = await http.GetByteArrayAsync(website);
            //String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
            //source = WebUtility.HtmlDecode(source);
            //HtmlDocument resultat = new HtmlDocument();
            //resultat.LoadHtml(source);

            GoodInfo goodinfo = new GoodInfo
            {
                Name = "test",
                Weight = 1.1,
                Brand = "test"
            };
            db.GoodInfoes.Add(goodinfo);
            db.SaveChanges();
            return prices;
        }
    }
}
