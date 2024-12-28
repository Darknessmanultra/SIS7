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
        public IActionResult Login([FromBody] AdminDTO loginUser)
        {
        var user = _context.Admin.Find(loginUser.email);

        if (user==null) return NotFound();
        if (!BCrypt.Net.BCrypt.Verify(loginUser.password,user.passwordHash)) return Unauthorized();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("Proton-Enhanced_Nuclear_Induction_Spectroscopy");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, user.email) }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "YourIssuer",
            Audience = "YourAudience",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { Token = tokenHandler.WriteToken(token) });
        }
        
        [Authorize]
        [HttpGet("empleados")]
        public async Task<ActionResult<IEnumerable<Empleado>>> GetEmpleados()
        {
            var empleados= await _context.Empleados.ToListAsync();
            return empleados;
        }

        [Authorize]
        [HttpGet("sueldos")]
        public async Task<ActionResult<IEnumerable<Sueldo>>> GetSueldos()
        {
            var sueldos= await _context.Sueldos.ToListAsync();
            return sueldos;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Empleado>> PostEmpleado([FromBody] postEmpleadoDTO dto)
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
                Mes=dto.Mes,
                SueldoBase=dto.SueldoBase,
                HorasExtra=dto.HorasExtra
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
            if(trabaja==null) return NotFound();
            var money = await _context.Sueldos.FindAsync(id);
            double gratis=(1.5*money.SueldoBase*money.HorasExtra*dto.Weekdays)/(dto.Monthdays*dto.Weekhours);
            return gratis;
        }
        [Authorize]
        [HttpGet("gratificacion/{id}")]
        public async Task<ActionResult<double>> Gratificacion(Guid id)
        {
            var trabaja = await _context.Empleados.FindAsync(id);
            if(trabaja==null) return NotFound();
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
            if(trabaja==null) return NotFound();
            double egg=trabaja.SueldoFinal+trabaja.SueldoBase;
            return egg;
        }
        [Authorize]
        [HttpGet("isapre-desc/{id}")]
        public async Task<ActionResult<double>> DescuentoIsapre(Guid id)
        {
            var trabaja = await _context.Empleados.FindAsync(id);
            if(trabaja!=null) return NotFound();
            double egg=0;
            if(trabaja.AFPNombre=="PROVIDA")
            {
                egg=0.5;
            }
            if(trabaja.AFPNombre=="MAGISTER")
            {
                egg=0.9;
            }
            return egg;
        }
        [Authorize]
        [HttpGet("AFP-desc/{id}")]
        public async Task<ActionResult<double>> DescuentoAFP(Guid id)
        {
            var trabaja = await _context.Empleados.FindAsync(id);
            if(trabaja!=null) return NotFound();
            double egg=0;
            if(trabaja.isapreNombre=="CRUZ BLANCA")
            {
                egg=0.5;
            }
            if(trabaja.isapreNombre=="BANMEDICA")
            {
                egg=0.9;
            }
            if(trabaja.isapreNombre=="CONSALUD")
            {
                egg=0.9;
            }
            return egg;
        }
        [Authorize]
        [HttpGet("LeyesSociales/{id}")]
        public async Task<ActionResult<double>> LeyesSociales(Guid id)
        {
            var trabaja = await _context.Empleados.FindAsync(id);
            if(trabaja!=null) return NotFound();
            double egg=0;
            double ham=0;
            if(trabaja.isapreNombre=="CRUZ BLANCA")
            {
                egg=0.5;
            }
            if(trabaja.isapreNombre=="BANMEDICA")
            {
                egg=0.9;
            }
            if(trabaja.isapreNombre=="CONSALUD")
            {
                egg=0.9;
            }
            if(trabaja.AFPNombre=="PROVIDA")
            {
                ham=0.5;
            }
            if(trabaja.AFPNombre=="MAGISTER")
            {
                ham=0.9;
            }
            var suelo = await _context.Sueldos.FindAsync(id);
            suelo.SueldoFinal*=(egg+ham);
            _context.Sueldos.Update(suelo);
            await _context.SaveChangesAsync();
            return egg+ham;
        }

        [Authorize]
        [HttpGet("SueldoFinal/{id}")]
        public async Task<ActionResult<double>> SueldoFinal(Guid id,HorasDTO dto)
        {
            var trabaja = await _context.Empleados.FindAsync(id);
            if(trabaja==null) return NotFound();
            var money = await _context.Sueldos.FindAsync(id);
            double gratis=(1.5*money.SueldoBase*money.HorasExtra*dto.Weekdays)/(dto.Monthdays*dto.Weekhours);
            double gratis2=0;
            string tipo=trabaja.tipoContrato;
            if(money.SueldoBase>400000 &&trabaja.CargasFamiliares>2 && tipo=="INDEFINIDO")
            {
                gratis=money.SueldoBase*0.25;
            }
            if(money.SueldoBase<400000 || money.HorasExtra<10 || tipo=="PLAZO")
            {
                gratis=money.SueldoBase*0.15;
            }
            double egger=money.SueldoBase+gratis+gratis2;
            double egg=0;
            double ham=0;
            if(trabaja.isapreNombre=="CRUZ BLANCA")
            {
                egg=0.5;
            }
            if(trabaja.isapreNombre=="BANMEDICA")
            {
                egg=0.9;
            }
            if(trabaja.isapreNombre=="CONSALUD")
            {
                egg=0.9;
            }
            if(trabaja.AFPNombre=="PROVIDA")
            {
                ham=0.5;
            }
            if(trabaja.AFPNombre=="MAGISTER")
            {
                ham=0.9;
            }
            money.SueldoFinal= egger*(egg+ham);
            _context.Sueldos.Update(money);
            await _context.SaveChangesAsync();
            return egg+ham;
        }
    }
}