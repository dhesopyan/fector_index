Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class DealWithProfit
    Public Property ID As Decimal

    <DisplayFormat(DataFormatString:="{0:yyyy/MM/dd}", ApplyFormatInEditMode:=True)> _
    Public Property DealDate As Nullable(Of Date)

    Public Property DealNumber As String
    Public Property AccNum As String
    Public Property AccName As String
    Public Property CurrencyDeal As String

    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property AmountDeal As Decimal

    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property DealRate As Decimal

    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property BEP As Nullable(Of Decimal)

    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property BranchProfit As Nullable(Of Decimal)

    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property GrandTotal As Decimal
End Class
