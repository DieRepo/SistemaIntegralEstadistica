<%@ Page Title="" Language="C#" MasterPageFile="~/Master1.Master" AutoEventWireup="true" CodeBehind="IgualdadCaptura.aspx.cs" Inherits="SistemaIntegralEstadistica.Vista.UnidadIgualdad.IgualdadCaptura" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Panel1" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <asp:Panel runat="server">
        <asp:Button ID="ClientButton" runat="server" Text="Launch Modal Popup (Client)" />

        <asp:Panel ID="ModalPanel" runat="server" Width="500px">
 ASP.NET AJAX is a free framework for quickly creating a new generation of more 
 efficient, more interactive and highly-personalized Web experiences that work 
 across all the most popular browsers.<br />
 <asp:Button ID="OKButton" runat="server" Text="Close" />
</asp:Panel>

    </asp:Panel>
</asp:Content>
