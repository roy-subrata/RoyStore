using Client.Web.Components;
using Client.Shared.Services;
using Client.Web.Services;
using MudBlazor.Services;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the Client.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddMudServices();


builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseAddress"] ?? throw new InvalidOperationException("API base address is not configured."));
});

builder.Services.AddLocalization();

var supportedCultures = new[] { "en", "bn" };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});

builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<PurchaseService>();
builder.Services.AddScoped<FeatureService>();

var app = builder.Build();
// Must be before other middleware that uses localization

app.UseRequestLocalization();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Client.Shared._Imports).Assembly);

app.UseRouting();
app.UseAntiforgery();

app.MapGet("/Culture/set", async (HttpContext context) =>
{
    var culture = context.Request.Query["culture"].ToString();
    var redirectUri = context.Request.Query["redirectUri"].ToString();

    if (!string.IsNullOrEmpty(culture))
    {
        context.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { IsEssential = true, Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );
    }

    context.Response.Redirect(string.IsNullOrEmpty(redirectUri) ? "/" : redirectUri);
    await Task.CompletedTask;
});

app.Run();
