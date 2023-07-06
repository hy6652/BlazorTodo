using BlazorTodo.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.Configure<CosmosDbServiceOptions>(builder.Configuration.GetSection("CosmosDb"));
builder.Services.AddSingleton<CosmosDbService>();

builder.Services.Configure<BlobImageServiceOptions>(builder.Configuration.GetSection("BlobServiceOptions"));
builder.Services.AddTransient<BlobImageServiceOptions>();

builder.Services.AddTransient<TodoService>();
builder.Services.AddTransient<BlobImageService>();
builder.Services.AddTransient<CsvService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
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
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
