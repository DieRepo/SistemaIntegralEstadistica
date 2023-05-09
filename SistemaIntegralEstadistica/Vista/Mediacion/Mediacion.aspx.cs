using MySql.Data.MySqlClient;
using SistemaIntegralEstadistica.Controlador;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SistemaIntegralEstadistica.Vista.Mediacion
{
    public partial class Mediacion : System.Web.UI.Page
    {
        MySqlCommand cmd;
        MySqlDataReader r;
        String sql;

        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<String, String> infoUser = (Dictionary<string, string>)Session["usuario"];

            if (!this.IsPostBack) {

                consultarCentrosAsignados(infoUser["idArea"]);
            }

        }

        public void agregarInformacionUusuario()
        {
           // usuario.InnerText = "Usuario: " + infoUser["nombre"];
           // areaAdscripcion.Text = infoUser["nombreArea"];
        }



        public void consultarCentrosAsignados(String id)
        {
            List<ModeloCatalogo> listaTexto = new List<ModeloCatalogo>(); ;

            String consulta = "SELECT cm.id , cm.nombre " +
                "    FROM tblusuarios u " +
                "    LEFT JOIN tblcentrosusuario cu " +
                "        ON u.idusuarios = cu.idUsuario" +
                "    LEFT JOIN tblcatcentrosm cm" +
                "        ON cu.id = cm.id" +
                "    where idusuarios = " + id + " ; ";

            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(consulta, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                while (resultado.Read())
                {
                    ModeloCatalogo obj = new ModeloCatalogo();
                    obj.Id = resultado.GetInt32("id");
                    obj.Descripcion = resultado.GetString("nombre");
                    listaTexto.Add(obj);
                }
                con.Close();
                
                centroMed.DataSource = listaTexto;
                centroMed.DataValueField = "id";
                centroMed.DataTextField = "descripcion";
                centroMed.DataBind();
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: " + error.ToString());
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

        }

        protected void centroMed_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}