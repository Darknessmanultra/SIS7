using System.ComponentModel.DataAnnotations;

namespace BackendSis7.Models
{
    public class Empleado
    {
        public string nombre {get;set;}
        [Key]
        public Guid IdTrabajador {get;set;}
        public int CargasFamiliares {get;set;}
        public string isapreNombre {get;set;}
        public string AFPNombre {get;set;}
        public string tipoContrato {get;set;}
    }

    public class Sueldo
    {
        [Key]
        public Guid IdTrabajador {get;set;}
        public DateOnly Mes {get;set;}
        public int SueldoBase {get;set;}
        public int HorasExtra{get;set;}
        public double SueldoFinal{get;set;} =-1;
    }
}