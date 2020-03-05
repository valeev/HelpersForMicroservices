using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Logging
{
    /// <summary>
    /// IAction result for custom validation errors response
    /// </summary>
    public class ValidationErrorsResult : IActionResult
    {
        /// <summary>
        /// Convert validation errors to proper response
        /// </summary>
        public async Task ExecuteResultAsync(ActionContext context)
        {
            var modelStateEntries = context.ModelState.Where(e => e.Value.Errors.Count > 0).ToArray();
            var errors = new List<ValidationError>();

            if (modelStateEntries.Any())
            {
                if (modelStateEntries.Length == 1 && modelStateEntries[0].Value.Errors.Count == 1 && modelStateEntries[0].Key == string.Empty)
                {
                    errors.Add(new ValidationError
                    {
                        Name = modelStateEntries[0].Key,
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
                                Name = modelStateEntry.Key,
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

            var result = new JsonResult(serviceError)
            {
                StatusCode = serviceError.StatusCode
            };
            await result.ExecuteResultAsync(context);
        }
    }
}
