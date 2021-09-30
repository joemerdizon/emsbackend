using Entities.Models;
using Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ModuleControlRepository : RepositoryBase<ModuleControl>, IModuleControlRepository
    {
        public ModuleControlRepository(EMSDBContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<ModuleControl>> GetAllByModuleId(int id)
        {
            return await _dbContext.ModuleControl.Where(x => x.ModuleId.Equals(id) && x.IsActive.Equals(true)).ToListAsync();
        }
        public async override Task<List<ModuleControl>> GetAll()
        {
            return await _dbContext.ModuleControl.Include(d => d.Module).OrderBy(x=>x.ModuleId).ThenBy(x=> x.ControlId).ToListAsync();
        }
    }
}
