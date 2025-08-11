Public Class DownloadFileStatementController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /DownloadFileStatement

    Function Index() As ActionResult
        Return View()
    End Function

    Sub DownloadUp25()
        Dim strRequest As String = "SuratPernyataanDiatasThresholdUSD25000.pdf"
        Dim path As String = Server.MapPath(strRequest)
        Dim file As System.IO.FileInfo = New System.IO.FileInfo(path)
        If file.Exists Then
            Response.Clear()
            Response.AddHeader("Content-Disposition", "attachment; filename=" & file.Name)
            Response.AddHeader("Content-Length", file.Length.ToString())
            Response.ContentType = "application/pdf"
            Response.WriteFile(file.FullName)
            Response.End() 'if file does not exist
        Else
            Response.Write("This file does not exist.")
        End If
    End Sub

    Sub DownloadUntil25()
        Dim strRequest As String = "SuratPernyataanSampaiThresholdUSD25000.pdf"
        Dim path As String = Server.MapPath(strRequest)
        Dim file As System.IO.FileInfo = New System.IO.FileInfo(path)
        If file.Exists Then
            Response.Clear()
            Response.AddHeader("Content-Disposition", "attachment; filename=" & file.Name)
            Response.AddHeader("Content-Length", file.Length.ToString())
            Response.ContentType = "application/pdf"
            Response.WriteFile(file.FullName)
            Response.End() 'if file does not exist
        Else
            Response.Write("This file does not exist.")
        End If
    End Sub

End Class