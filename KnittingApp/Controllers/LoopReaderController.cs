using Microsoft.AspNetCore.Mvc;

namespace KnittingApp.Controllers
{
    [Route("api/loopReader")]
    [ApiController]
    public class LoopReaderController : ControllerBase
    {
        private readonly LoopsReader loopreader;

        [HttpGet]

        public ActionResult<(List<string>, List<string>)> GetCurrentString()
        {
            return loopreader.GetCurrentString();
        }

        [HttpGet ("/progress")]
        public ActionResult<int> GetProgress()
        {
            return loopreader.GetProgress();
        }


        [HttpGet ("/time")]
        public ActionResult<TimeSpan> GetSpentTime()
        {
            return loopreader.GetSpentTime();
        }

        [HttpPost ("/time")]
        public IActionResult SetSpentTime(TimeSpan time)
        {
            loopreader.SetSpentTime(time);
            return Ok();
        }
    }
}
