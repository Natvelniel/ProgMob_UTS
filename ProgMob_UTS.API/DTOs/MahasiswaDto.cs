using System.ComponentModel.DataAnnotations;
using ProgMob_UTS.API.Validation;

namespace ProgMob_UTS.API.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) untuk menampilkan data mahasiswa beserta nama dosen walinya
    /// tanpa menghasilkan perulangan objek (circular reference) pada JSON.
    /// </summary>
    public class MahasiswaDto
    {
        public int MahasiswaId { get; set; }
        public string Nim { get; set; } = string.Empty;
        public string NamaMahasiswa { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Angkatan { get; set; }
        public int DosenId { get; set; }
        public string NamaDosenWali { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data Transfer Object (DTO) untuk menerima input penambahan atau pembaruan data mahasiswa.
    /// Menyertakan aturan validasi dasar dan validasi domain email @student.kampus.ac.id pada sisi server.
    /// </summary>
    public class MahasiswaInputDto
    {
        [Required(ErrorMessage = "NIM wajib diisi.")]
        [StringLength(20, ErrorMessage = "NIM maksimal 20 karakter.")]
        public string Nim { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama mahasiswa wajib diisi.")]
        [StringLength(100, ErrorMessage = "Nama mahasiswa maksimal 100 karakter.")]
        public string NamaMahasiswa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi.")]
        [EmailAddress(ErrorMessage = "Format email tidak valid.")]
        [StringLength(100, ErrorMessage = "Email maksimal 100 karakter.")]
        [AllowedEmailDomain("student.kampus.ac.id", ErrorMessage = "Email mahasiswa harus menggunakan domain @student.kampus.ac.id.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Angkatan wajib diisi.")]
        [Range(2020, 2026, ErrorMessage = "Angkatan harus berupa tahun antara 2020 dan 2026.")]
        public int Angkatan { get; set; }

        [Required(ErrorMessage = "Dosen wali wajib dipilih.")]
        [Range(1, int.MaxValue, ErrorMessage = "Dosen wali wajib dipilih.")]
        public int DosenId { get; set; }
    }
}
