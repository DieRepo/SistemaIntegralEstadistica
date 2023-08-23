using MySql.Data.MySqlClient;
using SistemaIntegralEstadistica.Controlador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SistemaIntegralEstadistica
{
    public partial class Master1 : System.Web.UI.MasterPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            //Revisar que el usuario se registró
            if (Session["usuario"] is null)
            {
                redirectLogin();
            }

            Session["tablasAcceso"] = (List<int>)Session["verTabla"];
            agregarInformacionUsuario();
        }

        public void agregarInformacionUsuario()
        {
            Dictionary<String, String> infoUser = (Dictionary<string, string>)Session["usuario"];
            usuario.InnerText = "Usuario: " + infoUser["nombre"];
            areaAdscripcion.Text = infoUser["nombreArea"];
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            
        }


        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.RemoveAll();
            Session.Clear();
            Session.Abandon();
            redirectLogin();
        }

        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            Session.RemoveAll();
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        protected void redirectLogin()
        {
            String[] sc = HttpContext.Current.Request.Url.Host.ToString().Split('/');
            Response.Redirect("~/Login.aspx");
        }
    }
}