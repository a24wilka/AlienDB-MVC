using AlienDB_MVC.Data;

// Skapar webbapplikationen
var builder = WebApplication.CreateBuilder(args);

// Registrerar databasklassen för dependency injection
builder.Services.AddSingleton<Db>();

// Session timeout
builder.Services.AddSession(options =>
{
    // Loggar ut användaren efter 20 minuter
    options.IdleTimeout = TimeSpan.FromMinutes(20);

    // Gör session-cookie säkrare
    options.Cookie.HttpOnly = true;

    // Session-cookie är viktig för appen
    options.Cookie.IsEssential = true;
});

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

// Aktiverar sessioner
app.UseSession();

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