//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EasySell.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Storage
    {
        public int Id { get; set; }
        public int GoodID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public int Quantity { get; set; }
        public double Cost { get; set; }
        public Nullable<double> TotalCost { get; set; }
        public Nullable<int> UserID { get; set; }
    }
}
