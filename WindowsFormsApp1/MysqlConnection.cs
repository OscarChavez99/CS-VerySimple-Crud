using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    //Heredar la clase "Conexion" para tener acceso a las variables de nuestra otra clase
    //(server, user, etc)
    internal class MysqlConnection: Connection
    {
        //Clic derecho en el proecto/administradar paquetes nugget/examinar y escribir
        //mysql.data. Si no se soluciona dar clic en "MySqlConnection" y teclear al mismo
        //tiempo "alt + enter"
        private MySqlConnection connection;
        private string connectionString;
        public MysqlConnection()
        {
            connectionString = "Database=" + database + "; "
            + "DataSource=" + server + "; "
            + "User Id=" + user + "; "
            + "Password=" + pass + "; ";

            connection = new MySqlConnection(connectionString);
        }

        public MySqlConnection GetConnection()
        {
            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();
            }
            catch (Exception e) 
            {
                MessageBox.Show(e.ToString());
            }
            return connection;
        }
    }
}
