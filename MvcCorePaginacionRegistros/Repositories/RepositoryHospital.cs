using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;

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

    }
}
