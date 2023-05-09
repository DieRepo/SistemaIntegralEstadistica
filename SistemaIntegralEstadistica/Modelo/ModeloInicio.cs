using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace SistemaIntegralEstadistica.Controlador
{
    public class ModeloInicio
    {
        private Table tabla;
        private Button boton;

        public Table Tabla { get => tabla; set => tabla = value; }
        public Button Boton { get => boton; set => boton = value; }
    }
}