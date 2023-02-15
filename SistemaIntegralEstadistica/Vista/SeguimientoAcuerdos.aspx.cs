using MySql.Data.MySqlClient;
using SistemaIntegralEstadistica.Controlador;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SistemaIntegralEstadistica.Vista
{
    public partial class SeguimientoAcuerdos : System.Web.UI.Page
    {

        MySqlCommand cmd;
        MySqlDataReader r;
        String sql;
        Dictionary<string, string> d;
        int idRegistroEditar;
        int[] idTabla = { 2, 3 };
        String[] meses = { "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre" };
        int mesActivo = -1;

        List<ModeloTabla> tablas = new List<ModeloTabla>();

        protected void Page_Load(object sender, EventArgs e)
        {
            
            /*
            generarTablas();
            if (!this.IsPostBack) {  


            }
            */
        }

        
}