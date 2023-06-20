<%@ Page Title="" Language="C#" MasterPageFile="~/Master1.Master" AutoEventWireup="true" CodeBehind="IgualdadCaptura.aspx.cs" Inherits="SistemaIntegralEstadistica.Vista.UnidadIgualdad.IgualdadCaptura" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Panel1" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Panel ID="panelCiudadMujeres" runat="server">

        <h1>Ciudad Mujeres</h1>
            <asp:Panel runat="server" ID="panelMediacion" Visible="false">
        <asp:Panel runat="server" ID="contentMediacion">
            <h1>
                Mediación
            </h1>

        </asp:Panel>
    </asp:Panel>


    <asp:Panel ID="panelUnidadTransparencia" runat="server">



    <br /><br />
     <h1>Unidad de Igualdad y Derechos Humanos
            </h1>
            
    <br />

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>



    <asp:Panel runat="server">
                <div class="row">
                    <div class="col">
                        <asp:Label runat="server" Text="Periodo de información:"></asp:Label>
                    </div>

                    <div class="col">
                            <asp:TextBox runat="server" ID="periodoInicio" class="form-control"  type="date" ValidationGroup="grupoBusqueda"></asp:TextBox>
                    </div>

                    <div class="col">
                        <asp:TextBox runat="server" ID="periodoFin" class="form-control"  type ="date" ValidationGroup="grupoBusqueda"></asp:TextBox>
                    </div>
                     <div class="col">
                        <asp:Button runat="server" ID="BuscarInfo" OnClick="BuscarInfo_Click1" Text="Buscar" ValidationGroup="grupoBusqueda" class="btn btn-danger" />
                     </div>   
                </div>
                <br />
                <asp:GridView runat="server" ID="tablaUT" ClientIDMode="Static" class="table table-sm table-dark">
                </asp:GridView>
                <asp:DataList ID="listaDHUT" runat="server"></asp:DataList>
            </asp:Panel>


<%--    <asp:UpdatePanel runat="server"> 
        <ContentTemplate>
--%>



<!-- Modal -->
<asp:Panel runat="server" class="modal fade" ID="exampleModalCenter" ClientIDMode="Static" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
  
    <div class="modal-dialog modal-dialog-centered" role="document">
    <div class="modal-content">
      <div class="modal-header"> 
        <h5 class="modal-title" id="exampleModalCenterTitle">Nuevo Registro</h5>
        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
      </div>
      <div class="modal-body">
                <br />
            <div class="form-group row">
                <label  class="col-sm-4 col-form-label">Fecha de registro</label>
                <div class="col-sm-8">
                    <asp:TextBox ID="fechaRegistro" runat="server" type="date" class="form-control"  ValidationGroup ="grupoUT"></asp:TextBox>
                </div>
            </div>
            <br />
            <table class="table">
                <thead class="thead-dark">
                    <tr>
                        <th scope="col">Servicio</th>
                        <th scope="col">Mujeres</th>
                        <th scope="col">Hombres</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <th scope="row">
                            <asp:DropDownList runat="server" ID="listaTipo" class="form-control" ValidationGroup ="grupoUT"></asp:DropDownList>
                        </th>
                        <td>
                            <asp:TextBox ID="totalHombres" runat="server" class="form-control" type="numer" min="0" max="100"   ValidationGroup ="grupoUT"></asp:TextBox>

                        </td>
                        <td>
                            <asp:TextBox ID="totalMujeres" runat="server" class="form-control" type="numer" min="0" max="100"  ValidationGroup ="grupoUT"></asp:TextBox>
                        </td>
                    </tr>
                </tbody>
            </table>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
            <asp:Button ID="botonGuardar"  runat="server" class="btn btn-primary" OnClick="Guardar_Click" Text="Guardar" ValidationGroup ="grupoUT" CausesValidation="false"/>
      </div>
    </div>
  </div>
</asp:Panel>






    <br />

    <asp:Panel runat="server" Visible="true">


        <asp:Panel ID="registroInformacion" runat="server">
           

                        <!-- Button trigger modal -->
