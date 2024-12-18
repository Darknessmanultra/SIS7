using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BackendSis7.Models;
using BackendSis7.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using BackendSis7.DTOs;
using System.Reflection.Metadata.Ecma335;

namespace BackendSis7.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public AuthController(ApiDbContext context)
        {
            _context=context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Administrador loginUser)
        {
        var user = _context.Admin.SingleOrDefault(u => u.nombre == loginUser.nombre && u.passwordHash == loginUser.passwordHash);

        if (user == null) return Unauthorized();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("YourSuperSecretKey");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.nombre) }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "YourIssuer",
            Audience = "YourAudience",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { Token = tokenHandler.WriteToken(token) });
        }
        
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Empleado>>> GetEmpleados()
        {
            var empleados= await _context.Empleados.ToListAsync();
            return empleados;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Empleado>> PostEmpleado([FromBody] EmpleadoDTO dto, SueldoDTO sto)
        {
            if(await _context.Empleados.FirstOrDefaultAsync(e=>e.nombre==dto.nombre)!=null)
            {
                return BadRequest("Nombre ya utilizado");
            }
            var empleo = new Empleado
            {
                IdTrabajador=Guid.NewGuid(),
                nombre=dto.nombre,
                CargasFamiliares=dto.CargasFamiliares,
                isapreNombre=dto.isapreNombre,
                AFPNombre=dto.AFPNombre,
                tipoContrato=dto.tipoContrato
            };
            var suelo = new Sueldo
            {
                IdTrabajador=empleo.IdTrabajador,
                Mes=sto.Mes,
                SueldoBase=sto.SueldoBase,
                HorasExtra=sto.HorasExtra
            };
            _context.Empleados.Add(empleo);
            _context.Sueldos.Add(suelo);
            await _context.SaveChangesAsync();
            return Created();
        }
        [Authorize]
        [HttpGet("HorasExtra/{id}")]
        public async Task<ActionResult<double>> HorasExtra(Guid id, HorasDTO dto)
        {
            var trabaja = await _context.Empleados.FindAsync(id);
            if(trabaja!=null) return NotFound();
            var money = await _context.Sueldos.FindAsync(id);
            double gratis=(1.5*money.SueldoBase*money.HorasExtra*dto.Weekdays)/(dto.Monthdays*dto.Weekhours);
            return gratis;
        }
        [Authorize]
        [HttpGet("gratificacion/{id}")]
        public async Task<ActionResult<double>> Gratificacion(Guid id)
        {
            var trabaja = await _context.Empleados.FindAsync(id);
            if(trabaja!=null) return NotFound();
            var money = await _context.Sueldos.FindAsync(id);
            string tipo=trabaja.tipoContrato;
            double gratis=0;
            if(money.SueldoBase>400000 &&trabaja.CargasFamiliares>2 && tipo=="INDEFINIDO")
            {
                gratis=money.SueldoBase*0.25;
            }
            if(money.SueldoBase<400000 || money.HorasExtra<10 || tipo=="PLAZO")
            {
                gratis=money.SueldoBase*0.15;
            }
            return gratis;
        }
        [Authorize]
        [HttpGet("haberes/{id}")]
        public async Task<ActionResult<double>> TotalHaberes(Guid id)
        {
            var trabaja = await _context.Sueldos.FindAsync(id);
            if(trabaja!=null) return NotFound();
            double egg=trabaja.SueldoBase;
            return egg;
        }
    }
}