<%@ Page Title="" Language="C#" MasterPageFile="~/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="SeguimientoAcuerdos.aspx.cs" Inherits="SistemaIntegralEstadistica.Vista.SeguimientoAcuerdos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Vista/js/suma.js"></script>
        <script src="../Vista/js/jquery-3.6.0.min.js"></script>
    <script src="../Vista/js/bootstrap.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style>
        td > input {
            width: 45px !important;
        }
    </style>
        <asp:Panel runat="server" ID="contenido_tabla_visitas">
            <asp:ScriptManager ID="ScriptManager1" 
                               runat="server" />
            <asp:UpdatePanel ID="UpdatePanel1"   runat="server">
                        <ContentTemplate>

            <h3>INICIO</h3>
            <asp:Table ID="Tabla1" runat="server"
                CellPadding="3" CellSpacing="5"
                GridLines="both" >
            </asp:Table>

                        <h3>PUBLICACIONES</h3>
            <asp:Table ID="Tabla2" runat="server"
                CellPadding="3" CellSpacing="5"
                GridLines="both" >
            </asp:Table>


                            <asp:Button runat="server" CausesValidation="false"></asp:Button>
            </ContentTemplate>
            </asp:UpdatePanel>

            
        </asp:Panel>


        <br /><br />
</asp:Content>
