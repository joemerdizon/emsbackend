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
    public class ClusterController : BaseController
    {
        private readonly IMapper _mapper;
        public ClusterController(
            IRepositoryWrapper repoWrapper,
            ILogger<ClusterController> logger,
            IConfiguration configuration,
            IMapper mapper)
        {
            _repoWrapper = repoWrapper;
            _logger = logger;
            _configuration = configuration;
            TimeSpan expiryHours = TimeSpan.FromHours(int.Parse(_configuration["JWT:ExpiryHours"]));
            sessionTimeout = expiryHours.TotalMinutes;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetClustersByBaragayId/{brgyId}")]
        public async Task<ActionResult<ApiResponse>> GetClustersByBaragayId(int brgyId)
        {
            try
            {
                var clusters = await _repoWrapper.Cluster.GetAll();

                var clustersVm = clusters.Where(e => e.BrgyId == brgyId).Select(x => _mapper.Map<ClusterViewModel>(x)).ToList();

                return Ok(new ApiResponse { data = clustersVm, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("GetClustersByBaragayId:" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });

            }

        }

        [HttpGet]
        [Route("GetClusters/")]
        public async Task<ActionResult<ApiResponse>> GetClusters()
        {
            try
            {
                var clusters = await _repoWrapper.Cluster.GetAll();

                var clustersVm = clusters.Select(x => _mapper.Map<ClusterViewModel>(x)).ToList();

                return Ok(new ApiResponse { data = clustersVm, sessionTimeout = sessionTimeout, restrictions = await GetUserAuthorityRestrictions(GetCurrentRoleId()) });
            }
            catch (Exception ex)
            {
                _logger.LogError("GetClustersByBaragayId:" + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { data = null, errors = ex.Message, message = ex.InnerException.Message, sessionTimeout = sessionTimeout });
            }
        }
    }
}
