<%@ Page Title="" Language="C#" MasterPageFile="~/Master1.Master" AutoEventWireup="true" CodeBehind="Mediacion.aspx.cs" Inherits="SistemaIntegralEstadistica.Vista.Mediacion.Mediacion" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Panel1" runat="server">
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="asm" runat="server" />


    <script runat="server">
 protected void ServerButton_Click(object sender, EventArgs e)
 {
 ClientScript.RegisterStartupScript(this.GetType(), "key", "launchModal();", true);
 }
</script>
    
    <ajaxToolkit:ModalPopupExtender ID="mpe" runat="server" TargetControlId="ClientButton"  PopupControlID="ModalPanel" OkControlID="OKButton" />
     <asp:Panel runat="server" >
        <asp:Button ID="ClientButton" runat="server" Text="Nuevo" />

        <asp:Panel ID="ModalPanel" runat="server" Width="500px">
 ASP.NET AJAX is a free framework for quickly creating a new generation of more 
 efficient, more interactive and highly-personalized Web experiences that work 
 across all the most popular browsers.<br />
 <asp:Button ID="OKButton" runat="server" Text="Close" />
</asp:Panel>

    </asp:Panel>


    <asp:Panel runat="server">

        <!-- Button trigger modal -->
<button type="button" class="btn btn-primary" data-toggle="modal" data-target="#exampleModalLong">
  Launch demo modal
</button>

<!-- Modal -->
<div class="modal fade" id="exampleModalLong" tabindex="-1" role="dialog" aria-labelledby="exampleModalLongTitle" aria-hidden="true">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="exampleModalLongTitle">Modal title</h5>
        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
      </div>
      <div class="modal-body">
        ...
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
        <button type="button" class="btn btn-primary">Save changes</button>
      </div>
    </div>
  </div>
</div>
    </asp:Panel>


    <asp:Panel runat="server">

                    <div class="col col-4" style="text-align:center;">
                        <asp:Label runat="server" Text="Centro de Mediación "></asp:Label>
                            <asp:DropDownList runat="server" ID="centroMed" OnSelectedIndexChanged="centroMed_SelectedIndexChanged"
                                class="form-control" aria-labelledby="dropdownMenuButton"
                                style="width: 40%!important;display:initial;">
                                
                            </asp:DropDownList>
                    </div>
    </asp:Panel>



</asp:Content>
