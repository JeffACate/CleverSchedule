using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CleverScheduleProject.ActionFilters
{
    public class GlobalRouting : IActionFilter
    {
        private readonly ClaimsPrincipal _claimsPrincipal;
        public GlobalRouting(ClaimsPrincipal claimsPrincipal)
        {
            _claimsPrincipal = claimsPrincipal;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.RouteData.Values["controller"];
            if (controller.Equals("Home"))
            {
                if (_claimsPrincipal.IsInRole("Contractor"))
                {
                    context.Result = new RedirectToActionResult("Index", "Contractors", null);
                }
                else if (_claimsPrincipal.IsInRole("Client"))
                {
                    context.Result = new RedirectToActionResult("Index", "Clients", null);
                }
                else if (_claimsPrincipal.IsInRole("Admin"))
                {
                    context.Result = new RedirectToActionResult("Index", "Admins", null);
                }
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

    }
}
