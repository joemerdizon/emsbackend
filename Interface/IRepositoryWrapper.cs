using System;
using System.Collections.Generic;
using System.Text;

namespace Interface
{
    public interface IRepositoryWrapper
    {
        #region Admin Modules
        IUserRoleRepository UserRole { get; }

        IUserRepository User { get; }

        IPersonRepository Person { get; }

        IUserRefreshTokenRepository UserRefreshToken { get; }

        IModuleRepository Module { get; }

        IModuleControlRepository ModuleControl { get; }
     
        IPolicyRepository Policy { get; }

        IPolicyModuleControlRepository PolicyModuleControl { get; }

        IVoterRepository Voter { get; }
        IDistrictRepository District { get; }
        IZoneRepository Zone { get; }
        IBarangayRepository Barangay { get; }
        IClusterRepository Cluster { get; }
        IPrecinctRepository Precinct { get; }

        #endregion

        #region Transactions

        IEventRepository Event { get; }

        ICashAidRecepientRepository CashAidRecepient { get; }

        ICashAidNonVoterRepository CashAidNonVoter { get; }

        IActivationTokenRepository ActivationToken { get; }

        IPollRepository Poll { get; }

        IPollOptionRepository PollOption { get; }

        #endregion


    }
}
