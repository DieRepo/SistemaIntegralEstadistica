using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SistemaIntegralEstadistica.Modelo
{
    public class ModeloTablaReporte
    {
        private String titulo;
        private DataTable tabla;
        private int orden;

        public string Titulo { get => titulo; set => titulo = value; }
        public DataTable Tabla { get => tabla; set => tabla = value; }
        public int Orden { get => orden; set => orden = value; }
    }
}