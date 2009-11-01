Imports System.IO
Imports System.Diagnostics

Public Class FFMpegWrapper

    Const chunk2 As Integer = 1048576
    Const chunk As Integer = 1023

    Public Shared Sub DoIt()

        Dim ffmpeg As Process = New Process()

        Dim duration As String = String.Empty
        Dim result As String = String.Empty
        Dim eReader As StreamReader = Nothing
        Dim oReader As StreamReader = Nothing

        ffmpeg.StartInfo.UseShellExecute = False
        ffmpeg.StartInfo.ErrorDialog = False
        ffmpeg.StartInfo.RedirectStandardError = True
        ffmpeg.StartInfo.RedirectStandardOutput = True
        ffmpeg.StartInfo.FileName = "D:\temp\ffmpeg\ffmpeg.exe"
        ffmpeg.StartInfo.Arguments = "-i D:\Temp\ffmpeg\test.ts " & _
                                     "-threads 4 -re -vcodec libx264 -s 480x272 -flags +loop -cmp +chroma " & _
                                     "-deblockalpha 0 -deblockbeta 0 -crf 24 -bt 256k -refs 1 -coder 0 -me_method umh " & _
                                     "-me_range 16 -subq 5 -partitions +parti4x4+parti8x8+partp8x8 -g 250 " & _
                                     "-keyint_min 25 -level 30 -qmin 10 -qmax 51 -trellis 2 -sc_threshold 40 " & _
                                     "-i_qfactor 0.71 -acodec libfaac -ab 96k -ar 48000 -ac 2 -async 2 - |"


        ffmpeg.Start()

        eReader = ffmpeg.StandardError
        oReader = ffmpeg.StandardOutput

        'ffmpeg.WaitForExit(1000)
        'result = eReader.ReadToEnd()
        'duration = result.Substring(result.IndexOf("Duration: ") + ("Duration: ").Length, ("00:00:00.00").Length)

        ' prepare the output filestreams
        Dim outFile As FileStream = Nothing

        ' allocate buffer (if all pre-conditions are met)
        Dim buffer(chunk) As Byte
        Dim count As Integer = buffer.Length
        Dim chunkCount As Integer = 1
        ' iterate read/writes between streams
        Dim i As Integer = 0
        Dim s As Stream

        Do
            If outFile Is Nothing Then
                outFile = New FileStream(String.Format("D:\temp\ffmpeg\out{0}.ts", chunkCount.ToString), FileMode.Create, FileAccess.Write, FileShare.None)
            End If
            i += count
            count = s.r.Read(buffer, 0, count)
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

End Class
