using Microsoft.AspNetCore.Antiforgery;
using CRM.Controllers;

namespace CRM.Web.Host.Controllers
{
    public class AntiForgeryController : CRMControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
