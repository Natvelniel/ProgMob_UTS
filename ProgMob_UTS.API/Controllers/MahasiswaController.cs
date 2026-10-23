using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgMob_UTS.API.Data;
using ProgMob_UTS.API.DTOs;
using ProgMob_UTS.API.Models;

namespace ProgMob_UTS.API.Controllers
{
    /// <summary>
    /// Controller API untuk mengelola operasi CRUD (Create, Read, Update, Delete) pada data Mahasiswa.
    /// </summary>
    [Route("api/mahasiswa")]
    [ApiController]
    public class MahasiswaController : ControllerBase
    {
        private readonly AcademicDbContext _context;

        /// <summary>
        /// Konstruktor untuk menginisialisasi MahasiswaController dengan konteks database akademik.
        /// </summary>
        /// <param name="context">Instance AcademicDbContext dari dependency injection.</param>
        public MahasiswaController(AcademicDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Mengambil seluruh daftar mahasiswa beserta nama dosen walinya menggunakan proyeksi LINQ ke DTO.
        /// </summary>
        /// <returns>Daftar mahasiswa beserta nama dosen wali dengan status 200 OK.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MahasiswaDto>>> GetAllMahasiswa()
        {
            // Menggunakan proyeksi LINQ (Select) ke MahasiswaDto agar nama dosen wali dapat diambil
            // tanpa menghasilkan circular reference pada serialisasi JSON.
            var mahasiswaList = await _context.Mahasiswa
                .Include(m => m.DosenWali)
                .OrderBy(m => m.Nim)
                .Select(m => new MahasiswaDto
                {
                    MahasiswaId = m.MahasiswaId,
                    Nim = m.Nim,
                    NamaMahasiswa = m.NamaMahasiswa,
                    Email = m.Email,
                    Angkatan = m.Angkatan,
                    DosenId = m.DosenId,
                    NamaDosenWali = m.DosenWali != null ? m.DosenWali.NamaDosen : string.Empty
                })
                .ToListAsync();

            return Ok(mahasiswaList);
        }

        /// <summary>
        /// Mengambil detail satu data mahasiswa berdasarkan MahasiswaId beserta informasi dosen walinya.
        /// </summary>
        /// <param name="id">ID mahasiswa yang dicari.</param>
        /// <returns>Data detail mahasiswa (200 OK) atau 404 Not Found jika tidak ditemukan.</returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MahasiswaDto>> GetMahasiswaById(int id)
        {
            // Mengambil mahasiswa berdasarkan ID dan memproyeksikannya ke MahasiswaDto.
            var mahasiswa = await _context.Mahasiswa
                .Include(m => m.DosenWali)
                .Where(m => m.MahasiswaId == id)
                .Select(m => new MahasiswaDto
                {
                    MahasiswaId = m.MahasiswaId,
                    Nim = m.Nim,
                    NamaMahasiswa = m.NamaMahasiswa,
                    Email = m.Email,
                    Angkatan = m.Angkatan,
                    DosenId = m.DosenId,
                    NamaDosenWali = m.DosenWali != null ? m.DosenWali.NamaDosen : string.Empty
                })
                .FirstOrDefaultAsync();

            if (mahasiswa == null)
            {
                return NotFound(new { message = $"Data mahasiswa dengan ID {id} tidak ditemukan." });
            }

            return Ok(mahasiswa);
        }

        /// <summary>
        /// Menambahkan data mahasiswa baru beserta relasi dosen walinya.
        /// </summary>
        /// <param name="input">Data mahasiswa baru yang dikirim melalui body request.</param>
        /// <returns>Data mahasiswa yang berhasil dibuat (201 Created), 400 Bad Request, atau 409 Conflict jika NIM duplikat.</returns>
        [HttpPost]
        public async Task<ActionResult<MahasiswaDto>> CreateMahasiswa([FromBody] MahasiswaInputDto input)
        {
            var normalizedNim = input.Nim.Trim();

            // Memeriksa apakah NIM sudah terdaftar untuk mencegah duplikasi data mahasiswa.
            var isDuplicateNim = await _context.Mahasiswa
                .AnyAsync(m => m.Nim.ToLower() == normalizedNim.ToLower());

            if (isDuplicateNim)
            {
                return Conflict(new { message = $"NIM '{normalizedNim}' sudah terdaftar. Gunakan NIM yang berbeda." });
            }

            // Memastikan dosen wali yang dipilih benar-benar ada di dalam tabel Dosen.
            var dosenWali = await _context.Dosen.FindAsync(input.DosenId);
            if (dosenWali == null)
            {
                return BadRequest(new { message = "Dosen wali yang dipilih tidak ditemukan di database." });
            }

            var mahasiswaBaru = new Mahasiswa
            {
                Nim = normalizedNim,
                NamaMahasiswa = input.NamaMahasiswa.Trim(),
                Email = input.Email.Trim(),
                Angkatan = input.Angkatan,
                DosenId = input.DosenId
            };

            // Menyimpan data mahasiswa baru ke database.
            _context.Mahasiswa.Add(mahasiswaBaru);
            await _context.SaveChangesAsync();

            var result = new MahasiswaDto
            {
                MahasiswaId = mahasiswaBaru.MahasiswaId,
                Nim = mahasiswaBaru.Nim,
                NamaMahasiswa = mahasiswaBaru.NamaMahasiswa,
                Email = mahasiswaBaru.Email,
                Angkatan = mahasiswaBaru.Angkatan,
                DosenId = mahasiswaBaru.DosenId,
                NamaDosenWali = dosenWali.NamaDosen
            };

            return CreatedAtAction(nameof(GetMahasiswaById), new { id = mahasiswaBaru.MahasiswaId }, result);
        }

        /// <summary>
        /// Memperbarui data mahasiswa dan/atau dosen walinya berdasarkan MahasiswaId.
        /// </summary>
        /// <param name="id">ID mahasiswa yang akan diperbarui.</param>
        /// <param name="input">Data perubahan mahasiswa.</param>
        /// <returns>Data mahasiswa setelah diperbarui (200 OK), 400 Bad Request, 404 Not Found, atau 409 Conflict.</returns>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<MahasiswaDto>> UpdateMahasiswa(int id, [FromBody] MahasiswaInputDto input)
        {
            // Memastikan data mahasiswa yang ingin diperbarui tersedia di database.
            var mahasiswa = await _context.Mahasiswa.FindAsync(id);
            if (mahasiswa == null)
            {
                return NotFound(new { message = $"Data mahasiswa dengan ID {id} tidak ditemukan." });
            }

            var normalizedNim = input.Nim.Trim();

            // Memeriksa apakah NIM baru bertabrakan dengan NIM mahasiswa lain (selain dirinya sendiri).
            var isDuplicateNim = await _context.Mahasiswa
                .AnyAsync(m => m.MahasiswaId != id && m.Nim.ToLower() == normalizedNim.ToLower());

            if (isDuplicateNim)
            {
                return Conflict(new { message = $"NIM '{normalizedNim}' sudah digunakan oleh mahasiswa lain." });
            }

            // Memastikan dosen wali yang dipilih tersedia di tabel Dosen.
            var dosenWali = await _context.Dosen.FindAsync(input.DosenId);
            if (dosenWali == null)
            {
                return BadRequest(new { message = "Dosen wali yang dipilih tidak ditemukan di database." });
            }

            // Memperbarui data mahasiswa dan menyimpannya ke database.
            mahasiswa.Nim = normalizedNim;
            mahasiswa.NamaMahasiswa = input.NamaMahasiswa.Trim();
            mahasiswa.Email = input.Email.Trim();
            mahasiswa.Angkatan = input.Angkatan;
            mahasiswa.DosenId = input.DosenId;

            await _context.SaveChangesAsync();

            var result = new MahasiswaDto
            {
                MahasiswaId = mahasiswa.MahasiswaId,
                Nim = mahasiswa.Nim,
                NamaMahasiswa = mahasiswa.NamaMahasiswa,
                Email = mahasiswa.Email,
                Angkatan = mahasiswa.Angkatan,
                DosenId = mahasiswa.DosenId,
                NamaDosenWali = dosenWali.NamaDosen
            };

            return Ok(result);
        }

        /// <summary>
        /// Menghapus data mahasiswa dari database berdasarkan MahasiswaId.
        /// </summary>
        /// <param name="id">ID mahasiswa yang akan dihapus.</param>
        /// <returns>204 No Content jika berhasil dihapus, atau 404 Not Found jika tidak ditemukan.</returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMahasiswa(int id)
        {
            var mahasiswa = await _context.Mahasiswa.FindAsync(id);
            if (mahasiswa == null)
            {
                return NotFound(new { message = $"Data mahasiswa dengan ID {id} tidak ditemukan." });
            }

            // Menghapus entitas mahasiswa dari database.
            _context.Mahasiswa.Remove(mahasiswa);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
