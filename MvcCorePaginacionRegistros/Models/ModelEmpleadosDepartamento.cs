using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCorePaginacionRegistros.Models
{
    public class ModelEmpleadosDepartamento
    {
        public int IdDepartamento { get; set; }
        public string Nombre { get; set; }
        public string Localidad { get; set; }
        public List<Empleado> Empleados { get; set; }
    }
}
