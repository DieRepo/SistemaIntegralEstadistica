using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaIntegralEstadistica.Controlador
{
    public class ModeloTabla
    {
        private int id;
        private String nombreTabla;
        private String tabla;
        private List<ModeloCampo> campos;
        private List<ModeloFila> filas;
        private List<String> colSpan;
        private List<String> rowSpan;

    }
}