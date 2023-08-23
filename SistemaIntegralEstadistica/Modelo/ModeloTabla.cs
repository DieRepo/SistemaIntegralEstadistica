using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaIntegralEstadistica.Controlador
{
    [Serializable]
    public class ModeloTabla 
    {
        private int id;
        private String idTabla;
        private String nombreTabla;
        private String tabla;
        private String idFilas;
        private String nombresColumnas;
        private List<ModeloCampo> campos;
        private List<ModeloFila> filas;
        private String colSpan;
        private String textoColSpan;
        private String rowSpan;
        private String textoRowSpan;
        private String colAdicional;
        private String mesActivo;
        private String fechaLimiteRegistro;
        private String camposActivos;
        private Boolean esMensual;
        private Boolean esRegistro;
        private String tipoTemporal;
        private String parametro;
        private Boolean mesActualValidado;
        private Boolean tieneFilaTrimestre;
         

        public int Id { get => id; set => id = value; }
        public string NombreTabla { get => nombreTabla; set => nombreTabla = value; }
        public string Tabla { get => tabla; set => tabla = value; }
        public string IdFilas { get => idFilas; set => idFilas = value; }
        public string NombresColumnas { get => nombresColumnas; set => nombresColumnas = value; }
        public List<ModeloCampo> Campos { get => campos; set => campos = value; }
        public List<ModeloFila> Filas { get => filas; set => filas = value; }
        public string ColSpan { get => colSpan; set => colSpan = value; }
        public string TextoColSpan { get => textoColSpan; set => textoColSpan = value; }
        public string RowSpan { get => rowSpan; set => rowSpan = value; }
        public string TextoRowSpan { get => textoRowSpan; set => textoRowSpan = value; }
        public string ColAdicional { get => colAdicional; set => colAdicional = value; }
        public string IdTabla { get => idTabla; set => idTabla = value; }
        public string MesActivo { get => mesActivo; set => mesActivo = value; }
        public string FechaLimiteRegistro { get => fechaLimiteRegistro; set => fechaLimiteRegistro = value; }
        public string CamposActivos { get => camposActivos; set => camposActivos = value; }
        public bool EsMensual { get => esMensual; set => esMensual = value; }
        public bool EsRegistro { get => esRegistro; set => esRegistro = value; }
        public String TipoTemporal { get => tipoTemporal; set => tipoTemporal = value; }
        public string Parametro { get => parametro; set => parametro = value; }
        public bool MesActualValidado { get => mesActualValidado; set => mesActualValidado = value; }
        public bool TieneFilaTrimestre { get => tieneFilaTrimestre; set => tieneFilaTrimestre = value; }
    }
}