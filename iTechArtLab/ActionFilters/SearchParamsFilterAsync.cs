using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace iTechArtLab.ActionFilters
{
    public class SearchParamsFilterAsync : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var parameters = context.ActionArguments.ToList();
            var param = parameters.SingleOrDefault(p => p.Key == "genreFilter").Value;
            if (param==null || (int)(param) < 0)
            {
                context.Result = new BadRequestObjectResult("Genre filter is invalid");
                return;
            }

            param = parameters.SingleOrDefault(p => p.Key == "ageFilter").Value;
            if (param == null || (int)(param) < 0)
            {
                context.Result = new BadRequestObjectResult("Age filter is invalid");
                return;
            }

            param = parameters.SingleOrDefault(p => p.Key == "sortField").Value;
            if (param == null)
            {
                context.Result = new BadRequestObjectResult("Sort Field is null");
                return;
            }

            param = parameters.SingleOrDefault(p => p.Key == "elementsOnPage").Value;
            if (param == null || (int)(param) < 0)
            {
                context.Result = new BadRequestObjectResult("Elements On Page number is invalid");
                return;
            }
            await next();
            return;
        }
    }
}
