using AutoMapper;
using ERWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace ERWebApi.Filters
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class CustomerResultFilterAttribute : ResultFilterAttribute
    {
        public override async Task OnResultExecutionAsync(
            ResultExecutingContext context,
            ResultExecutionDelegate next)
        {
            // zwracany rezultat z akcji
            var resultFromAction = context.Result as ObjectResult;
            
            // interesuje nas tylko status który coś zwraca np. CustomerDto
            if (resultFromAction?.Value == null
                || resultFromAction.StatusCode < 200
                || resultFromAction.StatusCode >= 300)
            {
                await next(); // do następnego filtra
                return; 
            }

            // mapper part
            var mapper = context.HttpContext.RequestServices.GetRequiredService<IMapper>();
            resultFromAction.Value = mapper.Map<CustomerDto>(resultFromAction.Value);

            await next(); // do następnego filtra
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
