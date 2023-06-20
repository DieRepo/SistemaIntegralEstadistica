<%@ Page Title="" Language="C#" MasterPageFile="~/Master1.Master" AutoEventWireup="true" CodeBehind="CuidadMujeres.aspx.cs" Inherits="SistemaIntegralEstadistica.Vista.CuidadMujeres" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Panel1" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server" />


    <script>
        const container = document.getElementById("myModal");
        const modal = new bootstrap.Modal(container);

        document.getElementById("botonGuardar").addEventListener("click", function () {
            modal.hide();
        });

        $(function () {
                $('#v-pills-tab li:last-child input').tab('show')
            })
    </script>

    <asp:Panel ID="panelCiudadMujeres" runat="server">

<div class="row">
  <div class="col-2">
    <div class="nav flex-column nav-pills" id="v-pills-tab" role="tablist" aria-orientation="vertical">
      <button class="nav-link active" id="v-pills-home-tab" data-toggle="pill" data-target="#v-pills-home" type="button" role="tab" aria-controls="v-pills-home" aria-selected="true">Home</button>
      <button class="nav-link" id="v-pills-profile-tab" data-toggle="pill" data-target="#v-pills-profile" type="button" role="tab" aria-controls="v-pills-profile" aria-selected="false">Profile</button>
      <button class="nav-link" id="v-pills-messages-tab" data-toggle="pill" data-target="#v-pills-messages" type="button" role="tab" aria-controls="v-pills-messages" aria-selected="false">Messages</button>
      <button class="nav-link" id="v-pills-settings-tab" data-toggle="pill" data-target="#v-pills-settings" type="button" role="tab" aria-controls="v-pills-settings" aria-selected="false">Settings</button>
    </div>
  </div>
  <div class="col-9">
    <div class="tab-content" id="v-pills-tabContent">
      <div class="tab-pane fade show active" id="v-pills-home" role="tabpanel" aria-labelledby="v-pills-home-tab">.a..</div>
      <div class="tab-pane fade" id="v-pills-profile" role="tabpanel" aria-labelledby="v-pills-profile-tab">..b.</div>
      <div class="tab-pane fade" id="v-pills-messages" role="tabpanel" aria-labelledby="v-pills-messages-tab">. c..</div>
      <div class="tab-pane fade" id="v-pills-settings" role="tabpanel" aria-labelledby="v-pills-settings-tab">..d.</div>
    </div>
  </div>
