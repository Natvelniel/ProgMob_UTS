using System.Net.Http.Json;
using ProgMob_UTS.Models;

namespace ProgMob_UTS.Services
{
    /// <summary>
    /// Service untuk mengelola komunikasi HTTP antara aplikasi frontend Blazor Hybrid
    /// dan endpoint backend /api/dosen.
    /// </summary>
    public class DosenService
    {
        private readonly HttpClient _httpClient;
        private const string Endpoint = "api/dosen";

        /// <summary>
        /// Konstruktor untuk menerima instance HttpClient melalui dependency injection.
        /// </summary>
        /// <param name="httpClient">Objek HttpClient yang telah dikonfigurasi BaseAddress-nya.</param>
        public DosenService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Mengambil seluruh daftar dosen dari Web API.
        /// </summary>
        /// <returns>Objek ApiResult yang memuat daftar dosen atau pesan kesalahan koneksi.</returns>
        public async Task<ApiResult<List<DosenModel>>> GetAllAsync()
        {
            try
            {
                // Mengambil seluruh data dosen melalui endpoint GET /api/dosen.
                var data = await _httpClient.GetFromJsonAsync<List<DosenModel>>(Endpoint);
                return ApiResult<List<DosenModel>>.Ok(data ?? new List<DosenModel>());
            }
            catch (HttpRequestException)
            {
                return ApiResult<List<DosenModel>>.Fail(
                    "Tidak dapat terhubung ke server API. Pastikan proyek backend (ProgMob_UTS.API) sedang berjalan.");
            }
            catch (Exception ex)
            {
                return ApiResult<List<DosenModel>>.Fail($"Terjadi kesalahan saat memuat data dosen: {ex.Message}");
            }
        }

        /// <summary>
        /// Mengambil detail satu dosen berdasarkan DosenId dari Web API.
        /// </summary>
        /// <param name="id">ID dosen yang ingin dilihat.</param>
        /// <returns>Objek ApiResult yang memuat data dosen atau pesan kesalahan (mis. 404 Not Found).</returns>
        public async Task<ApiResult<DosenModel>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{Endpoint}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<DosenModel>();
                    return ApiResult<DosenModel>.Ok(data);
                }

                var errorMsg = await ApiErrorParser.ReadErrorMessageAsync(response, "Data dosen tidak ditemukan.");
                return ApiResult<DosenModel>.Fail(errorMsg);
            }
            catch (HttpRequestException)
            {
                return ApiResult<DosenModel>.Fail(
                    "Tidak dapat terhubung ke server API. Pastikan proyek backend sedang berjalan.");
            }
            catch (Exception ex)
            {
                return ApiResult<DosenModel>.Fail($"Terjadi kesalahan saat memuat detail dosen: {ex.Message}");
            }
        }

        /// <summary>
        /// Mengirimkan data dosen baru ke Web API menggunakan metode POST.
        /// </summary>
        /// <param name="dosen">Data dosen yang akan ditambahkan.</param>
        /// <returns>Objek ApiResult yang memuat status penyimpanan dan pesan respons API.</returns>
        public async Task<ApiResult<DosenModel>> CreateAsync(DosenModel dosen)
        {
            try
            {
                var payload = new
                {
                    nidn = dosen.Nidn,
                    namaDosen = dosen.NamaDosen,
                    email = dosen.Email,
                    prodi = dosen.Prodi
                };

                var response = await _httpClient.PostAsJsonAsync(Endpoint, payload);
                if (response.IsSuccessStatusCode)
                {
                    var created = await response.Content.ReadFromJsonAsync<DosenModel>();
                    return ApiResult<DosenModel>.Ok(created, "Data dosen berhasil ditambahkan.");
                }

                var errorMsg = await ApiErrorParser.ReadErrorMessageAsync(response, "Gagal menambahkan data dosen.");
                return ApiResult<DosenModel>.Fail(errorMsg);
            }
            catch (HttpRequestException)
            {
                return ApiResult<DosenModel>.Fail(
                    "Tidak dapat terhubung ke server API. Pastikan proyek backend sedang berjalan.");
            }
            catch (Exception ex)
            {
                return ApiResult<DosenModel>.Fail($"Terjadi kesalahan saat menyimpan data dosen: {ex.Message}");
            }
        }

        /// <summary>
        /// Memperbarui data dosen yang sudah ada melalui metode PUT pada Web API.
        /// </summary>
        /// <param name="id">ID dosen yang akan diubah.</param>
        /// <param name="dosen">Data perubahan dosen.</param>
        /// <returns>Objek ApiResult yang memuat status pembaruan dan pesan respons API.</returns>
        public async Task<ApiResult<DosenModel>> UpdateAsync(int id, DosenModel dosen)
        {
            try
            {
                var payload = new
                {
                    nidn = dosen.Nidn,
                    namaDosen = dosen.NamaDosen,
                    email = dosen.Email,
                    prodi = dosen.Prodi
                };

                var response = await _httpClient.PutAsJsonAsync($"{Endpoint}/{id}", payload);
                if (response.IsSuccessStatusCode)
                {
                    var updated = await response.Content.ReadFromJsonAsync<DosenModel>();
                    return ApiResult<DosenModel>.Ok(updated, "Data dosen berhasil diperbarui.");
                }

                var errorMsg = await ApiErrorParser.ReadErrorMessageAsync(response, "Gagal memperbarui data dosen.");
                return ApiResult<DosenModel>.Fail(errorMsg);
            }
            catch (HttpRequestException)
            {
                return ApiResult<DosenModel>.Fail(
                    "Tidak dapat terhubung ke server API. Pastikan proyek backend sedang berjalan.");
            }
            catch (Exception ex)
            {
                return ApiResult<DosenModel>.Fail($"Terjadi kesalahan saat memperbarui data dosen: {ex.Message}");
            }
        }

        /// <summary>
        /// Menghapus data dosen berdasarkan DosenId melalui metode DELETE pada Web API.
        /// </summary>
        /// <param name="id">ID dosen yang akan dihapus.</param>
        /// <returns>Objek ApiResult yang menandakan keberhasilan atau alasan penolakan penghapusan.</returns>
        public async Task<ApiResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{Endpoint}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return ApiResult<bool>.Ok(true, "Data dosen berhasil dihapus.");
                }

                // Membaca alasan kegagalan (misalnya 409 Conflict karena dosen masih menjadi dosen wali).
                var errorMsg = await ApiErrorParser.ReadErrorMessageAsync(response, "Gagal menghapus data dosen.");
                return ApiResult<bool>.Fail(errorMsg);
            }
            catch (HttpRequestException)
            {
                return ApiResult<bool>.Fail(
                    "Tidak dapat terhubung ke server API. Pastikan proyek backend sedang berjalan.");
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail($"Terjadi kesalahan saat menghapus data dosen: {ex.Message}");
            }
        }
    }
}
