using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaIntegralEstadistica.Modelo
{
    public class ModeloMediacion
    {
        private String servicio;
        private int mujeres;
        private int hombres;
        private int personaMoral;
        private int total;

        public string Servicio { get => servicio; set => servicio = value; }
        public int Mujeres { get => mujeres; set => mujeres = value; }
        public int Hombres { get => hombres; set => hombres = value; }
        public int Total { get => total; set => total = value; }
        public int PersonaMoral { get => personaMoral; set => personaMoral = value; }
    }
}