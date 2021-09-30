using Entities.Models;
using Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private EMSDBContext _dbContext;
        private IUserRoleRepository _userRole;
        private IUserRepository _user;
        private IPersonRepository _person;
        private IUserRefreshTokenRepository _userRefreshToken;
        private IModuleRepository _module;
        private IModuleControlRepository _moduleControl;
        private IPolicyRepository _policy;
        private IPolicyModuleControlRepository _policyModuleControl;
        private IEventRepository _event;
        private ICashAidRecepientRepository _cashAidRecepient;
        private IVoterRepository _voter;
        private IDistrictRepository _district;
        private IZoneRepository _zone;
        private IBarangayRepository _brgy;
        private IClusterRepository _cluster;
        private IPrecinctRepository _precinct;
        private ICashAidNonVoterRepository _cashAidNonVoter;
        private IActivationTokenRepository _activationToken;
        private IPollRepository _poll;
        private IPollOptionRepository _pollOption;


        public RepositoryWrapper(EMSDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Admin Modules

        public IUserRoleRepository UserRole
        {
            get
            {
                if (_userRole == null) { _userRole = new UserRoleRepository(_dbContext); }

                return _userRole;
            }
        }

        public IUserRepository User
        {
            get
            {
                if (_user == null) { _user = new UserRepository(_dbContext); }

                return _user;
            }
        }

        public IPersonRepository Person
        {
            get
            {
                if (_person == null) { _person = new PersonRepository(_dbContext); }

                return _person;
            }
        }

        public IUserRefreshTokenRepository UserRefreshToken
        {
            get
            {
                if (_userRefreshToken == null) { _userRefreshToken = new UserRefreshTokenRepository(_dbContext); }

                return _userRefreshToken;
            }
        }

        public IModuleRepository Module
        {
            get
            {
                if (_module == null) { _module = new ModuleRepository(_dbContext); }

                return _module;
            }
        }
        public IModuleControlRepository ModuleControl
        {
            get
            {
                if (_moduleControl == null) { _moduleControl = new ModuleControlRepository(_dbContext); }

                return _moduleControl;
            }
        }

    

        public IPolicyRepository Policy
        {
            get
            {
                if (_policy == null) { _policy = new PolicyRepository(_dbContext); }

                return _policy;
            }
        }
        public IPolicyModuleControlRepository PolicyModuleControl
        {
            get
            {
                if (_policyModuleControl == null) { _policyModuleControl = new PolicyModuleControlRepository(_dbContext); }

                return _policyModuleControl;
            }
        }


        public IVoterRepository Voter
        {
            get
            {
                if (_voter == null) { _voter = new VoterRepository(_dbContext); }

                return _voter;
            }
        }
        public IDistrictRepository District
        {
            get
            {
                if (_district == null) { _district = new DistrictRepository(_dbContext); }

                return _district;
            }
        }
        public IZoneRepository Zone
        {
            get
            {
                if (_zone == null) { _zone = new ZoneRepository(_dbContext); }

                return _zone;
            }
        }
        public IBarangayRepository Barangay
        {
            get
            {
                if (_brgy == null) { _brgy = new BarangayRepository(_dbContext); }

                return _brgy;
            }
        }
        public IClusterRepository Cluster
        {
            get
            {
                if (_cluster == null) { _cluster = new ClusterRepository(_dbContext); }

                return _cluster;
            }
        }
        public IPrecinctRepository Precinct
        {
            get
            {
                if (_precinct == null) { _precinct = new PrecinctRepository(_dbContext); }

                return _precinct;
            }
        }


        public IActivationTokenRepository ActivationToken
        {
            get
            {
                if (_activationToken == null) { _activationToken = new ActivationTokenRepository(_dbContext); }

                return _activationToken;
            }
        }
        #endregion

        #region Transactions

        public IEventRepository Event
        {
            get
            {
                if (_event == null) { _event = new EventRepository(_dbContext); }

                return _event;
            }
        }
        public ICashAidRecepientRepository CashAidRecepient
        {
            get
            {
                if (_cashAidRecepient == null) { _cashAidRecepient = new CashAidRecepientRepository(_dbContext); }

                return _cashAidRecepient;
            }
        }

        public ICashAidNonVoterRepository CashAidNonVoter
        {
            get
            {
                if (_cashAidNonVoter == null) { _cashAidNonVoter = new CashAidNonVoterRepository(_dbContext); }

                return _cashAidNonVoter;
            }
        }

        public IPollRepository Poll
        {
            get
            {
                if (_poll == null) { _poll = new PollRepository(_dbContext); }

                return _poll;
            }
        }

        public IPollOptionRepository PollOption
        {
            get
            {
                if (_pollOption == null) { _pollOption = new PollOptionRepository(_dbContext); }

                return _pollOption;
            }
        }

        #endregion

    }
}
