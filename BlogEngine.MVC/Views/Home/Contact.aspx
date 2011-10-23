<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<BlogEngine.MVC.ViewsModels.ContactViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Contact
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Contact</h2>

    <% using (Html.BeginForm()) {%>
        <%= Html.ValidationSummary(true) %>

        <fieldset>
            <legend>Fields</legend>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.Name) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.Name) %>
                <%= Html.ValidationMessageFor(model => model.Name) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.Email) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.Email) %>
                <%= Html.ValidationMessageFor(model => model.Email) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.Subject) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.Subject) %>
                <%= Html.ValidationMessageFor(model => model.Subject) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.Message) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.Message) %>
                <%= Html.ValidationMessageFor(model => model.Message) %>
            </div>
            
            <p>
                <input type="submit" value=" Send " />
            </p>
        </fieldset>

    <% } %>


</asp:Content>

