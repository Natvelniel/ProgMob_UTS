using System.ComponentModel.DataAnnotations;
using ProgMob_UTS.API.Validation;

namespace ProgMob_UTS.API.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) untuk menampilkan data dosen pada respons API.
    /// </summary>
    public class DosenDto
    {
        public int DosenId { get; set; }
        public string Nidn { get; set; } = string.Empty;
        public string NamaDosen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Prodi { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data Transfer Object (DTO) untuk menerima input penambahan atau pembaruan data dosen.
    /// Menyertakan aturan validasi dasar dan validasi domain email @kampus.ac.id pada sisi server.
    /// </summary>
    public class DosenInputDto
    {
        [Required(ErrorMessage = "NIDN wajib diisi.")]
        [StringLength(20, ErrorMessage = "NIDN maksimal 20 karakter.")]
        public string Nidn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama dosen wajib diisi.")]
        [StringLength(100, ErrorMessage = "Nama dosen maksimal 100 karakter.")]
        public string NamaDosen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi.")]
        [EmailAddress(ErrorMessage = "Format email tidak valid.")]
        [StringLength(100, ErrorMessage = "Email maksimal 100 karakter.")]
        [AllowedEmailDomain("kampus.ac.id", ErrorMessage = "Email dosen harus menggunakan domain @kampus.ac.id.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Program studi wajib diisi.")]
        [StringLength(100, ErrorMessage = "Program studi maksimal 100 karakter.")]
        public string Prodi { get; set; } = string.Empty;
    }
}
