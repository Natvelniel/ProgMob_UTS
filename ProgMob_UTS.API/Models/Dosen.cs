using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ProgMob_UTS.API.Validation;

namespace ProgMob_UTS.API.Models
{
    /// <summary>
    /// Merepresentasikan entitas Dosen pada database akademik.
    /// Satu dosen dapat menjadi dosen wali bagi banyak mahasiswa.
    /// </summary>
    public class Dosen
    {
        /// <summary>
        /// Primary key dosen yang bertambah otomatis (auto-increment).
        /// </summary>
        [Key]
        public int DosenId { get; set; }

        /// <summary>
        /// Nomor Induk Dosen Nasional (NIDN), wajib diisi, unik, maksimal 20 karakter.
        /// </summary>
        [Required(ErrorMessage = "NIDN wajib diisi.")]
        [StringLength(20, ErrorMessage = "NIDN maksimal 20 karakter.")]
        public string Nidn { get; set; } = string.Empty;

        /// <summary>
        /// Nama lengkap dosen beserta gelar, wajib diisi, maksimal 100 karakter.
        /// </summary>
        [Required(ErrorMessage = "Nama dosen wajib diisi.")]
        [StringLength(100, ErrorMessage = "Nama dosen maksimal 100 karakter.")]
        public string NamaDosen { get; set; } = string.Empty;

        /// <summary>
        /// Alamat email resmi dosen, wajib diisi, berformat email valid, maksimal 100 karakter,
        /// dan wajib menggunakan domain @kampus.ac.id.
        /// </summary>
        [Required(ErrorMessage = "Email wajib diisi.")]
        [EmailAddress(ErrorMessage = "Format email tidak valid.")]
        [StringLength(100, ErrorMessage = "Email maksimal 100 karakter.")]
        [AllowedEmailDomain("kampus.ac.id", ErrorMessage = "Email dosen harus menggunakan domain @kampus.ac.id.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Nama program studi homebase dosen, wajib diisi, maksimal 100 karakter.
        /// </summary>
        [Required(ErrorMessage = "Program studi wajib diisi.")]
        [StringLength(100, ErrorMessage = "Program studi maksimal 100 karakter.")]
        public string Prodi { get; set; } = string.Empty;

        /// <summary>
        /// Daftar mahasiswa yang dibimbing oleh dosen ini sebagai dosen wali (relasi one-to-many).
        /// Diabaikan saat serialisasi JSON langsung untuk mencegah circular reference.
        /// </summary>
        [JsonIgnore]
        public ICollection<Mahasiswa> MahasiswaBimbingan { get; set; } = new List<Mahasiswa>();
    }
}
