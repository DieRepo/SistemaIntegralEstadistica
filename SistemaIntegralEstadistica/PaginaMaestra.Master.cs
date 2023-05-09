using System;
using System;
using System.Collections.Generic;
using System.Linq;

using MySql.Data.MySqlClient;
using System.Data;
using SistemaIntegralEstadistica.Controlador;
using System.Web.UI.WebControls;
using System.Web;
using OfficeOpenXml;
using System.IO;
using System.Drawing;
using OfficeOpenXml.Style;
using SistemaIntegralEstadistica.Modelo;

namespace SistemaIntegralEstadistica
{
    public partial class PaginaMaestra : System.Web.UI.MasterPage
    {

        MySqlCommand cmd;
        MySqlDataReader r;
        String sql;
        Dictionary<string, string> d;
        Dictionary<string, ModeloTabla> t;
        int[] catalogos = { 1, 2 };
        int[] celdaInicioTabla = { 4, 2 };
        int espacioTablas = 2;
        int anio = 2023;
        List<int> tablasAcceso, tablasAccesoArea;
        String[] mesesAnio = { "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre" };

        List<ModeloTabla> tablas = new List<ModeloTabla>();
        List<Table> listaTablas;
        List<Button> listaBotones, listaBotonesValidar;
        Dictionary<String, ModeloTabla> tablasPagina;
        Dictionary<String, List<ModeloCatalogo>> mapCatalogos;


        protected void Page_Load(object sender, EventArgs e)
        {
            Session["anioConsulta"] = AnioConsulta.SelectedValue;
            
            //Revisar que el usuario se registró
            if (Session["usuario"] is null)
            {
                redirectLogin();
            }         

            if (!this.IsPostBack)
            {
                cargarDatosCatalogos();
                Session["tablasAcceso"] = (List<int>)Session["verTabla"];
            }
            
            GeneraControlesDinamicos();
            agregarInformacionUusuario();
        }

        public void agregarInformacionUusuario() {
            Dictionary<String,String> infoUser = (Dictionary<string, string>)Session["usuario"];
            usuario.InnerText = "Usuario: " + infoUser["nombre"];
            areaAdscripcion.Text = infoUser["nombreArea"];
        }

        public void GeneraControlesDinamicos() {
            try
            {
                //Generar controles dinámicos
                listaTablas = cargarControlesTablas();
                obtenerDatosTablas();
                CargarControles();
            }
            catch (Exception error)
            {

                Console.WriteLine("Error al generar los controles dinamicos "+ error.ToString());
            }

        }


        protected void Inicio_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Peritos.aspx");

        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.RemoveAll();
            Session.Clear();
            Session.Abandon();
            redirectLogin();
        }

        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            Session.RemoveAll();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        protected void redirectLogin()
        {
            String[] sc = HttpContext.Current.Request.Url.Host.ToString().Split('/');
            Response.Redirect("~/Login.aspx");
        }


        private void cargarDatosCatalogos() {
            mapCatalogos = new Dictionary<string, List<ModeloCatalogo>>();
            for (int i = 0; i < catalogos.Length ; i++)
            {
                List<ModeloCatalogo> lista = obtenerDatosCatalogo(catalogos[i]);
                mapCatalogos.Add(catalogos[i].ToString(), lista);
            }
            Console.WriteLine("Total catalogos");
            Session["listaCatalogos"] = mapCatalogos;
        }

        
        protected void CargarControles()
        {
            tablasPagina = new Dictionary<String, ModeloTabla>();
            listaBotones = cargarControlesBotones(); //Botones para guardar información de cada Tabla
            listaBotonesValidar = cargarControlesBotonesValidar();  //Botones para validar información de cada tabla
            generarTablasDinamicamente();
        }


        protected void obtenerDatosTablas() {
            Dictionary<String, ModeloTabla> tablasPagina = new Dictionary<String, ModeloTabla>();
            try
            {
                tablasAcceso = (List<int>)Session["tablasAcceso"];

                for (int i = 0; i < tablasAcceso.Count; i++)
                {
                    var tabla = ConsultarDatosTablaDin(tablasAcceso[i], false);
                    tablasPagina.Add(listaTablas[i].ID.ToString(), tabla);
                }

            }
            catch (Exception)
            {

                Console.WriteLine("ERROR AL OBTENER INFORMACIÓN");
            }
            finally {
                Session["infoTablas"] = tablasPagina;
            }

        }

        protected void obtenerDatosTablasExcel(List<int> tablasExcel)
        {
            Dictionary<String, ModeloTabla> tablasPagina = new Dictionary<String, ModeloTabla>();
            try
            {

                for (int i = 0; i < tablasExcel.Count; i++)
                {
                    var tabla = ConsultarDatosTablaDin(tablasExcel[i], true);
                    tablasPagina.Add("Tabla"+ tablasExcel[i] , tabla);
                }

            }
            catch (Exception)
            {

                Console.WriteLine("ERROR AL OBTENER INFORMACIÓN");
            }
            finally
            {
                Session["infoTablasExcel"] = tablasPagina;
            }

        }


        public List<Table> cargarControlesTablas() {
            List<Table> lis = new List<Table>();
            tablasAcceso = (List<int>)Session["tablasAcceso"];

            try
            {
                for (int i = 0; i < tablasAcceso.Count; i++)
                {

                    String id = "Tabla" + (tablasAcceso[i]);
                    Table tabla = (Table)FindControl(id);
                    if (tabla == null)
                    {
                        tabla = new Table();
                        tabla.ID = id;
                    }
                    else
                    {
                    }
                    lis.Add(tabla);
                }

            }
            catch (Exception error)
            {

                Console.WriteLine("ERROR EN CONTROLES DE TABLA "+  error.ToString());
            }
            return lis;
        }



        public List<Button> cargarControlesBotones() {
            List<Button> lista = new List<Button>();
            try
            {
                Dictionary<String, ModeloTabla> tabla = (Dictionary<string, ModeloTabla>)Session["infoTablas"];
                tablasAcceso = (List<int>)Session["tablasAcceso"];


                for (int i = 0; i < tablasAcceso.Count; i++)
                {
                    String id = "Button_" + (tablasAcceso[i]) + "_mes";
                    Button botonGuardar = (Button)FindControl(id);
                    if (botonGuardar == null)
                    {
                        botonGuardar = new Button();
                        botonGuardar.ID = id;
                        botonGuardar.Text = "Guardar";
                        botonGuardar.CommandName = "Tabla" + (tablasAcceso[i]) + "_mes";
                        botonGuardar.UseSubmitBehavior = false;
                        botonGuardar.OnClientClick = "this.disabled = true; this.value = 'Guardando...'; ";
                        if (tabla["Tabla" + tablasAcceso[i]].EsRegistro)
                        {
                            botonGuardar.Command += new CommandEventHandler(ejemplo_Click2);
                        }
                        else {
                            botonGuardar.Command += new CommandEventHandler(ejemplo_Click);

                        }
                        // ContentPlaceHolder1.Controls.Add(botonGuardar);
                    }
                    else {
                    
                    }
                    botonGuardar.Attributes.Add("class" , "btn btn-dark");
                    //botonGuardar.Enabled = !tabla["Tabla" + tablasAcceso[i]].MesActualValidado;
                    Boolean activarBoton = (DateTime.Now <= Convert.ToDateTime(tabla["Tabla" + tablasAcceso[i]].FechaLimiteRegistro)) && !tabla["Tabla" + tablasAcceso[i]].MesActualValidado;
                    botonGuardar.Visible = activarBoton;

                    lista.Add(botonGuardar);
                }
               
            }
            catch (Exception error)
            {

                Console.WriteLine("ERROR AL CREAR LOS BOTONES: "+error.Message);
            }

            return lista;
        }




        public List<Button> cargarControlesBotonesValidar()
        {
            List<Button> lista = new List<Button>();
            String textoValida = "";
            try
            {
                Dictionary<String, ModeloTabla> tabla = (Dictionary<string, ModeloTabla>)Session["infoTablas"];
                tablasAcceso = (List<int>)Session["tablasAcceso"];


                for (int i = 0; i < tablasAcceso.Count; i++)
                {
                    String id = "BotonValida_" + (tablasAcceso[i]) + "_"+ tabla["Tabla" + tablasAcceso[i]].MesActivo;
                    Button botonValidar = (Button)FindControl(id);
                    if (botonValidar == null)
                    {
                        botonValidar = new Button();
                        botonValidar.ID = id;
                        botonValidar.CommandName = "Tabla_" + (tablasAcceso[i]) + "_" + tabla["Tabla" + tablasAcceso[i]].MesActivo;
                        botonValidar.UseSubmitBehavior = false;
                        botonValidar.OnClientClick = "this.disabled = true; this.value = 'Validando...'; ";

                        botonValidar.Command += new CommandEventHandler(ejemplo_ClickValidar);
                       
                    }

                    botonValidar.Attributes.Add("class", "btn btn-success");
                    botonValidar.Attributes.Add("style", ( tabla["Tabla" + tablasAcceso[i]].MesActualValidado ? "margin-left: 5px;" : "margin-left: 30px;") );
                    if (tabla["Tabla" + tablasAcceso[i]].MesActualValidado) {
                        botonValidar.Attributes.Add("disabled", "disabled");
                    }
                    textoValida = tabla["Tabla" + tablasAcceso[i]].MesActualValidado ? "Validado" : "Validar información";
                    botonValidar.Text = textoValida;


                    lista.Add(botonValidar);
                }

            }
            catch (Exception error)
            {

                Console.WriteLine("ERROR AL CREAR LOS BOTONES: " + error.Message);
            }

            return lista;
        }



