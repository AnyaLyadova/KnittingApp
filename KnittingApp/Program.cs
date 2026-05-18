using KnittingApp;
using KnittingApp.Repository;
using KnittingApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



// настройка jwt


var secretKey = builder.Configuration["Jwt:SecretKey"]; // получаем секретный ключ из конфигурации

var key = Encoding.UTF8.GetBytes(secretKey); // создаем ключ для подписи токенов

builder.Services.AddAuthentication(options =>  // регистрируем аутентификацию с JWT
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Включить проверки
        ValidateIssuer = true,      // проверяем издателя токена
        ValidateAudience = true,    // проверяем получателя токена
        ValidateLifetime = true,    // проверяем срок действия
        ValidateIssuerSigningKey = true, // проверяем подпись

        // Указываем валидные значения
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),

        // Небольшой запас времени (опционально)
        ClockSkew = TimeSpan.FromMinutes(1)
    };

});

// регистрируем авторизацию
builder.Services.AddAuthorization();




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//регистрация репозиториев
builder.Services.AddScoped<IDraftRepository, DraftRepository>();
builder.Services.AddScoped<IFormRepository, FormRepository>();
builder.Services.AddScoped<ILoopMapRepository, LoopMapRepository>();
builder.Services.AddScoped<ILoopsReaderRepository,LoopsReaderRepository>();
builder.Services.AddScoped<IModelRepository, ModelRepository>();
builder.Services.AddScoped<ISchemaRepository, SchemaRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

//регистрация сервисов
builder.Services.AddScoped<Constructor>();
builder.Services.AddScoped<Account>();
//builder.Services.AddSingleton<SchemaConstructor>();

builder.Services.AddScoped<IDraftService, DraftService>();
builder.Services.AddScoped<IFormService, FormService>();
builder.Services.AddScoped<ILoopMapService, LoopMapService>();
builder.Services.AddScoped<ILoopsReaderService, LoopsReaderService>();
builder.Services.AddScoped<IModelService, ModelService>();
builder.Services.AddScoped<ISchemaService, SchemaService>();
builder.Services.AddScoped<IUserService, UserService>();



//регистрация бд и контекста

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
