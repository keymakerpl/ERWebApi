using System;
using System.Text;
using AutoMapper;
using ERService.Infrastructure.Helpers;
using ERWebApi.ConfigOptions;
using ERWebApi.SQLDataAccess;
using ERWebApi.SQLDataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ERWebApi
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => 
                options.ReturnHttpNotAcceptable = true
            )
                .ConfigureApiBehaviorOptions(options =>   
                {
                    //dodaj dodatkowe informacje do zwracanych b³êdów walidacji
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        //utworz obiekt tworzacy walidacje
                        var problemDetailsFactory = context.HttpContext.RequestServices
                        .GetRequiredService<ProblemDetailsFactory>();
                        var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                            context.HttpContext, context.ModelState);

                        //dodaj dodatkowe informacje
                        problemDetails.Detail = "See the errors field for details.";
                        problemDetails.Instance = context.HttpContext.Request.Path;

                        //szukja kodu b³êdu którego u¿yæ
                        var actionExecutingContext = context as ActionExecutingContext;

                        //sprawdzamy czy s¹ b³êdy ModelState i dodajemy
                        if ((context.ModelState.ErrorCount > 0)
                            && (actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
                        {
                            problemDetails.Type = "https://courselibrary.com/modelvalidationproblems";
                            problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                            problemDetails.Title = "Validation errors accurred";

                            return new UnprocessableEntityObjectResult(problemDetails)
                            {
                                ContentTypes = { "application/problem+json" } //tak to te¿ jest standard
                            };
                        }

                        //jezeli brak bledow w modelstate to zwracamy standardowy validation error
                        problemDetails.Status = StatusCodes.Status400BadRequest;
                        problemDetails.Title = "Validation errors accurred";

                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" } //tak to te¿ jest standard
                        };
                    };
                })
                 .AddNewtonsoftJson(options =>
                 {
                     options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                 })
                 //dodaj serializer xml, ACCEPT: xml w Header;
                 .AddXmlDataContractSerializerFormatters();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());            

            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddDbContext<ERWebApiDbContext>(options =>
            {
                options.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=ERWebApiDB;Trusted_Connection=True;");
            });
            services.AddScoped<ICustomersRepository, CustomersRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // authorization part
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            // api versioning
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VV"; // konwencja po jakiej wybierany jest numer wersji
            });
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                //options.ApiVersionReader = new HeaderApiVersionReader("api-version"); // sprawdzaj jakiej u¿yæ wersji api z headera
                //options.ApiVersionReader = new MediaTypeApiVersionReader(); // sprawdzaj jakiej u¿yæ wersji api z media type
                // domyœlnie api version sprawdzany jest w route np /api/v1/customers
            });

            // swagger part            
            // dodaj doc dla ka¿dej wersji api
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(ApplicationBuilder =>
                {
                    ApplicationBuilder.Run(async ctx =>
                    {
                        ctx.Response.StatusCode = 500;
                        await ctx.Response.WriteAsync("Upssss...... something is wrong. Try again later :(");
                    });
                });
            }

            // utworz instancje DbContext i odpal seeda
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetService<IDbInitializer>();
                dbInitializer.Initialize();
                dbInitializer.SeedData();
            }

            // middleware part            
            // szwagier w middleware i ³adny szwagier
            app.UseSwagger();
            app.UseSwaggerUI(options => 
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/ERServiceOpenAPISpecification_{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }

                options.RoutePrefix = "";
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
