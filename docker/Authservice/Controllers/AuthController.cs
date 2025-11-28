using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Authservice.Data;
using Authservice.DTOs;
using Authservice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

namespace Authservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => api/auth
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public AuthController(AuthDbContext context)
        {
            _context = context;
        }

        // ========== Helpers pour le mot de passe ==========

        private string HashPassword(string password, byte[] salt)
        {
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));

            // On stocke "salt.hash"
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);

            var computed = HashPassword(password, salt);

            return storedHash == computed;
        }

        // ========== ENDPOINT REGISTER ==========

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Vérifier si l’email existe déjà
            var existingUser = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (existingUser)
                return BadRequest("Cet email est déjà utilisé.");

            // Générer un salt aléatoire
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            // Hacher le mot de passe
            string passwordHash = HashPassword(dto.Password, salt);

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // On ne renvoie pas le password
            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email
            });
        }

        // ========== ENDPOINT LOGIN ==========

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized("Identifiants incorrects.");

            var isValidPassword = VerifyPassword(dto.Password, user.PasswordHash);

            if (!isValidPassword)
                return Unauthorized("Identifiants incorrects.");

            // Ici plus tard on renverra un JWT
            return Ok(new
            {
                message = "Connexion réussie.",
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email
            });
        }
    }
}

