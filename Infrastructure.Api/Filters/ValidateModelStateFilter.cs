using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Api
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            var modelStateEntries = context.ModelState.Where(e => e.Value.Errors.Count > 0).ToArray();
            var errors = new List<ValidationError>();

            if (modelStateEntries.Any())
            {
                if (modelStateEntries.Length == 1 && modelStateEntries[0].Value.Errors.Count == 1 && modelStateEntries[0].Key == string.Empty)
                {
                    errors.Add(new ValidationError
                    {
                        Field = modelStateEntries[0].Key,
                        Description = modelStateEntries[0].Value.Errors[0].ErrorMessage
                    });
                }
                else
                {
                    foreach (var modelStateEntry in modelStateEntries)
                    {
                        foreach (var modelStateError in modelStateEntry.Value.Errors)
                        {
                            var error = new ValidationError
                            {
                                Field = modelStateEntry.Key,
                                Description = modelStateError.ErrorMessage
                            };
                            errors.Add(error);
                        }
                    }
                }
            }

            var serviceError = new ServiceValidationError
            {
                StatusCode = 400,
                Code = "validationError",
                Message = "Please, fix validation errors and try again",
                ValidationErrors = errors
            };

            context.Result = new JsonResult(serviceError)
            {
                StatusCode = serviceError.StatusCode
            };
        }
    }
}
