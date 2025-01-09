using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace negocio
{
    public class accesoDatos //Esta seria mi clase helper. 
    {
        private SqlConnection conexion; //representa la conexion con mi base de datos
        private SqlCommand comando; //Es el comando SQL que quieres ejecutar (como un SELECT, INSERT, etc.).
        private SqlDataReader lector; //lee los datos que devuelve el comando
        public SqlDataReader Lector
        {
            get { return lector; }
        }
        public accesoDatos()
        {
            conexion = new SqlConnection("server=.\\SQLEXPRESS; database=CATALOGO_DB; integrated security = true");
            comando = new SqlCommand();
        }

        public void setearConsulta(string consulta)
        {
            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = consulta;
        }

        public void ejecutarLectura() 
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                lector = comando.ExecuteReader();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ejecutarAccion()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                comando.ExecuteNonQuery();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void setearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor);
        }


        public void cerrarConexion()
        {
            if (lector!=null) 
                lector.Close();
            conexion.Close();
        }
    }
}
