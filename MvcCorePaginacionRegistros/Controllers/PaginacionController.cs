using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;
using System.Threading.Tasks;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class PaginacionController : Controller
    {
        private RepositoryHospital repo;

        public PaginacionController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> RegistroVistaDepartamento(int? posicion)
        {
            if(posicion == null)
            {
                posicion = 1;
            }
            int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();
            int siguiente = posicion.Value + 1;
            if(siguiente > numRegistros)
            {
                siguiente = numRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewData["ULTIMO"] = numRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            VistaDepartamento departamento = await this.repo.GetVistaDepartamentoAsync(posicion.Value);
            return View(departamento);
        }

        public async Task<IActionResult> GrupoVistaDepartamentos(int? posicion)
        {
            if(posicion == null)
            {
                posicion = 1;
            }
            ViewData["NUMPAGINA"] = 1;
            ViewData["NUMREGISTROS"] = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();
            List<VistaDepartamento> departamentos = await this.repo.GetGrupoVistaDepartamentoAsync(posicion.Value);
            return View(departamentos);
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GrupoDepartamentos(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            ViewData["NUMPAGINA"] = 1;
            ViewData["NUMREGISTROS"] = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();
            List<Departamento> departamentos = await this.repo.GetGrupoDepartamentosAsync(posicion.Value);
            return View(departamentos);
        }

        public async Task<IActionResult> GrupoEmpleados(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            ViewData["NUMPAGINA"] = 1;
            ViewData["NUMREGISTROS"] = await this.repo.GetEmpleadosCountAsync();
            List<Empleado> empleados = await this.repo.GetGrupoEmpleadosAsync(posicion.Value);
            return View(empleados);
        }

        public async Task<IActionResult> GrupoEmpleadosOficio(int? posicion, string oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetGrupoEmpleadosOficioAsync(posicion.Value, oficio);
                ViewData["NUMREGISTROS"] = await this.repo.GetEmpleadosOficioCountAsync(oficio);
                ViewData["OFICIO"] = oficio;
                return View(empleados);
            }
                
        }

        [HttpPost]
        public async Task<IActionResult> GrupoEmpleadosOficio(string oficio)
        {
            List<Empleado> empleados = await this.repo.GetGrupoEmpleadosOficioAsync(1, oficio);
            ViewData["NUMREGISTROS"] = await this.repo.GetEmpleadosOficioCountAsync(oficio);
            ViewData["OFICIO"] = oficio;
            return View(empleados);
        }

        public async Task<IActionResult> GrupoEmpleadosOficioOut(int? posicion, string oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {
                ModelEmpleadosOficio model = await this.repo.GetGrupoEmpleadosOficioOutAsync(posicion.Value, oficio);
                ViewData["NUMREGISTROS"] = model.NumeroRegistros;
                ViewData["OFICIO"] = oficio;
                return View(model.Empleados);
            }

        }

        [HttpPost]
        public async Task<IActionResult> GrupoEmpleadosOficioOut(string oficio)
        {
            ModelEmpleadosOficio model = await this.repo.GetGrupoEmpleadosOficioOutAsync(1, oficio);
            ViewData["NUMREGISTROS"] = model.NumeroRegistros;
            ViewData["OFICIO"] = oficio;
            return View(model.Empleados);
        }

        public async Task<IActionResult> EmpleadosDepartamento(int? posicion, int idDepartamento)
        {
            if(posicion == null)
            {
                posicion = 1;
            }
            ModelEmpleadosDepartamento empleadosDepartamento = await this.repo.GetEmpleadosDepartamentoAsync(idDepartamento);
            empleadosDepartamento.Empleados = await this.repo.GetEmpleadosAsync(idDepartamento);
            ViewData["NUMREGISTROS"] = empleadosDepartamento.Empleados.Count;
            int numRegistros = (int)ViewData["NUMREGISTROS"];
            int siguiente = posicion.Value + 1;
            if (siguiente > numRegistros)
            {
                siguiente = numRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewData["PRIMERO"] = posicion;
            ViewData["ULTIMO"] = numRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            return View(empleadosDepartamento);
        }

    }
}
