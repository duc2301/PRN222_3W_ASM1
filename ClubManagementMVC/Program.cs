using ClubManagement.Repository.Basic;
using ClubManagement.Repository.Basic.Interfaces;
using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Repositories;
using ClubManagement.Repository.Repositories.Interfaces;
using ClubManagement.Repository.UnitOfWork;
using ClubManagement.Repository.UnitOfWork.Interface;
using ClubManagement.Service;
using ClubManagement.Service.ServiceProviders;
using ClubManagement.Service.ServiceProviders.Interface;
using ClubManagement.Service.Services;
using ClubManagement.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ClubManagementContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServiceProviders, ServiceProviders>();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
builder.Services.AddScoped<IMembershipService, MembershipService>();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IJoinRequestRepository, JoinRequestRepository>();
builder.Services.AddScoped<IClubRepository, ClubRepository>();
builder.Services.AddScoped<IActivityParticipantRepository, ActivityParticipantRepository>();
builder.Services.AddScoped<IFeeRepository, FeeRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IJoinRequestService, JoinRequestService>();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



var app = builder.Build();


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


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");


app.Run();
