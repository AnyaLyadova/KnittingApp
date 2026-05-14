using KnittingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KnittingApp.Controllers
{
    [Route("api/loopReader")]
    [ApiController]
    [Authorize]
    public class LoopReaderController : ControllerBase
    {
        //private readonly LoopsReader loopreader;
        private readonly ILoopsReaderService loopreaderService;

        public LoopReaderController(ILoopsReaderService loopreaderService)
        {
            this.loopreaderService = loopreaderService;
        }

        [HttpGet( "{id}")]

        public async Task<ActionResult<(List<string>, List<string>)>> GetCurrentString(Guid id)
        {

            var curString= await loopreaderService.GetCurrentString(id);
            return Ok( curString );
        }

        /*[HttpGet]

        public ActionResult<(List<string>, List<string>)> GetCurrentString()
        {
            return loopreader.GetCurrentString();
        }*/

        [HttpGet("{id}/progress")]
        public async Task<ActionResult<int>> GetProgress(Guid id)
        {
            var progress = await loopreaderService.GetProgress(id);
            return progress;
        }

        /*[HttpGet ("/progress")]
        public ActionResult<int> GetProgress()
        {
            return loopreader.GetProgress();
        }*/


        /*[HttpGet ("/time")]
        public ActionResult<TimeSpan> GetSpentTime()
        {
            return loopreader.GetSpentTime();
        }*/

        [HttpGet("{id}/time")]
        public async Task<ActionResult<TimeSpan>> GetSpentTime(Guid id)
        {
            var time = await loopreaderService.GetSpentTime(id);
            return time;
        }

        [HttpPost ("{id}/time")]
        public async Task<IActionResult> SetSpentTime(Guid id, [FromQuery]TimeSpan time)
        {
            await loopreaderService.SetSpentTime(id, time);
            return Ok();
        }
    }
}
