using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgMob_UTS.API.Data;
using ProgMob_UTS.API.DTOs;
using ProgMob_UTS.API.Models;

namespace ProgMob_UTS.API.Controllers
{
    /// <summary>
    /// Controller API untuk mengelola operasi CRUD (Create, Read, Update, Delete) pada data Dosen.
    /// </summary>
    [Route("api/dosen")]
    [ApiController]
    public class DosenController : ControllerBase
    {
        private readonly AcademicDbContext _context;

        /// <summary>
        /// Konstruktor untuk menginisialisasi DosenController dengan konteks database akademik.
        /// </summary>
        /// <param name="context">Instance AcademicDbContext dari dependency injection.</param>
        public DosenController(AcademicDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Mengambil seluruh daftar dosen yang tersimpan di dalam database.
        /// </summary>
        /// <returns>Daftar seluruh dosen dengan status 200 OK.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DosenDto>>> GetAllDosen()
        {
            // Mengambil seluruh data dosen dari tabel Dosen dan memetakannya ke DosenDto.
            var dosenList = await _context.Dosen
                .OrderBy(d => d.NamaDosen)
                .Select(d => new DosenDto
                {
                    DosenId = d.DosenId,
                    Nidn = d.Nidn,
                    NamaDosen = d.NamaDosen,
                    Email = d.Email,
                    Prodi = d.Prodi
                })
                .ToListAsync();

            return Ok(dosenList);
        }

        /// <summary>
        /// Mengambil detail satu data dosen berdasarkan DosenId.
        /// </summary>
        /// <param name="id">ID dosen yang dicari.</param>
        /// <returns>Data detail dosen (200 OK) atau pesan 404 Not Found jika tidak ditemukan.</returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DosenDto>> GetDosenById(int id)
        {
            // Mencari dosen berdasarkan primary key DosenId.
            var dosen = await _context.Dosen.FindAsync(id);

            if (dosen == null)
            {
                return NotFound(new { message = $"Data dosen dengan ID {id} tidak ditemukan." });
            }

            var result = new DosenDto
            {
                DosenId = dosen.DosenId,
                Nidn = dosen.Nidn,
                NamaDosen = dosen.NamaDosen,
                Email = dosen.Email,
                Prodi = dosen.Prodi
            };

            return Ok(result);
        }

        /// <summary>
        /// Menambahkan data dosen baru ke dalam database.
        /// </summary>
        /// <param name="input">Data dosen baru yang dikirim melalui body request.</param>
        /// <returns>Data dosen yang berhasil dibuat (201 Created), 400 Bad Request, atau 409 Conflict jika NIDN duplikat.</returns>
        [HttpPost]
        public async Task<ActionResult<DosenDto>> CreateDosen([FromBody] DosenInputDto input)
        {
            var normalizedNidn = input.Nidn.Trim();

            // Memeriksa apakah NIDN sudah digunakan oleh dosen lain untuk menjaga keunikan data.
            var isDuplicateNidn = await _context.Dosen
                .AnyAsync(d => d.Nidn.ToLower() == normalizedNidn.ToLower());

            if (isDuplicateNidn)
            {
                return Conflict(new { message = $"NIDN '{normalizedNidn}' sudah terdaftar. Gunakan NIDN yang berbeda." });
            }

            var dosenBaru = new Dosen
            {
                Nidn = normalizedNidn,
                NamaDosen = input.NamaDosen.Trim(),
                Email = input.Email.Trim(),
                Prodi = input.Prodi.Trim()
            };

            // Menyimpan entitas dosen baru ke database SQLite melalui EF Core.
            _context.Dosen.Add(dosenBaru);
            await _context.SaveChangesAsync();

            var result = new DosenDto
            {
                DosenId = dosenBaru.DosenId,
                Nidn = dosenBaru.Nidn,
                NamaDosen = dosenBaru.NamaDosen,
                Email = dosenBaru.Email,
                Prodi = dosenBaru.Prodi
            };

            return CreatedAtAction(nameof(GetDosenById), new { id = dosenBaru.DosenId }, result);
        }

        /// <summary>
        /// Memperbarui data dosen yang sudah ada berdasarkan DosenId.
        /// </summary>
        /// <param name="id">ID dosen yang akan diperbarui.</param>
        /// <param name="input">Data perubahan dosen.</param>
        /// <returns>Data dosen setelah diperbarui (200 OK), 404 Not Found, atau 409 Conflict jika NIDN duplikat.</returns>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<DosenDto>> UpdateDosen(int id, [FromBody] DosenInputDto input)
        {
            // Memastikan data dosen yang ingin diubah tersedia di database.
            var dosen = await _context.Dosen.FindAsync(id);
            if (dosen == null)
            {
                return NotFound(new { message = $"Data dosen dengan ID {id} tidak ditemukan." });
            }

            var normalizedNidn = input.Nidn.Trim();

            // Memeriksa apakah NIDN baru bertabrakan dengan NIDN milik dosen lain (selain dirinya sendiri).
            var isDuplicateNidn = await _context.Dosen
                .AnyAsync(d => d.DosenId != id && d.Nidn.ToLower() == normalizedNidn.ToLower());

            if (isDuplicateNidn)
            {
                return Conflict(new { message = $"NIDN '{normalizedNidn}' sudah digunakan oleh dosen lain." });
            }

            // Memperbarui properti dosen dan menyimpannya ke database.
            dosen.Nidn = normalizedNidn;
            dosen.NamaDosen = input.NamaDosen.Trim();
            dosen.Email = input.Email.Trim();
            dosen.Prodi = input.Prodi.Trim();

            await _context.SaveChangesAsync();

            var result = new DosenDto
            {
                DosenId = dosen.DosenId,
                Nidn = dosen.Nidn,
                NamaDosen = dosen.NamaDosen,
                Email = dosen.Email,
                Prodi = dosen.Prodi
            };

            return Ok(result);
        }

        /// <summary>
        /// Menghapus data dosen berdasarkan DosenId apabila dosen tersebut tidak sedang menjadi dosen wali mahasiswa.
        /// </summary>
        /// <param name="id">ID dosen yang akan dihapus.</param>
        /// <returns>204 No Content jika berhasil, 404 Not Found jika tidak ada, atau 409 Conflict jika masih menjadi dosen wali.</returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDosen(int id)
        {
            var dosen = await _context.Dosen.FindAsync(id);
            if (dosen == null)
            {
                return NotFound(new { message = $"Data dosen dengan ID {id} tidak ditemukan." });
            }

            // Memeriksa apakah dosen ini masih terdaftar sebagai dosen wali pada tabel Mahasiswa.
            // Sesuai aturan bisnis, dosen yang masih membimbing mahasiswa tidak boleh dihapus.
            var masihMenjadiWali = await _context.Mahasiswa.AnyAsync(m => m.DosenId == id);
            if (masihMenjadiWali)
            {
                return Conflict(new
                {
                    message = $"Dosen '{dosen.NamaDosen}' tidak dapat dihapus karena masih menjadi dosen wali bagi mahasiswa."
                });
            }

            // Menghapus data dosen dari database karena tidak memiliki tanggungan mahasiswa wali.
            _context.Dosen.Remove(dosen);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
