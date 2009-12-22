Option Explicit

Dim oArgs
Set oArgs = WScript.Arguments

Dim oFS
Set oFS = CreateObject("Scripting.FileSystemObject")

If oArgs.Count <> 1 Then 
	WScript.Echo "ERROR"
	WScript.Quit(1)
End If

Dim root : root = oArgs(0)
Dim base : base = left(root,inStrRev(root,"\"))

ListFiles(root)

Sub ListDirs(path)
	Dim folder
	Dim cFolders
	Dim subFolder
	Set folder = oFS.GetFolder(path)
	Set cFolders = folder.SubFolders
	For Each subFolder in cFolders
		ListFiles(subFolder.Path)
	Next
End Sub

Sub ListFiles(path)
	Dim folder
	Dim cFiles
	Dim file
	Dim oFile

	Dim outFolder : outFolder = right(path, len(path)-len(base))
	If instr(outFolder,"svn")=0 Then 
		WScript.Echo
		WScript.Echo "  SetOutPath ""$INSTDIR\" & outFolder & """"
	End If

	Set folder = oFS.GetFolder(path)
	Set cFiles = folder.Files
	For each file in cFiles
		Set oFile = oFS.GetFile(file)
		If instr(file.path,"svn")=0 Then 
			WScript.Echo "  File ""..\Include\" & outFolder & "\" & file.Name & """"
		End If
	Next
	
	Dim cFolders
	Dim subFolder
	Set folder = oFS.GetFolder(path)
	Set cFolders = folder.SubFolders
	For Each subFolder in cFolders
		ListFiles(subFolder.Path)
	Next
End Sub
