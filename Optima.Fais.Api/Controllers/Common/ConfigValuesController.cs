using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/configvalues")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class ConfigValuesController : GenericApiController<Model.ConfigValue, Dto.ConfigValue>
    {
        public ConfigValuesController(ApplicationDbContext context, IConfigValuesRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string roleIds, string roleName, string filter)
        {
            List<Model.ConfigValue> items = null;
            IEnumerable<Dto.ConfigValue> itemsResult = null;
            List<string> rdIds = null;

            if (roleIds != null)
            {
                rdIds = new List<string>();
                rdIds.Add(roleIds);
            }

            items = (_itemsRepository as IConfigValuesRepository).GetByFilters(filter, "AspNetRole", rdIds, roleName, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.ConfigValue>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IConfigValuesRepository).GetCountByFilters(filter, rdIds, roleName);
                var pagedResult = new Dto.PagedResult<Dto.ConfigValue>(itemsResult, new Dto.PagingInfo()
                {
                    TotalItems = totalItems,
                    CurrentPage = page.Value,
                    PageSize = pageSize.Value
                });
                return Ok(pagedResult);
            }
            else
            {
                return Ok(itemsResult);
            }
        }

        [HttpGet("download")]
        public virtual async Task<IActionResult> Download()
        {
            IConfigValuesRepository repo = _itemsRepository as IConfigValuesRepository;
            //var items = await repo.GetExportDataAsync();
            var items = await repo.GetAllAsync(null, "", "");
            var data = items.Select(i => _mapper.Map<Dto.ConfigValueBase>(i));

            var dataJson = JsonConvert.SerializeObject(data, Formatting.Indented);

            HttpContext.Response.ContentType = "application/text";
            FileContentResult result = new FileContentResult(Encoding.UTF8.GetBytes(dataJson), "application/json")
            {
                FileDownloadName = "config_values.json"
            };

            return result;
        }

        [HttpPost("upload")]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null) throw new Exception("File is null");
            if (file.Length == 0) throw new Exception("File is empty");
            List<Dto.ConfigValueBase> configValuesDto = null;

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;

                using (var sr = new StreamReader(stream))
                {
                    string json = sr.ReadToEnd();
                    configValuesDto = JsonConvert.DeserializeObject<List<Dto.ConfigValueBase>>(json);
                }
            }

            foreach (Dto.ConfigValueBase configValueDto in configValuesDto)
            {
                string group = configValueDto.Group.ToUpper();
                string code = configValueDto.Code.ToUpper();

                Model.ConfigValue configValue = _context.Set<Model.ConfigValue>().Where(c => ((c.Group == group) && (c.Code == code))).SingleOrDefault();

                if (configValue == null)
                {
                    configValue = new Model.ConfigValue()
                    {
                        Group = group,
                        Code = code
                    };
                    _context.Add(configValue);
                }

                configValue.Description = configValueDto.Description;
                configValue.ValueType = configValueDto.ValueType;
                configValue.TextValue = configValueDto.TextValue;
                configValue.NumericValue = configValueDto.NumericValue;
                configValue.DateValue = configValueDto.DateValue;
                configValue.BoolValue = configValueDto.BoolValue;
                configValue.IsDeleted = configValueDto.IsDeleted;
            }

            _context.SaveChanges();

            return Ok();
        }
    }
}
