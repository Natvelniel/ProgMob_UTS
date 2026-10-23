using System.ComponentModel.DataAnnotations;
using ProgMob_UTS.Validation;

namespace ProgMob_UTS.Models
{
    /// <summary>
    /// Model data Dosen pada sisi frontend untuk menampung data dari API
    /// sekaligus menyediakan aturan validasi pada formulir Blazor.
    /// </summary>
    public class DosenModel
    {
        /// <summary>
        /// ID unik dosen (primary key dari backend).
        /// </summary>
        public int DosenId { get; set; }

        /// <summary>
        /// Nomor Induk Dosen Nasional (NIDN), wajib diisi dan maksimal 20 karakter.
        /// </summary>
        [Required(ErrorMessage = "NIDN wajib diisi.")]
        [StringLength(20, ErrorMessage = "NIDN maksimal 20 karakter.")]
        public string Nidn { get; set; } = string.Empty;

        /// <summary>
        /// Nama lengkap dosen, wajib diisi dan maksimal 100 karakter.
        /// </summary>
        [Required(ErrorMessage = "Nama dosen wajib diisi.")]
        [StringLength(100, ErrorMessage = "Nama dosen maksimal 100 karakter.")]
        public string NamaDosen { get; set; } = string.Empty;

        /// <summary>
        /// Alamat email dosen, wajib diisi dengan format email yang valid dan domain @kampus.ac.id.
        /// </summary>
        [Required(ErrorMessage = "Email wajib diisi.")]
        [EmailAddress(ErrorMessage = "Format email tidak valid.")]
        [StringLength(100, ErrorMessage = "Email maksimal 100 karakter.")]
        [AllowedEmailDomain("kampus.ac.id", ErrorMessage = "Email dosen harus menggunakan domain @kampus.ac.id.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Nama program studi dosen, wajib diisi dan maksimal 100 karakter.
        /// </summary>
        [Required(ErrorMessage = "Program studi wajib diisi.")]
        [StringLength(100, ErrorMessage = "Program studi maksimal 100 karakter.")]
        public string Prodi { get; set; } = string.Empty;
    }
}
