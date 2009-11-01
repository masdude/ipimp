﻿Imports System.IO
Imports System.Diagnostics

Public Class Streams

    Const chunk2 As Integer = 1048576
    Const chunk As Integer = 1023


    Public Shared Sub CopyStreamContents(ByVal inStream As Stream)

        ' assert these are the right kind of streams
        If inStream Is Nothing Then Throw New ArgumentNullException("input")
        If Not inStream.CanRead Then Throw New ArgumentException("Input stream must support CanRead")

        ' prepare the output filestreams
        Dim outFile As FileStream = Nothing

        ' skip if the input stream is empty (if seeking is supported)
        If inStream.CanSeek Then If inStream.Length = 0 Then Exit Sub

        ' allocate buffer (if all pre-conditions are met)
        Dim buffer(chunk) As Byte
        Dim count As Integer = buffer.Length
        Dim chunkCount As Integer = 1
        ' iterate read/writes between streams
        Dim i As Integer = 0

        Do
            If outFile Is Nothing Then
                outFile = New FileStream(String.Format("D:\temp\ffmpeg\out{0}.ts", chunkCount.ToString), FileMode.Create, FileAccess.Write, FileShare.None)
            End If
            i += count
            count = inStream.Read(buffer, 0, count)
            If count = 0 Then Exit Do
            outFile.Write(buffer, 0, count)

            ' Try this...launch the ffmpeg conversion is a thread and capture STDOUT as the input stream
            ' and STDERR as the progress information.  Parse STDERR looking at the time field and chunk
            ' on that.
            '
            ' Probably have to read small number of bytes (10k?) and check STDERR each time.
            '

            If i > 1000000 Then
                outFile.Close()
                outFile = Nothing

                Dim path As String = "D:\temp\ffmpeg\test.m3u8"
                Dim sw As StreamWriter
                If System.IO.File.Exists(path) = False Then
                    sw = System.IO.File.CreateText(path)
                    sw.WriteLine(GetDuration(String.Format("D:\Temp\ffmpeg\out{0}.ts", chunkCount.ToString)))
                    sw.Flush()
                    sw.Close()
                Else
                    sw = System.IO.File.AppendText(path)
                    sw.WriteLine(GetDuration(String.Format("D:\Temp\ffmpeg\out{0}.ts", chunkCount.ToString)))
                    sw.WriteLine(inStream.Position.ToString)
                    sw.Flush()
                    sw.Close()
                End If

                chunkCount += 1
                i = 0

            End If
        Loop

    End Sub



    Private Shared Function GetDuration(ByVal path As String) As String
        Dim ffmpeg As Process = New Process()

        Dim duration As String = String.Empty
        Dim result As String = String.Empty
        Dim eReader As StreamReader = Nothing

        ffmpeg.StartInfo.UseShellExecute = False
        ffmpeg.StartInfo.ErrorDialog = False
        ffmpeg.StartInfo.RedirectStandardError = True
        ffmpeg.StartInfo.FileName = "D:\temp\ffmpeg\ffmpeg.exe"
        ffmpeg.StartInfo.Arguments = String.Format("-i {0}", path)

        ffmpeg.Start()

        eReader = ffmpeg.StandardError

        ffmpeg.WaitForExit(1000)

        result = eReader.ReadToEnd()

        duration = result.Substring(result.IndexOf("Duration: ") + ("Duration: ").Length, ("00:00:00.00").Length)

        Return duration

    End Function


End Class


