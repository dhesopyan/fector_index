Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class MsExceptionUrl
    <Key> _
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property ID As Integer

    Public Property ExceptionUrl As String
End Class
