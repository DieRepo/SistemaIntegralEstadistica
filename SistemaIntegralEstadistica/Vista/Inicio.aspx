<%@ Page Title="" Language="C#" MasterPageFile="~/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="Inicio.aspx.cs" Inherits="SistemaIntegralEstadistica.Vista.Inicio" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Panel1" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Menu runat="server" ID="MenuHojas">
        <LevelMenuItemStyles>
        <asp:MenuItemStyle CssClass="main_menu00" />
        <asp:MenuItemStyle CssClass="level_menu" />
    </LevelMenuItemStyles>
        <Items>
            <asp:MenuItem Text="UNO"></asp:MenuItem>
            <asp:MenuItem Text="DOS"></asp:MenuItem>
        </Items>
    </asp:Menu>


</asp:Content>
