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
    public class PrecinctController : BaseController
    {
        private readonly IMapper _mapper;
        public PrecinctController(IRepositoryWrapper repoWrapper,
            ILogger<PrecinctController> logger,
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
        [Route("GetPrecinctsByClusterId/{clusterId}")]
        public async Task<ActionResult<ApiResponse>> GetPrecinctsByClusterId(int clusterId)
        {
            try
            {

                var precincts = await _repoWrapper.Precinct.GetAll();

                var precinctsVm = precincts.Where(e => e.ClusterId == clusterId).Select(x => _mapper.Map<PrecinctViewModel>(x)).ToList();

                return Ok(new ApiResponse { data = precinctsVm, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPrecinctsByClusterId:" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });

            }

        }

        [HttpGet]
        [Route("GetPrecincts/")]
        public async Task<ActionResult<ApiResponse>> GetPrecincts()
        {
            try
            {

                var precincts = await _repoWrapper.Precinct.GetAll();

                var precinctsVm = precincts.Select(x => _mapper.Map<PrecinctViewModel>(x)).ToList();

                return Ok(new ApiResponse { data = precinctsVm, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPrecinctsByClusterId:" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });

            }

        }
    }
}
