using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/tabledefinitions")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class TableDefinitionsController : GenericApiController<Model.TableDefinition, Dto.TableDefinition>
    {
        public TableDefinitionsController(ApplicationDbContext context, ITableDefinitionsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet("data")]
        public virtual async Task<IActionResult> GetDownloadData()
        {
            ITableDefinitionsRepository repo = _itemsRepository as ITableDefinitionsRepository;
            var items = await repo.GetAllIncludingColumnDefinitionsAsync();

            return Ok(items.Select(i => _mapper.Map<Dto.TableDefinitionData>(i)));
        }

        [HttpGet("download")]
        public virtual async Task<IActionResult> Download()
        {
            ITableDefinitionsRepository repo = _itemsRepository as ITableDefinitionsRepository;
            var items = await repo.GetAllIncludingColumnDefinitionsAsync();
            var data = items.Select(i => _mapper.Map<Dto.TableDefinitionData>(i));

            //foreach(Dto.TableDefinitionData item in data)
            //{
            //    item.ColumnDefinitions = item.ColumnDefinitions.OrderBy(p => p.Position);
            //}

            var dataJson = JsonConvert.SerializeObject(data, Formatting.Indented);

            HttpContext.Response.ContentType = "application/text";
            FileContentResult result = new FileContentResult(Encoding.UTF8.GetBytes(dataJson), "application/json")
            {
                FileDownloadName = "table_definition.json"
            };

            return result;
        }

        [HttpPost("upload")]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null) throw new Exception("File is null");
            if (file.Length == 0) throw new Exception("File is empty");
            List<Dto.TableDefinitionData> tableDefinitions = null;

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;

                using (var sr = new StreamReader(stream))
                {
                    string json = sr.ReadToEnd();
                    tableDefinitions = JsonConvert.DeserializeObject<List<Dto.TableDefinitionData>>(json);
                }
            }

            foreach(Dto.TableDefinitionData tableDefinitionData in tableDefinitions)
            {
                Model.TableDefinition tableDefinition = _context.Set<Model.TableDefinition>().Where(t => t.Code == tableDefinitionData.TableDefinition.Code).SingleOrDefault();

                if (tableDefinition == null)
                {
                    tableDefinition = new Model.TableDefinition()
                    {
                        Code = tableDefinitionData.TableDefinition.Code
                    };
                    _context.Add(tableDefinition);
                }

                tableDefinition.Name = tableDefinitionData.TableDefinition.Name;
                tableDefinition.Description = tableDefinitionData.TableDefinition.Description;

                foreach(Dto.ColumnDefinitionBase columnDefinitionDto in tableDefinitionData.ColumnDefinitions)
                {
                    Model.ColumnDefinition columnDefinition = null;
                    bool newColumnDefinition = false;

                    if (tableDefinition.Id <= 0)
                    {
                        newColumnDefinition = true;
                    }
                    else
                    {
                        columnDefinition = _context.Set<Model.ColumnDefinition>()
                            .Where(c => c.TableDefinitionId == tableDefinition.Id && c.Property == columnDefinitionDto.Property).SingleOrDefault();

                        if (columnDefinition == null) newColumnDefinition = true;
                    }

                    if (newColumnDefinition)
                    {
                        columnDefinition = new Model.ColumnDefinition()
                        {
                            TableDefinition = tableDefinition,
                            Property = columnDefinitionDto.Property
                        };
                        _context.Add(columnDefinition);
                    }

                    columnDefinition.HeaderCode = columnDefinitionDto.HeaderCode;
                    columnDefinition.Active = columnDefinitionDto.Active;
                    columnDefinition.Pipe = columnDefinitionDto.Pipe;
                    columnDefinition.Format = columnDefinitionDto.Format;
                    columnDefinition.Include = columnDefinitionDto.Include;
                    columnDefinition.Position = columnDefinitionDto.Position;
                    columnDefinition.SortBy = columnDefinitionDto.SortBy;
                    columnDefinition.TextAlign = columnDefinitionDto.TextAlign;
                }
            }

            _context.SaveChanges();

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("allowAnonymous", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter)
        {
            List<Model.TableDefinition> items = null;
            IEnumerable<Dto.TableDefinition> itemsResult = null;

            items = (_itemsRepository as ITableDefinitionsRepository).GetByFilters(filter, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.TableDefinition>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as ITableDefinitionsRepository).GetCountByFilters(filter);
                var pagedResult = new Dto.PagedResult<Dto.TableDefinition>(itemsResult, new Dto.PagingInfo()
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
    }
}
