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
        
        
        [HttpGet("empleados")]
        public async Task<ActionResult<IEnumerable<Empleado>>> GetEmpleados()
        {
            var empleados= await _context.Empleados.ToListAsync();
            return empleados;
        }

        
        [HttpGet("sueldos/{year}/{month}")]
        public async Task<ActionResult<IEnumerable<Sueldo>>> GetSueldos(int year, int month)
        {
            var sueldos = await _context.Sueldos
                .Where(s => s.Mes.Year == year && s.Mes.Month == month)
                .ToListAsync();

            if (!sueldos.Any()) return NotFound("No se encontraron registros para el mes y año especificados.");

            return Ok(sueldos);
        }

        [HttpPost("Registrar")]
        public async Task<ActionResult> PostEmpleado([FromBody] postEmpleadoDTO dto)
        {
            // Validar datos
            if (dto == null)
            {
                return BadRequest("Los datos del empleado son inválidos.");
            }

            // Crear la entidad Empleado
            var empleo = new Empleado
            {
                IdTrabajador = Guid.NewGuid(),
                nombre = dto.Nombre,
                CargasFamiliares = dto.CargasFamiliares,
                isapreNombre = dto.IsapreNombre,
                AFPNombre = dto.AFPNombre,
                tipoContrato = dto.TipoContrato
            };

            // Crear la entidad Sueldo
            var sueldo = new Sueldo
            {
                IdTrabajador = empleo.IdTrabajador,
                Mes = dto.Mes,
                SueldoBase = dto.SueldoBase,
                HorasExtra = dto.HorasExtra,
                SueldoFinal = 0 // Inicializar SueldoFinal
            };

            try
            {
                // Guardar en la base de datos
                _context.Empleados.Add(empleo);
                _context.Sueldos.Add(sueldo);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(PostEmpleado), new { id = empleo.IdTrabajador }, empleo);
            }
            catch (Exception ex)
            {
                // Manejar errores
                return StatusCode(500, $"Ocurrió un error al registrar el empleado: {ex.Message}");
            }
        }

        [HttpGet("SueldoFinal/{year}/{month}")]
        public async Task<ActionResult<IEnumerable<object>>> GetSueldoFinal(int year, int month)
        {
            var sueldos = await _context.Sueldos
                .Where(s => s.Mes.Year == year && s.Mes.Month == month)
                .ToListAsync();

            if (!sueldos.Any()) return NotFound("No se encontraron registros para el mes y año especificados.");

            var resultados = new List<object>();

            foreach (var sueldo in sueldos)
            {
                var empleado = await _context.Empleados.FindAsync(sueldo.IdTrabajador);
                if (empleado == null) continue;

                // 1. Calcular Horas Extras
                double horasExtras = (1.5 * sueldo.SueldoBase * sueldo.HorasExtra * 5) / (30 * 40);

                // 2. Calcular Gratificación
                double gratificacion = 0;
                if (empleado.tipoContrato == "INDEFINIDO" && sueldo.SueldoBase >= 400000 && empleado.CargasFamiliares > 2)
                {
                    gratificacion = sueldo.SueldoBase * 0.25;
                }
                else if (empleado.tipoContrato == "PLAZO" || sueldo.SueldoBase < 400000 || sueldo.HorasExtra < 10)
                {
                    gratificacion = sueldo.SueldoBase * 0.15;
                }

                // 3. Total de Haberes
                double totalHaberes = sueldo.SueldoBase + horasExtras + gratificacion;

                // 4. Descuento AFP
                double descuentoAFP = empleado.AFPNombre switch
                {
                    "MODELO" => 0.07,
                    "PROVIDA" => 0.05,
                    "HABITAT" => 0.06,
                    "CAPITAL" => 0.08,
                    "PLANVITAL" => 0.075,
                    _ => 0.07
                };

                // 5. Descuento Isapre
                double descuentoIsapre = empleado.isapreNombre switch
                {
                    "FONASA" => 0.07,
                    "CONSALUD" => 0.09,
                    "BANMEDICA" => 0.09,
                    "CRUZ BLANCA" => 0.05,
                    _ => 0.07
                };

                // 6. Leyes Sociales
                double leyesSociales = descuentoAFP + descuentoIsapre;

                // Sueldo Final
                sueldo.SueldoFinal = (int)(totalHaberes * (1 - leyesSociales));

                // Actualizar en la base de datos
                _context.Sueldos.Update(sueldo);
                await _context.SaveChangesAsync();

                resultados.Add(new
                {
                    Empleado = empleado.nombre,
                    SueldoBase = sueldo.SueldoBase,
                    HorasExtras = horasExtras,
                    Gratificacion = gratificacion,
                    TotalHaberes = totalHaberes,
                    LeyesSociales = leyesSociales,
                    SueldoFinal = sueldo.SueldoFinal
                });
            }

            return Ok(resultados);
        }

        [HttpPost("Excel/{year}/{month}")]
        public async Task<IActionResult> ImportarExcel(IFormFile file, int year, int month)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No se ha proporcionado un archivo.");
            }
            try
            {
                // Validar año y mes
                if (year < 1 || month < 1 || month > 12)
                {
                    return BadRequest("El año o mes proporcionado no son válidos.");
                }

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new OfficeOpenXml.ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            return BadRequest("El archivo Excel no contiene hojas.");
                        }

                        // Verificar nombres de columnas requeridas
                        var columnasRequeridas = new List<string>
                        {
                            "NOMBRE",
                            "CONTRATO EMPLEADO",
                            "SUELDO BASE",
                            "CANTIDAD HRS. EXTRAS",
                            "CARGAS FAMILIARES",
                            "A.F.P",
                            "ISAPRE"
                        };

                        var columnasEnExcel = new Dictionary<string, int>();
                        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        {
                            var columna = worksheet.Cells[1, col].Text.Trim();
                            if (columnasRequeridas.Contains(columna.ToUpper()))
                            {
                                columnasEnExcel[columna.ToUpper()] = col;
                            }
                        }

                        var columnasFaltantes = columnasRequeridas.Except(columnasEnExcel.Keys).ToList();
                        if (columnasFaltantes.Any())
                        {
                            return BadRequest($"Faltan las siguientes columnas en el archivo Excel: {string.Join(", ", columnasFaltantes)}");
                        }

                        // Procesar filas y registrar empleados
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var nombre = worksheet.Cells[row, columnasEnExcel["NOMBRE"]].Text.Trim();
                            var contrato = worksheet.Cells[row, columnasEnExcel["CONTRATO EMPLEADO"]].Text.Trim().ToUpper();
                            var sueldoBase = worksheet.Cells[row, columnasEnExcel["SUELDO BASE"]].Text.Trim();
                            var horasExtras = worksheet.Cells[row, columnasEnExcel["CANTIDAD HRS. EXTRAS"]].Text.Trim();
                            var cargasFamiliares = worksheet.Cells[row, columnasEnExcel["CARGAS FAMILIARES"]].Text.Trim();
                            var afp = worksheet.Cells[row, columnasEnExcel["A.F.P"]].Text.Trim();
                            var isapre = worksheet.Cells[row, columnasEnExcel["ISAPRE"]].Text.Trim();

                            // Validaciones de los campos
                            if (string.IsNullOrEmpty(nombre) ||
                                !new[] { "PLAZO", "INDEFINIDO" }.Contains(contrato) ||
                                !int.TryParse(sueldoBase, out int sueldo) || sueldo <= 0 ||
                                !int.TryParse(horasExtras, out int horas) || horas < 0 ||
                                !int.TryParse(cargasFamiliares, out int cargas) || cargas < 0 ||
                                string.IsNullOrEmpty(afp) ||
                                string.IsNullOrEmpty(isapre))
                            {
                                return BadRequest($"Error en los datos de la fila {row}. Verifique que todos los campos cumplan con los requisitos.");
                            }

                            // Crear entidad Empleado
                            var empleado = new Empleado
                            {
                                IdTrabajador = Guid.NewGuid(),
                                nombre = nombre,
                                CargasFamiliares = cargas,
                                isapreNombre = isapre,
                                AFPNombre = afp,
                                tipoContrato = contrato
                            };

                            // Crear entidad Sueldo
                            var dia = DateTime.Now.Day; // Tomar el día actual
                            var sueldoEntidad = new Sueldo
                            {
                                IdTrabajador = empleado.IdTrabajador,
                                Mes = new DateOnly(year, month, dia), // Convertir a DateOnly
                                SueldoBase = sueldo,
                                HorasExtra = horas,
                                SueldoFinal = 0 // Inicializado, se calculará después
                            };
                            // Guardar en la base de datos
                            _context.Empleados.Add(empleado);
                            _context.Sueldos.Add(sueldoEntidad);
                        }

                        await _context.SaveChangesAsync();
                        return Ok("Los datos del archivo Excel se han importado y registrado correctamente.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al procesar el archivo: {ex.Message}");
            }
        }


    }
}