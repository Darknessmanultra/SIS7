using System.ComponentModel.DataAnnotations;

namespace BackendSis7.Models
{
    public class Administrador
    {
        [Key]
        [EmailAddress]
        public string email {get;set;}
        public string nombre {get;set;}
        public string passwordHash {get;set;}
    }
}