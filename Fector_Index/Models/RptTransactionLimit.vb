Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel
Public Class RptTransactionLimitView
    <Required(ErrorMessage:="Please Select The Branch")> _
    Public Property Branch As String
End Class
