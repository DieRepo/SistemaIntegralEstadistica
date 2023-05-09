using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaIntegralEstadistica.Controlador
{
    public class ModeloCampo
    {
        private String idCampo;
        private String nombreCampo;
        private String mesCampo;
        private String nombreClass;
        private String classTotal;
        private String tipoCampo;
        private Boolean esActivo;




        public String IdCampo { get => idCampo; set => idCampo = value; }
        public String NombreCampo { get => nombreCampo; set => nombreCampo = value; }
        public string NombreClass { get => nombreClass; set => nombreClass = value; }
        public string ClassTotal { get => classTotal; set => classTotal = value; }
        public string MesCampo { get => mesCampo; set => mesCampo = value; }
        public bool EsActivo { get => esActivo; set => esActivo = value; }
        public string TipoCampo { get => tipoCampo; set => tipoCampo = value; }
    }
}