using System.ComponentModel.DataAnnotations;
using ProgMob_UTS.Validation;

namespace ProgMob_UTS.Models
{
    /// <summary>
    /// Model data Mahasiswa pada sisi frontend untuk menampung data dari API
    /// beserta nama dosen wali dan aturan validasi pada formulir Blazor.
    /// </summary>
    public class MahasiswaModel
    {
        /// <summary>
        /// ID unik mahasiswa (primary key dari backend).
        /// </summary>
        public int MahasiswaId { get; set; }

        /// <summary>
        /// Nomor Induk Mahasiswa (NIM), wajib diisi dan maksimal 20 karakter.
        /// </summary>
        [Required(ErrorMessage = "NIM wajib diisi.")]
        [StringLength(20, ErrorMessage = "NIM maksimal 20 karakter.")]
        public string Nim { get; set; } = string.Empty;

        /// <summary>
        /// Nama lengkap mahasiswa, wajib diisi dan maksimal 100 karakter.
        /// </summary>
        [Required(ErrorMessage = "Nama mahasiswa wajib diisi.")]
        [StringLength(100, ErrorMessage = "Nama mahasiswa maksimal 100 karakter.")]
        public string NamaMahasiswa { get; set; } = string.Empty;

        /// <summary>
        /// Alamat email mahasiswa, wajib diisi dengan format email yang valid dan domain @student.kampus.ac.id.
        /// </summary>
        [Required(ErrorMessage = "Email wajib diisi.")]
        [EmailAddress(ErrorMessage = "Format email tidak valid.")]
        [StringLength(100, ErrorMessage = "Email maksimal 100 karakter.")]
        [AllowedEmailDomain("student.kampus.ac.id", ErrorMessage = "Email mahasiswa harus menggunakan domain @student.kampus.ac.id.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Tahun angkatan masuk mahasiswa, wajib bernilai antara 2020 sampai 2026.
        /// </summary>
        [Required(ErrorMessage = "Angkatan wajib diisi.")]
        [Range(2020, 2026, ErrorMessage = "Angkatan harus berada pada rentang 2020 hingga 2026.")]
        public int Angkatan { get; set; } = 2024;

        /// <summary>
        /// ID dosen wali yang dipilih dari daftar dosen. Wajib bernilai lebih dari 0.
        /// </summary>
        [Required(ErrorMessage = "Dosen wali wajib dipilih.")]
        [Range(1, int.MaxValue, ErrorMessage = "Silakan pilih dosen wali terlebih dahulu.")]
        public int DosenId { get; set; }

        /// <summary>
        /// Nama lengkap dosen wali yang diterima dari hasil join/proyeksi API.
        /// </summary>
        public string NamaDosenWali { get; set; } = string.Empty;
    }
}
