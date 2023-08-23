using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SistemaIntegralEstadistica.Controlador;
using SistemaIntegralEstadistica.Modelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
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
        String idsCentrosMediacionConsulta;
        String idRegistro = "0";
        Dictionary<String, String> centros;
        protected void Page_Load(object sender, EventArgs e)
        {
            exampleModalCenterTitle.Text = "Registro";

            /*botonGuardar.Attributes.Add("data-dismiss", "modal");
            botonGuardar.Attributes.Add("data-backdrop", "true");*/

            //Session["idRegistro"] = "0";
            centros = new Dictionary<string, string>();
            centros.Add("21", "11637");
            centros.Add("22", "11638");
            centros.Add("21,22", "11637,11638");

            if (!this.IsPostBack)
            {
                consultarCatalogo();
                consultarCatalogoSexo();
                panelMediacion.Visible = true;
                panelParentalidad.Visible = false;
                panelUnidadIgualdad.Visible = false;
                panelAyC.Visible = false;
                //UpdatePanelCanalizacion.Visible = false;
            }
            Console.WriteLine("Termina page_load");
            menuCiudad.Visible = true;
            /*panelMediacion.Visible = true;
            panelParentalidad.Visible = false;
            panelUnidadIgualdad.Visible = false;
            panelAyC.Visible = false;*/
            obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 2, "listaAtencionCanalizacion");

            inicializarDatos();
        }


        protected String getUltimoDiaHabil() {
            String ultimoDia = "";
            try
            {
                DateTime fechaActualSrv = DateTime.Now;
                int diasResta = -1; // Por default solo se resta un día

                DayOfWeek dia = fechaActualSrv.DayOfWeek;

                if (dia.ToString().Equals("Saturday")) // Sabado
                {
                    Console.WriteLine("DIA");
                    diasResta = -1;
                }
                else if (dia.ToString().Equals("Sunday")) // Domingo
                {
                    Console.WriteLine("DIA");
                    diasResta = -2;
                }
                else if (dia.ToString().Equals("Monday")) // Lunes
                {
                    Console.WriteLine("DIA");
                    diasResta = -3;
                }
                fechaActualSrv = fechaActualSrv.AddDays( diasResta);

                ultimoDia = fechaActualSrv.ToString("yyyy-MM-dd");
            }
            catch (Exception error)
            {

                Console.WriteLine("Error: "+ error.ToString());
            }

            return ultimoDia;
        }



        protected void getInformacionUnidadIgualdad() {
            consultarRegistrosPorDia();
            obtenerDatosAlmacenadosProcedure("GetTotalServiciosUIDH", 1, "tablaUT");
            obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 2, "tablaAtencionCiudadana");
            obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 2, "listaAtencionCanalizacion");
           

        }

        protected void getInformacionMediacion() {
            panelMediacion.Visible = true;
            panelParentalidad.Visible = false;
            panelUnidadIgualdad.Visible = false;
            panelAyC.Visible = false;
            //UpdatePanelCanalizacion.Visible = false;
            panelTablasMediacion.Visible = true;
            this.obtenerFechasPeriodoInformacionMediacion();
            this.getTablasMediacion();
        }


        private void getTablasMediacion() {
            List<ModeloTablaReporte> listaRep = new List<ModeloTablaReporte>();
            listaRep.Add(consultarMediacionServicio());
            listaRep.Add(consultarMediacionIniciadosPorNaturaleza());
            listaRep.Add(consultarMediacionSolicitantesSesiones());
            listaRep.Add(consultarMediacionPersonasInvitadasSesiones());
            listaRep.Add(consultarMediacionConcluidosPorNaturaleza());
            listaRep.Add(consultarMediacionConcluidosPorNaturalezaTipoSolucion() );
            listaRep.Add(consultarMediacionSolicitantesCausaSolucion());
            listaRep.Add(consultarMediacionInvitadosCausaSolucion());

            //generarInformacionExcel(listaRep);
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
            listaAtencionCanalizacion.PageIndex = e.NewPageIndex;
            obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 2 , "listaAtencionCanalizacion");

        }


        public void consultarInformacionMediacion() { 
        
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


        public void consultarCatalogoSexo()
        {
            List<ModeloCatalogo> listaTexto = new List<ModeloCatalogo>(); ;
            String consulta = "SELECT id , nombre as descripcion FROM  tblcatsexo where activo =1 ; ";
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

                sexoCanalizacion.DataSource = listaTexto;
                sexoCanalizacion.DataValueField = "id";
                sexoCanalizacion.DataTextField = "descripcion";
                sexoCanalizacion.DataBind();
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

        protected void Cancelar_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Object: " + sender);
            Button boton = (Button)sender;

            if (boton.ID.Equals("ButtonCancelarUT"))
            {
                cerrarDialog("myModal");
            }
            else if (boton.ID.Equals("ButtonCancelarAC")) {
                cerrarDialog("Panel1");
            }
            limpiarFormularios();
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
            fechaAC.Text = "";
            expedienteAC.Text = "";
            nombrePersona.Text = "";
            primerApellido.Text = "";
            segundoApellido.Text = "";
            sexoCanalizacion.SelectedValue = "1";
            botonGuardarUT.Visible = true;
            fechaRegistro.Attributes.Add("min", getUltimoDiaHabil());
            fechaRegistro.Enabled = true;
            totalHombres.Enabled = true;
            totalMujeres.Enabled = true;
        }

        public void obtenerFechasPeriodoInformacion()
        {
            fechaActual = DateTime.Now;
            fechaDiaActual = fechaActual.ToString(pattern);
            fechaPeriodoInicio = periodoInicio.Text.Equals("") ? fechaDiaActual : periodoInicio.Text;
            fechaPeriodoFin = periodoFin.Text.Equals("") ? fechaDiaActual : periodoFin.Text;
        }

        public void obtenerFechasPeriodoInformacionMediacion()
        {
            fechaActual = DateTime.Now;
            fechaDiaActual = fechaActual.ToString(pattern);
            fechaPeriodoInicio = medFechaInicio.Text.Equals("") ? fechaDiaActual : medFechaInicio.Text;
            fechaPeriodoFin = medFechaFin.Text.Equals("") ? fechaDiaActual : medFechaFin.Text;
            if (!idCentroMediacion.SelectedValue.Equals("-1")) {
                idsCentrosMediacionConsulta = !idCentroMediacion.SelectedValue.Equals("999") ? idCentroMediacion.SelectedValue.ToString() : "21,22";
            }
         
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
                String errorMessage = error.ToString();
                Console.WriteLine("Error: " + errorMessage);
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
                consultarRegistrosPorDia();
                //obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 2, "listaDatos");

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
                String id = listaAtencionCanalizacion.DataKeys[e.RowIndex].Value.ToString();

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
                obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 2, "listaAtencionCanalizacion");

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
                //obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 2, "listaDatos");
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
            String ultimoDia = getUltimoDiaHabil();
            String diaActual = DateTime.Now.ToString("yyyy-MM-dd");

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

                    //Activar / Desactivar campos
                    if (fechaRegistro.Text.Equals(ultimoDia) || fechaRegistro.Text.Equals(diaActual))
                    {
                        botonGuardarUT.Visible = true;
                        fechaRegistro.Attributes.Add("min", ultimoDia);
                        fechaRegistro.Enabled = true;
                        totalHombres.Enabled = true;
                        totalMujeres.Enabled = true;
                    }
                    else
                    {
                        botonGuardarUT.Visible = false;
                        fechaRegistro.Attributes.Add("min", "");
                        fechaRegistro.Enabled = false;
                        totalHombres.Enabled = false;
                        totalMujeres.Enabled = false;
                        //listaTipo.Enabled = false;
                    }

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
            String ultimoDia = getUltimoDiaHabil();
            String diaActual = DateTime.Now.ToString("yyyy-MM-dd");
            try
            {
                if (command.ToLower().Contains("editar"))
                {
                    exampleModalCenterTitle.Text = "Editar Registro";
                    int rowIndex = Convert.ToInt32(e.CommandArgument);
                    GridViewRow row = listaAtencionCanalizacion.Rows[rowIndex];

                    idRegistro = listaAtencionCanalizacion.Rows[rowIndex].Cells[0].Text;
                    fechaAC.Text = listaAtencionCanalizacion.Rows[rowIndex].Cells[1].Text;
                    if (fechaAC.Text.Equals(ultimoDia) || fechaAC.Text.Equals(diaActual))
                    {
                        fechaAC.Enabled = true;
                        //fechaAC.Attributes.Add("disabled", "false");
                        ButtonGuardarAyC.Visible = true;
                        fechaAC.Attributes.Add("min", ultimoDia);
                    }
                    else {
                        fechaAC.Enabled = false;
                        //fechaAC.Attributes.Add("disabled", "true");
                        ButtonGuardarAyC.Visible = false;
                        fechaAC.Attributes.Add("min", "");
                    }
                    expedienteAC.Text = listaAtencionCanalizacion.Rows[rowIndex].Cells[2].Text;
                    nombrePersona.Text = listaAtencionCanalizacion.Rows[rowIndex].Cells[3].Text;
                    primerApellido.Text = listaAtencionCanalizacion.Rows[rowIndex].Cells[4].Text;
                    segundoApellido.Text = listaAtencionCanalizacion.Rows[rowIndex].Cells[5].Text;
                    sexoCanalizacion.SelectedValue = listaAtencionCanalizacion.Rows[rowIndex].Cells[6].Text;
                    Session["idRegistro"] = idRegistro;

                    abrirDialog("Panel1");
                }
                else
                {
                    Session["idRegistro"] = "0";
                    inicializarDatos();
                }
                Console.WriteLine("Terminarow command");

            }
            catch (Exception error)
            {

                Console.WriteLine("ERROR AL EDIATR " + error.ToString());
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

        protected void ButtonConcultarMediacion_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Empieza búsqueda");
            getInformacionMediacion();

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
                    + "',  nombre = '" + nombrePersona.Text + "',  primerApellido = '" + primerApellido.Text+"' ,  segundoApellido = '" + segundoApellido.Text + "' , sexo = " +sexoCanalizacion.SelectedValue +" "
                    + " WHERE  id = " + idReg;

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
                obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 2, "listaAtencionCanalizacion");
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
                
                query = " INSERT into  tblciudadatencion(fecha,expediente,nombre,primerApellido,segundoApellido,sexo) VALUES ('" +
                   fechaAC.Text + "' ,'" + expedienteAC.Text + "' , '" + nombrePersona.Text + "' , '" + primerApellido.Text + "' , '" + segundoApellido.Text + "' , "+ sexoCanalizacion.SelectedValue + " ) ";

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
                obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", 2, "listaAtencionCanalizacion");

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


        public ModeloTablaReporte consultarMediacionServicio()
        {
            String cveGestion = centros[idsCentrosMediacionConsulta]; 

            ModeloTablaReporte rep = new ModeloTablaReporte();
            String consulta = /*"select 'Orientación (Juntas informativas)' as 'SERVICIO' , count(distinct (case when u.CveSexo = '1'  then u.idJuntaParte end)) as 'Total Hombres'," +
                "        count(distinct(case when u.CveSexo = '2' then u.idJuntaParte end)) as 'Total Mujeres'," +
                "        count(distinct(case when u.CveSexo = '2' or u.CveSexo = '1'  then u.idJuntaParte end)) as 'total'" +
                "                    from htsj_mediacion.tbljuntas_partes  as u" +
                "                    inner join htsj_mediacion.tbljuntas_registros tr on u.idJuntaRegistro = tr.idJuntaRegistro" +
                "        where u.FechaRegistro between '2022-01-01' and '2022-12-31' and tr.activo = 'S'"+*/
                "SELECT  'Orientación (Juntas Informativas Agendadas)' as 'Servicio' ,  " +
                "           count(distinct (case when u.CveSexo = '1'   then u.idJuntaParte end)) as 'Total Hombres'," +
                "         count(distinct (case when u.CveSexo = '2' then u.idJuntaParte end)) as 'Total Mujeres', " +
                "         count(distinct (case when u.CveSexo = '2' or u.CveSexo = '1' then u.idJuntaParte end)) as 'Total' " +
                "              from htsj_mediacion.tbljuntas_fechas tf " +
                "              inner join htsj_mediacion.tbljuntas_sesiones ts on tf.idJuntaFecha = ts.idJuntaFecha" +
                "              inner join htsj_mediacion.tbljuntas_registros tr  on ts.idJuntaSesion = tr.idJuntaSesion" +
                "              inner join htsj_mediacion.tbljuntas_partes u on tr.idJuntaRegistro = u.idJuntaRegistro" +
                "              inner join htsj_mediacion.tblcentrom cm on tf.idRefJuzgado = cm.idJuzgadoGestion" +
                "                    where  tr.FechaRegistro  between  '" + fechaPeriodoInicio + " 00:00:00' and '" + fechaPeriodoFin + " 23:59:59' and" +
                "                    tr.activo = 's' and tf.activo = 's' and  ts.activo = 's' and u.activo = 's'" +
                "                    and tf.idRefJuzgado in (" + cveGestion + ")" +
                " UNION" +
                " SELECT  if(u.Tipo = 'I', 'Número de personas invitadas a sesiones de mediación', 'Número de personas solicitantes en sesiones de Mediación') 'SERVICIO'," +
                "    ifnull(count(distinct CASE WHEN(U.Sexo = 'H') then e.CveExp end), 0) as 'HOMBRE'," +
                "	ifnull(count(distinct CASE WHEN(U.Sexo = 'M') then e.CveExp end), 0) as 'MUJER', " +
                " 	ifnull(count(distinct CASE WHEN(U.Sexo = 'H') then e.CveExp end), 0) + ifnull(count(distinct CASE WHEN(U.Sexo = 'M') then e.CveExp end), 0) as 'TOTAL'" +
                "    from htsj_mediacion.tblexpediente e" +
                "        left" +
                "    join htsj_mediacion.tblusuario U on E.CveExp = U.CveExp  and(u.Tipo = 'I' OR u.Tipo = 's')" +
                "    where Activo = 1" +
                "        and FechaReg between  '" + fechaPeriodoInicio + "' and '" + fechaPeriodoFin + "' and e.idCentroM in (" + idsCentrosMediacionConsulta + ")" +
                "    group by(u.tipo)" +
                " union" +
                "    select   'Apertura de expediente' AS 'SERVICIO' , " +
                "		ifnull(count(distinct CASE WHEN(U.Sexo = 'H') then e.CveExp end), 0) as 'Hombre'," +
                "		ifnull(count(distinct CASE WHEN(U.Sexo = 'M') then e.CveExp end), 0) as 'Mujer', " +
                "		ifnull(count(distinct CASE WHEN(U.Sexo = 'H') then e.CveExp end), 0) + ifnull(count(distinct CASE WHEN(U.Sexo = 'M') then e.CveExp end), 0) as 'TOTAL'" +
                "    from htsj_mediacion.tblexpediente as e" +
                "        left join htsj_mediacion.tblusuario u on e.CveExp = u.CveExp" +
                "        where FechaReg between  '" + fechaPeriodoInicio + "' and '" + fechaPeriodoFin + "' and e.idCentroM in (" + idsCentrosMediacionConsulta + ")" +
                " UNION" +
                "    select   'Expedientes de Mediación concluidos' AS 'SERVICIO' , " +
                "	ifnull(count(distinct CASE WHEN(U.Sexo = 'H') then e.CveExp end), 0) as 'Hombre'," +
                "		ifnull(count(distinct CASE WHEN(U.Sexo = 'M') then e.CveExp end), 0) as 'Mujer', " +
                "		ifnull(count(distinct CASE WHEN(U.Sexo = 'H') then e.CveExp end), 0) + ifnull(count(distinct CASE WHEN(U.Sexo = 'M') then e.CveExp end), 0) as 'TOTAL'" +
                "    from htsj_mediacion.tblexpediente as e" +
                "        left join htsj_mediacion.tblusuario u on e.CveExp = u.CveExp" +
                "        where FechaTer between '" + fechaPeriodoInicio + "' and '" + fechaPeriodoFin + "' and e.idCentroM in (" + idsCentrosMediacionConsulta + ")";
            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["bd67"]);
                con.Open();
                cmd = new MySqlCommand(consulta, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(resultado);
                tablaMediacion1.DataSource = dt;
                tablaMediacion1.DataBind();
                rep.Titulo = "";
                rep.Tabla = dt;
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
            return rep;
        }


        public ModeloTablaReporte consultarMediacionIniciadosPorNaturaleza()
        {
            ModeloTablaReporte rep = new ModeloTablaReporte();
            String consulta = "Select n.Descripcion 'Naturaleza'," +
                " count(distinct e.CveExp) as 'Número de Expedientes'" +
                " from htsj_mediacion.tblexpediente e" +
                " inner" +
                " join htsj_mediacion.tblnaturaleza n on n.idNat = e.idNat" +
                " where n.Descripcion not like 'JR %'" +
                " and FechaReg between '" + fechaPeriodoInicio + "' and '" + fechaPeriodoFin + "'" +
                //" and e.idTipoTer is null "
                " and e.idCentroM in (" + idsCentrosMediacionConsulta + ")" +
                " group by(e.idNat) ORDER BY 1 ASC; ";
            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["bd67"]);
                con.Open();
                cmd = new MySqlCommand(consulta, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(resultado);
                tablaMediacion2.DataSource = dt;
                tablaMediacion2.DataBind();
                rep.Titulo = "Expedientes inciados, por naturaleza";
                rep.Tabla = dt;
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
            return rep;
        }



        public ModeloTablaReporte consultarMediacionSolicitantesSesiones()
        {
            ModeloTablaReporte rep = new ModeloTablaReporte();

            String consulta = "select n.Descripcion as Naturaleza,"
    + "		count(distinct (case when us.tipo = 'S' and us.Sexo = 'H' and us.tPersona = 'Física' then us.idUsu end)) as 'Total Hombres',"
    + "		count(distinct (case when us.tipo = 'S' and us.Sexo = 'M' and us.tPersona = 'Física' then us.idUsu end)) as 'Total Mujeres',"
    + "		count(distinct (case when us.tipo = 'S' and us.tPersona = 'Moral' then us.idUsu end)) as 'Moral',"
    + "		count(distinct (case when us.tipo = 'S' and us.tPersona = 'Estado' then us.idUsu end)) as 'N/Identificado',"
    + "		count(distinct (case when us.tipo = 'S' then us.idUsu end)) as 'Total'"
    + "	from	htsj_mediacion.tblexpediente as e"
    + "	left join htsj_mediacion.tblnaturaleza as n on e.idNat = n.idNat"
    + "	left join htsj_mediacion.tbltipoexpediente as te on e.idTipoExpediente = te.idTipoExpediente"
    + "	left join (select"
    + "					u.idUsu,"
    + "					u.CveExp,"
    + "					u.Tipo,"
    + "					u.Nombre,"
    + "					u.ApePaterno,"
    + "					u.ApeMaterno,"
    + "					u.Aclaracion,"
    + "					u.Sexo,"
    + "					u.Edad,"
    + "					case "
    + "						when ((upper(u.Nombre) like '%AYUNTAMIENTO%'"
    + "							or upper(u.Nombre) like '%MUNICIPIO%'"
    + "							or upper(u.Nombre) like '%ESTADO%'"
    + "							or upper(u.Nombre) like '%ESTATAL%'"
    + "							or upper(u.Nombre) like '%FEDERAL%'"
    + "							or upper(u.Nombre) like '%PROCURADOR%'"
    + "							or upper(u.Nombre) like '%F_SCAL%'"
    + "							or upper(u.Nombre) like '%GOBIERNO%'"
    + "							or upper(u.Nombre) like '%SECRETARIO%'"
    + "							or upper(u.Nombre) like '%SECRETAR_A%'"
    + "							or upper(u.Nombre) like '%INSTITUTO%'"
    + "							or upper(u.Nombre) like '%SALUD%'"
    + "							or upper(u.Nombre) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.Nombre) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.Nombre) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'"
    + "							or upper(u.ApePaterno) like '%AYUNTAMIENTO%'"
    + "							or upper(u.ApePaterno) like '%MUNICIPIO%'"
    + "							or upper(u.ApePaterno) like '%ESTADO%'"
    + "							or upper(u.ApePaterno) like '%ESTATAL%'"
    + "							or upper(u.ApePaterno) like '%FEDERAL%'"
    + "							or upper(u.ApePaterno) like '%PROCURADOR%'"
    + "							or upper(u.ApePaterno) like '%F_SCAL%'"
    + "							or upper(u.ApePaterno) like '%GOBIERNO%'"
    + "							or upper(u.ApePaterno) like '%SECRETARIO%'"
    + "							or upper(u.ApePaterno) like '%SECRETAR_A%'"
    + "							or upper(u.ApePaterno) like '%INSTITUTO%'"
    + "							or upper(u.ApePaterno) like '%SALUD%'"
    + "							or upper(u.ApePaterno) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.ApePaterno) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'"
    + "							or upper(u.ApeMaterno) like '%AYUNTAMIENTO%'"
    + "							or upper(u.ApeMaterno) like '%MUNICIPIO%'"
    + "							or upper(u.ApeMaterno) like '%ESTADO%'"
    + "							or upper(u.ApeMaterno) like '%ESTATAL%'"
    + "							or upper(u.ApeMaterno) like '%FEDERAL%'"
    + "							or upper(u.ApeMaterno) like '%PROCURADOR%'"
    + "							or upper(u.ApeMaterno) like '%F_SCAL%'"
    + "							or upper(u.ApeMaterno) like '%GOBIERNO%'"
    + "							or upper(u.ApeMaterno) like '%SECRETARIO%'"
    + "							or upper(u.ApeMaterno) like '%SECRETAR_A%'"
    + "							or upper(u.ApeMaterno) like '%INSTITUTO%'"
    + "							or upper(u.ApeMaterno) like '%SALUD%'"
    + "							or upper(u.ApeMaterno) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'"
    + "							or upper(u.Aclaracion) like '%AYUNTAMIENTO%'"
    + "							or upper(u.Aclaracion) like '%MUNICIPIO%'"
    + "							or upper(u.Aclaracion) like '%ESTADO%'"
    + "							or upper(u.Aclaracion) like '%ESTATAL%'"
    + "							or upper(u.Aclaracion) like '%FEDERAL%'"
    + "							or upper(u.Aclaracion) like '%PROCURADOR%'"
    + "							or upper(u.Aclaracion) like '%F_SCAL%'"
    + "							or upper(u.Aclaracion) like '%GOBIERNO%'"
    + "							or upper(u.Aclaracion) like '%SECRETARIO%'"
    + "							or upper(u.Aclaracion) like '%SECRETAR_A%'"
    + "							or upper(u.Aclaracion) like '%INSTITUTO%'"
    + "							or upper(u.Aclaracion) like '%SALUD%'"
    + "							or upper(u.Aclaracion) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.Aclaracion) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' )"
    + "							and upper(u.Nombre) not like '%ASOCIACI_N%'"
    + "							and upper(u.Nombre) not like '%SOCIEDAD%'"
    + "							and upper(u.ApePaterno) not like '%ASOCIACI_N%'"
    + "							and upper(u.ApePaterno) not like '%SOCIEDAD%'"
    + "							and upper(u.ApeMaterno) not like '%ASOCIACI_N%'"
    + "							and upper(u.ApeMaterno) not like '%SOCIEDAD%'"
    + "							and upper(u.Aclaracion) not like '%ASOCIACI_N%'"
    + "							and upper(u.Aclaracion) not like '%SOCIEDAD%'"
    + "							and upper(u.Nombre) not regexp '^A[.,; ]C[.,; ]'"
    + "							and upper(u.Nombre) not regexp ' A[.,; ]C[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'A[.,; ]C[.,; ]$'"
    + "							and upper(u.Nombre) not regexp '^A[.,; ]R[.,; ]'"
    + "							and upper(u.Nombre) not regexp ' A[.,; ]R[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'A[.,; ]R[.,; ]$'"
    + "							and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]'"
    + "							and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]$'"
    + "							and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							and upper(u.Nombre) not regexp '^C[.,; ]V[.,; ]'"
    + "							and upper(u.Nombre) not regexp ' C[.,; ]V[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'C[.,; ]V[.,; ]$'"
    + "							and upper(u.Nombre) not regexp '^R[.,; ]L[.,; ]'"
    + "							and upper(u.Nombre) not regexp ' R[.,; ]L[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'R[.,; ]L[.,; ]$'"
    + "							and upper(u.Nombre) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							and upper(u.Nombre) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "							and upper(u.ApePaterno) not regexp '^A[.,; ]C[.,; ]'"
    + "							and upper(u.ApePaterno) not regexp ' A[.,; ]C[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'A[.,; ]C[.,; ]$'"
    + "							and upper(u.ApePaterno) not regexp '^A[.,; ]R[.,; ]'"
    + "							and upper(u.ApePaterno) not regexp ' A[.,; ]R[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'A[.,; ]R[.,; ]$'"
    + "							and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]'"
    + "							and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]$'"
    + "							and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							and upper(u.ApePaterno) not regexp '^C[.,; ]V[.,; ]'"
    + "							and upper(u.ApePaterno) not regexp ' C[.,; ]V[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'C[.,; ]V[.,; ]$'"
    + "							and upper(u.ApePaterno) not regexp '^R[.,; ]L[.,; ]'"
    + "							and upper(u.ApePaterno) not regexp ' R[.,; ]L[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'R[.,; ]L[.,; ]$'"
    + "							and upper(u.ApePaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							and upper(u.ApePaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "							and upper(u.ApeMaterno) not regexp '^A[.,; ]C[.,; ]'"
    + "							and upper(u.ApeMaterno) not regexp ' A[.,; ]C[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'A[.,; ]C[.,; ]$'"
    + "							and upper(u.ApeMaterno) not regexp '^A[.,; ]R[.,; ]'"
    + "							and upper(u.ApeMaterno) not regexp ' A[.,; ]R[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'A[.,; ]R[.,; ]$'"
    + "							and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]'"
    + "							and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]$'"
    + "							and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							and upper(u.ApeMaterno) not regexp '^C[.,; ]V[.,; ]'"
    + "							and upper(u.ApeMaterno) not regexp ' C[.,; ]V[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'C[.,; ]V[.,; ]$'"
    + "							and upper(u.ApeMaterno) not regexp '^R[.,; ]L[.,; ]'"
    + "							and upper(u.ApeMaterno) not regexp ' R[.,; ]L[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'R[.,; ]L[.,; ]$'"
    + "							and upper(u.ApeMaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							and upper(u.ApeMaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "							and upper(u.Aclaracion) not regexp '^A[.,; ]C[.,; ]'"
    + "							and upper(u.Aclaracion) not regexp ' A[.,; ]C[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'A[.,; ]C[.,; ]$'"
    + "							and upper(u.Aclaracion) not regexp '^A[.,; ]R[.,; ]'"
    + "							and upper(u.Aclaracion) not regexp ' A[.,; ]R[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'A[.,; ]R[.,; ]$'"
    + "							and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]'"
    + "							and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]$'"
    + "							and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							and upper(u.Aclaracion) not regexp '^C[.,; ]V[.,; ]'"
    + "							and upper(u.Aclaracion) not regexp ' C[.,; ]V[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'C[.,; ]V[.,; ]$'"
    + "							and upper(u.Aclaracion) not regexp '^R[.,; ]L[.,; ]'"
    + "							and upper(u.Aclaracion) not regexp ' R[.,; ]L[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'R[.,; ]L[.,; ]$'"
    + "							and upper(u.Aclaracion) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							and upper(u.Aclaracion) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$') then 'Estado'"
    + "						when (upper(u.Nombre) like '%REPRE_ENTANTE%'"
    + "							or upper(u.Nombre) like '%APODERAD_%'"
    + "							or upper(u.Nombre) like '%LEGAL%'"
    + "							or upper(u.ApePaterno) like '%REPRE_ENTANTE%'"
    + "							or upper(u.ApePaterno) like '%APODERAD_%'"
    + "							or upper(u.ApePaterno) like '%LEGAL%'"
    + "							or upper(u.ApeMaterno) like '%REPRE_ENTANTE%'"
    + "							or upper(u.ApeMaterno) like '%APODERAD_%'"
    + "							or upper(u.ApeMaterno) like '%LEGAL%'"
    + "							or upper(u.Aclaracion) like '%REPRE_ENTANTE%'"
    + "							or upper(u.Aclaracion) like '%APODERAD_%'"
    + "							or upper(u.Aclaracion) like '%LEGAL%')"
    + "							and upper(u.Nombre) not like '%MENOR%'"
    + "							and upper(u.Nombre) not like '%V_CTIMA%'"
    + "							and upper(u.Nombre) not like '%HIJ_%'"
    + "							and upper(u.ApePaterno) not like '%MENOR%'"
    + "							and upper(u.ApePaterno) not like '%V_CTIMA%'"
    + "							and upper(u.ApePaterno) not like '%HIJ_%'"
    + "							and upper(u.ApeMaterno) not like '%MENOR%'"
    + "							and upper(u.ApeMaterno) not like '%V_CTIMA%'"
    + "							and upper(u.ApeMaterno) not like '%HIJ_%'"
    + "							and upper(u.Aclaracion) not like '%MENOR%'"
    + "							and upper(u.Aclaracion) not like '%V_CTIMA%'"
    + "							and upper(u.Aclaracion) not like '%HIJ_%'"
    + "							and upper(u.Nombre) not like '%AYUNTAMIENTO%'"
    + "							and upper(u.Nombre) not like '%MUNICIPIO%'"
    + "							and upper(u.Nombre) not like '%ESTADO%'"
    + "							and upper(u.Nombre) not like '%ESTATAL%'"
    + "							and upper(u.Nombre) not like '%FEDERAL%'"
    + "							and upper(u.Nombre) not like '%PROCURADOR%'"
    + "							and upper(u.Nombre) not like '%F_SCAL%'"
    + "							and upper(u.Nombre) not like '%GOBIERNO%'"
    + "							and upper(u.Nombre) not like '%SECRETARIO%'"
    + "							and upper(u.Nombre) not like '%SECRETAR_A%'"
    + "							and upper(u.Nombre) not like '%INSTITUTO%'"
    + "							and upper(u.Nombre) not like '%SALUD%'"
    + "							and upper(u.Nombre) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.Nombre) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' "
    + "							and upper(u.ApePaterno) not like '%AYUNTAMIENTO%'"
    + "							and upper(u.ApePaterno) not like '%MUNICIPIO%'"
    + "							and upper(u.ApePaterno) not like '%ESTADO%'"
    + "							and upper(u.ApePaterno) not like '%ESTATAL%'"
    + "							and upper(u.ApePaterno) not like '%FEDERAL%'"
    + "							and upper(u.ApePaterno) not like '%PROCURADOR%'"
    + "							and upper(u.ApePaterno) not like '%F_SCAL%'"
    + "							and upper(u.ApePaterno) not like '%GOBIERNO%'"
    + "							and upper(u.ApePaterno) not like '%SECRETARIO%'"
    + "							and upper(u.ApePaterno) not like '%SECRETAR_A%'"
    + "							and upper(u.ApePaterno) not like '%INSTITUTO%'"
    + "							and upper(u.ApePaterno) not like '%SALUD%'"
    + "							and upper(u.ApePaterno) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'"
    + "							and upper(u.ApeMaterno) not like '%AYUNTAMIENTO%'"
    + "							and upper(u.ApeMaterno) not like '%MUNICIPIO%'"
    + "							and upper(u.ApeMaterno) not like '%ESTADO%'"
    + "							and upper(u.ApeMaterno) not like '%ESTATAL%'"
    + "							and upper(u.ApeMaterno) not like '%FEDERAL%'"
    + "							and upper(u.ApeMaterno) not like '%PROCURADOR%'"
    + "							and upper(u.ApeMaterno) not like '%F_SCAL%'"
    + "							and upper(u.ApeMaterno) not like '%GOBIERNO%'"
    + "							and upper(u.ApeMaterno) not like '%SECRETARIO%'"
    + "							and upper(u.ApeMaterno) not like '%SECRETAR_A%'"
    + "							and upper(u.ApeMaterno) not like '%INSTITUTO%'"
    + "							and upper(u.ApeMaterno) not like '%SALUD%'"
    + "							and upper(u.ApeMaterno) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'"
    + "							and upper(u.Aclaracion) not like '%AYUNTAMIENTO%'"
    + "							and upper(u.Aclaracion) not like '%MUNICIPIO%'"
    + "							and upper(u.Aclaracion) not like '%ESTADO%'"
    + "							and upper(u.Aclaracion) not like '%ESTATAL%'"
    + "							and upper(u.Aclaracion) not like '%FEDERAL%'"
    + "							and upper(u.Aclaracion) not like '%PROCURADOR%'"
    + "							and upper(u.Aclaracion) not like '%F_SCAL%'"
    + "							and upper(u.Aclaracion) not like '%GOBIERNO%'"
    + "							and upper(u.Aclaracion) not like '%SECRETARIO%'"
    + "							and upper(u.Aclaracion) not like '%SECRETAR_A%'"
    + "							and upper(u.Aclaracion) not like '%INSTITUTO%'"
    + "							and upper(u.Aclaracion) not like '%SALUD%'"
    + "							and upper(u.Aclaracion) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' then 'Moral'"
    + "						when (upper(u.Nombre) like '%ASOCIACI_N%'"
    + "							or upper(u.Nombre) like '%SOCIEDAD%'"
    + "							or upper(u.ApePaterno) like '%ASOCIACI_N%'"
    + "							or upper(u.ApePaterno) like '%SOCIEDAD%'"
    + "							or upper(u.ApeMaterno) like '%ASOCIACI_N%'"
    + "							or upper(u.ApeMaterno) like '%SOCIEDAD%'"
    + "							or upper(u.Aclaracion) like '%ASOCIACI_N%'"
    + "							or upper(u.Aclaracion) like '%SOCIEDAD%'"
    + "							or upper(u.Nombre) regexp '^A[.,; ]C[.,; ]'"
    + "							or upper(u.Nombre) regexp ' A[.,; ]C[.,; ] '"
    + "							or upper(u.Nombre) regexp 'A[.,; ]C[.,; ]$'"
    + "							or upper(u.Nombre) regexp '^A[.,; ]R[.,; ]'"
    + "							or upper(u.Nombre) regexp ' A[.,; ]R[.,; ] '"
    + "							or upper(u.Nombre) regexp 'A[.,; ]R[.,; ]$'"
    + "							or upper(u.Nombre) regexp '^S[.,; ]A[.,; ]'"
    + "							or upper(u.Nombre) regexp ' S[.,; ]A[.,; ] '"
    + "							or upper(u.Nombre) regexp 'S[.,; ]A[.,; ]$'"
    + "							or upper(u.Nombre) regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							or upper(u.Nombre) regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							or upper(u.Nombre) regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							or upper(u.Nombre) regexp '^C[.,; ]V[.,; ]'"
    + "							or upper(u.Nombre) regexp ' C[.,; ]V[.,; ] '"
    + "							or upper(u.Nombre) regexp 'C[.,; ]V[.,; ]$'"
    + "							or upper(u.Nombre) regexp '^R[.,; ]L[.,; ]'"
    + "							or upper(u.Nombre) regexp ' R[.,; ]L[.,; ] '"
    + "							or upper(u.Nombre) regexp 'R[.,; ]L[.,; ]$'"
    + "							or upper(u.Nombre) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							or upper(u.Nombre) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							or upper(u.Nombre) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "							or upper(u.ApePaterno) regexp '^A[.,; ]C[.,; ]'"
    + "							or upper(u.ApePaterno) regexp ' A[.,; ]C[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'A[.,; ]C[.,; ]$'"
    + "							or upper(u.ApePaterno) regexp '^A[.,; ]R[.,; ]'"
    + "							or upper(u.ApePaterno) regexp ' A[.,; ]R[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'A[.,; ]R[.,; ]$'"
    + "							or upper(u.ApePaterno) regexp '^S[.,; ]A[.,; ]'"
    + "							or upper(u.ApePaterno) regexp ' S[.,; ]A[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'S[.,; ]A[.,; ]$'"
    + "							or upper(u.ApePaterno) regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							or upper(u.ApePaterno) regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							or upper(u.ApePaterno) regexp '^C[.,; ]V[.,; ]'"
    + "							or upper(u.ApePaterno) regexp ' C[.,; ]V[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'C[.,; ]V[.,; ]$'"
    + "							or upper(u.ApePaterno) regexp '^R[.,; ]L[.,; ]'"
    + "							or upper(u.ApePaterno) regexp ' R[.,; ]L[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'R[.,; ]L[.,; ]$'"
    + "							or upper(u.ApePaterno) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							or upper(u.ApePaterno) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "							or upper(u.ApeMaterno) regexp '^A[.,; ]C[.,; ]'"
    + "							or upper(u.ApeMaterno) regexp ' A[.,; ]C[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'A[.,; ]C[.,; ]$'"
    + "							or upper(u.ApeMaterno) regexp '^A[.,; ]R[.,; ]'"
    + "							or upper(u.ApeMaterno) regexp ' A[.,; ]R[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'A[.,; ]R[.,; ]$'"
    + "							or upper(u.ApeMaterno) regexp '^S[.,; ]A[.,; ]'"
    + "							or upper(u.ApeMaterno) regexp ' S[.,; ]A[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'S[.,; ]A[.,; ]$'"
    + "							or upper(u.ApeMaterno) regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							or upper(u.ApeMaterno) regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							or upper(u.ApeMaterno) regexp '^C[.,; ]V[.,; ]'"
    + "							or upper(u.ApeMaterno) regexp ' C[.,; ]V[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'C[.,; ]V[.,; ]$'"
    + "							or upper(u.ApeMaterno) regexp '^R[.,; ]L[.,; ]'"
    + "							or upper(u.ApeMaterno) regexp ' R[.,; ]L[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'R[.,; ]L[.,; ]$'"
    + "							or upper(u.ApeMaterno) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							or upper(u.ApeMaterno) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "							or upper(u.Aclaracion) regexp '^A[.,; ]C[.,; ]'"
    + "							or upper(u.Aclaracion) regexp ' A[.,; ]C[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'A[.,; ]C[.,; ]$'"
    + "							or upper(u.Aclaracion) regexp '^A[.,; ]R[.,; ]'"
    + "							or upper(u.Aclaracion) regexp ' A[.,; ]R[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'A[.,; ]R[.,; ]$'"
    + "							or upper(u.Aclaracion) regexp '^S[.,; ]A[.,; ]'"
    + "							or upper(u.Aclaracion) regexp ' S[.,; ]A[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'S[.,; ]A[.,; ]$'"
    + "							or upper(u.Aclaracion) regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							or upper(u.Aclaracion) regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							or upper(u.Aclaracion) regexp '^C[.,; ]V[.,; ]'"
    + "							or upper(u.Aclaracion) regexp ' C[.,; ]V[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'C[.,; ]V[.,; ]$'"
    + "							or upper(u.Aclaracion) regexp '^R[.,; ]L[.,; ]'"
    + "							or upper(u.Aclaracion) regexp ' R[.,; ]L[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'R[.,; ]L[.,; ]$'"
    + "							or upper(u.Aclaracion) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							or upper(u.Aclaracion) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$') then 'Moral' "
    + "						when (u.Sexo in ('H','M')"
    + "							and (upper(u.Nombre) like '%MENOR%'"
    + "								or upper(u.Nombre) like '%V_CTIMA%'"
    + "								or upper(u.Nombre) like '%HIJ_%'"
    + "								or upper(u.ApePaterno) like '%MENOR%'"
    + "								or upper(u.ApePaterno) like '%V_CTIMA%'"
    + "								or upper(u.ApePaterno) like '%HIJ_%'"
    + "								or upper(u.ApeMaterno) like '%MENOR%'"
    + "								or upper(u.ApeMaterno) like '%V_CTIMA%'"
    + "								or upper(u.ApeMaterno) like '%HIJ_%'"
    + "								or upper(u.Aclaracion) like '%MENOR%'"
    + "								or upper(u.Aclaracion) like '%V_CTIMA%'"
    + "								or upper(u.Aclaracion) like '%HIJ_%')) then 'Física' "
    + "						when (u.Sexo in ('H','M')"
    + "							and (upper(u.Nombre) not like '%ASOCIACI_N%'"
    + "								and upper(u.Nombre) not like '%SOCIEDAD%'"
    + "								and upper(u.ApePaterno) not like '%ASOCIACI_N%'"
    + "								and upper(u.ApePaterno) not like '%SOCIEDAD%'"
    + "								and upper(u.ApeMaterno) not like '%ASOCIACI_N%'"
    + "								and upper(u.ApeMaterno) not like '%SOCIEDAD%'"
    + "								and upper(u.Aclaracion) not like '%ASOCIACI_N%'"
    + "								and upper(u.Aclaracion) not like '%SOCIEDAD%'"
    + "								and upper(u.Nombre) not regexp '^A[.,; ]C[.,; ]'"
    + "								and upper(u.Nombre) not regexp ' A[.,; ]C[.,; ] '"
    + "								and upper(u.Nombre) not regexp 'A[.,; ]C[.,; ]$'"
    + "								and upper(u.Nombre) not regexp '^A[.,; ]R[.,; ]'"
    + "								and upper(u.Nombre) not regexp ' A[.,; ]R[.,; ] '"
    + "								and upper(u.Nombre) not regexp 'A[.,; ]R[.,; ]$'"
    + "								and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]'"
    + "								and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ] '"
    + "								and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]$'"
    + "								and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "								and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "								and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "								and upper(u.Nombre) not regexp '^C[.,; ]V[.,; ]'"
    + "								and upper(u.Nombre) not regexp ' C[.,; ]V[.,; ] '"
    + "								and upper(u.Nombre) not regexp 'C[.,; ]V[.,; ]$'"
    + "								and upper(u.Nombre) not regexp '^R[.,; ]L[.,; ]'"
    + "								and upper(u.Nombre) not regexp ' R[.,; ]L[.,; ] '"
    + "								and upper(u.Nombre) not regexp 'R[.,; ]L[.,; ]$'"
    + "								and upper(u.Nombre) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "								and upper(u.Nombre) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "								and upper(u.Nombre) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "								and upper(u.ApePaterno) not regexp '^A[.,; ]C[.,; ]'"
    + "								and upper(u.ApePaterno) not regexp ' A[.,; ]C[.,; ] '"
    + "								and upper(u.ApePaterno) not regexp 'A[.,; ]C[.,; ]$'"
    + "								and upper(u.ApePaterno) not regexp '^A[.,; ]R[.,; ]'"
    + "								and upper(u.ApePaterno) not regexp ' A[.,; ]R[.,; ] '"
    + "								and upper(u.ApePaterno) not regexp 'A[.,; ]R[.,; ]$'"
    + "								and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]'"
    + "								and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ] '"
    + "								and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]$'"
    + "								and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "								and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "								and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "								and upper(u.ApePaterno) not regexp '^C[.,; ]V[.,; ]'"
    + "								and upper(u.ApePaterno) not regexp ' C[.,; ]V[.,; ] '"
    + "								and upper(u.ApePaterno) not regexp 'C[.,; ]V[.,; ]$'"
    + "								and upper(u.ApePaterno) not regexp '^R[.,; ]L[.,; ]'"
    + "								and upper(u.ApePaterno) not regexp ' R[.,; ]L[.,; ] '"
    + "								and upper(u.ApePaterno) not regexp 'R[.,; ]L[.,; ]$'"
    + "								and upper(u.ApePaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "								and upper(u.ApePaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "								and upper(u.ApePaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "								and upper(u.ApeMaterno) not regexp '^A[.,; ]C[.,; ]'"
    + "								and upper(u.ApeMaterno) not regexp ' A[.,; ]C[.,; ] '"
    + "								and upper(u.ApeMaterno) not regexp 'A[.,; ]C[.,; ]$'"
    + "								and upper(u.ApeMaterno) not regexp '^A[.,; ]R[.,; ]'"
    + "								and upper(u.ApeMaterno) not regexp ' A[.,; ]R[.,; ] '"
    + "								and upper(u.ApeMaterno) not regexp 'A[.,; ]R[.,; ]$'"
    + "								and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]'"
    + "								and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ] '"
    + "								and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]$'"
    + "								and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "								and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "								and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "								and upper(u.ApeMaterno) not regexp '^C[.,; ]V[.,; ]'"
    + "								and upper(u.ApeMaterno) not regexp ' C[.,; ]V[.,; ] '"
    + "								and upper(u.ApeMaterno) not regexp 'C[.,; ]V[.,; ]$'"
    + "								and upper(u.ApeMaterno) not regexp '^R[.,; ]L[.,; ]'"
    + "								and upper(u.ApeMaterno) not regexp ' R[.,; ]L[.,; ] '"
    + "								and upper(u.ApeMaterno) not regexp 'R[.,; ]L[.,; ]$'"
    + "								and upper(u.ApeMaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "								and upper(u.ApeMaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "								and upper(u.ApeMaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "								and upper(u.Aclaracion) not regexp '^A[.,; ]C[.,; ]'"
    + "								and upper(u.Aclaracion) not regexp ' A[.,; ]C[.,; ] '"
    + "								and upper(u.Aclaracion) not regexp 'A[.,; ]C[.,; ]$'"
    + "								and upper(u.Aclaracion) not regexp '^A[.,; ]R[.,; ]'"
    + "								and upper(u.Aclaracion) not regexp ' A[.,; ]R[.,; ] '"
    + "								and upper(u.Aclaracion) not regexp 'A[.,; ]R[.,; ]$'"
    + "								and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]'"
    + "								and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ] '"
    + "								and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]$'"
    + "								and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "								and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "								and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "								and upper(u.Aclaracion) not regexp '^C[.,; ]V[.,; ]'"
    + "								and upper(u.Aclaracion) not regexp ' C[.,; ]V[.,; ] '"
    + "								and upper(u.Aclaracion) not regexp 'C[.,; ]V[.,; ]$'"
    + "								and upper(u.Aclaracion) not regexp '^R[.,; ]L[.,; ]'"
    + "								and upper(u.Aclaracion) not regexp ' R[.,; ]L[.,; ] '"
    + "								and upper(u.Aclaracion) not regexp 'R[.,; ]L[.,; ]$'"
    + "								and upper(u.Aclaracion) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "								and upper(u.Aclaracion) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "								and upper(u.Aclaracion) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$')) then 'Física' "
    + "						else 'N/Identificado' end as 'tPersona'"
    + "				from	htsj_mediacion.tblusuario as u) as us on e.CveExp = us.CveExp"
    + "				where	 FechaReg between '" + fechaPeriodoInicio + "' and '" + fechaPeriodoFin + "' and e.idCentroM in (" + idsCentrosMediacionConsulta + ")"
    + "				and n.Descripcion not like 'JR%'"
    + "				group by naturaleza ORDER BY 1 ASC";
            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["bd67"]);
                con.Open();
                cmd = new MySqlCommand(consulta, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(resultado);
                tablaMediacion3.DataSource = dt;
                tablaMediacion3.DataBind();
                rep.Titulo = "Número de personas solicitantes a sesiones de Mediación";
                rep.Tabla = dt;
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
            return rep;
        }


        /// <summary>
        /// Número de personas invitadas a sesiones de mediación
        /// </summary>
        public ModeloTablaReporte consultarMediacionPersonasInvitadasSesiones()
        {
            ModeloTablaReporte rep = new ModeloTablaReporte();

            String consulta = "select n.Descripcion as Naturaleza,"
    + "		count(distinct (case when us.tipo = 'I' and us.Sexo = 'H' and us.tPersona = 'Física' then us.idUsu end)) as 'Total Hombres',"
    + "		count(distinct (case when us.tipo = 'I' and us.Sexo = 'M' and us.tPersona = 'Física' then us.idUsu end)) as 'Total Mujeres',"
    + "		count(distinct (case when us.tipo = 'I' and us.tPersona = 'Moral' then us.idUsu end)) as 'Moral',"
    + "		count(distinct (case when us.tipo = 'I' and us.tPersona = 'Estado' then us.idUsu end)) as 'N/Identificado',"
    + "		count(distinct (case when us.tipo = 'I' then us.idUsu end)) as 'Total'"
    + "	from	htsj_mediacion.tblexpediente as e"
    + "	left join htsj_mediacion.tblnaturaleza as n on e.idNat = n.idNat"
    + "	left join htsj_mediacion.tbltipoexpediente as te on e.idTipoExpediente = te.idTipoExpediente"
    + "	left join (select"
    + "					u.idUsu,"
    + "					u.CveExp,"
    + "					u.Tipo,"
    + "					u.Nombre,"
    + "					u.ApePaterno,"
    + "					u.ApeMaterno,"
    + "					u.Aclaracion,"
    + "					u.Sexo,"
    + "					u.Edad,"
    + "					case "
    + "						when ((upper(u.Nombre) like '%AYUNTAMIENTO%'"
    + "							or upper(u.Nombre) like '%MUNICIPIO%'"
    + "							or upper(u.Nombre) like '%ESTADO%'"
    + "							or upper(u.Nombre) like '%ESTATAL%'"
    + "							or upper(u.Nombre) like '%FEDERAL%'"
    + "							or upper(u.Nombre) like '%PROCURADOR%'"
    + "							or upper(u.Nombre) like '%F_SCAL%'"
    + "							or upper(u.Nombre) like '%GOBIERNO%'"
    + "							or upper(u.Nombre) like '%SECRETARIO%'"
    + "							or upper(u.Nombre) like '%SECRETAR_A%'"
    + "							or upper(u.Nombre) like '%INSTITUTO%'"
    + "							or upper(u.Nombre) like '%SALUD%'"
    + "							or upper(u.Nombre) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.Nombre) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.Nombre) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'"
    + "							or upper(u.ApePaterno) like '%AYUNTAMIENTO%'"
    + "							or upper(u.ApePaterno) like '%MUNICIPIO%'"
    + "							or upper(u.ApePaterno) like '%ESTADO%'"
    + "							or upper(u.ApePaterno) like '%ESTATAL%'"
    + "							or upper(u.ApePaterno) like '%FEDERAL%'"
    + "							or upper(u.ApePaterno) like '%PROCURADOR%'"
    + "							or upper(u.ApePaterno) like '%F_SCAL%'"
    + "							or upper(u.ApePaterno) like '%GOBIERNO%'"
    + "							or upper(u.ApePaterno) like '%SECRETARIO%'"
    + "							or upper(u.ApePaterno) like '%SECRETAR_A%'"
    + "							or upper(u.ApePaterno) like '%INSTITUTO%'"
    + "							or upper(u.ApePaterno) like '%SALUD%'"
    + "							or upper(u.ApePaterno) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.ApePaterno) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'"
    + "							or upper(u.ApeMaterno) like '%AYUNTAMIENTO%'"
    + "							or upper(u.ApeMaterno) like '%MUNICIPIO%'"
    + "							or upper(u.ApeMaterno) like '%ESTADO%'"
    + "							or upper(u.ApeMaterno) like '%ESTATAL%'"
    + "							or upper(u.ApeMaterno) like '%FEDERAL%'"
    + "							or upper(u.ApeMaterno) like '%PROCURADOR%'"
    + "							or upper(u.ApeMaterno) like '%F_SCAL%'"
    + "							or upper(u.ApeMaterno) like '%GOBIERNO%'"
    + "							or upper(u.ApeMaterno) like '%SECRETARIO%'"
    + "							or upper(u.ApeMaterno) like '%SECRETAR_A%'"
    + "							or upper(u.ApeMaterno) like '%INSTITUTO%'"
    + "							or upper(u.ApeMaterno) like '%SALUD%'"
    + "							or upper(u.ApeMaterno) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'"
    + "							or upper(u.Aclaracion) like '%AYUNTAMIENTO%'"
    + "							or upper(u.Aclaracion) like '%MUNICIPIO%'"
    + "							or upper(u.Aclaracion) like '%ESTADO%'"
    + "							or upper(u.Aclaracion) like '%ESTATAL%'"
    + "							or upper(u.Aclaracion) like '%FEDERAL%'"
    + "							or upper(u.Aclaracion) like '%PROCURADOR%'"
    + "							or upper(u.Aclaracion) like '%F_SCAL%'"
    + "							or upper(u.Aclaracion) like '%GOBIERNO%'"
    + "							or upper(u.Aclaracion) like '%SECRETARIO%'"
    + "							or upper(u.Aclaracion) like '%SECRETAR_A%'"
    + "							or upper(u.Aclaracion) like '%INSTITUTO%'"
    + "							or upper(u.Aclaracion) like '%SALUD%'"
    + "							or upper(u.Aclaracion) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.Aclaracion) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' )"
    + "							and upper(u.Nombre) not like '%ASOCIACI_N%'"
    + "							and upper(u.Nombre) not like '%SOCIEDAD%'"
    + "							and upper(u.ApePaterno) not like '%ASOCIACI_N%'"
    + "							and upper(u.ApePaterno) not like '%SOCIEDAD%'"
    + "							and upper(u.ApeMaterno) not like '%ASOCIACI_N%'"
    + "							and upper(u.ApeMaterno) not like '%SOCIEDAD%'"
    + "							and upper(u.Aclaracion) not like '%ASOCIACI_N%'"
    + "							and upper(u.Aclaracion) not like '%SOCIEDAD%'"
    + "							and upper(u.Nombre) not regexp '^A[.,; ]C[.,; ]'"
    + "							and upper(u.Nombre) not regexp ' A[.,; ]C[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'A[.,; ]C[.,; ]$'"
    + "							and upper(u.Nombre) not regexp '^A[.,; ]R[.,; ]'"
    + "							and upper(u.Nombre) not regexp ' A[.,; ]R[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'A[.,; ]R[.,; ]$'"
    + "							and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]'"
    + "							and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]$'"
    + "							and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							and upper(u.Nombre) not regexp '^C[.,; ]V[.,; ]'"
    + "							and upper(u.Nombre) not regexp ' C[.,; ]V[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'C[.,; ]V[.,; ]$'"
    + "							and upper(u.Nombre) not regexp '^R[.,; ]L[.,; ]'"
    + "							and upper(u.Nombre) not regexp ' R[.,; ]L[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'R[.,; ]L[.,; ]$'"
    + "							and upper(u.Nombre) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							and upper(u.Nombre) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "							and upper(u.ApePaterno) not regexp '^A[.,; ]C[.,; ]'"
    + "							and upper(u.ApePaterno) not regexp ' A[.,; ]C[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'A[.,; ]C[.,; ]$'"
    + "							and upper(u.ApePaterno) not regexp '^A[.,; ]R[.,; ]'"
    + "							and upper(u.ApePaterno) not regexp ' A[.,; ]R[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'A[.,; ]R[.,; ]$'"
    + "							and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]'"
    + "							and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]$'"
    + "							and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							and upper(u.ApePaterno) not regexp '^C[.,; ]V[.,; ]'"
    + "							and upper(u.ApePaterno) not regexp ' C[.,; ]V[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'C[.,; ]V[.,; ]$'"
    + "							and upper(u.ApePaterno) not regexp '^R[.,; ]L[.,; ]'"
    + "							and upper(u.ApePaterno) not regexp ' R[.,; ]L[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'R[.,; ]L[.,; ]$'"
    + "							and upper(u.ApePaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							and upper(u.ApePaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "							and upper(u.ApeMaterno) not regexp '^A[.,; ]C[.,; ]'"
    + "							and upper(u.ApeMaterno) not regexp ' A[.,; ]C[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'A[.,; ]C[.,; ]$'"
    + "							and upper(u.ApeMaterno) not regexp '^A[.,; ]R[.,; ]'"
    + "							and upper(u.ApeMaterno) not regexp ' A[.,; ]R[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'A[.,; ]R[.,; ]$'"
    + "							and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]'"
    + "							and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]$'"
    + "							and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							and upper(u.ApeMaterno) not regexp '^C[.,; ]V[.,; ]'"
    + "							and upper(u.ApeMaterno) not regexp ' C[.,; ]V[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'C[.,; ]V[.,; ]$'"
    + "							and upper(u.ApeMaterno) not regexp '^R[.,; ]L[.,; ]'"
    + "							and upper(u.ApeMaterno) not regexp ' R[.,; ]L[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'R[.,; ]L[.,; ]$'"
    + "							and upper(u.ApeMaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							and upper(u.ApeMaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "							and upper(u.Aclaracion) not regexp '^A[.,; ]C[.,; ]'"
    + "							and upper(u.Aclaracion) not regexp ' A[.,; ]C[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'A[.,; ]C[.,; ]$'"
    + "							and upper(u.Aclaracion) not regexp '^A[.,; ]R[.,; ]'"
    + "							and upper(u.Aclaracion) not regexp ' A[.,; ]R[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'A[.,; ]R[.,; ]$'"
    + "							and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]'"
    + "							and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]$'"
    + "							and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							and upper(u.Aclaracion) not regexp '^C[.,; ]V[.,; ]'"
    + "							and upper(u.Aclaracion) not regexp ' C[.,; ]V[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'C[.,; ]V[.,; ]$'"
    + "							and upper(u.Aclaracion) not regexp '^R[.,; ]L[.,; ]'"
    + "							and upper(u.Aclaracion) not regexp ' R[.,; ]L[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'R[.,; ]L[.,; ]$'"
    + "							and upper(u.Aclaracion) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							and upper(u.Aclaracion) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$') then 'Estado'"
    + "						when (upper(u.Nombre) like '%REPRE_ENTANTE%'"
    + "							or upper(u.Nombre) like '%APODERAD_%'"
    + "							or upper(u.Nombre) like '%LEGAL%'"
    + "							or upper(u.ApePaterno) like '%REPRE_ENTANTE%'"
    + "							or upper(u.ApePaterno) like '%APODERAD_%'"
    + "							or upper(u.ApePaterno) like '%LEGAL%'"
    + "							or upper(u.ApeMaterno) like '%REPRE_ENTANTE%'"
    + "							or upper(u.ApeMaterno) like '%APODERAD_%'"
    + "							or upper(u.ApeMaterno) like '%LEGAL%'"
    + "							or upper(u.Aclaracion) like '%REPRE_ENTANTE%'"
    + "							or upper(u.Aclaracion) like '%APODERAD_%'"
    + "							or upper(u.Aclaracion) like '%LEGAL%')"
    + "							and upper(u.Nombre) not like '%MENOR%'"
    + "							and upper(u.Nombre) not like '%V_CTIMA%'"
    + "							and upper(u.Nombre) not like '%HIJ_%'"
    + "							and upper(u.ApePaterno) not like '%MENOR%'"
    + "							and upper(u.ApePaterno) not like '%V_CTIMA%'"
    + "							and upper(u.ApePaterno) not like '%HIJ_%'"
    + "							and upper(u.ApeMaterno) not like '%MENOR%'"
    + "							and upper(u.ApeMaterno) not like '%V_CTIMA%'"
    + "							and upper(u.ApeMaterno) not like '%HIJ_%'"
    + "							and upper(u.Aclaracion) not like '%MENOR%'"
    + "							and upper(u.Aclaracion) not like '%V_CTIMA%'"
    + "							and upper(u.Aclaracion) not like '%HIJ_%'"
    + "							and upper(u.Nombre) not like '%AYUNTAMIENTO%'"
    + "							and upper(u.Nombre) not like '%MUNICIPIO%'"
    + "							and upper(u.Nombre) not like '%ESTADO%'"
    + "							and upper(u.Nombre) not like '%ESTATAL%'"
    + "							and upper(u.Nombre) not like '%FEDERAL%'"
    + "							and upper(u.Nombre) not like '%PROCURADOR%'"
    + "							and upper(u.Nombre) not like '%F_SCAL%'"
    + "							and upper(u.Nombre) not like '%GOBIERNO%'"
    + "							and upper(u.Nombre) not like '%SECRETARIO%'"
    + "							and upper(u.Nombre) not like '%SECRETAR_A%'"
    + "							and upper(u.Nombre) not like '%INSTITUTO%'"
    + "							and upper(u.Nombre) not like '%SALUD%'"
    + "							and upper(u.Nombre) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.Nombre) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.Nombre) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' "
    + "							and upper(u.ApePaterno) not like '%AYUNTAMIENTO%'"
    + "							and upper(u.ApePaterno) not like '%MUNICIPIO%'"
    + "							and upper(u.ApePaterno) not like '%ESTADO%'"
    + "							and upper(u.ApePaterno) not like '%ESTATAL%'"
    + "							and upper(u.ApePaterno) not like '%FEDERAL%'"
    + "							and upper(u.ApePaterno) not like '%PROCURADOR%'"
    + "							and upper(u.ApePaterno) not like '%F_SCAL%'"
    + "							and upper(u.ApePaterno) not like '%GOBIERNO%'"
    + "							and upper(u.ApePaterno) not like '%SECRETARIO%'"
    + "							and upper(u.ApePaterno) not like '%SECRETAR_A%'"
    + "							and upper(u.ApePaterno) not like '%INSTITUTO%'"
    + "							and upper(u.ApePaterno) not like '%SALUD%'"
    + "							and upper(u.ApePaterno) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.ApePaterno) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'"
    + "							and upper(u.ApeMaterno) not like '%AYUNTAMIENTO%'"
    + "							and upper(u.ApeMaterno) not like '%MUNICIPIO%'"
    + "							and upper(u.ApeMaterno) not like '%ESTADO%'"
    + "							and upper(u.ApeMaterno) not like '%ESTATAL%'"
    + "							and upper(u.ApeMaterno) not like '%FEDERAL%'"
    + "							and upper(u.ApeMaterno) not like '%PROCURADOR%'"
    + "							and upper(u.ApeMaterno) not like '%F_SCAL%'"
    + "							and upper(u.ApeMaterno) not like '%GOBIERNO%'"
    + "							and upper(u.ApeMaterno) not like '%SECRETARIO%'"
    + "							and upper(u.ApeMaterno) not like '%SECRETAR_A%'"
    + "							and upper(u.ApeMaterno) not like '%INSTITUTO%'"
    + "							and upper(u.ApeMaterno) not like '%SALUD%'"
    + "							and upper(u.ApeMaterno) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.ApeMaterno) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'"
    + "							and upper(u.Aclaracion) not like '%AYUNTAMIENTO%'"
    + "							and upper(u.Aclaracion) not like '%MUNICIPIO%'"
    + "							and upper(u.Aclaracion) not like '%ESTADO%'"
    + "							and upper(u.Aclaracion) not like '%ESTATAL%'"
    + "							and upper(u.Aclaracion) not like '%FEDERAL%'"
    + "							and upper(u.Aclaracion) not like '%PROCURADOR%'"
    + "							and upper(u.Aclaracion) not like '%F_SCAL%'"
    + "							and upper(u.Aclaracion) not like '%GOBIERNO%'"
    + "							and upper(u.Aclaracion) not like '%SECRETARIO%'"
    + "							and upper(u.Aclaracion) not like '%SECRETAR_A%'"
    + "							and upper(u.Aclaracion) not like '%INSTITUTO%'"
    + "							and upper(u.Aclaracion) not like '%SALUD%'"
    + "							and upper(u.Aclaracion) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '"
    + "							and upper(u.Aclaracion) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' then 'Moral'"
    + "						when (upper(u.Nombre) like '%ASOCIACI_N%'"
    + "							or upper(u.Nombre) like '%SOCIEDAD%'"
    + "							or upper(u.ApePaterno) like '%ASOCIACI_N%'"
    + "							or upper(u.ApePaterno) like '%SOCIEDAD%'"
    + "							or upper(u.ApeMaterno) like '%ASOCIACI_N%'"
    + "							or upper(u.ApeMaterno) like '%SOCIEDAD%'"
    + "							or upper(u.Aclaracion) like '%ASOCIACI_N%'"
    + "							or upper(u.Aclaracion) like '%SOCIEDAD%'"
    + "							or upper(u.Nombre) regexp '^A[.,; ]C[.,; ]'"
    + "							or upper(u.Nombre) regexp ' A[.,; ]C[.,; ] '"
    + "							or upper(u.Nombre) regexp 'A[.,; ]C[.,; ]$'"
    + "							or upper(u.Nombre) regexp '^A[.,; ]R[.,; ]'"
    + "							or upper(u.Nombre) regexp ' A[.,; ]R[.,; ] '"
    + "							or upper(u.Nombre) regexp 'A[.,; ]R[.,; ]$'"
    + "							or upper(u.Nombre) regexp '^S[.,; ]A[.,; ]'"
    + "							or upper(u.Nombre) regexp ' S[.,; ]A[.,; ] '"
    + "							or upper(u.Nombre) regexp 'S[.,; ]A[.,; ]$'"
    + "							or upper(u.Nombre) regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							or upper(u.Nombre) regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							or upper(u.Nombre) regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							or upper(u.Nombre) regexp '^C[.,; ]V[.,; ]'"
    + "							or upper(u.Nombre) regexp ' C[.,; ]V[.,; ] '"
    + "							or upper(u.Nombre) regexp 'C[.,; ]V[.,; ]$'"
    + "							or upper(u.Nombre) regexp '^R[.,; ]L[.,; ]'"
    + "							or upper(u.Nombre) regexp ' R[.,; ]L[.,; ] '"
    + "							or upper(u.Nombre) regexp 'R[.,; ]L[.,; ]$'"
    + "							or upper(u.Nombre) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							or upper(u.Nombre) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							or upper(u.Nombre) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "							or upper(u.ApePaterno) regexp '^A[.,; ]C[.,; ]'"
    + "							or upper(u.ApePaterno) regexp ' A[.,; ]C[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'A[.,; ]C[.,; ]$'"
    + "							or upper(u.ApePaterno) regexp '^A[.,; ]R[.,; ]'"
    + "							or upper(u.ApePaterno) regexp ' A[.,; ]R[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'A[.,; ]R[.,; ]$'"
    + "							or upper(u.ApePaterno) regexp '^S[.,; ]A[.,; ]'"
    + "							or upper(u.ApePaterno) regexp ' S[.,; ]A[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'S[.,; ]A[.,; ]$'"
    + "							or upper(u.ApePaterno) regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							or upper(u.ApePaterno) regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							or upper(u.ApePaterno) regexp '^C[.,; ]V[.,; ]'"
    + "							or upper(u.ApePaterno) regexp ' C[.,; ]V[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'C[.,; ]V[.,; ]$'"
    + "							or upper(u.ApePaterno) regexp '^R[.,; ]L[.,; ]'"
    + "							or upper(u.ApePaterno) regexp ' R[.,; ]L[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'R[.,; ]L[.,; ]$'"
    + "							or upper(u.ApePaterno) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							or upper(u.ApePaterno) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							or upper(u.ApePaterno) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "							or upper(u.ApeMaterno) regexp '^A[.,; ]C[.,; ]'"
    + "							or upper(u.ApeMaterno) regexp ' A[.,; ]C[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'A[.,; ]C[.,; ]$'"
    + "							or upper(u.ApeMaterno) regexp '^A[.,; ]R[.,; ]'"
    + "							or upper(u.ApeMaterno) regexp ' A[.,; ]R[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'A[.,; ]R[.,; ]$'"
    + "							or upper(u.ApeMaterno) regexp '^S[.,; ]A[.,; ]'"
    + "							or upper(u.ApeMaterno) regexp ' S[.,; ]A[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'S[.,; ]A[.,; ]$'"
    + "							or upper(u.ApeMaterno) regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							or upper(u.ApeMaterno) regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							or upper(u.ApeMaterno) regexp '^C[.,; ]V[.,; ]'"
    + "							or upper(u.ApeMaterno) regexp ' C[.,; ]V[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'C[.,; ]V[.,; ]$'"
    + "							or upper(u.ApeMaterno) regexp '^R[.,; ]L[.,; ]'"
    + "							or upper(u.ApeMaterno) regexp ' R[.,; ]L[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'R[.,; ]L[.,; ]$'"
    + "							or upper(u.ApeMaterno) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							or upper(u.ApeMaterno) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							or upper(u.ApeMaterno) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "							or upper(u.Aclaracion) regexp '^A[.,; ]C[.,; ]'"
    + "							or upper(u.Aclaracion) regexp ' A[.,; ]C[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'A[.,; ]C[.,; ]$'"
    + "							or upper(u.Aclaracion) regexp '^A[.,; ]R[.,; ]'"
    + "							or upper(u.Aclaracion) regexp ' A[.,; ]R[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'A[.,; ]R[.,; ]$'"
    + "							or upper(u.Aclaracion) regexp '^S[.,; ]A[.,; ]'"
    + "							or upper(u.Aclaracion) regexp ' S[.,; ]A[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'S[.,; ]A[.,; ]$'"
    + "							or upper(u.Aclaracion) regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "							or upper(u.Aclaracion) regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "							or upper(u.Aclaracion) regexp '^C[.,; ]V[.,; ]'"
    + "							or upper(u.Aclaracion) regexp ' C[.,; ]V[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'C[.,; ]V[.,; ]$'"
    + "							or upper(u.Aclaracion) regexp '^R[.,; ]L[.,; ]'"
    + "							or upper(u.Aclaracion) regexp ' R[.,; ]L[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'R[.,; ]L[.,; ]$'"
    + "							or upper(u.Aclaracion) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "							or upper(u.Aclaracion) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "							or upper(u.Aclaracion) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$') then 'Moral' "
    + "						when (u.Sexo in ('H','M')"
    + "							and (upper(u.Nombre) like '%MENOR%'"
    + "								or upper(u.Nombre) like '%V_CTIMA%'"
    + "								or upper(u.Nombre) like '%HIJ_%'"
    + "								or upper(u.ApePaterno) like '%MENOR%'"
    + "								or upper(u.ApePaterno) like '%V_CTIMA%'"
    + "								or upper(u.ApePaterno) like '%HIJ_%'"
    + "								or upper(u.ApeMaterno) like '%MENOR%'"
    + "								or upper(u.ApeMaterno) like '%V_CTIMA%'"
    + "								or upper(u.ApeMaterno) like '%HIJ_%'"
    + "								or upper(u.Aclaracion) like '%MENOR%'"
    + "								or upper(u.Aclaracion) like '%V_CTIMA%'"
    + "								or upper(u.Aclaracion) like '%HIJ_%')) then 'Física' "
    + "						when (u.Sexo in ('H','M')"
    + "							and (upper(u.Nombre) not like '%ASOCIACI_N%'"
    + "								and upper(u.Nombre) not like '%SOCIEDAD%'"
    + "								and upper(u.ApePaterno) not like '%ASOCIACI_N%'"
    + "								and upper(u.ApePaterno) not like '%SOCIEDAD%'"
    + "								and upper(u.ApeMaterno) not like '%ASOCIACI_N%'"
    + "								and upper(u.ApeMaterno) not like '%SOCIEDAD%'"
    + "								and upper(u.Aclaracion) not like '%ASOCIACI_N%'"
    + "								and upper(u.Aclaracion) not like '%SOCIEDAD%'"
    + "								and upper(u.Nombre) not regexp '^A[.,; ]C[.,; ]'"
    + "								and upper(u.Nombre) not regexp ' A[.,; ]C[.,; ] '"
    + "								and upper(u.Nombre) not regexp 'A[.,; ]C[.,; ]$'"
    + "								and upper(u.Nombre) not regexp '^A[.,; ]R[.,; ]'"
    + "								and upper(u.Nombre) not regexp ' A[.,; ]R[.,; ] '"
    + "								and upper(u.Nombre) not regexp 'A[.,; ]R[.,; ]$'"
    + "								and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]'"
    + "								and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ] '"
    + "								and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]$'"
    + "								and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "								and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "								and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "								and upper(u.Nombre) not regexp '^C[.,; ]V[.,; ]'"
    + "								and upper(u.Nombre) not regexp ' C[.,; ]V[.,; ] '"
    + "								and upper(u.Nombre) not regexp 'C[.,; ]V[.,; ]$'"
    + "								and upper(u.Nombre) not regexp '^R[.,; ]L[.,; ]'"
    + "								and upper(u.Nombre) not regexp ' R[.,; ]L[.,; ] '"
    + "								and upper(u.Nombre) not regexp 'R[.,; ]L[.,; ]$'"
    + "								and upper(u.Nombre) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "								and upper(u.Nombre) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "								and upper(u.Nombre) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "								and upper(u.ApePaterno) not regexp '^A[.,; ]C[.,; ]'"
    + "								and upper(u.ApePaterno) not regexp ' A[.,; ]C[.,; ] '"
    + "								and upper(u.ApePaterno) not regexp 'A[.,; ]C[.,; ]$'"
    + "								and upper(u.ApePaterno) not regexp '^A[.,; ]R[.,; ]'"
    + "								and upper(u.ApePaterno) not regexp ' A[.,; ]R[.,; ] '"
    + "								and upper(u.ApePaterno) not regexp 'A[.,; ]R[.,; ]$'"
    + "								and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]'"
    + "								and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ] '"
    + "								and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]$'"
    + "								and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "								and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "								and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "								and upper(u.ApePaterno) not regexp '^C[.,; ]V[.,; ]'"
    + "								and upper(u.ApePaterno) not regexp ' C[.,; ]V[.,; ] '"
    + "								and upper(u.ApePaterno) not regexp 'C[.,; ]V[.,; ]$'"
    + "								and upper(u.ApePaterno) not regexp '^R[.,; ]L[.,; ]'"
    + "								and upper(u.ApePaterno) not regexp ' R[.,; ]L[.,; ] '"
    + "								and upper(u.ApePaterno) not regexp 'R[.,; ]L[.,; ]$'"
    + "								and upper(u.ApePaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "								and upper(u.ApePaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "								and upper(u.ApePaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "								and upper(u.ApeMaterno) not regexp '^A[.,; ]C[.,; ]'"
    + "								and upper(u.ApeMaterno) not regexp ' A[.,; ]C[.,; ] '"
    + "								and upper(u.ApeMaterno) not regexp 'A[.,; ]C[.,; ]$'"
    + "								and upper(u.ApeMaterno) not regexp '^A[.,; ]R[.,; ]'"
    + "								and upper(u.ApeMaterno) not regexp ' A[.,; ]R[.,; ] '"
    + "								and upper(u.ApeMaterno) not regexp 'A[.,; ]R[.,; ]$'"
    + "								and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]'"
    + "								and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ] '"
    + "								and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]$'"
    + "								and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "								and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "								and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "								and upper(u.ApeMaterno) not regexp '^C[.,; ]V[.,; ]'"
    + "								and upper(u.ApeMaterno) not regexp ' C[.,; ]V[.,; ] '"
    + "								and upper(u.ApeMaterno) not regexp 'C[.,; ]V[.,; ]$'"
    + "								and upper(u.ApeMaterno) not regexp '^R[.,; ]L[.,; ]'"
    + "								and upper(u.ApeMaterno) not regexp ' R[.,; ]L[.,; ] '"
    + "								and upper(u.ApeMaterno) not regexp 'R[.,; ]L[.,; ]$'"
    + "								and upper(u.ApeMaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "								and upper(u.ApeMaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "								and upper(u.ApeMaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'"
    + "								and upper(u.Aclaracion) not regexp '^A[.,; ]C[.,; ]'"
    + "								and upper(u.Aclaracion) not regexp ' A[.,; ]C[.,; ] '"
    + "								and upper(u.Aclaracion) not regexp 'A[.,; ]C[.,; ]$'"
    + "								and upper(u.Aclaracion) not regexp '^A[.,; ]R[.,; ]'"
    + "								and upper(u.Aclaracion) not regexp ' A[.,; ]R[.,; ] '"
    + "								and upper(u.Aclaracion) not regexp 'A[.,; ]R[.,; ]$'"
    + "								and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]'"
    + "								and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ] '"
    + "								and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]$'"
    + "								and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]B[.,; ]'"
    + "								and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ]B[.,; ] '"
    + "								and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'"
    + "								and upper(u.Aclaracion) not regexp '^C[.,; ]V[.,; ]'"
    + "								and upper(u.Aclaracion) not regexp ' C[.,; ]V[.,; ] '"
    + "								and upper(u.Aclaracion) not regexp 'C[.,; ]V[.,; ]$'"
    + "								and upper(u.Aclaracion) not regexp '^R[.,; ]L[.,; ]'"
    + "								and upper(u.Aclaracion) not regexp ' R[.,; ]L[.,; ] '"
    + "								and upper(u.Aclaracion) not regexp 'R[.,; ]L[.,; ]$'"
    + "								and upper(u.Aclaracion) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'"
    + "								and upper(u.Aclaracion) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '"
    + "								and upper(u.Aclaracion) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$')) then 'Física' "
    + "						else 'N/Identificado' end as 'tPersona'"
    + "				from	htsj_mediacion.tblusuario as u) as us on e.CveExp = us.CveExp"
    + "				where	FechaReg between ' " + fechaPeriodoInicio + "' and '" + fechaPeriodoFin + "' and e.idCentroM in (" + idsCentrosMediacionConsulta + ")" 
    + "				and n.Descripcion not like 'JR%'"
    + "				group by naturaleza ORDER BY 1 ASC;";
            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["bd67"]);
                con.Open();
                cmd = new MySqlCommand(consulta, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(resultado);
                tablaMediacion4.DataSource = dt;
                tablaMediacion4.DataBind();
                rep.Titulo = "Número de personas invitadas a sesiones de Mediación";
                rep.Tabla = dt;

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
            return rep;
        }


        /// <summary>
        /// Expedientes concluidos, por naturaleza
        /// </summary>
        public ModeloTablaReporte consultarMediacionConcluidosPorNaturaleza()
        {
             ModeloTablaReporte rep = new ModeloTablaReporte();
             String consulta = "select    n.Descripcion as 'Naturaleza'," +
                "        count(distinct e.CveExp) as 'Número de Expedientes'" +
                " from htsj_mediacion.tblexpediente as e" +
                "         left join htsj_mediacion.tblnaturaleza as n on e.idNat = n.idNat" +
                "         inner join htsj_mediacion.tblcentrom cm on e.idCentroM = cm.idCentroM" +
                "         left join htsj_mediacion.tbltipoexpediente as te on e.idTipoExpediente = te.idTipoExpediente" +
                " where e.Fechater between '" + fechaPeriodoInicio + "' and '" + fechaPeriodoFin + "'  and e.idCentroM in (" + idsCentrosMediacionConsulta + ") AND n.Descripcion not like 'JR %'" +
                " group by    1 order by  n.idNat; ";
            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["bd67"]);
                con.Open();
                cmd = new MySqlCommand(consulta, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(resultado);
                tablaMediacion5.DataSource = dt;
                tablaMediacion5.DataBind();
                rep.Titulo = "Expedientes concluidos, por naturaleza";
                rep.Tabla = dt;

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
            return rep;
        }



        /// <summary>
        /// Expedientes concluidos, por naturaleza y  tipo de solución
        /// </summary>
        public ModeloTablaReporte consultarMediacionConcluidosPorNaturalezaTipoSolucion()
        {
            ModeloTablaReporte rep = new ModeloTablaReporte();
            String consulta = "SELECT     n.Descripcion 'Naturaleza'," +
                "    COUNT(DISTINCT CASE WHEN(e.idTipoTer = 1) THEN e.CveExp END) AS 'Convenio Escrito', " +
                "    COUNT(DISTINCT CASE WHEN(e.idTipoTer = 2) THEN e.CveExp END) AS 'Convenio Verbal'," +
                "    COUNT(DISTINCT CASE WHEN(e.idTipoTer = 3) THEN e.CveExp END) AS 'Negativa a suscribir el convenio'," +
                "    COUNT(DISTINCT CASE WHEN(e.idTipoTer = 4) THEN e.CveExp END) AS 'Decisión del interesado'," +
                "    COUNT(DISTINCT CASE WHEN(e.idTipoTer = 5) THEN e.CveExp END) AS 'Inasistencia o falta de interés'," +
                "    COUNT(DISTINCT CASE WHEN(e.idTipoTer = 6) THEN e.CveExp END) AS 'Por simulación de procedimiento'," +
                "    COUNT(DISTINCT CASE WHEN(e.idTipoTer = 7) THEN e.CveExp END) AS 'Acuerdo Reparatorio'," +
                "    COUNT(DISTINCT CASE WHEN(e.idTipoTer = 8) THEN e.CveExp END) AS 'Pacto Ético Moral'," +
                "    COUNT(DISTINCT CASE WHEN(e.idTipoTer = 9) THEN e.CveExp END) AS 'Otros'" +
                "FROM" +
                "    htsj_mediacion.tblexpediente E INNER JOIN" +
                "    htsj_mediacion.tbltipoter ter ON ter.idTipoTer = e.idTipoTer INNER JOIN" +
                "    htsj_mediacion.tblnaturaleza n ON n.idNat = e.idNat LEFT JOIN" +
                "    htsj_mediacion.tblusuario U ON E.CveExp = U.CveExp" +
                " WHERE" +
                "    u.Activo = 1 and n.Descripcion not like 'JR %'" +
                //"    AND AnioExp = 2023" +
                "    AND FechaTer between '" + fechaPeriodoInicio + "' and '" + fechaPeriodoFin + "'  and e.idCentroM in (" + idsCentrosMediacionConsulta + ")" +
                "    AND e.idTipoTer IS NOT NULL" +
                " GROUP BY(e.idNat)";
            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["bd67"]);
                con.Open();
                cmd = new MySqlCommand(consulta, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(resultado);
                tablaMediacion6.DataSource = dt;
                tablaMediacion6.DataBind();
                rep.Titulo = "Expedientes concluidos, por naturaleza y  tipo de solución";
                rep.Tabla = dt;
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
            return rep;
        }




        /// <summary>
        /// Número de solicitantes por sexo, respecto a la causa de solución
        /// </summary>
        public ModeloTablaReporte consultarMediacionSolicitantesCausaSolucion0()
        {
            ModeloTablaReporte rep = new ModeloTablaReporte();
            String consulta = "select n.Descripcion as Naturaleza,	" 
    + "count(distinct (case when us.tipo = 'S' and us.Sexo = 'H' and us.tPersona = 'Física' then us.idUsu end)) as 'Total Hombres', "
    + "count(distinct (case when us.tipo = 'S' and us.Sexo = 'M' and us.tPersona = 'Física' then us.idUsu end)) as 'Total Mujeres', "
    + "count(distinct (case when us.tipo = 'S' and us.tPersona = 'Moral' then us.idUsu end)) as 'Total Moral', "
    + "count(distinct (case when us.tipo = 'S' and us.tPersona = 'N/Identificado' then us.idUsu end)) as 'Total N/Identificados' ,"
    + "count(distinct (case when us.tipo = 'S' then us.idUsu end)) as 'Total'"

+ " from	htsj_mediacion.tblexpediente as e "
    + "left join htsj_mediacion.tblnaturaleza as n on e.idNat = n.idNat "
    + "inner join htsj_mediacion.tblcentrom cm on e.idCentroM = cm.idCentroM "
    + "left join htsj_mediacion.tbltipoexpediente as te on e.idTipoExpediente = te.idTipoExpediente "
    + "left join htsj_mediacion.tbltipodocprocedencia as td on e.idTipoDocProcedencia = td.idTipoDocProcedencia "
    + "left join (select "
    + "				u.idUsu, "
    + "				u.CveExp, "
    + "				u.Tipo, "
    + "				u.Nombre, "
    + "				u.ApePaterno, "
    + "				u.ApeMaterno, "
    + "				u.Aclaracion, "
    + "				u.Sexo, "
    + "				u.Edad, "
    + "				case  "
    + "					when ((upper(u.Nombre) like '%AYUNTAMIENTO%' "
    + "						or upper(u.Nombre) like '%MUNICIPIO%' "
    + "						or upper(u.Nombre) like '%ESTADO%' "
    + "						or upper(u.Nombre) like '%ESTATAL%' "
    + "						or upper(u.Nombre) like '%FEDERAL%' "
    + "						or upper(u.Nombre) like '%PROCURADOR%' "
    + "						or upper(u.Nombre) like '%F_SCAL%' "
    + "						or upper(u.Nombre) like '%GOBIERNO%' "
    + "						or upper(u.Nombre) like '%SECRETARIO%' "
    + "						or upper(u.Nombre) like '%SECRETAR_A%' "
    + "						or upper(u.Nombre) like '%INSTITUTO%' "
    + "						or upper(u.Nombre) like '%SALUD%' "
    + "						or upper(u.Nombre) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.Nombre) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' "
    + "						or upper(u.ApePaterno) like '%AYUNTAMIENTO%' "
    + "						or upper(u.ApePaterno) like '%MUNICIPIO%' "
    + "						or upper(u.ApePaterno) like '%ESTADO%' "
    + "						or upper(u.ApePaterno) like '%ESTATAL%' "
    + "						or upper(u.ApePaterno) like '%FEDERAL%' "
    + "						or upper(u.ApePaterno) like '%PROCURADOR%' "
    + "						or upper(u.ApePaterno) like '%F_SCAL%' "
    + "						or upper(u.ApePaterno) like '%GOBIERNO%' "
    + "						or upper(u.ApePaterno) like '%SECRETARIO%' "
    + "						or upper(u.ApePaterno) like '%SECRETAR_A%' "
    + "						or upper(u.ApePaterno) like '%INSTITUTO%' "
    + "						or upper(u.ApePaterno) like '%SALUD%' "
    + "						or upper(u.ApePaterno) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' "
    + "						or upper(u.ApeMaterno) like '%AYUNTAMIENTO%' "
    + "						or upper(u.ApeMaterno) like '%MUNICIPIO%' "
    + "						or upper(u.ApeMaterno) like '%ESTADO%' "
    + "						or upper(u.ApeMaterno) like '%ESTATAL%' "
    + "						or upper(u.ApeMaterno) like '%FEDERAL%' "
    + "						or upper(u.ApeMaterno) like '%PROCURADOR%' "
    + "						or upper(u.ApeMaterno) like '%F_SCAL%' "
    + "						or upper(u.ApeMaterno) like '%GOBIERNO%' "
    + "						or upper(u.ApeMaterno) like '%SECRETARIO%' "
    + "						or upper(u.ApeMaterno) like '%SECRETAR_A%' "
    + "						or upper(u.ApeMaterno) like '%INSTITUTO%' "
    + "						or upper(u.ApeMaterno) like '%SALUD%' "
    + "						or upper(u.ApeMaterno) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' "
    + "						or upper(u.Aclaracion) like '%AYUNTAMIENTO%' "
    + "						or upper(u.Aclaracion) like '%MUNICIPIO%' "
    + "						or upper(u.Aclaracion) like '%ESTADO%' "
    + "						or upper(u.Aclaracion) like '%ESTATAL%' "
    + "						or upper(u.Aclaracion) like '%FEDERAL%' "
    + "						or upper(u.Aclaracion) like '%PROCURADOR%' "
    + "						or upper(u.Aclaracion) like '%F_SCAL%' "
    + "						or upper(u.Aclaracion) like '%GOBIERNO%' "
    + "						or upper(u.Aclaracion) like '%SECRETARIO%' "
    + "						or upper(u.Aclaracion) like '%SECRETAR_A%' "
    + "						or upper(u.Aclaracion) like '%INSTITUTO%' "
    + "						or upper(u.Aclaracion) like '%SALUD%' "
    + "						or upper(u.Aclaracion) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' ) "
    + "						and upper(u.Nombre) not like '%ASOCIACI_N%' "
    + "						and upper(u.Nombre) not like '%SOCIEDAD%' "
    + "						and upper(u.ApePaterno) not like '%ASOCIACI_N%' "
    + "						and upper(u.ApePaterno) not like '%SOCIEDAD%' "
    + "						and upper(u.ApeMaterno) not like '%ASOCIACI_N%' "
    + "						and upper(u.ApeMaterno) not like '%SOCIEDAD%' "
    + "						and upper(u.Aclaracion) not like '%ASOCIACI_N%' "
    + "						and upper(u.Aclaracion) not like '%SOCIEDAD%' "
    + "						and upper(u.Nombre) not regexp '^A[.,; ]C[.,; ]' "
    + "						and upper(u.Nombre) not regexp ' A[.,; ]C[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'A[.,; ]C[.,; ]$' "
    + "						and upper(u.Nombre) not regexp '^A[.,; ]R[.,; ]' "
    + "						and upper(u.Nombre) not regexp ' A[.,; ]R[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'A[.,; ]R[.,; ]$' "
    + "						and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]' "
    + "						and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]$' "
    + "						and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						and upper(u.Nombre) not regexp '^C[.,; ]V[.,; ]' "
    + "						and upper(u.Nombre) not regexp ' C[.,; ]V[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'C[.,; ]V[.,; ]$' "
    + "						and upper(u.Nombre) not regexp '^R[.,; ]L[.,; ]' "
    + "						and upper(u.Nombre) not regexp ' R[.,; ]L[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'R[.,; ]L[.,; ]$' "
    + "						and upper(u.Nombre) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						and upper(u.Nombre) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "						and upper(u.ApePaterno) not regexp '^A[.,; ]C[.,; ]' "
    + "						and upper(u.ApePaterno) not regexp ' A[.,; ]C[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'A[.,; ]C[.,; ]$' "
    + "						and upper(u.ApePaterno) not regexp '^A[.,; ]R[.,; ]' "
    + "						and upper(u.ApePaterno) not regexp ' A[.,; ]R[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'A[.,; ]R[.,; ]$' "
    + "						and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]' "
    + "						and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]$' "
    + "						and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						and upper(u.ApePaterno) not regexp '^C[.,; ]V[.,; ]' "
    + "						and upper(u.ApePaterno) not regexp ' C[.,; ]V[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'C[.,; ]V[.,; ]$' "
    + "						and upper(u.ApePaterno) not regexp '^R[.,; ]L[.,; ]' "
    + "						and upper(u.ApePaterno) not regexp ' R[.,; ]L[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'R[.,; ]L[.,; ]$' "
    + "						and upper(u.ApePaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						and upper(u.ApePaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "						and upper(u.ApeMaterno) not regexp '^A[.,; ]C[.,; ]' "
    + "						and upper(u.ApeMaterno) not regexp ' A[.,; ]C[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'A[.,; ]C[.,; ]$' "
    + "						and upper(u.ApeMaterno) not regexp '^A[.,; ]R[.,; ]' "
    + "						and upper(u.ApeMaterno) not regexp ' A[.,; ]R[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'A[.,; ]R[.,; ]$' "
    + "						and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]' "
    + "						and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]$' "
    + "						and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						and upper(u.ApeMaterno) not regexp '^C[.,; ]V[.,; ]' "
    + "						and upper(u.ApeMaterno) not regexp ' C[.,; ]V[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'C[.,; ]V[.,; ]$' "
    + "						and upper(u.ApeMaterno) not regexp '^R[.,; ]L[.,; ]' "
    + "						and upper(u.ApeMaterno) not regexp ' R[.,; ]L[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'R[.,; ]L[.,; ]$' "
    + "						and upper(u.ApeMaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						and upper(u.ApeMaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "						and upper(u.Aclaracion) not regexp '^A[.,; ]C[.,; ]' "
    + "						and upper(u.Aclaracion) not regexp ' A[.,; ]C[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'A[.,; ]C[.,; ]$' "
    + "						and upper(u.Aclaracion) not regexp '^A[.,; ]R[.,; ]' "
    + "						and upper(u.Aclaracion) not regexp ' A[.,; ]R[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'A[.,; ]R[.,; ]$' "
    + "						and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]' "
    + "						and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]$' "
    + "						and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						and upper(u.Aclaracion) not regexp '^C[.,; ]V[.,; ]' "
    + "						and upper(u.Aclaracion) not regexp ' C[.,; ]V[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'C[.,; ]V[.,; ]$' "
    + "						and upper(u.Aclaracion) not regexp '^R[.,; ]L[.,; ]' "
    + "						and upper(u.Aclaracion) not regexp ' R[.,; ]L[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'R[.,; ]L[.,; ]$' "
    + "						and upper(u.Aclaracion) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						and upper(u.Aclaracion) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$') then 'N/Identificado' "
    + "					when (upper(u.Nombre) like '%REPRE_ENTANTE%' "
    + "						or upper(u.Nombre) like '%APODERAD_%' "
    + "						or upper(u.Nombre) like '%LEGAL%' "
    + "						or upper(u.ApePaterno) like '%REPRE_ENTANTE%' "
    + "						or upper(u.ApePaterno) like '%APODERAD_%' "
    + "						or upper(u.ApePaterno) like '%LEGAL%' "
    + "						or upper(u.ApeMaterno) like '%REPRE_ENTANTE%' "
    + "						or upper(u.ApeMaterno) like '%APODERAD_%' "
    + "						or upper(u.ApeMaterno) like '%LEGAL%' "
    + "						or upper(u.Aclaracion) like '%REPRE_ENTANTE%' "
    + "						or upper(u.Aclaracion) like '%APODERAD_%' "
    + "						or upper(u.Aclaracion) like '%LEGAL%') "
    + "						and upper(u.Nombre) not like '%MENOR%' "
    + "						and upper(u.Nombre) not like '%V_CTIMA%' "
    + "						and upper(u.Nombre) not like '%HIJ_%' "
    + "						and upper(u.ApePaterno) not like '%MENOR%' "
    + "						and upper(u.ApePaterno) not like '%V_CTIMA%' "
    + "						and upper(u.ApePaterno) not like '%HIJ_%' "
    + "						and upper(u.ApeMaterno) not like '%MENOR%' "
    + "						and upper(u.ApeMaterno) not like '%V_CTIMA%' "
    + "						and upper(u.ApeMaterno) not like '%HIJ_%' "
    + "						and upper(u.Aclaracion) not like '%MENOR%' "
    + "						and upper(u.Aclaracion) not like '%V_CTIMA%' "
    + "						and upper(u.Aclaracion) not like '%HIJ_%' "
    + "						and upper(u.Nombre) not like '%AYUNTAMIENTO%' "
    + "						and upper(u.Nombre) not like '%MUNICIPIO%' "
    + "						and upper(u.Nombre) not like '%ESTADO%' "
    + "						and upper(u.Nombre) not like '%ESTATAL%' "
    + "						and upper(u.Nombre) not like '%FEDERAL%' "
    + "						and upper(u.Nombre) not like '%PROCURADOR%' "
    + "						and upper(u.Nombre) not like '%F_SCAL%' "
    + "						and upper(u.Nombre) not like '%GOBIERNO%' "
    + "						and upper(u.Nombre) not like '%SECRETARIO%' "
    + "						and upper(u.Nombre) not like '%SECRETAR_A%' "
    + "						and upper(u.Nombre) not like '%INSTITUTO%' "
    + "						and upper(u.Nombre) not like '%SALUD%' "
    + "						and upper(u.Nombre) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.Nombre) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'  "
    + "						and upper(u.ApePaterno) not like '%AYUNTAMIENTO%' "
    + "						and upper(u.ApePaterno) not like '%MUNICIPIO%' "
    + "						and upper(u.ApePaterno) not like '%ESTADO%' "
    + "						and upper(u.ApePaterno) not like '%ESTATAL%' "
    + "						and upper(u.ApePaterno) not like '%FEDERAL%' "
    + "						and upper(u.ApePaterno) not like '%PROCURADOR%' "
    + "						and upper(u.ApePaterno) not like '%F_SCAL%' "
    + "						and upper(u.ApePaterno) not like '%GOBIERNO%' "
    + "						and upper(u.ApePaterno) not like '%SECRETARIO%' "
    + "						and upper(u.ApePaterno) not like '%SECRETAR_A%' "
    + "						and upper(u.ApePaterno) not like '%INSTITUTO%' "
    + "						and upper(u.ApePaterno) not like '%SALUD%' "
    + "						and upper(u.ApePaterno) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' "
    + "						and upper(u.ApeMaterno) not like '%AYUNTAMIENTO%' "
    + "						and upper(u.ApeMaterno) not like '%MUNICIPIO%' "
    + "						and upper(u.ApeMaterno) not like '%ESTADO%' "
    + "						and upper(u.ApeMaterno) not like '%ESTATAL%' "
    + "						and upper(u.ApeMaterno) not like '%FEDERAL%' "
    + "						and upper(u.ApeMaterno) not like '%PROCURADOR%' "
    + "						and upper(u.ApeMaterno) not like '%F_SCAL%' "
    + "						and upper(u.ApeMaterno) not like '%GOBIERNO%' "
    + "						and upper(u.ApeMaterno) not like '%SECRETARIO%' "
    + "						and upper(u.ApeMaterno) not like '%SECRETAR_A%' "
    + "						and upper(u.ApeMaterno) not like '%INSTITUTO%' "
    + "						and upper(u.ApeMaterno) not like '%SALUD%' "
    + "						and upper(u.ApeMaterno) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' "
    + "						and upper(u.Aclaracion) not like '%AYUNTAMIENTO%' "
    + "						and upper(u.Aclaracion) not like '%MUNICIPIO%' "
    + "						and upper(u.Aclaracion) not like '%ESTADO%' "
    + "						and upper(u.Aclaracion) not like '%ESTATAL%' "
    + "						and upper(u.Aclaracion) not like '%FEDERAL%' "
    + "						and upper(u.Aclaracion) not like '%PROCURADOR%' "
    + "						and upper(u.Aclaracion) not like '%F_SCAL%' "
    + "						and upper(u.Aclaracion) not like '%GOBIERNO%' "
    + "						and upper(u.Aclaracion) not like '%SECRETARIO%' "
    + "						and upper(u.Aclaracion) not like '%SECRETAR_A%' "
    + "						and upper(u.Aclaracion) not like '%INSTITUTO%' "
    + "						and upper(u.Aclaracion) not like '%SALUD%' "
    + "						and upper(u.Aclaracion) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' then 'Moral' "
    + "					when (upper(u.Nombre) like '%ASOCIACI_N%' "
    + "						or upper(u.Nombre) like '%SOCIEDAD%' "
    + "						or upper(u.ApePaterno) like '%ASOCIACI_N%' "
    + "						or upper(u.ApePaterno) like '%SOCIEDAD%' "
    + "						or upper(u.ApeMaterno) like '%ASOCIACI_N%' "
    + "						or upper(u.ApeMaterno) like '%SOCIEDAD%' "
    + "						or upper(u.Aclaracion) like '%ASOCIACI_N%' "
    + "						or upper(u.Aclaracion) like '%SOCIEDAD%' "
    + "						or upper(u.Nombre) regexp '^A[.,; ]C[.,; ]' "
    + "						or upper(u.Nombre) regexp ' A[.,; ]C[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'A[.,; ]C[.,; ]$' "
    + "						or upper(u.Nombre) regexp '^A[.,; ]R[.,; ]' "
    + "						or upper(u.Nombre) regexp ' A[.,; ]R[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'A[.,; ]R[.,; ]$' "
    + "						or upper(u.Nombre) regexp '^S[.,; ]A[.,; ]' "
    + "						or upper(u.Nombre) regexp ' S[.,; ]A[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'S[.,; ]A[.,; ]$' "
    + "						or upper(u.Nombre) regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						or upper(u.Nombre) regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						or upper(u.Nombre) regexp '^C[.,; ]V[.,; ]' "
    + "						or upper(u.Nombre) regexp ' C[.,; ]V[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'C[.,; ]V[.,; ]$' "
    + "						or upper(u.Nombre) regexp '^R[.,; ]L[.,; ]' "
    + "						or upper(u.Nombre) regexp ' R[.,; ]L[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'R[.,; ]L[.,; ]$' "
    + "						or upper(u.Nombre) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						or upper(u.Nombre) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "						or upper(u.ApePaterno) regexp '^A[.,; ]C[.,; ]' "
    + "						or upper(u.ApePaterno) regexp ' A[.,; ]C[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'A[.,; ]C[.,; ]$' "
    + "						or upper(u.ApePaterno) regexp '^A[.,; ]R[.,; ]' "
    + "						or upper(u.ApePaterno) regexp ' A[.,; ]R[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'A[.,; ]R[.,; ]$' "
    + "						or upper(u.ApePaterno) regexp '^S[.,; ]A[.,; ]' "
    + "						or upper(u.ApePaterno) regexp ' S[.,; ]A[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'S[.,; ]A[.,; ]$' "
    + "						or upper(u.ApePaterno) regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						or upper(u.ApePaterno) regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						or upper(u.ApePaterno) regexp '^C[.,; ]V[.,; ]' "
    + "						or upper(u.ApePaterno) regexp ' C[.,; ]V[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'C[.,; ]V[.,; ]$' "
    + "						or upper(u.ApePaterno) regexp '^R[.,; ]L[.,; ]' "
    + "						or upper(u.ApePaterno) regexp ' R[.,; ]L[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'R[.,; ]L[.,; ]$' "
    + "						or upper(u.ApePaterno) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						or upper(u.ApePaterno) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "						or upper(u.ApeMaterno) regexp '^A[.,; ]C[.,; ]' "
    + "						or upper(u.ApeMaterno) regexp ' A[.,; ]C[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'A[.,; ]C[.,; ]$' "
    + "						or upper(u.ApeMaterno) regexp '^A[.,; ]R[.,; ]' "
    + "						or upper(u.ApeMaterno) regexp ' A[.,; ]R[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'A[.,; ]R[.,; ]$' "
    + "						or upper(u.ApeMaterno) regexp '^S[.,; ]A[.,; ]' "
    + "						or upper(u.ApeMaterno) regexp ' S[.,; ]A[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'S[.,; ]A[.,; ]$' "
    + "						or upper(u.ApeMaterno) regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						or upper(u.ApeMaterno) regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						or upper(u.ApeMaterno) regexp '^C[.,; ]V[.,; ]' "
    + "						or upper(u.ApeMaterno) regexp ' C[.,; ]V[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'C[.,; ]V[.,; ]$' "
    + "						or upper(u.ApeMaterno) regexp '^R[.,; ]L[.,; ]' "
    + "						or upper(u.ApeMaterno) regexp ' R[.,; ]L[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'R[.,; ]L[.,; ]$' "
    + "						or upper(u.ApeMaterno) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						or upper(u.ApeMaterno) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "						or upper(u.Aclaracion) regexp '^A[.,; ]C[.,; ]' "
    + "						or upper(u.Aclaracion) regexp ' A[.,; ]C[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'A[.,; ]C[.,; ]$' "
    + "						or upper(u.Aclaracion) regexp '^A[.,; ]R[.,; ]' "
    + "						or upper(u.Aclaracion) regexp ' A[.,; ]R[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'A[.,; ]R[.,; ]$' "
    + "						or upper(u.Aclaracion) regexp '^S[.,; ]A[.,; ]' "
    + "						or upper(u.Aclaracion) regexp ' S[.,; ]A[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'S[.,; ]A[.,; ]$' "
    + "						or upper(u.Aclaracion) regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						or upper(u.Aclaracion) regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						or upper(u.Aclaracion) regexp '^C[.,; ]V[.,; ]' "
    + "						or upper(u.Aclaracion) regexp ' C[.,; ]V[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'C[.,; ]V[.,; ]$' "
    + "						or upper(u.Aclaracion) regexp '^R[.,; ]L[.,; ]' "
    + "						or upper(u.Aclaracion) regexp ' R[.,; ]L[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'R[.,; ]L[.,; ]$' "
    + "						or upper(u.Aclaracion) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						or upper(u.Aclaracion) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$') then 'Moral'  "
    + "					when (u.Sexo in ('H','M') "
    + "						and (upper(u.Nombre) like '%MENOR%' "
    + "							or upper(u.Nombre) like '%V_CTIMA%' "
    + "							or upper(u.Nombre) like '%HIJ_%' "
    + "							or upper(u.ApePaterno) like '%MENOR%' "
    + "							or upper(u.ApePaterno) like '%V_CTIMA%' "
    + "							or upper(u.ApePaterno) like '%HIJ_%' "
    + "							or upper(u.ApeMaterno) like '%MENOR%' "
    + "							or upper(u.ApeMaterno) like '%V_CTIMA%' "
    + "							or upper(u.ApeMaterno) like '%HIJ_%' "
    + "							or upper(u.Aclaracion) like '%MENOR%' "
    + "							or upper(u.Aclaracion) like '%V_CTIMA%' "
    + "							or upper(u.Aclaracion) like '%HIJ_%')) then 'Física'  "
    + "					when (u.Sexo in ('H','M') "
    + "						and (upper(u.Nombre) not like '%ASOCIACI_N%' "
    + "							and upper(u.Nombre) not like '%SOCIEDAD%' "
    + "							and upper(u.ApePaterno) not like '%ASOCIACI_N%' "
    + "							and upper(u.ApePaterno) not like '%SOCIEDAD%' "
    + "							and upper(u.ApeMaterno) not like '%ASOCIACI_N%' "
    + "							and upper(u.ApeMaterno) not like '%SOCIEDAD%' "
    + "							and upper(u.Aclaracion) not like '%ASOCIACI_N%' "
    + "							and upper(u.Aclaracion) not like '%SOCIEDAD%' "
    + "							and upper(u.Nombre) not regexp '^A[.,; ]C[.,; ]' "
    + "							and upper(u.Nombre) not regexp ' A[.,; ]C[.,; ] ' "
    + "							and upper(u.Nombre) not regexp 'A[.,; ]C[.,; ]$' "
    + "							and upper(u.Nombre) not regexp '^A[.,; ]R[.,; ]' "
    + "							and upper(u.Nombre) not regexp ' A[.,; ]R[.,; ] ' "
    + "							and upper(u.Nombre) not regexp 'A[.,; ]R[.,; ]$' "
    + "							and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]' "
    + "							and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ] ' "
    + "							and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]$' "
    + "							and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "							and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "							and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "							and upper(u.Nombre) not regexp '^C[.,; ]V[.,; ]' "
    + "							and upper(u.Nombre) not regexp ' C[.,; ]V[.,; ] ' "
    + "							and upper(u.Nombre) not regexp 'C[.,; ]V[.,; ]$' "
    + "							and upper(u.Nombre) not regexp '^R[.,; ]L[.,; ]' "
    + "							and upper(u.Nombre) not regexp ' R[.,; ]L[.,; ] ' "
    + "							and upper(u.Nombre) not regexp 'R[.,; ]L[.,; ]$' "
    + "							and upper(u.Nombre) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "							and upper(u.Nombre) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "							and upper(u.Nombre) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "							and upper(u.ApePaterno) not regexp '^A[.,; ]C[.,; ]' "
    + "							and upper(u.ApePaterno) not regexp ' A[.,; ]C[.,; ] ' "
    + "							and upper(u.ApePaterno) not regexp 'A[.,; ]C[.,; ]$' "
    + "							and upper(u.ApePaterno) not regexp '^A[.,; ]R[.,; ]' "
    + "							and upper(u.ApePaterno) not regexp ' A[.,; ]R[.,; ] ' "
    + "							and upper(u.ApePaterno) not regexp 'A[.,; ]R[.,; ]$' "
    + "							and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]' "
    + "							and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ] ' "
    + "							and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]$' "
    + "							and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "							and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "							and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "							and upper(u.ApePaterno) not regexp '^C[.,; ]V[.,; ]' "
    + "							and upper(u.ApePaterno) not regexp ' C[.,; ]V[.,; ] ' "
    + "							and upper(u.ApePaterno) not regexp 'C[.,; ]V[.,; ]$' "
    + "							and upper(u.ApePaterno) not regexp '^R[.,; ]L[.,; ]' "
    + "							and upper(u.ApePaterno) not regexp ' R[.,; ]L[.,; ] ' "
    + "							and upper(u.ApePaterno) not regexp 'R[.,; ]L[.,; ]$' "
    + "							and upper(u.ApePaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "							and upper(u.ApePaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "							and upper(u.ApePaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "							and upper(u.ApeMaterno) not regexp '^A[.,; ]C[.,; ]' "
    + "							and upper(u.ApeMaterno) not regexp ' A[.,; ]C[.,; ] ' "
    + "							and upper(u.ApeMaterno) not regexp 'A[.,; ]C[.,; ]$' "
    + "							and upper(u.ApeMaterno) not regexp '^A[.,; ]R[.,; ]' "
    + "							and upper(u.ApeMaterno) not regexp ' A[.,; ]R[.,; ] ' "
    + "							and upper(u.ApeMaterno) not regexp 'A[.,; ]R[.,; ]$' "
    + "							and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]' "
    + "							and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ] ' "
    + "							and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]$' "
    + "							and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "							and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "							and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "							and upper(u.ApeMaterno) not regexp '^C[.,; ]V[.,; ]' "
    + "							and upper(u.ApeMaterno) not regexp ' C[.,; ]V[.,; ] ' "
    + "							and upper(u.ApeMaterno) not regexp 'C[.,; ]V[.,; ]$' "
    + "							and upper(u.ApeMaterno) not regexp '^R[.,; ]L[.,; ]' "
    + "							and upper(u.ApeMaterno) not regexp ' R[.,; ]L[.,; ] ' "
    + "							and upper(u.ApeMaterno) not regexp 'R[.,; ]L[.,; ]$' "
    + "							and upper(u.ApeMaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "							and upper(u.ApeMaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "							and upper(u.ApeMaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "							and upper(u.Aclaracion) not regexp '^A[.,; ]C[.,; ]' "
    + "							and upper(u.Aclaracion) not regexp ' A[.,; ]C[.,; ] ' "
    + "							and upper(u.Aclaracion) not regexp 'A[.,; ]C[.,; ]$' "
    + "							and upper(u.Aclaracion) not regexp '^A[.,; ]R[.,; ]' "
    + "							and upper(u.Aclaracion) not regexp ' A[.,; ]R[.,; ] ' "
    + "							and upper(u.Aclaracion) not regexp 'A[.,; ]R[.,; ]$' "
    + "							and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]' "
    + "							and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ] ' "
    + "							and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]$' "
    + "							and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "							and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "							and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "							and upper(u.Aclaracion) not regexp '^C[.,; ]V[.,; ]' "
    + "							and upper(u.Aclaracion) not regexp ' C[.,; ]V[.,; ] ' "
    + "							and upper(u.Aclaracion) not regexp 'C[.,; ]V[.,; ]$' "
    + "							and upper(u.Aclaracion) not regexp '^R[.,; ]L[.,; ]' "
    + "							and upper(u.Aclaracion) not regexp ' R[.,; ]L[.,; ] ' "
    + "							and upper(u.Aclaracion) not regexp 'R[.,; ]L[.,; ]$' "
    + "							and upper(u.Aclaracion) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "							and upper(u.Aclaracion) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "							and upper(u.Aclaracion) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$')) then 'Física'  "
    + "					else 'N/Identificado' end as 'tPersona' "
    + "			from	htsj_mediacion.tblusuario as u) as us on e.CveExp = us.CveExp"
+ " where	e.FechaTer between '" + fechaPeriodoInicio + "' and '" + fechaPeriodoFin + "'  and e.idCentroM in (" + idsCentrosMediacionConsulta + ") and n.Descripcion not like 'JR %' "
   + "			group by naturaleza;";
            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["bd67"]);
                con.Open();
                cmd = new MySqlCommand(consulta, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(resultado);
                tablaMediacion7.DataSource = dt;
                tablaMediacion7.DataBind();
                rep.Titulo = "Número de solicitantes por sexo, respecto a la causa de solución";
                rep.Tabla = dt;
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
            return rep;
        }



        /// <summary>
        /// Número de solicitantes por sexo, respecto a la causa de solución
        /// </summary>
        public ModeloTablaReporte consultarMediacionSolicitantesCausaSolucion()
        {
            ModeloTablaReporte rep = new ModeloTablaReporte();
            String consulta =

    "select n.Descripcion," +
"		count(distinct (case when us.tipo = 'S' and us.Sexo = 'H' and us.tPersona = 'Física' then us.idUsu end)) as 'Hombres'," +
    "	count(distinct (case when us.tipo = 'S' and us.Sexo = 'M' and us.tPersona = 'Física' then us.idUsu end)) as 'Mujeres'," +
    "	count(distinct (case when us.tipo = 'S' and us.tPersona = 'Moral' or us.tPersona = 'Estado' or us.tPersona = 'N/Identificado' then us.idUsu end)) as 'N/Identificados'," +
    "	count(distinct (case when us.tipo = 'S' then us.idUsu end)) as 'Total' "+
"from htsj_mediacion.tblexpediente as e" +	
    "	left join htsj_mediacion.tbltipoter  as n on e.idTipoTer  = n.idTipoTer " +
    "	left join htsj_mediacion.tbltipoexpediente as te on e.idTipoExpediente = te.idTipoExpediente" +
    "	left join htsj_mediacion.tbltipodocprocedencia as td on e.idTipoDocProcedencia = td.idTipoDocProcedencia" +
    "	left join (select" +
    "					u.idUsu," +
    "					u.CveExp," +
    "					u.Tipo," +
    "					u.Nombre," +
    "					u.ApePaterno," +
    "					u.ApeMaterno," +
    "					u.Aclaracion," +
    "					u.Sexo," +
    "					u.Edad," +
    "					case " +
    "						when ((upper(u.Nombre) like '%AYUNTAMIENTO%'" +
    "							or upper(u.Nombre) like '%MUNICIPIO%'" +
    "							or upper(u.Nombre) like '%ESTADO%'" +
    "							or upper(u.Nombre) like '%ESTATAL%'" +
    "							or upper(u.Nombre) like '%FEDERAL%'" +
    "							or upper(u.Nombre) like '%PROCURADOR%'" +
    "							or upper(u.Nombre) like '%F_SCAL%'" +
    "							or upper(u.Nombre) like '%GOBIERNO%'" +
    "							or upper(u.Nombre) like '%SECRETARIO%'" +
    "							or upper(u.Nombre) like '%SECRETAR_A%'" +
    "							or upper(u.Nombre) like '%INSTITUTO%'" +
    "							or upper(u.Nombre) like '%SALUD%'" +
    "							or upper(u.Nombre) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.Nombre) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.Nombre) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'" +
    "							or upper(u.ApePaterno) like '%AYUNTAMIENTO%'" +
    "							or upper(u.ApePaterno) like '%MUNICIPIO%'" +
    "							or upper(u.ApePaterno) like '%ESTADO%'" +
    "							or upper(u.ApePaterno) like '%ESTATAL%'" +
    "							or upper(u.ApePaterno) like '%FEDERAL%'" +
    "							or upper(u.ApePaterno) like '%PROCURADOR%'" +
    "							or upper(u.ApePaterno) like '%F_SCAL%'" +
    "							or upper(u.ApePaterno) like '%GOBIERNO%'" +
    "							or upper(u.ApePaterno) like '%SECRETARIO%'" +
    "							or upper(u.ApePaterno) like '%SECRETAR_A%'" +
    "							or upper(u.ApePaterno) like '%INSTITUTO%'" +
    "							or upper(u.ApePaterno) like '%SALUD%'" +
    "							or upper(u.ApePaterno) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.ApePaterno) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'" +
    "							or upper(u.ApeMaterno) like '%AYUNTAMIENTO%'" +
    "							or upper(u.ApeMaterno) like '%MUNICIPIO%'" +
    "							or upper(u.ApeMaterno) like '%ESTADO%'" +
    "							or upper(u.ApeMaterno) like '%ESTATAL%'" +
    "							or upper(u.ApeMaterno) like '%FEDERAL%'" +
    "							or upper(u.ApeMaterno) like '%PROCURADOR%'" +
    "							or upper(u.ApeMaterno) like '%F_SCAL%'" +
    "							or upper(u.ApeMaterno) like '%GOBIERNO%'" +
    "							or upper(u.ApeMaterno) like '%SECRETARIO%'" +
    "							or upper(u.ApeMaterno) like '%SECRETAR_A%'" +
    "							or upper(u.ApeMaterno) like '%INSTITUTO%'" +
    "							or upper(u.ApeMaterno) like '%SALUD%'" +
    "							or upper(u.ApeMaterno) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'" +
    "							or upper(u.Aclaracion) like '%AYUNTAMIENTO%'" +
    "							or upper(u.Aclaracion) like '%MUNICIPIO%'" +
    "							or upper(u.Aclaracion) like '%ESTADO%'" +
    "							or upper(u.Aclaracion) like '%ESTATAL%'" +
    "							or upper(u.Aclaracion) like '%FEDERAL%'" +
    "							or upper(u.Aclaracion) like '%PROCURADOR%'" +
    "							or upper(u.Aclaracion) like '%F_SCAL%'" +
    "							or upper(u.Aclaracion) like '%GOBIERNO%'" +
    "							or upper(u.Aclaracion) like '%SECRETARIO%'" +
    "							or upper(u.Aclaracion) like '%SECRETAR_A%'" +
    "							or upper(u.Aclaracion) like '%INSTITUTO%'" +
    "							or upper(u.Aclaracion) like '%SALUD%'" +
    "							or upper(u.Aclaracion) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.Aclaracion) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' )" +
    "							and upper(u.Nombre) not like '%ASOCIACI_N%'" +
    "							and upper(u.Nombre) not like '%SOCIEDAD%'" +
    "							and upper(u.ApePaterno) not like '%ASOCIACI_N%'" +
    "							and upper(u.ApePaterno) not like '%SOCIEDAD%'" +
    "							and upper(u.ApeMaterno) not like '%ASOCIACI_N%'" +
    "							and upper(u.ApeMaterno) not like '%SOCIEDAD%'" +
    "							and upper(u.Aclaracion) not like '%ASOCIACI_N%'" +
    "							and upper(u.Aclaracion) not like '%SOCIEDAD%'" +
    "							and upper(u.Nombre) not regexp '^A[.,; ]C[.,; ]'" +
    "							and upper(u.Nombre) not regexp ' A[.,; ]C[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'A[.,; ]C[.,; ]$'" +
    "							and upper(u.Nombre) not regexp '^A[.,; ]R[.,; ]'" +
    "							and upper(u.Nombre) not regexp ' A[.,; ]R[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'A[.,; ]R[.,; ]$'" +
    "							and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]'" +
    "							and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]$'" +
    "							and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							and upper(u.Nombre) not regexp '^C[.,; ]V[.,; ]'" +
    "							and upper(u.Nombre) not regexp ' C[.,; ]V[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'C[.,; ]V[.,; ]$'" +
    "							and upper(u.Nombre) not regexp '^R[.,; ]L[.,; ]'" +
    "							and upper(u.Nombre) not regexp ' R[.,; ]L[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'R[.,; ]L[.,; ]$'" +
    "							and upper(u.Nombre) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							and upper(u.Nombre) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "							and upper(u.ApePaterno) not regexp '^A[.,; ]C[.,; ]'" +
    "							and upper(u.ApePaterno) not regexp ' A[.,; ]C[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'A[.,; ]C[.,; ]$'" +
    "							and upper(u.ApePaterno) not regexp '^A[.,; ]R[.,; ]'" +
    "							and upper(u.ApePaterno) not regexp ' A[.,; ]R[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'A[.,; ]R[.,; ]$'" +
    "							and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]'" +
    "							and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]$'" +
    "							and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							and upper(u.ApePaterno) not regexp '^C[.,; ]V[.,; ]'" +
    "							and upper(u.ApePaterno) not regexp ' C[.,; ]V[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'C[.,; ]V[.,; ]$'" +
    "							and upper(u.ApePaterno) not regexp '^R[.,; ]L[.,; ]'" +
    "							and upper(u.ApePaterno) not regexp ' R[.,; ]L[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'R[.,; ]L[.,; ]$'" +
    "							and upper(u.ApePaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							and upper(u.ApePaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "							and upper(u.ApeMaterno) not regexp '^A[.,; ]C[.,; ]'" +
    "							and upper(u.ApeMaterno) not regexp ' A[.,; ]C[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'A[.,; ]C[.,; ]$'" +
    "							and upper(u.ApeMaterno) not regexp '^A[.,; ]R[.,; ]'" +
    "							and upper(u.ApeMaterno) not regexp ' A[.,; ]R[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'A[.,; ]R[.,; ]$'" +
    "							and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]'" +
    "							and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]$'" +
    "							and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							and upper(u.ApeMaterno) not regexp '^C[.,; ]V[.,; ]'" +
    "							and upper(u.ApeMaterno) not regexp ' C[.,; ]V[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'C[.,; ]V[.,; ]$'" +
    "							and upper(u.ApeMaterno) not regexp '^R[.,; ]L[.,; ]'" +
    "							and upper(u.ApeMaterno) not regexp ' R[.,; ]L[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'R[.,; ]L[.,; ]$'" +
    "							and upper(u.ApeMaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							and upper(u.ApeMaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "							and upper(u.Aclaracion) not regexp '^A[.,; ]C[.,; ]'" +
    "							and upper(u.Aclaracion) not regexp ' A[.,; ]C[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'A[.,; ]C[.,; ]$'" +
    "							and upper(u.Aclaracion) not regexp '^A[.,; ]R[.,; ]'" +
    "							and upper(u.Aclaracion) not regexp ' A[.,; ]R[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'A[.,; ]R[.,; ]$'" +
    "							and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]'" +
    "							and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]$'" +
    "							and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							and upper(u.Aclaracion) not regexp '^C[.,; ]V[.,; ]'" +
    "							and upper(u.Aclaracion) not regexp ' C[.,; ]V[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'C[.,; ]V[.,; ]$'" +
    "							and upper(u.Aclaracion) not regexp '^R[.,; ]L[.,; ]'" +
    "							and upper(u.Aclaracion) not regexp ' R[.,; ]L[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'R[.,; ]L[.,; ]$'" +
    "							and upper(u.Aclaracion) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							and upper(u.Aclaracion) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$') then 'Estado'" +
    "						when (upper(u.Nombre) like '%REPRE_ENTANTE%'" +
    "							or upper(u.Nombre) like '%APODERAD_%'" +
    "							or upper(u.Nombre) like '%LEGAL%'" +
    "							or upper(u.ApePaterno) like '%REPRE_ENTANTE%'" +
    "							or upper(u.ApePaterno) like '%APODERAD_%'" +
    "							or upper(u.ApePaterno) like '%LEGAL%'" +
    "							or upper(u.ApeMaterno) like '%REPRE_ENTANTE%'" +
    "							or upper(u.ApeMaterno) like '%APODERAD_%'" +
    "							or upper(u.ApeMaterno) like '%LEGAL%'" +
    "							or upper(u.Aclaracion) like '%REPRE_ENTANTE%'" +
    "							or upper(u.Aclaracion) like '%APODERAD_%'" +
    "							or upper(u.Aclaracion) like '%LEGAL%')" +
    "							and upper(u.Nombre) not like '%MENOR%'" +
    "							and upper(u.Nombre) not like '%V_CTIMA%'" +
    "							and upper(u.Nombre) not like '%HIJ_%'" +
    "							and upper(u.ApePaterno) not like '%MENOR%'" +
    "							and upper(u.ApePaterno) not like '%V_CTIMA%'" +
    "							and upper(u.ApePaterno) not like '%HIJ_%'" +
    "							and upper(u.ApeMaterno) not like '%MENOR%'" +
    "							and upper(u.ApeMaterno) not like '%V_CTIMA%'" +
    "							and upper(u.ApeMaterno) not like '%HIJ_%'" +
    "							and upper(u.Aclaracion) not like '%MENOR%'" +
    "							and upper(u.Aclaracion) not like '%V_CTIMA%'" +
    "							and upper(u.Aclaracion) not like '%HIJ_%'" +
    "							and upper(u.Nombre) not like '%AYUNTAMIENTO%'" +
    "							and upper(u.Nombre) not like '%MUNICIPIO%'" +
    "							and upper(u.Nombre) not like '%ESTADO%'" +
    "							and upper(u.Nombre) not like '%ESTATAL%'" +
    "							and upper(u.Nombre) not like '%FEDERAL%'" +
    "							and upper(u.Nombre) not like '%PROCURADOR%'" +
    "							and upper(u.Nombre) not like '%F_SCAL%'" +
    "							and upper(u.Nombre) not like '%GOBIERNO%'" +
    "							and upper(u.Nombre) not like '%SECRETARIO%'" +
    "							and upper(u.Nombre) not like '%SECRETAR_A%'" +
    "							and upper(u.Nombre) not like '%INSTITUTO%'" +
    "							and upper(u.Nombre) not like '%SALUD%'" +
    "							and upper(u.Nombre) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.Nombre) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' " +
    "							and upper(u.ApePaterno) not like '%AYUNTAMIENTO%'" +
    "							and upper(u.ApePaterno) not like '%MUNICIPIO%'" +
    "							and upper(u.ApePaterno) not like '%ESTADO%'" +
    "							and upper(u.ApePaterno) not like '%ESTATAL%'" +
    "							and upper(u.ApePaterno) not like '%FEDERAL%'" +
    "							and upper(u.ApePaterno) not like '%PROCURADOR%'" +
    "							and upper(u.ApePaterno) not like '%F_SCAL%'" +
    "							and upper(u.ApePaterno) not like '%GOBIERNO%'" +
    "							and upper(u.ApePaterno) not like '%SECRETARIO%'" +
    "							and upper(u.ApePaterno) not like '%SECRETAR_A%'" +
    "							and upper(u.ApePaterno) not like '%INSTITUTO%'" +
    "							and upper(u.ApePaterno) not like '%SALUD%'" +
    "							and upper(u.ApePaterno) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'" +
    "							and upper(u.ApeMaterno) not like '%AYUNTAMIENTO%'" +
    "							and upper(u.ApeMaterno) not like '%MUNICIPIO%'" +
    "							and upper(u.ApeMaterno) not like '%ESTADO%'" +
    "							and upper(u.ApeMaterno) not like '%ESTATAL%'" +
    "							and upper(u.ApeMaterno) not like '%FEDERAL%'" +
    "							and upper(u.ApeMaterno) not like '%PROCURADOR%'" +
    "							and upper(u.ApeMaterno) not like '%F_SCAL%'" +
    "							and upper(u.ApeMaterno) not like '%GOBIERNO%'" +
    "							and upper(u.ApeMaterno) not like '%SECRETARIO%'" +
    "							and upper(u.ApeMaterno) not like '%SECRETAR_A%'" +
    "							and upper(u.ApeMaterno) not like '%INSTITUTO%'" +
    "							and upper(u.ApeMaterno) not like '%SALUD%'" +
    "							and upper(u.ApeMaterno) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'" +
    "							and upper(u.Aclaracion) not like '%AYUNTAMIENTO%'" +
    "							and upper(u.Aclaracion) not like '%MUNICIPIO%'" +
    "							and upper(u.Aclaracion) not like '%ESTADO%'" +
    "							and upper(u.Aclaracion) not like '%ESTATAL%'" +
    "							and upper(u.Aclaracion) not like '%FEDERAL%'" +
    "							and upper(u.Aclaracion) not like '%PROCURADOR%'" +
    "							and upper(u.Aclaracion) not like '%F_SCAL%'" +
    "							and upper(u.Aclaracion) not like '%GOBIERNO%'" +
    "							and upper(u.Aclaracion) not like '%SECRETARIO%'" +
    "							and upper(u.Aclaracion) not like '%SECRETAR_A%'" +
    "							and upper(u.Aclaracion) not like '%INSTITUTO%'" +
    "							and upper(u.Aclaracion) not like '%SALUD%'" +
    "							and upper(u.Aclaracion) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' then 'Moral'" +
    "						when (upper(u.Nombre) like '%ASOCIACI_N%'" +
    "							or upper(u.Nombre) like '%SOCIEDAD%'" +
    "							or upper(u.ApePaterno) like '%ASOCIACI_N%'" +
    "							or upper(u.ApePaterno) like '%SOCIEDAD%'" +
    "							or upper(u.ApeMaterno) like '%ASOCIACI_N%'" +
    "							or upper(u.ApeMaterno) like '%SOCIEDAD%'" +
    "							or upper(u.Aclaracion) like '%ASOCIACI_N%'" +
    "							or upper(u.Aclaracion) like '%SOCIEDAD%'" +
    "							or upper(u.Nombre) regexp '^A[.,; ]C[.,; ]'" +
    "							or upper(u.Nombre) regexp ' A[.,; ]C[.,; ] '" +
    "							or upper(u.Nombre) regexp 'A[.,; ]C[.,; ]$'" +
    "							or upper(u.Nombre) regexp '^A[.,; ]R[.,; ]'" +
    "							or upper(u.Nombre) regexp ' A[.,; ]R[.,; ] '" +
    "							or upper(u.Nombre) regexp 'A[.,; ]R[.,; ]$'" +
    "							or upper(u.Nombre) regexp '^S[.,; ]A[.,; ]'" +
    "							or upper(u.Nombre) regexp ' S[.,; ]A[.,; ] '" +
    "							or upper(u.Nombre) regexp 'S[.,; ]A[.,; ]$'" +
    "							or upper(u.Nombre) regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							or upper(u.Nombre) regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							or upper(u.Nombre) regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							or upper(u.Nombre) regexp '^C[.,; ]V[.,; ]'" +
    "							or upper(u.Nombre) regexp ' C[.,; ]V[.,; ] '" +
    "							or upper(u.Nombre) regexp 'C[.,; ]V[.,; ]$'" +
    "							or upper(u.Nombre) regexp '^R[.,; ]L[.,; ]'" +
    "							or upper(u.Nombre) regexp ' R[.,; ]L[.,; ] '" +
    "							or upper(u.Nombre) regexp 'R[.,; ]L[.,; ]$'" +
    "							or upper(u.Nombre) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							or upper(u.Nombre) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							or upper(u.Nombre) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "							or upper(u.ApePaterno) regexp '^A[.,; ]C[.,; ]'" +
    "							or upper(u.ApePaterno) regexp ' A[.,; ]C[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'A[.,; ]C[.,; ]$'" +
    "							or upper(u.ApePaterno) regexp '^A[.,; ]R[.,; ]'" +
    "							or upper(u.ApePaterno) regexp ' A[.,; ]R[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'A[.,; ]R[.,; ]$'" +
    "							or upper(u.ApePaterno) regexp '^S[.,; ]A[.,; ]'" +
    "							or upper(u.ApePaterno) regexp ' S[.,; ]A[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'S[.,; ]A[.,; ]$'" +
    "							or upper(u.ApePaterno) regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							or upper(u.ApePaterno) regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							or upper(u.ApePaterno) regexp '^C[.,; ]V[.,; ]'" +
    "							or upper(u.ApePaterno) regexp ' C[.,; ]V[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'C[.,; ]V[.,; ]$'" +
    "							or upper(u.ApePaterno) regexp '^R[.,; ]L[.,; ]'" +
    "							or upper(u.ApePaterno) regexp ' R[.,; ]L[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'R[.,; ]L[.,; ]$'" +
    "							or upper(u.ApePaterno) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							or upper(u.ApePaterno) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "							or upper(u.ApeMaterno) regexp '^A[.,; ]C[.,; ]'" +
    "							or upper(u.ApeMaterno) regexp ' A[.,; ]C[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'A[.,; ]C[.,; ]$'" +
    "							or upper(u.ApeMaterno) regexp '^A[.,; ]R[.,; ]'" +
    "							or upper(u.ApeMaterno) regexp ' A[.,; ]R[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'A[.,; ]R[.,; ]$'" +
    "							or upper(u.ApeMaterno) regexp '^S[.,; ]A[.,; ]'" +
    "							or upper(u.ApeMaterno) regexp ' S[.,; ]A[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'S[.,; ]A[.,; ]$'" +
    "							or upper(u.ApeMaterno) regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							or upper(u.ApeMaterno) regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							or upper(u.ApeMaterno) regexp '^C[.,; ]V[.,; ]'" +
    "							or upper(u.ApeMaterno) regexp ' C[.,; ]V[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'C[.,; ]V[.,; ]$'" +
    "							or upper(u.ApeMaterno) regexp '^R[.,; ]L[.,; ]'" +
    "							or upper(u.ApeMaterno) regexp ' R[.,; ]L[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'R[.,; ]L[.,; ]$'" +
    "							or upper(u.ApeMaterno) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							or upper(u.ApeMaterno) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "							or upper(u.Aclaracion) regexp '^A[.,; ]C[.,; ]'" +
    "							or upper(u.Aclaracion) regexp ' A[.,; ]C[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'A[.,; ]C[.,; ]$'" +
    "							or upper(u.Aclaracion) regexp '^A[.,; ]R[.,; ]'" +
    "							or upper(u.Aclaracion) regexp ' A[.,; ]R[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'A[.,; ]R[.,; ]$'" +
    "							or upper(u.Aclaracion) regexp '^S[.,; ]A[.,; ]'" +
    "							or upper(u.Aclaracion) regexp ' S[.,; ]A[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'S[.,; ]A[.,; ]$'" +
    "							or upper(u.Aclaracion) regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							or upper(u.Aclaracion) regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							or upper(u.Aclaracion) regexp '^C[.,; ]V[.,; ]'" +
    "							or upper(u.Aclaracion) regexp ' C[.,; ]V[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'C[.,; ]V[.,; ]$'" +
    "							or upper(u.Aclaracion) regexp '^R[.,; ]L[.,; ]'" +
    "							or upper(u.Aclaracion) regexp ' R[.,; ]L[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'R[.,; ]L[.,; ]$'" +
    "							or upper(u.Aclaracion) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							or upper(u.Aclaracion) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$') then 'Moral' " +
    "						when (u.Sexo in ('H','M')" +
    "							and (upper(u.Nombre) like '%MENOR%'" +
    "								or upper(u.Nombre) like '%V_CTIMA%'" +
    "								or upper(u.Nombre) like '%HIJ_%'" +
    "								or upper(u.ApePaterno) like '%MENOR%'" +
    "								or upper(u.ApePaterno) like '%V_CTIMA%'" +
    "								or upper(u.ApePaterno) like '%HIJ_%'" +
    "								or upper(u.ApeMaterno) like '%MENOR%'" +
    "								or upper(u.ApeMaterno) like '%V_CTIMA%'" +
    "								or upper(u.ApeMaterno) like '%HIJ_%'" +
    "								or upper(u.Aclaracion) like '%MENOR%'" +
    "								or upper(u.Aclaracion) like '%V_CTIMA%'" +
    "								or upper(u.Aclaracion) like '%HIJ_%')) then 'Física' " +
    "						when (u.Sexo in ('H','M')" +
    "							and (upper(u.Nombre) not like '%ASOCIACI_N%'" +
    "								and upper(u.Nombre) not like '%SOCIEDAD%'" +
    "								and upper(u.ApePaterno) not like '%ASOCIACI_N%'" +
    "								and upper(u.ApePaterno) not like '%SOCIEDAD%'" +
    "								and upper(u.ApeMaterno) not like '%ASOCIACI_N%'" +
    "								and upper(u.ApeMaterno) not like '%SOCIEDAD%'" +
    "								and upper(u.Aclaracion) not like '%ASOCIACI_N%'" +
    "								and upper(u.Aclaracion) not like '%SOCIEDAD%'" +
    "								and upper(u.Nombre) not regexp '^A[.,; ]C[.,; ]'" +
    "								and upper(u.Nombre) not regexp ' A[.,; ]C[.,; ] '" +
    "								and upper(u.Nombre) not regexp 'A[.,; ]C[.,; ]$'" +
    "								and upper(u.Nombre) not regexp '^A[.,; ]R[.,; ]'" +
    "								and upper(u.Nombre) not regexp ' A[.,; ]R[.,; ] '" +
    "								and upper(u.Nombre) not regexp 'A[.,; ]R[.,; ]$'" +
    "								and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]'" +
    "								and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ] '" +
    "								and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]$'" +
    "								and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "								and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "								and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "								and upper(u.Nombre) not regexp '^C[.,; ]V[.,; ]'" +
    "								and upper(u.Nombre) not regexp ' C[.,; ]V[.,; ] '" +
    "								and upper(u.Nombre) not regexp 'C[.,; ]V[.,; ]$'" +
    "								and upper(u.Nombre) not regexp '^R[.,; ]L[.,; ]'" +
    "								and upper(u.Nombre) not regexp ' R[.,; ]L[.,; ] '" +
    "								and upper(u.Nombre) not regexp 'R[.,; ]L[.,; ]$'" +
    "								and upper(u.Nombre) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "								and upper(u.Nombre) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "								and upper(u.Nombre) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "								and upper(u.ApePaterno) not regexp '^A[.,; ]C[.,; ]'" +
    "								and upper(u.ApePaterno) not regexp ' A[.,; ]C[.,; ] '" +
    "								and upper(u.ApePaterno) not regexp 'A[.,; ]C[.,; ]$'" +
    "								and upper(u.ApePaterno) not regexp '^A[.,; ]R[.,; ]'" +
    "								and upper(u.ApePaterno) not regexp ' A[.,; ]R[.,; ] '" +
    "								and upper(u.ApePaterno) not regexp 'A[.,; ]R[.,; ]$'" +
    "								and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]'" +
    "								and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ] '" +
    "								and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]$'" +
    "								and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "								and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "								and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "								and upper(u.ApePaterno) not regexp '^C[.,; ]V[.,; ]'" +
    "								and upper(u.ApePaterno) not regexp ' C[.,; ]V[.,; ] '" +
    "								and upper(u.ApePaterno) not regexp 'C[.,; ]V[.,; ]$'" +
    "								and upper(u.ApePaterno) not regexp '^R[.,; ]L[.,; ]'" +
    "								and upper(u.ApePaterno) not regexp ' R[.,; ]L[.,; ] '" +
    "								and upper(u.ApePaterno) not regexp 'R[.,; ]L[.,; ]$'" +
    "								and upper(u.ApePaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "								and upper(u.ApePaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "								and upper(u.ApePaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "								and upper(u.ApeMaterno) not regexp '^A[.,; ]C[.,; ]'" +
    "								and upper(u.ApeMaterno) not regexp ' A[.,; ]C[.,; ] '" +
    "								and upper(u.ApeMaterno) not regexp 'A[.,; ]C[.,; ]$'" +
    "								and upper(u.ApeMaterno) not regexp '^A[.,; ]R[.,; ]'" +
    "								and upper(u.ApeMaterno) not regexp ' A[.,; ]R[.,; ] '" +
    "								and upper(u.ApeMaterno) not regexp 'A[.,; ]R[.,; ]$'" +
    "								and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]'" +
    "								and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ] '" +
    "								and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]$'" +
    "								and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "								and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "								and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "								and upper(u.ApeMaterno) not regexp '^C[.,; ]V[.,; ]'" +
    "								and upper(u.ApeMaterno) not regexp ' C[.,; ]V[.,; ] '" +
    "								and upper(u.ApeMaterno) not regexp 'C[.,; ]V[.,; ]$'" +
    "								and upper(u.ApeMaterno) not regexp '^R[.,; ]L[.,; ]'" +
    "								and upper(u.ApeMaterno) not regexp ' R[.,; ]L[.,; ] '" +
    "								and upper(u.ApeMaterno) not regexp 'R[.,; ]L[.,; ]$'" +
    "								and upper(u.ApeMaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "								and upper(u.ApeMaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "								and upper(u.ApeMaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "								and upper(u.Aclaracion) not regexp '^A[.,; ]C[.,; ]'" +
    "								and upper(u.Aclaracion) not regexp ' A[.,; ]C[.,; ] '" +
    "								and upper(u.Aclaracion) not regexp 'A[.,; ]C[.,; ]$'" +
    "								and upper(u.Aclaracion) not regexp '^A[.,; ]R[.,; ]'" +
    "								and upper(u.Aclaracion) not regexp ' A[.,; ]R[.,; ] '" +
    "								and upper(u.Aclaracion) not regexp 'A[.,; ]R[.,; ]$'" +
    "								and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]'" +
    "								and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ] '" +
    "								and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]$'" +
    "								and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "								and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "								and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "								and upper(u.Aclaracion) not regexp '^C[.,; ]V[.,; ]'" +
    "								and upper(u.Aclaracion) not regexp ' C[.,; ]V[.,; ] '" +
    "								and upper(u.Aclaracion) not regexp 'C[.,; ]V[.,; ]$'" +
    "								and upper(u.Aclaracion) not regexp '^R[.,; ]L[.,; ]'" +
    "								and upper(u.Aclaracion) not regexp ' R[.,; ]L[.,; ] '" +
    "								and upper(u.Aclaracion) not regexp 'R[.,; ]L[.,; ]$'" +
    "								and upper(u.Aclaracion) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "								and upper(u.Aclaracion) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "								and upper(u.Aclaracion) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$')) then 'Física' " +
    "						else 'N/Identificado' end as 'tPersona'" +
    "				from	htsj_mediacion.tblusuario as u) as us on e.CveExp = us.CveExp" +
    "				where	e.FechaTer between '" + fechaPeriodoInicio + "' and '" + fechaPeriodoFin + "'  and e.idCentroM in (" + idsCentrosMediacionConsulta + ") and n.Descripcion not like 'JR %' " +
    "			group by e.idTipoTer ";

          /*  + " where	e.FechaTer between '" + fechaPeriodoInicio + "' and '" + fechaPeriodoFin + "'  and e.idCentroM in (" + idsCentrosMediacionConsulta + ") and n.Descripcion not like 'JR %' "
   + "			group by naturaleza;";*/
            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["bd67"]);
                con.Open();
                cmd = new MySqlCommand(consulta, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(resultado);
                tablaMediacion7.DataSource = dt;
                tablaMediacion7.DataBind();
                rep.Titulo = "Número de solicitantes por sexo, respecto a la causa de solución";
                rep.Tabla = dt;
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
            return rep;
        }





        /// <summary>
        /// Número de invitados por sexo, respecto a la causa de solución
        /// </summary>
        public ModeloTablaReporte consultarMediacionInvitadosCausaSolucion0()
        {
            ModeloTablaReporte rep = new ModeloTablaReporte();
            String consulta = "select n.Descripcion as Naturaleza," 
    + "count(distinct (case when us.tipo = 'I' and us.Sexo = 'H' and us.tPersona = 'Física' then us.idUsu end)) as 'Total Hombres', "
    + "count(distinct (case when us.tipo = 'I' and us.Sexo = 'M' and us.tPersona = 'Física' then us.idUsu end)) as 'Total Mujeres', "
    + "count(distinct (case when us.tipo = 'I' and us.tPersona = 'Moral' then us.idUsu end)) as 'Total Moral', "
    + "count(distinct (case when us.tipo = 'I' and us.tPersona = 'N/Identificado' then us.idUsu end)) as 'Total N/Identificados' ,"
    + "	count(distinct (case when us.tipo = 'I' then us.idUsu end)) as 'Total' "
+ " from	htsj_mediacion.tblexpediente as e "
    + "left join htsj_mediacion.tblnaturaleza as n on e.idNat = n.idNat "
    + "inner join htsj_mediacion.tblcentrom cm on e.idCentroM = cm.idCentroM "
    + "left join htsj_mediacion.tbltipoexpediente as te on e.idTipoExpediente = te.idTipoExpediente "
    + "left join htsj_mediacion.tbltipodocprocedencia as td on e.idTipoDocProcedencia = td.idTipoDocProcedencia "
    + "left join (select "
    + "				u.idUsu, "
    + "				u.CveExp, "
    + "				u.Tipo, "
    + "				u.Nombre, "
    + "				u.ApePaterno, "
    + "				u.ApeMaterno, "
    + "				u.Aclaracion, "
    + "				u.Sexo, "
    + "				u.Edad, "
    + "				case  "
    + "					when ((upper(u.Nombre) like '%AYUNTAMIENTO%' "
    + "						or upper(u.Nombre) like '%MUNICIPIO%' "
    + "						or upper(u.Nombre) like '%ESTADO%' "
    + "						or upper(u.Nombre) like '%ESTATAL%' "
    + "						or upper(u.Nombre) like '%FEDERAL%' "
    + "						or upper(u.Nombre) like '%PROCURADOR%' "
    + "						or upper(u.Nombre) like '%F_SCAL%' "
    + "						or upper(u.Nombre) like '%GOBIERNO%' "
    + "						or upper(u.Nombre) like '%SECRETARIO%' "
    + "						or upper(u.Nombre) like '%SECRETAR_A%' "
    + "						or upper(u.Nombre) like '%INSTITUTO%' "
    + "						or upper(u.Nombre) like '%SALUD%' "
    + "						or upper(u.Nombre) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.Nombre) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' "
    + "						or upper(u.ApePaterno) like '%AYUNTAMIENTO%' "
    + "						or upper(u.ApePaterno) like '%MUNICIPIO%' "
    + "						or upper(u.ApePaterno) like '%ESTADO%' "
    + "						or upper(u.ApePaterno) like '%ESTATAL%' "
    + "						or upper(u.ApePaterno) like '%FEDERAL%' "
    + "						or upper(u.ApePaterno) like '%PROCURADOR%' "
    + "						or upper(u.ApePaterno) like '%F_SCAL%' "
    + "						or upper(u.ApePaterno) like '%GOBIERNO%' "
    + "						or upper(u.ApePaterno) like '%SECRETARIO%' "
    + "						or upper(u.ApePaterno) like '%SECRETAR_A%' "
    + "						or upper(u.ApePaterno) like '%INSTITUTO%' "
    + "						or upper(u.ApePaterno) like '%SALUD%' "
    + "						or upper(u.ApePaterno) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' "
    + "						or upper(u.ApeMaterno) like '%AYUNTAMIENTO%' "
    + "						or upper(u.ApeMaterno) like '%MUNICIPIO%' "
    + "						or upper(u.ApeMaterno) like '%ESTADO%' "
    + "						or upper(u.ApeMaterno) like '%ESTATAL%' "
    + "						or upper(u.ApeMaterno) like '%FEDERAL%' "
    + "						or upper(u.ApeMaterno) like '%PROCURADOR%' "
    + "						or upper(u.ApeMaterno) like '%F_SCAL%' "
    + "						or upper(u.ApeMaterno) like '%GOBIERNO%' "
    + "						or upper(u.ApeMaterno) like '%SECRETARIO%' "
    + "						or upper(u.ApeMaterno) like '%SECRETAR_A%' "
    + "						or upper(u.ApeMaterno) like '%INSTITUTO%' "
    + "						or upper(u.ApeMaterno) like '%SALUD%' "
    + "						or upper(u.ApeMaterno) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' "
    + "						or upper(u.Aclaracion) like '%AYUNTAMIENTO%' "
    + "						or upper(u.Aclaracion) like '%MUNICIPIO%' "
    + "						or upper(u.Aclaracion) like '%ESTADO%' "
    + "						or upper(u.Aclaracion) like '%ESTATAL%' "
    + "						or upper(u.Aclaracion) like '%FEDERAL%' "
    + "						or upper(u.Aclaracion) like '%PROCURADOR%' "
    + "						or upper(u.Aclaracion) like '%F_SCAL%' "
    + "						or upper(u.Aclaracion) like '%GOBIERNO%' "
    + "						or upper(u.Aclaracion) like '%SECRETARIO%' "
    + "						or upper(u.Aclaracion) like '%SECRETAR_A%' "
    + "						or upper(u.Aclaracion) like '%INSTITUTO%' "
    + "						or upper(u.Aclaracion) like '%SALUD%' "
    + "						or upper(u.Aclaracion) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' ) "
    + "						and upper(u.Nombre) not like '%ASOCIACI_N%' "
    + "						and upper(u.Nombre) not like '%SOCIEDAD%' "
    + "						and upper(u.ApePaterno) not like '%ASOCIACI_N%' "
    + "						and upper(u.ApePaterno) not like '%SOCIEDAD%' "
    + "						and upper(u.ApeMaterno) not like '%ASOCIACI_N%' "
    + "						and upper(u.ApeMaterno) not like '%SOCIEDAD%' "
    + "						and upper(u.Aclaracion) not like '%ASOCIACI_N%' "
    + "						and upper(u.Aclaracion) not like '%SOCIEDAD%' "
    + "						and upper(u.Nombre) not regexp '^A[.,; ]C[.,; ]' "
    + "						and upper(u.Nombre) not regexp ' A[.,; ]C[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'A[.,; ]C[.,; ]$' "
    + "						and upper(u.Nombre) not regexp '^A[.,; ]R[.,; ]' "
    + "						and upper(u.Nombre) not regexp ' A[.,; ]R[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'A[.,; ]R[.,; ]$' "
    + "						and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]' "
    + "						and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]$' "
    + "						and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						and upper(u.Nombre) not regexp '^C[.,; ]V[.,; ]' "
    + "						and upper(u.Nombre) not regexp ' C[.,; ]V[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'C[.,; ]V[.,; ]$' "
    + "						and upper(u.Nombre) not regexp '^R[.,; ]L[.,; ]' "
    + "						and upper(u.Nombre) not regexp ' R[.,; ]L[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'R[.,; ]L[.,; ]$' "
    + "						and upper(u.Nombre) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						and upper(u.Nombre) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "						and upper(u.ApePaterno) not regexp '^A[.,; ]C[.,; ]' "
    + "						and upper(u.ApePaterno) not regexp ' A[.,; ]C[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'A[.,; ]C[.,; ]$' "
    + "						and upper(u.ApePaterno) not regexp '^A[.,; ]R[.,; ]' "
    + "						and upper(u.ApePaterno) not regexp ' A[.,; ]R[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'A[.,; ]R[.,; ]$' "
    + "						and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]' "
    + "						and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]$' "
    + "						and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						and upper(u.ApePaterno) not regexp '^C[.,; ]V[.,; ]' "
    + "						and upper(u.ApePaterno) not regexp ' C[.,; ]V[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'C[.,; ]V[.,; ]$' "
    + "						and upper(u.ApePaterno) not regexp '^R[.,; ]L[.,; ]' "
    + "						and upper(u.ApePaterno) not regexp ' R[.,; ]L[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'R[.,; ]L[.,; ]$' "
    + "						and upper(u.ApePaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						and upper(u.ApePaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "						and upper(u.ApeMaterno) not regexp '^A[.,; ]C[.,; ]' "
    + "						and upper(u.ApeMaterno) not regexp ' A[.,; ]C[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'A[.,; ]C[.,; ]$' "
    + "						and upper(u.ApeMaterno) not regexp '^A[.,; ]R[.,; ]' "
    + "						and upper(u.ApeMaterno) not regexp ' A[.,; ]R[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'A[.,; ]R[.,; ]$' "
    + "						and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]' "
    + "						and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]$' "
    + "						and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						and upper(u.ApeMaterno) not regexp '^C[.,; ]V[.,; ]' "
    + "						and upper(u.ApeMaterno) not regexp ' C[.,; ]V[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'C[.,; ]V[.,; ]$' "
    + "						and upper(u.ApeMaterno) not regexp '^R[.,; ]L[.,; ]' "
    + "						and upper(u.ApeMaterno) not regexp ' R[.,; ]L[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'R[.,; ]L[.,; ]$' "
    + "						and upper(u.ApeMaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						and upper(u.ApeMaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "						and upper(u.Aclaracion) not regexp '^A[.,; ]C[.,; ]' "
    + "						and upper(u.Aclaracion) not regexp ' A[.,; ]C[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'A[.,; ]C[.,; ]$' "
    + "						and upper(u.Aclaracion) not regexp '^A[.,; ]R[.,; ]' "
    + "						and upper(u.Aclaracion) not regexp ' A[.,; ]R[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'A[.,; ]R[.,; ]$' "
    + "						and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]' "
    + "						and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]$' "
    + "						and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						and upper(u.Aclaracion) not regexp '^C[.,; ]V[.,; ]' "
    + "						and upper(u.Aclaracion) not regexp ' C[.,; ]V[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'C[.,; ]V[.,; ]$' "
    + "						and upper(u.Aclaracion) not regexp '^R[.,; ]L[.,; ]' "
    + "						and upper(u.Aclaracion) not regexp ' R[.,; ]L[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'R[.,; ]L[.,; ]$' "
    + "						and upper(u.Aclaracion) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						and upper(u.Aclaracion) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$') then 'N/Identificado' "
    + "					when (upper(u.Nombre) like '%REPRE_ENTANTE%' "
    + "						or upper(u.Nombre) like '%APODERAD_%' "
    + "						or upper(u.Nombre) like '%LEGAL%' "
    + "						or upper(u.ApePaterno) like '%REPRE_ENTANTE%' "
    + "						or upper(u.ApePaterno) like '%APODERAD_%' "
    + "						or upper(u.ApePaterno) like '%LEGAL%' "
    + "						or upper(u.ApeMaterno) like '%REPRE_ENTANTE%' "
    + "						or upper(u.ApeMaterno) like '%APODERAD_%' "
    + "						or upper(u.ApeMaterno) like '%LEGAL%' "
    + "						or upper(u.Aclaracion) like '%REPRE_ENTANTE%' "
    + "						or upper(u.Aclaracion) like '%APODERAD_%' "
    + "						or upper(u.Aclaracion) like '%LEGAL%') "
    + "						and upper(u.Nombre) not like '%MENOR%' "
    + "						and upper(u.Nombre) not like '%V_CTIMA%' "
    + "						and upper(u.Nombre) not like '%HIJ_%' "
    + "						and upper(u.ApePaterno) not like '%MENOR%' "
    + "						and upper(u.ApePaterno) not like '%V_CTIMA%' "
    + "						and upper(u.ApePaterno) not like '%HIJ_%' "
    + "						and upper(u.ApeMaterno) not like '%MENOR%' "
    + "						and upper(u.ApeMaterno) not like '%V_CTIMA%' "
    + "						and upper(u.ApeMaterno) not like '%HIJ_%' "
    + "						and upper(u.Aclaracion) not like '%MENOR%' "
    + "						and upper(u.Aclaracion) not like '%V_CTIMA%' "
    + "						and upper(u.Aclaracion) not like '%HIJ_%' "
    + "						and upper(u.Nombre) not like '%AYUNTAMIENTO%' "
    + "						and upper(u.Nombre) not like '%MUNICIPIO%' "
    + "						and upper(u.Nombre) not like '%ESTADO%' "
    + "						and upper(u.Nombre) not like '%ESTATAL%' "
    + "						and upper(u.Nombre) not like '%FEDERAL%' "
    + "						and upper(u.Nombre) not like '%PROCURADOR%' "
    + "						and upper(u.Nombre) not like '%F_SCAL%' "
    + "						and upper(u.Nombre) not like '%GOBIERNO%' "
    + "						and upper(u.Nombre) not like '%SECRETARIO%' "
    + "						and upper(u.Nombre) not like '%SECRETAR_A%' "
    + "						and upper(u.Nombre) not like '%INSTITUTO%' "
    + "						and upper(u.Nombre) not like '%SALUD%' "
    + "						and upper(u.Nombre) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.Nombre) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.Nombre) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'  "
    + "						and upper(u.ApePaterno) not like '%AYUNTAMIENTO%' "
    + "						and upper(u.ApePaterno) not like '%MUNICIPIO%' "
    + "						and upper(u.ApePaterno) not like '%ESTADO%' "
    + "						and upper(u.ApePaterno) not like '%ESTATAL%' "
    + "						and upper(u.ApePaterno) not like '%FEDERAL%' "
    + "						and upper(u.ApePaterno) not like '%PROCURADOR%' "
    + "						and upper(u.ApePaterno) not like '%F_SCAL%' "
    + "						and upper(u.ApePaterno) not like '%GOBIERNO%' "
    + "						and upper(u.ApePaterno) not like '%SECRETARIO%' "
    + "						and upper(u.ApePaterno) not like '%SECRETAR_A%' "
    + "						and upper(u.ApePaterno) not like '%INSTITUTO%' "
    + "						and upper(u.ApePaterno) not like '%SALUD%' "
    + "						and upper(u.ApePaterno) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.ApePaterno) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' "
    + "						and upper(u.ApeMaterno) not like '%AYUNTAMIENTO%' "
    + "						and upper(u.ApeMaterno) not like '%MUNICIPIO%' "
    + "						and upper(u.ApeMaterno) not like '%ESTADO%' "
    + "						and upper(u.ApeMaterno) not like '%ESTATAL%' "
    + "						and upper(u.ApeMaterno) not like '%FEDERAL%' "
    + "						and upper(u.ApeMaterno) not like '%PROCURADOR%' "
    + "						and upper(u.ApeMaterno) not like '%F_SCAL%' "
    + "						and upper(u.ApeMaterno) not like '%GOBIERNO%' "
    + "						and upper(u.ApeMaterno) not like '%SECRETARIO%' "
    + "						and upper(u.ApeMaterno) not like '%SECRETAR_A%' "
    + "						and upper(u.ApeMaterno) not like '%INSTITUTO%' "
    + "						and upper(u.ApeMaterno) not like '%SALUD%' "
    + "						and upper(u.ApeMaterno) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.ApeMaterno) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' "
    + "						and upper(u.Aclaracion) not like '%AYUNTAMIENTO%' "
    + "						and upper(u.Aclaracion) not like '%MUNICIPIO%' "
    + "						and upper(u.Aclaracion) not like '%ESTADO%' "
    + "						and upper(u.Aclaracion) not like '%ESTATAL%' "
    + "						and upper(u.Aclaracion) not like '%FEDERAL%' "
    + "						and upper(u.Aclaracion) not like '%PROCURADOR%' "
    + "						and upper(u.Aclaracion) not like '%F_SCAL%' "
    + "						and upper(u.Aclaracion) not like '%GOBIERNO%' "
    + "						and upper(u.Aclaracion) not like '%SECRETARIO%' "
    + "						and upper(u.Aclaracion) not like '%SECRETAR_A%' "
    + "						and upper(u.Aclaracion) not like '%INSTITUTO%' "
    + "						and upper(u.Aclaracion) not like '%SALUD%' "
    + "						and upper(u.Aclaracion) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] ' "
    + "						and upper(u.Aclaracion) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' then 'Moral' "
    + "					when (upper(u.Nombre) like '%ASOCIACI_N%' "
    + "						or upper(u.Nombre) like '%SOCIEDAD%' "
    + "						or upper(u.ApePaterno) like '%ASOCIACI_N%' "
    + "						or upper(u.ApePaterno) like '%SOCIEDAD%' "
    + "						or upper(u.ApeMaterno) like '%ASOCIACI_N%' "
    + "						or upper(u.ApeMaterno) like '%SOCIEDAD%' "
    + "						or upper(u.Aclaracion) like '%ASOCIACI_N%' "
    + "						or upper(u.Aclaracion) like '%SOCIEDAD%' "
    + "						or upper(u.Nombre) regexp '^A[.,; ]C[.,; ]' "
    + "						or upper(u.Nombre) regexp ' A[.,; ]C[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'A[.,; ]C[.,; ]$' "
    + "						or upper(u.Nombre) regexp '^A[.,; ]R[.,; ]' "
    + "						or upper(u.Nombre) regexp ' A[.,; ]R[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'A[.,; ]R[.,; ]$' "
    + "						or upper(u.Nombre) regexp '^S[.,; ]A[.,; ]' "
    + "						or upper(u.Nombre) regexp ' S[.,; ]A[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'S[.,; ]A[.,; ]$' "
    + "						or upper(u.Nombre) regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						or upper(u.Nombre) regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						or upper(u.Nombre) regexp '^C[.,; ]V[.,; ]' "
    + "						or upper(u.Nombre) regexp ' C[.,; ]V[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'C[.,; ]V[.,; ]$' "
    + "						or upper(u.Nombre) regexp '^R[.,; ]L[.,; ]' "
    + "						or upper(u.Nombre) regexp ' R[.,; ]L[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'R[.,; ]L[.,; ]$' "
    + "						or upper(u.Nombre) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						or upper(u.Nombre) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						or upper(u.Nombre) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "						or upper(u.ApePaterno) regexp '^A[.,; ]C[.,; ]' "
    + "						or upper(u.ApePaterno) regexp ' A[.,; ]C[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'A[.,; ]C[.,; ]$' "
    + "						or upper(u.ApePaterno) regexp '^A[.,; ]R[.,; ]' "
    + "						or upper(u.ApePaterno) regexp ' A[.,; ]R[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'A[.,; ]R[.,; ]$' "
    + "						or upper(u.ApePaterno) regexp '^S[.,; ]A[.,; ]' "
    + "						or upper(u.ApePaterno) regexp ' S[.,; ]A[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'S[.,; ]A[.,; ]$' "
    + "						or upper(u.ApePaterno) regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						or upper(u.ApePaterno) regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						or upper(u.ApePaterno) regexp '^C[.,; ]V[.,; ]' "
    + "						or upper(u.ApePaterno) regexp ' C[.,; ]V[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'C[.,; ]V[.,; ]$' "
    + "						or upper(u.ApePaterno) regexp '^R[.,; ]L[.,; ]' "
    + "						or upper(u.ApePaterno) regexp ' R[.,; ]L[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'R[.,; ]L[.,; ]$' "
    + "						or upper(u.ApePaterno) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						or upper(u.ApePaterno) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						or upper(u.ApePaterno) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "						or upper(u.ApeMaterno) regexp '^A[.,; ]C[.,; ]' "
    + "						or upper(u.ApeMaterno) regexp ' A[.,; ]C[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'A[.,; ]C[.,; ]$' "
    + "						or upper(u.ApeMaterno) regexp '^A[.,; ]R[.,; ]' "
    + "						or upper(u.ApeMaterno) regexp ' A[.,; ]R[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'A[.,; ]R[.,; ]$' "
    + "						or upper(u.ApeMaterno) regexp '^S[.,; ]A[.,; ]' "
    + "						or upper(u.ApeMaterno) regexp ' S[.,; ]A[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'S[.,; ]A[.,; ]$' "
    + "						or upper(u.ApeMaterno) regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						or upper(u.ApeMaterno) regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						or upper(u.ApeMaterno) regexp '^C[.,; ]V[.,; ]' "
    + "						or upper(u.ApeMaterno) regexp ' C[.,; ]V[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'C[.,; ]V[.,; ]$' "
    + "						or upper(u.ApeMaterno) regexp '^R[.,; ]L[.,; ]' "
    + "						or upper(u.ApeMaterno) regexp ' R[.,; ]L[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'R[.,; ]L[.,; ]$' "
    + "						or upper(u.ApeMaterno) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						or upper(u.ApeMaterno) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						or upper(u.ApeMaterno) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "						or upper(u.Aclaracion) regexp '^A[.,; ]C[.,; ]' "
    + "						or upper(u.Aclaracion) regexp ' A[.,; ]C[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'A[.,; ]C[.,; ]$' "
    + "						or upper(u.Aclaracion) regexp '^A[.,; ]R[.,; ]' "
    + "						or upper(u.Aclaracion) regexp ' A[.,; ]R[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'A[.,; ]R[.,; ]$' "
    + "						or upper(u.Aclaracion) regexp '^S[.,; ]A[.,; ]' "
    + "						or upper(u.Aclaracion) regexp ' S[.,; ]A[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'S[.,; ]A[.,; ]$' "
    + "						or upper(u.Aclaracion) regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "						or upper(u.Aclaracion) regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "						or upper(u.Aclaracion) regexp '^C[.,; ]V[.,; ]' "
    + "						or upper(u.Aclaracion) regexp ' C[.,; ]V[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'C[.,; ]V[.,; ]$' "
    + "						or upper(u.Aclaracion) regexp '^R[.,; ]L[.,; ]' "
    + "						or upper(u.Aclaracion) regexp ' R[.,; ]L[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'R[.,; ]L[.,; ]$' "
    + "						or upper(u.Aclaracion) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "						or upper(u.Aclaracion) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "						or upper(u.Aclaracion) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$') then 'Moral'  "
    + "					when (u.Sexo in ('H','M') "
    + "						and (upper(u.Nombre) like '%MENOR%' "
    + "							or upper(u.Nombre) like '%V_CTIMA%' "
    + "							or upper(u.Nombre) like '%HIJ_%' "
    + "							or upper(u.ApePaterno) like '%MENOR%' "
    + "							or upper(u.ApePaterno) like '%V_CTIMA%' "
    + "							or upper(u.ApePaterno) like '%HIJ_%' "
    + "							or upper(u.ApeMaterno) like '%MENOR%' "
    + "							or upper(u.ApeMaterno) like '%V_CTIMA%' "
    + "							or upper(u.ApeMaterno) like '%HIJ_%' "
    + "							or upper(u.Aclaracion) like '%MENOR%' "
    + "							or upper(u.Aclaracion) like '%V_CTIMA%' "
    + "							or upper(u.Aclaracion) like '%HIJ_%')) then 'Física'  "
    + "					when (u.Sexo in ('H','M') "
    + "						and (upper(u.Nombre) not like '%ASOCIACI_N%' "
    + "							and upper(u.Nombre) not like '%SOCIEDAD%' "
    + "							and upper(u.ApePaterno) not like '%ASOCIACI_N%' "
    + "							and upper(u.ApePaterno) not like '%SOCIEDAD%' "
    + "							and upper(u.ApeMaterno) not like '%ASOCIACI_N%' "
    + "							and upper(u.ApeMaterno) not like '%SOCIEDAD%' "
    + "							and upper(u.Aclaracion) not like '%ASOCIACI_N%' "
    + "							and upper(u.Aclaracion) not like '%SOCIEDAD%' "
    + "							and upper(u.Nombre) not regexp '^A[.,; ]C[.,; ]' "
    + "							and upper(u.Nombre) not regexp ' A[.,; ]C[.,; ] ' "
    + "							and upper(u.Nombre) not regexp 'A[.,; ]C[.,; ]$' "
    + "							and upper(u.Nombre) not regexp '^A[.,; ]R[.,; ]' "
    + "							and upper(u.Nombre) not regexp ' A[.,; ]R[.,; ] ' "
    + "							and upper(u.Nombre) not regexp 'A[.,; ]R[.,; ]$' "
    + "							and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]' "
    + "							and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ] ' "
    + "							and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]$' "
    + "							and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "							and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "							and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "							and upper(u.Nombre) not regexp '^C[.,; ]V[.,; ]' "
    + "							and upper(u.Nombre) not regexp ' C[.,; ]V[.,; ] ' "
    + "							and upper(u.Nombre) not regexp 'C[.,; ]V[.,; ]$' "
    + "							and upper(u.Nombre) not regexp '^R[.,; ]L[.,; ]' "
    + "							and upper(u.Nombre) not regexp ' R[.,; ]L[.,; ] ' "
    + "							and upper(u.Nombre) not regexp 'R[.,; ]L[.,; ]$' "
    + "							and upper(u.Nombre) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "							and upper(u.Nombre) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "							and upper(u.Nombre) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "							and upper(u.ApePaterno) not regexp '^A[.,; ]C[.,; ]' "
    + "							and upper(u.ApePaterno) not regexp ' A[.,; ]C[.,; ] ' "
    + "							and upper(u.ApePaterno) not regexp 'A[.,; ]C[.,; ]$' "
    + "							and upper(u.ApePaterno) not regexp '^A[.,; ]R[.,; ]' "
    + "							and upper(u.ApePaterno) not regexp ' A[.,; ]R[.,; ] ' "
    + "							and upper(u.ApePaterno) not regexp 'A[.,; ]R[.,; ]$' "
    + "							and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]' "
    + "							and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ] ' "
    + "							and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]$' "
    + "							and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "							and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "							and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "							and upper(u.ApePaterno) not regexp '^C[.,; ]V[.,; ]' "
    + "							and upper(u.ApePaterno) not regexp ' C[.,; ]V[.,; ] ' "
    + "							and upper(u.ApePaterno) not regexp 'C[.,; ]V[.,; ]$' "
    + "							and upper(u.ApePaterno) not regexp '^R[.,; ]L[.,; ]' "
    + "							and upper(u.ApePaterno) not regexp ' R[.,; ]L[.,; ] ' "
    + "							and upper(u.ApePaterno) not regexp 'R[.,; ]L[.,; ]$' "
    + "							and upper(u.ApePaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "							and upper(u.ApePaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "							and upper(u.ApePaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "							and upper(u.ApeMaterno) not regexp '^A[.,; ]C[.,; ]' "
    + "							and upper(u.ApeMaterno) not regexp ' A[.,; ]C[.,; ] ' "
    + "							and upper(u.ApeMaterno) not regexp 'A[.,; ]C[.,; ]$' "
    + "							and upper(u.ApeMaterno) not regexp '^A[.,; ]R[.,; ]' "
    + "							and upper(u.ApeMaterno) not regexp ' A[.,; ]R[.,; ] ' "
    + "							and upper(u.ApeMaterno) not regexp 'A[.,; ]R[.,; ]$' "
    + "							and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]' "
    + "							and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ] ' "
    + "							and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]$' "
    + "							and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "							and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "							and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "							and upper(u.ApeMaterno) not regexp '^C[.,; ]V[.,; ]' "
    + "							and upper(u.ApeMaterno) not regexp ' C[.,; ]V[.,; ] ' "
    + "							and upper(u.ApeMaterno) not regexp 'C[.,; ]V[.,; ]$' "
    + "							and upper(u.ApeMaterno) not regexp '^R[.,; ]L[.,; ]' "
    + "							and upper(u.ApeMaterno) not regexp ' R[.,; ]L[.,; ] ' "
    + "							and upper(u.ApeMaterno) not regexp 'R[.,; ]L[.,; ]$' "
    + "							and upper(u.ApeMaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "							and upper(u.ApeMaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "							and upper(u.ApeMaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$' "
    + "							and upper(u.Aclaracion) not regexp '^A[.,; ]C[.,; ]' "
    + "							and upper(u.Aclaracion) not regexp ' A[.,; ]C[.,; ] ' "
    + "							and upper(u.Aclaracion) not regexp 'A[.,; ]C[.,; ]$' "
    + "							and upper(u.Aclaracion) not regexp '^A[.,; ]R[.,; ]' "
    + "							and upper(u.Aclaracion) not regexp ' A[.,; ]R[.,; ] ' "
    + "							and upper(u.Aclaracion) not regexp 'A[.,; ]R[.,; ]$' "
    + "							and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]' "
    + "							and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ] ' "
    + "							and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]$' "
    + "							and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]B[.,; ]' "
    + "							and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ]B[.,; ] ' "
    + "							and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]B[.,; ]$' "
    + "							and upper(u.Aclaracion) not regexp '^C[.,; ]V[.,; ]' "
    + "							and upper(u.Aclaracion) not regexp ' C[.,; ]V[.,; ] ' "
    + "							and upper(u.Aclaracion) not regexp 'C[.,; ]V[.,; ]$' "
    + "							and upper(u.Aclaracion) not regexp '^R[.,; ]L[.,; ]' "
    + "							and upper(u.Aclaracion) not regexp ' R[.,; ]L[.,; ] ' "
    + "							and upper(u.Aclaracion) not regexp 'R[.,; ]L[.,; ]$' "
    + "							and upper(u.Aclaracion) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]' "
    + "							and upper(u.Aclaracion) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] ' "
    + "							and upper(u.Aclaracion) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$')) then 'Física'  "
    + "					else 'N/Identificado' end as 'tPersona' "
    + "			from	htsj_mediacion.tblusuario as u) as us on e.CveExp = us.CveExp "
+ " where	e.FechaTer between '" + fechaPeriodoInicio + "' and '" + fechaPeriodoFin + "'  and e.idCentroM in (" + idsCentrosMediacionConsulta  + ") and n.Descripcion not like 'JR %' "
   + "			group by naturaleza;";
            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["bd67"]);
                con.Open();
                cmd = new MySqlCommand(consulta, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(resultado);
                tablaMediacion8.DataSource = dt;
                tablaMediacion8.DataBind();
                rep.Titulo = "Número de invitados por sexo, respecto a la causa de solución";
                rep.Tabla = dt;
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
            return rep;
        }


        /// <summary>
        /// Número de invitados por sexo, respecto a la causa de solución
        /// </summary>
        public ModeloTablaReporte consultarMediacionInvitadosCausaSolucion()
        {
            ModeloTablaReporte rep = new ModeloTablaReporte();
            String consulta =

    "select n.Descripcion," +
"		count(distinct (case when us.tipo = 'I' and us.Sexo = 'H' and us.tPersona = 'Física' then us.idUsu end)) as 'Hombres'," +
    "	count(distinct (case when us.tipo = 'I' and us.Sexo = 'M' and us.tPersona = 'Física' then us.idUsu end)) as 'Mujeres'," +
    "	count(distinct (case when us.tipo = 'I' and us.tPersona = 'Moral' or us.tPersona = 'Estado' or us.tPersona = 'N/Identificado' then us.idUsu end)) as 'N/Identificados'," +
    "	count(distinct (case when us.tipo = 'I' then us.idUsu end)) as 'Total' " +
"from	htsj_mediacion.tblexpediente as e" +
    "	left join htsj_mediacion.tbltipoter  as n on e.idTipoTer  = n.idTipoTer " +
    "	left join htsj_mediacion.tbltipoexpediente as te on e.idTipoExpediente = te.idTipoExpediente" +
    "	left join htsj_mediacion.tbltipodocprocedencia as td on e.idTipoDocProcedencia = td.idTipoDocProcedencia" +
    "	left join (select" +
    "					u.idUsu," +
    "					u.CveExp," +
    "					u.Tipo," +
    "					u.Nombre," +
    "					u.ApePaterno," +
    "					u.ApeMaterno," +
    "					u.Aclaracion," +
    "					u.Sexo," +
    "					u.Edad," +
    "					case " +
    "						when ((upper(u.Nombre) like '%AYUNTAMIENTO%'" +
    "							or upper(u.Nombre) like '%MUNICIPIO%'" +
    "							or upper(u.Nombre) like '%ESTADO%'" +
    "							or upper(u.Nombre) like '%ESTATAL%'" +
    "							or upper(u.Nombre) like '%FEDERAL%'" +
    "							or upper(u.Nombre) like '%PROCURADOR%'" +
    "							or upper(u.Nombre) like '%F_SCAL%'" +
    "							or upper(u.Nombre) like '%GOBIERNO%'" +
    "							or upper(u.Nombre) like '%SECRETARIO%'" +
    "							or upper(u.Nombre) like '%SECRETAR_A%'" +
    "							or upper(u.Nombre) like '%INSTITUTO%'" +
    "							or upper(u.Nombre) like '%SALUD%'" +
    "							or upper(u.Nombre) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.Nombre) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.Nombre) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'" +
    "							or upper(u.ApePaterno) like '%AYUNTAMIENTO%'" +
    "							or upper(u.ApePaterno) like '%MUNICIPIO%'" +
    "							or upper(u.ApePaterno) like '%ESTADO%'" +
    "							or upper(u.ApePaterno) like '%ESTATAL%'" +
    "							or upper(u.ApePaterno) like '%FEDERAL%'" +
    "							or upper(u.ApePaterno) like '%PROCURADOR%'" +
    "							or upper(u.ApePaterno) like '%F_SCAL%'" +
    "							or upper(u.ApePaterno) like '%GOBIERNO%'" +
    "							or upper(u.ApePaterno) like '%SECRETARIO%'" +
    "							or upper(u.ApePaterno) like '%SECRETAR_A%'" +
    "							or upper(u.ApePaterno) like '%INSTITUTO%'" +
    "							or upper(u.ApePaterno) like '%SALUD%'" +
    "							or upper(u.ApePaterno) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.ApePaterno) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'" +
    "							or upper(u.ApeMaterno) like '%AYUNTAMIENTO%'" +
    "							or upper(u.ApeMaterno) like '%MUNICIPIO%'" +
    "							or upper(u.ApeMaterno) like '%ESTADO%'" +
    "							or upper(u.ApeMaterno) like '%ESTATAL%'" +
    "							or upper(u.ApeMaterno) like '%FEDERAL%'" +
    "							or upper(u.ApeMaterno) like '%PROCURADOR%'" +
    "							or upper(u.ApeMaterno) like '%F_SCAL%'" +
    "							or upper(u.ApeMaterno) like '%GOBIERNO%'" +
    "							or upper(u.ApeMaterno) like '%SECRETARIO%'" +
    "							or upper(u.ApeMaterno) like '%SECRETAR_A%'" +
    "							or upper(u.ApeMaterno) like '%INSTITUTO%'" +
    "							or upper(u.ApeMaterno) like '%SALUD%'" +
    "							or upper(u.ApeMaterno) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'" +
    "							or upper(u.Aclaracion) like '%AYUNTAMIENTO%'" +
    "							or upper(u.Aclaracion) like '%MUNICIPIO%'" +
    "							or upper(u.Aclaracion) like '%ESTADO%'" +
    "							or upper(u.Aclaracion) like '%ESTATAL%'" +
    "							or upper(u.Aclaracion) like '%FEDERAL%'" +
    "							or upper(u.Aclaracion) like '%PROCURADOR%'" +
    "							or upper(u.Aclaracion) like '%F_SCAL%'" +
    "							or upper(u.Aclaracion) like '%GOBIERNO%'" +
    "							or upper(u.Aclaracion) like '%SECRETARIO%'" +
    "							or upper(u.Aclaracion) like '%SECRETAR_A%'" +
    "							or upper(u.Aclaracion) like '%INSTITUTO%'" +
    "							or upper(u.Aclaracion) like '%SALUD%'" +
    "							or upper(u.Aclaracion) regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.Aclaracion) regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' )" +
    "							and upper(u.Nombre) not like '%ASOCIACI_N%'" +
    "							and upper(u.Nombre) not like '%SOCIEDAD%'" +
    "							and upper(u.ApePaterno) not like '%ASOCIACI_N%'" +
    "							and upper(u.ApePaterno) not like '%SOCIEDAD%'" +
    "							and upper(u.ApeMaterno) not like '%ASOCIACI_N%'" +
    "							and upper(u.ApeMaterno) not like '%SOCIEDAD%'" +
    "							and upper(u.Aclaracion) not like '%ASOCIACI_N%'" +
    "							and upper(u.Aclaracion) not like '%SOCIEDAD%'" +
    "							and upper(u.Nombre) not regexp '^A[.,; ]C[.,; ]'" +
    "							and upper(u.Nombre) not regexp ' A[.,; ]C[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'A[.,; ]C[.,; ]$'" +
    "							and upper(u.Nombre) not regexp '^A[.,; ]R[.,; ]'" +
    "							and upper(u.Nombre) not regexp ' A[.,; ]R[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'A[.,; ]R[.,; ]$'" +
    "							and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]'" +
    "							and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]$'" +
    "							and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							and upper(u.Nombre) not regexp '^C[.,; ]V[.,; ]'" +
    "							and upper(u.Nombre) not regexp ' C[.,; ]V[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'C[.,; ]V[.,; ]$'" +
    "							and upper(u.Nombre) not regexp '^R[.,; ]L[.,; ]'" +
    "							and upper(u.Nombre) not regexp ' R[.,; ]L[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'R[.,; ]L[.,; ]$'" +
    "							and upper(u.Nombre) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							and upper(u.Nombre) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "							and upper(u.ApePaterno) not regexp '^A[.,; ]C[.,; ]'" +
    "							and upper(u.ApePaterno) not regexp ' A[.,; ]C[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'A[.,; ]C[.,; ]$'" +
    "							and upper(u.ApePaterno) not regexp '^A[.,; ]R[.,; ]'" +
    "							and upper(u.ApePaterno) not regexp ' A[.,; ]R[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'A[.,; ]R[.,; ]$'" +
    "							and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]'" +
    "							and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]$'" +
    "							and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							and upper(u.ApePaterno) not regexp '^C[.,; ]V[.,; ]'" +
    "							and upper(u.ApePaterno) not regexp ' C[.,; ]V[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'C[.,; ]V[.,; ]$'" +
    "							and upper(u.ApePaterno) not regexp '^R[.,; ]L[.,; ]'" +
    "							and upper(u.ApePaterno) not regexp ' R[.,; ]L[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'R[.,; ]L[.,; ]$'" +
    "							and upper(u.ApePaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							and upper(u.ApePaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "							and upper(u.ApeMaterno) not regexp '^A[.,; ]C[.,; ]'" +
    "							and upper(u.ApeMaterno) not regexp ' A[.,; ]C[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'A[.,; ]C[.,; ]$'" +
    "							and upper(u.ApeMaterno) not regexp '^A[.,; ]R[.,; ]'" +
    "							and upper(u.ApeMaterno) not regexp ' A[.,; ]R[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'A[.,; ]R[.,; ]$'" +
    "							and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]'" +
    "							and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]$'" +
    "							and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							and upper(u.ApeMaterno) not regexp '^C[.,; ]V[.,; ]'" +
    "							and upper(u.ApeMaterno) not regexp ' C[.,; ]V[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'C[.,; ]V[.,; ]$'" +
    "							and upper(u.ApeMaterno) not regexp '^R[.,; ]L[.,; ]'" +
    "							and upper(u.ApeMaterno) not regexp ' R[.,; ]L[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'R[.,; ]L[.,; ]$'" +
    "							and upper(u.ApeMaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							and upper(u.ApeMaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "							and upper(u.Aclaracion) not regexp '^A[.,; ]C[.,; ]'" +
    "							and upper(u.Aclaracion) not regexp ' A[.,; ]C[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'A[.,; ]C[.,; ]$'" +
    "							and upper(u.Aclaracion) not regexp '^A[.,; ]R[.,; ]'" +
    "							and upper(u.Aclaracion) not regexp ' A[.,; ]R[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'A[.,; ]R[.,; ]$'" +
    "							and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]'" +
    "							and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]$'" +
    "							and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							and upper(u.Aclaracion) not regexp '^C[.,; ]V[.,; ]'" +
    "							and upper(u.Aclaracion) not regexp ' C[.,; ]V[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'C[.,; ]V[.,; ]$'" +
    "							and upper(u.Aclaracion) not regexp '^R[.,; ]L[.,; ]'" +
    "							and upper(u.Aclaracion) not regexp ' R[.,; ]L[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'R[.,; ]L[.,; ]$'" +
    "							and upper(u.Aclaracion) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							and upper(u.Aclaracion) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$') then 'Estado'" +
    "						when (upper(u.Nombre) like '%REPRE_ENTANTE%'" +
    "							or upper(u.Nombre) like '%APODERAD_%'" +
    "							or upper(u.Nombre) like '%LEGAL%'" +
    "							or upper(u.ApePaterno) like '%REPRE_ENTANTE%'" +
    "							or upper(u.ApePaterno) like '%APODERAD_%'" +
    "							or upper(u.ApePaterno) like '%LEGAL%'" +
    "							or upper(u.ApeMaterno) like '%REPRE_ENTANTE%'" +
    "							or upper(u.ApeMaterno) like '%APODERAD_%'" +
    "							or upper(u.ApeMaterno) like '%LEGAL%'" +
    "							or upper(u.Aclaracion) like '%REPRE_ENTANTE%'" +
    "							or upper(u.Aclaracion) like '%APODERAD_%'" +
    "							or upper(u.Aclaracion) like '%LEGAL%')" +
    "							and upper(u.Nombre) not like '%MENOR%'" +
    "							and upper(u.Nombre) not like '%V_CTIMA%'" +
    "							and upper(u.Nombre) not like '%HIJ_%'" +
    "							and upper(u.ApePaterno) not like '%MENOR%'" +
    "							and upper(u.ApePaterno) not like '%V_CTIMA%'" +
    "							and upper(u.ApePaterno) not like '%HIJ_%'" +
    "							and upper(u.ApeMaterno) not like '%MENOR%'" +
    "							and upper(u.ApeMaterno) not like '%V_CTIMA%'" +
    "							and upper(u.ApeMaterno) not like '%HIJ_%'" +
    "							and upper(u.Aclaracion) not like '%MENOR%'" +
    "							and upper(u.Aclaracion) not like '%V_CTIMA%'" +
    "							and upper(u.Aclaracion) not like '%HIJ_%'" +
    "							and upper(u.Nombre) not like '%AYUNTAMIENTO%'" +
    "							and upper(u.Nombre) not like '%MUNICIPIO%'" +
    "							and upper(u.Nombre) not like '%ESTADO%'" +
    "							and upper(u.Nombre) not like '%ESTATAL%'" +
    "							and upper(u.Nombre) not like '%FEDERAL%'" +
    "							and upper(u.Nombre) not like '%PROCURADOR%'" +
    "							and upper(u.Nombre) not like '%F_SCAL%'" +
    "							and upper(u.Nombre) not like '%GOBIERNO%'" +
    "							and upper(u.Nombre) not like '%SECRETARIO%'" +
    "							and upper(u.Nombre) not like '%SECRETAR_A%'" +
    "							and upper(u.Nombre) not like '%INSTITUTO%'" +
    "							and upper(u.Nombre) not like '%SALUD%'" +
    "							and upper(u.Nombre) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.Nombre) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.Nombre) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' " +
    "							and upper(u.ApePaterno) not like '%AYUNTAMIENTO%'" +
    "							and upper(u.ApePaterno) not like '%MUNICIPIO%'" +
    "							and upper(u.ApePaterno) not like '%ESTADO%'" +
    "							and upper(u.ApePaterno) not like '%ESTATAL%'" +
    "							and upper(u.ApePaterno) not like '%FEDERAL%'" +
    "							and upper(u.ApePaterno) not like '%PROCURADOR%'" +
    "							and upper(u.ApePaterno) not like '%F_SCAL%'" +
    "							and upper(u.ApePaterno) not like '%GOBIERNO%'" +
    "							and upper(u.ApePaterno) not like '%SECRETARIO%'" +
    "							and upper(u.ApePaterno) not like '%SECRETAR_A%'" +
    "							and upper(u.ApePaterno) not like '%INSTITUTO%'" +
    "							and upper(u.ApePaterno) not like '%SALUD%'" +
    "							and upper(u.ApePaterno) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.ApePaterno) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'" +
    "							and upper(u.ApeMaterno) not like '%AYUNTAMIENTO%'" +
    "							and upper(u.ApeMaterno) not like '%MUNICIPIO%'" +
    "							and upper(u.ApeMaterno) not like '%ESTADO%'" +
    "							and upper(u.ApeMaterno) not like '%ESTATAL%'" +
    "							and upper(u.ApeMaterno) not like '%FEDERAL%'" +
    "							and upper(u.ApeMaterno) not like '%PROCURADOR%'" +
    "							and upper(u.ApeMaterno) not like '%F_SCAL%'" +
    "							and upper(u.ApeMaterno) not like '%GOBIERNO%'" +
    "							and upper(u.ApeMaterno) not like '%SECRETARIO%'" +
    "							and upper(u.ApeMaterno) not like '%SECRETAR_A%'" +
    "							and upper(u.ApeMaterno) not like '%INSTITUTO%'" +
    "							and upper(u.ApeMaterno) not like '%SALUD%'" +
    "							and upper(u.ApeMaterno) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.ApeMaterno) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$'" +
    "							and upper(u.Aclaracion) not like '%AYUNTAMIENTO%'" +
    "							and upper(u.Aclaracion) not like '%MUNICIPIO%'" +
    "							and upper(u.Aclaracion) not like '%ESTADO%'" +
    "							and upper(u.Aclaracion) not like '%ESTATAL%'" +
    "							and upper(u.Aclaracion) not like '%FEDERAL%'" +
    "							and upper(u.Aclaracion) not like '%PROCURADOR%'" +
    "							and upper(u.Aclaracion) not like '%F_SCAL%'" +
    "							and upper(u.Aclaracion) not like '%GOBIERNO%'" +
    "							and upper(u.Aclaracion) not like '%SECRETARIO%'" +
    "							and upper(u.Aclaracion) not like '%SECRETAR_A%'" +
    "							and upper(u.Aclaracion) not like '%INSTITUTO%'" +
    "							and upper(u.Aclaracion) not like '%SALUD%'" +
    "							and upper(u.Aclaracion) not regexp ' I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp '^I[.,; ]S[.,; ]E[.,; ]M[.,; ] '" +
    "							and upper(u.Aclaracion) not regexp 'I[.,; ]S[.,; ]E[.,; ]M[.,; ]$' then 'Moral'" +
    "						when (upper(u.Nombre) like '%ASOCIACI_N%'" +
    "							or upper(u.Nombre) like '%SOCIEDAD%'" +
    "							or upper(u.ApePaterno) like '%ASOCIACI_N%'" +
    "							or upper(u.ApePaterno) like '%SOCIEDAD%'" +
    "							or upper(u.ApeMaterno) like '%ASOCIACI_N%'" +
    "							or upper(u.ApeMaterno) like '%SOCIEDAD%'" +
    "							or upper(u.Aclaracion) like '%ASOCIACI_N%'" +
    "							or upper(u.Aclaracion) like '%SOCIEDAD%'" +
    "							or upper(u.Nombre) regexp '^A[.,; ]C[.,; ]'" +
    "							or upper(u.Nombre) regexp ' A[.,; ]C[.,; ] '" +
    "							or upper(u.Nombre) regexp 'A[.,; ]C[.,; ]$'" +
    "							or upper(u.Nombre) regexp '^A[.,; ]R[.,; ]'" +
    "							or upper(u.Nombre) regexp ' A[.,; ]R[.,; ] '" +
    "							or upper(u.Nombre) regexp 'A[.,; ]R[.,; ]$'" +
    "							or upper(u.Nombre) regexp '^S[.,; ]A[.,; ]'" +
    "							or upper(u.Nombre) regexp ' S[.,; ]A[.,; ] '" +
    "							or upper(u.Nombre) regexp 'S[.,; ]A[.,; ]$'" +
    "							or upper(u.Nombre) regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							or upper(u.Nombre) regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							or upper(u.Nombre) regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							or upper(u.Nombre) regexp '^C[.,; ]V[.,; ]'" +
    "							or upper(u.Nombre) regexp ' C[.,; ]V[.,; ] '" +
    "							or upper(u.Nombre) regexp 'C[.,; ]V[.,; ]$'" +
    "							or upper(u.Nombre) regexp '^R[.,; ]L[.,; ]'" +
    "							or upper(u.Nombre) regexp ' R[.,; ]L[.,; ] '" +
    "							or upper(u.Nombre) regexp 'R[.,; ]L[.,; ]$'" +
    "							or upper(u.Nombre) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							or upper(u.Nombre) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							or upper(u.Nombre) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "							or upper(u.ApePaterno) regexp '^A[.,; ]C[.,; ]'" +
    "							or upper(u.ApePaterno) regexp ' A[.,; ]C[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'A[.,; ]C[.,; ]$'" +
    "							or upper(u.ApePaterno) regexp '^A[.,; ]R[.,; ]'" +
    "							or upper(u.ApePaterno) regexp ' A[.,; ]R[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'A[.,; ]R[.,; ]$'" +
    "							or upper(u.ApePaterno) regexp '^S[.,; ]A[.,; ]'" +
    "							or upper(u.ApePaterno) regexp ' S[.,; ]A[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'S[.,; ]A[.,; ]$'" +
    "							or upper(u.ApePaterno) regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							or upper(u.ApePaterno) regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							or upper(u.ApePaterno) regexp '^C[.,; ]V[.,; ]'" +
    "							or upper(u.ApePaterno) regexp ' C[.,; ]V[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'C[.,; ]V[.,; ]$'" +
    "							or upper(u.ApePaterno) regexp '^R[.,; ]L[.,; ]'" +
    "							or upper(u.ApePaterno) regexp ' R[.,; ]L[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'R[.,; ]L[.,; ]$'" +
    "							or upper(u.ApePaterno) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							or upper(u.ApePaterno) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							or upper(u.ApePaterno) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "							or upper(u.ApeMaterno) regexp '^A[.,; ]C[.,; ]'" +
    "							or upper(u.ApeMaterno) regexp ' A[.,; ]C[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'A[.,; ]C[.,; ]$'" +
    "							or upper(u.ApeMaterno) regexp '^A[.,; ]R[.,; ]'" +
    "							or upper(u.ApeMaterno) regexp ' A[.,; ]R[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'A[.,; ]R[.,; ]$'" +
    "							or upper(u.ApeMaterno) regexp '^S[.,; ]A[.,; ]'" +
    "							or upper(u.ApeMaterno) regexp ' S[.,; ]A[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'S[.,; ]A[.,; ]$'" +
    "							or upper(u.ApeMaterno) regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							or upper(u.ApeMaterno) regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							or upper(u.ApeMaterno) regexp '^C[.,; ]V[.,; ]'" +
    "							or upper(u.ApeMaterno) regexp ' C[.,; ]V[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'C[.,; ]V[.,; ]$'" +
    "							or upper(u.ApeMaterno) regexp '^R[.,; ]L[.,; ]'" +
    "							or upper(u.ApeMaterno) regexp ' R[.,; ]L[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'R[.,; ]L[.,; ]$'" +
    "							or upper(u.ApeMaterno) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							or upper(u.ApeMaterno) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							or upper(u.ApeMaterno) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "							or upper(u.Aclaracion) regexp '^A[.,; ]C[.,; ]'" +
    "							or upper(u.Aclaracion) regexp ' A[.,; ]C[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'A[.,; ]C[.,; ]$'" +
    "							or upper(u.Aclaracion) regexp '^A[.,; ]R[.,; ]'" +
    "							or upper(u.Aclaracion) regexp ' A[.,; ]R[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'A[.,; ]R[.,; ]$'" +
    "							or upper(u.Aclaracion) regexp '^S[.,; ]A[.,; ]'" +
    "							or upper(u.Aclaracion) regexp ' S[.,; ]A[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'S[.,; ]A[.,; ]$'" +
    "							or upper(u.Aclaracion) regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "							or upper(u.Aclaracion) regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "							or upper(u.Aclaracion) regexp '^C[.,; ]V[.,; ]'" +
    "							or upper(u.Aclaracion) regexp ' C[.,; ]V[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'C[.,; ]V[.,; ]$'" +
    "							or upper(u.Aclaracion) regexp '^R[.,; ]L[.,; ]'" +
    "							or upper(u.Aclaracion) regexp ' R[.,; ]L[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'R[.,; ]L[.,; ]$'" +
    "							or upper(u.Aclaracion) regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "							or upper(u.Aclaracion) regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "							or upper(u.Aclaracion) regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$') then 'Moral' " +
    "						when (u.Sexo in ('H','M')" +
    "							and (upper(u.Nombre) like '%MENOR%'" +
    "								or upper(u.Nombre) like '%V_CTIMA%'" +
    "								or upper(u.Nombre) like '%HIJ_%'" +
    "								or upper(u.ApePaterno) like '%MENOR%'" +
    "								or upper(u.ApePaterno) like '%V_CTIMA%'" +
    "								or upper(u.ApePaterno) like '%HIJ_%'" +
    "								or upper(u.ApeMaterno) like '%MENOR%'" +
    "								or upper(u.ApeMaterno) like '%V_CTIMA%'" +
    "								or upper(u.ApeMaterno) like '%HIJ_%'" +
    "								or upper(u.Aclaracion) like '%MENOR%'" +
    "								or upper(u.Aclaracion) like '%V_CTIMA%'" +
    "								or upper(u.Aclaracion) like '%HIJ_%')) then 'Física' " +
    "						when (u.Sexo in ('H','M')" +
    "							and (upper(u.Nombre) not like '%ASOCIACI_N%'" +
    "								and upper(u.Nombre) not like '%SOCIEDAD%'" +
    "								and upper(u.ApePaterno) not like '%ASOCIACI_N%'" +
    "								and upper(u.ApePaterno) not like '%SOCIEDAD%'" +
    "								and upper(u.ApeMaterno) not like '%ASOCIACI_N%'" +
    "								and upper(u.ApeMaterno) not like '%SOCIEDAD%'" +
    "								and upper(u.Aclaracion) not like '%ASOCIACI_N%'" +
    "								and upper(u.Aclaracion) not like '%SOCIEDAD%'" +
    "								and upper(u.Nombre) not regexp '^A[.,; ]C[.,; ]'" +
    "								and upper(u.Nombre) not regexp ' A[.,; ]C[.,; ] '" +
    "								and upper(u.Nombre) not regexp 'A[.,; ]C[.,; ]$'" +
    "								and upper(u.Nombre) not regexp '^A[.,; ]R[.,; ]'" +
    "								and upper(u.Nombre) not regexp ' A[.,; ]R[.,; ] '" +
    "								and upper(u.Nombre) not regexp 'A[.,; ]R[.,; ]$'" +
    "								and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]'" +
    "								and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ] '" +
    "								and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]$'" +
    "								and upper(u.Nombre) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "								and upper(u.Nombre) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "								and upper(u.Nombre) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "								and upper(u.Nombre) not regexp '^C[.,; ]V[.,; ]'" +
    "								and upper(u.Nombre) not regexp ' C[.,; ]V[.,; ] '" +
    "								and upper(u.Nombre) not regexp 'C[.,; ]V[.,; ]$'" +
    "								and upper(u.Nombre) not regexp '^R[.,; ]L[.,; ]'" +
    "								and upper(u.Nombre) not regexp ' R[.,; ]L[.,; ] '" +
    "								and upper(u.Nombre) not regexp 'R[.,; ]L[.,; ]$'" +
    "								and upper(u.Nombre) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "								and upper(u.Nombre) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "								and upper(u.Nombre) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "								and upper(u.ApePaterno) not regexp '^A[.,; ]C[.,; ]'" +
    "								and upper(u.ApePaterno) not regexp ' A[.,; ]C[.,; ] '" +
    "								and upper(u.ApePaterno) not regexp 'A[.,; ]C[.,; ]$'" +
    "								and upper(u.ApePaterno) not regexp '^A[.,; ]R[.,; ]'" +
    "								and upper(u.ApePaterno) not regexp ' A[.,; ]R[.,; ] '" +
    "								and upper(u.ApePaterno) not regexp 'A[.,; ]R[.,; ]$'" +
    "								and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]'" +
    "								and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ] '" +
    "								and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]$'" +
    "								and upper(u.ApePaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "								and upper(u.ApePaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "								and upper(u.ApePaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "								and upper(u.ApePaterno) not regexp '^C[.,; ]V[.,; ]'" +
    "								and upper(u.ApePaterno) not regexp ' C[.,; ]V[.,; ] '" +
    "								and upper(u.ApePaterno) not regexp 'C[.,; ]V[.,; ]$'" +
    "								and upper(u.ApePaterno) not regexp '^R[.,; ]L[.,; ]'" +
    "								and upper(u.ApePaterno) not regexp ' R[.,; ]L[.,; ] '" +
    "								and upper(u.ApePaterno) not regexp 'R[.,; ]L[.,; ]$'" +
    "								and upper(u.ApePaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "								and upper(u.ApePaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "								and upper(u.ApePaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "								and upper(u.ApeMaterno) not regexp '^A[.,; ]C[.,; ]'" +
    "								and upper(u.ApeMaterno) not regexp ' A[.,; ]C[.,; ] '" +
    "								and upper(u.ApeMaterno) not regexp 'A[.,; ]C[.,; ]$'" +
    "								and upper(u.ApeMaterno) not regexp '^A[.,; ]R[.,; ]'" +
    "								and upper(u.ApeMaterno) not regexp ' A[.,; ]R[.,; ] '" +
    "								and upper(u.ApeMaterno) not regexp 'A[.,; ]R[.,; ]$'" +
    "								and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]'" +
    "								and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ] '" +
    "								and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]$'" +
    "								and upper(u.ApeMaterno) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "								and upper(u.ApeMaterno) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "								and upper(u.ApeMaterno) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "								and upper(u.ApeMaterno) not regexp '^C[.,; ]V[.,; ]'" +
    "								and upper(u.ApeMaterno) not regexp ' C[.,; ]V[.,; ] '" +
    "								and upper(u.ApeMaterno) not regexp 'C[.,; ]V[.,; ]$'" +
    "								and upper(u.ApeMaterno) not regexp '^R[.,; ]L[.,; ]'" +
    "								and upper(u.ApeMaterno) not regexp ' R[.,; ]L[.,; ] '" +
    "								and upper(u.ApeMaterno) not regexp 'R[.,; ]L[.,; ]$'" +
    "								and upper(u.ApeMaterno) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "								and upper(u.ApeMaterno) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "								and upper(u.ApeMaterno) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$'" +
    "								and upper(u.Aclaracion) not regexp '^A[.,; ]C[.,; ]'" +
    "								and upper(u.Aclaracion) not regexp ' A[.,; ]C[.,; ] '" +
    "								and upper(u.Aclaracion) not regexp 'A[.,; ]C[.,; ]$'" +
    "								and upper(u.Aclaracion) not regexp '^A[.,; ]R[.,; ]'" +
    "								and upper(u.Aclaracion) not regexp ' A[.,; ]R[.,; ] '" +
    "								and upper(u.Aclaracion) not regexp 'A[.,; ]R[.,; ]$'" +
    "								and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]'" +
    "								and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ] '" +
    "								and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]$'" +
    "								and upper(u.Aclaracion) not regexp '^S[.,; ]A[.,; ]B[.,; ]'" +
    "								and upper(u.Aclaracion) not regexp ' S[.,; ]A[.,; ]B[.,; ] '" +
    "								and upper(u.Aclaracion) not regexp 'S[.,; ]A[.,; ]B[.,; ]$'" +
    "								and upper(u.Aclaracion) not regexp '^C[.,; ]V[.,; ]'" +
    "								and upper(u.Aclaracion) not regexp ' C[.,; ]V[.,; ] '" +
    "								and upper(u.Aclaracion) not regexp 'C[.,; ]V[.,; ]$'" +
    "								and upper(u.Aclaracion) not regexp '^R[.,; ]L[.,; ]'" +
    "								and upper(u.Aclaracion) not regexp ' R[.,; ]L[.,; ] '" +
    "								and upper(u.Aclaracion) not regexp 'R[.,; ]L[.,; ]$'" +
    "								and upper(u.Aclaracion) not regexp '^S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]'" +
    "								and upper(u.Aclaracion) not regexp ' S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ] '" +
    "								and upper(u.Aclaracion) not regexp 'S[.,; ]O[.,; ]F[.,; ]O[.,; ]M[.,; ]$')) then 'Física' " +
    "						else 'N/Identificado' end as 'tPersona'" +
    "				from	htsj_mediacion.tblusuario as u) as us on e.CveExp = us.CveExp" +
    "				where	e.FechaTer between '" + fechaPeriodoInicio + "' and '" + fechaPeriodoFin + "'  and e.idCentroM in (" + idsCentrosMediacionConsulta + ") and n.Descripcion not like 'JR %' " +
    "			group by e.idTipoTer ";
            /*" where	e.FechaTer between '" + fechaPeriodoInicio + "' and '" + fechaPeriodoFin + "'  and e.idCentroM in (" + idsCentrosMediacionConsulta + ") and n.Descripcion not like 'JR %' "
               + "			group by naturaleza;";*/
            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["bd67"]);
                con.Open();
                cmd = new MySqlCommand(consulta, con);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader resultado = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(resultado);
                tablaMediacion8.DataSource = dt;
                tablaMediacion8.DataBind();
                rep.Titulo = "Número de invitados por sexo, respecto a la causa de solución";
                rep.Tabla = dt;
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
            return rep;
        }




        public void generarInformacionExcel(List<ModeloTablaReporte> listaTablas)
        {
            try
            {
                int totalCols = 0;
                int[] celdaInicioTabla = { 6 , 2 };
                int espacioTablas = 2;
                String nombreCentro = "";
                if (idCentroMediacion.Text.Equals("22")) {
                    nombreCentro = "La Paz";
                } else if (idCentroMediacion.Text.Equals("21")) {
                    nombreCentro = "San Mateo";
                } else if (idCentroMediacion.Text.Equals("999")) {
                    nombreCentro = "San Mateo y La Paz";
                }

                ExcelPackage excel = null;
                excel = new ExcelPackage();

                ExcelWorksheet hoja;
                hoja = excel.Workbook.Worksheets.Add("Hoja 1");

                hoja.Cells[celdaInicioTabla[0] - 5, celdaInicioTabla[1] ].Value = "CIUDAD MUJERES";
                hoja.Cells[celdaInicioTabla[0] - 4, celdaInicioTabla[1]].Value = "Periodo de la información:  " + fechaPeriodoInicio + "  al  " + fechaPeriodoFin;
                hoja.Cells[celdaInicioTabla[0] - 3, celdaInicioTabla[1]].Value = "Fuente:  Sistema de Centro de Mediación, Conciliación y Justicia Restaurativa";
                hoja.Cells[celdaInicioTabla[0] - 2, celdaInicioTabla[1]].Value = "Centro de Mediación:  " + nombreCentro ;

                for (int i = 0; i < listaTablas.Count; i++)
                {
                    Session["celdasMerge"] = new List<ModeloRangoCelda>();
                    DataTable tabla = listaTablas[i].Tabla;
                    tabla.TableName = "Tabla_excel_" + i;
                    String[] temas = listaTablas[i].Titulo.Split('|');  //Obtener temas y subtemas y agregarlos a la hoja

                    totalCols = tabla.Columns.Count > totalCols ? tabla.Columns.Count : totalCols;
                    int totalFilasTable = listaTablas[i].Tabla == null ? tabla.Rows.Count : listaTablas[i].Tabla.Rows.Count;

                    //Agregar título y/o subtitulo de la tabla
                    for (int j = 0; j < temas.Length; j++)
                    {
                        hoja.Cells[celdaInicioTabla[0] + (j + 1), celdaInicioTabla[1]].Value = temas[j]; //Agregar texto en celda especificada
                        hoja.Cells[celdaInicioTabla[0] + (j + 1), celdaInicioTabla[1]].Style.Font.Bold = true;
                    }

                    celdaInicioTabla[0] = celdaInicioTabla[0] + temas.Length + espacioTablas; //Aumentar fila


                    hoja.Cells[celdaInicioTabla[0], celdaInicioTabla[1]].LoadFromDataTable(tabla, true);  // Cargar tabla en la hoja excel
                    hoja = aplicarEstilosTablasExcel(hoja, i, totalFilasTable, tabla.Columns.Count - 1 , celdaInicioTabla); //Aplicar estilos a los rangos de celdas de las tablas


                    // Modificar celdas de inicio para la siguiente tabla
                    
                    celdaInicioTabla[0] = celdaInicioTabla[0] + totalFilasTable + 4 + temas.Length;
                    celdaInicioTabla[1] = 2;
                }

                //Ajustar texto de la columna de inicio y fijar tamaño de la columna
                hoja.Column(celdaInicioTabla[1]).Width = 70;

                for (int i = 1; i < (celdaInicioTabla[1] + totalCols); i++)
                {
                    hoja.Column(celdaInicioTabla[1] + i).Width = 20;
                    hoja.Column(celdaInicioTabla[1] + 1 + i).Style.WrapText = true;
                }

                //Seteo que la celda A1 va a ser Editable.
                hoja.Cells["A1"].Style.Locked = false;
                //Seteo la contraseña para desbloquear la hoja.
                hoja.Protection.SetPassword("159753");
                //Activo la protección
                hoja.Protection.IsProtected = true;


                //DESCARGAR EXCEL
                descargarExcel(excel);
            }
            catch (Exception error)
            {
                Console.WriteLine("ERROR: " + error.Message);
            }

        }



        private ExcelWorksheet aplicarEstilosTablasExcel(ExcelWorksheet hojaFinal, int consecutivo, int totalRows , int totalCols,int[] inicio)
        {

            String celdaInicioHeader = "", celdaFinHeader = "", celdaInicioTabla = "", celdaFinTabla = "";
            try
            {
                    int[] finHeader = { (inicio[0]), (inicio[1] + totalCols) };
                    int[] inicioBody = { (inicio[0]), (inicio[1] + totalCols) };
                    int[] finBody= { inicio[0], (inicio[1] + totalCols) };

                    celdaInicioHeader = hojaFinal.Cells[inicio[0], inicio[1]].Address;
                    celdaFinHeader = hojaFinal.Cells[finHeader[0], finHeader[1]].Address;

                    celdaInicioTabla = hojaFinal.Cells[inicioBody[0], inicioBody[1]].Address;
                    celdaFinTabla = hojaFinal.Cells[finBody[0] , finBody[0]].Address;

                    hojaFinal.Cells[celdaInicioHeader + ":" + celdaFinHeader].Style.Font.Color.SetColor(ColorTranslator.FromHtml("WHITE"));
                    hojaFinal.Cells[celdaInicioHeader + ":" + celdaFinHeader].Style.Font.Bold = true;
                    hojaFinal.Cells[celdaInicioHeader + ":" + celdaFinHeader].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hojaFinal.Cells[celdaInicioHeader + ":" + celdaFinHeader].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#b71c1c"));
                    hojaFinal.Cells[celdaInicioHeader + ":" + celdaFinHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                //Agregar bordes a la tabla
                hojaFinal.Cells[hojaFinal.Dimension.Address].AutoFitColumns();


            }
            catch (Exception error)
            {

                Console.WriteLine("Error en aplicarEstilos " + error.ToString());
            }


            return hojaFinal;

        }






        private void descargarExcel(ExcelPackage archivo)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Reporte_Mediacion.xlsx");
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

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            this.obtenerFechasPeriodoInformacionMediacion();
            List<ModeloTablaReporte> listaRep = new List<ModeloTablaReporte>();
            listaRep.Add(consultarMediacionServicio());
            listaRep.Add(consultarMediacionIniciadosPorNaturaleza());
            listaRep.Add(consultarMediacionSolicitantesSesiones());
            listaRep.Add(consultarMediacionPersonasInvitadasSesiones());
            listaRep.Add(consultarMediacionConcluidosPorNaturaleza());
            listaRep.Add(consultarMediacionConcluidosPorNaturalezaTipoSolucion());
            listaRep.Add(consultarMediacionSolicitantesCausaSolucion());
            listaRep.Add(consultarMediacionInvitadosCausaSolucion());

            generarInformacionExcel(listaRep);
        }

        protected void menuCiudad_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelMediacion.Visible = false;
            panelParentalidad.Visible = false;
            panelUnidadIgualdad.Visible = false;
            panelAyC.Visible = false;
            //UpdatePanelCanalizacion.Visible = false;
            Session["idRegistro"] = 0;
            consultarRegistrosPorDia();
            getInformacionCoordinacionParentalidad();
            limpiarFormularios();

            String valorSeleccionado = menuCiudad.SelectedValue;
            
            try
            {

                Console.WriteLine("Selecciono valor: " + valorSeleccionado);

                Panel nombrePanel = (Panel)panelCiudadMujeres.FindControl(valorSeleccionado);
                nombrePanel.Visible = true;
            }
            catch (Exception error)
            {

                Console.WriteLine("Error :" +  error.ToString());
            }
          

        }

        protected void ButtonBuscarCanalicacion_Click(object sender, EventArgs e)
        {

        }
    }
}