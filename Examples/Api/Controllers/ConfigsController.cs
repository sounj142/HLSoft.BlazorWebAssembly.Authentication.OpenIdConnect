using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("Configs")]
    public class ConfigsController : ControllerBase
    {
        public async Task<IActionResult> Get()
        {
            await Task.Delay(5000);
            return new JsonResult(new
            {
                Authority = "https://localhost:5000/",
                ClientId = "Client.Code.RemotelyConfig",
                ResponseType = "code",
                Scopes = new[] { "openid", "profile", "api" },
            });
        }
    }
}