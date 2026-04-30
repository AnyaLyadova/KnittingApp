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

        [HttpGet ("draft")]
        public Draft GetDraft()
        {
            return constructor.GetDraft();
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
        public LoopMap GetLoopMap()
        {
            return constructor.GetLoopMap();
        }


        [HttpPost ("move")]

        public ActionResult<Draft> MovePoint([FromBody] MovePointRequest request, [FromQuery] double newX, [FromQuery]double newY)
        {
            
            return Ok(constructor.MovePoint(request.movingPoint, newX, newY, request.leftPoint, request.rightPoint));
        }

        public class MovePointRequest
        {
            public Point movingPoint { get; set; }
            public Point leftPoint { get; set; }
            public Point rightPoint { get; set; }
        }


        [HttpPut("color")]

        public ActionResult<LoopMap> ColorLoopMap([FromBody] ColorLoopMapRequest request)
        {
            if (request.mIndexes == null || request.mIndexes == null || request.colors == null)
                return BadRequest("Переданые списки равны null");
            constructor.ColorLoopMap(request.mIndexes, request.nIndexes, request.colors);
            return Ok(constructor.GetLoopMap());
        }

        public class ColorLoopMapRequest
        {
            public List<int> mIndexes { get; set; } = new();
            public List<int> nIndexes { get; set; } = new();
            public List<string> colors { get; set; } = new();
        }
    }
}
