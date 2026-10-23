using System.Net.Http.Json;
using ProgMob_UTS.Models;

namespace ProgMob_UTS.Services
{
    /// <summary>
    /// Service untuk mengelola komunikasi HTTP antara aplikasi frontend Blazor Hybrid
    /// dan endpoint backend /api/mahasiswa.
    /// </summary>
    public class MahasiswaService
    {
        private readonly HttpClient _httpClient;
        private const string Endpoint = "api/mahasiswa";

        /// <summary>
        /// Konstruktor untuk menerima instance HttpClient melalui dependency injection.
        /// </summary>
        /// <param name="httpClient">Objek HttpClient yang telah dikonfigurasi BaseAddress-nya.</param>
        public MahasiswaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Mengambil seluruh daftar mahasiswa beserta nama dosen walinya dari Web API.
        /// </summary>
        /// <returns>Objek ApiResult yang memuat daftar mahasiswa atau pesan kesalahan koneksi.</returns>
        public async Task<ApiResult<List<MahasiswaModel>>> GetAllAsync()
        {
            try
            {
                // Mengambil seluruh data mahasiswa melalui endpoint GET /api/mahasiswa.
                var data = await _httpClient.GetFromJsonAsync<List<MahasiswaModel>>(Endpoint);
                return ApiResult<List<MahasiswaModel>>.Ok(data ?? new List<MahasiswaModel>());
            }
            catch (HttpRequestException)
            {
                return ApiResult<List<MahasiswaModel>>.Fail(
                    "Tidak dapat terhubung ke server API. Pastikan proyek backend (ProgMob_UTS.API) sedang berjalan.");
            }
            catch (Exception ex)
            {
                return ApiResult<List<MahasiswaModel>>.Fail($"Terjadi kesalahan saat memuat data mahasiswa: {ex.Message}");
            }
        }

        /// <summary>
        /// Mengambil detail satu mahasiswa berdasarkan MahasiswaId dari Web API.
        /// </summary>
        /// <param name="id">ID mahasiswa yang ingin dilihat.</param>
        /// <returns>Objek ApiResult yang memuat data detail mahasiswa atau pesan kesalahan.</returns>
        public async Task<ApiResult<MahasiswaModel>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{Endpoint}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<MahasiswaModel>();
                    return ApiResult<MahasiswaModel>.Ok(data);
                }

                var errorMsg = await ApiErrorParser.ReadErrorMessageAsync(response, "Data mahasiswa tidak ditemukan.");
                return ApiResult<MahasiswaModel>.Fail(errorMsg);
            }
            catch (HttpRequestException)
            {
                return ApiResult<MahasiswaModel>.Fail(
                    "Tidak dapat terhubung ke server API. Pastikan proyek backend sedang berjalan.");
            }
            catch (Exception ex)
            {
                return ApiResult<MahasiswaModel>.Fail($"Terjadi kesalahan saat memuat detail mahasiswa: {ex.Message}");
            }
        }

        /// <summary>
        /// Mengirimkan data mahasiswa baru ke Web API menggunakan metode POST.
        /// </summary>
        /// <param name="mahasiswa">Data mahasiswa baru beserta DosenId dosen walinya.</param>
        /// <returns>Objek ApiResult yang memuat status penyimpanan dan pesan respons API.</returns>
        public async Task<ApiResult<MahasiswaModel>> CreateAsync(MahasiswaModel mahasiswa)
        {
            try
            {
                var payload = new
                {
                    nim = mahasiswa.Nim,
                    namaMahasiswa = mahasiswa.NamaMahasiswa,
                    email = mahasiswa.Email,
                    angkatan = mahasiswa.Angkatan,
                    dosenId = mahasiswa.DosenId
                };

                var response = await _httpClient.PostAsJsonAsync(Endpoint, payload);
                if (response.IsSuccessStatusCode)
                {
                    var created = await response.Content.ReadFromJsonAsync<MahasiswaModel>();
                    return ApiResult<MahasiswaModel>.Ok(created, "Data mahasiswa berhasil ditambahkan.");
                }

                var errorMsg = await ApiErrorParser.ReadErrorMessageAsync(response, "Gagal menambahkan data mahasiswa.");
                return ApiResult<MahasiswaModel>.Fail(errorMsg);
            }
            catch (HttpRequestException)
            {
                return ApiResult<MahasiswaModel>.Fail(
                    "Tidak dapat terhubung ke server API. Pastikan proyek backend sedang berjalan.");
            }
            catch (Exception ex)
            {
                return ApiResult<MahasiswaModel>.Fail($"Terjadi kesalahan saat menyimpan data mahasiswa: {ex.Message}");
            }
        }

        /// <summary>
        /// Memperbarui data mahasiswa dan/atau dosen walinya melalui metode PUT pada Web API.
        /// </summary>
        /// <param name="id">ID mahasiswa yang akan diubah.</param>
        /// <param name="mahasiswa">Data perubahan mahasiswa.</param>
        /// <returns>Objek ApiResult yang memuat status pembaruan dan pesan respons API.</returns>
        public async Task<ApiResult<MahasiswaModel>> UpdateAsync(int id, MahasiswaModel mahasiswa)
        {
            try
            {
                var payload = new
                {
                    nim = mahasiswa.Nim,
                    namaMahasiswa = mahasiswa.NamaMahasiswa,
                    email = mahasiswa.Email,
                    angkatan = mahasiswa.Angkatan,
                    dosenId = mahasiswa.DosenId
                };

                var response = await _httpClient.PutAsJsonAsync($"{Endpoint}/{id}", payload);
                if (response.IsSuccessStatusCode)
                {
                    var updated = await response.Content.ReadFromJsonAsync<MahasiswaModel>();
                    return ApiResult<MahasiswaModel>.Ok(updated, "Data mahasiswa berhasil diperbarui.");
                }

                var errorMsg = await ApiErrorParser.ReadErrorMessageAsync(response, "Gagal memperbarui data mahasiswa.");
                return ApiResult<MahasiswaModel>.Fail(errorMsg);
            }
            catch (HttpRequestException)
            {
                return ApiResult<MahasiswaModel>.Fail(
                    "Tidak dapat terhubung ke server API. Pastikan proyek backend sedang berjalan.");
            }
            catch (Exception ex)
            {
                return ApiResult<MahasiswaModel>.Fail($"Terjadi kesalahan saat memperbarui data mahasiswa: {ex.Message}");
            }
        }

        /// <summary>
        /// Menghapus data mahasiswa berdasarkan MahasiswaId melalui metode DELETE pada Web API.
        /// </summary>
        /// <param name="id">ID mahasiswa yang akan dihapus.</param>
        /// <returns>Objek ApiResult yang menandakan keberhasilan atau kegagalan penghapusan.</returns>
        public async Task<ApiResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{Endpoint}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return ApiResult<bool>.Ok(true, "Data mahasiswa berhasil dihapus.");
                }

                var errorMsg = await ApiErrorParser.ReadErrorMessageAsync(response, "Gagal menghapus data mahasiswa.");
                return ApiResult<bool>.Fail(errorMsg);
            }
            catch (HttpRequestException)
            {
                return ApiResult<bool>.Fail(
                    "Tidak dapat terhubung ke server API. Pastikan proyek backend sedang berjalan.");
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail($"Terjadi kesalahan saat menghapus data mahasiswa: {ex.Message}");
            }
        }
    }
}
