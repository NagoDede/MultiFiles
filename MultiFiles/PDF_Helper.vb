Imports PdfSharp
Imports PdfSharp.Drawing
Imports PdfSharp.Pdf
Imports PdfSharp.Pdf.IO

Public Class PDF_Helper
    ''' <summary>
    ''' Retrieve the data stored in the PDF metadata fields and store them in a PdfDocumentInforamtion object
    ''' In case of error (it cculd happend that the metadata could not be read), then return Nothing. 
    ''' User will have to check the returned value before use.
    ''' </summary>
    ''' <param name="filePath"></param>
    ''' <returns></returns>
    Public Shared Function RetrievePdfMetadata(filePath As String) As PdfDocumentInformation
        Dim pdfDoc As PdfDocument = New PdfDocument()
        Dim pdfInfo As PdfDocumentInformation
        Try
            pdfDoc = PdfReader.Open(filePath)
            pdfInfo = pdfDoc.Info
            pdfDoc.Close()
            Return pdfInfo
        Catch ex As Exception
            Debug.Print(ex.ToString())
            pdfDoc.Close()
            Return Nothing
        End Try
    End Function
End Class
