using TimeGhazi.Models; // Importerer prosjektets modeller
using TimeGhazi.Hubs; // Importerer SignalR-hub for sanntidsoppdateringer
using Microsoft.EntityFrameworkCore; // Gir tilgang til Entity Framework Core for databasehåndtering
using Microsoft.OpenApi.Models; // Brukes for å generere Swagger-dokumentasjon
using Microsoft.AspNetCore.Identity; // Gir tilgang til brukerhåndtering via Identity
using Microsoft.AspNetCore.Authentication.JwtBearer; // Brukes til å autentisere brukere med JWT
using Microsoft.IdentityModel.Tokens; // Brukes til å generere og validere JWT-tokens
using System.Text; // Gir tilgang til tekstbehandling

var builder = WebApplication.CreateBuilder(args);

// **Konfigurer JWT-autentisering**
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// **Konfigurer CORS (Cross-Origin Resource Sharing)**
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod() // Tillater alle HTTP-metoder (GET, POST, PUT, DELETE, etc.)
            .AllowAnyHeader()); // Tillater alle HTTP-headere
});

// **Legger til SignalR for sanntidskommunikasjon**
builder.Services.AddSignalR();

// **Legger til kontrollerstøtte (API-kontrollere)**
builder.Services.AddControllers();

// **Konfigurer Swagger for API-dokumentasjon**
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TimeGhazi", Version = "v1" });
});

// **Konfigurer SQLite database**
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// **Konfigurer ASP.NET Identity for autentisering og brukerhåndtering**
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false; // Krever ikke e-postbekreftelse ved registrering
    })
    .AddEntityFrameworkStores<ApplicationDbContext>() // Bruker Entity Framework for Identity-lagring
    .AddDefaultTokenProviders() // Legger til støtte for passordtilbakestilling, e-postbekreftelse, etc.
    .AddDefaultUI(); // Inkluderer standard UI for Identity (f.eks. login, register)

// **Registrer RoleManager slik at roller kan administreres**
builder.Services.AddScoped<RoleManager<IdentityRole>>();

// **Legger til støtte for MVC-visninger og Razor Pages**
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// **Aktiver CORS-policyen**
app.UseCors("AllowAll");

// **Konfigurer midlertidig rørledning avhengig av miljø**
if (app.Environment.IsDevelopment())
{
    // **Bruk Swagger i utviklingsmiljø**
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TimeGhazi API v1");
    });
}
else
{
    // **Bruk feilbehandling og HSTS (HTTP Strict Transport Security) i produksjon**
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// **Konfigurer HTTP-håndtering**
app.UseHttpsRedirection(); // **Tvinger HTTPS**
app.UseStaticFiles(); // **Gjør det mulig å servere statiske filer (CSS, JS, bilder, etc.)**
app.UseRouting(); // **Aktiverer ruting**
app.UseWebSockets();
app.UseAuthentication(); // **Aktiverer autentisering (brukerhåndtering)**
app.UseAuthorization(); // **Aktiverer autorisasjon (tilgangskontroll basert på roller)**

// **Kartlegger API-kontrollere og Razor Pages**
app.MapControllers();
app.MapRazorPages();
app.MapHub<ShiftHub>("/shiftHub"); // **Definerer SignalR-endepunkt for skiftoppdateringer**

// **Konfigurer standardruting for web-applikasjonen**
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Shift}/{action=Index}/{id?}");

// **Kjør DataSeeder for å opprette standard admin-bruker og roller**
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        await DataSeeder.SeedAdminUser(services); // **Oppretter admin-bruker og roller hvis de ikke finnes**
        logger.LogInformation("Admin user seeding completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the admin user."); // **Logger eventuelle feil**
    }
}

// **Starter applikasjonen**
app.Run();
