﻿using EasySell.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasySell.Models
{    
    public class OrderViewModel
    {
        public Order OrderInfo { get; set; }
        public Customer CustomerInfo { get; set; }
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
    }

    public class NewOrderViewModel
    {
        [Required]
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


    //************************
    // Dashboard
    //*************************
    public class TopCustomerViewModel
    {
        public int CustomerID { set; get; }
        public string CustomerName { set; get; }
        public double TotalOrderedAmount { set; get; }
        public int TotalOrders { set; get; }
    }

    public class TopGoodCountViewModel
    {
        public int GoodID { set; get; }
        public string GoodName { set; get; }
        public int SoldCount { set; get; }
    }

    public class TopGoodRevenueViewModel
    {
        public int GoodID { set; get; }
        public string GoodName { set; get; }
        public int Revenue { set; get; }
    }

    public class DashboardViewModel
    {
        //bar
        public int OrdersOfThisWeek { set; get; }
        public double OrderIncrease { set; get; }
        public double SalesOfThisWeek { set; get; }
        public double SalesIncrease { set; get; }
        public double CostOfThisWeek { set; get; }
        public double CostIncrease { set; get; }
        public double TotalSales { set; get; }
        public double TotalCost { set; get; }
        public double TotalRevenue { set; get; }

        public List<TopCustomerViewModel> TopCustomers { set; get; }
        public List<TopGoodCountViewModel> TopGoodsByCount { set; get; }


    }


    //***************************
    // Process Order View
    //***************************


    public class StorageGoodViewModel
    {
        public Storage StorageGoodInfo { set; get; }
        public string GoodName { set; get; }
    }
    public class OrderedGoodViewModel
    {     
        public OrderedGood OrderedGoodInfo { set; get; }
        public string GoodName { set; get; }
    }
    public class ProcessOrderViewModel
    {
        public double TotalCost { set; get; }
        public double Revenue { set; get; }
        public int Duration { set; get; }
        public OrderViewModel OrderInfoView { set; get; }
        public List<StorageGoodViewModel> AssignedGoodInStorage { set; get; }
        public List<PackageViewModel> Packages { set; get; }
        public List<OrderedGoodViewModel> OrderedGoods { set; get; }
    }

    public class NewOrderedGoodViewModel
    {
        public int Quantity { set; get; }
        [Required]
        public int OrderID { set; get; }
        public double Price { set; get; }
        [Required]
        public int CustomerID { set; get; }
        public List<SelectListItem> AllGoods { set; get; }
        [Required]
        public int SelectedGoodID { set; get; }
    }

    public class NewStorageGoodViewModel
    {
        public int Quantity { set; get; }
        public double Cost { set; get; }
        public List<SelectListItem> AllGoods { set; get; }
        public int SelectedGoodID { set; get; }
    }

    public class MatchGoodViewModel
    {
        public OrderedGoodViewModel OrderedGood { set; get; }
        //public OrderViewModel OrderInfoView { set; get; }
        public List<StorageGoodViewModel> AvailableGoodsInStorage { set; get; }
        public int SelectedStorageID { set; get; }
        public int SelectedOrderedGoodID { set; get; }
    }


    //Package
    public class PackageViewModel
    {
        public Package PackageInfo { set; get; }
        public string DeliverName { set; get; }
    }

    public class NewPackageViewModel
    {
        public List<PackageProvider> AllPackageDelivers { set; get; }
        public string SerialNumber { set; get; }
        [Required]
        public int OrderID { set; get; }
        [Required]
        public int SelectedDeliverID { set; get; }
    }

    //Invoice
    public class InvoiceViewModel
    {
        public OrderViewModel OrderVM { set; get; }
        public List<OrderedGoodViewModel> OrderedGoods { set; get; }
        public List<PackageViewModel> Packages { set; get; }
        public double TotalPackagePrice { set; get; }
        public double TotalPrice { set; get; }
    }


    //Login

    public class SystemLoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}