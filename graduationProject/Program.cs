using graduationProject.core.DbContext;
using graduationProject.Helpers;
using graduationProject.Models;
using graduationProject.Services;
using graduationProject.Services.Implementations;
using graduationProject.Services.Interfaces;
using graduationProject.Settings;
using Investor.BusinessLayer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Swagger mainly is used as the documentation for the api
    options.SwaggerDoc("v1", new OpenApiInfo //Open API Info Object, it provides the metadata about the Open API.
    {
        Version = "v1",
        Title = "Investment System",
        Description = "Investment System",
        //TermsOfService = new Uri("https://www.google.com"),
        License = new OpenApiLicense
        {
            Name = "License",
            Url = new Uri("https://github.com/Mahmoud-Mshrf/CRM")
        }

    });
    // Here we add authorization for all endpoints in one time
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Scheme = "Bearer",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter Your JWT Key",

    });
    // Here we add aauthorization on each endpoint 
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                        Name= "Bearer",
                        Reference = new OpenApiReference
                        {
                            Id= "Bearer",
                            Type = ReferenceType.SecurityScheme
                        },
                        In = ParameterLocation.Header,
                        BearerFormat="JWT",
                        Scheme="Bearer"
                    },
                    new List<string>()
                    }
                });
});// add DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var ConnectionStrings = builder.Configuration.GetConnectionString("local");
    options.UseSqlServer(ConnectionStrings);
});
//add identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// config identity

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = false;

});
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IMailingService, MailingService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();// Add IUserProfileService to the container
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICategoryService,CategoryService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IOfferService, OfferService>();
builder.Services.AddScoped<IuserService, userService>();
builder.Services.AddTransient<IFileHandling, FileHandling>();
builder.Services.AddSignalR();

//add Authentication
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme= JwtBearerDefaults .AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    })
    .AddJwtBearer(options=>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapHub<ChatHub>("/Chat");
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors(builder => builder
.AllowAnyHeader()
.AllowAnyMethod()
.AllowAnyOrigin()
);

app.Run();
