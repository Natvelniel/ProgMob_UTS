using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgMob_UTS.API.Data;

/// <summary>
/// Titik masuk utama (entry point) aplikasi backend ASP.NET Core Web API.
/// Mengonfigurasi layanan database SQLite, validasi model, Swagger, dan routing controller.
/// </summary>
public class Program
{
    /// <summary>
    /// Menjalankan inisialisasi service container, migrasi database otomatis, dan HTTP request pipeline.
    /// </summary>
    /// <param name="args">Argumen baris perintah aplikasi.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Mengambil connection string SQLite dari appsettings.json (fallback ke academic.db jika kosong).
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                               ?? "Data Source=academic.db";

        // Mendaftarkan AcademicDbContext menggunakan provider SQLite.
        builder.Services.AddDbContext<AcademicDbContext>(options =>
            options.UseSqlite(connectionString));

        // Mendaftarkan controller dan menyelaraskan format pesan error validasi (400 Bad Request)
        // agar mudah dipahami oleh pengguna maupun aplikasi frontend.
        builder.Services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .Where(m => !string.IsNullOrWhiteSpace(m))
                        .Distinct()
                        .ToList();

                    var firstMessage = errors.FirstOrDefault() ?? "Data yang dikirim tidak valid.";
                    return new BadRequestObjectResult(new
                    {
                        message = firstMessage,
                        errors = errors
                    });
                };
            });

        // Mengaktifkan pembuatan dokumentasi OpenAPI / Swagger untuk pengujian endpoint.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Mengizinkan akses CORS agar frontend dapat berkomunikasi tanpa kendala lintas-origin.
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        // Menerapkan EF Core Migration secara otomatis saat aplikasi dijalankan
        // sehingga database dan data awal (seeding) langsung tersedia jika belum ada.
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AcademicDbContext>();
            dbContext.Database.Migrate();
        }

        // Mengaktifkan Swagger UI untuk pengujian API secara interaktif.
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistem Informasi Akademik API v1");
        });

        app.UseCors();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
