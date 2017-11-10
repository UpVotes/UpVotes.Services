﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UpVotes.DataModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class UpVotesEntities : DbContext
    {
        public UpVotesEntities()
            : base("name=UpVotesEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanyBranch> CompanyBranches { get; set; }
        public virtual DbSet<CompanyFocus> CompanyFocus1 { get; set; }
        public virtual DbSet<CompanyPortFolio> CompanyPortFolios { get; set; }
        public virtual DbSet<CompanyReview> CompanyReviews { get; set; }
        public virtual DbSet<CompanyReviewThankNote> CompanyReviewThankNotes { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<FocusArea> FocusAreas { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<UserRegistration> UserRegistrations { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserToken> UserTokens { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<CompanyVote> CompanyVotes { get; set; }
    
        public virtual ObjectResult<Sp_GetCompany_Result> Sp_GetCompany(string companyName, Nullable<decimal> minRate, Nullable<decimal> maxRate, Nullable<int> minEmployee, Nullable<int> maxEmployee, string sortByVotes, Nullable<int> focusAreaID, Nullable<int> userID)
        {
            var companyNameParameter = companyName != null ?
                new ObjectParameter("companyName", companyName) :
                new ObjectParameter("companyName", typeof(string));
    
            var minRateParameter = minRate.HasValue ?
                new ObjectParameter("minRate", minRate) :
                new ObjectParameter("minRate", typeof(decimal));
    
            var maxRateParameter = maxRate.HasValue ?
                new ObjectParameter("maxRate", maxRate) :
                new ObjectParameter("maxRate", typeof(decimal));
    
            var minEmployeeParameter = minEmployee.HasValue ?
                new ObjectParameter("minEmployee", minEmployee) :
                new ObjectParameter("minEmployee", typeof(int));
    
            var maxEmployeeParameter = maxEmployee.HasValue ?
                new ObjectParameter("maxEmployee", maxEmployee) :
                new ObjectParameter("maxEmployee", typeof(int));
    
            var sortByVotesParameter = sortByVotes != null ?
                new ObjectParameter("sortByVotes", sortByVotes) :
                new ObjectParameter("sortByVotes", typeof(string));
    
            var focusAreaIDParameter = focusAreaID.HasValue ?
                new ObjectParameter("focusAreaID", focusAreaID) :
                new ObjectParameter("focusAreaID", typeof(int));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("userID", userID) :
                new ObjectParameter("userID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetCompany_Result>("Sp_GetCompany", companyNameParameter, minRateParameter, maxRateParameter, minEmployeeParameter, maxEmployeeParameter, sortByVotesParameter, focusAreaIDParameter, userIDParameter);
        }
    
        public virtual ObjectResult<Sp_GetCompanyBranches_Result> Sp_GetCompanyBranches(Nullable<int> companyID)
        {
            var companyIDParameter = companyID.HasValue ?
                new ObjectParameter("CompanyID", companyID) :
                new ObjectParameter("CompanyID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetCompanyBranches_Result>("Sp_GetCompanyBranches", companyIDParameter);
        }
    
        public virtual ObjectResult<Sp_GetCompanyFocus_Result> Sp_GetCompanyFocus(Nullable<int> companyID)
        {
            var companyIDParameter = companyID.HasValue ?
                new ObjectParameter("CompanyID", companyID) :
                new ObjectParameter("CompanyID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetCompanyFocus_Result>("Sp_GetCompanyFocus", companyIDParameter);
        }
    
        public virtual ObjectResult<Sp_GetCompanyPortFolio_Result> Sp_GetCompanyPortFolio(Nullable<int> companyID)
        {
            var companyIDParameter = companyID.HasValue ?
                new ObjectParameter("CompanyID", companyID) :
                new ObjectParameter("CompanyID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetCompanyPortFolio_Result>("Sp_GetCompanyPortFolio", companyIDParameter);
        }
    
        public virtual ObjectResult<Sp_GetCompanyReviews_Result> Sp_GetCompanyReviews(Nullable<int> companyID)
        {
            var companyIDParameter = companyID.HasValue ?
                new ObjectParameter("CompanyID", companyID) :
                new ObjectParameter("CompanyID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetCompanyReviews_Result>("Sp_GetCompanyReviews", companyIDParameter);
        }
    
        public virtual ObjectResult<Sp_GetCompanyReviewThankNotedUsers_Result> Sp_GetCompanyReviewThankNotedUsers(Nullable<int> companyID, Nullable<int> companyReviewID)
        {
            var companyIDParameter = companyID.HasValue ?
                new ObjectParameter("CompanyID", companyID) :
                new ObjectParameter("CompanyID", typeof(int));
    
            var companyReviewIDParameter = companyReviewID.HasValue ?
                new ObjectParameter("CompanyReviewID", companyReviewID) :
                new ObjectParameter("CompanyReviewID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetCompanyReviewThankNotedUsers_Result>("Sp_GetCompanyReviewThankNotedUsers", companyIDParameter, companyReviewIDParameter);
        }
    
        public virtual ObjectResult<Sp_GetFocusArea_Result> Sp_GetFocusArea()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetFocusArea_Result>("Sp_GetFocusArea");
        }
    }
}
