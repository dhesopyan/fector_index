Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel

Public Class MsCurrency
    <Key> _
    <StringLength(3)> _
    Public Property CurrId As String

    <StringLength(3)> _
    Public Property CoreCurrId As String

    <StringLength(100)> _
    Public Property Description As String

    Public ReadOnly Property CurrencyDisplay As String
        Get
            Return String.Format("{0} - {1}", CurrId, Description)
        End Get
    End Property
End Class
