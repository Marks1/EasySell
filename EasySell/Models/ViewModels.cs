using EasySell.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

    public class OrdersViewModel
    {
        public int ActiveOrdersNum { set; get; }
        public int InactiveOrdersNum { set; get; }
        public int UnshippedOrdersNum { set; get; }
        public int UnpaidOrdersNum { set; get; }
        public List<OrderViewModel> OrdersList { set; get; }
        public List<SelectListItem> AllCuseroms { set; get; }
    }

    public class NewOrderViewModel
    {
        public int SelectedCustomerID { set; get; }
        public bool PriorityHigh { set; get; }
        public List<SelectListItem> AllCuseroms { set; get; }
    }

    public class CustomerViewModel
    {
        public Customer CustomerInfo { get; set; }
        public int Rating { get; set; }        
    }


    public class CustomerDetailsViewModel
    {
        public Customer CustomerInfo { get; set; }
        public List<OrderViewModel> OrdersOfCustomer { get; set; }
    }

    public class GoodInfoViewModel
    {
        public GoodInfo GoodInfo { get; set; }
        public string AverageCost { get; set; }
        public string AveragePrice { get; set; }
        public int TotalSold { get; set; }
    }
}