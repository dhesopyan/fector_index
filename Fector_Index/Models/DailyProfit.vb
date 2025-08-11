Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class DailyProfit
    <Key> _
    <DisplayName("ID")> _
    Public Property ID As Decimal

    <DisplayName("BEP")> _
    Public Property BEP As Nullable(Of Decimal)

    <DisplayName("BranchProfit")> _
    Public Property BranchProfit As Nullable(Of Decimal)

    Public Shared Function GetTransDealWithDailyProfit(ByVal StartPeriod As DateTime, ByVal EndPeriod As DateTime, ByVal db As FectorEntities) As DealWithProfit
        Dim DealWithProfit = From a In db.TransactionDeal
                             Join b In db.DailyProfit On a.ID Equals b.ID
                             Where a.DealDate >= StartPeriod And a.DealDate <= EndPeriod
                             Select New DealWithProfit With {.ID = a.ID, _
                                                             .DealDate = a.DealDate, _
                                                             .DealNumber = a.DealNumber, _
                                                             .AccNum = a.AccNum, _
                                                             .AccName = a.AccName, _
                                                             .CurrencyDeal = a.CurrencyDeal, _
                                                             .AmountDeal = a.AmountDeal, _
                                                             .DealRate = a.DealRate, _
                                                             .BEP = b.BEP, _
                                                             .BranchProfit = b.BranchProfit, _
                                                             .GrandTotal = If(b.BEP = 0, b.BranchProfit * a.AmountDeal, If(b.BEP = 0, 0, (-(b.BEP - a.DealRate) * a.AmountDeal) / 2))}

        Return DealWithProfit
    End Function
End Class
