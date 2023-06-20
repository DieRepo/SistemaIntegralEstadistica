using MySql.Data.MySqlClient;
using SistemaIntegralEstadistica.Controlador;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SistemaIntegralEstadistica.Vista.UnidadIgualdad
{
    public partial class IgualdadCaptura : System.Web.UI.Page
    {
        MySqlCommand cmd;
        MySqlDataReader r;
        String sql;
        string pattern = "yyyy-MM-dd";

        DateTime fechaActual;
        String fechaDiaActual;
        String fechaPeriodoInicio;
        String fechaPeriodoFin;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack) {
                consultarCatalogo();
                consultarRegistrosPorDia();
                obtenerDatosAlmacenadosProcedure("GetTotalServiciosUIDH", "1" , "tablaUT");
                obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", "2", "tablaAtencionCiudadana");
            }
            Console.WriteLine("Termina page_load");
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
            String consulta = "SELECT a.id clave , fecha , a.idTipo , b.descripcion , a.numeroHombres , a.numeroMujeres , a.total FROM tblciudaduidhservicios a" +
                "    LEFT JOIN tblcatuidhservicio b" +
                "    ON a.idTipo = b.id" +
                "    where a.activo = 1 order by  1 desc, 2; ";
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
            insertarInformacion();

        }


        private void insertarInformacion()
        {
            long id = 0;
            MySqlConnection con = null;
            int tipoOrientacion = Convert.ToInt32(listaTipo.SelectedValue);

            String query = "";
            try
            {
                query = " INSERT into  tblciudaduidhservicios(fecha,idTipo,numeroHombres,numeroMujeres,total) VALUES ('"+
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


        public void limpiarFormularios() {
            fechaRegistro.Text = "";
            totalHombres.Text = "";
            totalMujeres.Text = "";
        }

        public void obtenerFechasPeriodoInformacion() {
            fechaActual = DateTime.Now;
            fechaDiaActual = fechaActual.ToString(pattern);
            fechaPeriodoInicio = periodoInicio.Text.Equals("") ? fechaDiaActual : periodoInicio.Text;
            fechaPeriodoFin = periodoFin.Text.Equals("") ? fechaDiaActual : periodoFin.Text;
        }


        private void obtenerDatosAlmacenadosProcedure(String nombreProcedure, String id, String nombreTabla)
        {
            MySqlConnection con = null;
            String parameter = "a";
            this.obtenerFechasPeriodoInformacion();

            DataTable dt = new DataTable("Tabla_"+nombreProcedure+"_"+id);
            try
            {
                con = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["local"]);
                con.Open();
                cmd = new MySqlCommand(nombreProcedure, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd = agregaParametros(cmd , nombreProcedure, id);    
                MySqlDataReader resultado = cmd.ExecuteReader();

                dt.Load(resultado);
                GridView table = (GridView)panelCiudadMujeres.FindControl(nombreTabla);
                table.DataSource = dt;
                table.DataBind();
                //tablaUT.DataSource = dt;
                //tablaUT.DataBind();
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

        public MySqlCommand agregaParametros(MySqlCommand command, String nombre, String id) {

            if (nombre.Equals("GetTotalServiciosUIDH")) {
                cmd.Parameters.AddWithValue("tipo", id);
                cmd.Parameters.AddWithValue("param", id);
                cmd.Parameters.AddWithValue("fechaInicio", fechaPeriodoInicio);
                cmd.Parameters.AddWithValue("fechaFin", fechaPeriodoFin);
            } else if (nombre.Equals("obtenInformacionCiudadMujeres")) {
                cmd.Parameters.AddWithValue("_id", id);
                cmd.Parameters.AddWithValue("_fechaIni", fechaPeriodoInicio);
                cmd.Parameters.AddWithValue("_fechaFin", fechaPeriodoFin);
            }


            return command;
        }

        protected void listaDatos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].Text = Convert.ToDateTime(e.Row.Cells[0].Text).ToString(pattern);
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

                String sql = "UPDATE tbciudaduidhservicios SET activo = 0 where id = " + id + "; ";
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
            String valor = e.CommandArgument.ToString();
            try
            {
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
          

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Mostrar información ");


        }

        protected void BuscarInfo_Click1(object sender, EventArgs e)
        {
            Console.WriteLine("Empieza búsqueda");
            obtenerDatosAlmacenadosProcedure("GetTotalServiciosUIDH", "1","tablaUT");
            Console.WriteLine("Buscar");
        }

        protected void ButtonAC_Click(object sender, EventArgs e)
        {
            this.insertarInformacionAC();
        }

        private void insertarInformacionAC()
        {
            long id = 0;
            MySqlConnection con = null;

            String query = "";
            try
            {
                query = " INSERT into  tblciudadatencion(fecha,expediente,nombre,primerApellido,segundoApellido) VALUES ('" +
                   fechaAC.Text + "' ,'" + expedienteAC.Text + "' , '" + nombrePersona.Text + "' , '" + primerApellido.Text + "' , '" + segundoApellido.Text +"' ) ";

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
                obtenerDatosAlmacenadosProcedure("obtenInformacionCiudadMujeres", "2", "tablaAtencionCiudadana");

                limpiarFormularios();
                //exampleModalCenter.Visible = false;
            }

            //return id;
        }
    }
}