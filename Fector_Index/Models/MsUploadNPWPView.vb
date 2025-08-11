Imports System.ComponentModel.DataAnnotations

Public Class MsUploadNPWPView
    <Required>
    <Display(Name:="File")>
    Public Property File As HttpPostedFileBase
    Public Property accno As String
    Public Property LastFileDirectory As String
End Class
