using AutoMapper;
using Entities.Models;
using Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS_Backend.AddOns.AutoMapper.Profiles
{
    public class PrecinctProfile: Profile
    {
        public PrecinctProfile()
        {
            CreateMap<Precinct, PrecinctViewModel>();
        }
    }
}
