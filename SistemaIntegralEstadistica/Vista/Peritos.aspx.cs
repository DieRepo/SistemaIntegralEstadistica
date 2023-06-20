using MySql.Data.MySqlClient;
using SistemaIntegralEstadistica.Controlador;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SistemaIntegralEstadistica.Vista
{
    public partial class Peritos : System.Web.UI.Page
    {
        MySqlCommand cmd;
        MySqlDataReader r;
        String sql;
        Dictionary<string, string> d;
        Dictionary<string, ModeloTabla> t;

        int idRegistroEditar;
        int[] idTabla = { 2, 3 };
        String[] meses = { "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre" };
        int mesActivo = -1;

        int[] tablasAceso ;
        //int[] tablasAceso = { 7, 8 , 9  }; //4, 5, 6, 7,  8, 9, 10, 11, 12, 13

        List<ModeloTabla> tablas = new List<ModeloTabla>();
        List<Table> listaTablas;
        List<Button> listaBotones;
        Dictionary<String, ModeloTabla> tablasPagina;
        Dictionary<int, ModeloInicio> infoTabla;



        protected void btnSubmit_Click_p(object sender, EventArgs e)
        {
            /*lblMessage.Text = String.Format("Hi {0}, welcome!", txtName.Text);
            txtName.Visible = false;
            btnSubmitP.Visible = false;*/
        }

        protected override void OnInit(EventArgs e)
        {
            //CargarControles();
        }

        protected void CargarControles()
        {
            tablasPagina = new Dictionary<String, ModeloTabla>();
            listaTablas = new List<Table>();
            listaTablas.Add(Table4);
            listaTablas.Add(Table5);
            listaTablas.Add(Table6);
            listaTablas.Add(Table7);
            listaTablas.Add(Table8);
            listaTablas.Add(Table9);
            listaTablas.Add(Table10);
            listaTablas.Add(Table11);
            listaTablas.Add(Table12);
            listaTablas.Add(Table13);

            listaBotones = new List<Button>();

            generarTablas2();
        }

        private void setDatosTablas() {
            infoTabla = new Dictionary<int, ModeloInicio>();
            for (int i = 0; i < tablasAceso.Length; i++)
            {
                ModeloInicio inicio = new ModeloInicio();
                inicio.Tabla = listaTablas[i];
                inicio.Boton = listaBotones[i];
                infoTabla.Add(tablasAceso[i], inicio);
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            //Session["tablasAcceso"] = tablasAceso;

        }


        private void generarTablas()
        {
            Dictionary<int, ModeloTabla> tablas = new Dictionary<int, ModeloTabla>();
            tablasAceso = (int[])Session["verTabla"];
            //Obtener fecha actual
            DateTime fecha = DateTime.Now;
            int dia = fecha.Day;
            int mes = fecha.Month;

            for (int i = 0; i < listaTablas.Count; i++)
            {
                var tabla = ConsultarDatosTabla(tablasAceso[i]);
                tablas.Add(tablasAceso[i], tabla);
                generarTablaDinamica(listaTablas[i], tabla);
            }
        }





        public List<String[]> ConsultarCampos(int idTable)
        {
            List<String[]> filaColumnas = new List<String[]>();
            MySqlConnection con = null;
            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                sql = "SELECT group_concat( rowspan order by orden ASC  SEPARATOR '|') span , " +
                    "   group_concat( textorowspan order by orden ASC  SEPARATOR '|') textors , " +
                    "       group_concat( descripcion order by orden ASC  SEPARATOR '|') nombre, " +
                    "       group_concat(id ORDER BY orden ASC SEPARATOR '|')  campos " +
                    "   FROM tblfilas where idtabla =   " + idTable + "   " +
                    "UNION  " +
                    "   SELECT  group_concat( colspan order by orden ASC  SEPARATOR '|') span , ' - ' as textors, " +
                    "       group_concat(descripcion order by orden ASC  SEPARATOR '|') nombre, " +
                    "		group_concat(campo ORDER BY orden ASC SEPARATOR '|')  campo " +
                    "    FROM tblcolumnas where idtabla = " + idTable + "; ";
                cmd = new MySqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                while (resultado.Read())
                {
                    string[] dato = resultado["nombre"].ToString().Split('|');
                    filaColumnas.Add(dato);
                    string[] dato2 = resultado["campos"].ToString().Split('|');
                    filaColumnas.Add(dato2);
                    string[] dato3 = resultado["span"].ToString().Split('|');
                    filaColumnas.Add(dato3);
                    string[] dato4 = resultado["textors"].ToString().Split('|');
                    filaColumnas.Add(dato4);
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

            return filaColumnas;
        }

        public List<String[]> ConsultarDatos(int idTable)
        {
            List<String[]> filaColumnas = new List<String[]>();
            MySqlConnection con = null;
            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                sql = "SELECT group_concat( rowspan order by orden ASC  SEPARATOR '|') span , " +
                    "   group_concat( textorowspan order by orden ASC  SEPARATOR '|') textors , " +
                    "       group_concat( descripcion order by orden ASC  SEPARATOR '|') nombre, " +
                    "       group_concat(id ORDER BY orden ASC SEPARATOR '|')  campos " +
                    "   FROM tblfilas where idtabla =   " + idTable + "   " +
                    "UNION  " +
                    "   SELECT  group_concat( colspan order by orden ASC  SEPARATOR '|') span , ' - ' as textors, " +
                    "       group_concat(descripcion order by orden ASC  SEPARATOR '|') nombre, " +
                    "		group_concat(campo ORDER BY orden ASC SEPARATOR '|')  campo " +
                    "    FROM tblcolumnas where idtabla = " + idTable + "; ";
                cmd = new MySqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                while (resultado.Read())
                {
                    string[] dato = resultado["nombre"].ToString().Split('|');
                    filaColumnas.Add(dato);
                    string[] dato2 = resultado["campos"].ToString().Split('|');
                    filaColumnas.Add(dato2);
                    string[] dato3 = resultado["span"].ToString().Split('|');
                    filaColumnas.Add(dato3);
                    string[] dato4 = resultado["textors"].ToString().Split('|');
                    filaColumnas.Add(dato4);
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

            return filaColumnas;
        }

        public ModeloTabla ConsultarDatosTabla(int idTable)
        {
            List<String[]> filaColumnas = new List<String[]>();
            ModeloTabla tabla = new ModeloTabla();
            tabla.Id = idTable;

            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                sql = "(select 'columna' tipo, GROUP_CONCAT( if(campo <> '', campo, NULL) order by orden ASC  SEPARATOR '|') campos, " +
                    "    GROUP_CONCAT(if (campo <> '', descripcion, NULL) order by orden ASC  SEPARATOR '|') nombre, " +
                    "	GROUP_CONCAT(If(orden > -1, orden, NULL) order by orden ASC  SEPARATOR '|') orden," +
                    "	GROUP_CONCAT(colspan order by orden ASC  SEPARATOR '|') span," +
                    "	GROUP_CONCAT(textocolspan order by orden ASC  SEPARATOR '|') textoColspan ," +
                    "    GROUP_CONCAT( if (campo = '', descripcion, NULL) order by orden ASC  SEPARATOR '|') campoAdicional" +
                    "     FROM tblcolumnas WHERE idtabla = " + idTable + " and mostrar = 1 ) " +
                    "	union" +
                    "    (select 'fila' tipo, GROUP_CONCAT(id order by orden ASC  SEPARATOR '|')," +
                    "    GROUP_CONCAT(descripcion order by orden ASC  SEPARATOR '|') nombre," +
                    "    GROUP_CONCAT(orden order by orden ASC  SEPARATOR '|') orden," +
                    "    GROUP_CONCAT(rowspan order by orden ASC  SEPARATOR '|') span," +
                    "    GROUP_CONCAT(textorowspan order by orden ASC  SEPARATOR '|') textoColspan," +
                    "    GROUP_CONCAT( if (descripcion = '', descripcion, NULL) order by orden ASC  SEPARATOR '|') campoAdicional" +
                    "     from tblfilas WHERE idtabla = " + idTable + ") ; ";
                cmd = new MySqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                while (resultado.Read())
                {

                    if (resultado["tipo"].Equals("columna"))
                    {
                        tabla.NombresColumnas = resultado["campos"].ToString();
                        String[] nombresColumnas = resultado["nombre"].ToString().Split('|');
                        tabla.ColSpan = resultado["span"].ToString();
                        tabla.TextoColSpan = resultado["textoColSpan"].ToString();

                        String[] idColumnas = resultado["campos"].ToString().Split('|');
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
                                columna.NombreCampo = nombresColumnas[i].ToString();
                                filas.Add(columna);
                            }

                        }
                        tabla.Campos = filas;
                        Console.WriteLine("Termina columa");
                    }
                    else
                    {
                        tabla.IdFilas = resultado["campos"].ToString();
                        String[] idFilas = resultado["campos"].ToString().Split('|');
                        String[] nombresFilas = resultado["nombre"].ToString().Split('|');
                        List<ModeloFila> filas = new List<ModeloFila>();
                        if (idFilas.Length == nombresFilas.Length)
                        {
                            for (int i = 0; i < idFilas.Length; i++)
                            {
                                ModeloFila fila = new ModeloFila();
                                fila.IdFila = Convert.ToInt32(idFilas[i]);
                                fila.NombreFila = nombresFilas[i];
                                filas.Add(fila);
                            }

                        }
                        tabla.Filas = filas;
                        tabla.RowSpan = resultado["span"].ToString();
                        tabla.TextoRowSpan = resultado["span"].ToString();
                        Console.WriteLine("Termina fila");
                    }
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



        public List<string[]> obtenerTiposVisitas()
        {
            List<string[]> datos = new List<string[]>();
            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                sql = "SELECT CONCAT(' 1 , ', descripcion , ', a')  info FROM  tblfilas where idtabla = " + idTabla + "; ";
                cmd = new MySqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                while (resultado.Read())
                {
                    string[] dato = resultado["info"].ToString().Split(',');
                    datos.Add(dato);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Excepción : " + ex.ToString());
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
            return datos;
        }




        protected void generarTablaDinamica(Table nombreTabla, ModeloTabla tabla)
        {

            int numrows = tabla.Filas.Count;
            int numcells = tabla.Campos.Count;
            String[] spanFilas = tabla.RowSpan.Split('|');
            String[] textoSpanFilas = tabla.TextoRowSpan.Split('|');
            int temporalRowSpan = 1;
            int indiceFila = 0;
            int rowSpanNum = 0;
            int sumaRowSpan = 0;
            int continuo = 0, continuoClass = 0;
            int inicioCol = (!tabla.RowSpan.Equals("")) ? -2 : -1;

            Boolean banderaRowSpan;
            String textoRowSpanTable = "";

            Boolean deshabilitar = false;


            try
            {
                for (int j = 0; j <= tabla.Filas.Count; j++)
                {
                    TableRow tr_f = new TableRow();
                    string textoFila = j < tabla.Filas.Count ? tabla.Filas[j].NombreFila : "***";
                    banderaRowSpan = false;

                    rowSpanNum = (spanFilas.Length == 0 || (spanFilas.Length == 1 && spanFilas[0].Equals("")))
                                ? 1 : Convert.ToInt32(spanFilas[indiceFila]);

                    textoRowSpanTable = (spanFilas.Length == 0 || (spanFilas.Length == 1 && spanFilas[0].Equals("")))
                                ? "" : tabla.Filas[indiceFila].NombreFila; ;

                    if (j == numrows)
                    {
                        for (int k = inicioCol; k < numcells; k++)
                        {
                            TableCell celda = new TableCell();
                            if (k == mesActivo)
                            {
                                //TableCell celda = new TableCell();
                                Button botonGuardar = new Button();
                                botonGuardar.Text = "Guardar";
                                botonGuardar.CommandName = nombreTabla.ID + "_" + tabla.Campos[k].IdCampo;
                                //botonGuardar.CausesValidation = false;
                                botonGuardar.ID = nombreTabla.ID + "_" + tabla.Campos[k].IdCampo;
                                botonGuardar.Command += new CommandEventHandler(ejemplo_Click);

                                celda.Controls.Add(botonGuardar);
                            }
                            tr_f.Cells.Add(celda);
                        }
                        nombreTabla.Rows.Add(tr_f);
                    }
                    else
                    {
                        String idCampo = tabla.Filas[j].IdFila.ToString();

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
                                if ((temporalRowSpan == 1 || temporalRowSpan == (sumaRowSpan + 1)) && (rowSpanNum > 1))
                                {
                                    celda.RowSpan = rowSpanNum;
                                    banderaRowSpan = true;
                                    sumaRowSpan = rowSpanNum + sumaRowSpan;
                                    continuoClass++;
                                }
                            }
                            else if (i == -1)
                            {
                                Label lbl = new Label();
                                lbl.Text = textoFila;
                                celda.Controls.Add(lbl);
                                deshabilitar = !textoFila.Contains("TOTAL");
                            }
                            else
                            {
                                String idElemento = nombreTabla.ID + "_" + idCampo + "_" + tabla.Campos[i].IdCampo;
                                String nombreClase = nombreTabla.ID + "_" + tabla.Campos[i].IdCampo + "_" + continuoClass;
                                var classAdicional = !deshabilitar ? (nombreClase + "_Total") : nombreClase;

                                TextBox tb = new TextBox();
                                //tb.Enabled = deshabilitar;

                                if (deshabilitar)
                                {
                                    tb.ID = idElemento;
                                    tb.Attributes.Add("onkeyup", "sumarTotales( '" + nombreClase + "')");
                                    tb.Attributes.Add("class", "form-control " + nombreClase);

                                }
                                else
                                {
                                    //idElemento = nombreClase   + "_Total";
                                    tb.Attributes.Add("class", "form-control  classTotal " + nombreClase + "_Total");
                                    tb.Attributes.Add("readonly", "readonly");
                                    continuo++;
                                }
                                tb.ID = idElemento;
                                tb.Attributes.Add("Type", "number");
                                tb.Attributes.Add("min", "0");
                                tb.Attributes.Add("id", idElemento);
                                celda.Controls.Add(tb);

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
            catch (Exception ex)
            {
                String error = ex.ToString();
                Console.WriteLine("Error: " + error);
            }
            finally
            {
                //miPanel.Controls.Add(nombreTabla);
            }


        }

        protected void ejemplo_Click(object sender, CommandEventArgs e)
        {
            t = (Dictionary<String, ModeloTabla>)Session["infoTablas"];

            String nombre = e.CommandName;
            String[] subnombre = nombre.Split('_');
            Boolean esInformacionMensual = t[subnombre[0]].EsMensual;
            String campoMes = "", onDuplicateMes="", valorMes ="";

            String[] camposGuardar = t[subnombre[0]].CamposActivos.TrimEnd(',').Split(',');
            try
            {
                Console.WriteLine("Total CamposGuardar : " + camposGuardar.Length);
                String[] camposActivosGuardar = nombre.TrimEnd(',').Split(',');
                if (esInformacionMensual) {
                    campoMes = " mes , ";
                    onDuplicateMes = " mes = VALUES(mes) , ";
                    valorMes = t[subnombre[0]].MesActivo + " , ";
                }

                for (int j = 0; j  < camposGuardar.Length; j++)
                {
                    List<String> lista = new List<string>();

                    Console.WriteLine("Comando: " + subnombre[0] + subnombre[1]);
                    subnombre[1] = camposGuardar[j];
                    String valores = "", idFila = "";
                    String onDuplicate = onDuplicateMes + " idFila = VALUES(idFila) , " + subnombre[1] + " = VALUES(" + subnombre[1] + ") ";
                    String[] controles = HttpContext.Current.Request.Form.AllKeys;
                    String campos = " (anio,idFila,"+ campoMes + camposGuardar[j] + " )";
                    for (int i = 0; i < controles.Length; i++)
                    {
                        String[] fila = controles[i].Split('_');

                        idFila = (fila.Length > 2) ? fila[fila.Length - 2] : "";
                        //idFila = (fila.Length == 4) ? fila[fila.Length - 3] : idFila;

                        if (!idFila.Equals("") && controles[i].Contains(subnombre[0]) && controles[i].EndsWith(camposGuardar[j]))
                        {
                            String valor = HttpContext.Current.Request.Form[i];
                            valores += "  ( 2023 , " + idFila + " , " + valorMes + (valor.Equals("") ? "0" : valor) + ")  ,";
                            Console.WriteLine("Valor : " + valor);
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

            }
            catch (Exception exc)
            {

                Console.WriteLine("ERROR: " + exc.ToString());
            }
            finally {
                CargarControles();
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























        private void generarTablas2()
        {
            Dictionary<int, ModeloTabla> tablas = new Dictionary<int, ModeloTabla>();
            Dictionary<String, ModeloTabla> tablasPagina = new Dictionary<String, ModeloTabla>();

            //Obtener fecha actual
            DateTime fecha = DateTime.Now;
            int dia = fecha.Day;
            int mes = fecha.Month;

            for (int i = 0; i < listaTablas.Count; i++)
            {

                var tabla = ConsultarDatosTablaDin(tablasAceso[i]);
                tablasPagina.Add(listaTablas[i].ID.ToString(), tabla);

                //tablas.Add(listaTablas[i]], tabla);
                generarTablaDinamica0(listaTablas[i], tabla);

            }
            Session["infoTablas"] = tablasPagina;
        }


        private Table agregarHeaderTabla(Table tabla, ModeloTabla objTabla)
        {
            String[] adicional1 = { "1", "1" };
            String[] adicional2 = { "1" };
            int inicioCol = (!objTabla.ColSpan.Equals("")) ? -2 : -1;
            String[] columnaAdicional = objTabla.ColAdicional.Split('|');

            String[] csHeader = objTabla.ColAdicional.Split('|').Length > 1 ? adicional1 : adicional2;

            String[] spanColumnas = objTabla.ColSpan.Split('|');
            csHeader.Concat(spanColumnas).ToArray();

            String[] textoSpanColumnas = objTabla.TextoColSpan.Split('|');

            int totalFilasHeader = inicioCol == -2 ? 2 : 1;

            int totalColumnas = (totalFilasHeader) + objTabla.Campos.Count();


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
                            celda.Controls.Add(lbl);
                            celda.ColumnSpan = Convert.ToInt32(colSpanHead);
                            tr_f.Cells.Add(celda);
                        }
                    }
                    else
                    {
                        for (int k = 0; k < (objTabla.Campos.Count + csHeader.Length); k++)
                        {
                            String textoLabel = (k < csHeader.Length) ? columnaAdicional[k] : objTabla.Campos[k - csHeader.Length].NombreCampo;
                            TableCell celda = new TableCell();
                            Label lbl = new Label();
                            lbl.Text = textoLabel;
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




        protected void generarTablaDinamica0(Table nombreTabla, ModeloTabla tabla)
        {
            Console.WriteLine("GENERAR TABLA");
            //OBTENER NUMERO DE FILAS Y COLUMNAS DEL CUERPO DE LA TABLA
            int numrows = tabla.Filas.Count;
            int numcells = tabla.Campos.Count;

            //OBTENER INFORMACIÓN PARA FILAS Y COLUMNAS
            String[] numcolAdicional = tabla.ColAdicional.Split('|');
            String[] spanFilas = tabla.RowSpan.Split('|');
            String[] textoSpanFilas = tabla.TextoRowSpan.Split('|');

            int temporalRowSpan = 1;
            int indiceFila = 0;
            int rowSpanNum = 0;
            int sumaRowSpan = 0;
            int continuo = 0, continuoClass = 0;
            int inicioCol = (!tabla.RowSpan.Equals("")) ? -2 : -1;

            Boolean banderaRowSpan;
            String textoRowSpanTable = "";
            Boolean deshabilitar = false;
            String keyObject = "";
            String idCampo = "";

            Dictionary<String, String> datos = obtenerDatosAlmacenados(tabla);
            nombreTabla = agregarHeaderTabla(nombreTabla, tabla); //Agregar informacipon de los headers

            try
            {
                for (int j = 0; j <= tabla.Filas.Count; j++)
                {
                    TableRow tr_f = new TableRow();
                    string textoFila = j < tabla.Filas.Count ? tabla.Filas[j].NombreFila : "***";
                    banderaRowSpan = false;

                    Boolean banderaSpan = false;
                    if (!spanFilas[0].Equals(""))
                    {
                        banderaSpan = Convert.ToInt32(spanFilas[0]) > 1 && j != numrows;
                    }

                    rowSpanNum = banderaSpan ? Convert.ToInt32(spanFilas[indiceFila]) : 1;
                    textoRowSpanTable = banderaSpan ? textoSpanFilas[indiceFila] : "";

                    if (j == numrows)
                    {
                        for (int k = inicioCol; k < numcells; k++)
                        {
                            TableCell celda = new TableCell();
                            if (k == mesActivo)
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
                        nombreTabla.Rows.Add(tr_f);
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
                                if ((temporalRowSpan == 1 || temporalRowSpan == (sumaRowSpan + 1)) && (rowSpanNum > 1))
                                {
                                    celda.RowSpan = rowSpanNum;
                                    banderaRowSpan = true;
                                    sumaRowSpan = rowSpanNum + sumaRowSpan;
                                    continuoClass++;
                                }
                            }
                            else if (i == -1)
                            {
                                Label lbl = new Label();
                                lbl.Text = textoFila;
                                celda.Controls.Add(lbl);
                                deshabilitar = !textoFila.Contains("TOTAL");
                            }
                            else
                            {
                                if (i > 11)
                                {
                                    Console.WriteLine("Columna 13");
                                }
                                Console.WriteLine("Agregar información");
                                keyObject = idCampo + "_" + tabla.Campos[i].IdCampo;
                                String valor = "";
                                String valorBD = (datos.TryGetValue(keyObject , out valor) ? datos[keyObject] : "0"); 

                                Boolean columnaTotal = tabla.Campos[i].NombreCampo.Contains("Total");

                                String claseFila = !tabla.Campos[i].NombreClass.Trim().Equals("") ? idCampo + "_" + tabla.Campos[i].NombreClass : "";


                                String idElemento = nombreTabla.ID + "_" + idCampo + "_" + tabla.Campos[i].IdCampo;
                                String nombreClase = nombreTabla.ID + "_" + tabla.Campos[i].IdCampo + "_" + continuoClass;
                                var classAdicional = !deshabilitar ? (nombreClase + "_Total") : nombreClase;
                                var classAdicionalCol = tabla.Campos[i].ClassTotal.Trim().Equals("") ? " " : idCampo + "_" + tabla.Campos[i].ClassTotal + "_Total";

                                TextBox tb = new TextBox();
                                //tb.Enabled = deshabilitar;

                                if (deshabilitar && !columnaTotal)
                                {
                                    String funcion2 = !claseFila.Equals("") ? "; sumarTotales('" + claseFila + "'); " : "";
                                    tb.ID = idElemento;
                                    tb.Attributes.Add("class", "form-control " + nombreClase + "  " + claseFila + " " + classAdicionalCol);
                                    if (!classAdicionalCol.Equals(" "))
                                    {
                                        //tb.Attributes.Add("readonly", "readonly");
                                        tb.Attributes.Add("onchange", "sumarTotales( '" + nombreClase + "')" + funcion2);

                                    }
                                    else
                                    {
                                        tb.Attributes.Add("onkeyup", "sumarTotales( '" + nombreClase + "')" + funcion2);
                                    }
                                }
                                else
                                {
                                    //idElemento = nombreClase   + "_Total";
                                    tb.Attributes.Add("class", "form-control  " + classAdicional + "   " + classAdicionalCol);
                                    tb.Attributes.Add("readonly", "readonly");
                                    continuo++;
                                }
                                tb.ID = idElemento;
                                tb.Attributes.Add("Type", "number");
                                tb.Attributes.Add("min", "0");
                                tb.Attributes.Add("id", idElemento);
                                tb.Text = valorBD;

                                // Crear Label por si no es del mes activo
                                Label labelTex = new Label();
                                labelTex.ID = idElemento;
                                labelTex.Text = valorBD;

                                if (tabla.Campos[i].EsActivo || tabla.Campos[i].MesCampo.Equals("-1"))
                                {
                                    celda.Controls.Add(tb);
                                }
                                else {
                                    celda.Controls.Add(labelTex);
                                }


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
            catch (Exception ex)
            {
                String error = ex.ToString();
                Console.WriteLine("Error: " + error +"  key: "+keyObject);
            }
            finally
            {
                //miPanel.Controls.Add(nombreTabla);
            }


        }

        private Dictionary<string, string> obtenerDatosAlmacenados(ModeloTabla tabla)
        {
            String[] camposIniciales = { "anio", "idFila", "mes" };
            String[] camposAdicionales = tabla.NombresColumnas.Split('|');
            String idsColumnas = tabla.NombresColumnas.Replace("|",",");
            String idsFilas = tabla.IdFilas.Replace("|",",");
            Console.WriteLine("Total campos " + camposAdicionales.Length);
            Dictionary<string, string> info = new Dictionary<String, String>();
            MySqlConnection con = null;
            String key = "";

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                sql = "SELECT  anio , idFila , " + idsColumnas  + " FROM " + tabla.Tabla + " WHERE idFila IN (" + idsFilas + ")";
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
                        info.Add( key , resultado[campo].ToString() );
                    }

                }
                con.Close();


            }
            catch (Exception error)
            {
                Console.WriteLine("Key : "+key);
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


        public ModeloTabla ConsultarDatosTablaDin(int idTable)
        {
            List<String[]> filaColumnas = new List<String[]>();
            Boolean esInformacionMensual = false;
            String camposActivos = "";
            ModeloTabla tabla = new ModeloTabla();
            tabla.Id = idTable;
            tabla.EsMensual = false;

            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                sql = "SELECT id, tabla, nombreTabla, mesActivo,  fechaActivo , t2.* FROM tbltabla t1" +
                "    LEFT JOIN (select idtabla, 'columna' tipo, GROUP_CONCAT( if (campo <> '', campo, NULL) order by orden ASC  SEPARATOR '|') campos,  " +
                "	GROUP_CONCAT(if (campo <> '', descripcion, NULL) order by orden ASC  SEPARATOR '|') nombre, " +
                " GROUP_CONCAT(If(orden > -1, orden, NULL) order by orden ASC  SEPARATOR '|') orden," +
                "	GROUP_CONCAT(colspan order by orden ASC  SEPARATOR '|') span," +
                "	GROUP_CONCAT(textocolspan order by orden ASC  SEPARATOR '|') textoSpan  ," +
                "    GROUP_CONCAT( if (campo = '', descripcion, NULL) order by orden ASC  SEPARATOR '|') campoAdicional," +
                "    GROUP_CONCAT( if (classSuma is not null, classSuma, NULL) order by orden ASC  SEPARATOR '|') sumaColumnas," +
                "    GROUP_CONCAT( if (classTotal is not null, classTotal, NULL) order by orden ASC  SEPARATOR '|') sumaTotales," +
                "       GROUP_CONCAT(if(campo <> '', mes, NULL)order by orden ASC  SEPARATOR '|') mes" +
                "     FROM tblcolumnas WHERE      idtabla = " + idTable + " and mostrar = 1" +
                "    union" +
                "    select idtabla, 'fila' tipo, GROUP_CONCAT(id order by orden ASC  SEPARATOR '|'), " +
                "	GROUP_CONCAT(descripcion order by orden ASC  SEPARATOR '|') nombre, " +
                "	GROUP_CONCAT(orden order by orden ASC  SEPARATOR '|') orden," +
                "	GROUP_CONCAT(rowspan order by orden ASC  SEPARATOR '|') span," +
                "	GROUP_CONCAT(textorowspan order by orden ASC  SEPARATOR '|') textoSpan," +
                "    GROUP_CONCAT( if (descripcion = '', descripcion, NULL) order by orden ASC  SEPARATOR '|') campoAdicional," +
                "   '' as sumaColumnas," +
                "   '' as sumaTotal, '' mes " +
                "    from tblfilas WHERE idtabla = " + idTable + ") t2 ON t1.id = t2.idtabla" +
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
                        String[] nombresColumnas = resultado["nombre"].ToString().Split('|');
                        tabla.ColSpan = resultado["span"].ToString();
                        tabla.TextoColSpan = resultado["textoSpan"].ToString();
                        tabla.ColAdicional = resultado["campoAdicional"].ToString();
                        String[] idColumnas = resultado["campos"].ToString().Split('|');
                        String[] classTotales = resultado["sumaTotales"].ToString().Split('|');
                        String[] clases = resultado["sumaColumnas"].ToString().Split('|');
                        String[] meses = resultado["mes"].ToString().Split('|');
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
                                    columna.EsActivo = tabla.MesActivo.Equals(meses[i]);
                                }
                                if (nombresColumnas.Length == clases.Length)
                                {
                                    columna.NombreClass = clases[i].ToString();

                                }
                                esInformacionMensual = columna.MesCampo.Equals("-1");
                                camposActivos = camposActivos + (tabla.MesActivo.Equals(meses[i]) || columna.MesCampo.Equals("-1") ? columna.IdCampo + "," : "");

                                columna.ClassTotal = classTotales[i].ToString();
                                filas.Add(columna);
                            }

                        }
                        tabla.Campos = filas;
                        tabla.EsMensual = esInformacionMensual;
                        Console.WriteLine("Termina columa");
                    }
                    else
                    {
                        tabla.IdFilas = resultado["campos"].ToString();
                        String[] idFilas = resultado["campos"].ToString().Split('|');
                        String[] nombresFilas = resultado["nombre"].ToString().Split('|');
                        List<ModeloFila> filas = new List<ModeloFila>();
                        if (idFilas.Length == nombresFilas.Length)
                        {
                            for (int i = 0; i < idFilas.Length; i++)
                            {
                                ModeloFila fila = new ModeloFila();
                                fila.IdFila = Convert.ToInt32(idFilas[i]);
                                fila.NombreFila = nombresFilas[i];
                                filas.Add(fila);
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



        protected void ejemplo2_Click(object sender, EventArgs e)
        {
            List<String> lista = new List<string>();
            String cadena = sender.ToString();
            try
            {
                Console.WriteLine("FUNCTION -> "+e.ToString());
            }
            catch (Exception exc)
            {

                Console.WriteLine("ERROR: " + exc.ToString());
            }

        }



    }



   


}