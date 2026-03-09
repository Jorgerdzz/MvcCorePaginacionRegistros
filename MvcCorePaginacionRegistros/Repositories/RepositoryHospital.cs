using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

#region VIEWS STORED PROCEDURE
//select* from
//(select ROW_NUMBER() over (order by DEPT_NO) as POSICION,
//	DEPT_NO, DNOMBRE, LOC from DEPT) as QUERY
//	where QUERY.POSICION = 2

//alter view V_DEPARTAMENTOS_INDIVIDUAL
//as
//	select cast(ROW_NUMBER() over (order by DEPT_NO) as int) as POSICION,
//    DEPT_NO, DNOMBRE, LOC from DEPT
//go

//select * from V_DEPARTAMENTOS_INDIVIDUAL where POSICION = 1

//create procedure SP_GRUPO_DEPARTAMENTOS
//(@posicion int)
//as
//	select DEPT_NO, DNOMBRE, LOC from V_DEPARTAMENTOS_INDIVIDUAL
//	where POSICION >= @posicion and POSICION < (@posicion + 2)
//go

//exec SP_GRUPO_DEPARTAMENTOS 1

/////EMPLEADOS///////

//create view V_GRUPO_EMPLEADOS
//as
//	select cast(ROW_NUMBER() over (order by EMP_NO) as int) as POSICION,
//    EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO from EMP
//go

//select * from V_GRUPO_EMPLEADOS where POSICION = 1

//create procedure SP_GRUPO_EMPLEADOS
//(@posicion int)
//as
//	select * from V_GRUPO_EMPLEADOS where POSICION >= @posicion and POSICION < (@posicion + 3)
//go

//alter procedure SP_GRUPO_EMPLEADOS_OFICIO
//(@posicion int, @oficio nvarchar(50), @registros int out)
//as
//	select @registros = cast(count(EMP_NO) as int) from EMP where OFICIO = @oficio
//	select EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO from
//	(select cast(ROW_NUMBER() over (order by APELLIDO) as int) as POSICION,
//        EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO from EMP
//		where OFICIO = @oficio) QUERY
//		where (QUERY.POSICION >= @posicion and QUERY.POSICION < (@posicion + 3))
//go

//ALTER view V_EMPLEADOS_DEPARTAMENTO
//as
//	select cast(ROW_NUMBER() over (order by EMP_NO) as int) as POSICION,
//    E.EMP_NO, E.APELLIDO, E.OFICIO, E.SALARIO from EMP as E
//go

//select * from V_DEPARTAMENTOS_INDIVIDUAL where POSICION = 1

//create procedure SP_GRUPO_EMPLEADOS_DEPARTAMENTO
//(@posicion int)
//as
//	select EMP_NO, APELLIDO, OFICIO, SALARIO from V_EMPLEADOS_DEPARTAMENTO
//	where POSICION <= @posicion
//go
#endregion

namespace MvcCorePaginacionRegistros.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<int> GetNumeroRegistrosVistaDepartamentosAsync()
        {
            return await this.context.VistaDepartamentos.CountAsync();
        }

        public async Task<VistaDepartamento> GetVistaDepartamentoAsync(int posicion)
        {
            return await this.context.VistaDepartamentos.Where(d => d.Posicion == posicion)
                .FirstOrDefaultAsync();
        }

        public async Task<List<VistaDepartamento>> GetGrupoVistaDepartamentoAsync(int posicion)
        {
            var consulta = from datos in this.context.VistaDepartamentos
                           where datos.Posicion >= posicion && datos.Posicion < (posicion + 2)
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task<List<Departamento>> GetGrupoDepartamentosAsync(int posicion)
        {
            string sql = "SP_GRUPO_DEPARTAMENTOS @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Departamentos.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetEmpleadosCountAsync()
        {
            return await this.context.Empleados.CountAsync();
        }

        public async Task<List<Empleado>> GetGrupoEmpleadosAsync(int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetEmpleadosOficioCountAsync(string oficio)
        {
            return await this.context.Empleados
                .Where(e => e.Oficio == oficio).CountAsync();
        }

        public async Task<List<Empleado>> GetGrupoEmpleadosOficioAsync(int posicion, string oficio)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @posicion, @oficio";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamOficio);
            return await consulta.ToListAsync();
        }

        public async Task<ModelEmpleadosOficio> GetGrupoEmpleadosOficioOutAsync(int posicion, string oficio)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @posicion, @oficio, @registros out";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);
            SqlParameter pamRegistros = new SqlParameter("@registros", 0);
            pamRegistros.DbType = System.Data.DbType.Int32;
            pamRegistros.Direction = System.Data.ParameterDirection.Output;
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamOficio, pamRegistros);
            List<Empleado> empleados = await consulta.ToListAsync();
            int registros = (int)pamRegistros.Value;
            return new ModelEmpleadosOficio
            {
                Empleados = empleados,
                NumeroRegistros = registros
            };
        }

        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            return await this.context.Departamentos.ToListAsync();
        }
        public async Task<List<Empleado>> GetEmpleadosAsync(int idDepartamento)
        {
            return await this.context.Empleados
                .Where(e => e.IdDepartamento == idDepartamento)
                .ToListAsync();
        }


        public async Task<ModelEmpleadosDepartamento> GetEmpleadosDepartamentoAsync(int idDepartamento)
        {
            Departamento dept = await this.context.Departamentos
                .Where(d => d.IdDepartamento == idDepartamento)
                .FirstOrDefaultAsync();

            if (dept == null)
            {
                return null;
            }

            // 2. Creamos nuestro objeto modelo y le pasamos los datos
            ModelEmpleadosDepartamento model = new ModelEmpleadosDepartamento
            {
                IdDepartamento = dept.IdDepartamento, // Ajusta los nombres si en tu clase Departamento se llaman distinto
                Nombre = dept.Nombre,
                Localidad = dept.Localidad
            };

            return model;
        }

        public async Task<List<Empleado>> GetGrupoEmpleadosDepartamentoAsync(int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS_DEPARTAMENTO @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }



    }
}
