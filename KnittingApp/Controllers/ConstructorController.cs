using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace KnittingApp.Controllers
{
    [ApiController]
    [Route("api/constructor")]
    
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

        [HttpGet( "model")]
        public ActionResult<Model> GetModel()
        {
            try
            { return Ok(constructor.GetModel()); }
            catch (NullReferenceException ex){ 
            return BadRequest(ex.Message);
            }
        }


        [HttpGet("model/{modelIndex}")]

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
        }

        [HttpGet ("frontDraft")]
        public Draft GetFrontDraft()
        {
            return constructor.GetFrontDraft();
        }

        [HttpGet("backDraft")]
        public Draft GetBackDraft()
        {
            return constructor.GetBackDraft();
        }

        [HttpGet("sleeveDraft")]
        public Draft GetSleeveDraft()
        {
            return constructor.GetSleeveDraft();
        }

        [HttpGet("createDrats")]
        public Draft CreateDrafts()
        {
            return constructor.CreateDrafts();
        }

        [HttpPost ("create")]
        public ActionResult<Model> CreateModel([FromQuery] string name,[FromQuery] List<string> stringParts/*, [FromBody] Dictionary<string, double> measures,
           [FromQuery] double height, [FromQuery] double width, [FromQuery] int loopInHeight, [FromQuery] int loopInWidth*/)
        {
            try
            { 
                return Ok(constructor.CreateNewModel(name, stringParts /*measures,*/ /*height, width, loopInHeight, loopInWidth*/)); 
            }
            catch (InvalidDataException ex) { 
                return BadRequest(ex.Message);
            }
        }

        [HttpGet ("models")]
        public ActionResult<List<Model>> GetModels()
        {
            return Ok(constructor.GetModels());
        }

        [HttpPut("measures/initialize")]
        public IActionResult InizializeModel([FromBody] Dictionary<string, double> measures,
            [FromQuery] double height, [FromQuery] double width,
            [FromQuery]  int loopInHeight, [FromQuery] int loopInWidth)
        {
            try
            {
                constructor.InitializeModel(measures, height, width, loopInHeight, loopInWidth);
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
        }

        [HttpPut("measures/{mKey}")]
        public void ChangeMeasure(string mKey, [FromQuery]double mValue)
        {
            constructor.ChangeMeasure(mKey, mValue);
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

        [HttpGet ("measures")]
        public Dictionary<string, double> GetAllMeasures()
        {
            return constructor.GetAllMeasures();
        }

        [HttpGet("loopMap")]
        public LoopMap GetLoopMap([FromQuery] string draftType)
        {
            return constructor.GetLoopMap(draftType);
        }


        [HttpPost ("move")]

        public ActionResult<Draft> MovePoint([FromBody] MovePointRequest request, [FromQuery] double newX, [FromQuery]double newY, [FromQuery] string draftType)
        {
            
            return Ok(constructor.MovePoint(request.movingPoint, newX, newY, request.leftPoint, request.rightPoint, draftType));
        }

        public class MovePointRequest
        {
            public Point movingPoint { get; set; }
            public Point leftPoint { get; set; }
            public Point rightPoint { get; set; }
        }


        [HttpPut("color")]

        public ActionResult<LoopMap> ColorLoopMap([FromBody] ColorLoopMapRequest request, [FromQuery] string draftType)
        {
            if (request.mIndexes == null || request.mIndexes == null || request.colors == null)
                return BadRequest("Переданые списки равны null");
            return  Ok(constructor.ColorLoopMap(request.mIndexes, request.nIndexes, request.colors, draftType));
        }

        [HttpPut("color/all")]

        public ActionResult<LoopMap> ColorAllLoopMap([FromQuery] string color,  [FromQuery] string draftType)
        {
            return Ok(constructor.ColorAllLoopMap(color,draftType));
        }

        public class ColorLoopMapRequest
        {
            public List<int> mIndexes { get; set; } = new();
            public List<int> nIndexes { get; set; } = new();
            public List<string> colors { get; set; } = new();
        }
    }
}
