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
    public partial class SeguimientoAcuerdos : System.Web.UI.Page
    {

        MySqlCommand cmd;
        MySqlDataReader r;
        String sql;
        Dictionary<string, string> d;
        int idRegistroEditar;
        int[] idTabla = { 2, 3 };
        String[] meses = { "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre" };
        int mesActivo = -1;

        List<ModeloTabla> tablas = new List<ModeloTabla>();

        protected void Page_Load(object sender, EventArgs e)
        {
            generarTablas();
            if (!this.IsPostBack) { 
        

            }

        }

        public void generarTablas()
        {
            //Obtener fecha actual
            DateTime fecha = DateTime.Now;
            int dia = fecha.Day;
            int mes = fecha.Month;

            if (mes > 1 && mes <= 12)
            {
                mesActivo = (mes - 2);

            }
            else
            {
                mesActivo = 11;
            }

            List<String[]> lista = ConsultarDatos(2);
            List<String[]> lista2 = ConsultarDatos(3);

            if (lista2.Count == 8)
            {
                generarTablaDinamica(Tabla1, lista);
                generarTablaDinamica(Tabla2, lista2);

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




        protected void generarTablaDinamica(Table nombreTabla, List<String[]> filasColumnas)
        {
            String[] nombreFilas = filasColumnas[0];
            String[] camposFilas = filasColumnas[1];
            String[] spanFilas = filasColumnas[2];
            String[] textoFilas = filasColumnas[3];

            String[] nombreColumnas = filasColumnas[4];
            String[] camposColumnas = filasColumnas[5];
            String[] spanColumnas = filasColumnas[6];
            String[] textoColumnas = filasColumnas[7];
            //int[] valores = obtenerFilasColumnas();
            //List<string[]> lista = nombreColumnas;
            int numrows = nombreFilas.Length;
            int numcells = nombreColumnas.Length;
            int temporalRowSpan = 1;
            int indiceFila = 0;
            int rowSpanNum = 0;
            int sumaRowSpan = 0;
            int continuo = 0;
            Boolean banderaRowSpan;
            String textoRowSpanTable = "";

            Boolean deshabilitar = false;


            try
            {
                for (int j = 0; j <= numrows; j++)
                {
                    TableRow tr_f = new TableRow();
                    string textoFila = nombreFilas[j];
                    banderaRowSpan = false;

                    rowSpanNum = (spanFilas.Length == 0 || (spanFilas.Length == 1 && spanFilas[0].Equals("")))
                                ? 1 : Convert.ToInt32(spanFilas[indiceFila]);

                    textoRowSpanTable = (spanFilas.Length == 0 || (spanFilas.Length == 1 && spanFilas[0].Equals("")))
                                ? "" : textoFilas[indiceFila];
                    String idCampo = camposFilas[j];

                    if (j == (numrows - 1))
                    {
                        for (int k = -2; k < numcells; k++)
                        {
                            TableCell celda = new TableCell();
                            if (k == mesActivo)
                            {
                                //TableCell celda = new TableCell();
                                Button botonGuardar = new Button();
                                botonGuardar.CommandName = nombreTabla.ID + "_" + camposColumnas[k];
                                //botonGuardar.CausesValidation = false;
                                botonGuardar.ID = nombreTabla.ID + "_" + camposColumnas[k];
                                botonGuardar.Command += new CommandEventHandler(ejemplo_Click);

                                celda.Controls.Add(botonGuardar);
                            }
                            tr_f.Cells.Add(celda);
                        }
                        nombreTabla.Rows.Add(tr_f);
                    }
                    else
                    {
                        for (int i = -2; i < numcells; i++)
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
                                String idElemento = nombreTabla.ID + "_" + idCampo + "_" + camposColumnas[i];
                                String nombreClase = nombreTabla.ID + "_" + camposColumnas[i] + "_" +continuo;
                                var classAdicional = !deshabilitar ? (nombreClase + "_Total") : nombreClase;

                                TextBox tb = new TextBox();
                              
                                //tb.Enabled = deshabilitar;

                                if (deshabilitar)
                                {
                                    tb.ID = idElemento;
                                    tb.Attributes.Add("onkeyup", "sumarTotales( '" + nombreClase + "')");
                                    tb.Attributes.Add("class", "form-control " + nombreClase);

                                }
                                else {
                                    idElemento = nombreClase + "_" + continuo + "_Total";
                                    tb.Attributes.Add("class", "form-control  classTotal");

                                    continuo++;
                                }
                                tb.ID = idElemento;
                                tb.Attributes.Add("Type", "number");
                                tb.Attributes.Add("min", "0");
                                tb.Attributes.Add("id", idElemento);
                                //tb.Attributes.Add("disabled", Convert.ToString(deshabilitar));
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
            }


        }

        protected void ejemplo_Click(object sender, CommandEventArgs e)
        {
            List<String> lista = new List<string>();
            try
            {
                String nombre = e.CommandName;
                String[] subnombre = nombre.Split('_');
                Console.WriteLine("Comando: " + subnombre[0] + subnombre[1]);
                String valores = "", idFila = "";
                String onDuplicate = " idFila = VALUES(idFila) , " + subnombre[1] + " = VALUES(" + subnombre[1] + ") ";
                String[] controles = HttpContext.Current.Request.Form.AllKeys;
                String campos = " (anio,idFila," + subnombre[1] + " )";
                for (int i = 0; i < controles.Length; i++)
                {
                    String[] fila = controles[i].Split('_');

                    idFila = (fila.Length > 2) ? fila[fila.Length - 2] : "";

                    if (!idFila.Equals("") && controles[i].Contains(subnombre[0]) && controles[i].EndsWith(subnombre[1]))
                    {
                        String valor = HttpContext.Current.Request.Form[i];
                        valores += "  ( 2023 , " + idFila + " , " + (valor.Equals("") ? "0" : valor) + ")  ,";
                        Console.WriteLine("Valor : " + valor);
                    }

                }
                valores = valores.TrimEnd(',');
                Console.WriteLine("VALORES: " + valores);
                lista.Add(campos);
                lista.Add(valores);
                lista.Add(onDuplicate);
                insertarInformacion(lista);
            }
            catch (Exception exc)
            {

                Console.WriteLine("ERROR: " + exc.ToString());
            }

        }


        private long insertarInformacion(List<String> datos)
        {
            long id = 0;
            MySqlConnection con = null;
            d = (Dictionary<String, String>)Session["userpass"];
            String query = "";
            try
            {
                query = "INSERT into tblacuerdos "
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
    }
}