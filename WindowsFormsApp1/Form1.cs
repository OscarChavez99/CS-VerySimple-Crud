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

            LoadDataGrid();
        }

        private void LoadDataGrid(string filter = "")
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
            LoadDataGrid(txtSearch.Text);
        }
        private bool CorrectData()
        {
            //----Nombre----
            if (txtName.Text == "")
            {
                MessageBox.Show("Empty name", "Error");
                return false;
            }
            //----Precio-----
            if (txtPrice.Text == "")
            {
                MessageBox.Show("Empty price", "Error");
                return false;
            }
            //En caso de haber precio, validar si es númerico
            if (!float.TryParse(txtPrice.Text.Trim(), out float precio))
            {
                MessageBox.Show("Price must be numeric/decimal value", "Error");
                return false;
            }
            //----Cantidad-----
            if (txtStock.Text == "")
            {
                MessageBox.Show("Empty stock", "Error");
                return false;
            }
            //En caso de haber precio, validar si es númerico
            if (!int.TryParse(txtStock.Text.Trim(), out int cantidad))
            {
                MessageBox.Show("Stock must be numeric value", "Error");
                return false;
            }
            
            return true;
        }

        private void ClearText()
        {
            txtID.Text    = "";
            txtName.Text   = "";
            txtPrice.Text   = "";
            txtStock.Text = "";
            pbImage.Image = WindowsFormsApp1.Properties.Resources.agregar_imagen;
            txtAgregarImagen.Text = "Add image:";
        }

        private void LoadProductObject()
        {
            mProduct.id    = GetFolioIfExist();
            mProduct.name  = txtName.Text;
            mProduct.price = float.Parse(txtPrice.Text);
            mProduct.stock = int.Parse(txtStock.Text);
            mProduct.image = ImageToByteArray(pbImage.Image); 
        }

        private int GetFolioIfExist()
        {
            if (txtID.Text != "")
            {
                if (int.TryParse(txtID.Text, out int folio))
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
                txtAgregarImagen.Text = "Image:";
            }
        }
        //Clic en la tabla
        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvProducts.Rows[e.RowIndex];
            txtID.Text = Convert.ToString(row.Cells[0].Value);
            txtName.Text = Convert.ToString(row.Cells["NameHeader"].Value);
            txtPrice.Text = Convert.ToString(row.Cells[2].Value);
            txtStock.Text = Convert.ToString(row.Cells["StockHeader"].Value);

            MemoryStream memoryStream = new MemoryStream();
            Bitmap bitmap = (Bitmap)dgvProducts.CurrentRow.Cells[4].Value;
            bitmap.Save(memoryStream, ImageFormat.Png);
            pbImage.Image = Image.FromStream(memoryStream);
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!CorrectData())
                return;

            LoadProductObject();
            if (mProductSql.AddProduct(mProduct))
            {
                MessageBox.Show("Product added!", "Success");
                LoadDataGrid();
                ClearText();
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!CorrectData())
                return;

            LoadProductObject();
            if (mProductSql.UpdateProduct(mProduct))
            {
                //Actualizar dataGrid
                MessageBox.Show("Product Updated", "Success");
                LoadDataGrid();
                ClearText();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (GetFolioIfExist() == -1)
                MessageBox.Show("No selected item to delete", "Error");

            else if (MessageBox.Show("Are you sure you want to delete '" + txtName.Text+ "'?", 
                "Delete product",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                LoadProductObject();
                if (mProductSql.DeleteProduct(mProduct.id))
                {
                    MessageBox.Show("Product deleted", "Success");
                    LoadDataGrid();
                    ClearText();
                }
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearText();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
