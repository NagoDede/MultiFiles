Imports System.Security
Imports System.Security.Cryptography
Imports System.IO

Imports System.Text

Public Class SHA_Helper
    ''' <summary>
    ''' Provide SHA512 computations for the indicated file. Default value is set to 4096 bytes.
    ''' The value has been selected in order to have a good way to identify file similar files 
    ''' even if they are not complete (such as different version of a text file).
    ''' </summary>
    ''' <param name="fileName">path to the file</param>
    ''' <param name="fileSize">number of bytes on zhich the SHA512 will be computed</param>
    ''' <returns></returns>
    Public Shared Function GetSHA512ForFile(ByVal fileName As String, Optional fileSize As Int32 = 4096) As Byte()
        Dim fileBytes(fileSize) As Byte
        Dim shaM As New SHA512Managed()
        Dim fileStream As FileStream
        If (fileName.Length < 260) Then
            fileStream = New FileStream(fileName, FileMode.Open, FileAccess.Read)
        End If
        Try
            If (fileStream IsNot Nothing) Then
                fileStream.Read(fileBytes, 0, fileSize)
                fileStream.Close()
                Return shaM.ComputeHash(fileBytes)
            Else
                Dim b(2) As Byte
                Return b
            End If
        Catch ex As Exception
            Debug.WriteLine(ex.ToString())
            fileStream.Close()
            Dim b(2) As Byte
            Return b
        End Try




    End Function

    Public Shared Function GetSHA512ForFile_s(ByVal fileName As String, Optional fileSize As Int32 = 4096) As String
        Return ByteArrayToString(GetSHA512ForFile(fileName, fileSize))
    End Function

    ''' <summary>
    ''' Convert a Byte Array to a string
    ''' </summary>
    ''' <param name="arrInput"></param>
    ''' <returns></returns>
    Private Shared Function ByteArrayToString(ByVal arrInput() As Byte) As String
        Dim i As Integer
        Dim sOutput As New StringBuilder(arrInput.Length)
        For i = 0 To arrInput.Length - 1
            sOutput.Append(arrInput(i).ToString("X2"))
        Next
        Return sOutput.ToString()
    End Function
End Class
