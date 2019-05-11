using System;
using System.Collections.Generic;
using UpVotes.BusinessEntities.Entities;
using UpVotes.BusinessServices.Interface;
using UpVotes.DataModel;
using UpVotes.DataModel.Utility;
using System.Linq;
using System.Threading;

namespace UpVotes.BusinessServices.Service
{
    public class OverviewNewsService : IOverviewAndNewsService
    {
        private static Logger Log
        {
            get
            {
                return Logger.Instance();
            }
        }        

        private UpVotesEntities _context = null;        



        public OverviewNewsResponse GetCompanySoftwareNews(OverviewNewsEntity NewsRequest)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    OverviewNewsResponse OverviewNewsList = new OverviewNewsResponse();
                    OverviewNewsList.OverviewNewsData = new List<OverviewNewsResponseEntity>();
                    var result = _context.Sp_GetSoftwareCompanyNews(NewsRequest.CategoryID, NewsRequest.SubcategoryID, NewsRequest.location, NewsRequest.IsCompanySoftware, NewsRequest.Title, NewsRequest.CompanySoftwareID).ToList();
                    if (result != null)
                    {
                        result.ToList().ForEach(q => OverviewNewsList.OverviewNewsData.Add(new OverviewNewsResponseEntity
                        {
                            CategoryID = q.CategoryID,
                            categoryname = q.categoryname,
                            City = q.City,
                            CompanyOrSoftwareID = q.CompanyOrSoftwareID,
                            CompanySoftwareNewsID = q.CompanySoftwareNewsID,
                            CountryID = q.CountryID,
                            CountryName = q.CountryName,
                            CreatedDate = q.CreatedDate,
                            ImageName = q.ImageName,
                            IsCompanySoftware = q.IsCompanySoftware,
                            isRelated = q.isRelated,
                            LogoName = q.LogoName,
                            NewsDescription = q.NewsDescription,
                            NewsTitle = q.NewsTitle,
                            SoftwareCompanyName = q.SoftwareCompanyName,
                            StateID = q.StateID,
                            StateName = q.StateName,
                            SubCategoryID = q.SubCategoryID,
                            subcategoryname = q.subcategoryname,
                            WebsiteURL = q.WebsiteURL,
                            YoutubeURL = q.YoutubeURL
                        }));
                    }
                    return OverviewNewsList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsNewsExists(string Title, string Url)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    var newslist= _context.Sp_IsNewsExists(Title, Url).ToList();
                    if(newslist != null && newslist.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int SaveAdminNews(OverviewNewsEntity NewsRequest)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                   var isNews = _context.Sp_InsAdminNews(NewsRequest.IsCompanySoftware, NewsRequest.CategoryID, NewsRequest.Subcategory,
                        NewsRequest.CountryID, NewsRequest.StateID, NewsRequest.City, NewsRequest.CompanySoftwareID, NewsRequest.WebsiteURL, NewsRequest.Title, NewsRequest.Description,
                        NewsRequest.ImageName, NewsRequest.YoutubeURL, NewsRequest.CreatedBy).FirstOrDefault();
                    if(isNews > 0)
                    {
                        return Convert.ToInt32(isNews);
                    }
                    else {
                        return 0;
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public int SaveUserNews(OverviewNewsEntity NewsRequest)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                   var isNews = _context.Sp_InsUserNews(NewsRequest.IsCompanySoftware, NewsRequest.CategoryID, NewsRequest.CompanySoftwareID, NewsRequest.WebsiteURL, 
                       NewsRequest.Title, NewsRequest.Description, NewsRequest.ImageName, NewsRequest.YoutubeURL, NewsRequest.CreatedBy).FirstOrDefault();
                    if(isNews > 0)
                    {
                        return Convert.ToInt32(isNews);
                    }
                    else {
                        return 0;
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public OverviewNewsResponse GetSoftwareCompanyNewsByName(OverviewNewsEntity NewsRequest)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    OverviewNewsResponse OverviewNewsList = new OverviewNewsResponse();
                    OverviewNewsList.OverviewNewsData = new List<OverviewNewsResponseEntity>();
                    var result = _context.Sp_GetSoftwareCompanyNewsByName(NewsRequest.IsCompanySoftware, NewsRequest.CompanySoftwareName).ToList();
                    if (result != null)
                    {
                        result.ToList().ForEach(q => OverviewNewsList.OverviewNewsData.Add(new OverviewNewsResponseEntity
                        {
                            CategoryID = q.CategoryID,
                            categoryname = q.categoryname,
                            City = q.City,
                            CompanyOrSoftwareID = q.CompanyOrSoftwareID,
                            CompanySoftwareNewsID = q.CompanySoftwareNewsID,
                            CountryID = q.CountryID,
                            CountryName = q.CountryName,
                            CreatedDate = q.CreatedDate,
                            ImageName = q.ImageName,
                            IsCompanySoftware = q.IsCompanySoftware,
                            isRelated = q.isRelated,
                            LogoName = q.LogoName,
                            NewsDescription = q.NewsDescription,
                            NewsTitle = q.NewsTitle,
                            SoftwareCompanyName = q.SoftwareCompanyName,
                            StateID = q.StateID,
                            StateName = q.StateName,
                            SubCategoryID = q.SubCategoryID,
                            subcategoryname = q.subcategoryname,
                            WebsiteURL = q.WebsiteURL,
                            YoutubeURL = q.YoutubeURL
                        }));
                    }
                    return OverviewNewsList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<OverviewNewsResponseEntity> GetCompanySoftwareNewsByID(int IsCompanySoftware, int CompanySoftwareID)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    List<OverviewNewsResponseEntity> OverviewNewsData = new List<OverviewNewsResponseEntity>();
                    var result = _context.Sp_GetSoftwareCompanyNews(0, "", "", IsCompanySoftware, "", CompanySoftwareID).ToList();
                    if (result != null)
                    {
                        result.ToList().ForEach(q => OverviewNewsData.Add(new OverviewNewsResponseEntity
                        {
                            CategoryID = q.CategoryID,
                            categoryname = q.categoryname,
                            City = q.City,
                            CompanyOrSoftwareID = q.CompanyOrSoftwareID,
                            CompanySoftwareNewsID = q.CompanySoftwareNewsID,
                            CountryID = q.CountryID,
                            CountryName = q.CountryName,
                            CreatedDate = q.CreatedDate,
                            ImageName = q.ImageName,
                            IsCompanySoftware = q.IsCompanySoftware,
                            isRelated = q.isRelated,
                            LogoName = q.LogoName,
                            NewsDescription = q.NewsDescription,
                            NewsTitle = q.NewsTitle,
                            SoftwareCompanyName = q.SoftwareCompanyName,
                            StateID = q.StateID,
                            StateName = q.StateName,
                            SubCategoryID = q.SubCategoryID,
                            subcategoryname = q.subcategoryname,
                            WebsiteURL = q.WebsiteURL,
                            YoutubeURL = q.YoutubeURL
                        }));
                    }
                    return OverviewNewsData;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
