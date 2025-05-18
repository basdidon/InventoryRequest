using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Marten;
using Marten.Events.Projections;
using Weasel.Core;
using Api.Features.Users.Auth;
using Microsoft.AspNetCore.Identity;
using Api.Extensions;
using Api.Features.Users.Auth.RefreshToken;
using Api.Services;
using Api.Settings;
using Api.Features.Products;

var builder = WebApplication.CreateBuilder(args);

var signingKey = builder.Configuration.GetSection("jwt:signingKey").Value;
builder.Services.Configure<StorageSettings>(builder.Configuration.GetSection("Storage"));

builder.Services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
builder.Services.AddTransient<PasswordService>();
builder.Services.AddTransient<IImageService, ImageService>();

builder.Services.AddMarten(options =>
{
    // Establish the connection string to your Marten database
    options.Connection(builder.Configuration.GetConnectionString("Marten")!);

    options.Events.AddEventType<ProductCreated>();

    // Specify that we want to use STJ as our serializer
    options.UseSystemTextJsonForSerialization();

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }

    options.Projections.Add<ProductProjection>(ProjectionLifecycle.Inline);
    // Register a documents
    options.Schema.For<Product>();
    options.Schema.For<RefreshToken>()
    .Identity(x => x.Token);
})
    .UseLightweightSessions();

builder.Services
        .Configure<JwtCreationOptions>(o => o.SigningKey = signingKey!)
   .AddAuthenticationJwtBearer(s => s.SigningKey = signingKey!)
   .AddAuthorization()
   .AddFastEndpoints()
   .SwaggerDocument(o =>
   {
       o.MaxEndpointVersion = 1;
       o.DocumentSettings = s =>
       {
           s.DocumentName = "Initial Release";
           s.Title = "PriceScanner API";
           s.Version = "v1";
       };
   });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.SeedDb();
}

app.UseFastEndpoints(c =>
{
    c.Endpoints.Configurator = ep =>
    {
        ep.Options(b => b.AddEndpointFilter<EndpointRequestFilter>());
    };
    c.Endpoints.RoutePrefix = "api";
    c.Versioning.Prefix = "v";
    c.Versioning.PrependToRoute = true;
    c.Versioning.DefaultVersion = 1;
}).UseSwaggerGen();

app.Run();