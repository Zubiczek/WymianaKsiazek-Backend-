using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace WymianaKsiazek.Api.Controllers
{
    public class BaseController : Controller
    {
        public string CurrentUserId
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    var identity = User.Identity as ClaimsIdentity;

                    return identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                }

                return null;
            }
        }
    }
}
