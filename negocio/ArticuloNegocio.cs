using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using dominio;
using System.Globalization;




namespace negocio
{
    public class ArticuloNegocio // clase donde yo creo los metodos de acceso a datos para mis articulos
    {
        public List<articulo> listar() 
        { 
            List<articulo> lista = new List<articulo>();
            SqlConnection conexion = new SqlConnection(); //me conecto.
            SqlCommand comando = new SqlCommand(); //me deja realizar acciones.
            SqlDataReader lector; //aloja los datos en un lector.


            try 
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=CATALOGO_DB; integrated security = true";
                comando. CommandType = System.Data.CommandType.Text;
                comando.CommandText = "SELECT Codigo, Nombre, A.Descripcion, ImagenUrl, Precio, C.Descripcion as Categoria, M.Descripcion as Marca, A.IdMarca, A.IdCategoria, A.Id FROM ARTICULOS A, CATEGORIAS C, MARCAS M WHERE C.Id = A.IdCategoria And M.Id = A.IdMarca";
                comando.Connection = conexion;

                conexion.Open();
                lector = comando.ExecuteReader();

                while (lector.Read()) 
                {
                    articulo aux = new articulo();
                    aux.Id = (int)lector["Id"];
                    aux.Codigo = (string)lector["Codigo"];
                    aux.Nombre = (string)lector["Nombre"];
                    aux.Descripcion = (string)lector["Descripcion"];
                    if(!(lector.IsDBNull(lector.GetOrdinal("ImagenUrl")))) //ESTO ES PARA CUANDO IMAGEN ES NULL EN BD
                    aux.ImagenUrl = (string)lector["ImagenUrl"];
                    aux.Precio = (decimal)lector["Precio"];
                    aux.Categoria = new categoria();
                    aux.Categoria.Id = (int)lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)lector["Categoria"];
                    aux.Marca = new marca();
                    aux.Marca.Id = (int)lector["IdMarca"];
                    aux.Marca.Descripcion = (string)lector["Marca"];
                    

                    lista.Add(aux);
                }

                conexion.Close();
                return lista;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }

        }

        public void agregar (articulo nuevo)
        {
            accesoDatos datos = new accesoDatos();

            try
            {
                datos.setearConsulta("INSERT INTO ARTICULOS (Codigo, Nombre, Descripcion, Precio, IdMarca, IdCategoria, ImagenUrl) VALUES ('"+nuevo.Codigo+"','"+nuevo.Nombre+"', '"+nuevo.Descripcion+"', '"+nuevo.Precio+"', @idMarca, @idCategoria, @ImagenUrl)");
                datos.setearParametro("@idMarca", nuevo.Marca.Id);
                datos.setearParametro("idCategoria", nuevo.Categoria.Id);
                datos.setearParametro("@ImagenUrl", nuevo.ImagenUrl);
                datos.ejecutarAccion();
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

        public void modificar(articulo articulo)
        {
            accesoDatos datos = new accesoDatos();
            try
            {
                datos.setearConsulta("update ARTICULOS set Codigo = @codigo, Nombre = @nombre, Descripcion = @desc, ImagenUrl = @img, Precio = @precio, IdMarca = @idMarca, IdCategoria = @idCategoria where Id = @id");
                datos.setearParametro("@codigo", articulo.Codigo);
                datos.setearParametro("@nombre", articulo.Nombre);
                datos.setearParametro("@desc", articulo.Descripcion);
                datos.setearParametro("@img", articulo.ImagenUrl);
                datos.setearParametro("@precio", articulo.Precio);
                datos.setearParametro("@idMarca", articulo.Marca.Id);
                datos.setearParametro("@idCategoria", articulo.Categoria.Id);
                datos.setearParametro("@id", articulo.Id);

                datos.ejecutarAccion();
                
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

        public void eliminar(int id)
        {
            try 
            {
                accesoDatos datos = new accesoDatos();
                datos.setearConsulta("delete from ARTICULOS where id = @Id");
                datos.setearParametro("@id",id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<articulo> filtrar(string campo, string criterio, string filtro)
        {
            List<articulo> lista = new List<articulo>();
            accesoDatos datos = new accesoDatos();
            try
            {
                string consulta = "SELECT Codigo, Nombre, A.Descripcion, ImagenUrl, Precio, C.Descripcion as Categoria, M.Descripcion as Marca, A.IdMarca, A.IdCategoria, A.Id FROM ARTICULOS A, CATEGORIAS C, MARCAS M WHERE C.Id = A.IdCategoria And M.Id = A.IdMarca And ";
                if (campo == "Codigo")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "Codigo > '" + filtro + "'"; // Agregar comillas simples
                            break;
                        case "Menor a":
                            consulta += "Codigo < '" + filtro + "'"; // Agregar comillas simples
                            break;
                        default:
                            consulta += "Codigo = '" + filtro + "'"; // Agregar comillas simples
                            break;
                    }
                }
                else if (campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Nombre like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "Nombre like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "Nombre like '%" + filtro + "%'";
                            break;
                    }
                }
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "A.Descripcion like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "A.Descripcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "A.Descripcion like '%" + filtro + "%'";
                            break;
                    }
                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();
                
                while (datos.Lector.Read())
                {
                    articulo aux = new articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    if (!(datos.Lector.IsDBNull(datos.Lector.GetOrdinal("ImagenUrl")))) //ESTO ES PARA CUANDO IMAGEN ES NULL EN BD
                        aux.ImagenUrl = (string)datos.Lector["ImagenUrl"];
                    aux.Precio = (decimal)datos.Lector["Precio"];
                    aux.Categoria = new categoria();
                    aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];
                    aux.Marca = new marca();
                    aux.Marca.Id = (int)datos.Lector["IdMarca"];
                    aux.Marca.Descripcion = (string)datos.Lector["Marca"];


                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
