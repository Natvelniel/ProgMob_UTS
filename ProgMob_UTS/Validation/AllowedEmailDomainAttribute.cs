using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace ProgMob_UTS.Validation
{
    /// <summary>
    /// Atribut validasi kustom pada frontend Blazor untuk memastikan alamat email
    /// memiliki format yang benar dan menggunakan domain yang sesuai dengan entitas
    /// (@kampus.ac.id untuk Dosen dan @student.kampus.ac.id untuk Mahasiswa).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class AllowedEmailDomainAttribute : ValidationAttribute
    {
        private readonly string _expectedDomain;

        /// <summary>
        /// Konstruktor untuk menentukan domain email yang diwajibkan pada field form.
        /// </summary>
        /// <param name="expectedDomain">Domain yang diizinkan (contoh: "kampus.ac.id" atau "@kampus.ac.id").</param>
        public AllowedEmailDomainAttribute(string expectedDomain)
        {
            // Menghapus karakter '@' di awal jika ada agar pencocokan nama domain konsisten.
            _expectedDomain = expectedDomain.Trim().TrimStart('@');
        }

        /// <summary>
        /// Memeriksa apakah input email pada form Blazor memiliki format email yang valid
        /// dan bagian domain setelah '@' cocok secara eksak dengan aturan entitas.
        /// </summary>
        /// <param name="value">Nilai email dari input pengguna pada form.</param>
        /// <param name="validationContext">Konteks validasi komponen form Blazor.</param>
        /// <returns>ValidationResult.Success jika valid, atau pesan kesalahan di dekat kolom email jika tidak valid.</returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Jika input kosong, biarkan atribut [Required] yang menampilkan pesan "Email wajib diisi."
            if (value is not string email || string.IsNullOrWhiteSpace(email))
            {
                return ValidationResult.Success;
            }

            var trimmedEmail = email.Trim();
            var memberNames = validationContext.MemberName != null
                ? new[] { validationContext.MemberName }
                : null;

            // Jika struktur dasar '@' tidak terpenuhi, biarkan atribut [EmailAddress] yang menangani
            // agar pesan "Format email tidak valid." tidak ditampilkan ganda di bawah input Blazor.
            var atIndex = trimmedEmail.IndexOf('@');
            if (atIndex <= 0 ||
                atIndex != trimmedEmail.LastIndexOf('@') ||
                atIndex >= trimmedEmail.Length - 1)
            {
                return ValidationResult.Success;
            }

            if (trimmedEmail.Contains(' '))
            {
                return new ValidationResult("Format email tidak valid.", memberNames);
            }

            var localPart = trimmedEmail[..atIndex];
            var domainPart = trimmedEmail[(atIndex + 1)..];

            // Memeriksa keabsahan struktur local-part dan domain-part menggunakan MailAddress.
            if (localPart.StartsWith('.') || localPart.EndsWith('.') || localPart.Contains("..") ||
                domainPart.StartsWith('.') || domainPart.EndsWith('.') || domainPart.Contains("..") ||
                !MailAddress.TryCreate(trimmedEmail, out _))
            {
                return new ValidationResult("Format email tidak valid.", memberNames);
            }

            // Memeriksa kecocokan domain secara eksak (bukan pemeriksaan substring longgar).
            if (!string.Equals(domainPart, _expectedDomain, StringComparison.OrdinalIgnoreCase))
            {
                var errorMessage = !string.IsNullOrWhiteSpace(ErrorMessage)
                    ? ErrorMessage
                    : $"Email harus menggunakan domain @{_expectedDomain}.";

                return new ValidationResult(errorMessage, memberNames);
            }

            return ValidationResult.Success;
        }
    }
}
