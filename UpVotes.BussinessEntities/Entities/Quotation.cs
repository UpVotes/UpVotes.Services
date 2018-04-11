using System.Collections.Generic;

namespace UpVotes.BusinessEntities.Entities
{
    //[DataContract]
    //public class ServiceRequest<T>
    //{
    //   public T RequestObject { get; set; }
    //}    
    public class QuotationRequest
    {
        
        public string platform { get; set; }
        
        public string Theme { get; set; }
        
        public string LoginSecurity { get; set; }
        
        public string Profile { get; set; }
        
        public string Security { get; set; }
        
        public string ReviewRate { get; set; }
        
        public string Service { get; set; }
        
        public string Database { get; set; }
        
        public string featuresstring { get; set; }
        
        public string EmailId { get; set; }
        
        public string Name { get; set; }
        
        public string CompanyName { get; set; }
        public List<string> Features { get; set; }
    }
    
    public class QuotationResponse
    {
        
        public decimal? TotalMinZone1 { get; set; }
        
        public decimal? TotalMaxZone1 { get; set; }
        public decimal? TotalMinZone2 { get; set; }
        
        public decimal? TotalMaxZone2 { get; set; }
        
        public decimal? TotalMinZone3 { get; set; }
        
        public decimal? TotalMaxZone3 { get; set; }
        
        public List<QuotationInfo> QuotationData { get; set; }   
        
    }
    
    public class QuotationInfo
    {
        
        public int QuotationRateCardID { get; set; }
       
        public string MainCategory { get; set; }
        
        public string SubCategory { get; set; }
        
        public string Ctypes { get; set; }
        
        public string Classname { get; set; }
        
        public decimal? MinPriceZone1 { get; set; }
        
        public decimal? MaxPriceZone1 { get; set; }
        
        public decimal? MinPriceZone2 { get; set; }
        
        public decimal? MaxPriceZone2 { get; set; }
        
        public decimal? MinPriceZone3 { get; set; }
        
        public decimal? MaxPriceZone3 { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
    public class ServiceResponse<T>
    {
        
        public T ResponseObject { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
    }
}
