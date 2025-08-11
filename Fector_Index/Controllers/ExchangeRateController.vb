Imports System.Net
Imports System.Data.Objects.SqlClient
Imports System.Data.Objects

Public Class ExchangeRateController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /ExchangeRate

    Public Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > Exchange Rate"
        Return View("Index")
    End Function

    ' GET: /ExchangeRate/Approval
    <Authorize> _
    Public Function Approval() As ActionResult
        ViewBag.Breadcrumb = "Home > Exchange Rate > Approval"
        Return View("Index")
    End Function

    <Authorize> _
    Public Function History() As ActionResult
        ViewBag.Breadcrumb = "Home > Exchange Rate > History"
        Return View("History")
    End Function

    '
    ' POST: /ExchangeRate/Approve
    <Authorize> _
    <HttpPost> _
    Public Function Approve() As ActionResult
        Dim db As New FectorEntities()
        Dim id As Decimal = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedRate = db.ExchangeRateMaster.Find(id)

        If IsNothing(editedRate) Then
            Return New HttpNotFoundResult
        End If

        Dim flagDelete As Boolean = False
        editedRate.Status = "ACTIVE"
        editedRate.ApproveBy = User.Identity.Name
        editedRate.ApproveDate = Now
        db.Entry(editedRate).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "APPROVE EXCHANGE RATE", editedRate.CurrId, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' POST: /DocumentLHBU/Reject
    <Authorize> _
    <HttpPost> _
    Public Function Reject() As ActionResult
        Dim db As New FectorEntities()
        Dim id As Decimal = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedRate = db.ExchangeRateMaster.Find(id)

        If IsNothing(editedRate) Then
            Return New HttpNotFoundResult
        End If

        Dim flagDelete As Boolean = False
        editedRate.Status = "REJECTED"
        editedRate.ApproveBy = User.Identity.Name
        editedRate.ApproveDate = Now
        db.Entry(editedRate).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "REJECT EXCHANGE RATE", editedRate.CurrId, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' Get: /ExchangeRate/Create
    <Authorize> _
    Public Function Create() As ActionResult
        Dim model As New MsExchangeRate
        Dim db As New FectorEntities

        ViewData("Title") = "Create New Exchange Rate"
        ViewBag.Breadcrumb = "Home > Exchange Rate > Create"
        ViewBag.CurrencyList = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")

        model.TTBuyRate = 0.0
        model.TTSellRate = 0.0
        model.BNBuyRate = 0.0
        model.BNSellRate = 0.0
        model.ClosingRate = 0.0

        Return View("Detail", model)
    End Function

    '
    ' Post: /ExchangeRate/Create
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Create(model As MsExchangeRate) As ActionResult
        Dim db As New FectorEntities()

        ViewData("Title") = "Create New Exchange Rate"
        ViewBag.Breadcrumb = "Home > Exchange Rate > Create"
        ViewBag.CurrencyList = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")

        If ModelState.IsValid Then
            Dim Curr As String = model.CurrId
            Dim CoreCurr As String = db.Currencies.Find(Curr).CoreCurrId
            Dim TTBuy As Decimal = model.TTBuyRate
            Dim TTSell As Decimal = model.TTSellRate
            Dim BNBuy As Decimal = model.BNBuyRate
            Dim BNSell As Decimal = model.BNSellRate
            Dim ClosingRate As Decimal = model.ClosingRate

            Dim newRate As New MsExchangeRate
            With newRate
                .TTime = DateTime.Now
                .CurrId = Curr
                .CoreCurrId = CoreCurr
                .TTBuyRate = TTBuy
                .TTSellRate = TTSell
                .BNBuyRate = BNBuy
                .BNSellRate = BNSell
                .ClosingRate = ClosingRate
                .EditBy = User.Identity.Name
                .EditDate = DateTime.Now
                .ApproveBy = ""
                .ApproveDate = New DateTime(1900, 1, 1)
                .Status = "CREATE - PENDING"
            End With
            db.ExchangeRateMaster.Add(newRate)
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "CREATE EXCHANGE RATE", model.CurrId, db)

            Return RedirectToAction("Index", "ExchangeRate")
        End If

        Return View("Detail", model)
    End Function

    Private Class ExRate
        Public RateId As String
        Public TDate As DateTime
        Public TTime As DateTime
        Public CoreCurrId As String
        Public CurrId As String
        Public TTSellRate As Nullable(Of Decimal)
        Public TTBuyRate As Nullable(Of Decimal)
        Public BNSellRate As Nullable(Of Decimal)
        Public BNBuyRate As Nullable(Of Decimal)
        Public ClosingRate As Nullable(Of Decimal)
        Public Status As String
    End Class

    <Authorize> _
    <HttpGet> _
    Public Function DTExchangeRateAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim ExchangeRateMaster = (From s In db.ExchangeRateMaster
                                  Where s.TTime.Year = Now.Year And s.TTime.Month = Now.Month And s.TTime.Day = Now.Day And s.Status = "ACTIVE"
                                  Order By s.TTime Descending
                                 Select New ExRate With {.RateId = s.RateId, .TDate = s.TTime, .TTime = s.TTime, .CoreCurrId = s.CoreCurrId, .CurrId = s.CurrId, _
                                                         .TTSellRate = s.TTSellRate, .TTBuyRate = s.TTBuyRate, .BNSellRate = s.BNSellRate, .BNBuyRate = s.BNBuyRate, .ClosingRate = s.ClosingRate, .Status = s.Status}).AsQueryable

        Return ReturnExchangeRateDataTable(param, ExchangeRateMaster)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTExchangeRateApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim ExchangeRateMaster = (From s In db.ExchangeRateMaster
                                   Where s.TTime.Year = Now.Year And s.TTime.Month = Now.Month And s.TTime.Day = Now.Day And s.Status.Contains("PENDING")
                                  Order By s.TTime Descending
                                 Select New ExRate With {.RateId = s.RateId, .TDate = s.TTime, .TTime = s.TTime, .CoreCurrId = s.CoreCurrId, .CurrId = s.CurrId, _
                                                         .TTSellRate = s.TTSellRate, .TTBuyRate = s.TTBuyRate, .BNSellRate = s.BNSellRate, .BNBuyRate = s.BNBuyRate, .ClosingRate = s.ClosingRate, .Status = s.Status}).AsQueryable

        Return ReturnExchangeRateDataTable(param, ExchangeRateMaster)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTExchangeRateHistoryAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim ExchangeRateMaster = From s In db.ExchangeRateMaster.Include("Currency")
                                 Select New ExRate With {.RateId = s.RateId, .TDate = s.TTime, .TTime = s.TTime, .CoreCurrId = s.CoreCurrId, .CurrId = s.CurrId, _
                                                         .TTSellRate = s.TTSellRate, .TTBuyRate = s.TTBuyRate, .BNSellRate = s.BNSellRate, .BNBuyRate = s.BNBuyRate, .ClosingRate = s.ClosingRate, .Status = s.Status}

        Return ReturnExchangeRateDataTable(param, ExchangeRateMaster)
    End Function

    Private Function ReturnExchangeRateDataTable(param As jQueryDataTableParamModel, ExRate As IQueryable(Of ExRate)) As JsonResult
        Dim totalRecords = ExRate.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim TDate As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim TTime As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1").Replace("%3a", ":"))
        Dim Rate As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        Dim Status As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))

        If TDate.Trim.Length > 0 And AppHelper.checkDate(TDate) Then
            Dim dTDate As Date
            dTDate = AppHelper.dateConvert(TDate)
            ExRate = ExRate.Where(Function(f) f.TTime.Day = dTDate.Day And f.TTime.Month = dTDate.Month And f.TTime.Year = dTDate.Year)
        End If

        If TTime <> "" Then
            Dim dTTime As TimeSpan
            dTTime = AppHelper.timeConvert(TTime)
            ExRate = ExRate.Where(Function(f) f.TTime.Hour = dTTime.Hours And f.TTime.Minute = dTTime.Minutes And f.TTime.Second = dTTime.Seconds)
        End If

        ExRate = ExRate.Where(Function(f) f.CoreCurrId.Contains(Rate) Or f.CurrId.Contains(Rate))
        If Status <> "" Then
            ExRate = ExRate.Where(Function(f) f.Status.StartsWith(Status))
        End If


        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    ExRate = ExRate.OrderBy(Function(f) f.RateId)
                Else
                    ExRate = ExRate.OrderByDescending(Function(f) f.RateId)
                End If
            Case 1
                If sortOrder = "asc" Then
                    ExRate = ExRate.OrderBy(Function(f) f.TDate)
                Else
                    ExRate = ExRate.OrderByDescending(Function(f) f.TDate)
                End If
            Case 2
                If sortOrder = "asc" Then
                    ExRate = ExRate.OrderBy(Function(f) f.TTime)
                Else
                    ExRate = ExRate.OrderByDescending(Function(f) f.TTime)
                End If
            Case 3
                If sortOrder = "asc" Then
                    ExRate = ExRate.OrderBy(Function(f) f.CoreCurrId)
                Else
                    ExRate = ExRate.OrderByDescending(Function(f) f.CoreCurrId)
                End If
            Case 4
                If sortOrder = "asc" Then
                    ExRate = ExRate.OrderBy(Function(f) f.Status)
                Else
                    ExRate = ExRate.OrderByDescending(Function(f) f.Status)
                End If
        End Select

        Dim result As New List(Of String())
        For Each data As ExRate In ExRate
            result.Add({data.RateId, data.TDate.Date.ToString("dd-MM-yyyy"), data.TTime.ToString("HH:mm:ss"), data.CoreCurrId & " - " & data.CurrId, CDec(data.TTSellRate).ToString("N2"), CDec(data.TTBuyRate).ToString("N2"), CDec(data.BNSellRate).ToString("N2"), CDec(data.BNBuyRate).ToString("N2"), CDec(data.ClosingRate).ToString("N2"), data.Status, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)}, JsonRequestBehavior.AllowGet)
    End Function


End Class