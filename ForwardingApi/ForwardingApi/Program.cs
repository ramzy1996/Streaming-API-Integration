using ForwardingApi;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddSignalR(); // Register SignalR
//builder.Services.AddSingleton<SignalRClient>(); // Register the SignalR client
//builder.Services.AddHostedService<SignalRClient>();

//// Add services to the container.
//builder.Services.AddControllersWithViews();




builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy => policy
            .WithOrigins("http://127.0.0.1:5500")  // Allow only the frontend origin
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials() // Enables cookies, authentication, etc.
    );
});

builder.Services.AddScoped<IWebSocketManagerService, WebSocketManagerService>();

builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowSpecificOrigin");
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");


app.Run();
