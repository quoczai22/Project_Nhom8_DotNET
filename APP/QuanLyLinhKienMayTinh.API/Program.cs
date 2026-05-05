
using Microsoft.EntityFrameworkCore;
using QuanLyLinhKienMayTinh.API.Models; 

namespace QuanLyLinhKienMayTinh.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<QL_LinhKien_PC_NETContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/api/payment/momo-ipn"))
                {
                    context.Request.EnableBuffering();
                    var body = await new System.IO.StreamReader(context.Request.Body).ReadToEndAsync();
                    context.Request.Body.Position = 0;
                    var folder = Path.Combine(AppContext.BaseDirectory, "MomoLogs");
                    Directory.CreateDirectory(folder);
                    await System.IO.File.WriteAllTextAsync(
                        Path.Combine(folder, "raw_request.txt"),
                        $"ContentType: {context.Request.ContentType}\n\nBody:\n{body}");
                }
                await next();
            });

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
