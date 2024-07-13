
using TrackIt.Data;
using TrackIt.Repositories;
using TrackIt.Repository.Common;
using TrackIt.Repository;
using TrackIt.Service;
using TrackIt.Service.Common;
using TrackIt.Service.Common.Interface;
using TrackIt.WebAPI.AutoMapperProfiles;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "Access-Control-Allow-Origin",
        policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyOrigin();
        policy.AllowAnyMethod();

    });
});

builder.Services.AddAutoMapper(typeof(UserDtoProfile));
builder.Services.AddControllers();
builder.Services.AddSingleton<IUserRepository, UserRepository>()
    .AddTransient<IUserService, UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Authorization header using the JWT bearer scheme (\"bearer {token}\") ",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});


builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton<IClientRepository>(provider => new ClientRepository(connectionString));
builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddScoped<IPackageRepository>(provider => new PackageRepository(connectionString));
builder.Services.AddScoped<IPackageService, PackageService>();

builder.Services.AddSingleton<ISenderRepository>(provider => new SenderRepository(connectionString));
builder.Services.AddScoped<ISenderService, SenderService>();

builder.Services.AddSingleton<ICourierRepository>(provider => new CourierRepository(connectionString));
builder.Services.AddScoped<ICourierService, CourierService>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Access-Control-Allow-Origin");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();