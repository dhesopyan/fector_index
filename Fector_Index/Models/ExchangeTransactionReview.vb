Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel
Imports System.IO

Public Class ExchangeTransactionReview
    <Key> _
    <ForeignKey("ExchangeTransactionHead")> _
    <DisplayName("Transaction Number")> _
    Public Property TransNum As Decimal
    Public Overridable Property ExchangeTransactionHead As ExchangeTransactionHead

    Public Property FlagReview As Integer
    Public Property ReviewBy As String
    Public Property ReviewDate As Nullable(Of DateTime)

    Public Shared Function GetExchangeTransReview(ByVal TransNum As String, ByVal db As FectorEntities) As ExchangeTransactionReview
        Return db.ExchangeTransactionReview.Where(Function(f) f.TransNum = TransNum).SingleOrDefault
    End Function
End Class