        private void generarTablasDinamicamente()
        {

            //Obtener fecha actual
            DateTime fecha = DateTime.Now;
            int dia = fecha.Day;
            int mes = fecha.Month;
            Dictionary<String, ModeloTabla> tabla = (Dictionary<string, ModeloTabla>)Session["infoTablas"];
            tablasAcceso = (List<int>)Session["tablasAcceso"];


            for (int i = 0; i < tablasAcceso.Count ; i++)
            {
                generarTablaDinamica0(listaTablas[i], listaBotones[i], listaBotonesValidar[i], tabla["Tabla"+tablasAcceso[i]]);

            }
        }


        public ModeloTabla ConsultarDatosTablaDin(int idTable, Boolean allColumn)
        {
            Boolean esInformacionMensual = false;
            String camposActivos = "";
            String mostrarCampos = allColumn ? " and mostrar IN (0,1)" : " and mostrar = 1";
            ModeloTabla tabla = new ModeloTabla();
            tabla.Id = idTable;
            tabla.EsMensual = false;
            tabla.TieneFilaTrimestre = false;


            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                sql = "SELECT id, tabla, nombreTabla, mesActivo,  fechaActivo, esMensual , esPorRegistro, tipoDato , esTemporal , parametro,"
                    + " ifnull(t2.tipoDat, 'columna') tipo, t3.*,  t2.* FROM tbltabla t1" +
                "    LEFT JOIN (select idtabla, 'columna' tipoDat, GROUP_CONCAT( if (campo <> '', campo, NULL) order by orden ASC  SEPARATOR '|') campos,  " +
                "	GROUP_CONCAT(if (campo <> '', descripcion, NULL) order by orden ASC  SEPARATOR '|') nombre, " +
                " GROUP_CONCAT(If(orden > -1, orden, NULL) order by orden ASC  SEPARATOR '|') orden," +
                "	GROUP_CONCAT(colspan order by orden ASC  SEPARATOR '|') span," +
                "	GROUP_CONCAT(textocolspan order by orden ASC  SEPARATOR '|') textoSpan  ," +
                "    GROUP_CONCAT( if (campo = '', descripcion, NULL) order by orden ASC  SEPARATOR '|') campoAdicional," +
                "    GROUP_CONCAT( if (classSuma is not null, classSuma, NULL) order by orden ASC  SEPARATOR '|') sumaColumnas," +
                "    GROUP_CONCAT( if (classTotal is not null, classTotal, NULL) order by orden ASC  SEPARATOR '|') sumaTotales," +
                "       GROUP_CONCAT(if(campo <> '', mes, NULL)order by orden ASC  SEPARATOR '|') mes ," +
                " GROUP_CONCAT(If(orden > -1, tipo, NULL) order by orden ASC  SEPARATOR '|') tipoDato " +
                "     FROM tblcolumnas WHERE      idtabla = " + idTable + " " + mostrarCampos +
                "    union" +
                "    select idtabla, 'fila' tipoDat, GROUP_CONCAT(id order by orden ASC  SEPARATOR '|'), " +
                "	GROUP_CONCAT(descripcion order by orden ASC  SEPARATOR '|') nombre, " +
                "	GROUP_CONCAT(orden order by orden ASC  SEPARATOR '|') orden," +
                "	GROUP_CONCAT(rowspan order by orden ASC  SEPARATOR '|') span," +
                "	GROUP_CONCAT(textorowspan order by orden ASC  SEPARATOR '|') textoSpan," +
                "   GROUP_CONCAT( if (descripcion = '', descripcion, NULL) order by orden ASC  SEPARATOR '|') campoAdicional," +
                "   GROUP_CONCAT( if (classSuma is not null, classSuma, NULL) order by orden ASC  SEPARATOR '|') sumaColumnasFila," +
                "   GROUP_CONCAT( if (classTotal is not null, classTotal, NULL) order by orden ASC  SEPARATOR '|') sumaTotalesFila," +
                "    GROUP_CONCAT(If(orden > -1, mes, NULL)  order by orden ASC  SEPARATOR '|') mes , " +
                "   '' as tipoDato " +
                "    from tblfilas WHERE idtabla = " + idTable + ") t2 ON t1.id = t2.idtabla" +
                "   LEFT JOIN tblvalidacion t3 ON t2.idtabla = t3.idtabla" +
                "    WHERE t1.id = " + idTable + "; ";

                cmd = new MySqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                while (resultado.Read())
                {

                    if (resultado["tipo"].Equals("columna"))
                    {
                        tabla.Tabla = resultado["tabla"].ToString();
                        tabla.NombreTabla = resultado["nombreTabla"].ToString();
                        tabla.NombresColumnas = resultado["campos"].ToString();
                        tabla.MesActivo = resultado["mesActivo"].ToString();
                        tabla.EsMensual = resultado["esMensual"].ToString().Equals("1");
                        tabla.ColSpan = resultado["span"].ToString();
                        tabla.TextoColSpan = resultado["textoSpan"].ToString();
                        tabla.ColAdicional = resultado["campoAdicional"].ToString();
                        tabla.EsRegistro = resultado["esPorRegistro"].ToString().Equals("1");
                        tabla.TipoTemporal = resultado["esTemporal"].ToString();
                        tabla.Parametro = resultado["parametro"].ToString();
                        tabla.FechaLimiteRegistro = resultado["fechaActivo"].ToString();
                        String mesActualValida = mesesAnio[Convert.ToInt32(tabla.MesActivo) - 1];
                        tabla.MesActualValidado = resultado[mesActualValida].ToString().Equals("1");
                        String[] nombresColumnas = null;
                        if (resultado["nombre"].ToString().Length > 1023) {
                            nombresColumnas =ConsultarDatosTablaCamposFilas(idTable, mostrarCampos, true);
                        }
                        else {
                            nombresColumnas = resultado["nombre"].ToString().Split('|');
                        }
                        String[] idColumnas = resultado["campos"].ToString().Split('|');
                        String[] classTotales = resultado["sumaTotales"].ToString().Split('|');
                        String[] clases = resultado["sumaColumnas"].ToString().Split('|');
                        String[] meses = resultado["mes"].ToString().Split('|');
                        String[] tipoDato = resultado["tipoDato"].ToString().Split('|');

                        if (resultado["campos"].ToString().Split('|').Length == 0)
                        {
                            int[] valor = new int[1];
                            valor[0] = Convert.ToInt32(resultado["campos"]);
                        }
                        else
                        {
                            Console.WriteLine("OTRO");

                        }
                        List<ModeloCampo> filas = new List<ModeloCampo>();
                        if (idColumnas.Length == nombresColumnas.Length)
                        {
                            for (int i = 0; i < idColumnas.Length; i++)
                            {
                                ModeloCampo columna = new ModeloCampo();
                                columna.IdCampo = idColumnas[i];
                                if (nombresColumnas.Length == classTotales.Length)
                                {
                                    columna.NombreCampo = nombresColumnas[i].ToString();
                                    columna.MesCampo = meses[i].ToString();
                                    columna.EsActivo = (tabla.MesActivo.Equals(meses[i]) || meses[i].Equals("-1"));
                                    columna.TipoCampo = tipoDato[i];
                                }
                                if (nombresColumnas.Length == clases.Length)
                                {
                                    columna.NombreClass = clases[i].ToString().Trim();

                                }
                                if (esInformacionMensual)
                                {
                                    tabla.EsMensual = true;
                                }
                                camposActivos = camposActivos + (tabla.MesActivo.Equals(meses[i]) || columna.MesCampo.Equals("-1") ? columna.IdCampo + "," : "");

                                columna.ClassTotal = classTotales[i].ToString().Trim();
                                filas.Add(columna);
                            }

                        }
                        tabla.Campos = filas;
                        //tabla.EsMensual = esInformacionMensual;
                        Console.WriteLine("Termina columa");
                    }
                    else
                    {
                        tabla.IdFilas = resultado["campos"].ToString();
                        String[] idFilas = resultado["campos"].ToString().Split('|');
                        String[] nombresFilas = resultado["nombre"].ToString().Split('|');
                        String[] mesFila = resultado["mes"].ToString().Split('|');
                        String[] classTotales = resultado["sumaTotales"].ToString().Split('|');
                        String[] clases = resultado["sumaColumnas"].ToString().Split('|');

                        List<ModeloFila> filas = new List<ModeloFila>();
                        if (idFilas.Length == nombresFilas.Length)
                        {
                            for (int i = 0; i < idFilas.Length; i++)
                            {
                                ModeloFila fila = new ModeloFila();
                                fila.IdFila = Convert.ToInt32(idFilas[i]);
                                fila.NombreFila = nombresFilas[i];
                                fila.MesFila = Convert.ToInt32(mesFila[i]);
                                fila.EsFilaActiva = ( tabla.MesActivo.Equals(mesFila[i]) || mesFila[i].Equals("-1")) ;
                                fila.ClaseSuma = clases[i].Trim();
                                fila.ClaseTotal = classTotales[i].Trim();
                                
                                filas.Add(fila);
                                if (!tabla.TieneFilaTrimestre && fila.NombreFila.ToUpper().Contains("TRIMESTRE")) {
                                    tabla.TieneFilaTrimestre = true;
                                }
                            }

                        }
                        tabla.Filas = filas;
                        tabla.RowSpan = resultado["span"].ToString();
                        tabla.TextoRowSpan = resultado["textoSpan"].ToString();

                        

                        Console.WriteLine("Termina fila");
                    }

                    tabla.CamposActivos = camposActivos;
                }
                con.Close();
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: " + error.ToString());
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

            return tabla;
        }



        public String[] ConsultarDatosTablaCamposFilas(int idTable,String mostrar,Boolean esColumna)
        {
           List<String> listaTexto = new List<string>(); ;

            String queryColumnas = "SELECT descripcion as nombre FROM tblcolumnas WHERE   campo <> '' and  idtabla = " + idTable + " " + mostrar +";";
            String queryFilas = "SELECT descripcion as nombre FROM tblfilas WHERE   idtabla = " + idTable + " ;";

            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                sql = esColumna ? queryColumnas : queryFilas;
                cmd = new MySqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                while (resultado.Read())
                {
                    listaTexto.Add(resultado["nombre"].ToString());
                }
                con.Close();
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: " + error.ToString());
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

            String[] lista = listaTexto.ToArray();
            return lista;
        }


