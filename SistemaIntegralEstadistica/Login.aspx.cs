using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;


namespace SistemaIntegralEstadistica
{
    public partial class Login : System.Web.UI.Page
    {

        string str;
        MySqlCommand com;
        MySqlDataReader r;
        string passw;
        protected void Page_Load(object sender, EventArgs e)
        {
            textoError.Visible = false;
        }

        protected void btn_login_Click(object sender, EventArgs e)
        {
            MySqlConnection con = null;
            try
            {

                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                str = "SELECT * FROM tblusuarios " +
                    "  where usuario=@UserName and pas=@Password and activo = 1 ";
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
                   

                    while (r.Read())
                    {
                        id = r.GetString("idusuarios");
                        nombre = r.GetString("nombre");
                        
                    }
                    Dictionary<String, String> uss = new Dictionary<String, String>();
                    Random rnd = new Random();
                    uss.Add("user", TextBox_user_name.Text);
                    uss.Add("pass", passw);
                    uss.Add("idusuarios", id);
                    uss.Add("nombre", nombre);
                    Session["usuario"] = uss;

                  /*  Response.Redirect("~/Vista/SegAcuerdos.aspx");*/
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