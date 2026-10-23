using System.Text.Json;

namespace ProgMob_UTS.Models
{
    /// <summary>
    /// Kelas pembungkus (wrapper) sederhana untuk menyampaikan hasil pemanggilan API
    /// dari layer service ke komponen halaman Blazor, termasuk status keberhasilan dan pesan kesalahan.
    /// </summary>
    /// <typeparam name="T">Tipe data yang dikembalikan oleh API.</typeparam>
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        /// <summary>
        /// Membuat objek ApiResult yang menandakan operasi berhasil beserta datanya.
        /// </summary>
        /// <param name="data">Data hasil operasi API.</param>
        /// <param name="message">Pesan informasi opsional.</param>
        /// <returns>Instance ApiResult dengan status berhasil.</returns>
        public static ApiResult<T> Ok(T? data, string message = "Operasi berhasil.")
        {
            return new ApiResult<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        /// <summary>
        /// Membuat objek ApiResult yang menandakan operasi gagal beserta pesan penyebabnya.
        /// </summary>
        /// <param name="message">Pesan kesalahan yang mudah dipahami pengguna.</param>
        /// <returns>Instance ApiResult dengan status gagal.</returns>
        public static ApiResult<T> Fail(string message)
        {
            return new ApiResult<T>
            {
                Success = false,
                Message = message
            };
        }
    }

    /// <summary>
    /// Kelas pembantu untuk mengekstrak pesan kesalahan JSON yang dikirim oleh Web API.
    /// </summary>
    public static class ApiErrorParser
    {
        /// <summary>
        /// Membaca isi respons HTTP gagal dan mengambil properti 'message' atau daftar 'errors'
        /// agar pesan kesalahan dari backend dapat ditampilkan dengan jelas kepada pengguna.
        /// </summary>
        /// <param name="response">Objek HttpResponseMessage dari HttpClient.</param>
        /// <param name="defaultMessage">Pesan cadangan jika respons tidak memuat JSON yang dikenali.</param>
        /// <returns>Teks pesan kesalahan berbahasa Indonesia.</returns>
        public static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, string defaultMessage)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content))
                {
                    return defaultMessage;
                }

                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                // Memeriksa apakah JSON memiliki properti "message" dari controller backend.
                if (root.TryGetProperty("message", out var msgElement) && msgElement.ValueKind == JsonValueKind.String)
                {
                    var msg = msgElement.GetString();
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        return msg;
                    }
                }

                // Memeriksa apakah JSON memiliki properti "errors" dari validasi model ASP.NET Core.
                if (root.TryGetProperty("errors", out var errorsElement))
                {
                    if (errorsElement.ValueKind == JsonValueKind.Array && errorsElement.GetArrayLength() > 0)
                    {
                        return errorsElement[0].GetString() ?? defaultMessage;
                    }

                    if (errorsElement.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var prop in errorsElement.EnumerateObject())
                        {
                            if (prop.Value.ValueKind == JsonValueKind.Array && prop.Value.GetArrayLength() > 0)
                            {
                                return prop.Value[0].GetString() ?? defaultMessage;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Mengabaikan error parsing JSON dan menggunakan pesan default.
            }

            return defaultMessage;
        }
    }
}
