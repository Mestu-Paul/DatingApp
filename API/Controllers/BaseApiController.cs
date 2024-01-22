using API.Data;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")] // routing to {base}/api/users
    public class BaseApiController : ControllerBase
    {
        
    }
}