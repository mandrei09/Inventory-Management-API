using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.EfRepository;
using Optima.Fais.Model;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
	/// <summary>
	/// Identity Web API controller.
	/// </summary>
	[Authorize(Policy = "Manage Accounts")] // Authorization policy for this API.
	[Route("api/roles")]
	public class RoleController : Controller
	{
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly ILogger _logger;
		private readonly ApplicationDbContext _context;
		private readonly IMapper mapper;

		public RoleController(
			RoleManager<ApplicationRole> roleManager,
			ILoggerFactory loggerFactory,
			ApplicationDbContext context,
			IMapper mapper)
		{
			_roleManager = roleManager;
			_logger = loggerFactory.CreateLogger<IdentityController>();
			_context = context;
			this.mapper = mapper;
		}

		//[
		//
		//]
		[AllowAnonymous]
		public async Task<IActionResult> GetAllRoles(string sortColumn, string sortDirection, int? page, int? pageSize)
		{

			var roles = _roleManager.Roles;

			int totalItems = roles.Count();

			var allRoles = roles.ToList();


			var rolesMap = mapper.Map<List<ApplicationRole>, List<Dto.Role>>(roles.ToList());

			var pagedResult = new Dto.PagedResult<Dto.Role>(rolesMap, new Dto.PagingInfo()
			{
				TotalItems = totalItems,
				CurrentPage = page.Value,
				PageSize = pageSize.Value
			});

			return Ok(pagedResult);

			//return new JsonResult(usersMap);
		}


		[HttpGet("role/{id}")]
		public async Task<IActionResult> GetRoleById(string id)
		{

			var role = await _roleManager.FindByIdAsync(id);

			return Ok(role);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetRoleByStringId(string id)
		{

			var role = await _roleManager.FindByIdAsync(id);

			return Ok(role);
		}

	}
}