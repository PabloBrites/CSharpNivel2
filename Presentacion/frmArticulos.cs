using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;

namespace Presentacion
{
    public partial class frmArticulos : Form
    {

        private List<articulo> listaArticulos; //se está utilizando la lista listaArticulos para almacenar los datos recuperados de la base de datos a través del método listar()
        public frmArticulos()
        {
            InitializeComponent();
        }

        private void frmArticulos_Load(object sender, EventArgs e)//aca accedemos invocamos trabajamos con la lectura a la base de datos
        {
            cargar();
            cboCampo.Items.Add("Codigo");
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Descripcion");


        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e) //evento selecion SelectionChanged vas al rayito(eventos) lo apretas y te sale, cuando cambias la seleccion de la grilla del dgv te cambia la imagen
        {
            if (dgvArticulos.CurrentRow != null)
            {
                articulo seleccionado = (articulo)dgvArticulos.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.ImagenUrl);
            }
        }

        private void cargar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                listaArticulos = negocio.listar();
                dgvArticulos.DataSource = listaArticulos;
                ocultarColumnas();
                dgvArticulos.Columns["Precio"].DefaultCellStyle.Format = "N2"; //saca dos ceros al Precio
                cargarImagen(listaArticulos[0].ImagenUrl);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void ocultarColumnas()
        {
            dgvArticulos.Columns["ImagenUrl"].Visible = false;//oculta url
            dgvArticulos.Columns["Id"].Visible = false;
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbxArticulos.Load(imagen);
            }
            catch (Exception ex )
            {

                pbxArticulos.Load("https://www.getbgd.com/wp-content/uploads/2017/05/placeholder-image.png");
            }
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            FrmAltaArticulo alta = new FrmAltaArticulo();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            articulo seleccionado;
            seleccionado = (articulo)dgvArticulos.CurrentRow.DataBoundItem;
            FrmAltaArticulo modificar = new FrmAltaArticulo(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            articulo seleccionado;
            try
            {
                DialogResult respuesta = MessageBox.Show("¿Estás seguro que queres eliminar este artículo?" ,"Eliminado",MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if(respuesta == DialogResult.Yes)
                {
                    seleccionado = (articulo)dgvArticulos.CurrentRow.DataBoundItem;
                    negocio.eliminar(seleccionado.Id);
                    cargar();
                }
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private bool validarFiltro()
        {
            if(cboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el campo para filtrar.");
                return true;
            }
            if(cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el criterio para filtrar.");
                return true;
            }
            if (cboCampo.SelectedItem.ToString() == "Codigo")
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(txtFiltroAvanzado.Text, @"^[a-zA-Z0-9]{3}$"))
                {
                    MessageBox.Show("El código debe ser una combinación de 3 letras y/o números (por ejemplo, K09 o FF5).");
                    return true;
                }

                if (!soloLetrasYNumeros(txtFiltroAvanzado.Text))
                {                  
                    return true;
                }
            }

            return false;
        }
        private bool soloLetrasYNumeros(string cadena)
        {
            // Validación con Regex.
            if (!System.Text.RegularExpressions.Regex.IsMatch(cadena, @"^[a-zA-Z0-9]+$"))
                return false;

            // Validación con foreach como respaldo (por si decides usarlo en lugar de Regex).
            foreach (char caracter in cadena)
            {
                if (!char.IsLetterOrDigit(caracter))
                    return false;
            }

            // Si pasa ambas validaciones, la cadena es válida.
            return true;
        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {   
                if (validarFiltro()) 
                    return;
                
                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;
                dgvArticulos.DataSource = negocio.filtrar(campo , criterio, filtro);

            }
            catch(NullReferenceException ne) 
            {
                MessageBox.Show(ne.ToString());
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void txtFiltro_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) 
            {
                MessageBox.Show("filtro");
                e.Handled = true; // Opcional: Evita que el Enter tenga otro efecto (como hacer un sonido de error)
            }
        }

       
        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            List<articulo> listaFiltrada;
            string filtro = txtFiltro.Text;

            
            if (filtro.Length >= 3)
            {
                filtro = filtro.ToUpper();
                listaFiltrada = 
                    
              listaArticulos.FindAll(item => {
                    foreach (var propiedad in item.GetType().GetProperties())
                    {
                        var valor = propiedad.GetValue(item, null);

                        if (valor != null)
                        {
                            if (valor is string texto && texto.ToUpper().Contains(filtro))
                            {
                                return true;
                            }
                            else if (!(valor is string) && valor.GetType().IsClass)
                            {
                                
                                foreach (var subPropiedad in valor.GetType().GetProperties())
                                {
                                    var subValor = subPropiedad.GetValue(valor, null);
                                    if (subValor is string subTexto && subTexto.ToUpper().Contains(filtro))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    return false;
                });

            }
            else
            {
                listaFiltrada = listaArticulos;
            }

            dgvArticulos.DataSource = null;
            dgvArticulos.DataSource = listaFiltrada;
            ocultarColumnas();

            dgvArticulos.Columns["Precio"].DefaultCellStyle.Format = "N2";
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();
            if (opcion == "Codigo")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Mayor a");
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            }
            else
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene");
            }
        }

        private void txtFiltroAvanzado_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
