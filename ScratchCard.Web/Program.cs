using Minio;
using NPOI.POIFS.Crypt;
using ScratchCard.Web.Components;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

var services = builder.Services;

services.AddControllers();
services.AddSwaggerGen();
services.AddEndpointsApiExplorer();
//使用antDesign
services.AddAntDesign();

//使用minio
services.AddMinio(minio =>
    minio.WithEndpoint(config["MinioSettings:serverAddress"], int.Parse(config["MinioSettings:port"]))
    .WithCredentials(config["MinioSettings:AccessKey"], config["MinioSettings:SecretKey"])
    .WithSSL(bool.Parse(config["MinioSettings:Secure"]))
    .Build()
);

// Add services to the container.
services.AddRazorComponents()
.AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
