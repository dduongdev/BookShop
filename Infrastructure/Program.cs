using Infrastructure.SqlServer.Repositories;
using Infrastructure.SqlServer.Repositories.SqlServer.DataContext;
using Infrastructure.SqlServer.Repositories.SqlServer.MapperProfile;
using Infrastructure.SqlServer.UnitOfWork;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using UseCases;
using UseCases.Repositories;
using UseCases.UnitOfWork;
using Infrastructure.Services;
using VNPay;
using Microsoft.AspNetCore.Authentication.Cookies;
using NuGet.Packaging.Signing;

namespace Infrastructure
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình xác thực (authentication) với CookieAuthentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/Authentication/Login";
                options.AccessDeniedPath = "/Authentication/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
            });

            // Đăng ký các dịch vụ hạ tầng của bạn
            RegisterInfrastructureServices(builder.Configuration, builder.Services);

            // Cấu hình Razor View Engine cho Areas và các Views chung
            builder.Services.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Clear();
                options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });

            // Thêm dịch vụ hỗ trợ cho Controllers và Views (MVC)
            builder.Services.AddControllersWithViews();

            // Thêm dịch vụ hỗ trợ Razor Pages
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Cấu hình pipeline yêu cầu HTTP
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // Cấu hình HSTS cho các môi trường không phải là Development
                app.UseHsts();
            }

            // Các bước cấu hình HTTPS
            app.UseHttpsRedirection();
            app.UseRouting();

            // Ánh xạ các Razor Pages
            app.MapRazorPages(); // Đây là bước để ánh xạ Razor Pages vào pipeline

            // Sử dụng xác thực (authorization)
            app.UseAuthorization();

            // Ánh xạ các static assets (hình ảnh, CSS, JS)
            app.MapStaticAssets();

            // Ánh xạ các controller route cho Areas và controller mặc định
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();  // Tải các static assets cho các controller routes

            // Chạy ứng dụng
            app.Run();
        }

        // Đăng ký các dịch vụ trong hạ tầng (Infrastructure)
        private static void RegisterInfrastructureServices(ConfigurationManager configuration, IServiceCollection services)
        {
            var repositoryOptions = configuration.GetSection("Repository").Get<RepositoryOptions>() ?? throw new Exception("No RepositoryOptions found.");
            if (repositoryOptions.RepositoryType == RepositoryTypes.SqlServer)
            {
                // Đăng ký AutoMapper để sử dụng cho các mapping giữa Entity và ViewModel
                services.AddAutoMapper(typeof(SqlServer2EntityProfile));

                // Cấu hình DbContext cho SQL Server với Lazy Loading
                services.AddDbContext<BookShopDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("IBookDb")).UseLazyLoadingProxies());

                // Đăng ký các Repository với SqlServer
                services.AddTransient<IBookRepository, SqlServerBookRepository>();
                services.AddTransient<ICartItemRepository, SqlServerCartItemRepository>();
                services.AddTransient<ICategoryRepository, SqlServerCategoryRepository>();
                services.AddTransient<IFeedbackRepository, SqlServerFeedbackRepository>();
                services.AddTransient<IOrderItemRepository, SqlServerOrderItemRepository>();
                services.AddTransient<IOrderRepository, SqlServerOrderRepository>();
                services.AddTransient<IPublisherRepository, SqlServerPublisherRepository>();
                services.AddTransient<IUserRepository, SqlServerUserRepository>();
                services.AddTransient<IPaymentTransactionRepository, SqlServerPaymentTransactionRepository>();

                // Đăng ký các UnitOfWork
                services.AddTransient<ICategoryUnitOfWork, SqlServerCategoryUnitOfWork>();
                services.AddTransient<IOrderUnitOfWork, SqlServerOrderUnitOfWork>();
                services.AddTransient<IPublisherUnitOfWork, SqlServerPublisherUnitOfWork>();
                services.AddTransient<IPaymentTransactionUnitOfWork, SqlServerPaymentTransactionUnitOfWork>();
            }
            else
            {
                throw new Exception("Cannot register infrastructure services.");
            }

            // Đăng ký các manager
            services.AddTransient<AuthenticationManager>();
            services.AddTransient<BookManager>();
            services.AddTransient<CartItemManager>();
            services.AddTransient<CategoryManager>();
            services.AddTransient<FeedbackManager>();
            services.AddTransient<OrderManager>();
            services.AddTransient<PublisherManager>();
            services.AddTransient<UserManager>();
            services.AddTransient<PaymentTransactionManager>();

            // Đăng ký các dịch vụ xử lý ảnh và các dịch vụ khác
            services.AddTransient<BookProcessingService>();
            services.AddTransient<BookMappingService>();
            services.AddTransient<ImageService>();
            services.AddTransient<FeedbackMappingService>();
            services.AddTransient<OrderMappingService>();

            // Đăng ký dịch vụ VNPay
            services.AddTransient<IVNPay, VNPayImpl>();
        }
    }
}
