Imports System.IO


Module Module1

    Sub Main()

        Dim inputStream As Stream = Console.OpenStandardInput()

        'Dim br As New IO.BinaryReader(IO.File.OpenRead("D:\Temp\ffmpeg\test.ts"))

        Streams.CopyStreamContents(inputStream)

    End Sub

End Module

