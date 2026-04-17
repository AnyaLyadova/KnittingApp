using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static KnittingApp.SharedConstants;

namespace KnittingApp.Controllers
{
    [Route("api/schema")]
    [ApiController]
    public class SchemaConstructorController : ControllerBase
    {
        private readonly SchemaConstructor schemaConstructor;

        public SchemaConstructorController(SchemaConstructor schemaConstructor)
        {
            this.schemaConstructor = schemaConstructor;
        }

       [HttpGet ("{schemaId}")]
        public ActionResult<Schema> GetSchema(Guid schemaId)
        {
            try
            { return Ok(schemaConstructor.GetSchema(schemaId)); }
            catch(NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet ("all")]

        public ActionResult<List<Schema>> GetAll()
        {
            try
            {
                return Ok(schemaConstructor.GetAllSchemas());
            }
            catch(NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut ("color")]

        public IActionResult ChangeLoopColor([FromQuery] int m, [FromQuery] int n,[FromQuery] string colorCode)
        {
            try
            {
                schemaConstructor.ChangeLoopColor(m, n, colorCode);
                return Ok();
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut ("type")]
        public IActionResult ChangeLoopType([FromQuery] int m, [FromQuery] int n, [FromQuery] LoopType type)
        {
            try
            {
                schemaConstructor.ChangeLoopType(m, n, type);
                return Ok();
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut ("color/{colorCode}")]

        public IActionResult ColorSchema(string colorCode)
        {
            try
            {
                schemaConstructor.ColorSchema(colorCode);
                return Ok();
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost ("create")]

        public ActionResult<Schema> CreateSchema([FromQuery] int m, [FromQuery] int n, [FromQuery] string name)
        {
            return Ok(schemaConstructor.CreaterSchema(m, n, name));
        }
    }
}
