using KnittingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KnittingApp.Controllers
{
    [Route("api/account")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly Account account;

        public AccountController(Account account)
        {
            this.account = account;
        }

        [HttpGet("{readerId}")]

        public async Task<ActionResult<(List<string>, List<string>)>> GetCurrentString(Guid readerId)
        {

            var curString = await account.GetCurrentStringById(readerId);
            return Ok(curString);
        }

        /*[HttpGet]

        public ActionResult<(List<string>, List<string>)> GetCurrentString()
        {
            return loopreader.GetCurrentString();
        }*/

        [HttpGet("{readerId}/progress")]
        public async Task<ActionResult<int>> GetProgress(Guid readerId)
        {
            var progress = await account.GetProgress(readerId);
            return progress;
        }


        [HttpGet("{readerId}/time")]
        public async Task<ActionResult<TimeSpan>> GetSpentTime(Guid readerId)
        {
            var time = await account.GetSpentTime(readerId);
            return time;
        }

        [HttpPost("{readerId}/time")]
        public async Task<IActionResult> SetSpentTime(Guid readerId, [FromQuery] TimeSpan time)
        {
            await account.SetSpentTime(readerId, time);
            return Ok();
        }

        [HttpGet ("/models")]

        public async Task<List<Model>> GetModels()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await account.GetModelsByUser(Guid.Parse(userId));
        }

        [HttpGet("/models/{modelId}")]
        public async Task<Model> GetModel(Guid modelId)
        {
            return await account.GetModel(modelId);
        }
    }
}
