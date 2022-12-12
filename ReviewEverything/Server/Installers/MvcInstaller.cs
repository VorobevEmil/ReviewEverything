using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using ReviewEverything.Server.Common.Filters;

namespace ReviewEverything.Server.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallerServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.WriteIndented = true;
                });
            builder.Services.AddRazorPages();
            builder.Services.AddMvc(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });
        }
    }
}