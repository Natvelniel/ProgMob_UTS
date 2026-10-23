using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices;
using ProgMob_UTS.Services;

namespace ProgMob_UTS
{
    /// <summary>
    /// Kelas konfigurasi utama untuk membangun dan menginisialisasi aplikasi .NET MAUI Blazor Hybrid.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Mengonfigurasi aplikasi MAUI, Blazor WebView, HttpClient, serta mendaftarkan service frontend
        /// ke dalam Dependency Injection container.
        /// </summary>
        /// <returns>Instance MauiApp yang siap dijalankan.</returns>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            // Menentukan alamat dasar (BaseAddress) Web API sesuai platform tempat aplikasi dijalankan.
            // Pada emulator Android, localhost komputer host diakses melalui 10.0.2.2,
            // sedangkan pada Windows / MacCatalyst menggunakan localhost secara langsung.
            var apiBaseUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5008/"
                : "http://localhost:5008/";

            // Mendaftarkan HttpClient dengan BaseAddress API agar dapat di-inject ke dalam service class.
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(apiBaseUrl),
                Timeout = TimeSpan.FromSeconds(15)
            });

            // Mendaftarkan service untuk komunikasi CRUD Dosen dan Mahasiswa ke Web API.
            builder.Services.AddScoped<DosenService>();
            builder.Services.AddScoped<MahasiswaService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
