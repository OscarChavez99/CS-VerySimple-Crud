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
        private List<Product> productsList;
        private Product mProduct;
        private ProductSql mProductSql;
        public Form1()
        {
            InitializeComponent();
            productsList = new List<Product>();
            mProductSql  = new ProductSql();
            mProduct     = new Product();

            LoadProducts();
        }

        private void LoadProducts(string filter = "")
        {
            dgvProducts.Rows.Clear();
            dgvProducts.Refresh();
            productsList.Clear();
            productsList = mProductSql.GetProducts(filter);

            for (int i = 0; i < productsList.Count(); i++)
            {
                dgvProducts.RowTemplate.Height = 100;
                dgvProducts.Rows.Add(
                    productsList[i].id,
                    productsList[i].name,
                    productsList[i].price,
                    productsList[i].stock,
                    Image.FromStream(new MemoryStream(productsList[i].image))
                    );
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            //cargarProductos(txtBusqueda.Text.Trim());
            LoadProducts(txtSearch.Text);
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
            txtAgregarImagen.Text = "Agregar imagen:";
        }

        private void CargarDatosProducto()
        {
            mProduct.id    = GetFolioIfExist();
            mProduct.name  = txtNombre.Text;
            mProduct.price = float.Parse(txtPrecio.Text);
            mProduct.stock = int.Parse(txtCantidad.Text);
            mProduct.image = ImageToByteArray(pbImage.Image); 
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
                txtAgregarImagen.Text = "Imagen:";
            }
        }
        //Clic en la tabla
        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvProducts.Rows[e.RowIndex];
            txtFolio.Text = Convert.ToString(row.Cells[0].Value);
            txtNombre.Text = Convert.ToString(row.Cells["nombre"].Value);
            txtPrecio.Text = Convert.ToString(row.Cells[2].Value);
            txtCantidad.Text = Convert.ToString(row.Cells["cantidad"].Value);

            MemoryStream memoryStream = new MemoryStream();
            Bitmap bitmap = (Bitmap)dgvProducts.CurrentRow.Cells[4].Value;
            bitmap.Save(memoryStream, ImageFormat.Png);
            pbImage.Image = Image.FromStream(memoryStream);
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!DatosCorrectos())
                return;

            CargarDatosProducto();
            if (mProductSql.AddProduct(mProduct))
            {
                MessageBox.Show("Producto agregado", "Éxito");
                LoadProducts();
                LimpiarCampos();
            }
        }
        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (!DatosCorrectos())
                return;

            CargarDatosProducto();
            if (mProductSql.UpdateProduct(mProduct))
            {
                //Actualizar dataGrid
                MessageBox.Show("Producto modificado", "Éxito");
                LoadProducts();
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
                if (mProductSql.DeleteProduct(mProduct.id))
                {
                    MessageBox.Show("Producto eliminado", "Éxito");
                    LoadProducts();
                    LimpiarCampos();
                }
            }
        }
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
