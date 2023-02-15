using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaIntegralEstadistica.Controlador
{
    public class ModeloCampo
    {
        private int idCampo;
        private int nombreCampo;

        public ModeloCampo(int idCampo, int nombreCampo)
        {
            this.idCampo = idCampo;
            this.nombreCampo = nombreCampo;
        }

        public int IdCampo { get => idCampo; set => idCampo = value; }
        public int NombreCampo { get => nombreCampo; set => nombreCampo = value; }
    }
}