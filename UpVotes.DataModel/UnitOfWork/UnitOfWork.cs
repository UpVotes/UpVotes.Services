using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using UpVotes.DataModel.Repository;

namespace UpVotes.DataModel.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Private Member Variables
        private UpVotesEntities _context = null;
        private Repository<Company> _companyRepository = null;
        private Repository<CompanyBranch> _companyBranchRepository = null;
        private Repository<CompanyFocus> _companyFocusRepository = null;
        private Repository<CompanyPortFolio> _companyPortFolioRepository = null;
        private Repository<CompanyReview> _companyReviewRepository = null;
        private Repository<CompanyReviewThankNote> _companyReviewThankNoteRepository = null;
        private Repository<CompanyVote> _companyVoteRepository = null;
        private Repository<Country> _countryRepository = null;
        private Repository<Currency> _currencyRepository = null;
        private Repository<FocusArea> _focusAreaRepository = null;
        private Repository<State> _stateRepository = null;
        private Repository<UserRegistration> _userRegistrationRepository = null;
        private Repository<User> _usersRepository = null;
        private Repository<UserToken> _userTokensRepository = null;
        private Repository<UserType> _userTypeRepository = null;

        private Repository<Sp_GetCompany_Result> _spGetCompanyResult = null;
        private Repository<Sp_GetCompanyBranches_Result> _spGetCompanyBranchesResult = null;
        private Repository<Sp_GetCompanyFocus_Result> _spGetCompanyFocusResult = null;
        private Repository<Sp_GetCompanyPortFolio_Result> _spGetCompanyPortFolioResult = null;
        private Repository<Sp_GetCompanyReviews_Result> _spGetCompanyReviewsResult = null;
        private Repository<Sp_GetCompanyReviewThankNotedUsers_Result> _spGetCompanyReviewThankNotedUsersResult = null;
        private Repository<Sp_GetFocusArea_Result> _spGetFocusAreaResult = null;

        #endregion

        #region public constructor to initialize connection string
        public UnitOfWork()
        {
            _context = new UpVotesEntities();
        }
        #endregion


        #region Public Repository Creation properties...
        /// <summary>
        /// Get/Set Property for Company repository.
        /// </summary>
        public Repository<Company> CompanyRepository
        {
            get
            {
                if (this._companyRepository == null)
                    this._companyRepository = new Repository<Company>(_context);
                return _companyRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for CompanyBranch repository.
        /// </summary>
        public Repository<CompanyBranch> CompanyBranchRepository
        {
            get
            {
                if (this._companyBranchRepository == null)
                    this._companyBranchRepository = new Repository<CompanyBranch>(_context);
                return _companyBranchRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for CompanyFocus repository.
        /// </summary>
        public Repository<CompanyFocus> CompanyFocusRepository
        {
            get
            {
                if (this._companyFocusRepository == null)
                    this._companyFocusRepository = new Repository<CompanyFocus>(_context);
                return _companyFocusRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for CompanyPortFolio repository.
        /// </summary>
        public Repository<CompanyPortFolio> CompanyPortFolioRepository
        {
            get
            {
                if (this._companyPortFolioRepository == null)
                    this._companyPortFolioRepository = new Repository<CompanyPortFolio>(_context);
                return _companyPortFolioRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for CompanyPortFolio repository.
        /// </summary>
        public Repository<CompanyReview> CompanyReviews
        {
            get
            {
                if (this._companyReviewRepository == null)
                    this._companyReviewRepository = new Repository<CompanyReview>(_context);
                return _companyReviewRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for CompanyPortFolio repository.
        /// </summary>
        public Repository<CompanyReviewThankNote> CompanyReviewThankNote
        {
            get
            {
                if (this._companyReviewThankNoteRepository == null)
                    this._companyReviewThankNoteRepository = new Repository<CompanyReviewThankNote>(_context);
                return _companyReviewThankNoteRepository;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Repository<CompanyVote> CompanyVoteRepository
        {
            get
            {
                if (this._companyVoteRepository == null)
                    this._companyVoteRepository = new Repository<CompanyVote>(_context);
                return this._companyVoteRepository;
            }
        }


        /// <summary>
        /// Get/Set Property for Country repository.
        /// </summary>
        public Repository<Country> CountryRepository
        {
            get
            {
                if (this._countryRepository == null)
                    this._countryRepository = new Repository<Country>(_context);
                return _countryRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for Currency repository.
        /// </summary>
        public Repository<Currency> CurrencyRepository
        {
            get
            {
                if (this._currencyRepository == null)
                    this._currencyRepository = new Repository<Currency>(_context);
                return _currencyRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for FocusArea repository.
        /// </summary>
        public Repository<FocusArea> FocusAreaRepository
        {
            get
            {
                if (this._focusAreaRepository == null)
                    this._focusAreaRepository = new Repository<FocusArea>(_context);
                return _focusAreaRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for State repository.
        /// </summary>
        public Repository<State> StateRepository
        {
            get
            {
                if (this._stateRepository == null)
                    this._stateRepository = new Repository<State>(_context);
                return _stateRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for UserRegistration repository.
        /// </summary>
        public Repository<UserRegistration> UserRegistrationRepository
        {
            get
            {
                if (this._userRegistrationRepository == null)
                    this._userRegistrationRepository = new Repository<UserRegistration>(_context);
                return _userRegistrationRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for Users repository.
        /// </summary>
        public Repository<User> UsersRepository
        {
            get
            {
                if (this._usersRepository == null)
                    this._usersRepository = new Repository<User>(_context);
                return _usersRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for UserTokens repository.
        /// </summary>
        public Repository<UserToken> UserTokensRepository
        {
            get
            {
                if (this._userTokensRepository == null)
                    this._userTokensRepository = new Repository<UserToken>(_context);
                return _userTokensRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for UserType repository.
        /// </summary>
        public Repository<UserType> UserTypeRepository
        {
            get
            {
                if (this._userTypeRepository == null)
                    this._userTypeRepository = new Repository<UserType>(_context);
                return _userTypeRepository;
            }
        }

        public Repository<Sp_GetCompany_Result> SpGetCompanyResults
        {
            get
            {
                if (this._spGetCompanyResult == null)
                    this._spGetCompanyResult = new Repository<Sp_GetCompany_Result>(_context);
                return _spGetCompanyResult;
            }
        }

        public Repository<Sp_GetCompanyBranches_Result> SpGetCompanyBranches
        {
            get
            {
                if (this._spGetCompanyBranchesResult == null)
                    this._spGetCompanyBranchesResult = new Repository<Sp_GetCompanyBranches_Result>(_context);
                return _spGetCompanyBranchesResult;
            }
        }

        public Repository<Sp_GetCompanyFocus_Result> SpGetCompanyFocus
        {
            get
            {
                if (this._spGetCompanyFocusResult == null)
                    this._spGetCompanyFocusResult = new Repository<Sp_GetCompanyFocus_Result>(_context);
                return _spGetCompanyFocusResult;
            }
        }

        public Repository<Sp_GetCompanyPortFolio_Result> SpGetCompanyPortFolio
        {
            get
            {
                if (this._spGetCompanyPortFolioResult == null)
                    this._spGetCompanyPortFolioResult = new Repository<Sp_GetCompanyPortFolio_Result>(_context);
                return _spGetCompanyPortFolioResult;
            }
        }

        public Repository<Sp_GetCompanyReviews_Result> SpGetCompanyReviews
        {
            get
            {
                if (this._spGetCompanyReviewsResult == null)
                    this._spGetCompanyReviewsResult = new Repository<Sp_GetCompanyReviews_Result>(_context);
                return _spGetCompanyReviewsResult;
            }
        }

        public Repository<Sp_GetCompanyReviewThankNotedUsers_Result> SpGetCompanyReviewThankNotedUsers
        {
            get
            {
                if (this._spGetCompanyReviewThankNotedUsersResult == null)
                    this._spGetCompanyReviewThankNotedUsersResult = new Repository<Sp_GetCompanyReviewThankNotedUsers_Result>(_context);
                return _spGetCompanyReviewThankNotedUsersResult;
            }
        }

        public Repository<Sp_GetFocusArea_Result> SpGetFocusArea
        {
            get
            {
                if (this._spGetFocusAreaResult == null)
                    this._spGetFocusAreaResult = new Repository<Sp_GetFocusArea_Result>(_context);
                return _spGetFocusAreaResult;
            }
        }
        #endregion

        #region Public member methods...
        /// <summary>
        /// Save method.
        /// </summary>
        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                var outputLines = new List<string>();
                foreach (var eve in e.EntityValidationErrors)
                {
                    outputLines.Add(string.Format("{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:", DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        outputLines.Add(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }
                System.IO.File.AppendAllLines(@"C:\errors.txt", outputLines);
                throw e;
            }
        }
        #endregion


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UnitOfWork() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
