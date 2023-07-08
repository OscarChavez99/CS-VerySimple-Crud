using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal class ProductoConsultas
    {
        private ConexionMysql conexionMysql;
        private List<Producto> mProductos;
        public ProductoConsultas()
        {
            conexionMysql = new ConexionMysql();
            mProductos = new List<Producto>();
        }
        public List<Producto> getProductos(string filtro)
        {
            string QUERY = "SELECT * FROM producto ";
            MySqlDataReader mReader = null;
            try
            {
                if (filtro != "")
                {
                    QUERY += "WHERE " +
                    "id LIKE '%" + filtro + "%' OR " +
                    "nombre LIKE '%" + filtro + "%' OR " +
                    "precio LIKE '%" + filtro + "%' OR " +
                    "cantidad LIKE '%" + filtro + "%';";
                }
                MySqlCommand mComando = new MySqlCommand(QUERY);
                mComando.Connection = conexionMysql.GetConnection();
                mReader = mComando.ExecuteReader();

                Producto mProducto = null;
                while (mReader.Read())
                {
                    mProducto = new Producto();
                    mProducto.id       = mReader.GetInt16("id");
                    mProducto.nombre   = mReader.GetString("nombre");
                    mProducto.precio   = mReader.GetInt16("precio");
                    mProducto.cantidad = mReader.GetInt16("cantidad");
                    mProducto.imagen   = (byte[]) mReader.GetValue(4);
                    mProductos.Add(mProducto);
                }
                mReader.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return mProductos;
        }

        internal bool AgregarProducto(Producto mProducto)
        {
            string INSERT = "INSERT INTO producto (nombre, precio, cantidad, imagen)" +
                "VALUES (@nombre, @precio, @cantidad, @imagen);";
            MySqlCommand mCommand = new MySqlCommand(INSERT, conexionMysql.GetConnection());

            mCommand.Parameters.Add(new MySqlParameter("@nombre", mProducto.nombre));
            mCommand.Parameters.Add(new MySqlParameter("@precio", mProducto.precio));
            mCommand.Parameters.Add(new MySqlParameter("@cantidad", mProducto.cantidad));
            mCommand.Parameters.Add(new MySqlParameter("@imagen", mProducto.imagen));

            //Si el resultado de la consulta es mayor a 0 se hizo correctamente
            return mCommand.ExecuteNonQuery() > 0;
        }
        internal bool ModificarProducto(Producto mProducto)
        {
            string UPDATE = "UPDATE producto SET " +
                "nombre   = @nombre," +
                "precio   = @precio," +
                "cantidad = @cantidad," +
                "imagen   = @imagen " +
                "WHERE id = @id;";
            MySqlCommand mCommand = new MySqlCommand(UPDATE, conexionMysql.GetConnection());

            mCommand.Parameters.Add(new MySqlParameter("@nombre", mProducto.nombre));
            mCommand.Parameters.Add(new MySqlParameter("@precio", mProducto.precio));
            mCommand.Parameters.Add(new MySqlParameter("@cantidad", mProducto.cantidad));
            mCommand.Parameters.Add(new MySqlParameter("@imagen", mProducto.imagen));
            mCommand.Parameters.Add(new MySqlParameter("@id", mProducto.id));

            //Si el resultado de la consulta es mayor a 0 se hizo correctamente
            return mCommand.ExecuteNonQuery() > 0;
        }
        internal bool EliminarProducto(int id)
        {
            string DELETE = "DELETE FROM producto WHERE id = @id";
            MySqlCommand mCommand = new MySqlCommand(DELETE, conexionMysql.GetConnection());
            
            mCommand.Parameters.Add(new MySqlParameter("@id", id));
            
            return mCommand.ExecuteNonQuery() > 0;
        }
    }
}
