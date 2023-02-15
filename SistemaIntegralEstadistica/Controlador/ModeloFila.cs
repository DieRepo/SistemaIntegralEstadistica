using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaIntegralEstadistica.Controlador
{
    public class ModeloFila
    {
        private int idFila;
        private String nombreFila;

        public ModeloFila(int idFila, string nombreFila)
        {
            this.idFila = idFila;
            this.nombreFila = nombreFila;
        }

        public int IdFila { get => idFila; set => idFila = value; }
        public string NombreFila { get => nombreFila; set => nombreFila = value; }
    }
}