<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SistemaIntegralEstadistica.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
  <%--CSS--%>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.6.3/css/all.css"  />
    <link rel="stylesheet" href="../Vista/css/Login.css" />
    <link rel="stylesheet" href="PNotify/animate.css" />
    <link rel="stylesheet" href="PNotify/pnotify.custom.min.css" />

    <%--JS--%>
      <script src="https://code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js" ></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/js/bootstrap.min.js"></script>
    <script src="Scripts/Sesion.js"></script>
     <script src="PNotify/pnotify.custom.min.js"></script>
</head>
<body>
   <form runat="server">

          <asp:Panel runat="server" CssClass="﻿sesion_login_contenedor2">
              
              <asp:ScriptManager ID="ajax_login" runat="server"/>
                                
               <asp:Panel runat="server" CssClass="central_panel2">
                   <asp:Panel runat="server" CssClass="">
                            <asp:Image runat="server" ImageUrl="../Vista/Img/portada1-01.png" style="height: 650px; border: 1px solid;border-radius: 5px;position: absolute;top: 50%;left: 50%;transform: translate(-40%, -50%);"/>
                        </asp:Panel>
                            <asp:Panel runat="server" CssClass="">
                              <asp:Label runat="server" CssClass="titulo_central_panel2 font-weight-bold">SISTEMA INTEGRAL DE ESTADISTICA</asp:Label>
                                 </asp:Panel>
                      </asp:Panel>
                               <asp:Panel runat="server" CssClass="contenedor_completo2"> 
                                       <asp:Panel runat="server" CssClass="form-group">
                                            <asp:Label runat="server" CssClass="">Usuario</asp:Label>
                                            <asp:Panel runat="server" CssClass="input-group">
                                                <asp:Panel runat="server" CssClass="input-group-prepend">
                                                    <span class="input-group-text">
                                                        <i class="fas fa-user-tie"></i>
                                                    </span>
                                                </asp:Panel>
                                                <asp:TextBox ID="TextBox_user_name" runat="server" CssClass="form-control" Placeholder="Usuario"/>
                                            </asp:Panel>
                                        </asp:Panel>
                                        <asp:Panel runat="server" CssClass="form-group">
                                            <asp:Label runat="server" CssClass="">Contraseña</asp:Label>
                                            <asp:Panel runat="server" CssClass="input-group">
                                                <asp:Panel runat="server" CssClass="input-group-prepend">
                                                    <asp:Label runat="server" CssClass="input-group-text"><i class="fas fa-lock"></i></asp:Label>
                                                </asp:Panel>
                                                <asp:TextBox TextMode="Password" ID="TextBox_password" runat="server" CssClass="form-control" Placeholder="Contraseña"/>
                                            </asp:Panel>
                                        </asp:Panel>
                                        <asp:Panel runat="server" CssClass="form-group d-flex align-items-center justify-content-center">
                                            <asp:Button runat="server" ID="btn_login" Text="Iniciar sesión" CssClass="btn btn-secondary" OnClick="btn_login_Click"/>
                                        </asp:Panel>  
                                        <asp:Panel runat="server" CssClass="form-group d-flex align-items-center justify-content-center">
                                            <asp:Label runat="server" ID="textoError" CssClass=""></asp:Label>
                                        </asp:Panel>  
                                    </asp:Panel>
               
                     </asp:Panel>
          
         </form>



</body>
</html>
