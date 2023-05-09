using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;


namespace SistemaIntegralEstadistica
{
    public partial class Login : System.Web.UI.Page
    {

        MySqlCommand com;
        MySqlDataReader r;
        protected void Page_Load(object sender, EventArgs e)
        {
            textoError.Visible = false;
        }

        protected void btn_login_Click(object sender, EventArgs e)
        {
            string str;
            string passw;

            Session["verTabla"] = null;
            Session["usuario"] = null;

            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                str = "SELECT * FROM tblusuarios a " +
                    " LEFT JOIN tblpermisostablas b ON a.idusuarios = b.idusuarios" +
                    " LEFT JOIN tblcatareas c ON a.idArea = c.id " +
                    " where a.usuario=@UserName and a.pas=@Password and a.idusuarios = b.idusuarios and a.activo = 1 AND b.activo=1; ";
                com = new MySqlCommand(str, con);
                com.CommandType = CommandType.Text;
                passw = TextBox_password.Text;



                com.Parameters.AddWithValue("@UserName", TextBox_user_name.Text);
                com.Parameters.AddWithValue("@Password", TextBox_password.Text);
                r = com.ExecuteReader();


                if (r.HasRows)
                {

                    string id = "";
                    string nombre = "";
                    int idpermisostablas = 0;
                    String idArea = "", nombreArea="";

                    List<int> idtablas = new List<int>();

                    while (r.Read())
                    {
                        id = r.GetString("idusuarios");
                        nombre = r.GetString("nombre").Trim() + " "+ r.GetString("primerApellido").Trim() + " " + r.GetString("segundoApellido").Trim();
                        idpermisostablas = r.GetInt32("idtablacceso");
                        idtablas.Add(idpermisostablas);
                        idArea = r.GetString("idArea");
                        nombreArea = r.GetString("nombreArea");
                    }

                    Session["verTabla"] = idtablas;
                    //var vertablas = Session["verTabla"];
                    Dictionary<String, String> uss = new Dictionary<String, String>();
                    Random rnd = new Random();
                    uss.Add("user", TextBox_user_name.Text);
                    uss.Add("pass", passw);
                    uss.Add("idusuarios", id);
                    uss.Add("nombre", nombre);
                    uss.Add("area", idArea);
                    uss.Add("nombreArea", nombreArea);

                    Session["usuario"] = uss;
                    
                    if (id.Equals(-1))
                    {
                        Response.Redirect("~/Mediacion/Mediacion.aspx");
                    }
                    else {
                        Response.Redirect("~/Vista/Inicio.aspx");
                    }

                    textoError.Text = "Usuario correcto";
                    textoError.Visible = true;

                }
                else
                {
                    textoError.Visible = true;
                    textoError.Text = "Usuario o contraseña incorrectos";
                }


                con.Close();

            }
            catch (Exception ex)
            {


                Console.WriteLine(ex.ToString());
                textoError.Visible = true;
                textoError.Text = "Usuario o contraseña incorrectos";

            }
            finally
            {
                if (con != null)
                {
                    con.Dispose();
                    con.Close();
                }
            }

        }




    }
}