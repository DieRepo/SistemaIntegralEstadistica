<%@ Page Title="" Language="C#" MasterPageFile="~/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="Magistrados.aspx.cs" Inherits="SistemaIntegralEstadistica.Vista.Magistrados" %>
<%@ MasterType VirtualPath="~/PaginaMaestra.Master" %>

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
        .classTitulo {
            font-size: 18px;
            font-weight: bold;
            padding-bottom: 20px;
        }
    </style>
   
    <!--
    <asp:Panel runat="server" ID="contenido_tabla_visitas">
          <asp:Table ID="Table1111" runat="server"
                    CellPadding="3" CellSpacing="5"
                    GridLines="both">
                </asp:Table>
                <asp:Button runat="server" ID="Button3"
                    OnClientClick="this.disabled = true; this.value = 'Submitting...';"
                    CommandName="Tabla1111_enero00"
                    UseSubmitBehavior="false"
                    OnCommand="guardarInformacion"
                    Text="Submit Me!" />

        <asp:ScriptManager ID="ScriptManager1"
            runat="server" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>

                
                
                <h3>JUECES Y ÓRGANOS JURISDICCIONALES DEL PODER JUDICIAL DEL ESTADO DE MÉXICO</h3>
                <asp:Table ID="Tabla178" runat="server"
                    CellPadding="3" CellSpacing="5"
                    GridLines="both">
                </asp:Table>
                <asp:Button runat="server" ID="Button4"
                    OnClientClick="this.disabled = true; this.value = 'Submitting...';"
                    CommandName="Tabla178_enero"
                    UseSubmitBehavior="false"
                    OnCommand="guardarInformacion"
                    Text="Submit Me!" />

                
                
                <h3>JUECES Y JUEZAS DEL PODER JUDICIAL DEL ESTADO DE MÉXICO</h3>
                <asp:Table ID="Tabla2123" runat="server"
                    CellPadding="3" CellSpacing="5"
                    GridLines="both">
                </asp:Table>
                <asp:Button runat="server" ID="Button1"
                    OnClientClick="this.disabled = true; this.value = 'Submitting...';"
                    CommandName="Tabla2123_enero"
                    UseSubmitBehavior="false"
                    OnCommand="guardarInformacion"
                    Text="Submit Me!" />

                
                
                <h3>ÓRGANOS JURISDICCIONALES DEL PODER JUDICIAL DEL ESTADO DE MÉXICO</h3>
                <asp:Table ID="Tabla3456" runat="server"
                    CellPadding="3" CellSpacing="5"
                    GridLines="both">
                </asp:Table>
                <asp:Button runat="server" ID="Button2"
                    OnClientClick="this.disabled = true; this.value = 'Submitting...';"
                    CommandName="Tabla3456_enero"
                    UseSubmitBehavior="false"
                    OnCommand="guardarInformacion"
                    Text="Submit Me!" />


            </ContentTemplate>
        </asp:UpdatePanel>


    </asp:Panel>
    -->

    <br />
    <br />
</asp:Content>
