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
    
        public virtual DbSet<CompanyBranch> CompanyBranches { get; set; }
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
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<CategoryBasedMetaTags> CategoryBasedMetaTags { get; set; }
        public virtual DbSet<CompanySubFocus> CompanySubFocus { get; set; }
        public virtual DbSet<QuotationRateCard> QuotationRateCard { get; set; }
        public virtual DbSet<SubFocusArea> SubFocusArea { get; set; }
        public virtual DbSet<UserQuotation> UserQuotation { get; set; }
        public virtual DbSet<CompanyPendingForApproval> CompanyPendingForApproval { get; set; }
        public virtual DbSet<CompanyFocus> CompanyFocus { get; set; }
        public virtual DbSet<AverageHourlyRate> AverageHourlyRate { get; set; }
        public virtual DbSet<EmployeeRange> EmployeeRange { get; set; }
        public virtual DbSet<Email> Email { get; set; }
    
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
    
        public virtual ObjectResult<Sp_GetCompanyReviews_Result> Sp_GetCompanyReviews(string companyName)
        {
            var companyNameParameter = companyName != null ?
                new ObjectParameter("companyName", companyName) :
                new ObjectParameter("companyName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetCompanyReviews_Result>("Sp_GetCompanyReviews", companyNameParameter);
        }
    
        public virtual ObjectResult<Sp_GetCompany_Result> Sp_GetCompany(string companyName, Nullable<decimal> minRate, Nullable<decimal> maxRate, Nullable<int> minEmployee, Nullable<int> maxEmployee, string sortByVotes, Nullable<int> focusAreaID, Nullable<int> userID, string location, Nullable<int> pageNo, Nullable<int> pageSize, string subFocusArea)
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
    
            var locationParameter = location != null ?
                new ObjectParameter("location", location) :
                new ObjectParameter("location", typeof(string));
    
            var pageNoParameter = pageNo.HasValue ?
                new ObjectParameter("PageNo", pageNo) :
                new ObjectParameter("PageNo", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("PageSize", pageSize) :
                new ObjectParameter("PageSize", typeof(int));
    
            var subFocusAreaParameter = subFocusArea != null ?
                new ObjectParameter("SubFocusArea", subFocusArea) :
                new ObjectParameter("SubFocusArea", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetCompany_Result>("Sp_GetCompany", companyNameParameter, minRateParameter, maxRateParameter, minEmployeeParameter, maxEmployeeParameter, sortByVotesParameter, focusAreaIDParameter, userIDParameter, locationParameter, pageNoParameter, pageSizeParameter, subFocusAreaParameter);
        }
    
        public virtual ObjectResult<string> Sp_GetCompanyNames(Nullable<int> type, Nullable<int> focusAreaID, string companyNamelocation)
        {
            var typeParameter = type.HasValue ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(int));
    
            var focusAreaIDParameter = focusAreaID.HasValue ?
                new ObjectParameter("focusAreaID", focusAreaID) :
                new ObjectParameter("focusAreaID", typeof(int));
    
            var companyNamelocationParameter = companyNamelocation != null ?
                new ObjectParameter("CompanyNamelocation", companyNamelocation) :
                new ObjectParameter("CompanyNamelocation", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("Sp_GetCompanyNames", typeParameter, focusAreaIDParameter, companyNamelocationParameter);
        }
    
        public virtual ObjectResult<Sp_GetQuoteForMobileApp_Result> Sp_GetQuoteForMobileApp(string platformType, string themeType, string loginsecurityType, string profileType, string securityType, string reviewrateType, string serviceType, string databaseType, string featuresType)
        {
            var platformTypeParameter = platformType != null ?
                new ObjectParameter("platformType", platformType) :
                new ObjectParameter("platformType", typeof(string));
    
            var themeTypeParameter = themeType != null ?
                new ObjectParameter("themeType", themeType) :
                new ObjectParameter("themeType", typeof(string));
    
            var loginsecurityTypeParameter = loginsecurityType != null ?
                new ObjectParameter("loginsecurityType", loginsecurityType) :
                new ObjectParameter("loginsecurityType", typeof(string));
    
            var profileTypeParameter = profileType != null ?
                new ObjectParameter("profileType", profileType) :
                new ObjectParameter("profileType", typeof(string));
    
            var securityTypeParameter = securityType != null ?
                new ObjectParameter("securityType", securityType) :
                new ObjectParameter("securityType", typeof(string));
    
            var reviewrateTypeParameter = reviewrateType != null ?
                new ObjectParameter("reviewrateType", reviewrateType) :
                new ObjectParameter("reviewrateType", typeof(string));
    
            var serviceTypeParameter = serviceType != null ?
                new ObjectParameter("serviceType", serviceType) :
                new ObjectParameter("serviceType", typeof(string));
    
            var databaseTypeParameter = databaseType != null ?
                new ObjectParameter("databaseType", databaseType) :
                new ObjectParameter("databaseType", typeof(string));
    
            var featuresTypeParameter = featuresType != null ?
                new ObjectParameter("featuresType", featuresType) :
                new ObjectParameter("featuresType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetQuoteForMobileApp_Result>("Sp_GetQuoteForMobileApp", platformTypeParameter, themeTypeParameter, loginsecurityTypeParameter, profileTypeParameter, securityTypeParameter, reviewrateTypeParameter, serviceTypeParameter, databaseTypeParameter, featuresTypeParameter);
        }
    
        public virtual ObjectResult<Nullable<decimal>> Sp_InsUserQuotation(string platformType, string themeType, string loginsecurityType, string profileType, string securityType, string reviewrateType, string serviceType, string databaseType, string featuresType, string emailID, string name, string companyName)
        {
            var platformTypeParameter = platformType != null ?
                new ObjectParameter("platformType", platformType) :
                new ObjectParameter("platformType", typeof(string));
    
            var themeTypeParameter = themeType != null ?
                new ObjectParameter("themeType", themeType) :
                new ObjectParameter("themeType", typeof(string));
    
            var loginsecurityTypeParameter = loginsecurityType != null ?
                new ObjectParameter("loginsecurityType", loginsecurityType) :
                new ObjectParameter("loginsecurityType", typeof(string));
    
            var profileTypeParameter = profileType != null ?
                new ObjectParameter("profileType", profileType) :
                new ObjectParameter("profileType", typeof(string));
    
            var securityTypeParameter = securityType != null ?
                new ObjectParameter("securityType", securityType) :
                new ObjectParameter("securityType", typeof(string));
    
            var reviewrateTypeParameter = reviewrateType != null ?
                new ObjectParameter("reviewrateType", reviewrateType) :
                new ObjectParameter("reviewrateType", typeof(string));
    
            var serviceTypeParameter = serviceType != null ?
                new ObjectParameter("serviceType", serviceType) :
                new ObjectParameter("serviceType", typeof(string));
    
            var databaseTypeParameter = databaseType != null ?
                new ObjectParameter("databaseType", databaseType) :
                new ObjectParameter("databaseType", typeof(string));
    
            var featuresTypeParameter = featuresType != null ?
                new ObjectParameter("featuresType", featuresType) :
                new ObjectParameter("featuresType", typeof(string));
    
            var emailIDParameter = emailID != null ?
                new ObjectParameter("emailID", emailID) :
                new ObjectParameter("emailID", typeof(string));
    
            var nameParameter = name != null ?
                new ObjectParameter("name", name) :
                new ObjectParameter("name", typeof(string));
    
            var companyNameParameter = companyName != null ?
                new ObjectParameter("companyName", companyName) :
                new ObjectParameter("companyName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("Sp_InsUserQuotation", platformTypeParameter, themeTypeParameter, loginsecurityTypeParameter, profileTypeParameter, securityTypeParameter, reviewrateTypeParameter, serviceTypeParameter, databaseTypeParameter, featuresTypeParameter, emailIDParameter, nameParameter, companyNameParameter);
        }
    
        public virtual ObjectResult<Sp_GetIndustrialFocus_Result> Sp_GetIndustrialFocus(Nullable<int> companyID)
        {
            var companyIDParameter = companyID.HasValue ?
                new ObjectParameter("CompanyID", companyID) :
                new ObjectParameter("CompanyID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetIndustrialFocus_Result>("Sp_GetIndustrialFocus", companyIDParameter);
        }
    
        public virtual ObjectResult<Sp_GetClientFocus_Result> Sp_GetClientFocus(Nullable<int> companyID)
        {
            var companyIDParameter = companyID.HasValue ?
                new ObjectParameter("CompanyID", companyID) :
                new ObjectParameter("CompanyID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetClientFocus_Result>("Sp_GetClientFocus", companyIDParameter);
        }
    
        public virtual ObjectResult<Sp_GetSubFocus_Result> Sp_GetSubFocus(Nullable<int> companyID)
        {
            var companyIDParameter = companyID.HasValue ?
                new ObjectParameter("CompanyID", companyID) :
                new ObjectParameter("CompanyID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetSubFocus_Result>("Sp_GetSubFocus", companyIDParameter);
        }
    
        public virtual ObjectResult<string> Sp_GetDistinctSubFocusNames(Nullable<int> companyID)
        {
            var companyIDParameter = companyID.HasValue ?
                new ObjectParameter("CompanyID", companyID) :
                new ObjectParameter("CompanyID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("Sp_GetDistinctSubFocusNames", companyIDParameter);
        }
    
        public virtual ObjectResult<Sp_CategoryMetaTags_Result> Sp_CategoryMetaTags(string focusAreaName, string subFocusAreaName)
        {
            var focusAreaNameParameter = focusAreaName != null ?
                new ObjectParameter("FocusAreaName", focusAreaName) :
                new ObjectParameter("FocusAreaName", typeof(string));
    
            var subFocusAreaNameParameter = subFocusAreaName != null ?
                new ObjectParameter("SubFocusAreaName", subFocusAreaName) :
                new ObjectParameter("SubFocusAreaName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_CategoryMetaTags_Result>("Sp_CategoryMetaTags", focusAreaNameParameter, subFocusAreaNameParameter);
        }
    
        public virtual ObjectResult<Sp_GetCompanyBranches_Result> Sp_GetCompanyBranches(Nullable<int> companyID)
        {
            var companyIDParameter = companyID.HasValue ?
                new ObjectParameter("CompanyID", companyID) :
                new ObjectParameter("CompanyID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_GetCompanyBranches_Result>("Sp_GetCompanyBranches", companyIDParameter);
        }
    }
}