        public List<int> ConsultarTablasPorArea(int idArea)
        {
            List<int> listaTablas = new List<int>(); ;
            MySqlConnection con = null;

            try
            {
                String queryFilas = "SELECT id FROM tbltabla WHERE   idArea = " + idArea + " order by orden asc ;";

                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(queryFilas, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                while (resultado.Read())
                {
                    listaTablas.Add(resultado.GetInt32("id"));
                }
                con.Close();
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: " + error.ToString());
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

            return listaTablas;
        }



        protected void generarTablaDinamica0(Table nombreTabla, Button botonGuardar,  Button botonValidar, ModeloTabla tabla)
        {
            Dictionary<String, String> datos = new Dictionary<string, string>();
            GridView tablaTemp = new GridView(); ;
            String[] spanFilas, textoSpanFilas, numcolAdicional;
            Boolean banderaRowSpan;
            String textoRowSpanTable = "";
            Boolean soloLectura = false, filaTotalMensualAnual =false;
            String keyObject = "";
            String idCampo = "";
            int temporalRowSpan = 1;
            int indiceFila = 0;
            int rowSpanNum = 0;
            int sumaRowSpan = 0;
            int continuo = 0, continuoClass = 0, inicioCol= 0;

            Console.WriteLine("GENERAR TABLA");

            //OBTENER NUMERO DE FILAS Y COLUMNAS DEL CUERPO DE LA TABLA
            int numrows  = tabla.Filas  == null ? 0 : tabla.Filas.Count;
            int numcells = tabla.Campos == null ? 0 : tabla.Campos.Count;

            nombreTabla.Attributes.Add("class", "table table-bordered");
            if (tabla.TipoTemporal.Equals("2"))
            {
                //OBTENER DATOS PARA MOSTRARLOS EN UNA TABLA SIMPLE
                DataTable table = obtenerDatosAlmacenadosProcedureTemporal(tabla);
                tablaTemp.ID = "Grid_view_" + tabla.Id;
                tablaTemp.DataSource = table;

                //tablaTemp.AutoGenerateEditButton = true;
                tablaTemp.DataBind();
            }else if (tabla.TipoTemporal.Equals("1")) {
                datos = obtenerDatosAlmacenadosProcedure(tabla);
            }else{
                datos = obtenerDatosAlmacenados(tabla);
            }

            try
            {
                if (!tabla.TipoTemporal.Equals("2"))
                {
                    //OBTENER INFORMACIÓN PARA FILAS Y COLUMNAS
                    numcolAdicional = tabla.ColAdicional.Split('|');
                    inicioCol = (!tabla.RowSpan.Equals("")) ? -2 : -1;
                    spanFilas = tabla.RowSpan.Split('|');
                    nombreTabla = agregarHeaderTabla(nombreTabla, tabla); //Agregar información del header de la tabla
                    textoSpanFilas = tabla.TextoRowSpan.Split('|');


                    for (int j = 0; j < tabla.Filas.Count; j++)
                    {
                        TableRow tr_f = new TableRow();
                        string textoFila = j < tabla.Filas.Count ? tabla.Filas[j].NombreFila : "***";
                        banderaRowSpan = false;
                        Boolean mesActivoFila = tabla.Filas[j].MesFila == Convert.ToInt32(tabla.MesActivo);

                        Boolean banderaSpan = false;
                        if (!spanFilas[0].Equals(""))
                        {
                            banderaSpan = Convert.ToInt32(spanFilas[0]) > 1 && j != numrows;
                        }

                        if ( (indiceFila > spanFilas.Length) || (indiceFila > textoSpanFilas.Length)) {
                            Console.WriteLine("Revisar informacion");
                        }
                        rowSpanNum = banderaSpan ? Convert.ToInt32(spanFilas[indiceFila]) : 1;
                        textoRowSpanTable = banderaSpan ? textoSpanFilas[indiceFila] : "";

                        if (j == numrows)
                        {
                            /*for (int k = inicioCol; k < numcells; k++)
                            {
                                TableCell celda = new TableCell();
                                int mesColumna = k < 0 ? k : Convert.ToInt32(tabla.Campos[k].MesCampo);
                                if ( (k == mesColumna || mesColumna == -1) && k>=0)
                                {
                                    Button botonGuardar = new Button();
                                    botonGuardar.Text = "Guardar";
                                    botonGuardar.CommandName = nombreTabla.ID + "_" + tabla.Campos[k].IdCampo;
                                    //botonGuardar.CausesValidation = false;
                                    botonGuardar.ID = nombreTabla.ID + "_" + tabla.Campos[k].IdCampo;
                                    botonGuardar.UseSubmitBehavior = false;
                                    //botonGuardar.OnClientClick = "this.disabled = true; this.value = 'Submitting...'; __doPostBack('" + botonGuardar.ID + "', '');";
                                    botonGuardar.Command += new CommandEventHandler(ejemplo_Click);
                                    //botonGuardar.Attributes.Add("onclick",);
                                    celda.Controls.Add(botonGuardar);
                                }
                                tr_f.Cells.Add(celda);
                            }
                            nombreTabla.Rows.Add(tr_f);*/
                        }
                        else
                        {
                            idCampo = tabla.Filas[j].IdFila.ToString();

                            for (int i = inicioCol; i < numcells; i++)
                            {
                                TableCell celda = new TableCell();

                                if (i == -2)
                                {
                                    //Celdas combinadas
                                    Label lbl = new Label();
                                    lbl.Text = (textoRowSpanTable).ToString();
                                    lbl.ID = nombreTabla.ID + "_" + j;
                                    celda.Controls.Add(lbl);
                                    String nombreClass = textoRowSpanTable.Trim().Length > 2 ? "classHeader" : "";

                                    if ((temporalRowSpan == 1 || temporalRowSpan == (sumaRowSpan + 1)) && (rowSpanNum > 1))
                                    {
                                        celda.RowSpan = rowSpanNum;
                                        banderaRowSpan = true;
                                        sumaRowSpan = rowSpanNum + sumaRowSpan;
                                        continuoClass++;
                                        celda.Attributes.Add("class", nombreClass);
                                    }
                                }
                                else if (i == -1)
                                {
                                    Label lbl = new Label();
                                    lbl.Text = textoFila;

                                    String fijarColumna = (numcells > 20  || tabla.Id == 7) ? " fijar-col1" : "";
                                    String nombreClass = textoFila.Trim().Length > 1 ? "classHeader"+fijarColumna : "";
                                    celda.Attributes.Add("class", nombreClass);
                                    celda.Controls.Add(lbl);
                                    soloLectura = textoFila.Contains("TOTAL") || textoFila.Contains("TRIMESTRE") || (!tabla.Filas[j].EsFilaActiva && tabla.TieneFilaTrimestre); // Fila de solo lectura
                                    filaTotalMensualAnual = textoFila.ToUpper().Contains("TOTAL MENSUAL") || textoFila.ToUpper().Contains("TOTAL ANUAL");
                                }
                                else
                                {
                                    

                                    //ACTIVAR O DESACTIVAR LA CELDA, DE ACUERDO A LOS MESES DE LAS FILAS Y COLUMNAS
                                    celda = activaDesactivaCelda(nombreTabla, tabla, celda, idCampo, i, j, datos, continuoClass, soloLectura, filaTotalMensualAnual);
                                }

                                if (rowSpanNum == 1 || (i == -2 && rowSpanNum > 1 && banderaRowSpan) || i > -2)
                                {
                                    tr_f.Cells.Add(celda);
                                }

                            }
                            nombreTabla.Rows.Add(tr_f);
                        } //Termina if-else

                        if (temporalRowSpan == sumaRowSpan)
                        {
                            indiceFila++;
                        }
                        temporalRowSpan++;
                    }
                }
            }
            catch (Exception ex)
            {
                String error = ex.ToString();
                Console.WriteLine("Error: " + error + "  key: " + keyObject);
            }
            finally
            {
                Boolean banderaPanel = false;
                Panel panelScroll = null;
                if (tabla.Campos.Count > 20 || tabla.Id == 7) {
                    banderaPanel = true;
                    panelScroll = new Panel();
                    panelScroll.ID = "Panel_" + tabla.Id;
                    panelScroll.Attributes.Add("style" , "overflow-x:auto ;width:auto; overflow-y: auto; height: 800px;");
                }
                //Agregar información de tabla al panel
                if (tabla.TipoTemporal.Equals("2")) {
                    ContentPlaceHolder1.Controls.Add(tablaTemp);
                }
                else {
                    Table nom_tabla = (Table)FindControl(nombreTabla.ID);
                    //botonGuardar = (Button)FindControl(botonGuardar.ID);
                    if (nom_tabla == null)
                    {
                        nom_tabla = nombreTabla;
                    }

                    Label texto = new Label();
                    texto.Text = "<br/><br/><br/>";
                    ContentPlaceHolder1.Controls.Add(texto);

                    Label textoTitulo = new Label();
                    textoTitulo.Text = tabla.NombreTabla;
                    textoTitulo.Attributes.Add("class", "classTitulo");

                    ContentPlaceHolder1.Controls.Add(textoTitulo);
                    if (banderaPanel)
                    {
                        panelScroll.Controls.Add(nom_tabla);
                        ContentPlaceHolder1.Controls.Add(panelScroll);
                    }
                    else {
                        ContentPlaceHolder1.Controls.Add(nom_tabla);
                    }
                    ContentPlaceHolder1.Controls.Add(botonGuardar);
                    ContentPlaceHolder1.Controls.Add(botonValidar);

                }

            }


        }

        private TableCell activaDesactivaCelda(Table nombreTabla, ModeloTabla tabla, TableCell celda, 
            String idCampo, int i, int j, Dictionary<String, String> datos, int continuoClass, Boolean soloLectura, Boolean esTotMensual) {
            Console.WriteLine("Agregar información");
            Dictionary<String, String> tipoDatoDefault = new Dictionary<string, string>();
            tipoDatoDefault.Add("number","0");
            tipoDatoDefault.Add("text","");
            tipoDatoDefault.Add("date", "");

            try
            {
                String tipo = tabla.Campos[i].TipoCampo;
                bool filaTotalMensual = tabla.Campos[i].NombreCampo.ToUpper().Contains("TOTAL MENSUAL");
                String keyObject = idCampo + "_" + tabla.Campos[i].IdCampo;
                String valor = "";
                String valorBD = (datos.TryGetValue(keyObject, out valor) ? datos[keyObject] : tipoDatoDefault[tipo]);
                Boolean esCampoActivo = ( tabla.Campos[i].EsActivo && tabla.Filas[j].EsFilaActiva) ;
                Boolean columnaTotal = tabla.Campos[i].NombreCampo.ToLower().Contains("total");
                Boolean columnaTotalAnual = tabla.Campos[i].NombreCampo.ToLower().Contains("anual");
                String claseFila = !tabla.Campos[i].NombreClass.Trim().Equals("") ? idCampo + "_" + tabla.Campos[i].NombreClass : "";
                String idElemento = nombreTabla.ID + "_" + idCampo + "_" + tabla.Campos[i].IdCampo;         // ID del control Textbox o Label
                String nombreClase = nombreTabla.ID + "_" + tabla.Campos[i].IdCampo + "_" + continuoClass;  // Clase que se le dará al control de la fila en caso de tener rowspan
                var classAdicional = soloLectura && !esTotMensual ? (nombreClase + "_Total") :  (nombreClase + (esTotMensual ? "_Total_Mensual" : "")) ;                  // Clase en la que se tendrá el total, en caso de ser necesario
                var classAdicionalCol = tabla.Campos[i].ClassTotal.Trim().Equals("") ? " " : idCampo + "_" + tabla.Campos[i].ClassTotal + "_Total";
                Boolean columnaTotalMensual = tabla.Campos[i].NombreCampo.ToLower().Contains("total");



                //Clases para sumar las filas
                String claseFilaSuma = nombreTabla.ID + "_" + tabla.Filas[j].ClaseSuma.Trim() + "_" + tabla.Campos[i].IdCampo  ;
                String[] claseFilaTotal = tabla.Filas[j].ClaseTotal.Split(',');

                String claseFilaTotalColumna = "";
                for (int k = 0; k < claseFilaTotal.Length; k++)
                {
                    claseFilaTotalColumna += nombreTabla.ID + "_" + claseFilaTotal[k] + "_" + tabla.Campos[i].IdCampo + ",";
                }
                claseFilaTotalColumna = claseFilaTotalColumna.TrimEnd(',');


                if (tipo.StartsWith("combo")) //Agregar datos si es un combo
                {
                    String[] tipos = tipo.Split('>');
                    mapCatalogos = (Dictionary<string, List<ModeloCatalogo>>)Session["listaCatalogos"];
                    List<ModeloCatalogo> listaElementos = mapCatalogos[tipos[1]];
                    DropDownList dl = new DropDownList();
                    dl.ID = idElemento;
                    dl.DataValueField = "id";
                    dl.DataTextField = "descripcion";
                    dl.DataSource = listaElementos;
                    dl.DataBind();
                    celda.Controls.Add(dl);
                 }
                    else if ( soloLectura  || esCampoActivo ) // Agregar input en caso de ser un campo activo o una fila de total
                    {
                        String funcion2 = !claseFila.Equals("") ? "; sumarTotales('" + claseFila + "'); " : "";
                        String funcionFila = !claseFilaSuma.Equals("") ? "; sumarTotales('" + claseFilaSuma + "'); " : "";
                        String funcionTotalGeneral = !tabla.Filas[j].ClaseTotal.Trim().Equals("") ? "; sumarTotalesGeneral('" + claseFilaTotalColumna + "', '" + nombreTabla.ID + "_" +  tabla.Campos[i].IdCampo + "_Mensual" + "'); " : "";

                        TextBox tb = (TextBox)FindControl(idElemento);
                        if (tb == null) {
                            tb = new TextBox();
                        }
                        tb.ID = idElemento;
                        tb.Text = valorBD;
                        tb.Attributes.Add("Type", tipo);

                    if (tipo.Contains("number"))
                    {
                        tb.Attributes.Add("min", "0");

                    }
                    else if (tipo.Contains("text"))
                    {

                    }
                    else if (tipo.Contains("date"))
                    {
                    }


                        tb.Attributes.Add("id", idElemento);
                    //tb.Attributes.Add("class", "form-control " + claseFilaSuma);

                    claseFilaSuma = tabla.Filas[j].ClaseSuma.Trim().Equals("") ? "" : claseFilaSuma;

                    if (esTotMensual) {
                        tb.Attributes.Add("class", "form-control " + nombreTabla.ID + "_" + tabla.Campos[i].IdCampo + "_Mensual " + claseFila + " " + classAdicionalCol);
                    }
                    else if (!claseFilaSuma.Equals(""))
                    {
                        tb.Attributes.Add("class", "form-control " + claseFilaSuma + " " + claseFila + " " + classAdicionalCol);
                        tb.Attributes.Add("onkeyup", funcionFila + " " + funcionTotalGeneral);
                    }
                    else if (claseFilaSuma.Equals("") && !tabla.Filas[j].ClaseTotal.Equals(""))
                    {
                        tb.Attributes.Add("class", "form-control " + nombreTabla.ID.Trim() + "_" + tabla.Filas[j].ClaseTotal + "_" + tabla.Campos[i].IdCampo + "_Total " + claseFila + " " + classAdicionalCol);
                    } 
                    else if (claseFilaSuma.Equals("")){
                        tb.Attributes.Add("class", "form-control " + claseFila + " " + classAdicionalCol);
                        tb.Attributes.Add("onkeyup", funcion2);
                    }

                    /*if (!classAdicionalCol.Trim().Equals(""))
                        {
                            tb.Attributes.Add("class",  nombreClase + " " + claseFila + " " + classAdicionalCol);
                        }
                        else
                        {
                            tb.Attributes.Add("class", "" + classAdicional + " " + classAdicionalCol);
                            tb.Attributes.Add("onkeyup", "sumarTotales('" + nombreClase + "')"  +  funcionTotalGeneral);
                        }*/

                    if (soloLectura || columnaTotalMensual)
                        {
                            tb.Attributes.Add("readonly", "readonly");
                        }
                        celda.Controls.Add(tb);
                    }
                    else
                        {
                            Label labelTex = (Label)FindControl(idElemento);
                            if (labelTex == null) {
                                labelTex = new Label();
                            }

                            labelTex.ID = idElemento;
                            labelTex.Text = valorBD;
                            labelTex.Attributes.Add("class", " " + nombreClase + "  " + claseFila );
                            celda.Controls.Add(labelTex);
                        }

            }
            catch (Exception error)
            {

                Console.WriteLine("ERROR AL ACTIVAR O DESACTIVAR CELDA: " + error.Message);
            }

            
           

    return celda;
}




        private Dictionary<string, string> obtenerDatosAlmacenados(ModeloTabla tabla)
        {
            String[] camposIniciales = { "anio", "idFila", "mes" };
            String[] camposAdicionales = tabla.NombresColumnas.Split('|');
            String idsColumnas = tabla.NombresColumnas.Replace("|", ",");
            String idsFilas = tabla.IdFilas.Replace("|", ",");
            Console.WriteLine("Total campos " + camposAdicionales.Length);
            Dictionary<string, string> info = new Dictionary<String, String>();
            MySqlConnection con = null;
            String key = "";
            String condicionMes = tabla.EsMensual ? " AND mes = " + tabla.MesActivo : " ";

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                sql = "SELECT  anio , idFila , " + idsColumnas + " FROM " + tabla.Tabla + " WHERE idFila IN (" + idsFilas + ") " + condicionMes + " ;";
                cmd = new MySqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                while (resultado.Read())
                {
                    String clave = resultado[1] + "_";

                    for (int i = 0; i < camposAdicionales.Length; i++)
                    {
                        String campo = camposAdicionales[i].ToString();
                        key = (clave + campo);
                        if (tabla.Tabla.Contains("tblperitospexternos"))
                        {
                            Console.Write("Tabla de peritos externos");
                        }
                        info.Add(key, resultado[campo].ToString());
                    }

                }
                con.Close();


            }
            catch (Exception error)
            {
                Console.WriteLine("Key : " + key);
                Console.WriteLine("Error: " + error.ToString());
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

                if (info.Count == 0)
                {
                    info = obtenerDatosAlmacenadosMesAnterior(tabla);
                }
            }

            return info;
        }


        private Dictionary<string, string> obtenerDatosAlmacenadosProcedure(ModeloTabla tabla)
        {
            String[] camposIniciales = { "anio", "idFila", "mes" };
            String[] camposAdicionales = tabla.NombresColumnas.Split('|');
            String idsColumnas = tabla.NombresColumnas.Replace("|", ",");
            String idsFilas = tabla.IdFilas.Replace("|", ",");
            Console.WriteLine("Total campos " + camposAdicionales.Length);
            Dictionary<string, string> info = new Dictionary<String, String>();
            MySqlConnection con = null;
            String key = "";
            String condicionMes = tabla.EsMensual ? " AND mes = " + tabla.MesActivo : " ";
            String idFila = "";
            String parameter = tabla.Parametro;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                sql = tabla.Tabla.Trim();
                cmd = new MySqlCommand(sql, con);
                if (parameter != null || !parameter.Equals("")) {
                    cmd.Parameters.AddWithValue("_tipo", parameter);
                }
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataReader resultado = cmd.ExecuteReader();
                while (resultado.Read())
                {
                    String idFilaTemp = tabla.Filas[Convert.ToInt32(resultado["mes"])-1].IdFila.ToString();
                    String clave = idFilaTemp + "_";

                    for (int i = 0; i < camposAdicionales.Length; i++)
                    {
                        String campo = camposAdicionales[i].ToString();
                        key = (clave + campo);

                        info.Add(key, resultado[campo].ToString());
                    }

                }
                con.Close();


            }
            catch (Exception error)
            {
                Console.WriteLine("Key : " + key);
                Console.WriteLine("Error: " + error.ToString());
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

            return info;
        }


        private DataTable obtenerDatosAlmacenadosProcedureTemporal(ModeloTabla tabla)
        {
            List<String[]> lista = new List<String[]>();
            MySqlConnection con = null;
            DataTable dt = new DataTable("Tabla_"+tabla.Id);

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"] + "CHARSET=utf8");
                con.Open();
                sql = tabla.Tabla.Trim();
                cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("_id", tabla.Parametro);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataReader resultado = cmd.ExecuteReader();
                /*while (resultado.Read())
                {
                    dt.Load(resultado);

                   int total =  resultado.FieldCount;
                   String[] obj = new String[total];
                    for (int i = 0; i < total; i++)
                    {
                        obj[i] = resultado[i].ToString();
                    }
                    lista.Add(obj);
                }*/
                dt.Load(resultado);
                con.Close();


            }
            catch (Exception error)
            {
                Console.WriteLine("Error: " + error.ToString());
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

            
            return dt;
        }



        private List<ModeloCatalogo> obtenerDatosCatalogo(int parameter)
        {
            MySqlConnection con = null;

            /*String[] camposIniciales = { "anio", "idFila", "mes" };
            String[] camposAdicionales = tabla.NombresColumnas.Split('|');
            String idsColumnas = tabla.NombresColumnas.Replace("|", ",");
            String idsFilas = tabla.IdFilas.Replace("|", ",");
            Console.WriteLine("Total campos " + camposAdicionales.Length);
            Dictionary<string, string> info = new Dictionary<String, String>();
            String key = "";
            String condicionMes = tabla.EsMensual ? " AND mes = " + tabla.MesActivo : " ";
            String idFila = "";
            String parameter = tabla.Parametro;*/
            List<ModeloCatalogo> lista = new List<ModeloCatalogo>(); 

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                sql = "obtenCatalogos";
                cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("idcatalogo", parameter);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataReader resultado = cmd.ExecuteReader();
                while (resultado.Read())
                {
                    ModeloCatalogo obj = new ModeloCatalogo();
                    obj.Id = Convert.ToInt32(resultado["id"]);
                    obj.Descripcion = resultado["descripcion"].ToString();
                    
                    lista.Add(obj);
                }
                con.Close();
            }
            catch (Exception error)
            {
                // Console.WriteLine("Key : " + key);
                Console.WriteLine("Error: " + error.ToString());
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

            return lista;
        }


        private Dictionary<string, string> obtenerDatosAlmacenadosMesAnterior(ModeloTabla tabla)
        {
            String[] camposIniciales = { "anio", "idFila", "mes" };
            String[] camposAdicionales = tabla.NombresColumnas.Split('|');
            String idsColumnas = tabla.NombresColumnas.Replace("|", ",");
            String idsFilas = tabla.IdFilas.Replace("|", ",");
            Console.WriteLine("Total campos " + camposAdicionales.Length);
            Dictionary<string, string> info = new Dictionary<String, String>();
            MySqlConnection con = null;
            String key = "";
            String condicionMes = tabla.EsMensual ? " AND mes = " + (Convert.ToInt32(tabla.MesActivo) - 1) : " ";

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                sql = "SELECT  anio , idFila , " + idsColumnas + " FROM " + tabla.Tabla + " WHERE idFila IN (" + idsFilas + ") " + condicionMes + " ;";
                cmd = new MySqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                while (resultado.Read())
                {
                    String clave = resultado[1] + "_";

                    for (int i = 0; i < camposAdicionales.Length; i++)
                    {
                        String campo = camposAdicionales[i].ToString();
                        key = (clave + campo);
                        if (tabla.Tabla.Contains("tblperitospexternos"))
                        {
                            Console.Write("Tabla de peritos externos");
                        }
                        info.Add(key, resultado[campo].ToString());
                    }
                }
                con.Close();
            }
            catch (Exception error)
            {
                Console.WriteLine("Key : " + key);
                Console.WriteLine("Error: " + error.ToString());
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

            return info;
        }



        private Table agregarHeaderTabla(Table tabla, ModeloTabla objTabla)
        {
            String[] adicional1 = { "1", "1" } , adicional2 = { "1"};
            int inicioCol = (!objTabla.ColSpan.Equals("")) ? -2 : -1;
            String[] columnaAdicional = objTabla.ColAdicional.Split('|');
            String[] csHeader = objTabla.ColAdicional.Split('|').Length > 1 ? adicional1 : adicional2;
            String[] spanColumnas = objTabla.ColSpan.Split('|');
            csHeader.Concat(spanColumnas).ToArray();
            String[] textoSpanColumnas = objTabla.TextoColSpan.Split('|');
            int totalFilasHeader = inicioCol == -2 ? 2 : 1;
            int totalColumnas = (totalFilasHeader) + objTabla.Campos.Count();
            String fijarFila1 = "", fijarFila2="";

            fijarFila1 = (totalColumnas > 20 || objTabla.Id == 7) ? " fijar-r1" : "";
            fijarFila2 = (totalColumnas > 20 && totalFilasHeader == 2) || objTabla.Id == 7  ? " fijar-r2" : "";

            try
            {

                //AGREGAR HEADER DE LA TABLA
                for (int i = 0; i < totalFilasHeader; i++)
                {
                    TableRow tr_f = new TableRow();
                    if (i == 0 && spanColumnas.Length > 0 && !spanColumnas[0].Equals(""))
                    {
                        for (int j = 0; j < (spanColumnas.Length + csHeader.Length); j++)
                        {
                            String colSpanHead = (j < csHeader.Length) ? csHeader[j] : spanColumnas[j - csHeader.Length];
                            TableCell celda = new TableCell();
                            Label lbl = new Label();
                            lbl.Text = (j < csHeader.Length) ? " " : textoSpanColumnas[j - csHeader.Length];
                            String nombreClass = lbl.Text.Trim().Length > 3 ? "classHeader"+ fijarFila1 : "" + fijarFila1 ;
                            celda.Attributes.Add("class", nombreClass);
                            celda.Controls.Add(lbl);
                            celda.ColumnSpan = Convert.ToInt32(colSpanHead);
                            tr_f.Cells.Add(celda);
                        }
                    }
                    else
                    {
                        if (objTabla.Id == 7) {
                            fijarFila2 = fijarFila1;
                        }
                        for (int k = 0; k < (objTabla.Campos.Count + csHeader.Length); k++)
                        {
                            String textoLabel = (k < csHeader.Length) ? columnaAdicional[k] : objTabla.Campos[k - csHeader.Length].NombreCampo;
                            String nombreClass = textoLabel.Trim().Length > 2 ? "classHeader" + fijarFila2 : ""+ fijarFila2;
                            TableCell celda = new TableCell();
                            Label lbl = new Label();
                            lbl.Text = textoLabel;
                            celda.Attributes.Add("class", nombreClass);
                            celda.Controls.Add(lbl);
                            tr_f.Cells.Add(celda);
                        }

                    }
                    tabla.Rows.Add(tr_f);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("ERROR " + error.Message);
            }
            return tabla;
        }


       


        public void ejemplo_Click(object sender, CommandEventArgs e)
        {
            t = (Dictionary<String, ModeloTabla>)Session["infoTablas"];
            Dictionary<String, String> usuario = (Dictionary<string, string>)Session["usuario"];

            String nombre;
            String[] subnombre;
            Boolean esInformacionMensual, guardarPorRegistro;
            try
            {

            nombre = e.CommandName;
            subnombre = nombre.Split('_');
            esInformacionMensual = t[subnombre[0]].EsMensual;
            String campoMes = "", onDuplicateMes = "", valorMes = "";

            String[] camposGuardar = t[subnombre[0]].CamposActivos.TrimEnd(',').Split(',');

                Console.WriteLine("Total CamposGuardar : " + camposGuardar.Length);
                String[] camposActivosGuardar = nombre.TrimEnd(',').Split(',');
                if (esInformacionMensual)
                {
                    campoMes = " mes , ";
                    onDuplicateMes = " mes = VALUES(mes) , ";
                    valorMes = t[subnombre[0]].MesActivo + " , ";
                }



                for (int j = 0; j < camposGuardar.Length; j++)
                {
                    List<String> lista = new List<string>();

                    Console.WriteLine("Comando: " + subnombre[0] + subnombre[1]);
                    subnombre[1] = camposGuardar[j];
                    String valores = "", idFila = "";
                    String onDuplicate = onDuplicateMes + " idFila = VALUES(idFila) ,  fechaActualiza = VALUES(fechaActualiza), usuarioActualiza = VALUES(usuarioActualiza) , " + subnombre[1] + " = VALUES(" + subnombre[1] + ") ";
                    String[] controles = HttpContext.Current.Request.Form.AllKeys;
                    String campos = " (anio, idFila,fechaActualiza,usuarioActualiza," + campoMes + camposGuardar[j] + " )";
                    for (int i = 0; i < controles.Length; i++)
                    {
                        if (controles[i] != null)
                        {
                            String[] fila = controles[i].Split('_');

                            idFila = (fila.Length > 2) ? fila[fila.Length - 2] : "";

                            if (!idFila.Equals("") && controles[i].Contains(subnombre[0]) && controles[i].EndsWith("_"+camposGuardar[j]))
                            {
                                String valor = HttpContext.Current.Request.Form[i];
                                valores += "  ( " + AnioConsulta.SelectedValue + " , " + idFila + " , now(), "  +  usuario["idusuarios"]  +" , " + valorMes + (valor.Equals("") ? "0" : valor) + ")  ,";
                                Console.WriteLine("Valor : " + valor);
                            }
                        }
                    }
                    valores = valores.TrimEnd(',');
                    Console.WriteLine("VALORES: " + valores);
                    if (!valores.Equals(""))
                    {
                        lista.Add(campos);
                        lista.Add(valores);
                        lista.Add(onDuplicate);
                        insertarInformacion(subnombre[0], lista);
                    }

                }
                Session["ButtonCreated"] = true;

            }
            catch (Exception exc)
            {

                Console.WriteLine("ERROR: " + exc.ToString());
            }
            finally
            {
                Console.WriteLine("Cargar controles");
            }

        }


        public void ejemplo_Click2(object sender, CommandEventArgs e)
        {
            t = (Dictionary<String, ModeloTabla>)Session["infoTablas"];
            String nombre = "" , campos=" ( ", valores=" ( ";
            String[] subnombre;
            Boolean esInformacionMensual, guardarPorRegistro;
            try
            {

                nombre = e.CommandName;
                subnombre = nombre.Split('_');
                esInformacionMensual = t[subnombre[0]].EsMensual;
                String campoMes = "", onDuplicateMes = "", valorMes = "";

                String[] camposGuardar = t[subnombre[0]].CamposActivos.TrimEnd(',').Split(',');
                List <ModeloCampo> tipoDat = t[subnombre[0]].Campos;

                Console.WriteLine("Total CamposGuardar : " + camposGuardar.Length);
                String[] camposActivosGuardar = nombre.TrimEnd(',').Split(',');

                List<String> lista = new List<string>();
                String onDuplicate = "";


                for (int j = 0; j < camposGuardar.Length; j++)
                {

                    Console.WriteLine("Comando: " + subnombre[0] + subnombre[1]);
                    subnombre[1] = camposGuardar[j];
                    String idFila = "";
                    onDuplicate = onDuplicateMes + " idFila = VALUES(idFila) , " + subnombre[1] + " = VALUES(" + subnombre[1] + ") ";
                    String[] controles = HttpContext.Current.Request.Form.AllKeys;
                    for (int i = 0; i < controles.Length; i++)
                    {
                        if (controles[i] != null)
                        {
                            String[] fila = controles[i].Split('_');

                            idFila = (fila.Length > 2) ? fila[fila.Length - 2] : "";
                            //idFila = (fila.Length == 4) ? fila[fila.Length - 3] : idFila;

                            if (!idFila.Equals("") && controles[i].Contains(subnombre[0]) && controles[i].EndsWith(camposGuardar[j]))
                            {
                                String valor = HttpContext.Current.Request.Form[i];
                                campos += " " + camposGuardar[j] + " ,";
                                String modifValor = !tipoDat[j].TipoCampo.Equals("number") || tipoDat[j].TipoCampo.StartsWith("combo") ? "'" + valor + "'" : valor;
                                valores += " " +  (valor.Equals("''") ? "NULL" : modifValor) + " ,";
                            }
                        }
                    }


                }
                if (!valores.Equals(""))
                {
                    campos = campos.TrimEnd(',');
                    valores = valores.TrimEnd(',');
                    Console.WriteLine("VALORES: " + valores);
                    lista.Add(campos + " )");
                    lista.Add(valores + " )");
                    lista.Add(onDuplicate);
                    insertarInformacion(subnombre[0], lista);

                }
            }
            catch (Exception exc)
            {

                Console.WriteLine("ERROR: " + exc.ToString());
            }
            finally
            {
                Console.WriteLine("Cargar controles");
                //CargarControles();
            }

        }



        public void ejemplo_ClickValidar(object sender, CommandEventArgs e)
        {
            t = (Dictionary<String, ModeloTabla>)Session["infoTablas"];
            String nombre = "";
            MySqlConnection con = null;
            String query = "";
            String[] subnombre;
            String nombreTabla = "Tabla";
            Dictionary<String, String> usuario = (Dictionary<string, string>)Session["usuario"];

            try
            {
                nombre = e.CommandName;
                subnombre = nombre.Split('_');
                nombreTabla += subnombre[1];
                String campoMesActivo = mesesAnio[Convert.ToInt32(subnombre[2]) - 1];
                query = "INSERT INTO  tblvalidacion (anio,idtabla, " + campoMesActivo + ", fechaActualiza, idUsuarioValida) " +
                        "   VALUES (" + Session["anioConsulta"] + " , " + subnombre[1] + ", 1 , now() , " +  usuario["idusuarios"] + " ) " +
                        "   ON DUPLICATE KEY UPDATE idtabla = VALUES(idtabla) , " + campoMesActivo + " = VALUES(" + campoMesActivo + ") , idUsuarioValida =  VALUES(idUsuarioValida) ";
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(query, con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception exc)
            {

                Console.WriteLine("ERROR: " + exc.ToString());
            }
            finally
            {
                if (con != null)
                {
                    con.Dispose();
                    con.Close();
                }
                Console.WriteLine("Cargar controles");
                ContentPlaceHolder1.Controls.Clear();
                GeneraControlesDinamicos();
            }

        }



        private long insertarInformacion(String tabla, List<String> datos)
        {
            long id = 0;
            MySqlConnection con = null;
            d = (Dictionary<String, String>)Session["userpass"];
            t = (Dictionary<String, ModeloTabla>)Session["infoTablas"];
            String query = "";
            try
            {
                query = "INSERT into  " + t[tabla].Tabla
                   + " " + datos[0] +
                   "   VALUES " + datos[1] +
                   "   ON DUPLICATE KEY UPDATE " + datos[2];

                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(query, con);
                cmd.ExecuteNonQuery();

                //id = cmd.LastInsertedId;
                /*ScriptManager.RegisterStartupScript(this, this.GetType(), "Correcto insertar", "mensaje('exitoso','registro exitoso','success');", true);*/
                con.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar la información " + ex.ToString());

            }
            finally
            {
                if (con != null)
                {
                    con.Dispose();
                    con.Close();
                }
            }

            return id;
        }



        public void generarInformacionExcel() {
            try
            {
                t = null;
                Session["filasEstilos"] = new List<ModeloRangoCelda>();
                Session["estiloFila"] = new List<ModeloRangoCelda>();
                Dictionary<String, String> usuario = (Dictionary<string, string>)Session["usuario"];

                tablasAccesoArea = ConsultarTablasPorArea(Convert.ToInt32(usuario["area"]));
                obtenerDatosTablasExcel(tablasAccesoArea);
                t = (Dictionary<String, ModeloTabla>)Session["infoTablasExcel"];

                ExcelPackage excel = null;
                excel = new ExcelPackage();

                ExcelWorksheet hoja;
                hoja = excel.Workbook.Worksheets.Add("Hoja 1");

                for (int i = 0; i < tablasAccesoArea.Count ; i++)
                {
                    Session["celdasMerge"] = new List<ModeloRangoCelda>();
                    DataTable tabla = new DataTable();
                    tabla.TableName = "Tabla_excel_" + i;
                    String[] temas = t["Tabla" + tablasAccesoArea[i]].NombreTabla.Split('|');  //Obtener temas y subtemas y agregarlos a la hoja
                    
                    //Agregar título y/o subtitulo de la tabla
                    for (int j = 0; j < temas.Length; j++)
                    {
                        hoja.Cells[celdaInicioTabla[0] + (j+1), celdaInicioTabla[1]].Value = temas[j]; //Agregar texto en celda especificada
                        hoja.Cells[celdaInicioTabla[0] + (j + 1), celdaInicioTabla[1]].Style.Font.Bold = true;
                    }
                    
                    celdaInicioTabla[0] = celdaInicioTabla[0] + temas.Length + espacioTablas; //Aumentar fila
                    tabla = generarDataTableExcel(tabla, t["Tabla" + tablasAccesoArea[i]], celdaInicioTabla , hoja); //Generar DataTable con datos para agregarla a la hoja posteriormente


                    hoja.Cells[celdaInicioTabla[0] , celdaInicioTabla[1] ].LoadFromDataTable(tabla, true);  // Cargar tabla en la hoja excel
                    hoja = aplicarEstilosTablasExcel(hoja, i , t["Tabla" + tablasAccesoArea[i]]); //Aplicar estilos a los rangos de celdas de las tablas


                    // Modificar celdas de inicio para la siguiente tabla
                    int totalFilasTable = t["Tabla" + tablasAccesoArea[i]].Filas == null ? tabla.Rows.Count : t["Tabla" + tablasAccesoArea[i]].Filas.Count;
                    celdaInicioTabla[0] = celdaInicioTabla[0] + totalFilasTable + 4 + temas.Length;
                    celdaInicioTabla[1] = 2;
                }

                //Ajustar texto de la columna de inicio y fijar tamaño de la columna
                hoja.Column(celdaInicioTabla[1] ).Width = 50;
                hoja.Column(celdaInicioTabla[1] ).Style.WrapText = true;


                //DESCARGAR EXCEL
                descargarExcel(excel);
            }
            catch (Exception error) {
                Console.WriteLine("ERROR: " + error.Message);
            }
            
        }


        private ExcelWorksheet aplicarEstilosTablasExcel(ExcelWorksheet hojaFinal, int index, ModeloTabla tabla) {
            String celdaInicio = "", celdaFin = "", celdaInicioTabla ="", celdaFinTabla="";
            List<ModeloRangoCelda> listaCombina = (List<ModeloRangoCelda>)Session["celdasMerge"];
            List<ModeloRangoCelda> listaEstilos = (List<ModeloRangoCelda>)Session["filasEstilos"];
            int indexFila = -1, indexColumna = -1;
            String textoCS = tabla.ColAdicional.Split('|').Length == 1 ? " |" : " | |";
            
            try
            {
                //Estilos para las fila con celdas combinadas
                if (listaCombina.Count > 0)
                {
                    int[] inicio = listaCombina[0].CeldaInicio;
                    int[] fin = listaCombina[listaCombina.Count - 1].CeldaFin;

                    celdaInicio = hojaFinal.Cells[inicio[0], inicio[1]].Address;
                    celdaFin = hojaFinal.Cells[fin[0], fin[1]].Address;
                    hojaFinal.Cells[celdaInicio + ":" + celdaFin].Style.Font.Color.SetColor(ColorTranslator.FromHtml("WHITE"));
                    hojaFinal.Cells[celdaInicio + ":" + celdaFin].Style.Font.Bold = true;
                    hojaFinal.Cells[celdaInicio + ":" + celdaFin].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hojaFinal.Cells[celdaInicio + ":" + celdaFin].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#b71c1c"));
                    hojaFinal.Cells[celdaInicio + ":" + celdaFin].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //hojaFinal.Cells[celdaInicio + ":" + celdaFin].Style.Border;
                }

                //Combinar celdas 
                String[] textoColSpan = (textoCS+ tabla.TextoColSpan).Split('|');
                String[] textoRowSpan = (tabla.TextoRowSpan + "| ").Split('|');
                for (int k = 0; k < listaCombina.Count; k++)
                {
                    if (listaCombina[k].EsTipoFila)
                    {
                        indexFila++;
                    }
                    else {
                        indexColumna++;
                    }
                    int[] inicio = listaCombina[k].CeldaInicio;
                    int[] fin = listaCombina[k].CeldaFin;

                    hojaFinal.Cells[inicio[0], inicio[1], fin[0], fin[1]].Merge = true;
                    String text = listaCombina[k].EsTipoFila ? textoRowSpan[indexFila] : textoColSpan[indexColumna];
                    hojaFinal.Cells[inicio[0], inicio[1], fin[0], fin[1]].Value = text;
                    hojaFinal.Cells[inicio[0], inicio[1], fin[0], fin[1]].Style.Border.Top.Style = ExcelBorderStyle.Dotted;
                    hojaFinal.Cells[inicio[0], inicio[1], fin[0], fin[1]].Style.Border.Top.Color.SetColor(Color.Gray);
                    hojaFinal.Cells[inicio[0], inicio[1], fin[0], fin[1]].Style.Border.Bottom.Style = ExcelBorderStyle.Dotted;
                    hojaFinal.Cells[inicio[0], inicio[1], fin[0], fin[1]].Style.Border.Bottom.Color.SetColor(Color.Gray);
                    hojaFinal.Cells[inicio[0], inicio[1], fin[0], fin[1]].Style.Border.Left.Style = ExcelBorderStyle.Dotted;
                    hojaFinal.Cells[inicio[0], inicio[1], fin[0], fin[1]].Style.Border.Left.Color.SetColor(Color.Gray);
                    hojaFinal.Cells[inicio[0], inicio[1], fin[0], fin[1]].Style.Border.Right.Style = ExcelBorderStyle.Dotted;
                    hojaFinal.Cells[inicio[0], inicio[1], fin[0], fin[1]].Style.Border.Right.Color.SetColor(Color.Gray);

                }


                //Aplicar estilo (color de texto y background) al rango de celdas
                for (int i = 0; i < listaEstilos.Count; i++)
                {
                    int[] inicio = listaEstilos[i].CeldaInicio;
                    int[] fin = listaEstilos[i].CeldaFin;

                    celdaInicio = hojaFinal.Cells[inicio[0], inicio[1]].Address;
                    celdaFin = hojaFinal.Cells[fin[0] , fin[1]].Address;

                    /*ExcelStyle estilo = new ExcelStyle();
                    estilo.Font.Color*/

                    hojaFinal.Cells[celdaInicio + ":" + celdaFin].Style.Font.Color.SetColor(ColorTranslator.FromHtml("WHITE"));
                    hojaFinal.Cells[celdaInicio + ":" + celdaFin].Style.Font.Bold = true;
                    hojaFinal.Cells[celdaInicio + ":" + celdaFin].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hojaFinal.Cells[celdaInicio + ":" + celdaFin].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#d32f2f"));
                    //hojaFinal.Cells[celdaInicio + ":" + celdaFin].Style.WrapText = true;
                }

                //Agregar bordes a la tabla
                hojaFinal.Cells[hojaFinal.Dimension.Address].AutoFitColumns();


            }
            catch (Exception error)
            {

                Console.WriteLine("Error en aplicarEstilos "+ error.ToString());
            }
            Session["celdasMerge"] = null;
            Session["filasEstilos"] = null;

            return hojaFinal;
        
        }


        private DataTable generarDataTableExcel(DataTable nombreTabla, ModeloTabla tabla, int[] celdaInicio , ExcelWorksheet hoja) {
            Table data = new Table();
            Dictionary<String, String> datos = new Dictionary<string, string>();
            GridView tablaTemp = new GridView(); ;
            DataGrid tablaExc = new DataGrid();
            String[] spanFilas, textoSpanFilas, numcolAdicional;
            Boolean banderaRowSpan, banderaFilaCombinada=false;
            String textoRowSpanTable = "";
            Boolean deshabilitar = false;
            String keyObject = "";
            String idCampo = "";
            int temporalRowSpan = 1;
            int indiceFila = 0;
            int rowSpanNum = 0;
            int sumaRowSpan = 0;
            int continuo = 0, continuoClass = 0;
            int inicioCol = (tabla.TextoRowSpan!=null && tabla.TextoRowSpan.Length > 2 )? 1 : 0;
            int[] celdaFinFila = new int[2];

            int[] celdaFin = { 0, 0 };

            Console.WriteLine("-> GENERAR TABLA PARA EXCEL");

            //OBTENER NUMERO DE FILAS Y COLUMNAS DEL CUERPO DE LA TABLA
            int numrows = tabla.Filas == null ? 0 : tabla.Filas.Count;
            int numcells = tabla.Campos == null ? 0 : tabla.Campos.Count;

            obtenListaCeldasEstilos(celdaInicio, numcells, false);
            obtenListaCeldasEstilos(celdaInicio, numrows , true);

            


            if (tabla.TipoTemporal.Equals("2"))
            {
                //OBTENER DATOS PARA MOSTRARLOS EN UNA TABLA SIMPLE
                DataTable table = obtenerDatosAlmacenadosProcedureTemporal(tabla);
                tablaTemp.ID = "Grid_view_" + tabla.Id;
                tablaTemp.DataSource = table;
                tablaTemp.DataBind();
                Console.WriteLine("Total filas: "+tablaTemp.Rows.Count);
                DataTable miTabla = (DataTable)tablaTemp.DataSource;

                obtenListaCeldasEstilos(celdaInicio,  table.Columns.Count  ,  false);

                return miTabla;
            }
            else if (tabla.TipoTemporal.Equals("1"))
            {
                datos = obtenerDatosAlmacenadosProcedure(tabla);
                Console.WriteLine("Datos encontrados: "+datos.Count);
            }
            else
            {
                datos = obtenerDatosAlmacenados(tabla);
            }

            try
            {
                if (!tabla.TipoTemporal.Equals("2"))
                {
                    //Modificar celda de inicio en caso de ser necesario
                    
                    //OBTENER INFORMACIÓN PARA FILAS Y COLUMNAS
                    numcolAdicional = tabla.ColAdicional.Split('|');
                    inicioCol = (!tabla.RowSpan.Equals("")) ? 2 : 1;
                    spanFilas = tabla.RowSpan.Split('|');
                    nombreTabla = agregarHeaderTablaExcel(nombreTabla, tabla, celdaInicio); //Agregar información del header de la tabla
                    textoSpanFilas = tabla.TextoRowSpan.Split('|');
                    int spanTemporal = 0;
                    int inicioRow = (!tabla.ColSpan.Equals("")) ? -2 : -1;
                    int totalFilasHeader = inicioRow == -2 ? 2 : 1;


                    for (int i = 0; i < tabla.Filas.Count ; i++)
                    {
                        int contTemp = 0;
                        idCampo = tabla.Filas[i].IdFila.ToString();
                        string textoFila = i < tabla.Filas.Count ? tabla.Filas[i].NombreFila : "***";

                        Boolean banderaSpan = false;
                        if (!spanFilas[0].Equals(""))
                        {
                            banderaSpan = Convert.ToInt32(spanFilas[0]) > 1 && i != numrows;
                        }

                        rowSpanNum = banderaSpan ? Convert.ToInt32(spanFilas[indiceFila]) : 1;
                        textoRowSpanTable = banderaSpan ? textoSpanFilas[indiceFila] : "";
                      

                        DataRow fila = nombreTabla.NewRow();
                        for (int j = 0 ; j < (tabla.Campos.Count + inicioCol); j++)
                        {

                            if (inicioCol == 2 && j==0)
                            {
                                fila[j] = textoRowSpanTable;
                                int[] celdaInicioFila = new int[2];
                                celdaInicioFila[0] = celdaInicio[0] + totalFilasHeader + (spanTemporal);
                                celdaInicioFila[1] = celdaInicio[1] ;
                                if (!banderaFilaCombinada) {
                                    obtenCeldaCombinada(celdaInicioFila, rowSpanNum, 0, true);
                                    spanTemporal = spanTemporal + rowSpanNum;
                                }

                                if ((temporalRowSpan == 1 || temporalRowSpan == (sumaRowSpan + 1)) && (rowSpanNum > 1))
                                {
                                    banderaRowSpan = true;
                                    sumaRowSpan = rowSpanNum + sumaRowSpan;
                                }
                                banderaFilaCombinada = true;
                               

                            }
                            else if ( (inicioCol == 1 && contTemp==0) || (j==1  && contTemp == 0))
                            {
                                fila[j] = textoFila;
                                contTemp++;
                                int[] celdaInicioFila = new int[2];
                                if (inicioCol == 2) {
                                    celdaInicioFila[0] = celdaInicio[0];
                                    celdaInicioFila[1] = celdaInicio[1] + 1;
                                    obtenListaCeldasEstilos(celdaInicioFila, tabla.Filas.Count , true);
                                }
                            }
                            else {
                                int indice = j - inicioCol;
                                String key = idCampo + "_" + tabla.Campos[indice].IdCampo;
                                keyObject = key;

                                String valor = "";
                                String valorBD = "";
                                try
                                {
                                    valorBD = (datos.TryGetValue(key, out valor) ? datos[key] : "0");
                                }
                                catch (Exception)
                                {
                                    //Console.log("Error al encontrar key: "+ key);  
                                }
                               
                                fila[j] = valorBD;
                            }
                        }//Termina for j

                        if (temporalRowSpan == sumaRowSpan)
                        {
                            indiceFila++;
                            banderaFilaCombinada = false;
                        }
                        temporalRowSpan++;

                        nombreTabla.Rows.Add(fila);
                    }//Termina for i


                }
            }
            catch (Exception ex)
            {
                String error = ex.ToString();
                Console.WriteLine("Error: " + error + "  key: " + keyObject);
            }
            finally
            {
                

            }
            return nombreTabla;

        }


        private DataTable agregarHeaderTablaExcel(DataTable tabla, ModeloTabla objTabla, int[] celda)
        {
            String[] adicional1 = { "1", "1" }, adicional2 = { "1" } , colTemporal = { "-", "."};
            int inicioCol = (!objTabla.ColSpan.Equals("")) ? -2 : -1;
            String[] columnaAdicional = objTabla.ColAdicional.Split('|');
            String[] csHeader = objTabla.ColAdicional.Split('|').Length > 1 ? adicional1 : adicional2;
            String[] spanColumnas = objTabla.ColSpan.Split('|');
            csHeader.Concat(spanColumnas).ToArray();
            String[] textoSpanColumnas = objTabla.TextoColSpan.Split('|');
            int totalFilasHeader = inicioCol == -2 ? 2 : 1;
            int totalColumnas = (totalFilasHeader) + objTabla.Campos.Count();
            DataRow fila;
            Boolean banderaColumna = false;

            try
            {
                obtenListaCeldasEstilos( celda , objTabla.Campos.Count + csHeader.Length , false);



                //AGREGAR HEADER DE LA TABLA
                for (int i = 0; i < totalFilasHeader; i++)
                {
                    TableRow tr_f = new TableRow();
                    if (i == 0 && spanColumnas.Length > 0 && !spanColumnas[0].Equals(""))
                    {
                        DataRow filaEnc = tabla.NewRow();
                        banderaColumna = true;
                        int contadorColumna = 0;

                        for (int j = 0; j < (spanColumnas.Length + csHeader.Length); j++)
                        {
                            String textoColumna = (j < csHeader.Length) ? colTemporal[j] : textoSpanColumnas[j - csHeader.Length];
                            String colSpanHead = (j < csHeader.Length) ? csHeader[j] : spanColumnas[j - csHeader.Length];
                            for (int k = 0; k < Convert.ToInt32(colSpanHead); k++)
                            {
                                textoColumna = (k == 0) ? textoColumna : textoColumna + "_" + i;
                                tabla.Columns.Add(textoColumna, typeof(String));
                                contadorColumna++;
                            }

                            if (colSpanHead.Equals("1") && j == 1 && csHeader.Length == 2)
                            {
                                int[] temporalCelda = new int[2];
                                temporalCelda[0] = celda[0];
                                temporalCelda[1] = celda[1] + (totalFilasHeader - 1);
                                obtenCeldaCombinada(temporalCelda, Convert.ToInt32(colSpanHead), contadorColumna, false);

                            }
                            else {
                                obtenCeldaCombinada(celda, Convert.ToInt32(colSpanHead), contadorColumna, false);
                            }


                        }
                        //tabla.Rows.Add(filaEnc);

                    }
                    else
                    {
                        DataRow filaSub = tabla.NewRow();

                        for (int k = 0; k < (objTabla.Campos.Count + csHeader.Length); k++)
                        {
                            String textoLabel = (k < csHeader.Length) ? columnaAdicional[k] : objTabla.Campos[k - csHeader.Length].NombreCampo;

                            if (banderaColumna)
                            {
                                filaSub[k] = textoLabel;
                            }
                            else
                            {
                                tabla.Columns.Add(textoLabel, typeof(String));
                            }
                        }

                        if (banderaColumna)
                        {
                            tabla.Rows.Add(filaSub);
                        }
                    }
                }//Termina for

                int[] celdaNuevoInicio = new int[2];
                celdaNuevoInicio[0] = celda[0] + (totalFilasHeader > 1 ? 1 : 0);
                celdaNuevoInicio[1] = celda[1];
                obtenListaCeldasEstilos(celdaNuevoInicio, (objTabla.Campos.Count + csHeader.Length) , false);
            }
            catch (Exception error)
            {
                Console.WriteLine("ERROR " + error.Message);
            }
            return tabla;
        }


        private void  obtenCeldaCombinada(int[] celdaInicio, int span, int indice, Boolean esFila)
        {
            List<ModeloRangoCelda> listaTemporal = (List<ModeloRangoCelda>)Session["celdasMerge"];
            ModeloRangoCelda rango = new ModeloRangoCelda();
            
            int[] celdaFin = new int[2];
            int[] celdaDeInicio = new int[2];
            if (!esFila)
            {
                if (span == 1)
                {
                    celdaDeInicio[0] = celdaInicio[0];
                    celdaDeInicio[1] = celdaInicio[1];
                    celdaFin[0] = celdaInicio[0];
                    celdaFin[1] = celdaInicio[1];
                }
                else
                {
                    int sizeLista = listaTemporal.Count;
                    if (sizeLista > 0)
                    {
                        var obj = listaTemporal[sizeLista - 1].CeldaFin;
                        celdaDeInicio[0] = obj[0];
                        celdaDeInicio[1] = obj[1] + 1;
                    }

                    celdaFin[0] = celdaDeInicio[0];
                    celdaFin[1] = celdaDeInicio[1] + (span - 1);
                }
            }
            else {
                int sizeLista = listaTemporal.Count;
                celdaDeInicio = celdaInicio;

                celdaFin[0] = celdaDeInicio[0] + (span - 1);
                celdaFin[1] = celdaDeInicio[1];

            }

            if (listaTemporal == null)
            {
                listaTemporal = new List<ModeloRangoCelda>();
            }

            rango.EsTipoFila = esFila;
            rango.CeldaInicio = celdaDeInicio;
            rango.CeldaFin = celdaFin;

            Boolean duplicado = false;
            for (int i = 0; i < listaTemporal.Count; i++)
            {
                if (listaTemporal[i].CeldaInicio == celdaDeInicio && listaTemporal[i].CeldaFin == celdaFin) {
                    duplicado = true;
                }
            }
            if (!duplicado) {
                listaTemporal.Add(rango);
            }
            Session["celdasMerge"] = listaTemporal; 
        }


        private void obtenListaCeldasEstilos(int[] celda , int total, Boolean esFila) {
            //Obtener celda inicial y final de la cabecera
            int[] inicio = new int[2];
            int[] fin = new int[2];
            int totalFila = 0, totalColumna = 0;
            if (esFila) { totalFila = total; } else { totalColumna = total - 1; }

            var listaEstilos = (List<ModeloRangoCelda>)Session["filasEstilos"];

            if (listaEstilos == null)
            {
                listaEstilos = new List<ModeloRangoCelda>();
            }

            inicio = celda;
            fin[0] = celda[0] + totalFila;
            fin[1] = celda[1] + totalColumna;
            
            ModeloRangoCelda rango = new ModeloRangoCelda();
            rango.EsTipoFila = esFila;
            rango.CeldaInicio = inicio;
            rango.CeldaFin = fin;

            listaEstilos.Add(rango);

            Session["filasEstilos"] = listaEstilos;
        }


        private void descargarExcel(ExcelPackage archivo)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Reporte_SIE.xlsx");
                archivo.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
            catch (Exception error)
            {

                Console.WriteLine("Error al descargar archivo " + error.ToString());
            }

        }





        protected void ActualizaInformacionTabla(ModeloTabla objTabla)
        {
            Boolean validado = objTabla.MesActualValidado;
            int id = objTabla.Id;
            Table tabla = (Table)FindControl("Tabla" + id);

            Button botonGuardar = (Button)FindControl("Boton_" + objTabla.Id + "_mes");
            botonGuardar.Attributes.Add("class", "btn btn-dark");
            Boolean activarBoton = (DateTime.Now <= Convert.ToDateTime(objTabla.FechaLimiteRegistro) && !objTabla.MesActualValidado );
            botonGuardar.Visible = activarBoton;

            Button botonValidar = (Button)FindControl("BotonValida_" + objTabla.Id + "_mes");
            botonValidar.Attributes.Add("class", "btn btn-success");
            botonValidar.Attributes.Add("style", (validado ? "margin-left: 5px;" : "margin-left: 30px;"));
            if (validado)
            {
                botonValidar.Attributes.Add("disabled", "disabled");
            }
            String textoValida = validado ? "Validado" : "Validar información";
            botonValidar.Text = textoValida;

        }







        protected void Anio_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["anioConsulta"] = AnioConsulta.SelectedValue;
        }

        protected void ButtonDownload_Click(object sender, EventArgs e)
        {
            generarInformacionExcel();
        }
    }




    /*int i = 1000;
foreach (var foo in meses)
{
    Button b = new Button();
    b.ID = "button" + i.ToString();
    b.CommandName = "var_value";
    b.CommandArgument = foo;
    b.Command += ejemplo_Click;

    //add to panel p
    ContentPlaceHolder1.Controls.Add(b);

    i++;
}


cargarControlesBotones();*/
}