using Entities.Models;
using Entities.ViewModel;
using Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Repository
{
    public class UserPrintersRepository : RepositoryBase<Printers>, IUserPrintersRepository
    {
        public UserPrintersRepository(CNIWMSContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Printers> UserPrintersList(int userId)
        {
            var res = await _dbContext.Printers.FindAsync(userId);
            return res;
        }

        public async Task<Printers> GetByUserIdByWhIdByPrinterType(int userid, int whId, int printerType)
        {
            return  _dbContext.Printers.Where(u => 
            //u.UserId.Equals(userid) && 
            u.WarehouseId.Equals(whId) && u.PrinterType.Equals(printerType)).FirstOrDefault();
        }
    }
}