Imports System.IO


Module Module1

    Sub Main()

        Dim inputStream As Stream = Console.OpenStandardInput()
        Streams.CopyStreamContents(inputStream)

    End Sub

End Module

