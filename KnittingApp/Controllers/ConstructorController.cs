using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
namespace KnittingApp.Controllers
{
    [ApiController]
    [Route("api/constructor")]
    [Authorize]
    public class ConstructorController:ControllerBase
    {
        // [HttpGet ("/base")]
        /*public Draft CreateBaseDraft()
        {

        }*/
        private readonly Constructor constructor;
        public
            ConstructorController(Constructor constructor)
        {
            this.constructor = constructor;
        }

        [HttpGet( "model/{modelId}")]
        public async Task<ActionResult<Model>> GetModel(Guid modelId)
        {
            try
            { return Ok(await constructor.GetModel(modelId)); }
            catch (NullReferenceException ex){ 
            return BadRequest(ex.Message);
            }
        }


        /*[HttpGet("model/{modelIndex}")]

        public ActionResult<Model> ChooseModel(Guid modelIndex)
        {
            try
            {
                return Ok(constructor.ChooseModel(modelIndex));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }*/


        [HttpGet("form/{formId}")]

        public async Task<ActionResult<Dictionary<string, double>>> ChooseForm(Guid formId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                return Ok(await constructor.ChooseForm(formId, Guid.Parse(userId)));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet ("{modelId}/frontDraft")]
        public async Task<Draft> GetFrontDraft(Guid modelId)
        {
            return await constructor.GetFrontDraft(modelId);
        }

        [HttpGet("{modelId}/backDraft")]
        public async Task<Draft> GetBackDraft(Guid modelId)
        {
            return await constructor.GetBackDraft(modelId);
        }

        [HttpGet("{modelId}/sleeveDraft")]
        public async Task<Draft> GetSleeveDraft(Guid modelId)
        {
            return await constructor.GetSleeveDraft(modelId);
        }

        [HttpGet("{modelId}/createDrats")]
        public async Task<Draft> CreateDrafts(Guid modelId)
        {
            return await constructor.CreateDrafts(modelId);
        }

        [HttpPost ("create")]
        /*public ActionResult<Model> CreateModel([FromQuery] string name,[FromQuery] List<string> stringParts*//*, [FromBody] Dictionary<string, double> measures,
           [FromQuery] double height, [FromQuery] double width, [FromQuery] int loopInHeight, [FromQuery] int loopInWidth*//*)
        {
            try
            { 
                return Ok(constructor.CreateNewModel(name, stringParts *//*measures,*/ /*height, width, loopInHeight, loopInWidth*//*)); 
            }
            catch (InvalidDataException ex) { 
                return BadRequest(ex.Message);
            }
        }*/

        public async Task<ActionResult<Model>> CreateForm([FromQuery] string name, [FromQuery] List<string> stringParts)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Ok(await constructor.CreateNewForm(Guid.Parse(userId),name, stringParts ));
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet ("models")]
        public async Task<ActionResult<List<Model>>> GetModels()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await constructor.GetModels(Guid.Parse(userId)));
        }

        [HttpGet("forms")]
        public async Task<ActionResult<List<Form>>> GetForms()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await constructor.GetForms(Guid.Parse(userId)));
        }

        /*[HttpPut("{modelId}/measures/initialize")]
        public async Task<IActionResult> InizializeModel(Guid modelId,[FromBody] Dictionary<string, double> measures,
            [FromQuery] double height, [FromQuery] double width,
            [FromQuery]  int loopInHeight, [FromQuery] int loopInWidth)
        {
            try
            {
                await constructor.InitializeModel(modelId,measures, height, width, loopInHeight, loopInWidth);
                return Ok();
            }
            catch(NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
        }*/


        [HttpPut("model/{formId}/initialize")]
        public async Task<ActionResult<Model>> InizializeModel(Guid formId, [FromQuery]string modelName, [FromBody] Dictionary<string, double> measures,
            [FromQuery] double height, [FromQuery] double width,
            [FromQuery] int loopInHeight, [FromQuery] int loopInWidth)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var model=await constructor.CreateModelWithDrafts(formId,Guid.Parse(userId), modelName, measures, height, width,
                    loopInHeight, loopInWidth);
                return Ok(model);
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{modelId}/measures/{mKey}")]
        public async Task ChangeMeasure(Guid modelId,string mKey, [FromQuery]double mValue)
        {
            await constructor.ChangeMeasure(modelId, mKey, mValue);
        }

        [HttpGet ("neckparts")]
        public Dictionary <string, string> GetNeckParts()
        {
            return constructor.GetNeckParts();
        }

        [HttpGet ("armholeparts")]
        public Dictionary<string, string> GetArmholeParts()
        {
            return constructor.GetArmholeParts();
        }

        [HttpGet ("sleeverollparts")]
        public Dictionary<string, string> GetSleeveRollParts()
        {
            return constructor.GetSleeveRollParts();
        }

        [HttpGet ("{modelId}/measures")]
        public async Task<Dictionary<string, double>> GetAllMeasures(Guid modelId)
        {
            return await constructor.GetAllMeasures(modelId);
        }

        [HttpGet("{modelId}/loopMap")]
        public async Task<LoopMap> GetLoopMap(Guid modelId,[FromQuery] string draftType)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await constructor.GetLoopMap(modelId, draftType);
        }


        [HttpPost ("{draftId}/move")]

        public ActionResult<Draft> MovePoint(Guid draftId,[FromBody] MovePointRequest request, [FromQuery] double newX, [FromQuery]double newY, 
           [FromQuery] double loopWidth, [FromQuery]double loopHeight, [FromQuery]double loopInWidth, [FromQuery]double loopInHeight)
        {
            
            return Ok(constructor.MovePoint(draftId,request.movingPoint, newX, newY, request.leftPoint, request.rightPoint,
                 loopWidth, loopHeight, loopInWidth, loopInHeight));
        }

        public class MovePointRequest
        {
            public Point movingPoint { get; set; }
            public Point leftPoint { get; set; }
            public Point rightPoint { get; set; }
        }


        [HttpPut("{modelId}/color")]

        public ActionResult<LoopMap> ColorLoopMap(Guid modelId, [FromBody] ColorLoopMapRequest request, [FromQuery] string draftType)
        {
            if (request.mIndexes == null || request.mIndexes == null || request.colors == null)
                return BadRequest("Переданые списки равны null");
            return  Ok(constructor.ColorLoopMap(modelId,request.mIndexes, request.nIndexes, request.colors, draftType));
        }

        [HttpPut("{modelId}/color/all")]

        public ActionResult<LoopMap> ColorAllLoopMap(Guid modelId,[FromQuery] string color,  [FromQuery] string draftType)
        {
            return Ok(constructor.ColorAllLoopMap(modelId,color,draftType));
        }

        public class ColorLoopMapRequest
        {
            public List<int> mIndexes { get; set; } = new();
            public List<int> nIndexes { get; set; } = new();
            public List<string> colors { get; set; } = new();
        }
    }
}
