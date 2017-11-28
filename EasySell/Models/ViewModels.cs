using EasySell.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasySell.Models
{
    public class OrderViewModel
    {
        public Order OrderInfo { get; set; }
        public string CustomerName { get; set; }
        public string OrderNumber { get; set; }
        public double OrderTotalPrice { get; set; }
        public int OrderedGoodQty { get; set; }
    }
}