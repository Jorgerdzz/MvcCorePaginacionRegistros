using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;

#region VIEWS STORED PROCEDURE
//--ROW_NUMBER() OVER(ORDER BY COLUMNA)
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

    }
}
