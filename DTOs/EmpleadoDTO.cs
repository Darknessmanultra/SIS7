namespace BackendSis7.DTOs
{
    public class EmpleadoDTO
    {
        public string nombre {get;set;}
        public int CargasFamiliares {get;set;}
        public string isapreNombre {get;set;}
        public string AFPNombre {get;set;}
        public string tipoContrato {get;set;}
    }
    public class SueldoDTO
    {
        public DateOnly Mes {get;set;}
        public int SueldoBase {get;set;}
        public int HorasExtra{get;set;}
    }

    public class HorasDTO
    {
        public int Weekdays {get;set;}
        public int Monthdays {get;set;}
        public int Weekhours {get;set;}
    }

    public class postEmpleadoDTO
    {
        public string nombre {get;set;}
        public int CargasFamiliares {get;set;}
        public string isapreNombre {get;set;}
        public string AFPNombre {get;set;}
        public string tipoContrato {get;set;}
        public DateOnly Mes {get;set;}
        public int SueldoBase {get;set;}
        public int HorasExtra{get;set;}
    }
}