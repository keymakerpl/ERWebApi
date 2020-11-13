using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ERWebApi.ConfigOptions
{
    /// <summary>
    /// Configures the Swagger generation options.
    /// </summary>
    /// <remarks>This allows API versioning to define a Swagger document per API version after the
    /// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
        /// </summary>
        /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {
            // add a swagger document for each discovered API version
            // note: you might choose to skip or document deprecated API versions differently
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc($"ERServiceOpenAPISpecification_{description.GroupName}", CreateInfoForApiVersion(description));
            }

            // przypnij doc do wersji
            options.DocInclusionPredicate((documentName, apiDescription) =>
            {
                var actionApiVersionModel = apiDescription.ActionDescriptor
                .GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

                if (actionApiVersionModel == null)
                {
                    return true;
                }

                if (actionApiVersionModel.DeclaredApiVersions.Any())
                {
                    return actionApiVersionModel.DeclaredApiVersions.Any(v =>
                    CheckName(documentName, v));
                }

                return actionApiVersionModel.ImplementedApiVersions.Any(v =>
                    CheckName(documentName, v));
            });

            // niech szwgier wie, że api korzysta z jwt
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
            {
                In = ParameterLocation.Header,
                Description = "Please provide JWT with Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement 
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });

            // szwagier niech zrobi z komentarzy opisy do akcji API 
            var xmlComments = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlComments)); 
        }

        private static bool CheckName(string documentName, ApiVersion v)
        {
            var groupName = $"ERServiceOpenAPISpecification_v{v}";
            return groupName == documentName;
        }

        static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "ERService API",
                Version = description.ApiVersion.ToString(),
                Description = "ERService WEB API - Customers and Orders repository.",
                Contact = new OpenApiContact() { Name = "Radosław Kurek", Email = "serwis@erkomserwis.pl" },
                License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
