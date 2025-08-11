Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel
Imports System.IO

Public Class MsDocUnderlying
    <Key> _
    <Column(order:=1)>
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property ID As Decimal

    <Key> _
    <Column(order:=2)>
    <ForeignKey("ExchangeTransactionHead")> _
    Public Property TransNum As Decimal
    Public Overridable Property ExchangeTransactionHead As ExchangeTransactionHead

    Public Property DocNum As String
    Public Property DocType As String
    Public Property NominalDoc As Nullable(Of Decimal)

    Public Shared Function GetDocUnderlying(ByVal DocNum As String, ByVal db As FectorEntities) As MsDocUnderlying
        Return db.DocUnderlying.Where(Function(f) f.DocNum = DocNum).SingleOrDefault
    End Function

End Class
