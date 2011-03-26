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

Public Class DesktopLogin
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub LoginButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        Dim MembershipProvider As uWiMP.TVServer.SQLiteMembershipProvider = DirectCast(Membership.Providers("SQLiteMembershipProvider"), uWiMP.TVServer.SQLiteMembershipProvider)
        Dim validuser As Boolean = Membership.ValidateUser(Username.Text, Password.Text)

        If validuser Then
            FormsAuthentication.SetAuthCookie(Username.Text, CheckBox1.Checked)
            Response.Redirect("Default.aspx")
        Else
            Label4.Text = "Login failed! Retry"
            Label4.ForeColor = Drawing.Color.Red
        End If
    End Sub

End Class