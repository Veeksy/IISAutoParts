﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IISAutoParts.DBcontext
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class IISAutoPartsEntities : DbContext
    {
        public IISAutoPartsEntities()
            : base("name=IISAutoPartsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<autoparts> autoparts { get; set; }
        public virtual DbSet<autopartsCategory> autopartsCategory { get; set; }
        public virtual DbSet<carModels> carModels { get; set; }
        public virtual DbSet<cars> cars { get; set; }
        public virtual DbSet<customers> customers { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<provide> provide { get; set; }
        public virtual DbSet<providers> providers { get; set; }
        public virtual DbSet<users> users { get; set; }
        public virtual DbSet<OrdersDoc> OrdersDoc { get; set; }
        public virtual DbSet<provideDoc> provideDoc { get; set; }
        public virtual DbSet<orderReports> orderReports { get; set; }
        public virtual DbSet<provideReports> provideReports { get; set; }
    }
}
