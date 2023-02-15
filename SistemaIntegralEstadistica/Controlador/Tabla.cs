using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SistemaIntegralEstadistica.Controlador
{
    public class Tabla {
        
        /*
         * private const string V = ">>";
        MySqlCommand cmd;
        MySqlDataReader r;
        String sql;
        Dictionary<string, string> d;
        int idRegistroEditar;

        public List<String[]> ConsultarDatos(int idTabla) {
            List<String[]> filaColumnas = new List<String[]>();
            MySqlConnection con = null;
            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                sql = "SELECT group_concat( descripcion order by orden ASC  SEPARATOR '>>') nombre, " +
                    "        group_concat(campo ORDER BY orden ASC SEPARATOR '|')  campos" +
                    "    FROM tblfilas where idtabla = 1" +
                    "UNION " +
                    "SELECT group_concat(descripcion order by orden ASC  SEPARATOR '>>') nombre, " +
                    "		group_concat(campo ORDER BY orden ASC SEPARATOR '|')  campo" +
                    "    FROM tblcolumnas where idtabla = " + idTabla + "; ";
                cmd = new MySqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                while (resultado.Read())
                {
                    string[] dato = resultado["nombre"].ToString().Split('|');
                    filaColumnas.Add(dato);
                    string[] dato2 = resultado["campos"].ToString().Split('|');
                    filaColumnas.Add(dato2);
                }
                con.Close();


            }
            catch (Exception error)
            {
                Console.WriteLine("Error: " +  error.ToString());
            }
            finally {
                if (con != null)
                {
                    con.Close();
                }
            }

            return filaColumnas;        */
    }
    
     
}