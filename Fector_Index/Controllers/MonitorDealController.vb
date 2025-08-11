Public Class MonitorDealController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /MonitorDeal/Index

    Function Index() As ActionResult
        Session("tempDDLValue") = "NotFinished"
        ViewBag.Breadcrumb = "Home > Monitor Deal"
        Return View("Index")
    End Function

    '
    ' GET: /MonitorDeal/Approval
    Function Approval() As ActionResult
        ViewBag.Breadcrumb = "Home > Approval Monitor Deal"
        Return View("Approval")
    End Function

    '
    ' POST : /MonitorDeal/CloseDeal
    <HttpPost> _
    Function CloseDeal(model As CloseDealView, DealNumber As String, Reason As String) As ActionResult
        Dim db As New FectorEntities

        Dim Deal = TransactionDeal.GetTransDealWithDealNumber(model.DealNumber, db)

        Deal.CloseReason = model.CloseDealReason
        Deal.EditBy = User.Identity.Name
        Deal.EditDate = Now
        Deal.Status = "CLOSE - PENDING"
        db.Entry(Deal).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "CLOSE DEAL", model.DealNumber & " / " & model.DealNumber, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST : /MonitorDeal/ApprovalCloseDeal
    <HttpPost> _
    Function ApprovalCloseDeal(model As CloseDealView, ApprovalType As String) As ActionResult
        Dim db As New FectorEntities

        Dim CloseDeal = TransactionDeal.GetTransDealWithDealNumber(model.DealNumber, db)

        CloseDeal.ApproveBy = User.Identity.Name
        CloseDeal.ApproveDate = Now
        If ApprovalType IsNot Nothing Then
            If ApprovalType = "Approve" Then
                CloseDeal.Status = "CLOSED"
            Else
                CloseDeal.CloseReason = Nothing
                CloseDeal.Status = "ACTIVE"
            End If
        End If

        db.Entry(CloseDeal).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "APPROVAL CLOSE DEAL", model.DealNumber, db)

        Return RedirectToAction("Approval")
    End Function

    <HttpPost> _
    Function Approval(model As CloseDealView) As ActionResult
        Dim db As New FectorEntities

        Dim CloseDeal = TransactionDeal.GetTransDealWithDealNumber(model.DealNumber, db)

        CloseDeal.ApproveBy = User.Identity.Name
        CloseDeal.ApproveDate = Now
        If model.ProcessName = "Approve" Then
            CloseDeal.Status = "CLOSED"
        Else
            CloseDeal.CloseReason = Nothing
            CloseDeal.Status = "ACTIVE"
        End If

        db.Entry(CloseDeal).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "APPROVAL CLOSE DEAL", model.DealNumber, db)

        Return RedirectToAction("Approval")
    End Function

    Private Class strMonitorDeal
        Public DealNumber As String
        Public Reason As String
        Public AccNum As String
        Public AccName As String
        Public BranchId As String
        Public TransactionType As String
        Public DealType As String
        Public DealRate As Nullable(Of Decimal)
        Public CurrencyDeal As String
        Public CurrencyCustomer As String
        Public RateCustomer As Nullable(Of Decimal)
        Public AmountCustomer As Nullable(Of Decimal)
        Public DealPeriod As String
        Public DealDate As DateTime
        Public AmountDeal As Nullable(Of Decimal)
        Public RemainDeal As Nullable(Of Decimal)
        Public Status As String
    End Class

    <Authorize> _
    <HttpGet> _
    Public Function DTMonitorDealAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim HeadBranch = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1
        Dim UserBranchID As Decimal = Session("BranchId")
        Dim monitordeal As IQueryable(Of strMonitorDeal)

        If UserBranchID = HeadBranch Then
            monitordeal = (From s In db.TransactionDeal.Include("Branch")
                        Group Join t In db.ExchangeTransactionDetail On s.DealNumber Equals t.DealNumber Into usedTrans = Sum(t.TransactionNominal)
                       Select New strMonitorDeal With {.DealNumber = s.DealNumber, _
                                                       .Reason = s.CloseReason, _
                                                       .AccNum = s.AccNum, _
                                                       .AccName = s.AccName, _
                                                       .BranchId = s.BranchId + " - " + s.Branch.Name, _
                                                       .TransactionType = s.TransactionType, _
                                                       .DealType = s.DealType, _
                                                       .DealRate = s.DealRate, _
                                                       .CurrencyDeal = s.CurrencyDeal, _
                                                       .CurrencyCustomer = s.CurrencyCustomer, _
                                                       .RateCustomer = s.RateCustomer, _
                                                       .AmountCustomer = s.AmountCustomer, _
                                                       .DealPeriod = s.DealPeriod, _
                                                       .DealDate = s.DealDate, _
                                                       .AmountDeal = s.AmountDeal, _
                                                       .RemainDeal = s.AmountDeal - usedTrans, _
                                                       .Status = s.Status}).Distinct
        Else
            monitordeal = (From s In db.TransactionDeal.Include("Branch")
                        Group Join t In db.ExchangeTransactionDetail On s.DealNumber Equals t.DealNumber Into usedTrans = Sum(t.TransactionNominal) From t In db.ExchangeTransactionDetail
                        Where s.BranchId = UserBranchID And t.FlagReconcile = 0
                       Select New strMonitorDeal With {.DealNumber = s.DealNumber, _
                                                       .Reason = s.CloseReason, _
                                                       .AccNum = s.AccNum, _
                                                       .AccName = s.AccName, _
                                                       .BranchId = s.BranchId + " - " + s.Branch.BranchAbbr, _
                                                       .TransactionType = s.TransactionType, _
                                                       .DealType = s.DealType, _
                                                       .DealRate = s.DealRate, _
                                                       .CurrencyDeal = s.CurrencyDeal, _
                                                       .CurrencyCustomer = s.CurrencyCustomer, _
                                                       .RateCustomer = s.RateCustomer, _
                                                       .AmountCustomer = s.AmountCustomer, _
                                                       .DealPeriod = s.DealPeriod, _
                                                       .DealDate = s.DealDate, _
                                                       .AmountDeal = s.AmountDeal, _
                                                       .RemainDeal = s.AmountDeal - usedTrans, _
                                                       .Status = s.Status}).Distinct
        End If

        Return ReturnMonitorDealDataTable(param, monitordeal, "Index")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTMonitorDealApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim monitordeal = From s In db.TransactionDeal.Include("Branch")
                          Group Join t In db.ExchangeTransactionDetail On s.DealNumber Equals t.DealNumber Into usedTrans = Sum(t.TransactionNominal)
                          Where s.Status = "CLOSE - PENDING"
                         Select New strMonitorDeal With {.DealNumber = s.DealNumber, _
                                                         .Reason = s.CloseReason, _
                                                         .AccNum = s.AccNum, _
                                                         .AccName = s.AccName, _
                                                         .BranchId = s.BranchId + " - " + s.Branch.BranchAbbr, _
                                                         .TransactionType = s.TransactionType, _
                                                         .DealType = s.DealType, _
                                                         .DealRate = s.DealRate, _
                                                         .CurrencyDeal = s.CurrencyDeal, _
                                                         .CurrencyCustomer = s.CurrencyCustomer, _
                                                         .RateCustomer = s.RateCustomer, _
                                                         .AmountCustomer = s.AmountCustomer, _
                                                         .DealPeriod = s.DealPeriod, _
                                                         .DealDate = s.DealDate, _
                                                         .AmountDeal = s.AmountDeal, _
                                                         .RemainDeal = s.AmountDeal - usedTrans, _
                                                         .Status = s.Status}

        Return ReturnMonitorDealDataTable(param, monitordeal, "Approval")
    End Function

    Private Function ReturnMonitorDealDataTable(param As jQueryDataTableParamModel, MonitorDeal As IQueryable(Of strMonitorDeal), Mode As String) As JsonResult
        Dim totalRecords = MonitorDeal.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim DealNumberSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim AccNumSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim AccNameSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        Dim BranchSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))
        Dim TransactionTypeSearch As String = If(IsNothing(Request("sSearch_4")), "", Request("sSearch_4"))
        Dim CurrencyDealSearch As String = If(IsNothing(Request("sSearch_5")), "", Request("sSearch_5"))
        Dim DealDateSearch As String = If(IsNothing(Request("sSearch_6")), "", Request("sSearch_6"))
        Dim StatusSearch As String = If(IsNothing(Request("sSearch_11")), "", Request("sSearch_11"))
        MonitorDeal = MonitorDeal.Where(Function(f) f.DealNumber.Contains(DealNumberSearch))
        MonitorDeal = MonitorDeal.Where(Function(f) f.AccNum.Contains(AccNumSearch))
        MonitorDeal = MonitorDeal.Where(Function(f) f.AccName.Contains(AccNameSearch))
        MonitorDeal = MonitorDeal.Where(Function(f) f.BranchId.Contains(BranchSearch))
        MonitorDeal = MonitorDeal.Where(Function(f) f.TransactionType.Contains(TransactionTypeSearch) Or f.DealType.Contains(TransactionTypeSearch))
        MonitorDeal = MonitorDeal.Where(Function(f) f.CurrencyDeal.Contains(CurrencyDealSearch) Or f.CurrencyCustomer.Contains(CurrencyDealSearch))
        'MonitorDeal = MonitorDeal.Where(Function(f) f.AmountCustomer.Contains(AmountCustomerSearch))
        'MonitorDeal = MonitorDeal.Where(Function(f) f.DealDate.Contains(DealDateSearch))
        If DealDateSearch.Trim.Length > 0 And AppHelper.checkDate(DealDateSearch) Then
            Dim dTDate As Date
            dTDate = AppHelper.dateConvert(DealDateSearch)
            MonitorDeal = MonitorDeal.Where(Function(f) f.DealDate.Day = dTDate.Day And f.DealDate.Month = dTDate.Month And f.DealDate.Year = dTDate.Year)
        End If

        If Mode = "Index" Then
            If StatusSearch = "UNFINISHED" Or StatusSearch = "" Then
                MonitorDeal = MonitorDeal.Where(Function(f) f.Status.Contains("UNFINISHED") Or f.Status.Contains("ACTIVE"))
            Else
                MonitorDeal = MonitorDeal.Where(Function(f) f.Status.Contains("FINISHED") And f.Status <> "UNFINISHED")
            End If
        End If
        
        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    MonitorDeal = MonitorDeal.OrderBy(Function(f) f.DealNumber)
                Else
                    MonitorDeal = MonitorDeal.OrderByDescending(Function(f) f.DealNumber)
                End If
            Case 1
                If sortOrder = "asc" Then
                    MonitorDeal = MonitorDeal.OrderBy(Function(f) f.AccNum)
                Else
                    MonitorDeal = MonitorDeal.OrderByDescending(Function(f) f.AccNum)
                End If
            Case 2
                If sortOrder = "asc" Then
                    MonitorDeal = MonitorDeal.OrderBy(Function(f) f.AccName)
                Else
                    MonitorDeal = MonitorDeal.OrderByDescending(Function(f) f.AccName)
                End If
            Case 3
                If sortOrder = "asc" Then
                    MonitorDeal = MonitorDeal.OrderBy(Function(f) f.BranchId)
                Else
                    MonitorDeal = MonitorDeal.OrderByDescending(Function(f) f.BranchId)
                End If
            Case 4
                If sortOrder = "asc" Then
                    MonitorDeal = MonitorDeal.OrderBy(Function(f) f.TransactionType)
                Else
                    MonitorDeal = MonitorDeal.OrderByDescending(Function(f) f.TransactionType)
                End If
            Case 5
                If sortOrder = "asc" Then
                    MonitorDeal = MonitorDeal.OrderBy(Function(f) f.DealType)
                Else
                    MonitorDeal = MonitorDeal.OrderByDescending(Function(f) f.DealType)
                End If
            Case 6
                If sortOrder = "asc" Then
                    MonitorDeal = MonitorDeal.OrderBy(Function(f) f.CurrencyDeal)
                Else
                    MonitorDeal = MonitorDeal.OrderByDescending(Function(f) f.CurrencyDeal)
                End If
            Case 7
                If sortOrder = "asc" Then
                    MonitorDeal = MonitorDeal.OrderBy(Function(f) f.CurrencyCustomer)
                Else
                    MonitorDeal = MonitorDeal.OrderByDescending(Function(f) f.CurrencyCustomer)
                End If
            Case 8
                If sortOrder = "asc" Then
                    MonitorDeal = MonitorDeal.OrderBy(Function(f) f.DealPeriod)
                Else
                    MonitorDeal = MonitorDeal.OrderByDescending(Function(f) f.DealPeriod)
                End If
            Case 9
                If sortOrder = "asc" Then
                    MonitorDeal = MonitorDeal.OrderBy(Function(f) f.Status)
                Else
                    MonitorDeal = MonitorDeal.OrderByDescending(Function(f) f.Status)
                End If
            Case 10
                If sortOrder = "asc" Then
                    MonitorDeal = MonitorDeal.OrderBy(Function(f) f.RemainDeal)
                Else
                    MonitorDeal = MonitorDeal.OrderByDescending(Function(f) f.RemainDeal)
                End If
            Case 11
                If sortOrder = "asc" Then
                    MonitorDeal = MonitorDeal.OrderBy(Function(f) f.Status)
                Else
                    MonitorDeal = MonitorDeal.OrderByDescending(Function(f) f.Status)
                End If
        End Select

        Dim result As New List(Of String())
        For Each data As strMonitorDeal In MonitorDeal
            result.Add({data.DealNumber, _
                        data.Reason, _
                        data.AccNum, _
                        data.AccName, _
                        data.BranchId, _
                        data.TransactionType + " - " + data.DealType, _
                        data.CurrencyDeal + " - " + data.CurrencyCustomer + " - " + CStr(CDec(data.DealRate).ToString("N2")), _
                        CStr(CDec(data.AmountCustomer).ToString("N2")) + " - " + CStr(CDec(data.AmountDeal).ToString("N2")), _
                        CDate(data.DealDate).ToString("dd-MM-yyyy"), _
                        CDec(data.AmountDeal).ToString("N2"), _
                        CDec(IIf(data.RemainDeal Is Nothing, data.AmountDeal, data.RemainDeal)).ToString("N2"), _
                        data.Status, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

End Class