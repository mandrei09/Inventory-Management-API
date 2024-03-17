using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/documenttypes")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class DocumentTypesController : GenericApiController<Model.DocumentType, Dto.DocumentType>
    {
        public DocumentTypesController(ApplicationDbContext context, IDocumentTypesRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet("filtered")]
        public IActionResult GetDocumentTypesByFilters(string parentCode)
        {
            IEnumerable<Model.DocumentType> items = (_itemsRepository as IDocumentTypesRepository).GetDocumentTypesByFilters(parentCode);

            return Ok(items.Select(i => _mapper.Map<Dto.DocumentType>(i)));
        }

        [HttpGet("prefix/{prefix}")]
        public IActionResult GetDocumentTypesByPrefix(string prefix)
        {
            IEnumerable<Model.DocumentType> items = (_itemsRepository as IDocumentTypesRepository).GetDocumentTypesByPrefix(prefix);

            return Ok(items.Select(i => _mapper.Map<Dto.DocumentType>(i)));
        }
    }
}
