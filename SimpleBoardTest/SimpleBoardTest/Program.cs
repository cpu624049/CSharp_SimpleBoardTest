using Microsoft.EntityFrameworkCore;
using SimpleBoardTest.Data;

var builder = WebApplication.CreateBuilder(args);

// DB 연결 설정
var connectionString = builder.Configuration.GetConnectionString("ShipDBContext") ?? throw new InvalidOperationException("Connection string 'ShipDBContext' not found.");
builder.Services.AddDbContext<ShipDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddControllersWithViews();

// 세션 서비스 추가
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
