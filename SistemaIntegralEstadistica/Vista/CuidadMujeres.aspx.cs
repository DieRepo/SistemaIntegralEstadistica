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
    public partial class CuidadMujeres : System.Web.UI.Page
    {
        MySqlCommand cmd;
        MySqlDataReader r;
        String sql;
        string pattern = "yyyy-MM-dd";

        DateTime fechaActual;
        String fechaDiaActual;
        String fechaPeriodoInicio;
        String fechaPeriodoFin;
        String idRegistro = "0";
        protected void Page_Load(object sender, EventArgs e)
        {
            exampleModalCenterTitle.Text = "Registro";

            /*botonGuardar.Attributes.Add("data-dismiss", "modal");
            botonGuardar.Attributes.Add("data-backdrop", "true");*/

            //Session["idRegistro"] = "0";

            if (!this.IsPostBack)
            {
                consultarCatalogo();
                consultarRegistrosPorDia();
                obtenerDatosAlmacenadosProcedure("GetTotalServiciosUIDH", 1, "tablaUT");
                obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 2, "tablaAtencionCiudadana");
                obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 2, "listaAtencion");
            }
            Console.WriteLine("Termina page_load");
            getInformacionCoordinacionParentalidad();
        }


        protected void BuscarInfo_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Buscar_información");
        }

        protected void listaDatos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            listaDatos.PageIndex = e.NewPageIndex;
            consultarRegistrosPorDia();
        }

        protected void listaAtencion_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            listaAtencion.PageIndex = e.NewPageIndex;
            obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 3 , "listaAtencion");

        }


        public void consultarCatalogo()
        {
            List<ModeloCatalogo> listaTexto = new List<ModeloCatalogo>(); ;
            String consulta = "SELECT id , descripcion FROM  tblcatuidhservicio where activo =1 ; ";
            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(consulta, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                while (resultado.Read())
                {
                    ModeloCatalogo obj = new ModeloCatalogo();
                    obj.Id = resultado.GetInt32("id");
                    obj.Descripcion = resultado.GetString("descripcion");
                    listaTexto.Add(obj);
                }
                con.Close();

                listaTipo.DataSource = listaTexto;
                listaTipo.DataValueField = "id";
                listaTipo.DataTextField = "descripcion";
                listaTipo.DataBind();
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

        }


        public void consultarRegistrosPorDia()
        {
            List<ModeloCatalogo> listaTexto = new List<ModeloCatalogo>(); ;
            String consulta = "SELECT a.id ID , fecha , a.idTipo , b.descripcion , a.numeroHombres , a.numeroMujeres , a.total FROM tblciudaduidhservicios a" +
                "    LEFT JOIN tblcatuidhservicio b" +
                "    ON a.idTipo = b.id" +
                "    where a.activo = 1 order by  1 desc ; ";
            MySqlConnection con = null;

            try
            {
                DataTable dt = new DataTable();
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(consulta, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                dt.Load(resultado);
                listaDatos.DataSource = dt;
                listaDatos.DataBind();
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

        }





        protected void fechaRegistro_TextChanged(object sender, EventArgs e)
        {

        }

        protected void Guardar_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Guadar información ");
            int idReg = Convert.ToInt32( Session["idRegistro"] );
            try
            {
                if (idReg == 0)
                {
                    Console.WriteLine("AGREGAR REGISTRO ");
                    insertarInformacion();
                }
                else
                {
                    actualizarInformacion(idReg);
                }
                
                Session["idRegistro"] = "0";
            }
            catch (Exception ex)
            {

                Console.WriteLine("ERROR: " + ex.ToString());
            }
            Console.WriteLine("termina guardar ");
            inicializarDatos();
            cerrarDialog("myModal");
        }

       


        private void insertarInformacion()
        {
            long id = 0;
            MySqlConnection con = null;
            int tipoOrientacion = Convert.ToInt32(listaTipo.SelectedValue);

            String query = "";
            try
            {
                query = " INSERT into  tblciudaduidhservicios(fecha,idTipo,numeroHombres,numeroMujeres,total) VALUES ('" +
                   fechaRegistro.Text + "' ," + tipoOrientacion + " , " + totalHombres.Text + " , " + totalMujeres.Text + ", " + (Convert.ToInt32(totalHombres.Text) + Convert.ToInt32(totalMujeres.Text)) +
                   ")   ON DUPLICATE KEY UPDATE numeroHombres = VALUES(numeroHombres) , numeroMujeres = VALUES(numeroMujeres) , total = VALUES(total) ";

                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(query, con);
                cmd.ExecuteNonQuery();

                //id = cmd.LastInsertedId;
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
                consultarRegistrosPorDia();
                limpiarFormularios();
                //exampleModalCenter.Visible = false;
            }

            //return id;
        }



        private void actualizarInformacion(int id)
        {
            MySqlConnection con = null;
            int tipoOrientacion = Convert.ToInt32(listaTipo.SelectedValue);

            String query = "";
            try
            {
                query = " UPDATE  tblciudaduidhservicios SET idTipo = " + tipoOrientacion + ",  numeroHombres = " + totalHombres.Text
                    + ",  numeroMujeres = " + totalMujeres.Text + ",  total = " + (Convert.ToInt32(totalHombres.Text) + Convert.ToInt32(totalMujeres.Text)) + "  WHERE  id = " + id;

                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(query, con);
                cmd.ExecuteNonQuery();
                //id = cmd.LastInsertedId;
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
                consultarRegistrosPorDia();
                limpiarFormularios();
                //exampleModalCenter.Visible = false;
            }

        }


            public void limpiarFormularios()
        {
            fechaRegistro.Text = "";
            totalHombres.Text = "";
            totalMujeres.Text = "";

        }

        public void obtenerFechasPeriodoInformacion()
        {
            fechaActual = DateTime.Now;
            fechaDiaActual = fechaActual.ToString(pattern);
            fechaPeriodoInicio = periodoInicio.Text.Equals("") ? fechaDiaActual : periodoInicio.Text;
            fechaPeriodoFin = periodoFin.Text.Equals("") ? fechaDiaActual : periodoFin.Text;
        }


        private void obtenerDatosAlmacenadosProcedure(String nombreProcedure, int id, String nombreTabla)
        {
            MySqlConnection con = null;
            String parameter = "a";
            this.obtenerFechasPeriodoInformacion();
            DataTable dt = new DataTable("Tabla_" + nombreProcedure + "_" + id);
            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(nombreProcedure, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd = agregaParametros(cmd, nombreProcedure, id , 0, 0) ;
                MySqlDataReader resultado = cmd.ExecuteReader();

                dt.Load(resultado);
                GridView table = (GridView)panelCiudadMujeres.FindControl(nombreTabla);
                table.DataSource = dt;
                table.DataBind();
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
        }


        private List<int> obtenerDatosAlmacenadosProcedureLista(String nombreProcedure, int id, int anio, int region)
        {
            MySqlConnection con = null;
            String parameter = "a";
            this.obtenerFechasPeriodoInformacion();
            List<int> lista = new List<int>();
            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(nombreProcedure, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd = agregaParametros(cmd, nombreProcedure, id , anio, region);
                MySqlDataReader resultado = cmd.ExecuteReader();

                while (resultado.Read()) {
                    lista.Add(resultado.GetInt32("total"));
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
            return lista;
        }

        public MySqlCommand agregaParametros(MySqlCommand command, String nombre, int id , int anio, int region)
        {

            if (nombre.Equals("GetTotalServiciosUIDH"))
            {
                cmd.Parameters.AddWithValue("tipo", id);
                cmd.Parameters.AddWithValue("param", id);
                cmd.Parameters.AddWithValue("fechaInicio", fechaPeriodoInicio);
                cmd.Parameters.AddWithValue("fechaFin", fechaPeriodoFin);
            }
            else if (nombre.Equals("obtenInformacionCiudadMujeres"))
            {
                cmd.Parameters.AddWithValue("_id", id);
                cmd.Parameters.AddWithValue("_fechaIni", fechaPeriodoInicio);
                cmd.Parameters.AddWithValue("_fechaFin", fechaPeriodoFin);
            }
            else if (nombre.Equals("obtenInformacionCiudadMujeresParentalidad"))
            {
                cmd.Parameters.AddWithValue("_id", id);
                cmd.Parameters.AddWithValue("_anio", anio);
                cmd.Parameters.AddWithValue("_region", region);
            }


            return command;
        }

        protected void listaDatos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].Text = Convert.ToDateTime(e.Row.Cells[1].Text).ToString(pattern);
            }
        }



        protected void listaAtencion_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].Text = Convert.ToDateTime(e.Row.Cells[1].Text).ToString(pattern);
            }

        }




        protected void listaDatos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            MySqlConnection con = null;
            GridViewRow row = (GridViewRow)listaDatos.Rows[e.RowIndex];
            Label lbldeleteid = (Label)row.FindControl("lblID");

            try
            {
                String id = listaDatos.DataKeys[e.RowIndex].Value.ToString();
                //String ids0 = listaDatos.DataKeys[e.RowIndex].Values[0].ToString();
                //String ids1 = listaDatos.DataKeys[e.RowIndex].Values[1].ToString();

                String sql = "UPDATE tblciudaduidhservicios SET activo = 0 where id = " + id + "; ";
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(sql, con);
                cmd.ExecuteNonQuery();

                //id = cmd.LastInsertedId;
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "Correcto insertar", "mensaje('exitoso','registro exitoso','success');", true);
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
                obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 2, "listaAtencion");

            }

            Console.WriteLine("Termina eliminacion");
        }


        protected void listaAtencion_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            MySqlConnection con = null;
            GridViewRow row = (GridViewRow)listaDatos.Rows[e.RowIndex];
            Label lbldeleteid = (Label)row.FindControl("lblID");

            try
            {
                String id = listaAtencion.DataKeys[e.RowIndex].Value.ToString();

                String sql = "UPDATE tblciudadatencion SET activo = 0 where id = " + id + "; ";
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(sql, con);
                cmd.ExecuteNonQuery();

                //id = cmd.LastInsertedId;
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "Correcto insertar", "mensaje('exitoso','registro exitoso','success');", true);
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
                obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 2, "listaAtencion");

            }

            Console.WriteLine("Termina eliminacion");
        }

        protected void listaDatos_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            MySqlConnection con = null;
            GridViewRow row = listaDatos.Rows[e.RowIndex];
            try
            {
                String id = listaDatos.DataKeys[e.RowIndex].Value.ToString();
                String sql = "UPDATE tbciudaduidhservicios SET numeroHombres=" + totalHombres.Text + " , numeroMujeres= " + totalMujeres.Text + " where id = " + id + "; ";
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(sql, con);
                cmd.ExecuteNonQuery();

                //id = cmd.LastInsertedId;
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "Correcto insertar", "mensaje('exitoso','registro exitoso','success');", true);
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
                consultarRegistrosPorDia();
            }

        }

        protected void listaDatos_RowEditing(object sender, GridViewEditEventArgs e)
        {
            /*listaDatos.EditIndex = e.NewEditIndex;
            consultarRegistrosPorDia();*/

            //ClientScript.RegisterStartupScript(this.GetType(), "Pop", "openModal();", true);


        }

        protected void listaDatos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            String command = e.CommandName.ToString();

            try
            {
                if (command.ToLower().Contains("editar"))
                {
                    exampleModalCenterTitle.Text = "Editar Registro";
                    int rowIndex = Convert.ToInt32(e.CommandArgument);
                    GridViewRow row = listaDatos.Rows[rowIndex];

                    idRegistro = listaDatos.Rows[rowIndex].Cells[0].Text;
                    fechaRegistro.Text = listaDatos.Rows[rowIndex].Cells[1].Text;
                    listaTipo.SelectedValue = listaDatos.Rows[rowIndex].Cells[2].Text;
                    totalHombres.Text = listaDatos.Rows[rowIndex].Cells[4].Text;
                    totalMujeres.Text = listaDatos.Rows[rowIndex].Cells[5].Text;
                    Session["idRegistro"] = idRegistro;
                    abrirDialog("myModal");
                    /*UpdatePanel up = (UpdatePanel)Master.FindControl("UpdatePanelCM");
                    up.Update();
                    ClientScript.RegisterStartupScript(this.GetType(), "Pop", "openModal();", true);
                    botonEditar.Visible = true;
                    botonGuardar.Visible = false;*/

                    //ClientScript.RegisterStartupScript(this.GetType(), "Pop", "openModal();", true);

                }
                else {
                    Session["idRegistro"] = "0";
                    inicializarDatos();
                }
                Console.WriteLine("Terminarow command");
                //exampleModalCenter.Visible = true;
                //ClientScript.RegisterStartupScript(this.GetType(), "Pop", "openModal();", true);
                //ClientScript.RegisterStartupScript(this.GetType(), "Popup", "$('#exampleModalCenter').modal('show')", true);
                //exampleModalCenter.Visible = true;
               
            }
            catch (Exception)
            {

                Console.WriteLine("ERROR AL EDIATR");
            }
            finally {
                
            }


        }



        protected void listaAtencion_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            String command = e.CommandName.ToString();
            try
            {
                if (command.ToLower().Contains("editar"))
                {
                    exampleModalCenterTitle.Text = "Editar Registro";
                    int rowIndex = Convert.ToInt32(e.CommandArgument);
                    GridViewRow row = listaAtencion.Rows[rowIndex];

                    idRegistro = listaAtencion.Rows[rowIndex].Cells[0].Text;
                    fechaAC.Text = listaAtencion.Rows[rowIndex].Cells[1].Text;
                    expedienteAC.Text = listaAtencion.Rows[rowIndex].Cells[2].Text;
                    nombrePersona.Text = listaAtencion.Rows[rowIndex].Cells[3].Text;
                    primerApellido.Text = listaAtencion.Rows[rowIndex].Cells[4].Text;
                    segundoApellido.Text = listaAtencion.Rows[rowIndex].Cells[5].Text;
                    Session["idRegistro"] = idRegistro;

                    abrirDialog("myModalAyC");
                }
                else
                {
                    Session["idRegistro"] = "0";
                    inicializarDatos();
                }
                Console.WriteLine("Terminarow command");

            }
            catch (Exception)
            {

                Console.WriteLine("ERROR AL EDIATR");
            }
            finally
            {

            }


        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Mostrar información ");


        }

        protected void BuscarInfo_Click1(object sender, EventArgs e)
        {
            Console.WriteLine("Empieza búsqueda");
            obtenerDatosAlmacenadosProcedure("GetTotalServiciosUIDH", 1 , "tablaUT");
            Console.WriteLine("Buscar");
        }

        protected void ButtonAC_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Guadar información ");
            int idReg = Convert.ToInt32(Session["idRegistro"]);
            try
            {
                if (idReg == 0)
                {
                    Console.WriteLine("AGREGAR REGISTRO ");
                    insertarInformacionAC();
                }
                else
                {
                    actualizarInformacionAC(idReg);
                }

                Session["idRegistro"] = "0";
            }
            catch (Exception ex)
            {

                Console.WriteLine("ERROR: " + ex.ToString());
            }
            Console.WriteLine("termina guardar ");
            //inicializarDatos();
            cerrarDialog("myModalAyC");
        }

        

        private void actualizarInformacionAC(int idReg)
        {
            MySqlConnection con = null;
            int tipoOrientacion = Convert.ToInt32(listaTipo.SelectedValue);

            String query = "";
            try
            {
                query = " UPDATE  tblciudadatencion SET fecha = '" + fechaAC.Text + "' ,  expediente = '" + expedienteAC.Text
                    + "',  nombre = '" + nombrePersona.Text + "',  primerApellido = '" + primerApellido.Text+"' ,  segundoApellido = '" + segundoApellido.Text + "'  WHERE  id = " + idReg;

                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(query, con);
                cmd.ExecuteNonQuery();
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

                limpiarFormularios();
                obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 2, "listaAtencion");
                //updatePanel2.Update();
            }

        }

        private void insertarInformacionAC()
        {
            long id = 0;
            MySqlConnection con = null;

            String query = "";
            try
            {
                
                query = " INSERT into  tblciudadatencion(fecha,expediente,nombre,primerApellido,segundoApellido) VALUES ('" +
                   fechaAC.Text + "' ,'" + expedienteAC.Text + "' , '" + nombrePersona.Text + "' , '" + primerApellido.Text + "' , '" + segundoApellido.Text + "' ) ";

                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(query, con);
                cmd.ExecuteNonQuery();

                //id = cmd.LastInsertedId;
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

                limpiarFormularios();
                obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 2, "listaAtencion");

            }

            //return id;
        }

        protected void botonEditar_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// Obtener y crear todas las tablas que se mostrarán para "Centros de Mediación"
        /// </summary>
        public void getInformacionCentrosMediacion()
        {
            generarTablasParentalidadRubros();
            generarTablasParentalidadNinos();
            generarTablasParentalidadParticipantes();
        }





        /// <summary>
        /// Obtener y crear todas las tablas que se mostrarán para "Coordinación de Parentalidad"
        /// </summary>
        public void getInformacionCoordinacionParentalidad() {
            generarTablasParentalidadRubros();
            generarTablasParentalidadNinos();
            generarTablasParentalidadParticipantes();
        }

        public void generarTablasParentalidadRubros()
        {
            String[] nombresTablasParentalidad = { "tablaRubrosTotales"};
            //String[] titulosTablas = { "Centro de Convivencia Familiar de Ecatepec ", "Centro de Convivencia Familiar de Toluca " };
            int[] idRegion = { 4, 1 };
            String[] header = { "Rubro", "Total" };
            string[] nomRubro = { "Sesiones de Parentalidad", "Hombres", "Mujeres" };
            try
            {
                for (int k = 0; k < nombresTablasParentalidad.Length; k++)
                {
                    List<int> lista = obtenerDatosAlmacenadosProcedureLista("obtenInformacionCiudadMujeresParentalidad", 0, 2023, 0);
                    Table tabla = new Table();
                    TableRow tr_h = new TableRow();
                    tabla.ID = nombresTablasParentalidad[k] + "Rubros";
                    tabla.Attributes.Add("class", "table");

                    for (int i = 0; i < header.Length; i++)
                    {
                        TableCell celda = new TableCell();
                        celda.Text = header[i];
                        tr_h.Cells.Add(celda);
                    }
                    tabla.Rows.Add(tr_h);

                    int sizeLista = lista.ToArray().Length;
                    for (int i = 0; i < sizeLista; i++)
                    {
                        TableRow tr_f = new TableRow();

                        TableCell celdaGen = new TableCell();
                        celdaGen.Text = nomRubro[i];
                        celdaGen.RowSpan = 1;
                        tr_f.Cells.Add(celdaGen);

                        TableCell celdaTotal = new TableCell();
                        celdaTotal.Text = lista[i].ToString();
                        celdaTotal.RowSpan = 1;
                        tr_f.Cells.Add(celdaTotal);
                        tabla.Rows.Add(tr_f);
                    }
                   
                    Label texto1 = new Label();
                    texto1.Text = "<br/>";
                    contentParentalidad.Controls.Add(texto1);
                    contentParentalidad.Controls.Add(tabla);
                    Label texto = new Label();
                    texto.Text = "<br/><br/><br/>";
                    contentParentalidad.Controls.Add(texto);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Error en generarTablasCoordinacionParentalidad" + error.ToString());
            }
        }


        public void generarTablasParentalidadNinos() {
            String[] nombresTablasParentalidad = { "tablaEcatapec" ,  "tablaToluca"};
            String[] titulosTablas = { "Centro de Convivencia Familiar de Ecatepec ", "Centro de Convivencia Familiar de Toluca " };
            int[] idRegion = { 4 , 1 };
            String[] header = { "Etapa","Edad", "Género","Total"};
            string[] nomEtapa = { "Lactantes o Infantes", "Preescolar", "Escolar o Infantil", "Adolescentes" };
            string[] edad = { "0 a 3 años", "3 a 6 años", "6 a 12 años", "12 a 18 años" };
            string[] genero = {"Niñas","Niños" };
            try
            {
                for (int k = 0; k < nombresTablasParentalidad.Length; k++)
                {
                    List<int> lista= obtenerDatosAlmacenadosProcedureLista("obtenInformacionCiudadMujeresParentalidad", 1, 2023, idRegion[k]);
                    Table tabla = new Table();
                    TableRow tr_h = new TableRow();
                    tabla.ID = nombresTablasParentalidad[k] + "Parentalidad";
                    tabla.Attributes.Add("class", "table");

                    for (int i = 0; i < header.Length; i++)
                    {
                        TableCell celda = new TableCell();
                        celda.Text = header[i];
                        tr_h.Cells.Add(celda);
                    }
                    tabla.Rows.Add(tr_h);

                    int rowspan = 2;
                    int contEtapa = 0;
                    int sizeLista = lista.ToArray().Length;
                    for (int i = 0; i < sizeLista; i++)
                    {
                        int indGen = 1;
                        TableRow tr_f = new TableRow();

                        if ((i) % 2 == 0 &&  i < (sizeLista-1))
                        {
                            TableCell celdaEtapa = new TableCell();
                            celdaEtapa.Text = nomEtapa[contEtapa];
                            celdaEtapa.RowSpan = rowspan;
                            tr_f.Cells.Add(celdaEtapa);

                            TableCell celdaEdad = new TableCell();
                            celdaEdad.Text = edad[contEtapa];
                            celdaEdad.RowSpan = rowspan;
                            tr_f.Cells.Add(celdaEdad);
                            indGen = 0;
                            contEtapa++;
                        }

                        if (i == (sizeLista - 1))
                        {
                            TableRow tr_lr = new TableRow();
                            TableCell celdaTotalText = new TableCell();
                            celdaTotalText.Text = "Total";
                            celdaTotalText.ColumnSpan = 3;
                            tr_lr.Cells.Add(celdaTotalText);

                            TableCell celdaNumeroTotal = new TableCell();
                            celdaNumeroTotal.Text = lista[i].ToString();
                            celdaNumeroTotal.RowSpan = 1;
                            tr_lr.Cells.Add(celdaNumeroTotal);
                            tabla.Rows.Add(tr_lr);
                        }
                        else {
                            TableCell celdaGen = new TableCell();
                            celdaGen.Text = genero[indGen];
                            celdaGen.RowSpan = 1;
                            tr_f.Cells.Add(celdaGen);

                            TableCell celdaTotal = new TableCell();
                            celdaTotal.Text = lista[i].ToString();
                            celdaTotal.RowSpan = 1;
                            tr_f.Cells.Add(celdaTotal);
                            tabla.Rows.Add(tr_f);
                        }
                    }
                    Label textoTitulo = new Label();
                    textoTitulo.Text = titulosTablas[k] ;
                    textoTitulo.Attributes.Add("class", "h4");
                    contentParentalidad.Controls.Add(textoTitulo);
                    Label texto1 = new Label();
                    texto1.Text = "<br/>";
                    contentParentalidad.Controls.Add(texto1);
                    contentParentalidad.Controls.Add(tabla);
                    Label texto = new Label();
                    texto.Text = "<br/><br/><br/>";
                    contentParentalidad.Controls.Add(texto);
                }      
            }
            catch (Exception error)
            {
                Console.WriteLine("Error en generarTablasCoordinacionParentalidad" + error.ToString());
            }
        }


        public void generarTablasParentalidadParticipantes()
        {
            String[] nombresTablasParentalidad = { "tablaEcatapec", "tablaToluca" };
            String[] titulosTablas = { "Centro de Convivencia Familiar de Ecatepec (Participantes)", "Centro de Convivencia Familiar de Toluca (Participantes)" };
            int[] idRegion = { 4, 1 };
            String[] header = { "PARTICIPANTES",  "Género", "Total" };
            string[] nompAR = { "Coordinados", "Juzgadores", "Abogados", "Familias extensas" };
            string[] genero = { "HOMBRE", "MUJER" };
            try
            {
                for (int k = 0; k < nombresTablasParentalidad.Length; k++)
                {
                    List<int> lista = obtenerDatosAlmacenadosProcedureLista("obtenInformacionCiudadMujeresParentalidad", 2, 2023, idRegion[k]);
                    Table tabla = new Table();
                    TableRow tr_h = new TableRow();
                    tabla.ID = nombresTablasParentalidad[k] + "ParentalidadParticipantes";
                    tabla.Attributes.Add("class", "table table-striped");

                    for (int i = 0; i < header.Length; i++)
                    {
                        TableCell celda = new TableCell();
                        celda.Text = header[i];
                        tr_h.Cells.Add(celda);
                    }
                    tabla.Rows.Add(tr_h);

                    int rowspan = 2;
                    int contEtapa = 0;
                    int sizeLista = lista.ToArray().Length;
                    for (int i = 0; i < sizeLista; i++)
                    {
                        int indGen = 1;
                        TableRow tr_f = new TableRow();

                        if ((i) % 2 == 0 && i < (sizeLista - 1))
                        {
                            TableCell celdaEtapa = new TableCell();
                            celdaEtapa.Text = nompAR[contEtapa];
                            celdaEtapa.RowSpan = rowspan;
                            tr_f.Cells.Add(celdaEtapa);
                            indGen = 0;
                            contEtapa++;
                        }

                        /*if (i == (sizeLista - 1))
                        {
                            TableRow tr_lr = new TableRow();
                            TableCell celdaTotalText = new TableCell();
                            celdaTotalText.Text = "Total";
                            celdaTotalText.ColumnSpan = 3;
                            tr_lr.Cells.Add(celdaTotalText);

                            TableCell celdaNumeroTotal = new TableCell();
                            celdaNumeroTotal.Text = lista[i].ToString();
                            celdaNumeroTotal.RowSpan = 1;
                            tr_lr.Cells.Add(celdaNumeroTotal);
                            tabla.Rows.Add(tr_lr);
                        }
                        else
                        {*/
                            TableCell celdaGen = new TableCell();
                            celdaGen.Text = genero[indGen];
                            celdaGen.RowSpan = 1;
                            tr_f.Cells.Add(celdaGen);

                            TableCell celdaTotal = new TableCell();
                            celdaTotal.Text = lista[i].ToString();
                            celdaTotal.RowSpan = 1;
                            tr_f.Cells.Add(celdaTotal);
                            tabla.Rows.Add(tr_f);
                        //}
                    }
                    Label textoTitulo = new Label();
                    textoTitulo.Text = titulosTablas[k];
                    textoTitulo.Attributes.Add("class", "h4");
                    contentParentalidad.Controls.Add(textoTitulo);
                    Label texto1 = new Label();
                    texto1.Text = "<br/>";
                    contentParentalidad.Controls.Add(texto1);
                    contentParentalidad.Controls.Add(tabla);
                    Label texto = new Label();
                    texto.Text = "<br/><br/><br/>";
                    contentParentalidad.Controls.Add(texto);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Error en generarTablasCoordinacionParentalidad" + error.ToString());
            }
            // panelParentalidad.Controls.Add();
        }



























        public void inicializarDatos()
        {
            fechaRegistro.Text = "";
            listaTipo.SelectedValue = "1";
            totalHombres.Text = "";
            totalMujeres.Text = "";

            fechaActual = DateTime.Now;
            fechaDiaActual = fechaActual.ToString(pattern);
            fechaAC.Text = fechaDiaActual;

            //Session["idRegistro"] = "0";
        }


        public void cerrarDialog(String idDialog)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('.modal-backdrop').remove();");
            sb.Append("$(document.body).removeClass('modal-open'); ");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), idDialog, sb.ToString(), false);
        }


        public void abrirDialog(String idDialog) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#" + idDialog + "').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), idDialog, sb.ToString(), false);
        }












    }
}