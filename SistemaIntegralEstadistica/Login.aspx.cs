using MySql.Data.MySqlClient;
using SistemaIntegralEstadistica.Controlador;
using SistemaIntegralEstadistica.Modelo;
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
            if (!this.IsPostBack) {
                Session.RemoveAll();
                Session.Clear();
                Session.Abandon();
            }
        }

        protected void btn_login_Click(object sender, EventArgs e)
        {
            string str;
            string passw;

            Session["verTabla"] = null;
            Session["usuario"] = null;

            MySqlConnection con = null;

            Dictionary<int , ModeloHoja > hojas = new Dictionary<int, ModeloHoja>();

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                str = "SELECT * FROM tblusuarios a " +
                    " LEFT JOIN tblpermisostablas b ON a.idusuarios = b.idusuarios" +
                    " LEFT JOIN tbltabla t ON b.idtablacceso = t.id "+
                    " LEFT JOIN tblcatareas c ON a.idArea = c.id " +
                    " LEFT JOIN tbltablahoja h ON t.idHoja = h.id " +
                    " where a.usuario=@UserName and a.pas=@Password and a.idusuarios = b.idusuarios and a.activo = 1 AND b.activo=1 ORDER BY t.orden; ";
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
                      
                        
                        idArea = r.GetString("idArea");
                        nombreArea = r.GetString("nombreArea");
                        try
                        {
                            idpermisostablas = r.GetInt32("idtablacceso");

                            int idHoja = r.GetInt32("idHoja");

                            ModeloHoja value;
                            bool hasValue = hojas.TryGetValue(idHoja, out value);
                            if (idHoja > 0)
                            {
                                if (hasValue)
                                {
                                    ModeloHoja val = hojas[idHoja];
                                    ModeloTabla tab = new ModeloTabla();
                                    tab.Id = idpermisostablas;
                                    if (val.Tablas == null)
                                    {
                                        val.Tablas = new List<ModeloTabla>();
                                        val.Tablas.Add(tab);
                                    }
                                    else
                                    {
                                        val.Tablas.Add(tab);
                                    }
                                }
                                else
                                {
                                    ModeloHoja hoja = new ModeloHoja();
                                    hoja.IdHoja = r.GetInt32("idHoja");
                                    hoja.NombreHoja = r.GetString("descripcion");
                                    hoja.Orden = r.GetInt32("ordenHoja");
                                    //ModeloHoja val = hojas[idHoja];
                                    List<ModeloTabla> lista = new List<ModeloTabla>();
                                    ModeloTabla tab = new ModeloTabla();
                                    tab.Id = idpermisostablas;
                                    hoja.Tablas = new List<ModeloTabla>();
                                    hoja.Tablas.Add(tab);
                                    hojas.Add(idHoja, hoja);
                                }
                            }


                            idtablas.Add(idpermisostablas);

                        }
                        catch (Exception excep)
                        {

                            Console.WriteLine(excep.ToString());
                        }

                        
                    }
                    Session["verHojas"] = hojas;
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
                    //uss.Add("hojas", hojas.ToString());

                    Session["usuario"] = uss;
                    
                    if (idArea.Equals("5"))
                    {
                        Response.Redirect("~/Vista/CiudadMujeres.aspx");
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