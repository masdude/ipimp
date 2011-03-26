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

Imports Jayrock.Json

Public Class RecordProgram
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.ContentType = "application/json"

        Dim programID As String = Request.QueryString("programID")
        Dim recordOption As String = Request.QueryString("option")
        Dim success As Boolean

        If User.IsInRole("recorder") Then
            success = uWiMP.TVServer.Recordings.RecordProgramById(CInt(programID), CInt(recordOption))
        Else
            success = False
        End If
        
        Using jw As New JsonTextWriter()

            jw.PrettyPrint = True
            jw.WriteStartObject()
            jw.WriteMember("result")
            jw.WriteBoolean(success)
            jw.WriteEndObject()

            Response.Write(jw.ToString)
        End Using

    End Sub

End Class