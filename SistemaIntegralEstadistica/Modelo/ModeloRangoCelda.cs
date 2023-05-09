using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaIntegralEstadistica.Modelo
{
    public class ModeloRangoCelda
    {
        private Boolean esTipoFila;
        private int[] celdaInicio;
        private int[] celdaFin;

        public bool EsTipoFila { get => esTipoFila; set => esTipoFila = value; }
        public int[] CeldaInicio { get => celdaInicio; set => celdaInicio = value; }
        public int[] CeldaFin { get => celdaFin; set => celdaFin = value; }
    }
}