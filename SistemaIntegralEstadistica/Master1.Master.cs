﻿using MySql.Data.MySqlClient;
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
            Session["tablasAcceso"] = (List<int>)Session["verTabla"];


        }




    }
}