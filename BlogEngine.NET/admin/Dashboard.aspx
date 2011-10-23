<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="Admin.Dashboard" %>
<%@ Import Namespace="BlogEngine.Core" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server">
    <script src="jquery.masonry.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function(){
            $('#widgets').masonry({ singleMode: true, itemSelector: '.dashboardWidget' });
        });
    </script>
	<div class="content-box-outer">
		<div class="content-widgets-box-full">
            <div id="stats" style="width:20%;">
                <div class="dashboardStats">
                    <div class="rounded">
                        <h2><%=Resources.labels.stats %></h2>
                        <ul>
                            <li>
                                <%=PostsPublished%> <%=Resources.labels.posts %><a class="viewAction endline" href="Posts/Posts.aspx"><%=Resources.labels.viewAll %></a><br />
                                <%=DraftPostCount%> <%=Resources.labels.draftPosts %>
                            </li>
                            <li>
                                <%=PagesCount%> <%=Resources.labels.pages %><a class="viewAction endline" href="Pages/Pages.aspx"><%=Resources.labels.viewAll %></a><br />
                                <%=DraftPageCount%> <%=Resources.labels.draftPages %>
                            </li>
                            <li>
                                <%=CommentsAll%> <%=Resources.labels.comments %> <a class="viewAction endline" href="Comments/Approved.aspx"><%=Resources.labels.viewAll %></a><br />
                                <%=CommentsUnapproved%> <%=Resources.labels.unapproved %> <a class="viewAction endline" href="Comments/Pending.aspx" ><%=Resources.labels.viewAll %></a><br />
                                <%=CommentsSpam%> <%=Resources.labels.spam %> <a class="viewAction endline" href="Comments/Spam.aspx"><%=Resources.labels.viewAll %></a>
                            </li>
                            <li>
                                <%=CategoriesCount%> <%=Resources.labels.categories %>
                                    <a class="viewAction endline" href="Posts/Categories.aspx"><%=Resources.labels.viewAll %></a>
                            </li>
                            <li>
                                <%=TagsCount%> <%=Resources.labels.tags %> 
                                    <a class="viewAction endline" href="Posts/Tags.aspx"><%=Resources.labels.viewAll %></a>
                            </li>
                            <li>
                                <%=UsersCount%> <%=Resources.labels.users %>
                                    <a class="viewAction endline" href="Users/Users.aspx"><%=Resources.labels.viewAll %></a>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
            <div id="widgets" style="width:80%;">
                <div class="dashboardWidget">
                    <div class="rounded">
                        <h2><%=Resources.labels.draftPosts %> <a class="addNew" href="Posts/Add_entry.aspx"><%=Resources.labels.writeNewPost %></a></h2>
                        <ul id="DraftPosts" runat="server"></ul>
                    </div>
                </div>
                <div class="dashboardWidget rounded">
                    <div class="rounded">
                        <h2><%=Resources.labels.draftPages %> <a class="addNew" href="Pages/EditPage.aspx"><%=Resources.labels.addNewPage %></a></h2>
                        <ul id="DraftPages" runat="server"></ul>
                    </div>
                </div>
                <div class="dashboardWidget rounded">
                    <div class="rounded">
                        <h2><%=Resources.labels.trash %></h2>
                        <a class="viewAction" href="Trash.aspx"><%=Resources.labels.viewAll %></a> &nbsp;&nbsp;
                        <a class="deleteAction" href="#" onclick="return ProcessTrash('Purge', 'All');"><%=Resources.labels.emptyTrash %></a>
                    </div>
                </div>
                <div class="dashboardWidget rounded">
                    <div class="rounded">
                        <%=GetCommentsList()%>                      
                    </div>
                </div>
                <div class="clear"></div>
            </div>
            <div class="clear"></div>
        </div>
    </div>
</asp:Content>

