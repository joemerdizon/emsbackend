using AutoMapper;
using EMS_Backend.Helper;
using Entities.ViewModel;
using Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS_Backend.Controllers
{
    [RoleAuthorize("superadmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class BarangayController : BaseController
    {
        private readonly IMapper _mapper;
        public BarangayController(IRepositoryWrapper repoWrapper, 
            ILogger<BarangayController> logger, 
            IConfiguration configuration,
            IMapper mapper
            )
        {
            _repoWrapper = repoWrapper;
            _logger = logger;
            _configuration = configuration;
            TimeSpan expiryHours = TimeSpan.FromHours(int.Parse(_configuration["JWT:ExpiryHours"]));
            sessionTimeout = expiryHours.TotalMinutes;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetBaragays()
        {
            try
            {

                var barangays = await _repoWrapper.Barangay.GetAll();

                var baragaysVm = barangays.Select(x => _mapper.Map<BarangayViewModel>(x)).ToList();

                return Ok(new ApiResponse { data = baragaysVm, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("GetBaragays:" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });

            }

        }
    }
}
