using KnittingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Claims;
using System.Threading.Tasks;
using static KnittingApp.SharedConstants;

namespace KnittingApp.Controllers
{
    [Route("api/schema")]
    [ApiController]
    [Authorize]
    public class SchemaConstructorController : ControllerBase
    {
        private readonly SchemaConstructor schemaConstructor;
        private readonly ISchemaService schemaService;

        /*public SchemaConstructorController(SchemaConstructor schemaConstructor)
        {
            this.schemaConstructor = schemaConstructor;
        }*/

        public SchemaConstructorController(ISchemaService schemaService)
        {
            this.schemaService = schemaService;
        }

        /*[HttpGet ("{schemaId}")]
        public ActionResult<Schema> GetSchema(Guid schemaId)
        {
            try
            { return Ok(schemaConstructor.GetSchema(schemaId)); }
            catch(NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
        }*/

        [HttpGet("{schemaId}")]
        public async Task<ActionResult<Schema>> GetSchema(Guid schemaId)
        {
            try
            { return Ok(await schemaService.GetSchema(schemaId)); }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /*[HttpGet ("all")]
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
        }*/

        [HttpGet("all")]
        public async Task<ActionResult<List<Schema>>> GetAll()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Ok(await schemaService.GetAllSchemas(Guid.Parse(userId)));
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /*[HttpPut ("color")]
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
        }*/

        [HttpPut("{id}/color")]
        public async Task<IActionResult> ChangeLoopColor(Guid id,[FromQuery] int m, [FromQuery] int n, [FromQuery] string colorCode)
        {
            try
            {
                await schemaService.ChangeLoopColor(id, m, n, colorCode);
                return Ok();
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /*[HttpPut ("type")]
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
        }*/

        [HttpPut("{id}/type")]
        public async Task<IActionResult> ChangeLoopType(Guid id,[FromQuery] int m, [FromQuery] int n, [FromQuery] LoopType type)
        {
            try
            {
                await schemaService.ChangeLoopType(id, m,n, type);
                return Ok();
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /* [HttpPut ("color/{colorCode}")]

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
         }*/


        [HttpPut("{id}/color/{colorCode}")]

        public async Task<IActionResult> ColorSchema(Guid id,string colorCode)
        {
            try
            {
                await schemaService.ColorSchema(id,colorCode);
                return Ok();
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /*[HttpPost ("create")]

        public ActionResult<Schema> CreateSchema([FromQuery] int m, [FromQuery] int n, [FromQuery] string name)
        {
            return Ok(schemaConstructor.CreaterSchema(m, n, name));
        }*/

        [HttpPost("create")]

        public async Task<ActionResult<Schema>> CreateSchema([FromQuery] int m, [FromQuery] int n, [FromQuery] string name)
        {
            return Ok(await schemaService.CreateSchema(m,n, name));
        }

        /*[HttpPost("{schemaId}/image")]
        public async Task<IActionResult> UploadImage(Guid schemaId, IFormFile schemaImage)
        {
            if (schemaImage == null || schemaImage.Length == 0)
                return BadRequest("No image file");
            // Конвертируем IFormFile в Base64 строку
            var base64Image = await ConvertToBase64(schemaImage);
            schemaConstructor.SetSchemaImage(schemaId, base64Image);
            return Ok();
        }

*/

        [HttpPost("{schemaId}/image")]
        public async Task<IActionResult> UploadImage(Guid schemaId, IFormFile schemaImage)
        {
            if (schemaImage == null || schemaImage.Length == 0)
                return BadRequest("No image file");
            // Конвертируем IFormFile в Base64 строку
            var base64Image = await ConvertToBase64(schemaImage);
            await schemaService.SetSchemaImage(schemaId, base64Image);
            return Ok();
        }



        // Вспомогательный метод для конвертации в Base64
        private async Task<string> ConvertToBase64(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            return Convert.ToBase64String(bytes);
        }
    }
}