</div>


        <br />
        <h2>Ciudad Mujeres</h2>

        <asp:Panel ID="panelUnidadIgualdad" runat="server">



            <br />
            <br />
            <h3>Unidad de Igualdad y Derechos Humanos
            </h3>

            <br />


            <br />

            <%--            <asp:Panel runat="server" Visible="true">

                <asp:Panel ID="registroInformacion" runat="server">--%>

            <button type="button" class="btn btn-secondary" data-toggle="modal" data-target="#myModal">
                Agregar
            </button>

            <br />

            <asp:UpdatePanel runat="server" ChildrenAsTriggers="true" ID="updatePanelRegistro">
                <ContentTemplate>

                    <asp:GridView runat="server" ID="listaDatos" class="table table-sm"
                        AutoGenerateColumns="false" AllowPaging="true" PageSize="5"
                        OnRowDeleting="listaDatos_RowDeleting"
                        OnPageIndexChanging="listaDatos_PageIndexChanging"
                        OnRowDataBound="listaDatos_RowDataBound"
                        OnRowCommand="listaDatos_RowCommand"
                        DataKeyNames="ID">
                        <Columns>
                            <asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-CssClass="ColumnaOculta" HeaderStyle-CssClass="ColumnaOculta" />
                            <asp:BoundField DataField="fecha" HeaderText="FECHA" />
                            <asp:BoundField DataField="idTipo" HeaderText="ID_TIPO" ItemStyle-CssClass="ColumnaOculta" HeaderStyle-CssClass="ColumnaOculta" />
                            <asp:BoundField DataField="descripcion" HeaderText="TIPO" />
                            <asp:BoundField DataField="numeroHombres" HeaderText="HOMBRES" />
                            <asp:BoundField DataField="numeroMujeres" HeaderText="MUJERES" />
                            <asp:BoundField DataField="total" HeaderText="TOTAL" />

                            <asp:ButtonField ButtonType="Button" CommandName="EditarRegistro" Text="Editar"  ItemStyle-CssClass="btn btn-light" />

                            <asp:CommandField ShowDeleteButton="true" />
                        </Columns>

                        <%--                        <Columns>
                            <asp:TemplateField HeaderText="Banquet Name">
                                <ItemTemplate>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <center>Events</center>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Button ID="btnEdiit" runat="server" CssClass="btn btn-danger btn-sm" CommandName="EditarRegistro" Text="Edit" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>--%>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>

            <%--                </asp:Panel>
                               


            </asp:Panel>--%>

            <br />
            
            <asp:Panel runat="server" ID="panelConsultaPeriodoDerechosHumanos">
                <div class="row">
                    <div class="col">
                        <asp:Label runat="server" Text="Periodo de información:"></asp:Label>
                    </div>

                    <div class="col">
                        <asp:TextBox runat="server" ID="periodoInicio" class="form-control" type="date" ValidationGroup="grupoBusqueda"></asp:TextBox>
                    </div>

                    <div class="col">
                        <asp:TextBox runat="server" ID="periodoFin" class="form-control" type="date" ValidationGroup="grupoBusqueda"></asp:TextBox>
                    </div>
                    <div class="col">
                        <asp:Button runat="server" ID="BuscarInfo" OnClick="BuscarInfo_Click1" Text="Buscar" ValidationGroup="grupoBusqueda" class="btn btn-danger" />
                    </div>
                </div>
                <br />
                <div class="col-lg-8 table-responsive">
                    <asp:GridView runat="server" ID="tablaUT" ClientIDMode="Static" class="table table-sm table-dark">
                    </asp:GridView>
                    <asp:DataList ID="listaDHUT" runat="server"></asp:DataList>
                </div>
            </asp:Panel>



            <!-- Modal  UNIDAD DE IGUALDAD Y DERECHOS HUMANOS -->

            <asp:UpdatePanel ID="UpdatePanelCM" runat="server">
                <ContentTemplate>
                    <asp:Panel runat="server" class="modal fade" ID="myModal" ClientIDMode="Static" TabIndex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">

                        <div class="modal-dialog modal-dialog-centered" role="document">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <asp:Label runat="server" class="modal-title" ID="exampleModalCenterTitle">-</asp:Label>
                                    <button type="button" class="close" data-dismiss="modal" data-backdrop="false" aria-label="Close">
                                        <span aria-hidden="true">&times;</span>
                                    </button>
                                </div>
                                <div class="modal-body">
                                    <br />
                                    <div class="form-group row">
                                        <label class="col-sm-4 col-form-label">Fecha de registro</label>
                                        <div class="col-sm-8">
                                            <asp:TextBox ID="fechaRegistro" runat="server" type="date" class="form-control" ValidationGroup="grupoUT"></asp:TextBox>
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
                                                    <asp:DropDownList runat="server" ID="listaTipo" class="form-control" ValidationGroup="grupoUT"></asp:DropDownList>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="totalHombres" runat="server" class="form-control" type="numer" min="0" max="100" ValidationGroup="grupoUT"></asp:TextBox>

                                                </td>
                                                <td>
                                                    <asp:TextBox ID="totalMujeres" runat="server" class="form-control" type="numer" min="0" max="100" ValidationGroup="grupoUT"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-secondary" data-dismiss="modal" data-backdrop="false">Cancelar</button>
                                    <asp:Button ID="botonGuardar" ClientIDMode="Static" runat="server" class="btn btn-danger"
                                        OnClick="Guardar_Click" Text="Guardar" ValidationGroup="grupoUT" CausesValidation="false" />
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="listaDatos" EventName="RowCommand" />
                </Triggers>
            </asp:UpdatePanel>
        </asp:Panel>











