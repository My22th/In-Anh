using Amazon.Runtime;
using Google.Api;
using In_Anh.Models;
using In_Anh.Models.MongoModel;
using In_Anh.RabitMQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = long.MaxValue;
});

builder.Services.AddSingleton<IImageMgDatabase>(provider =>
    provider.GetRequiredService<IOptions<ImageMgDatabase>>().Value);

builder.Services.AddScoped<IRabitMQProducer, RabitMQProducer>();

builder.Services.Configure<ImageMgDatabase>(
    builder.Configuration.GetSection("ImageMgDatabase"));
builder.Services.AddScoped<IImageMgDatabase, ImageMgDatabase>();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});
//AWSOptions awsOptions = new AWSOptions
//{
//    Credentials = new BasicAWSCredentials("AKIAVUBFKDVE4NQYSNMS", "NK2NjfE/hs/TYJAsfBAo7/6vdsui8s8Bb90CHHSQ"),
//    Region = Amazon.RegionEndpoint.APSoutheast2
//};
//builder.Services.AddDefaultAWSOptions(awsOptions);
//builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
   {
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidIssuer = builder.Configuration["Jwt:Issuer"],
           ValidAudience = builder.Configuration["Jwt:Issuer"],
           IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = false,
           ValidateIssuerSigningKey = true
       };
   });


builder.Services.AddHostedService<RabitMQConsumer>();
var app = builder.Build();
//app.UseDeveloperExceptionPage();
//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}
app.UseDeveloperExceptionPage();
          
app.UseStaticFiles(new StaticFileOptions()
{
    HttpsCompression = Microsoft.AspNetCore.Http.Features.HttpsCompressionMode.Compress,
    OnPrepareResponse = (context) =>
    {
        var headers = context.Context.Response.GetTypedHeaders();
        headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromDays(30)
        };
    }
});

app.UseSession();
app.UseHttpsRedirection();

app.UseResponseCompression();
app.UseRouting();
app.UseCors("AllPolicy");
app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
