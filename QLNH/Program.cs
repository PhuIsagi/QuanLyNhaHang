using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Rewrite;
using QLNH.DAL;
using QLNH.DAL.Repositories;
using QLNH.BLL;

namespace QLNH
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            ConfigureMiddleware(app);

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Core API Services
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddScoped<AuthRepository>();
            services.AddScoped<AuthService>();

            services.AddScoped<OrderRepository>();
            services.AddScoped<OrderService>();

            services.AddScoped<TableRepository>();
            services.AddScoped<TableService>();

            services.AddScoped<MenuRepository>();
            services.AddScoped<MenuService>();

            services.AddScoped<KitchenRepository>();
            services.AddScoped<KitchenService>();

            services.AddScoped<CashierRepository>();
            services.AddScoped<CashierService>();

            services.AddScoped<ManagerRepository>();
            services.AddScoped<ManagerService>();
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            var rewriteOptions = new RewriteOptions()
                .AddRewrite("^menu$", "layout/menu.html", skipRemainingRules: true)
                .AddRewrite("^table-list$", "layout/table_list.html", skipRemainingRules: true)
                .AddRewrite("^table_list$", "layout/table_list.html", skipRemainingRules: true)
                .AddRewrite("^order-list$", "layout/order_list.html", skipRemainingRules: true)
                .AddRewrite("^order_list$", "layout/order_list.html", skipRemainingRules: true)
                .AddRewrite("^kitchen$", "layout/kitchen.html", skipRemainingRules: true)
                .AddRewrite("^cashier$", "layout/cashier.html", skipRemainingRules: true)
                .AddRewrite("^manager$", "layout/manager.html", skipRemainingRules: true);

            app.UseRewriter(rewriteOptions);

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthorization();
            app.MapControllers();
        }
    }
}