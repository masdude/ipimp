Imports System.IO

Namespace WinSegmenter

    Public Class Streams

        Public Sub Test()

            Dim inputStream As Stream = Console.OpenStandardInput()
            Dim bytes(100) As Byte
            Console.WriteLine("To decode, type or paste the UTF7 encoded string and press enter:")
            Console.WriteLine("(Example: ""M+APw-nchen ist wundervoll"")")
            Dim outputLength As Integer = inputStream.Read(bytes, 0, 100)
            Dim chars As Char() = Encoding.UTF7.GetChars(bytes, 0, outputLength)
            Console.WriteLine("Decoded string:")
            Console.WriteLine(New String(chars))

        End Sub

        Public Sub CopyStreamContents(ByVal objInput As Stream, ByVal objOutput As Stream)

            ' assert these are the right kind of streams
            If objInput Is Nothing Then Throw New ArgumentNullException("input")
            If objOutput Is Nothing Then Throw New ArgumentNullException("output")
            If Not objInput.CanRead Then Throw New ArgumentException("Input stream must support CanRead")
            If Not objOutput.CanWrite Then Throw New ArgumentException("Output stream must support CanWrite")

            ' skip if the input stream is empty (if seeking is supported)
            If objInput.CanSeek Then If objInput.Length = 0 Then Exit Sub

            ' allocate buffer (if all pre-conditions are met)
            Dim buffer(1023) As Byte
            Dim count As Integer = buffer.Length

            ' iterate read/writes between streams
            Do
                count = objInput.Read(buffer, 0, count)
                If count = 0 Then Exit Do
                objOutput.Write(buffer, 0, count)
            Loop

        End Sub

    End Class


End Namespace
