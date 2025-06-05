
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MovieApplicationApi.Helpers;
using MovieApplicationApi.Interfaces;
using MovieApplicationApi.Models;
using MovieApplicationApi.Repository;
using MovieApplicationApi.Services;
using MovieApplicationApi.Signatures;
using MovieApplicationApi.Validations;
using MovieApplicationApi.Data;
using MovieApplicationApi.Models.Entities;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IValidator<Genre>, GenreValidator>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<EncryptionHelper>();
builder.Services.AddScoped<IValidator<AddEmployeeDTO>, AddEmployeeValidator>();
builder.Services.AddScoped<IValidator<UpdateEmployeeDTO>, UpdateEmployeeValidator>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Enter Access Token",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme,Array.Empty<string>()}
    });
});
var key = builder.Configuration["JWTConfig:Key"];
if(string.IsNullOrEmpty(key))
{
    throw new ArgumentException("JWT configuration key is not set.");
}   
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata =true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWTConfig:Issuer"],
        ValidAudience = builder.Configuration["JWTConfig:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key))
    };

}
);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Your API", Version = "v1" });

    c.AddSecurityDefinition("X-Signature", new OpenApiSecurityScheme
    {
        Name = "X-Signature",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Custom signature for securing API calls"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "X-Signature"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddAuthorization();


builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<IDapperRepository, DapperRepository>();
builder.Services.AddScoped<ApiSignature>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbConnection"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();    
app.UseAuthorization();

app.MapControllers();


app.Run();
