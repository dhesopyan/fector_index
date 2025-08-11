Public Class DailyProfitController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /DailyProfit/Index
    <Authorize> _
    Function Index() As ActionResult
        Dim model As New DailyProfitViewModel
        model.StartPeriod = Now.Date
        model.EndPeriod = Now.Date

        ViewBag.Breadcrumb = String.Format("Home > Daily Profit")
        Return View("Index", model)
    End Function

    '
    ' POST: /DailyProfit/Index
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Function Index(model As DailyProfitViewModel) As ActionResult
        Dim db As New FectorEntities
        Dim Profit As New DailyProfit

        ViewBag.Breadcrumb = String.Format("Home > Daily Profit")

        If ModelState.IsValid Then
            For Each row As DealWithProfit In model.ListDailyProfit
                Dim DeletedProfit = db.DailyProfit.Find(row.ID)
                If DeletedProfit IsNot Nothing Then
                    db.DailyProfit.Remove(DeletedProfit)
                End If

                If row.GrandTotal <> CDec(0).ToString("N2") Then
                    Profit = New DailyProfit

                    Profit.ID = row.ID
                    Profit.BEP = row.BEP
                    Profit.BranchProfit = row.BranchProfit

                    db.DailyProfit.Add(Profit)
                End If

                db.SaveChanges()
            Next
        End If

        model = New DailyProfitViewModel
        Return View("Index", model)
    End Function

    <HttpPost> _
    Public Function JsOnDailyProfit(ByVal startperiod As String, ByVal endperiod As String, model As DailyProfitViewModel) As ActionResult
        Dim db As New FectorEntities

        startperiod = AppHelper.dateConvert(startperiod)
        endperiod = AppHelper.dateConvert(endperiod)

        Dim SelectDealWithProfit = (From a In db.TransactionDeal
                             Group Join b In db.DailyProfit On a.ID Equals b.ID Into Group From b In Group.DefaultIfEmpty
                             Where a.DealDate >= startperiod And a.DealDate <= endperiod
                             Select New DealWithProfit With {.ID = a.ID, _
                                                             .DealDate = a.DealDate, _
                                                             .DealNumber = a.DealNumber, _
                                                             .AccNum = a.AccNum, _
                                                             .AccName = a.AccName, _
                                                             .CurrencyDeal = a.CurrencyDeal, _
                                                             .AmountDeal = a.AmountDeal, _
                                                             .DealRate = a.DealRate, _
                                                             .BEP = If(b.BEP Is Nothing, 0, b.BEP), _
                                                             .BranchProfit = If(b.BranchProfit Is Nothing, 0, b.BranchProfit), _
                                                             .GrandTotal = If(If(b.BEP Is Nothing, 0, b.BEP) = 0, _
                                                                              If(If(b.BranchProfit Is Nothing, 0, b.BranchProfit) = 0, _
                                                                                0, _
                                                                                b.BranchProfit * a.AmountDeal),
                                                                            (((b.BEP - a.DealRate) * -1) * a.AmountDeal) / 2)}).tolist

        Dim result As New List(Of String())
        For Each row As DealWithProfit In SelectDealWithProfit
            result.Add({row.ID, CDate(row.DealDate).ToString("yyyy/MM/dd"), row.DealNumber, row.AccNum, row.AccName, row.CurrencyDeal, CDec(row.AmountDeal).ToString("N2"), CDec(row.DealRate).ToString("N2"), CDec(row.BEP).ToString("N2"), CDec(row.BranchProfit).ToString("N2"), CDec(row.GrandTotal).ToString("N2")})
        Next
        model.ListDailyProfit = SelectDealWithProfit
        Return Json(result)
        'Return View("Index", model)
    End Function

End Class