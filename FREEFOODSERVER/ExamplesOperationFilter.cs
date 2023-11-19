using FREEFOODSERVER.Controllers;
using FREEFOODSERVER.Models.Users;
using FREEFOODSERVER.Models.ViewModel;
using FREEFOODSERVER.Views.Admin;
using Microsoft.VisualBasic;
using Swashbuckle.AspNetCore.Filters;

namespace FREEFOODSERVER
{
    public class ExamplesOperationFilter : IMultipleExamplesProvider<LoginViewModel>
    {

        IEnumerable<SwaggerExample<LoginViewModel>> IMultipleExamplesProvider<LoginViewModel>.GetExamples()
        {
            yield return SwaggerExample.Create("Company", new LoginViewModel()
            {
                Email = "company@example.com",
                Password = "_Aa123456",
                
            });
            yield return SwaggerExample.Create("User", new LoginViewModel()
            {
                Email = "user@example.com",
                Password = "_Aa123456",

            });
            yield return SwaggerExample.Create("Admin", new LoginViewModel()
            {
                Email = "admin@example.com",
                Password = "_Aa123456",

            });
        }
    }
}
