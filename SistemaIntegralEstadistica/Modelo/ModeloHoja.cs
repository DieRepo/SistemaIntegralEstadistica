using SistemaIntegralEstadistica.Controlador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaIntegralEstadistica.Modelo
{
    public class ModeloHoja
    {
        private int idHoja;
        private String nombreHoja;
        private List<ModeloTabla> tablas;
        private int orden;


        public int IdHoja { get => idHoja; set => idHoja = value; }
        public string NombreHoja { get => nombreHoja; set => nombreHoja = value; }
        public List<ModeloTabla> Tablas { get => tablas; set => tablas = value; }
        public int Orden { get => orden; set => orden = value; }
    }
}