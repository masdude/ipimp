<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="Website._DesktopDefault" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link rel="Stylesheet" href="css/desktop.css" />
    <script type="text/javascript" src="scripts/jquery.tools.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="float:right;">
        <ul class="right-tabs">
            <li><a href="Logout.aspx"><asp:Literal ID="Literal12" runat="server" Text="<%$ Resources:uWiMPStrings, logout %>" /></a></li>
			<li><a href="../iPhone/Default.aspx"><asp:Literal ID="Literal133" runat="server" Text="<%$ Resources:uWiMPStrings, mobile %>" /></a></li>
            <li class="orange-tabs"><a href="https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=6722080" target="_blank"><asp:Literal ID="Literal13" runat="server" Text="<%$ Resources:uWiMPStrings, donate %>" /></a></li>
        </ul>
        </div>

        <asp:PlaceHolder ID="PlaceHolder1" runat="server" />
                 
        <div class="css-panes"> 
	        <!-- tv guide -->
            <div>
                <asp:Table ID="Table1" runat="server" Width="100%" >
                    <asp:TableRow ID="TableRow1" runat="server">
                        <asp:TableCell ID="TableCell4" runat="server" Width="100%">
                            <asp:Literal ID="litChannelGroups" runat="server" />
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <asp:PlaceHolder ID="ph1" runat="server" />
                <script type="text/javascript">
                    $("td[title]").tooltip({ effect: "fade", predelay: 500, position: 'bottom center', offset: [-10, 0] });
					var pid;
					$(document).ready(function () {
					    $(".record").overlay({
					        mask: { color: '#404040', loadSpeed: 100, opacity: 0.9 },
					        closeOnClick: true,
					        fixed: false,
					        onBeforeLoad: function () {
					            pid = this.getTrigger().attr('pid');
					            $.get("ProgramDetails.aspx?programID=" + this.getTrigger().attr('ID'), function (data) {
					                document.getElementById('modalimage').innerHTML = '<img src="../TVLogos/' + data.channel + '.png" height="40px" style="float:left" />';
					                document.getElementById('modaltitle').innerHTML = '<h2>' + data.title + '</h2>';
					                document.getElementById('modaldesc').innerHTML = '<p>' + data.description + '</p>';
					                if (data.scheduled) {
					                    document.getElementById('cancelbutton').style.display = 'inline-block';
					                    document.getElementById('canceloptions').style.display = 'inline-block';
					                    document.getElementById('recordbutton').style.display = 'none';
					                    document.getElementById('recoption').style.display = 'none';
					                } else {
					                    document.getElementById('recordbutton').style.display = 'inline-block';
					                    document.getElementById('recoption').style.display = 'inline-block';
					                    document.getElementById('cancelbutton').style.display = 'none';
					                    document.getElementById('canceloptions').style.display = 'none';
					                }
					                if (data.running) {
					                    document.getElementById('streambutton').style.display = 'none';
					                } else {
					                    document.getElementById('streambutton').style.display = 'none';
					                }
					            });
					        },
					        onClose: function () {
					            document.getElementById('modalimage').innerHTML = '';
					            document.getElementById('modaltitle').innerHTML = '';
					            document.getElementById('modaldesc').innerHTML = '';
					            document.getElementById('streambutton').style.display = 'none';
					        }
					    });

					    var buttons = $("#record button").click(function (e) {
					        var recoption = document.getElementById('recoption').options[document.getElementById('recoption').selectedIndex].value;
					        var dorecord = buttons.index(this) === 0;
					        if (dorecord) {
					            $.get("RecordProgram.aspx?programID=" + pid + "&option=" + recoption, function (data) {
					                if (data.result) {
					                    var programdiv = document.getElementById(pid);
					                    programdiv.className = "rounded-corners red largewhite record";
					                }
					            });
					        }
					    });
					});
                </script>
            </div>
            <!--TV Guide Search -->
	        <div>
                <form>
                    <div>
                        <asp:Label ID="Label1" runat="server" Text="<%$ Resources:uWiMPStrings, search_term %>"  CssClass="largeblack"/>:&nbsp;
                        <input type="text" id="searchterm" />&nbsp;
                        <asp:Label ID="Label2" runat="server" Text="<%$ Resources:uWiMPStrings, search_description %>" CssClass="largeblack" />&nbsp;
                        <input type="checkbox" id="searchdesc" />&nbsp;
                        <button type="button" class="button" onclick="GetTVGuide()"><asp:Literal ID="Literal18" runat="server" Text="<%$ Resources:uWiMPStrings, search %>" /></button>
						<script type="text/javascript">
							document.getElementById("searchterm").focus();
						</script>
                    </div>
                </form>
                <div id="searchresults"></div>
            </div> 
	        <div>Third tab content</div> 
            <div>Fourth tab content</div> 
            <div>Fifth tab content</div> 
        </div> 
        <!-- record popup -->
        <div class="modal" id="record"> 
            <div id="modalimage"></div>
            <div id="modaltitle"></div>
        	<div id="modaldesc"></div>
            <form>
                <button type="button" class="button redbutton close" id="recordbutton" style="display:none;"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:uWiMPStrings, record %>" /></button>
                <select id="recoption" style="display:none;">
                    <option value=0><asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:uWiMPStrings, once %>" /></option>
                    <option value=1><asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:uWiMPStrings, at_this_time_every_day %>" /></option>
                    <option value=2><asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:uWiMPStrings, at_this_time_every_week %>" /></option>
                    <option value=3><asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:uWiMPStrings, each_time_on_this_channel %>" /></option>
                    <option value=4><asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:uWiMPStrings, each_time_on_any_channel %>" /></option>
                    <option value=5><asp:Literal ID="Literal8" runat="server" Text="<%$ Resources:uWiMPStrings, at_this_time_at_weekends %>" /></option>
                    <option value=6><asp:Literal ID="Literal9" runat="server" Text="<%$ Resources:uWiMPStrings, at_this_time_on_weekdays %>" /></option>
                </select>
                <button type="button" class="button redbutton close" id="cancelbutton" style="display:none;"><asp:Literal ID="Literal22" runat="server" Text="<%$ Resources:uWiMPStrings, cancel %>" /></button>
                <select id="canceloptions" style="display:none;">
                    <option value=0><asp:Literal ID="Literal24" runat="server" Text="<%$ Resources:uWiMPStrings, this_program %>" /></option>
                    <option value=1><asp:Literal ID="Literal25" runat="server" Text="<%$ Resources:uWiMPStrings, entire_schedule%>" /></option>
                </select>
                <button type="button" class="button redbutton close" id="streambutton" style="display:none;"><asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:uWiMPStrings, stream %>" /></button>
            </form>
        </div>
        <div class="modal" id="record2"> 
            <div id="modalimage2"></div>
            <div id="modaltitle2"></div>
        	<div id="modaldesc2"></div>
            <form>
                <button type="button" class="button redbutton close"><asp:Literal ID="Literal10" runat="server" Text="<%$ Resources:uWiMPStrings, record %>" /></button>
                <select id="recoption2">
                    <option value=0><asp:Literal ID="Literal11" runat="server" Text="<%$ Resources:uWiMPStrings, once %>" /></option>
                    <option value=1><asp:Literal ID="Literal14" runat="server" Text="<%$ Resources:uWiMPStrings, at_this_time_every_day %>" /></option>
                    <option value=2><asp:Literal ID="Literal15" runat="server" Text="<%$ Resources:uWiMPStrings, at_this_time_every_week %>" /></option>
                    <option value=3><asp:Literal ID="Literal16" runat="server" Text="<%$ Resources:uWiMPStrings, each_time_on_this_channel %>" /></option>
                    <option value=4><asp:Literal ID="Literal17" runat="server" Text="<%$ Resources:uWiMPStrings, each_time_on_any_channel %>" /></option>
                    <option value=5><asp:Literal ID="Literal19" runat="server" Text="<%$ Resources:uWiMPStrings, at_this_time_at_weekends %>" /></option>
                    <option value=6><asp:Literal ID="Literal20" runat="server" Text="<%$ Resources:uWiMPStrings, at_this_time_on_weekdays %>" /></option>
                </select>
                <button type="button" class="button redbutton close" id="streambutton2" style="display:none;"><asp:Literal ID="Literal21" runat="server" Text="<%$ Resources:uWiMPStrings, stream %>" /></button>
                <button type="button" class="button redbutton close" id="cancelbutton2" style="display:none;"><asp:Literal ID="Literal23" runat="server" Text="<%$ Resources:uWiMPStrings, cancel %>" /></button>
            </form>
        </div>
    </form>
    <script type="text/javascript">
        $(function () {
            $(".css-tabs:first").tabs(".css-panes:first > div");
        });

        function GetTVGuide() {
            var text = document.getElementById('searchterm').value;
            var desc = document.getElementById('searchdesc').checked;
            $("#searchresults").html("<div id='loader' style='text-align: center;'><img alt='activity indicator' src='css/ajax-loader.gif'></div>");
            $("#searchresults").load("SearchTVGuide.aspx?search=" + text + "&desc=" + desc);
        };

    </script>
</body>
</html>
