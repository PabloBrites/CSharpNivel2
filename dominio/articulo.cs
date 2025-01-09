using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dominio
{
    public class articulo
    {
        public int Id { get; set; }
        public string Codigo {  get; set; }
        public string Nombre { get; set; }
        [DisplayName("Descripción")] //personalizar el nombre
        public string Descripcion { get; set; }
        public string ImagenUrl { get; set; }
        public decimal Precio { get; set; }
        public categoria Categoria { get; set; }
        public marca Marca { get; set;}

    }
}


