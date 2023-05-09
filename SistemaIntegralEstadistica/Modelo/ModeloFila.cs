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
        private int mesFila;
        private Boolean esFilaActiva;
        private String claseSuma;
        private String claseTotal;


        public int IdFila { get => idFila; set => idFila = value; }
        public string NombreFila { get => nombreFila; set => nombreFila = value; }
        public int MesFila { get => mesFila; set => mesFila = value; }
        public bool EsFilaActiva { get => esFilaActiva; set => esFilaActiva = value; }
        public string ClaseSuma { get => claseSuma; set => claseSuma = value; }
        public string ClaseTotal { get => claseTotal; set => claseTotal = value; }
    }
}