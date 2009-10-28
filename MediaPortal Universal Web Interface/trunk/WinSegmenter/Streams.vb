Imports System.IO


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
            If i > 1000000 Then
                outFile.Close()
                outFile = Nothing
                chunkCount += 1
                i = 0
            End If
        Loop

    End Sub

End Class


