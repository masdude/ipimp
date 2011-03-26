' 
'   Copyright (C) 2008-2011 Martin van der Boon
' 
'  This program is free software: you can redistribute it and/or modify 
'  it under the terms of the GNU General Public License as published by 
'  the Free Software Foundation, either version 3 of the License, or 
'  (at your option) any later version. 
' 
'   This program is distributed in the hope that it will be useful, 
'   but WITHOUT ANY WARRANTY; without even the implied warranty of 
'   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
'   GNU General Public License for more details. 
' 
'   You should have received a copy of the GNU General Public License 
'   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
' 

Imports TvDatabase
Imports Jayrock.Json

Public Class ProgramDetails
    Inherits System.Web.UI.Page



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "application/json"

        Dim programID As String = Request.QueryString("programID")
        Dim program As Program = uWiMP.TVServer.Programs.GetProgramByProgramId(CInt(programID))
        Dim channel As Channel = uWiMP.TVServer.Channels.GetChannelByChannelId(program.IdChannel)

        Dim timetext As String = String.Format("<b>{0}-{1}</b><br>", program.StartTime.ToShortTimeString, program.EndTime.ToShortTimeString)
        Dim title As String = IIf(program.Title = "", GetGlobalResourceObject("uWiMPStrings", "unknown_title"), program.Title)
        Dim episodeName As String = IIf(program.EpisodeName = "", timetext, timetext & "<b>" & program.EpisodeName & "</b><br>")
        Dim description As String = IIf(program.Description = "", GetGlobalResourceObject("uWiMPStrings", "description_not_found"), episodeName & program.Description)

        Using jw As New JsonTextWriter()

            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("title")
            jw.WriteString(title)
            jw.WriteMember("description")
            jw.WriteString(description)
            jw.WriteMember("channel")
            jw.WriteString(channel.Name)
            jw.WriteMember("running")
            jw.WriteBoolean(program.IsRunningAt(Now))
            jw.WriteMember("scheduled")
            jw.WriteBoolean(uWiMP.TVServer.Schedules.IsProgramScheduled(program))
            jw.WriteEndObject()

            Response.Write(jw.ToString)

        End Using
        
    End Sub

End Class