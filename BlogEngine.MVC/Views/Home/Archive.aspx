<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<BlogEngine.MVC.ViewsModels.ArchiveViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Archive
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%foreach (var category in Model.Categories)
      {%>
    <h2>
        <%= category.Title %></h2>
    <ul>
        <% foreach (var post in category.Posts.FindAll(delegate(BlogEngine.Core.Post p) { return p.IsVisible; }))
           {%>
        <li>
            <%= post.Title %></li>
        <%} %>
    </ul>
    <% } %>
    <h2>
        None</h2>
    <ul>
        <% foreach (var post in Model.NoCatList)
           {%>
        <li>
            <%= post.Title %></li>
        <%} %>
    </ul>
</asp:Content>