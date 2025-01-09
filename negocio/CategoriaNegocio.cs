using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dominio;

namespace negocio
{
    public class CategoriaNegocio
    {
        public List<categoria> listar()
        {
			List<categoria>lista = new List<categoria>();
			accesoDatos datos = new accesoDatos();
			try
			{
				datos.setearConsulta("Select Id,Descripcion From CATEGORIAS");
				datos.ejecutarLectura();

				while (datos.Lector.Read())
				{
					categoria aux = new categoria();
					aux.Id = (int)datos.Lector["Id"];
					aux.Descripcion = (String)datos.Lector["Descripcion"];

					lista.Add(aux);
				}



				return lista;
			}
			catch (Exception ex)
			{

				throw ex;
			}
			finally
			{
				datos.cerrarConexion();
			}
        }
    }
}
