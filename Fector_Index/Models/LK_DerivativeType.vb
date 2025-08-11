Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class LK_DerivativeType
    <Key> _
    <DisplayName("ID")> _
    Public Property ID As Integer

    <DisplayName("Description")> _
    Public Property Description As String
End Class
