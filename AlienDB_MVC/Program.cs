using AlienDB_MVC.Data;

// Skapar webbapplikationen
var builder = WebApplication.CreateBuilder(args);

// Registrerar databasklassen för dependency injection
builder.Services.AddSingleton<Db>();

// Lägger till stöd för MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// Bygger applikationen
var app = builder.Build();

// =============================================
// Konfigurering av HTTP-pipeline
// =============================================

// Körs om applikationen INTE är i utvecklingsläge
if (!app.Environment.IsDevelopment())
{
    // Visar generell felsida vid undantag
    app.UseExceptionHandler("/Home/Error");

    // Aktiverar HSTS för säkrare HTTPS-anslutningar
    app.UseHsts();
}

// Tvingar HTTPS
app.UseHttpsRedirection();

// Aktiverar routing
app.UseRouting();

// Aktiverar authorization
app.UseAuthorization();

// Hanterar statiska filer (CSS, JS, bilder)
app.MapStaticAssets();

// Standardroute för MVC-applikationen
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Startar applikationen
app.Run();