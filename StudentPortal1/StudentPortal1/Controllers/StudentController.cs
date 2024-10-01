using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentPortal1.Data;
using StudentPortal1.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StudentPortal1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<Student> _passwordHasher;
        private readonly byte[] _encryptionKey;
        public StudentController(StudentDbContext context, IConfiguration configuration, IPasswordHasher<Student> passwordHasher, IOptions<EncryptionSettings> encryptionSettings)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _encryptionKey = Encoding.UTF8.GetBytes(encryptionSettings.Value.key);
        }




        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            Console.WriteLine($"Received encrypted password: {login.Password}");

            


            try
            {

                var decryptPassword = DecryptPassword(login.Password);
                var student = await _context.Students.SingleOrDefaultAsync(x => x.Email == login.Email && x.Password == decryptPassword);

                
                
            if (student == null)
            {
                return Unauthorized("Invalid email or password.");
            }
                
                return Ok();
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"FormatException: {ex.Message}");
                return BadRequest("Invalid encrypted password format.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "An error occurred during login.");
            }
        }
      /*  private string GenerateJWTToken(Student student)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
                    new Claim(ClaimTypes.Email, student.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }*/

        private string DecryptPassword(string encryptedPassword)
        {
            var encryptionKey = "jafgjj21565123ya$@#%@$&^sh#@#%$^"; 
            var fullCipher = Convert.FromBase64String(encryptedPassword);

            var key = Encoding.UTF8.GetBytes(encryptionKey);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7; 

                using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }



        


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

       
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
           /* student.Password = _passwordHasher.HashPassword(student, student.Password);*/
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

       
        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<Student>> GetStudentByEmail(string email)
        {
            var student = await _context.Students.SingleOrDefaultAsync(x => x.Email == email);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        [HttpPost("pay")]
        public async Task<IActionResult> PayFee([FromBody] FeeDto feeDto)
        {
            if (feeDto == null)
            {
                return BadRequest("Invalid fee data.");
            }

            
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == feeDto.Email);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

           
           
            student.Course=feeDto.Course;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Fee payment updated successfully.");
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
