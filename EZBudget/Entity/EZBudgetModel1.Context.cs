﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EZBudget.Entity
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EZBudget_DBEntities1 : DbContext
    {
        public EZBudget_DBEntities1()
            : base("name=EZBudget_DBEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Budget> Budgets { get; set; }
        public virtual DbSet<Category_Global> Category_Global { get; set; }
        public virtual DbSet<Category_Monthly> Category_Monthly { get; set; }
        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<User_Setting> User_Setting { get; set; }
    }
}
