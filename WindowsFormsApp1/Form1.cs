using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private List<Producto> mProductos;
        private Producto mProducto;
        private ProductoConsultas mProductoConsultas;
        public Form1()
        {
            InitializeComponent();
            mProductos = new List<Producto>();
            mProductoConsultas = new ProductoConsultas();
            mProducto = new Producto();

            cargarProductos();
        }

        private void cargarProductos(string filtro = "")
        {
            dgvProductos.Rows.Clear();
            dgvProductos.Refresh();
            mProductos.Clear();
            mProductos = mProductoConsultas.getProductos(filtro);

            for (int i = 0; i < mProductos.Count(); i++)
            {
                dgvProductos.RowTemplate.Height = 100;
                dgvProductos.Rows.Add(
                    mProductos[i].id,
                    mProductos[i].nombre,
                    mProductos[i].precio,
                    mProductos[i].cantidad,
                    Image.FromStream(new MemoryStream(mProductos[i].imagen))
                    );
            }
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            //cargarProductos(txtBusqueda.Text.Trim());
            cargarProductos(txtBusqueda.Text);
        }
        private bool DatosCorrectos()
        {
            //----Nombre----
            if (txtNombre.Text == "")
            {
                MessageBox.Show("Ingrese el nombre");
                return false;
            }
            //----Precio-----
            if (txtPrecio.Text == "")
            {
                MessageBox.Show("Ingrese el precio");
                return false;
            }
            //En caso de haber precio, validar si es númerico
            if (!float.TryParse(txtPrecio.Text.Trim(), out float precio))
            {
                MessageBox.Show("Ingrese un precio decimal");
                return false;
            }
            //----Cantidad-----
            if (txtCantidad.Text == "")
            {
                MessageBox.Show("Ingrese la cantidad");
                return false;
            }
            //En caso de haber precio, validar si es númerico
            if (!int.TryParse(txtCantidad.Text.Trim(), out int cantidad))
            {
                MessageBox.Show("Ingrese cantidad númerica");
                return false;
            }
            
            return true;
        }

        private void LimpiarCampos()
        {
            txtFolio.Text    = "";
            txtNombre.Text   = "";
            txtPrecio.Text   = "";
            txtCantidad.Text = "";
            pbImage.Image = WindowsFormsApp1.Properties.Resources.agregar_imagen;
        }

        private void CargarDatosProducto()
        {
            mProducto.id       = GetFolioIfExist();
            mProducto.nombre   = txtNombre.Text;
            mProducto.precio   = float.Parse(txtPrecio.Text);
            mProducto.cantidad = int.Parse(txtCantidad.Text);
            mProducto.imagen   = ImageToByteArray(pbImage.Image); 
        }

        private int GetFolioIfExist()
        {
            if (txtFolio.Text != "")
            {
                if (int.TryParse(txtFolio.Text, out int folio))
                {
                    return folio;
                }
            }
            return -1;
        }

        private byte[] ImageToByteArray(Image image)
        {
            if (image == null)
                return null;
            MemoryStream mMemoryStream = new MemoryStream();
            image.Save(mMemoryStream, ImageFormat.Png);
            return mMemoryStream.ToArray();
        }

        private void pbImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //Especificar desde donde se abrirá el explorador de archivos
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pbImage.ImageLocation = openFileDialog.FileName;
            }
        }
        //Clic en la tabla
        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvProductos.Rows[e.RowIndex];
            txtFolio.Text = Convert.ToString(row.Cells[0].Value);
            txtNombre.Text = Convert.ToString(row.Cells["nombre"].Value);
            txtPrecio.Text = Convert.ToString(row.Cells[2].Value);
            txtCantidad.Text = Convert.ToString(row.Cells["cantidad"].Value);

            MemoryStream memoryStream = new MemoryStream();
            Bitmap bitmap = (Bitmap)dgvProductos.CurrentRow.Cells[4].Value;
            bitmap.Save(memoryStream, ImageFormat.Png);
            pbImage.Image = Image.FromStream(memoryStream);
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!DatosCorrectos())
                return;

            CargarDatosProducto();
            if (mProductoConsultas.AgregarProducto(mProducto))
            {
                MessageBox.Show("Producto agregado", "Éxito");
                cargarProductos();
                LimpiarCampos();
            }
        }
        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (!DatosCorrectos())
                return;

            CargarDatosProducto();
            if (mProductoConsultas.ModificarProducto(mProducto))
            {
                //Actualizar dataGrid
                MessageBox.Show("Producto modificado", "Éxito");
                cargarProductos();
                LimpiarCampos();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (GetFolioIfExist() == -1)
                MessageBox.Show("No ha seleccionado ningún producto para eliminar", "Error");

            else if (MessageBox.Show("¿Seguro que deseas eliminar '"+txtNombre.Text+ "'?", 
                "Elimnar producto",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                CargarDatosProducto();
                if (mProductoConsultas.EliminarProducto(mProducto.id))
                {
                    MessageBox.Show("Producto eliminado", "Éxito");
                    cargarProductos();
                    LimpiarCampos();
                }
            }
        }
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            MessageBox.Show("Limpio");
        }
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
