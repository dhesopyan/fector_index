Imports System.Net
Imports System.Data.Objects.SqlClient
Imports System.IO
Imports System.Data.Entity.Validation
Imports System.Data.Objects

Public Class HomeController
    Inherits System.Web.Mvc.Controller

    <Authorize> _
    Function Index() As ActionResult
        Dim db As New FectorEntities
        ViewBag.BranchList = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.CurrencyList = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")
        ViewBag.Breadcrumb = "Home"
        Return View()
    End Function

    Private Class DataExRate
        Public TTime As DateTime
        Public BNSellRate As Nullable(Of Decimal)
        Public BNBuyRate As Nullable(Of Decimal)
        Public TTSellRate As Nullable(Of Decimal)
        Public TTBuyRate As Nullable(Of Decimal)
    End Class

    Private Class SubQueryExRate
        Public CurrId As String
        Public TTime As Date
        Public BNSellRate As Nullable(Of Decimal)
        Public BNBuyRate As Nullable(Of Decimal)
        Public TTSellRate As Nullable(Of Decimal)
        Public TTBuyRate As Nullable(Of Decimal)
    End Class

    '
    ' Post:  /Home/JsOnLineBarExRate
    Public Function JsOnLineBarExRate(currency As String) As ActionResult
        Dim db As New FectorEntities

        Dim SubQuery = From a In db.ExchangeRateMaster
                       Select New SubQueryExRate With {.CurrId = a.CurrId, _
                                                       .TTime = a.TTime,
                                                       .BNSellRate = a.BNSellRate, _
                                                       .BNBuyRate = a.BNBuyRate, _
                                                       .TTSellRate = a.TTSellRate, _
                                                       .TTBuyRate = a.TTBuyRate}

        Dim DataSubQuery = From a In SubQuery
                           Group a By a.CurrId, a.TTSellRate, a.TTBuyRate, a.BNSellRate, a.BNBuyRate, _
                           tmp = EntityFunctions.TruncateTime(a.TTime) Into Group = Max(a.TTime)
                           Select New With {.CurrId = CurrId, .TTime = Group, .ttsellrate = TTSellRate, _
                                           .ttbuyrate = TTBuyRate, _
                                           .bnsellrate = BNSellRate, _
                                            .bnbuyrate = BNBuyRate}

        Dim day = From a In db.ExchangeRateMaster
                  Select a.TTime

        Dim dday = ToString()

        Dim ExRate = (From a In db.ExchangeRateMaster
                     Join b In DataSubQuery On a.TTime Equals b.TTime
                     Where a.CurrId = currency And a.Status = "ACTIVE" And SqlFunctions.DateName("dw", a.TTime) = "Friday"
                     Select New DataExRate With {.TTime = a.TTime, _
                                                 .BNSellRate = a.BNSellRate, _
                                                 .BNBuyRate = a.BNBuyRate, _
                                                 .TTSellRate = a.TTSellRate, _
                                                 .TTBuyRate = a.TTBuyRate
                                                 }).Distinct


        Dim result As New List(Of String())
        For Each Data As DataExRate In ExRate
            result.Add({Data.TTime.ToString("yyyy-MM-dd"), _
                        Data.BNSellRate, _
                        Data.BNBuyRate, _
                        Data.TTSellRate, _
                        Data.TTBuyRate})
        Next

        Return Json(result, JsonRequestBehavior.AllowGet)
    End Function

    Private Class DataDeal
        Public Property DealDate As DateTime
        Public Property CurrId As String
        Public Property TotalDeal As Nullable(Of Decimal)
    End Class

    '
    ' Post:  /Home/JsOnLineBarDeal
    Public Function JsOnLineBarDeal(currency As String, branchId As String, startPeriod As String, endPeriod As String) As ActionResult
        Dim db As New FectorEntities

        If startPeriod = "" Then
            startPeriod = Now.ToString("dd-MM-yyyy")
        End If

        If endPeriod = "" Then
            endPeriod = Now.ToString("dd-MM-yyyy")
        End If

        Dim ConverStartDate As Date = AppHelper.dateConvert(startPeriod)
        Dim ConvertEndDate As Date = AppHelper.dateConvert(endPeriod)

        Dim Deal = From a In db.TransactionDeal
                   Where a.Status.Contains("PENDING") = False And a.CurrencyDeal = currency And a.BranchId = branchId And a.DealDate >= ConverStartDate And a.DealDate <= ConvertEndDate
                   Select a

        Dim ListDeal = From a In Deal
                       Group By tmp = EntityFunctions.TruncateTime(a.DealDate) Into Group = Sum(a.AmountDeal)
                       Select New DataDeal With {.DealDate = tmp, _
                                                 .TotalDeal = Group}

        Dim result As New List(Of String())
        For Each Data As DataDeal In ListDeal
            result.Add({Data.DealDate.ToString("yyyy-MM-dd"), _
                        Data.TotalDeal})
        Next

        Return Json(result, JsonRequestBehavior.AllowGet)
    End Function

    Private Class Transaction
        Public Property TDate As DateTime
        Public Property TransCurr As String
        Public Property TransNominal As Nullable(Of Decimal)
    End Class

    Private Class DataTransaction
        Public Property TDate As DateTime
        Public Property TotalTrans As Nullable(Of Decimal)
    End Class

    '
    ' Post:  /Home/JsOnLineBarTrans
    Public Function JsOnLineBarTrans(currency As String) As ActionResult
        Dim db As New FectorEntities

        Dim Trans = From a In db.ExchangeTransactionDetail
                    Join b In db.ExchangeTransactionHead On a.TransNum Equals b.TransNum
                    Where b.Status.Contains("PENDING") = False And a.TransactionCurrency = currency
                    Select New Transaction With {.TDate = b.TDate, _
                                               .TransCurr = a.TransactionCurrency, _
                                               .TransNominal = a.TransactionNominal}

        Dim ListTrans = From a In Trans
                        Group By tmp = EntityFunctions.TruncateTime(a.TDate) Into Group = Sum(a.TransNominal)
                        Select New DataTransaction With {.TDate = tmp, _
                                                         .TotalTrans = Group}

        Dim result As New List(Of String())
        For Each Data As DataTransaction In ListTrans
            result.Add({Data.TDate.ToString("yyyy-MM-dd"), _
                        Data.TotalTrans})
        Next

        Return Json(result, JsonRequestBehavior.AllowGet)
    End Function
End Class