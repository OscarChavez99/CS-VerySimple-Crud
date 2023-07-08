using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal class ProductSql
    {
        private MysqlConnection mySqlConnection;
        private List<Product> productsList;
        public ProductSql()
        {
            mySqlConnection = new MysqlConnection();
            productsList = new List<Product>();
        }
        public List<Product> GetProducts(string filter)
        {
            string QUERY = "SELECT * FROM product ";
            MySqlDataReader mReader = null;
            try
            {
                if (filter != "")
                {
                    QUERY += "WHERE " +
                    "id LIKE '%" + filter + "%' OR " +
                    "name LIKE '%" + filter + "%' OR " +
                    "price LIKE '%" + filter + "%' OR " +
                    "stock LIKE '%" + filter + "%';";
                }
                MySqlCommand mCommand = new MySqlCommand(QUERY);
                mCommand.Connection = mySqlConnection.GetConnection();
                mReader = mCommand.ExecuteReader();

                Product mProduct = null;
                while (mReader.Read())
                {
                    mProduct = new Product();
                    mProduct.id    = mReader.GetInt16("id");
                    mProduct.name  = mReader.GetString("name");
                    mProduct.price = mReader.GetInt16("price");
                    mProduct.stock = mReader.GetInt16("stock");
                    mProduct.image = (byte[]) mReader.GetValue(4);
                    productsList.Add(mProduct);
                }
                mReader.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return productsList;
        }

        internal bool AddProduct(Product mProducto)
        {
            string INSERT = "INSERT INTO product (name, price, stock, image)" +
                "VALUES (@name, @price, @stock, @image);";
            MySqlCommand mCommand = new MySqlCommand(INSERT, mySqlConnection.GetConnection());

            mCommand.Parameters.Add(new MySqlParameter("@name", mProducto.name));
            mCommand.Parameters.Add(new MySqlParameter("@price", mProducto.price));
            mCommand.Parameters.Add(new MySqlParameter("@stock", mProducto.stock));
            mCommand.Parameters.Add(new MySqlParameter("@image", mProducto.image));

            //Si el resultado de la consulta es mayor a 0 se hizo correctamente
            return mCommand.ExecuteNonQuery() > 0;
        }
        internal bool UpdateProduct(Product mProducto)
        {
            string UPDATE = "UPDATE product SET " +
                "name     = @name," +
                "price    = @price," +
                "stock    = @stock," +
                "image    = @image " +
                "WHERE id = @id;";
            MySqlCommand mCommand = new MySqlCommand(UPDATE, mySqlConnection.GetConnection());

            mCommand.Parameters.Add(new MySqlParameter("@name", mProducto.name));
            mCommand.Parameters.Add(new MySqlParameter("@price", mProducto.price));
            mCommand.Parameters.Add(new MySqlParameter("@stock", mProducto.stock));
            mCommand.Parameters.Add(new MySqlParameter("@image", mProducto.image));
            mCommand.Parameters.Add(new MySqlParameter("@id", mProducto.id));

            //Si el resultado de la consulta es mayor a 0 se hizo correctamente
            return mCommand.ExecuteNonQuery() > 0;
        }
        internal bool DeleteProduct(int id)
        {
            string DELETE = "DELETE FROM product WHERE id = @id";
            MySqlCommand mCommand = new MySqlCommand(DELETE, mySqlConnection.GetConnection());
            
            mCommand.Parameters.Add(new MySqlParameter("@id", id));
            
            return mCommand.ExecuteNonQuery() > 0;
        }
    }
}
