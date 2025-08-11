Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel

Public Class MonitorDeal
    <StringLength(20)> _
    <Key> _
    <DisplayName("Deal Number")> _
    <Required(ErrorMessage:="Please fill deal number")> _
    Public Property DealNumber As String

    <StringLength(100)> _
    <DisplayName("Account Number")> _
    <Required(ErrorMessage:="Please fill account number")> _
    Public Property AccNum As String

    <StringLength(100)> _
    <DisplayName("Account Name")> _
    <Required(ErrorMessage:="Please fill account name")> _
    Public Property AccName As String

    <DisplayName("Deal Rate")> _
    <Required(ErrorMessage:="Please fill deal rate")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property DealRate As Nullable(Of Decimal)

    <DisplayName("Amount Deal")> _
    <Required(ErrorMessage:="Please fill amount deal")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property AmountDeal As Nullable(Of Decimal)

    Public Property Status As String
End Class