<%--<asp:Button runat="server"  class="btn btn-primary" data-toggle="modal" data-target="#exampleModalCenter" Text="Agregar">

</asp:Button>--%>

            <button type="button" class="btn btn-secondary" data-toggle="modal" data-target="#exampleModalCenter" >
  Agregar
</button>

            <br />
            <asp:GridView runat="server" ID ="listaDatos" class="table table-sm"
                AutoGenerateColumns="false" AllowPaging="true" PageSize="5"
                OnRowDeleting="listaDatos_RowDeleting"
                OnPageIndexChanging="listaDatos_PageIndexChanging" 
                OnRowDataBound="listaDatos_RowDataBound"
                OnRowCommand ="listaDatos_RowCommand"
                DataKeyNames="clave" >
                 <Columns>  
                     
                        <asp:BoundField DataField="fecha" HeaderText="FECHA" />  
                        <asp:BoundField DataField="descripcion" HeaderText="TIPO" />  
                        <asp:BoundField DataField="numeroHombres" HeaderText="HOMBRES" />  
                        <asp:BoundField DataField="numeroMujeres" HeaderText="MUJERES" />  
                        <asp:BoundField DataField="total" HeaderText="TOTAL" /> 
                        
                        <asp:ButtonField ButtonType="Button" CommandName="EditarRegistro" Text="Editar"  />

                        <asp:CommandField ShowDeleteButton="true" /> 
                 </Columns>  
            </asp:GridView>
        
        </asp:Panel>

    </asp:Panel> <!-- -->

<%--        </ContentTemplate>
   

    </asp:UpdatePanel>
--%>



    </asp:Panel>













    <br />

    <asp:Panel runat="server" ID="panelAyC">

        <asp:Panel ID="contenidoAyC" runat="server">
            
            <h1>Atención y Canalización</h1>
            <br /><br />
            <div class="form-group row">
                <label for="fechaAC" class="col-sm-2 col-form-label">Fecha:</label>
                <div class="col-sm-10">
                    <asp:TextBox runat="server" type="date" class="form-control" ID="fechaAC" ValidationGroup="grupoAC"> </asp:TextBox>
                </div>
            </div>
            <div class="form-group row">
                <label for="expedienteAC" class="col-sm-2 col-form-label">No. Expediente:</label>
                <div class="col-sm-10">
                    <asp:TextBox runat="server" type="text" class="form-control" ID="expedienteAC" placeholder="0/0000" ValidationGroup="grupoAC"></asp:TextBox>
                </div>
            </div>
            <div class="form-group row">
                <label for="nombrePersona" class="col-sm-2 col-form-label">Nombre:</label>
                <div class="col-sm-3">
                    <asp:TextBox runat="server" type="text" class="form-control" ID="nombrePersona" ValidationGroup="grupoAC"></asp:TextBox>
                </div>
                 <div class="col-sm-3">
                    <asp:TextBox runat="server" type="text" class="form-control" ID="primerApellido" ValidationGroup="grupoAC"></asp:TextBox>
                </div>
                 <div class="col-sm-3">
                    <asp:TextBox runat="server" type="text" class="form-control" ID="segundoApellido" ValidationGroup="grupoAC"></asp:TextBox>
                </div>
            </div>

            <asp:Button ID="ButtonAC" runat="server" class="btn btn-primary" OnClick="ButtonAC_Click" Text="Guardar" ValidationGroup="grupoAC" CausesValidation="false" />

        </asp:Panel>

        <asp:GridView runat="server" ID="tablaAtencionCiudadana" ClientIDMode="Static" class="table table-sm table-dark">
                </asp:GridView>

    </asp:Panel>



    <asp:Panel runat="server" ID="panelAtencionCiudadana">
        <asp:Panel runat="server" ID="contentAtencionCiudadana">
            <h1>
                Atención Ciudadana
            </h1>

        </asp:Panel>
    </asp:Panel>


        <asp:Panel runat="server" ID="panelCoordinacionPar">
        <asp:Panel runat="server" ID="contentCoordinacionPar">
            <h1>
               Coordinación de Parentalidad
            </h1>

        </asp:Panel>
    </asp:Panel>


</asp:Panel>

</asp:Content>
