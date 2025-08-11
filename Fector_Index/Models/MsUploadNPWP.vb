Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class MsUploadNPWP
    <Key> _
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property UploadId As Decimal

    Public Property AccNum As String
    Public Property FileDirectory As String
End Class
