<%@ Page Title="" Language="C#" MasterPageFile="~/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="Peritos.aspx.cs" Inherits="SistemaIntegralEstadistica.Vista.Peritos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
            <script src="../Vista/js/jquery-3.6.0.min.js"></script>

        <script src="../Vista/js/suma.js"></script>

    <style>
        .classHeader {
            background-color: #b71c1c;
            color: white;
        }
        .table td, .table th {
    vertical-align: middle!important;
}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Panel ID="miPanel" runat="server">

    </asp:Panel>


    <!--

    <h3>PUBLICACIONES</h3>
    <asp:Table ID="Table4" runat="server"
        CellPadding="3" CellSpacing="5"
        GridLines="both">
    </asp:Table>
    <asp:Button runat="server" ID="Button4"
        OnClientClick="this.disabled = true; this.value = 'Submitting...';"
        CommandName="Table4_enero"
        UseSubmitBehavior="false"
        OnCommand="ejemplo_Click"
        Text="Submit Me!" />




    <h3>INICIO</h3>

    <asp:Table ID="Table5" runat="server"
        CellPadding="3" CellSpacing="5"
        GridLines="both">
    </asp:Table>
    <asp:Button runat="server" ID="Button1"
        OnClientClick="this.disabled = true; this.value = 'Submitting...';"
        CommandName="Table5_enero"
        UseSubmitBehavior="false"
        OnCommand="ejemplo_Click"
        Text="Submit Me!" />



    <h3>PUBLICACIONES</h3>
    <asp:Table ID="Table6" runat="server"
        CellPadding="3" CellSpacing="5"
        GridLines="both">
    </asp:Table>
    <asp:Button runat="server" ID="Button2"
        OnClientClick="this.disabled = true; this.value = 'Submitting...';"
        CommandName="Table6_enero"
        UseSubmitBehavior="false"
        OnCommand="ejemplo_Click"
        Text="Submit Me!" />




    <h3>INICIO</h3>

    <asp:Table ID="Table7" runat="server"
        CellPadding="3" CellSpacing="5"
        GridLines="both">
    </asp:Table>
    <asp:Button runat="server" ID="Button3"
        OnClientClick="this.disabled = true; this.value = 'Submitting...';"
        CommandName="Table7_enero"
        UseSubmitBehavior="false"
        OnCommand="ejemplo_Click"
        Text="Submit Me!" />

    <h3>PUBLICACIONES</h3>
    <asp:Table ID="Table8" runat="server"
        CellPadding="3" CellSpacing="5"
        GridLines="both">
    </asp:Table>
    <asp:Button runat="server" ID="Button5"
        OnClientClick="this.disabled = true; this.value = 'Submitting...';"
        CommandName="Table8_c1"
        UseSubmitBehavior="false"
        OnCommand="ejemplo_Click"
        Text="Submit Me!" />




    <h3>INICIO</h3>

    <asp:Table ID="Table9" runat="server"
        CellPadding="3" CellSpacing="5"
        GridLines="both">
    </asp:Table>
    <asp:Button runat="server" ID="Button6"
        OnClientClick="this.disabled = true; this.value = 'Submitting...';"
        CommandName="Table9_asuntosAsignados"
        UseSubmitBehavior="false"
        OnCommand="ejemplo_Click"
        Text="Submit Me!" />



    <h3>PUBLICACIONES</h3>
    <asp:Table ID="Table10" runat="server"
        CellPadding="3" CellSpacing="5"
        GridLines="both">
    </asp:Table>
    <asp:Button runat="server" ID="Button8"
        OnClientClick="this.disabled = true; this.value = 'Submitting...';"
        UseSubmitBehavior="false"
        CommandName="Table10_enero"
        OnCommand="ejemplo_Click"
        Text="Submit Me!" />


    <h3>INICIO</h3>

    <asp:Table ID="Table11" runat="server"
        CellPadding="3" CellSpacing="5"
        GridLines="both">
    </asp:Table>
    <asp:Button runat="server" ID="Button7"
        OnClientClick="this.disabled = true; this.value = 'Submitting...';"
        UseSubmitBehavior="false"
        CommandName="Table11_enero"
        OnCommand="ejemplo_Click"
        Text="Submit Me!" />



    <h3>PUBLICACIONES</h3>
    <asp:Table ID="Table12" runat="server"
        CellPadding="3" CellSpacing="5"
        GridLines="both">
    </asp:Table>
    <asp:Button runat="server" ID="BtnSubmit"
        OnClientClick="this.disabled = true; this.value = 'Submitting...';"
        UseSubmitBehavior="false"
        CommandName="Table12_enero"
        OnCommand="ejemplo_Click"
         Text="Submit Me!"/>


    <h3>INICIO</h3>

    <asp:Table ID="Table13" runat="server"
        CellPadding="3" CellSpacing="5"
        GridLines="both">
    </asp:Table>
    <asp:Button runat="server" ID="BtnSubmit13"
        OnClientClick="this.disabled = true; this.value = 'Submitting...';"
        UseSubmitBehavior="false"
        CommandName="Table13_enero"
        OnCommand="ejemplo_Click"
        Text="Submit Me!" />

    -->

</asp:Content>
