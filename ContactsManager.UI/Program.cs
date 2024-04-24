using Serilog;
using CRUDExample;
using CRUDExample.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

builder.Services.CongifureServices(builder.Configuration);


var app = builder.Build();



//create application pipeline
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware();
}

app.UseHsts();
 app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseHttpLogging();

//app.Logger.LogDebug("debug-message");
//app.Logger.LogInformation("information-message");
//app.Logger.LogWarning("warning-message");
//app.Logger.LogError("error-message");
//app.Logger.LogCritical("critical-message");

if (builder.Environment.IsEnvironment("Test") == false)
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles();

app.UseRouting();//Identifying action method based on the route
app.UseAuthentication();//Reading identity cookie 
app.UseAuthorization();//Validate access permissions of the user
app.MapControllers();//Execute the filter pipeline(action + filters)

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name:"areas",
        pattern:"{area:exists}/{controller=Home}/{action=Index}"  
        //Admin/Home/Index
        //Admin
        );


});

app.Run();

public partial class Program { } //make the auto-generated Program accessible programmatically

