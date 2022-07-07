using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<Data.AppointmentsDbContext>(config =>
    config.UseNpgsql(builder.Configuration.GetConnectionString("Default")));


builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(config =>
    {
        config.RequireHttpsMetadata = false;

        var keyInput = "ranDom_text_with_at_least_32_charecTersticS";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyInput));

        config.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "My api",
            ValidateAudience = true,
            ValidAudience = "My frontend",
            ValidateLifetime = true,
            IssuerSigningKey = key
        };
    }); // Tokern : JWT = json web token

// Add swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Enable CORS
builder.Services.AddCors(config => config.AddPolicy("Default" , policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Default");
// Authentication vs Authorization = Auth
// Authentication : who are you?
// Authorization : are you able to do this action?

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