<%--INICIA ATENCION Y CANALIZACION--%>
        <asp:Panel runat="server" ID="panelAyC">

            <asp:Panel ID="contenidoAyC" runat="server">

                <h2>Atención y Canalización</h2>

                <button type="button" class="btn btn-secondary" data-toggle="modal" data-target="#myModalAyC">
                    Agregar
                </button>


                 <asp:UpdatePanel runat="server" ChildrenAsTriggers="true" ID="updatePanel2" >
                <ContentTemplate>

                    <asp:GridView runat="server" ID="listaAtencion" class="table table-sm"
                        AutoGenerateColumns="false" AllowPaging="true" PageSize="5"
                        OnRowDeleting="listaAtencion_RowDeleting"
                        OnPageIndexChanging="listaAtencion_PageIndexChanging"
                        OnRowDataBound="listaAtencion_RowDataBound"
                        OnRowCommand="listaAtencion_RowCommand"
                        DataKeyNames="ID">
                        <Columns>
                            <asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-CssClass="ColumnaOculta" HeaderStyle-CssClass="ColumnaOculta" />
                            <asp:BoundField DataField="FECHA" HeaderText="FECHA" />
                            <asp:BoundField DataField="EXPEDIENTE" HeaderText="NO. EXPEDIENTE"  />
                            <asp:BoundField DataField="NOMBRE" HeaderText="NOMBRE" />
                            <asp:BoundField DataField="AP" HeaderText="APELLIDO PATERNO"  ItemStyle-CssClass="ColumnaOculta" HeaderStyle-CssClass="ColumnaOculta"/>
                            <asp:BoundField DataField="AM" HeaderText="APELLIDO MATERNO"  ItemStyle-CssClass="ColumnaOculta" HeaderStyle-CssClass="ColumnaOculta"/>

                            <asp:ButtonField ButtonType="Button" CommandName="EditarRegistroAtencion" Text="Editar"  ItemStyle-CssClass="btn btn-light" />

                            <asp:CommandField ShowDeleteButton="true" />
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>

            </asp:Panel>

            <br />
            <asp:GridView runat="server" ID="tablaAtencionCiudadana" ClientIDMode="Static" class="table table-sm table-dark" Visible="false">
            </asp:GridView>

        </asp:Panel>


        <%--MODAL ATENCION Y CANALIZACION--%>


        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Panel runat="server" class="modal fade" ID="myModalAyC" ClientIDMode="Static" TabIndex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">

                    <div class="modal-dialog modal-xl" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <asp:Label runat="server" class="modal-title" ID="Label1">Registro</asp:Label>
                                <button type="button" class="close" data-dismiss="modal" data-backdrop="false" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div class="modal-body">
                                <br />
                                <br />
                                <div class="form-group row">
                                    <label for="fechaAC" class="col-sm-3 col-form-label">Fecha:</label>
                                    <div class="col-sm-9">
                                        <asp:TextBox runat="server" type="date" class="form-control" ID="fechaAC" ValidationGroup="grupoAyC"> </asp:TextBox>
                                    </div>
                                  
                                </div>
                                <div class="form-group row">
                                      <label for="expedienteAC" class="col-sm-3 col-form-label">No. Expediente:</label>
                                    <div class="col-sm-9">
                                        <asp:TextBox runat="server" type="text" class="form-control" ID="expedienteAC" placeholder="0/0000" ValidationGroup="grupoAyC"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label for="nombrePersona" class="col-sm-3 col-form-label">Nombre:</label>
                                    <div class="col-sm-3">
                                        <asp:TextBox runat="server" type="text" class="form-control" ID="nombrePersona" ValidationGroup="grupoAyC"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox runat="server" type="text" class="form-control" ID="primerApellido" ValidationGroup="grupoAyC"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox runat="server" type="text" class="form-control" ID="segundoApellido" ValidationGroup="grupoAyC"></asp:TextBox>
                                    </div>
                                </div>

<%--                                <asp:Button ID="ButtonAC" runat="server" class="btn btn-primary" OnClick="ButtonAC_Click" Text="Guardar" ValidationGroup="grupoAyC" CausesValidation="false" />--%>


                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-dismiss="modal" data-backdrop="false">Cancelar</button>
                                <asp:Button ID="Button1" ClientIDMode="Static" runat="server" class="btn btn-danger"
                                    OnClick="ButtonAC_Click" Text="Guardar" ValidationGroup="grupoAyC" CausesValidation="false" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="listaDatos" EventName="RowCommand" />
            </Triggers>
        </asp:UpdatePanel>
        <%--TERMINA ATENCI[ON Y CANALIZACION--%>


       
        
        
        
        
        
        
        
        
        
        <%--MODULO COORDINACIÓN DE PARENTALIDAD--%>

        <asp:Panel runat="server" ID ="panelParentalidad">
            <h2>Coordinación de Parentalidad</h2>

            <asp:Panel ID="contentParentalidad" runat="server">

            </asp:Panel>


            <asp:UpdatePanel ID="updatePatentalidad" runat="server">

            </asp:UpdatePanel>


        </asp:Panel>




    </asp:Panel>















</asp:Content>


