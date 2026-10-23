using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace ProgMob_UTS.API.Validation
{
    /// <summary>
    /// Atribut validasi kustom yang dapat digunakan kembali untuk memastikan alamat email
    /// memiliki format yang valid dan menggunakan domain email yang tepat sesuai entitas
    /// (misalnya @kampus.ac.id untuk Dosen dan @student.kampus.ac.id untuk Mahasiswa).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class AllowedEmailDomainAttribute : ValidationAttribute
    {
        private readonly string _expectedDomain;

        /// <summary>
        /// Konstruktor untuk menentukan domain email yang diwajibkan pada properti.
        /// </summary>
        /// <param name="expectedDomain">Domain yang diizinkan (contoh: "kampus.ac.id" atau "@kampus.ac.id").</param>
        public AllowedEmailDomainAttribute(string expectedDomain)
        {
            // Menghapus karakter '@' di awal jika disertakan agar perbandingan bagian domain konsisten.
            _expectedDomain = expectedDomain.Trim().TrimStart('@');
        }

        /// <summary>
        /// Memvalidasi apakah nilai email memiliki struktur yang benar dan bagian domain setelah '@'
        /// sama persis dengan domain yang diwajibkan (bukan sekadar pemeriksaan substring longgar).
        /// </summary>
        /// <param name="value">Nilai input email dari pengguna atau request API.</param>
        /// <param name="validationContext">Konteks validasi model saat ini.</param>
        /// <returns>ValidationResult.Success jika valid, atau pesan kesalahan jika tidak sesuai.</returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Jika kosong/null, biarkan atribut [Required] yang menangani pesan wajib diisi.
            if (value is not string email || string.IsNullOrWhiteSpace(email))
            {
                return ValidationResult.Success;
            }

            var trimmedEmail = email.Trim();
            var memberNames = validationContext.MemberName != null
                ? new[] { validationContext.MemberName }
                : null;

            // Memastikan hanya terdapat tepat satu simbol '@' yang tidak berada di awal atau akhir.
            // Jika kondisi dasar ini gagal, biarkan atribut [EmailAddress] yang mengeluarkan pesan "Format email tidak valid."
            // agar pesan kesalahan tidak muncul dua kali.
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

            // Memastikan local-part dan domain-part tidak diawali/diakhiri titik serta format email valid menurut MailAddress.
            if (localPart.StartsWith('.') || localPart.EndsWith('.') || localPart.Contains("..") ||
                domainPart.StartsWith('.') || domainPart.EndsWith('.') || domainPart.Contains("..") ||
                !MailAddress.TryCreate(trimmedEmail, out _))
            {
                return new ValidationResult("Format email tidak valid.", memberNames);
            }

            // Membandingkan bagian domain secara eksak (case-insensitive) agar subdomain lain
            // seperti @student.kampus.ac.id tidak lolos saat memvalidasi @kampus.ac.id, begitu pula sebaliknya.
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
