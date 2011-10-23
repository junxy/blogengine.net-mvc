<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<BlogEngine.MVC.ViewsModels.IndexViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% foreach (var post in Model.Posts)
       {%>

       <div style="border:1px solid #ccc; padding:5px auto; margin-bottom:20px;">

        <h1><%= Html.ActionLink(post.Title, "post", new { Id = post.Id })%></h1>

        <p><%= post.Content%></p>

       </div>
           
       <%} %>

</asp:Content>
