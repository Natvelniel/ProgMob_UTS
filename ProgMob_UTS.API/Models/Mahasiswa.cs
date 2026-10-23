using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProgMob_UTS.API.Validation;

namespace ProgMob_UTS.API.Models
{
    /// <summary>
    /// Merepresentasikan entitas Mahasiswa pada database akademik.
    /// Setiap mahasiswa wajib memiliki satu dosen wali.
    /// </summary>
    public class Mahasiswa
    {
        /// <summary>
        /// Primary key mahasiswa yang bertambah otomatis (auto-increment).
        /// </summary>
        [Key]
        public int MahasiswaId { get; set; }

        /// <summary>
        /// Nomor Induk Mahasiswa (NIM), wajib diisi, unik, maksimal 20 karakter.
        /// </summary>
        [Required(ErrorMessage = "NIM wajib diisi.")]
        [StringLength(20, ErrorMessage = "NIM maksimal 20 karakter.")]
        public string Nim { get; set; } = string.Empty;

        /// <summary>
        /// Nama lengkap mahasiswa, wajib diisi, maksimal 100 karakter.
        /// </summary>
        [Required(ErrorMessage = "Nama mahasiswa wajib diisi.")]
        [StringLength(100, ErrorMessage = "Nama mahasiswa maksimal 100 karakter.")]
        public string NamaMahasiswa { get; set; } = string.Empty;

        /// <summary>
        /// Alamat email mahasiswa, wajib diisi, berformat email valid, maksimal 100 karakter,
        /// dan wajib menggunakan domain @student.kampus.ac.id.
        /// </summary>
        [Required(ErrorMessage = "Email wajib diisi.")]
        [EmailAddress(ErrorMessage = "Format email tidak valid.")]
        [StringLength(100, ErrorMessage = "Email maksimal 100 karakter.")]
        [AllowedEmailDomain("student.kampus.ac.id", ErrorMessage = "Email mahasiswa harus menggunakan domain @student.kampus.ac.id.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Tahun angkatan masuk mahasiswa, wajib berada pada rentang 2020 sampai 2026.
        /// </summary>
        [Required(ErrorMessage = "Angkatan wajib diisi.")]
        [Range(2020, 2026, ErrorMessage = "Angkatan harus berada pada rentang 2020 hingga 2026.")]
        public int Angkatan { get; set; }

        /// <summary>
        /// Foreign key yang merujuk pada DosenId milik dosen wali mahasiswa.
        /// </summary>
        [Required(ErrorMessage = "Dosen wali wajib dipilih.")]
        public int DosenId { get; set; }

        /// <summary>
        /// Properti navigasi ke objek Dosen yang menjadi dosen wali mahasiswa ini.
        /// </summary>
        [ForeignKey(nameof(DosenId))]
        public Dosen? DosenWali { get; set; }
    }
}
