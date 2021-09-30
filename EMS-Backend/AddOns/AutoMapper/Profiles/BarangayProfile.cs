using Entities.Models;
using Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoMapper.Profiles
{
    public class BarangayProfile: Profile
    {
        public BarangayProfile()
        {
            CreateMap<Barangay, BarangayViewModel>();
        }
    }
}
