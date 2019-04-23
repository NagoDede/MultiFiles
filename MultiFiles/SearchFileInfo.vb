Imports System.IO
Imports PdfSharp
Imports PdfSharp.Drawing
Imports PdfSharp.Pdf
Imports PdfSharp.Pdf.IO
Public Class SearchFileInfo
    ''' <summary>
    ''' Return the size of a file in human readable value. 
    ''' The results will be displayed in the most appropriate format (KB for small file, MB, GB then TB).
    ''' </summary>
    ''' <param name="BytesCaller"></param>
    ''' <returns></returns>
    Public Shared Function FormatBytes(ByVal BytesCaller As ULong) As String
        Try
            Select Case BytesCaller
                Case Is >= 1099511627776
                    Return FormatNumber(CDbl(BytesCaller / 1099511627776), 2) & " TB"
                Case 1073741824 To 1099511627775
                    Return FormatNumber(CDbl(BytesCaller / 1073741824), 2) & " GB"
                Case 1048576 To 1073741823
                    Return FormatNumber(CDbl(BytesCaller / 1048576), 2) & " MB"
                Case 1024 To 1048575
                    Return FormatNumber(CDbl(BytesCaller / 1024), 2) & " KB"
                Case 0 To 1023
                    Return FormatNumber(BytesCaller, 2) & " bytes"
                Case Else
                    Return ""
            End Select
        Catch
            Return ""
        End Try
    End Function

    Private PdfInfo As PdfDocumentInformation

    Public Property ToBeDel As Boolean
    Public ReadOnly Property FileName As String
        Get
            Dim s = Path.GetFileName(FullPath)
            If (Not String.IsNullOrEmpty(s)) Then
                Return s
            Else
                Return ""
            End If
            Return ""
        End Get
    End Property

    Public ReadOnly Property FileSize As String
        Get
            Return FormatBytes(New FileInfo(FullPath).Length)
        End Get
    End Property
    Public ReadOnly Property CheckSum As String
        Get
            Return SHA_Helper.GetSHA512ForFile_s(FullPath)
        End Get
    End Property

    Public ReadOnly Property IsPdf As Boolean
        Get
            If (Path.GetExtension(FullPath).ToUpper().EndsWith("PDF")) Then
                PdfInfo = PDF_Helper.RetrievePdfMetadata(FullPath)
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    Public ReadOnly Property PdfCreator As String
        Get
            If Not PdfInfo Is Nothing Then
                If PdfInfo.Creator Is Nothing Then
                    Return ""
                Else
                    Return PdfInfo.Creator
                End If
            Else
                Return ""
            End If
        End Get
    End Property
    Public ReadOnly Property PdfCreationDate As String
        Get

            If Not PdfInfo Is Nothing Then
                Try
                    Return PdfInfo.CreationDate.ToString("dd/MM/yyyy-hh:mm")
                Catch ex As Exception
                    Return ""
                End Try
            Else
                Return ""
            End If
        End Get
    End Property
    Public ReadOnly Property PdfModifDate As String
        Get
            If Not PdfInfo Is Nothing Then
                Try
                    Return PdfInfo.ModificationDate.ToString("dd/MM/yyyy-hh:mm")
                Catch ex As Exception
                    Return ""
                End Try
            Else
                Return ""
            End If
        End Get
    End Property
    Public Property FullPath As String


End Class
