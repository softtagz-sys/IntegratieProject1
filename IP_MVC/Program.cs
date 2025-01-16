using System.Text.Json;
using BL;
using BL.Domain.Questions;
using BL.Implementations;
using BL.Interfaces;
using DAL;
using DAL.EF;
using DAL.Implementations;
using DAL.Interfaces;
using Google.Cloud.SecretManager.V1;
using IP_MVC;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddMvc().AddRazorPagesOptions(options=> {
    options.Conventions.AddAreaPageRoute("Identity", "/Account/Login",""); 
});

builder.Services.AddMvc().AddMvcOptions(options =>
{
    options.EnableEndpointRouting = false;
});

builder.Services.AddDbContext<PhygitalDbContext>(options =>
{
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "service-acc-key.json");
    try
    {
        var connectionString = $"Host={AccessSecret("DB_IP")}" + builder.Configuration.GetConnectionString("Connection") + AccessSecret("db_password") + ";";
        // var connectionString = builder.Configuration.GetConnectionString("LocalConnection");
        var testConnection = new NpgsqlConnection(connectionString);
        testConnection.Open();
        testConnection.Close();
        options.UseNpgsql(connectionString);
    }
    catch (NpgsqlException)
    {
        Console.WriteLine("Google Cloud database not available. Trying local database.");
        try
        {
            var localConnectionString = builder.Configuration.GetConnectionString("LocalConnection");
            var localTestConnection = new NpgsqlConnection(localConnectionString);
            localTestConnection.Open();
            localTestConnection.Close();
            options.UseNpgsql(localConnectionString);
        }
        catch (Exception e)
        {
            Console.WriteLine("No valid database available. Check database or connection string in appsettings.json.");
            Console.WriteLine(e.Message);
            Environment.Exit(1);
        }
    }
});

// Add Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<PhygitalDbContext>();

// Add session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add scoped
builder.Services.AddScoped<UnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRepository,Repository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IFlowRepository, FlowRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IOptionRepository, OptionRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<Manager<Question>>();
builder.Services.AddScoped<Manager<Option>>();
builder.Services.AddScoped<IProjectManager, ProjectManager>();
builder.Services.AddScoped<IQuestionManager, QuestionManager>();
builder.Services.AddScoped<IOptionManager, OptionManager>();
builder.Services.AddScoped<ISessionManager, SessionManager>();
builder.Services.AddScoped<IFlowManager, FlowManager>();
builder.Services.AddScoped<ICloudManager, CloudManager>();
builder.Services.AddScoped<ICloudStorageRepository, CloudStorageRepository>();
builder.Services.AddScoped<IAnswerManager, AnswerManager>();
builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
builder.Services.AddScoped<IAnalyticsManager, AnalyticsManager>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<INoteManager, NoteManager>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserManager, UserManager>();



// Add authorization
builder.Services.ConfigureApplicationCookie(cfg =>
{
    cfg.Events.OnRedirectToLogin += ctx =>
    {
        if (ctx.Request.Path.StartsWithSegments("/api"))
        {
            ctx.Response.StatusCode = 401;
        }

        return Task.CompletedTask;
    };

    cfg.Events.OnRedirectToAccessDenied += ctx =>
    {
        if (ctx.Request.Path.StartsWithSegments("/api"))
        {
            ctx.Response.StatusCode = 403;
        }

        return Task.CompletedTask;
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PhygitalDbContext>();
    if (context.CreateDatabase())
    {
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();
        await SeedIdentity(userManager, roleManager);
       
        // var dataSeeder = new DataSeeder(userManager);
        // await dataSeeder.Seed(context);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.UseMvc(routes =>
{
    routes.MapGet("/", (context) =>
    {
        context.Response.Redirect("/Identity/Account/Login", permanent: false);
        return System.Threading.Tasks.Task.CompletedTask;
    });
    
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{parentFlowId?}");

app.MapRazorPages();

app.Run();
return;

async Task SeedIdentity(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
{
    var defaultRole = new IdentityRole
    {
        Name = CustomIdentityConstants.DefaultRole
    };
    await roleManager.CreateAsync(defaultRole);
    var facilitatorRole = new IdentityRole
    {
        Name = CustomIdentityConstants.FacilitatorRole
    };
    await roleManager.CreateAsync(facilitatorRole);
    var adminRole = new IdentityRole
    {
        Name = CustomIdentityConstants.AdminRole
    };
    await roleManager.CreateAsync(adminRole);
    var platformAdminRole = new IdentityRole
    {
        Name = CustomIdentityConstants.PlatformAdminRole
    };
    await roleManager.CreateAsync(platformAdminRole);

    var adminUser = new IdentityUser
    {
        Email = "admin@kdg.be",
        UserName = "admin@kdg.be",
        EmailConfirmed = true
    };
    await userManager.CreateAsync(adminUser, "Password1!");
    await userManager.AddToRoleAsync(adminUser, CustomIdentityConstants.AdminRole);

    var facilitatorUser = new IdentityUser
    {
        Email = "fac@kdg.be",
        UserName = "fac@kdg.be",
        EmailConfirmed = true
    };
    await userManager.CreateAsync(facilitatorUser, "Password1!");
    await userManager.AddToRoleAsync(facilitatorUser, CustomIdentityConstants.FacilitatorRole);
    
    var platformAdminUser = new IdentityUser
    {
        Email = "pAdmin@kdg.be",
        UserName = "pAdmin@kdg.be",
        EmailConfirmed = true
    };
    await userManager.CreateAsync(platformAdminUser, "Password1!");
    await userManager.AddToRoleAsync(platformAdminUser, CustomIdentityConstants.PlatformAdminRole);
}

string AccessSecret(string secretId)
{
    // Read the JSON file
    string json = File.ReadAllText("service-acc-key.json");

    // Parse the JSON file
    JsonDocument doc = JsonDocument.Parse(json);

    // Extract the project_id value
    string projectId = doc.RootElement.GetProperty("project_id").GetString();
    
    // Create the Secret Manager client.
    SecretManagerServiceClient client = SecretManagerServiceClient.Create();
    
    // Use the project_id value
    SecretVersionName secretVersionName = new SecretVersionName(projectId, secretId, "latest");

    // Access the secret version.
    AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);

    // Get the secret payload and decode it.
    string payload = result.Payload.Data.ToStringUtf8();

    return payload;
}

//Do not delete
public partial class Program
{
}