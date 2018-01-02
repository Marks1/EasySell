using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Checker
{
    public class Coles : CheckerBase
    {
        private string ColesOnlineURL = string.Empty;
        private string ColesOnlineRoot = string.Empty;
        PriceCheckerDataEntities db = new PriceCheckerDataEntities();
        public Coles()
        {
            ColesOnlineURL = @"https://shop.coles.com.au/a/a-national/everything/search/";
            ColesOnlineRoot = @"https://shop.coles.com.au";
        }


        public async Task GetPricesAsync(string goodkeyword)
        {
            List<HtmlNode> goodslist = new List<HtmlNode>();
            string searchURL = string.Format("{0}{1}?pageNumber=1", ColesOnlineURL, HttpUtility.UrlEncode(goodkeyword));
            try
            {
                CheckerHtml htmlparse = new CheckerHtml();
                HtmlDocument html = htmlparse.GetHTML2(searchURL);
                goodslist = html.DocumentNode.Descendants().Where(x => (x.Name == "div" && x.HasClass("product"))).ToList();
            }catch(Exception e)
            {
                return;
            }
            
            foreach(HtmlNode product in goodslist)
            {
                try
                {
                    string Goodrealname = product.Descendants().Where(x => (x.Name == "span" && x.HasClass("accessibility"))).ToList()[0].InnerText;
                    int retailer = (int)Retailer.COLES;
                    int qty = 1;
                    Int32.TryParse(product.Descendants().Where(x => (x.Name == "span" && x.HasClass("product-qty"))).ToList()[0].InnerText, out qty);
                    string priceString = product.Descendants().Where(x => (x.Name == "span" && x.HasClass("product-price"))).ToList()[0].InnerText;
                    float price = ConvertPriceString(priceString) / qty;
                    string sublink = product.Descendants().Where(x => (x.Name=="h3" && x.HasClass("product-title"))).ToList()[0].GetAttributeValue("href", "");
                    string link = ColesOnlineRoot + sublink;
                    string brand = product.Descendants().Where(x => (x.Name=="span" && x.HasClass("product-brand"))).ToList()[0].InnerText;

                    GoodPrice goodprice = new GoodPrice
                    {
                        GoodRealName = Goodrealname,
                        Retailer = retailer,
                        Price = price,
                        Goodlink = link,
                        Brand = brand
                    };
                    db.GoodPrices.Add(goodprice);
                    db.SaveChanges();

                }
                catch (Exception e)
                {

                }
            }            
        }
        
    }
}
