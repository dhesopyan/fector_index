Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class ExchangeTransactionClose
    <Key> _
    <ForeignKey("ExchangeTransactionHead")> _
    <DisplayName("Transaction Number")> _
    Public Property TransNum As Decimal
    Public Overridable Property ExchangeTransactionHead As ExchangeTransactionHead

    <DisplayName("Deal Number")> _
    Public Property DealNumber As String

    <DisplayName("Close Transaction Reason")> _
    Public Property CloseTransactionReason As String

    Public Property CloseBy As String
    Public Property CloseDate As Nullable(Of DateTime)

    Public Shared Function GetCloseTransactionReason(ByVal TransNum As String, ByVal db As FectorEntities) As ExchangeTransactionClose
        Dim ExchangeTransactionClose = db.ExchangeTransactionClose.Where(Function(f) f.TransNum = TransNum).SingleOrDefault

        Dim mdl As New ExchangeTransactionClose
        With mdl
            .TransNum = ExchangeTransactionClose.TransNum
            .DealNumber = ExchangeTransactionClose.DealNumber
            .CloseTransactionReason = ExchangeTransactionClose.CloseTransactionReason
            .CloseBy = ExchangeTransactionClose.CloseBy
            .CloseDate = ExchangeTransactionClose.CloseDate
        End With

        Return mdl
    End Function
End Class
