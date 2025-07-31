using Microsoft.AspNetCore.Mvc;
using Shopping.WebFramework.Extensions;
using Shopping.WebFramework.Filters;
using Shopping.WebFramework.Models;
using Shopping.WebFramework.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddLogging()
    .AddSwagger()
    .AddVersioning()
    .AddIdentityServices()
    .ConfigureAuthenticationAndAuthorization()
    .AddFileStorageServices()
    .AddApplicationAutoMapper()
    .AddApplicationMediatorServices()
    .RegisterApplicationValidator()
    .AddPersistenceDbContext();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<OkResultAttribute>();
    options.Filters.Add<NotFoundAttribute>();
    options.Filters.Add<ModelStateValidationAttribute>();
    options.Filters.Add<BadRequestAttribute>();
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ApiResult<Dictionary<string, List<string>>>),
        StatusCodes.Status400BadRequest));
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ApiResult),
        StatusCodes.Status401Unauthorized));
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ApiResult),
        StatusCodes.Status403Forbidden));
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ApiResult),
        StatusCodes.Status500InternalServerError));
}).ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
    options.SuppressMapClientErrors = true;
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    await app.ApplyMigrationsAsync();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();