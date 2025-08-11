Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class ExchangeTransactionDetail
    <Key> _
    <Column(order:=1)> _
    <ForeignKey("ExchangeTransactionHead")> _
    <DisplayName("Transaction Number")> _
    Public Property TransNum As Decimal
    Public Overridable Property ExchangeTransactionHead As ExchangeTransactionHead

    <Key> _
    <Column(order:=2)> _
    <DisplayName("Deal Number")> _
    Public Property DealNumber As String

    <DisplayName("Transaction Currency")> _
    <StringLength(3)> _
    <Required(ErrorMessage:="Please select the transaction currency")> _
    Public Property TransactionCurrency As String

    <DisplayName("Transaction Rate")> _
    <Required(ErrorMessage:="Please fill transaction rate")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property TransactionRate As Nullable(Of Decimal)

    <DisplayName("Transaction Nominal")> _
    <Required(ErrorMessage:="Please fill transaction nominal")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property TransactionNominal As Nullable(Of Decimal)

    <DisplayName("Customer Currency")> _
    <StringLength(3)> _
    <Required(ErrorMessage:="Please select the customer currency")> _
    Public Property CustomerCurrency As String

    <DisplayName("Customer Nominal")> _
    <Required(ErrorMessage:="Please fill customer nominal")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property CustomerNominal As Nullable(Of Decimal)

    <DisplayName("Value Period")> _
    <StringLength(4)> _
    <Required(ErrorMessage:="Please select the value period")> _
    Public Property ValuePeriod As String

    <DisplayName("Value Date")> _
    <Required(ErrorMessage:="Please fill the value date")> _
    <DisplayFormat(DataFormatString:="{0:yyyy/MM/dd}", ApplyFormatInEditMode:=True)> _
    Public Property ValueDate As Nullable(Of Date)

    Public Property FlagReconcile As Nullable(Of Integer)

    Public Shared Function GetExchangeTransDetail(ByVal TransNum As String, ByVal db As FectorEntities) As ExchangeTransactionDetail
        Return db.ExchangeTransactionDetail.Where(Function(f) f.TransNum = TransNum).SingleOrDefault
    End Function
End Class
