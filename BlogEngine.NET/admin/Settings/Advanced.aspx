<%@ Page Title="" Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Advanced.aspx.cs" Inherits="admin.Settings.Advanced" %>
<%@ Register src="Menu.ascx" tagname="TabMenu" tagprefix="menu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphAdmin" Runat="Server"> 
    <script type="text/javascript">
        $(document).ready(function () {
            var frm = document.forms.aspnetForm;
            $(frm).validate({
                onsubmit: false
            });

            $("#btnSave").click(function (evt) {
                if ($(frm).valid())
                    SaveSettings();

                evt.preventDefault();
            });
        });
        function SaveSettings() {
            $('.loader').show();
			
            var dto = { 
				"enableCompression": $("[id$='_cbEnableCompression']").attr('checked'),
				"removeWhitespaceInStyleSheets": $("[id$='_cbRemoveWhitespaceInStyleSheets']").attr('checked'),
				"compressWebResource": $("[id$='_cbCompressWebResource']").attr('checked'),
				"enableOpenSearch": $("[id$='_cbEnableOpenSearch']").attr('checked'),
				"requireSslForMetaWeblogApi": $("[id$='_cbRequireSslForMetaWeblogApi']").attr('checked'),
				"wwwSubdomain": $('.rblSubdomain input:radio:checked').val(),
				"enableTrackBackSend": $("[id$='_cbEnableTrackBackSend']").attr('checked'),
				"enableTrackBackReceive": $("[id$='_cbEnableTrackBackReceive']").attr('checked'),
				"enablePingBackSend": $("[id$='_cbEnablePingBackSend']").attr('checked'),
				"enablePingBackReceive": $("[id$='_cbEnablePingBackReceive']").attr('checked'),
				"enableErrorLogging": $("[id$='_cbEnableErrorLogging']").attr('checked'),
				"allowRemoteFileDownloads": $("[id$='_cbAllowRemoteFileDownloads']").attr('checked'),
				"remoteTimeout": $("[id$='_txtRemoteTimeout']").attr('value'),
				"remoteMaxFileSize": $("[id$='_txtRemoteMaxFileSize']").attr('value')                
			};
			
            $.ajax({
                url: "Advanced.aspx/Save",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(dto),
                success: function (result) {
                    var rt = result.d;
                    if (rt.Success)
                        ShowStatus("success", rt.Message);
                    else
                        ShowStatus("warning", rt.Message);
                }
            });
            $('.loader').hide();
            return false;
        }  
    </script>
     
	<div class="content-box-outer">
		<div class="content-box-right">
			<menu:TabMenu ID="TabMenu" runat="server" />
		</div>
		<div class="content-box-left">

           <h1><%=Resources.labels.advancedSettings %></h1>

                <ul class="fl leftaligned">
                    <li>
                        <label class="lbl" for=""><%=Resources.labels.enableTrackbacks %></label>
                            <asp:CheckBox runat="server" ID="cbEnableTrackBackSend" /><label><%=Resources.labels.send %></label>
                            &nbsp;&nbsp;
                            <asp:CheckBox runat="server" ID="cbEnableTrackBackReceive" /><label><%=Resources.labels.receive %></label>
                    </li>
                    <li>
                        <label for="" class="lbl"><%=Resources.labels.enablePingbacks %></label>
                            <asp:CheckBox runat="server" ID="cbEnablePingBackSend" /><label><%=Resources.labels.send %></label>
                            &nbsp;&nbsp;
                            <asp:CheckBox runat="server" ID="cbEnablePingBackReceive" /><label><%=Resources.labels.receive %></label>
                    </li>
                    <li>
                        <label class="lbl" for="<%=rblWwwSubdomain.ClientID %>"><%=Resources.labels.handleWwwSubdomain %></label>
                            <asp:RadioButtonList runat="server" CssClass="rblSubdomain" ID="rblWwwSubdomain" RepeatLayout="flow" RepeatDirection="horizontal">
                                <asp:ListItem Text="<%$ Resources:labels, remove %>" Value="remove" />
                                <asp:ListItem Text="<%$ Resources:labels, enforce %>" Value="add" />
                                <asp:ListItem Text="<%$ Resources:labels, ignore %>" Value="" Selected="true" />
                            </asp:RadioButtonList>
                    </li>
                    <li>
                        <label class="lbl"><%=Resources.labels.otherSettings %></label>
                        <asp:CheckBox runat="server" ID="cbEnableCompression" />
                        <label for="<%=cbEnableCompression.ClientID %>"><%=Resources.labels.enableHttpCompression %></label>
                        <span class="insetHelp">(<%=Resources.labels.enableHttpCompressionDescription %>)</span>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbRemoveWhitespaceInStyleSheets" />
                        <label for="<%=cbRemoveWhitespaceInStyleSheets.ClientID %>"><%=Resources.labels.trimStylesheet %></label>
                        <span class="insetHelp">(<%=Resources.labels.trimStylesheetDescription %>)</span>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbCompressWebResource" />
                        <label for="<%=cbCompressWebResource.ClientID %>"><%=Resources.labels.compressWebResource %></label>
                        <span class="insetHelp">(<%=Resources.labels.compressWebResourceDescription%>)</span>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbEnableOpenSearch" />
                        <label for="<%=cbEnableOpenSearch.ClientID %>"><%=Resources.labels.enableOpenSearch %></label>
                        <span class="insetHelp">(<%=Resources.labels.enableOpenSearchDescription %>)</span>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbRequireSslForMetaWeblogApi" />
                        <label for="<%=cbRequireSslForMetaWeblogApi.ClientID %>"><%=Resources.labels.requireSslForMetaWeblogApi %></label>
                        <span class="insetHelp">(<%=Resources.labels.requireSslForMetaWeblogApiDescription%>)</span>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbEnableErrorLogging" />
                        <label for="<%=cbEnableErrorLogging.ClientID %>"><%=Resources.labels.enableErrorLogging %></label>
                        <span class="insetHelp">(<%=Resources.labels.enableErrorLoggingDescription%>)</span>
                    </li>
                </ul>
                <h2><%=Resources.labels.securitySettings %></h2>

                <ul class="fl leftaligned">
                    <li>
                        <label class="lbl" for="<%=txtRemoteTimeout.ClientID %>"><%=Resources.labels.remoteTimeout %></label>
                        <asp:TextBox runat="server" Width="80" ID="txtRemoteTimeout" />
                        <span class="belowHelp"><label for="<%=txtRemoteTimeout.ClientID %>"><%=Resources.labels.remoteTimeoutDescription %></label></span>
                    </li>
                    <li>
                        <label class="lbl" for="<%=txtRemoteMaxFileSize.ClientID %>"><%=Resources.labels.maximumRemoteFileSize%></label>
                        <asp:TextBox runat="server" ID="txtRemoteMaxFileSize" />
                        <span class="belowHelp"><label><%=Resources.labels.maximumRemoteFileSizeDescription%></label></span>
                    </li>
                    <li>
                        <span class="filler"></span>
                        <asp:CheckBox runat="server" ID="cbAllowRemoteFileDownloads" /><label for="<%=cbAllowRemoteFileDownloads.ClientID %>"><%=Resources.labels.allowRemoteFileDownloads %></label>
                        <span class="insetHelp"><%=Resources.labels.allowRemoteFileDownloadsDescription%></span>
                    </li>
                </ul>
            <div class="action_buttons">
                <input type="submit" id="btnSave" class="btn primary rounded" value="<%=Resources.labels.saveSettings %>" />
            </div>
		</div>
	</div>       
</asp:Content>

