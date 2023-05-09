using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SistemaIntegralEstadistica.Vista
{
    public partial class Magistrados : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Console.WriteLine("PAGE_LOAD MAGISTRADOS");
        }


        protected void guardarInformacion(object sender, CommandEventArgs e)
        {
            // Cast the loosely-typed Page.Master property and then set the GridMessageText property 
            Master.ejemplo_Click(sender,e);
        }
    }
}