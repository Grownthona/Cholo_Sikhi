﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cholo_Sikhi.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class usersEntities4 : DbContext
    {
        public usersEntities4()
            : base("name=usersEntities4")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<userinfo> userinfoes { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Cours> Courses { get; set; }
        public virtual DbSet<SectionContent> SectionContents { get; set; }
        public virtual DbSet<Quiz> Quizs { get; set; }
        public virtual DbSet<Result> Results { get; set; }
    }
}
