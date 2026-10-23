using Microsoft.EntityFrameworkCore;
using ProgMob_UTS.API.Models;

namespace ProgMob_UTS.API.Data
{
    /// <summary>
    /// Kelas DbContext utama untuk mengelola koneksi database SQLite, pemetaan entitas,
    /// relasi one-to-many antara Dosen dan Mahasiswa, serta pengisian data awal (seeding).
    /// </summary>
    public class AcademicDbContext : DbContext
    {
        /// <summary>
        /// Konstruktor untuk menerima opsi konfigurasi DbContext dari dependency injection.
        /// </summary>
        /// <param name="options">Opsi konfigurasi AcademicDbContext.</param>
        public AcademicDbContext(DbContextOptions<AcademicDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Tabel data Dosen pada database.
        /// </summary>
        public DbSet<Dosen> Dosen { get; set; }

        /// <summary>
        /// Tabel data Mahasiswa pada database.
        /// </summary>
        public DbSet<Mahasiswa> Mahasiswa { get; set; }

        /// <summary>
        /// Mengonfigurasi batasan unik (unique constraint), relasi antar-tabel,
        /// dan data awal (seed data) saat skema database dibuat melalui EF Core Migration.
        /// </summary>
        /// <param name="modelBuilder">Objek pembentuk model EF Core.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurasi tabel Dosen: memastikan NIDN bersifat unik di tingkat database.
            modelBuilder.Entity<Dosen>(entity =>
            {
                entity.HasIndex(d => d.Nidn).IsUnique();
            });

            // Konfigurasi tabel Mahasiswa: memastikan NIM unik dan mengatur relasi one-to-many ke Dosen.
            modelBuilder.Entity<Mahasiswa>(entity =>
            {
                entity.HasIndex(m => m.Nim).IsUnique();

                // Satu dosen dapat membimbing banyak mahasiswa, dan setiap mahasiswa wajib memiliki satu dosen wali.
                // DeleteBehavior.Restrict mencegah penghapusan dosen jika masih ada mahasiswa yang merujuk padanya.
                entity.HasOne(m => m.DosenWali)
                      .WithMany(d => d.MahasiswaBimbingan)
                      .HasForeignKey(m => m.DosenId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Data awal (Seeding) minimal 3 data dosen dengan nilai yang realistis dan valid.
            modelBuilder.Entity<Dosen>().HasData(
                new Dosen
                {
                    DosenId = 1,
                    Nidn = "0512038801",
                    NamaDosen = "Dr. Anita Wijaya, S.Kom., M.Cs.",
                    Email = "anita.wijaya@kampus.ac.id",
                    Prodi = "Sistem Informasi"
                },
                new Dosen
                {
                    DosenId = 2,
                    Nidn = "0524078502",
                    NamaDosen = "Ir. Bambang Pratama, M.Kom.",
                    Email = "bambang.pratama@kampus.ac.id",
                    Prodi = "Informatika"
                },
                new Dosen
                {
                    DosenId = 3,
                    Nidn = "0509119003",
                    NamaDosen = "Siti Rahmawati, S.Kom., M.T.",
                    Email = "siti.rahmawati@kampus.ac.id",
                    Prodi = "Sistem Informasi"
                }
            );

            // Data awal (Seeding) minimal 5 data mahasiswa dengan relasi dosen wali yang valid.
            modelBuilder.Entity<Mahasiswa>().HasData(
                new Mahasiswa
                {
                    MahasiswaId = 1,
                    Nim = "72230001",
                    NamaMahasiswa = "Budi Santoso",
                    Email = "budi.santoso@student.kampus.ac.id",
                    Angkatan = 2023,
                    DosenId = 1
                },
                new Mahasiswa
                {
                    MahasiswaId = 2,
                    Nim = "72230002",
                    NamaMahasiswa = "Clara Kusuma",
                    Email = "clara.kusuma@student.kampus.ac.id",
                    Angkatan = 2023,
                    DosenId = 1
                },
                new Mahasiswa
                {
                    MahasiswaId = 3,
                    Nim = "71220003",
                    NamaMahasiswa = "Dimas Saputra",
                    Email = "dimas.saputra@student.kampus.ac.id",
                    Angkatan = 2022,
                    DosenId = 2
                },
                new Mahasiswa
                {
                    MahasiswaId = 4,
                    Nim = "72240004",
                    NamaMahasiswa = "Eka Putri Lestari",
                    Email = "eka.lestari@student.kampus.ac.id",
                    Angkatan = 2024,
                    DosenId = 2
                },
                new Mahasiswa
                {
                    MahasiswaId = 5,
                    Nim = "72250005",
                    NamaMahasiswa = "Farhan Nugroho",
                    Email = "farhan.nugroho@student.kampus.ac.id",
                    Angkatan = 2025,
                    DosenId = 3
                }
            );
        }
    }
}
