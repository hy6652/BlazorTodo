using BlazorTodo.Server.Services.Blob;
using BlazorTodo.Server.Services.Cosmos;
using BlazorTodo.Server.Services.DIexample;
using BlazorTodo.Server.Services.DIexampleWithCommonModel;
using BlazorTodo.Server.Services.Search;
using BlazorTodo.Server.Services.SignalR;
using BlazorTodo.Server.Services.Todo;
using BlazorTodo.Server.Services.Utility;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

// Swashbuckle
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Todo",
        Description = "ASP.NET Core app for Todo"
    });

    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});

// NSwag
//builder.Services.AddOpenApiDocument(options =>
//{
//    options.PostProcess = document =>
//    {
//        document.Info = new NSwag.OpenApiInfo
//        {
//            Version = "v1",
//            Title = "Todo API",
//            Description = "ASP.NET Core app for Todo"
//        };
//    };
//});

// options
builder.Services.Configure<CosmosDbServiceOptions>(builder.Configuration.GetSection("CosmosDb"));
builder.Services.AddSingleton<CosmosDbService>();

builder.Services.Configure<BlobImageServiceOptions>(builder.Configuration.GetSection("BlobServiceOptions"));
builder.Services.AddTransient<BlobImageServiceOptions>();

builder.Services.Configure<BlobTestServiceOptions>(builder.Configuration.GetSection("BlobForTest"));
builder.Services.AddTransient<BlobTestService>();

builder.Services.Configure<CognitiveSearchServiceOption>(builder.Configuration.GetSection("CognitiveForTest"));
builder.Services.AddTransient<CognitiveSearchService>();

// Service.cs
builder.Services.AddTransient<TodoService>();
builder.Services.AddTransient<BlobImageService>();
builder.Services.AddTransient<CsvService>();
builder.Services.AddTransient<RegexService>();
builder.Services.AddTransient<ChatHub>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient(typeof(ICommonRepository<>), typeof(CommonRepository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    // Swashbuclke
    app.UseSwagger();
    app.UseSwaggerUI();

    // ±âº»ÀÌ api-docs
    app.UseReDoc(c =>
    {
        c.RoutePrefix = "docs";
        //c.SpecUrl("/v1/swagger.json");
    });

    //NSwag
    //app.UseOpenApi();
    //app.UseSwaggerUi3();
    //app.UseReDoc(options =>
    //{
    //    options.Path = "/redoc";
    //});
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapHub<ChatHub>("/Chat");
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
