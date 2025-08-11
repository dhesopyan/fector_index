Imports System.Net
Imports System.Data.Objects.SqlClient
Imports System.Data.Entity.Validation

Public Class TransactionDealController
    Inherits System.Web.Mvc.Controller

    Dim RateTypeOption As IEnumerable(Of SelectListItem) = {New SelectListItem With {.Text = "TT", .Value = "TT"}, _
                                                            New SelectListItem With {.Text = "Bank Notes", .Value = "BN"}}

    Dim TransactionTypeOption As IEnumerable(Of SelectListItem) = {New SelectListItem With {.Text = "Sell", .Value = "Sell"}, _
                                                            New SelectListItem With {.Text = "Buy", .Value = "Buy"}}

    Dim DealPeriodOption As IEnumerable(Of SelectListItem) = {New SelectListItem With {.Text = "TOD", .Value = "TOD"}, _
                                                              New SelectListItem With {.Text = "TOM", .Value = "TOM"}, _
                                                              New SelectListItem With {.Text = "SPOT", .Value = "SPOT"}, _
                                                              New SelectListItem With {.Text = "FORWARD", .Value = "FWD"}}

    ' GET: /TransactionDeal/Index
    <Authorize> _
    Function Index() As ActionResult
        ViewBag.Breadcrumb = String.Format("Home > Transaction Deal")
        Return View()
    End Function

    ' GET: /TransactionDeal/Approval
    <Authorize> _
    Public Function Approval() As ActionResult
        ViewBag.Breadcrumb = String.Format("Home > Transaction Deal > Approval")
        Return View("Index")
    End Function

    ' GET: /TransactionDeal/History
    <Authorize> _
    Public Function History() As ActionResult
        ViewBag.Breadcrumb = String.Format("Home > Transaction Deal > History")
        Return View("Index")
    End Function

    Private Class GetCurrency_View
        Public Property CurrId As String
        Public Property CurrDisplay As String
    End Class

    Public Function GenerateDealNumber() As String
        Dim model As New TransactionDeal
        Dim db As New FectorEntities

        Dim maxNumber = db.TransactionDeal.Where(Function(f) f.DealNumber.Contains(Now.Year)).OrderByDescending(Function(f) f.DealNumber).FirstOrDefault

        model.DealDate = DateTime.Now.Date

        Dim month As String = DateTime.Now.Date.Month
        Dim year As String = DateTime.Now.Date.Year
        Dim MonthRoman As String = ""

        If month = "1" Then
            MonthRoman = "I"
        ElseIf month = "2" Then
            MonthRoman = "II"
        ElseIf month = "3" Then
            MonthRoman = "III"
        ElseIf month = "4" Then
            MonthRoman = "IV"
        ElseIf month = "5" Then
            MonthRoman = "V"
        ElseIf month = "6" Then
            MonthRoman = "VI"
        ElseIf month = "7" Then
            MonthRoman = "VII"
        ElseIf month = "8" Then
            MonthRoman = "VIII"
        ElseIf month = "9" Then
            MonthRoman = "IX"
        ElseIf month = "10" Then
            MonthRoman = "X"
        ElseIf month = "11" Then
            MonthRoman = "XI"
        Else
            MonthRoman = "XII"
        End If

        If maxNumber Is Nothing Then
            Return "1".ToString.PadLeft(6, "0") & "/DC/" & MonthRoman & "/" & year
        Else
            Dim tempDealNumber = CInt(maxNumber.DealNumber.Substring(0, 6)) + 1
            Return tempDealNumber.ToString.PadLeft(6, "0") & "/DC/" & MonthRoman & "/" & year
        End If
    End Function

    '
    ' Get: /TransactionDeal/Create
    <Authorize> _
    Public Function Create() As ActionResult
        Dim model As New TransactionDeal
        Dim db As New FectorEntities
        Session("BranchHO") = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1

        model.DealNumber = "(AUTO)"
        model.DealRate = 0
        model.AmountDeal = 0
        model.AmountCustomer = 0
        ViewData("Title") = "Create Transaction Deal"
        ViewBag.Breadcrumb = String.Format("Home > Transaction Deal > Create")
        ViewBag.DealNumber = New SelectList(db.TransactionDeal, "DealNumber", "DealNumber")
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(db.ExchangeRate.Where(Function(f) f.CurrId <> "IDR"), "CurrId", "CurrencyDisplay")
        ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
        ViewBag.RateTypeOption = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption

        Return View("Detail", model)
    End Function

    '
    ' Post: /TransactionDeal/Create
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Create(model As TransactionDeal) As ActionResult
        Dim db As New FectorEntities()

        ViewData("Title") = "Create Transaction Deal"
        ViewBag.Breadcrumb = String.Format("Home > Transaction Deal > Create")
        ViewBag.DealNumber = New SelectList(db.TransactionDeal, "DealNumber", "DealNumber")
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(db.ExchangeRate.Where(Function(f) f.CurrId <> "IDR"), "CurrId", "CurrencyDisplay")
        ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
        ViewBag.RateTypeOption = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption

        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim AccountNumber As String = model.AccNum
            Dim AccountName As String = model.AccName
            Dim BranchId As String = ""
            If Session("BranchId") = Session("BranchHO") Then
                BranchId = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1
            Else
                BranchId = Session("BranchId")
            End If
            Dim TransactionType As String = model.TransactionType
            Dim DealType As String = model.DealType
            Dim CurrencyDeal As String = model.CurrencyDeal
            Dim DealRate As Decimal = model.DealRate
            Dim AmountDeal As Decimal = model.AmountDeal
            Dim CurrencyCustomer As String = model.CurrencyCustomer
            Dim RateCustomer As Decimal = model.RateCustomer
            Dim AmountCustomer As Decimal = model.AmountCustomer
            Dim DealPeriod As String = model.DealPeriod
            Dim DealDate As String = model.DealDate

            Dim DayDealDate As String = DealDate.Split(CChar("/"))(1)
            Dim MonthDealDate As String = DealDate.Split(CChar("/"))(0)
            Dim YearDealDate As String = DealDate.Split(CChar("/"))(2)

            Dim RealDealDate As DateTime = CDate(YearDealDate & "-" & MonthDealDate & "-" & DayDealDate) + Now.TimeOfDay

            ViewData("Title") = "Create Transaction Deal"
            ViewBag.Breadcrumb = String.Format("Home > Transaction Deal > Create")
            Dim newTransactionDeal As New TransactionDeal
            With newTransactionDeal
                .AccNum = AccountNumber
                .AccName = AccountName
                .BranchId = BranchId
                .TransactionType = TransactionType
                .DealType = DealType
                .CurrencyDeal = CurrencyDeal
                .DealRate = DealRate
                .AmountDeal = AmountDeal
                .CurrencyCustomer = CurrencyCustomer
                .RateCustomer = RateCustomer
                .AmountCustomer = AmountCustomer
                .DealPeriod = DealPeriod
                .DealDate = RealDealDate
                .EditBy = User.Identity.Name
                .EditDate = DateTime.Now
                .ApproveBy = ""
                .ApproveDate = New DateTime(1900, 1, 1)

                If Session("BranchID") = Session("BranchHO") Then
                    If CheckLimit(AccountNumber, AccountName, TransactionType, DealType, CurrencyDeal, CurrencyCustomer, AmountDeal, DealPeriod, "CREATE") = False Then
                        .DealNumber = ""
                        .Status = "CREATE - PENDING"

                        LogTransaction.WriteLog(User.Identity.Name, "CREATE TRANSACTION DEAL", newTransactionDeal.AccNum & "-" & newTransactionDeal.CurrencyDeal & "-" & newTransactionDeal.AmountDeal, db)
                    Else
                        .DealNumber = GenerateDealNumber()
                        .Status = "ACTIVE"

                        LogTransaction.WriteLog(User.Identity.Name, "CREATE TRANSACTION DEAL", newTransactionDeal.DealNumber, db)
                    End If
                Else
                    .DealNumber = ""
                    .Status = "CREATE - PENDING"

                    LogTransaction.WriteLog(User.Identity.Name, "CREATE TRANSACTION DEAL", newTransactionDeal.AccNum & "-" & newTransactionDeal.CurrencyDeal & "-" & newTransactionDeal.AmountDeal, db)
                End If
                'If CheckLimit(AccountNumber, AccountName, TransactionType, DealType, CurrencyDeal, CurrencyCustomer, AmountDeal, DealPeriod, "CREATE") = False Then
                '    .DealNumber = ""
                '    .Status = "CREATE - PENDING"

                '    LogTransaction.WriteLog(User.Identity.Name, "CREATE TRANSACTION DEAL", newTransactionDeal.AccNum & "-" & newTransactionDeal.CurrencyDeal & "-" & newTransactionDeal.AmountDeal, db)
                'Else
                '    .DealNumber = GenerateDealNumber()
                '    .Status = "ACTIVE"

                '    LogTransaction.WriteLog(User.Identity.Name, "CREATE TRANSACTION DEAL", newTransactionDeal.DealNumber, db)
                'End If
            End With
            db.TransactionDeal.Add(newTransactionDeal)
            db.SaveChanges()

            Return RedirectToAction("Index", "TransactionDeal")
        End If

        Return View("Detail", model)
    End Function

    '
    ' Get: /TransactionDeal/Edit
    <Authorize> _
    Public Function Edit(ByVal id As String) As ActionResult
        Dim model As New TransactionDeal
        Dim db As New FectorEntities
        Session("BranchHO") = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return RedirectToAction("Index", "TransactionDeal")
        End If

        If IsNothing(model.DealNumber) Then
            model = TransactionDeal.GetTransactionDeal(id, db)
        End If

        Dim ExchRateNoIDR = (From a In db.ExchangeRate
                             Where a.CoreCurrId <> "IDR" And a.CurrId <> model.CurrencyDeal
                             Select a).ToList

        Dim ExchRate = (From a In db.ExchangeRate
                        Where a.CurrId <> model.CurrencyCustomer
                        Select a).ToList

        Dim ListCurrNoIDR As New List(Of GetCurrency_View)
        ListCurrNoIDR.Add(New GetCurrency_View With {.CurrId = model.CurrencyDeal, .CurrDisplay = model.CurrencyDeal})
        For i As Integer = 0 To ExchRateNoIDR.Count - 1
            ListCurrNoIDR.Add(New GetCurrency_View With {.CurrId = ExchRateNoIDR.Item(i).CurrId, .CurrDisplay = ExchRateNoIDR.Item(i).CurrencyDisplay})
        Next

        Dim ListCurr As New List(Of GetCurrency_View)
        ListCurr.Add(New GetCurrency_View With {.CurrId = model.CurrencyCustomer, .CurrDisplay = model.CurrencyCustomer})

        For i As Integer = 0 To ExchRate.Count - 1
            ListCurr.Add(New GetCurrency_View With {.CurrId = ExchRate.Item(i).CurrId, .CurrDisplay = ExchRate.Item(i).CurrencyDisplay})
        Next

        ViewData("Title") = "Edit Transaction Deal"
        If model.DealNumber = "" Then
            model.DealNumber = "(AUTO)"
        End If
        ViewBag.Breadcrumb = String.Format("Home > Transaction Deal > Edit")
        ViewBag.DealNumber = New SelectList(db.TransactionDeal, "DealNumber", "DealNumber")
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(ListCurrNoIDR, "CurrId", "CurrDisplay")
        ViewBag.Currency = New SelectList(ListCurr, "CurrId", "CurrDisplay")
        ViewBag.RateTypeOption = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption

        Return View("Detail", model)
    End Function

    '
    ' Post: /TransactionDeal/Edit
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Edit(model As TransactionDeal) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Edit Transaction Deal"
        ViewBag.Breadcrumb = String.Format("Home > Transaction Deal > Edit")
        ViewBag.DealNumber = New SelectList(db.TransactionDeal, "DealNumber", "DealNumber")
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
        ViewBag.RateTypeOption = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption

        If ModelState.IsValid Then
            Dim ID As String = model.ID
            Dim AccountNumber As String = model.AccNum
            Dim AccountName As String = model.AccName
            Dim BranchId As String = ""
            If Session("BranchId") = Session("BranchHO") Then
                BranchId = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1
            Else
                BranchId = Session("BranchId")
            End If
            Dim TransactionType As String = model.TransactionType
            Dim DealType As String = model.DealType
            Dim CurrencyDeal As String = model.CurrencyDeal
            Dim DealRate As Decimal = model.DealRate
            Dim AmountDeal As Decimal = model.AmountDeal
            Dim CurrencyCustomer As String = model.CurrencyCustomer
            Dim RateCustomer As Decimal = model.RateCustomer
            Dim AmountCustomer As Decimal = model.AmountCustomer
            Dim DealPeriod As String = model.DealPeriod
            Dim DealDate As String = model.DealDate

            Dim DayDealDate As String = DealDate.Split(CChar("/"))(1)
            Dim MonthDealDate As String = DealDate.Split(CChar("/"))(0)
            Dim YearDealDate As String = DealDate.Split(CChar("/"))(2)

            Dim RealDealDate As DateTime = CDate(YearDealDate & "-" & MonthDealDate & "-" & DayDealDate) + Now.TimeOfDay

            ViewData("Title") = "Edit Transaction Deal"
            ViewBag.Breadcrumb = String.Format("Home > Transaction Deal > Edit")

            Dim editedTransactionDeal As TransactionDeal = TransactionDeal.GetTransactionDeal(model.ID, db)
            With editedTransactionDeal
                If model.DealNumber <> "(AUTO)" Then
                    .DealNumber = model.DealNumber
                Else
                    .DealNumber = ""
                End If
                .AccNum = AccountNumber
                .AccName = AccountName
                .BranchId = BranchId
                .TransactionType = TransactionType
                .DealType = DealType
                .CurrencyDeal = CurrencyDeal
                .DealRate = DealRate
                .AmountDeal = AmountDeal
                .CurrencyCustomer = CurrencyCustomer
                .RateCustomer = RateCustomer
                .AmountCustomer = AmountCustomer
                .DealPeriod = DealPeriod
                .DealDate = RealDealDate
                .EditBy = User.Identity.Name
                .EditDate = DateTime.Now
                .ApproveBy = ""
                .ApproveDate = New DateTime(1900, 1, 1)

                If Session("BranchID") = Session("BranchHO") Then
                    If CheckLimit(AccountNumber, AccountName, TransactionType, DealType, CurrencyDeal, CurrencyCustomer, AmountDeal, DealPeriod, "EDIT", ID) = False Then
                        .Status = "EDIT - PENDING"
                    Else
                        .Status = "ACTIVE"
                    End If
                Else
                    .Status = "EDIT - PENDING"
                End If

                'If CheckLimit(AccountNumber, AccountName, TransactionType, DealType, CurrencyDeal, CurrencyCustomer, AmountDeal, DealPeriod, "EDIT", ID) = False Then
                '    .Status = "EDIT - PENDING"
                'Else
                '    .Status = "ACTIVE"
                'End If
            End With
            db.Entry(editedTransactionDeal).State = EntityState.Modified
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "EDIT TRANSACTION DEAL", model.DealNumber, db)

            Return RedirectToAction("Index")
        Else
            Return View("Detail", model)
        End If
    End Function

    ' GET: /TransactionDeal/Viewed
    <Authorize> _
    Function Viewed(ByVal id As String) As ActionResult
        Dim model As New TransactionDeal
        Dim db As New FectorEntities
        Session("BranchHO") = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        If IsNothing(model.DealNumber) Then
            model = TransactionDeal.GetTransactionDeal(id, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        Dim ListCurrNoIDR As New List(Of GetCurrency_View)
        ListCurrNoIDR.Add(New GetCurrency_View With {.CurrId = model.CurrencyDeal, .CurrDisplay = model.CurrencyDeal})

        Dim ListCurr As New List(Of GetCurrency_View)
        ListCurr.Add(New GetCurrency_View With {.CurrId = model.CurrencyCustomer, .CurrDisplay = model.CurrencyCustomer})

        ViewData("Title") = "View Transaction Deal"
        If model.DealNumber = "" Then
            model.DealNumber = "(AUTO)"
        End If
        ViewBag.Breadcrumb = String.Format("Home > Transaction Deal > View")
        ViewBag.DealNumber = New SelectList(db.TransactionDeal, "DealNumber", "DealNumber")
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(ListCurrNoIDR, "CurrId", "CurrDisplay")
        ViewBag.Currency = New SelectList(ListCurr, "CurrId", "CurrDisplay")
        ViewBag.RateTypeOption = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption

        Return View("Detail", model)
    End Function

    ' POST: /TransactionDeal/Viewed
    <Authorize> _
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    Function Viewed(model As TransactionDeal) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Process Transaction Deal"
        ViewBag.Breadcrumb = String.Format("Home > Transaction Deal > Viewed")
        ViewBag.DealNumber = New SelectList(db.TransactionDeal, "DealNumber", "DealNumber")
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
        ViewBag.RateTypeOption = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption

        Dim TransDeal = TransactionDeal.GetTransactionDeal(model.ID, db)

        TransDeal.ApproveBy = User.Identity.Name
        TransDeal.ApproveDate = Now
        TransDeal.Status = "DELETE - PENDING"

        db.Entry(TransDeal).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "DELETE TRANSACTION DEAL", model.DealNumber, db)

        Return RedirectToAction("Index")
    End Function

    ' GET: /TransactionDeal/Process
    <Authorize> _
    Function Process(ByVal id As String) As ActionResult
        Dim model As New TransactionDeal
        Dim db As New FectorEntities
        Session("BranchHO") = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        If IsNothing(model.DealNumber) Then
            model = TransactionDeal.GetTransactionDeal(id, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        Dim ListCurrNoIDR As New List(Of GetCurrency_View)
        ListCurrNoIDR.Add(New GetCurrency_View With {.CurrId = model.CurrencyDeal, .CurrDisplay = model.CurrencyDeal})

        Dim ListCurr As New List(Of GetCurrency_View)
        ListCurr.Add(New GetCurrency_View With {.CurrId = model.CurrencyCustomer, .CurrDisplay = model.CurrencyCustomer})

        ViewData("Title") = "View Transaction Deal"
        If model.DealNumber = "" Then
            model.DealNumber = "(AUTO)"
        End If
        ViewBag.Breadcrumb = String.Format("Home > Transaction Deal > View")
        ViewBag.DealNumber = New SelectList(db.TransactionDeal, "DealNumber", "DealNumber")
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(ListCurrNoIDR, "CurrId", "CurrDisplay")
        ViewBag.Currency = New SelectList(ListCurr, "CurrId", "CurrDisplay")
        ViewBag.RateTypeOption = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption

        Return View("Detail", model)
    End Function

    ' POST: /TransactionDeal/Process
    <Authorize> _
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    Function Process(model As TransactionDeal, approvalButton As String) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Process Transaction Deal"
        ViewBag.Breadcrumb = String.Format("Home > Transaction Deal > Approval > Process")
        ViewBag.DealNumber = New SelectList(db.TransactionDeal, "DealNumber", "DealNumber")
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
        ViewBag.RateTypeOption = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption

        Dim TransDeal = TransactionDeal.GetTransactionDeal(model.ID, db)

        TransDeal.ApproveBy = User.Identity.Name
        TransDeal.ApproveDate = Now
        Dim Status As String = ""
        Select Case TransDeal.Status
            Case "EDIT - PENDING", "CREATE - PENDING"
                If approvalButton = "Approve" Then
                    If TransDeal.DealNumber = "" Then
                        TransDeal.DealNumber = GenerateDealNumber()
                    End If

                    TransDeal.Status = "ACTIVE"
                    Status = "APPROVE"
                Else
                    TransDeal.Status = "REJECTED"
                    Status = "REJECT"
                End If
            Case "DELETE - PENDING"
                If approvalButton = "Approve" Then
                    TransDeal.Status = "INACTIVE"
                    Status = "DELETE"
                Else
                    TransDeal.Status = "ACTIVE"
                    Status = "ACTIVE"
                End If
        End Select

        db.Entry(TransDeal).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, Status & " TRANSACTION DEAL", model.DealNumber, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' Post:  /TransactionDeal/JsOnGetRate
    <HttpPost> _
    Public Function JsOnGetRate(tranType As String, rateType As String, fromCurr As String, toCurr As String) As ActionResult
        Dim db As New FectorEntities
        Dim exchangeRate As Decimal = 0
        Dim baserate = (From ex In db.ExchangeRate
                       Where ex.CurrId = fromCurr
                       Select New With {.BNBuyRate = ex.BNBuyRate, .BNSellRate = ex.BNSellRate, _
                                       .TTSellRate = ex.TTSellRate, .TTBuyRate = ex.TTBuyRate}).FirstOrDefault

        If IsNothing(baserate) Then
            Return Json(New With {.exchangeRate = exchangeRate})
        End If

        Dim counterrate = (From ex In db.ExchangeRate
                           Where ex.CurrId = toCurr
                           Select New With {.BNBuyRate = ex.BNBuyRate, .BNSellRate = ex.BNSellRate, _
                                       .TTSellRate = ex.TTSellRate, .TTBuyRate = ex.TTBuyRate}).FirstOrDefault

        If IsNothing(baserate) Then
            Return Json(New With {.exchangeRate = exchangeRate})
        End If

        If tranType = "Sell" Then
            If rateType = "TT" Then
                exchangeRate = CDec(baserate.TTSellRate / counterrate.TTBuyRate).ToString("N2")
            Else
                exchangeRate = CDec(baserate.BNSellRate / counterrate.BNBuyRate).ToString("N2")
            End If
        Else
            If rateType = "TT" Then
                exchangeRate = CDec(baserate.TTBuyRate / counterrate.TTSellRate).ToString("N2")
            Else
                exchangeRate = CDec(baserate.BNBuyRate / counterrate.BNSellRate).ToString("N2")
            End If
        End If

        Return Json(New With {.exchangeRate = exchangeRate})
    End Function

    '
    ' Post:  /TransactionDeal/JsOnGetValueDate
    <HttpPost> _
    Public Function JsOnGetValueDate(valuetype As String, basecurrency As String) As ActionResult
        Dim db As New FectorEntities
        Dim exchangeRate As Decimal = 0
        Dim valDate As Date

        If valuetype = "TOD" Then
            Dim tempDate As DateTime = Now
            valDate = getWorkingDate(tempDate, basecurrency)
        ElseIf valuetype = "TOM" Then
            Dim tempDate As DateTime = Now.AddDays(1)
            valDate = getWorkingDate(tempDate, basecurrency)
        ElseIf valuetype = "SPOT" Then
            Dim tempDate As DateTime = Now.AddDays(2)
            valDate = getWorkingDate(tempDate, basecurrency, "SPOT")
        ElseIf valuetype = "FWD" Then
            Dim tempDate As DateTime = Now
            valDate = getWorkingDate(tempDate, basecurrency)
        End If

        Return Json(New With {.valueDate = valDate.ToString("dd-MM-yyyy")})
    End Function

    Private Function getWorkingDate(ByVal dtInitial As DateTime, ByVal BaseCurrency As String, Optional ValueType As String = "") As DateTime
        If isHoliday(dtInitial, BaseCurrency) Then
            If ValueType = "SPOT" Then
                Return getWorkingDate(dtInitial.AddDays(2), BaseCurrency)
            Else
                Return getWorkingDate(dtInitial.AddDays(1), BaseCurrency)
            End If

        Else
            Return dtInitial
        End If
    End Function

    Private Function isHoliday(ByVal dtCheck As DateTime, ByVal BaseCurrency As String) As Boolean
        If dtCheck.DayOfWeek = DayOfWeek.Sunday Or dtCheck.DayOfWeek = DayOfWeek.Saturday Then
            Return True
        End If

        Dim db As New FectorEntities
        Dim temp = From hl In db.Holidays
                    Group Join no In db.MappingNostro On hl.Nostro Equals no.BIC Into Group From p In Group.DefaultIfEmpty
                    Where dtCheck.Date <= hl.EndDate And dtCheck.Date >= hl.StartDate
                    Select New With {hl.HolidayDesc}

        Dim holiday = (From hl In db.Holidays
                       Group Join no In db.MappingNostro On hl.Nostro Equals no.BIC Into Group From p In Group.DefaultIfEmpty
                        Where dtCheck.Date <= hl.EndDate And dtCheck.Date >= hl.StartDate And p.CurrID = BaseCurrency
                        Select New With {hl.HolidayDesc}).FirstOrDefault

        If IsNothing(holiday) Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Function CheckLimit(AccountNumber As String, AccountName As String, TransactionType As String, DealType As String, BaseCurrency As String, CounterCurrency As String, BaseAmount As Decimal, DealPeriod As String, Mode As String, Optional ID As String = "") As Boolean
        Dim db As New FectorEntities

        'Start Limit User Who Login and Convert it into IDR
        Dim DataUser = (From u In db.Users
                    Where u.UserId = User.Identity.Name
                    Select u).ToList

        Dim LimitCurrencyUser As String = DataUser.Item(0).TransactionLimitCurrency
        Dim LimitUser As Decimal = DataUser.Item(0).TransactionLimit

        Dim DataRate = (From e In db.ExchangeRate
                        Where e.CurrId = LimitCurrencyUser
                        Select e).ToList

        If DataRate.Count > 0 Then
            'If TransactionType = "Sell" Then
            '    If DealType = "TT" Then
            '        LimitUser = CDec(LimitUser * DataRate.Item(0).TTSellRate).ToString("N2")
            '    Else
            '        LimitUser = CDec(LimitUser * DataRate.Item(0).BNSellRate).ToString("N2")
            '    End If
            'Else
            '    If DealType = "TT" Then
            '        LimitUser = CDec(LimitUser * DataRate.Item(0).TTBuyRate).ToString("N2")
            '    Else
            '        LimitUser = CDec(LimitUser * DataRate.Item(0).BNBuyRate).ToString("N2")
            '    End If
            'End If
            LimitUser = CDec(LimitUser * DataRate.Item(0).ClosingRate).ToString("N2")
        End If
        'End Limit User Who Login and Convert it into IDR

        'Start SUM Deal Transaction Today (IDR) with That User, but status not in 'REJECTED', 'INACTIVE', 'CLOSED'
        Dim SUMDealUserToday As Nullable(Of Decimal) = 0

        If Mode = "CREATE" Then
            SUMDealUserToday = Aggregate s In db.TransactionDeal
                                Where (s.Status <> "REJECTED" And s.Status <> "INACTIVE" And s.Status <> "CLOSED") _
                                And s.EditBy = User.Identity.Name _
                                And s.EditDate.Year = Now.Year And s.EditDate.Month = Now.Month And s.EditDate.Day = Now.Day _
                                Into Sum(s.DealRate * s.AmountDeal)
        ElseIf Mode = "EDIT" Then
            SUMDealUserToday = Aggregate s In db.TransactionDeal
                                Where (s.Status <> "REJECTED" And s.Status <> "INACTIVE" And s.Status <> "CLOSED") _
                                And s.EditBy = User.Identity.Name _
                                And s.EditDate.Year = Now.Year And s.EditDate.Month = Now.Month And s.EditDate.Day = Now.Day _
                                And s.ID <> ID
                                Into Sum(s.DealRate * s.AmountDeal)
        End If

        If SUMDealUserToday Is Nothing Then
            SUMDealUserToday = 0
        Else
            DataRate = (From e In db.ExchangeRate
                    Where e.CurrId = CounterCurrency
                    Select e).ToList

            If DataRate.Count > 0 Then
                'If TransactionType = "Sell" Then
                '    If DealType = "TT" Then
                '        SUMDealUserToday = CDec(SUMDealUserToday * DataRate.Item(0).TTSellRate).ToString("N2")
                '    Else
                '        SUMDealUserToday = CDec(SUMDealUserToday * DataRate.Item(0).BNSellRate).ToString("N2")
                '    End If
                'Else
                '    If DealType = "TT" Then
                '        SUMDealUserToday = CDec(SUMDealUserToday * DataRate.Item(0).TTBuyRate).ToString("N2")
                '    Else
                '        SUMDealUserToday = CDec(SUMDealUserToday * DataRate.Item(0).BNBuyRate).ToString("N2")
                '    End If
                'End If
                SUMDealUserToday = CDec(SUMDealUserToday * DataRate.Item(0).ClosingRate).ToString("N2")
            End If
        End If
        'End SUM Deal Transaction Today (IDR) with That User but status not in 'REJECTED', 'INACTIVE', 'CLOSED'

        'Start Get Limit Customer and Convert it into IDR
        Dim LimitCurrencyCustomer As String
        Dim LimitCustomer As Nullable(Of Decimal) = 0

        Dim DataCustomer = (From a In db.Accounts
                             Where a.Status = "ACTIVE" And (a.AccNo = AccountNumber Or a.Name = AccountName)
                             Select New AllAccount With {.AccNo = a.AccNo, .AccName = a.Name, _
                                                         .TODLimitCurrency = a.TODLimitcurrency, _
                                                         .TODLimit = a.TODLimit, _
                                                         .TOMLimitCurrency = a.TOMLimitcurrency, _
                                                         .TOMLimit = a.TOMLimit, _
                                                         .SPOTLimitCurrency = a.SPOTLimitcurrency, _
                                                         .SPOTLimit = a.SPOTLimit, _
                                                         .AllLimitCurrency = a.ALLLimitcurrency, _
                                                         .AllLimit = a.ALLLimit}).Union(
                             From o In db.OtherAccounts
                             Where o.Status = "ACTIVE" And (o.AccNo = AccountNumber Or o.Name = AccountName)
                              Select New AllAccount With {.AccNo = o.AccNo, .AccName = o.Name, _
                                                         .TODLimitCurrency = o.TODLimitcurrency, _
                                                         .TODLimit = o.TODLimit, _
                                                         .TOMLimitCurrency = o.TOMLimitcurrency, _
                                                         .TOMLimit = o.TOMLimit, _
                                                         .SPOTLimitCurrency = o.SPOTLimitcurrency, _
                                                         .SPOTLimit = o.SPOTLimit, _
                                                         .AllLimitCurrency = o.ALLLimitcurrency, _
                                                         .AllLimit = o.ALLLimit}).ToList

        If DataCustomer.Count > 0 Then
            If DealPeriod = "TOD" Then
                LimitCurrencyCustomer = DataCustomer.Item(0).TODLimitCurrency
                LimitCustomer = DataCustomer.Item(0).TODLimit
            ElseIf DealPeriod = "TOM" Then
                LimitCurrencyCustomer = DataCustomer.Item(0).TOMLimitCurrency
                LimitCustomer = DataCustomer.Item(0).TOMLimit
            Else
                LimitCurrencyCustomer = DataCustomer.Item(0).SPOTLimitCurrency
                LimitCustomer = DataCustomer.Item(0).SPOTLimit
            End If

            DataRate = (From e In db.ExchangeRate
                   Where e.CurrId = LimitCurrencyCustomer
                   Select e).ToList
        End If

        If DataRate.Count > 0 Then
            'If TransactionType = "Sell" Then
            '    If DealType = "TT" Then
            '        LimitCustomer = CDec(LimitCustomer * DataRate.Item(0).TTSellRate).ToString("N2")
            '    Else
            '        LimitCustomer = CDec(LimitCustomer * DataRate.Item(0).BNSellRate).ToString("N2")
            '    End If
            'Else
            '    If DealType = "TT" Then
            '        LimitCustomer = CDec(LimitCustomer * DataRate.Item(0).TTBuyRate).ToString("N2")
            '    Else
            '        LimitCustomer = CDec(LimitCustomer * DataRate.Item(0).BNBuyRate).ToString("N2")
            '    End If
            'End If
            LimitCustomer = CDec(LimitCustomer * DataRate.Item(0).ClosingRate).ToString("N2")
        End If
        'End Get Limit Customer and Convert it into IDR

        'Start Limit Combination Transaction Customer and Convert it into IDR
        Dim LimitCombinationCustomer As Nullable(Of Decimal) = 0

        LimitCurrencyCustomer = DataCustomer.Item(0).AllLimitCurrency
        LimitCombinationCustomer = DataCustomer.Item(0).AllLimit

        DataRate = (From e In db.ExchangeRate
                    Where e.CurrId = LimitCurrencyCustomer
                    Select e).ToList

        If DataRate.Count > 0 Then
            'If TransactionType = "Sell" Then
            '    If DealType = "TT" Then
            '        LimitCombinationCustomer = CDec(LimitCombinationCustomer * DataRate.Item(0).TTSellRate).ToString("N2")
            '    Else
            '        LimitCombinationCustomer = CDec(LimitCombinationCustomer * DataRate.Item(0).BNSellRate).ToString("N2")
            '    End If
            'Else
            '    If DealType = "TT" Then
            '        LimitCombinationCustomer = CDec(LimitCombinationCustomer * DataRate.Item(0).TTBuyRate).ToString("N2")
            '    Else
            '        LimitCombinationCustomer = CDec(LimitCombinationCustomer * DataRate.Item(0).BNBuyRate).ToString("N2")
            '    End If
            'End If
            LimitCombinationCustomer = CDec(LimitCombinationCustomer * DataRate.Item(0).ClosingRate).ToString("N2")
        End If
        'End Limit Combination Transaction Customer and Convert it into IDR

        'Start SUM Deal Transaction Today (IDR) with That Customer, but status not in 'REJECTED', 'INACTIVE', 'CLOSED'
        Dim SUMDealCustomerToday As Nullable(Of Decimal) = 0
        Dim tempDealPeriod As String

        If DealPeriod = "TOD" Then
            tempDealPeriod = "TOD"
        ElseIf DealPeriod = "TOM" Then
            tempDealPeriod = "TOM"
        ElseIf DealPeriod = "SPOT" Then
            tempDealPeriod = "SPOT"
        Else
            tempDealPeriod = "FWD"
        End If

        If Mode = "CREATE" Then
            SUMDealCustomerToday = Aggregate s In db.TransactionDeal
                            Where (s.Status <> "REJECTED" And s.Status <> "CLOSED" And s.Status <> "INACTIVE") _
                            And s.DealPeriod = tempDealPeriod _
                            And s.AccName = AccountName _
                            And s.EditDate.Year = Now.Year And s.EditDate.Month = Now.Month And s.EditDate.Day = Now.Day
                            Into Sum(s.DealRate * s.AmountDeal)

        ElseIf Mode = "EDIT" Then
            SUMDealCustomerToday = Aggregate s In db.TransactionDeal
                                Where (s.Status <> "REJECTED" And s.Status <> "CLOSED" And s.Status <> "INACTIVE") _
                                And s.DealPeriod = tempDealPeriod _
                                And s.AccName = AccountName _
                                And s.EditDate.Year = Now.Year And s.EditDate.Month = Now.Month And s.EditDate.Day = Now.Day _
                                And s.ID <> ID
                                Into Sum(s.DealRate * s.AmountDeal)
        End If

        If SUMDealCustomerToday Is Nothing Then
            SUMDealCustomerToday = 0
        Else
            DataRate = (From e In db.ExchangeRate
                    Where e.CurrId = CounterCurrency
                    Select e).ToList

            If DataRate.Count > 0 Then
                'If TransactionType = "Sell" Then
                '    If DealType = "TT" Then
                '        SUMDealCustomerToday = CDec(SUMDealCustomerToday * DataRate.Item(0).TTSellRate).ToString("N2")
                '    Else
                '        SUMDealCustomerToday = CDec(SUMDealCustomerToday * DataRate.Item(0).BNSellRate).ToString("N2")
                '    End If
                'Else
                '    If DealType = "TT" Then
                '        SUMDealCustomerToday = CDec(SUMDealCustomerToday * DataRate.Item(0).TTBuyRate).ToString("N2")
                '    Else
                '        SUMDealCustomerToday = CDec(SUMDealCustomerToday * DataRate.Item(0).BNBuyRate).ToString("N2")
                '    End If
                'End If
                SUMDealCustomerToday = CDec(SUMDealCustomerToday * DataRate.Item(0).ClosingRate).ToString("N2")
            End If
        End If
        'End SUM Deal Transaction Today (IDR) with That Customer, but status not in 'REJECTED', 'INACTIVE', 'CLOSED'

        'Start SUM Combination Deal Transaction Today(IDR) with That Customer, but Status not in 'REJECTED', 'INACTIVE', 'CLOSED' 
        Dim SUMDealCustomerCombinationToday As Nullable(Of Decimal) = 0

        If Mode = "CREATE" Then
            SUMDealCustomerCombinationToday = Aggregate s In db.TransactionDeal
                                                Where (s.Status <> "REJECTED" And s.Status <> "CLOSED" And s.Status <> "INACTIVE") _
                                                And s.AccName = AccountName _
                                                And s.EditDate.Year = Now.Year And s.EditDate.Month = Now.Month And s.EditDate.Day = Now.Day
                                                Into Sum(s.DealRate * s.AmountDeal)
        ElseIf Mode = "EDIT" Then
            SUMDealCustomerCombinationToday = Aggregate s In db.TransactionDeal
                                                Where (s.Status <> "REJECTED" And s.Status <> "CLOSED" And s.Status <> "INACTIVE") _
                                                And s.AccName = AccountName _
                                                And s.EditDate.Year = Now.Year And s.EditDate.Month = Now.Month And s.EditDate.Day = Now.Day _
                                                And s.ID <> ID
                                                Into Sum(s.DealRate * s.AmountDeal)
        End If

        If SUMDealCustomerCombinationToday Is Nothing Then
            SUMDealCustomerCombinationToday = 0
        End If
        'End Start SUM Combination Deal Transaction Today(IDR) with That Customer, but Status not in 'REJECTED', 'INACTIVE', 'CLOSED' 

        'Start Calculate Deal Amount Which Customer Used
        DataRate = (From e In db.ExchangeRate
                    Where e.CurrId = BaseCurrency
                    Select e).ToList

        If DataRate.Count > 0 Then
            If TransactionType = "Sell" Then
                If DealType = "TT" Then
                    BaseAmount = CDec(BaseAmount * DataRate.Item(0).TTSellRate).ToString("N2")
                Else
                    BaseAmount = CDec(BaseAmount * DataRate.Item(0).BNSellRate).ToString("N2")
                End If
            Else
                If DealType = "TT" Then
                    BaseAmount = CDec(BaseAmount * DataRate.Item(0).TTBuyRate).ToString("N2")
                Else
                    BaseAmount = CDec(BaseAmount * DataRate.Item(0).BNBuyRate).ToString("N2")
                End If
            End If
        End If
        'End Calculate Deal Amount Which Customer Used

        If DealPeriod = "TOD" Then
            If BaseAmount > LimitUser And LimitUser <> 0 Then
                Return False
            End If
        Else
            If SUMDealCustomerToday + BaseAmount > LimitCustomer And LimitCustomer <> 0 Then
                Return False
            End If
            If SUMDealCustomerCombinationToday + BaseAmount > LimitCombinationCustomer And LimitCombinationCustomer <> 0 Then
                Return False
            End If
            If BaseAmount > LimitUser And LimitUser <> 0 Then
                Return False
            End If
        End If

        'If SUMDealCustomerToday + BaseAmount > LimitCustomer And LimitCustomer <> 0 Then
        '    If DealPeriod = "TOD" Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End If
        'If SUMDealCustomerCombinationToday + BaseAmount > LimitCombinationCustomer And LimitCombinationCustomer <> 0 Then
        '    Return False
        'End If
        'If BaseAmount > LimitUser And LimitUser <> 0 Then
        '    Return False
        'End If
        'If SUMDealUserToday + BaseAmount > LimitUser And LimitUser <> 0 Then
        '    Return False
        'End If

        Return True
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTDealHistoryAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim HeadBranch = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1
        Dim UserBranchID As Decimal = Session("BranchId")
        Dim TransactionDeal As IQueryable(Of TransactionDeal)

        If UserBranchID = HeadBranch Then
            TransactionDeal = From s In db.TransactionDeal.Include("Branch")
                              Order By s.DealNumber
                              Select s
        Else
            TransactionDeal = From s In db.TransactionDeal.Include("Branch")
                              Where ((s.BranchId = UserBranchID Or s.BranchId Is Nothing))
                              Order By s.DealNumber
                              Select s
        End If

        Return ReturnDealDataTable(param, TransactionDeal, "History")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTDealAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim HeadBranch = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1
        Dim UserBranchID As Decimal = Session("BranchId")
        Dim TransactionDeal As IQueryable(Of TransactionDeal)

        'If UserBranchID = HeadBranch Then
        '    TransactionDeal = From s In db.TransactionDeal.Include("Branch")
        '                      Where s.Status <> "FINISHED"
        '                      Select s
        'Else
        '    TransactionDeal = From s In db.TransactionDeal.Include("Branch")
        '                      Where s.Status <> "FINISHED" And ((s.BranchId = UserBranchID Or s.BranchId Is Nothing))
        '                      Select s
        'End If

        TransactionDeal = From s In db.TransactionDeal.Include("Branch")
                              Where s.Status <> "FINISHED" And s.Status <> "INACTIVE"
                              Select s
        Return ReturnDealDataTable(param, TransactionDeal, "Index")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTDealApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim HeadBranch = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1
        Dim UserBranchID As Decimal = Session("BranchId")
        Dim TransactionDeal As IQueryable(Of TransactionDeal)

        'If UserBranchID = HeadBranch Then
        '    TransactionDeal = From s In db.TransactionDeal.Include("Branch")
        '                        Where s.Status.Contains("PENDING") And s.EditBy <> User.Identity.Name
        '                        Select s
        'Else
        '    TransactionDeal = From s In db.TransactionDeal.Include("Branch")
        '                        Where s.Status.Contains("PENDING") And s.EditBy <> User.Identity.Name And ((s.BranchId = UserBranchID Or s.BranchId Is Nothing))
        '                        Select s
        'End If

        TransactionDeal = From s In db.TransactionDeal.Include("Branch")
                                Where s.Status.Contains("PENDING") And s.EditBy <> User.Identity.Name
                                Select s
        Return ReturnDealDataTable(param, TransactionDeal, "Approval")
    End Function

    Private Function ReturnDealDataTable(param As jQueryDataTableParamModel, TransactionDeal As IQueryable(Of TransactionDeal), mode As String) As JsonResult
        Dim totalRecords = TransactionDeal.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim DealNumberSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim AccNumSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        Dim AccNameSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))
        Dim BranchSearch As String = If(IsNothing(Request("sSearch_4")), "", Request("sSearch_4"))
        Dim TransactionTypeSearch As String = If(IsNothing(Request("sSearch_5")), "", Request("sSearch_5"))
        Dim CurrencyDealSearch As String = If(IsNothing(Request("sSearch_6")), "", Request("sSearch_6"))
        Dim DealDateSearch As String = If(IsNothing(Request("sSearch_7")), "", Request("sSearch_7"))
        Dim StatusSearch As String = If(IsNothing(Request("sSearch_8")), "", Request("sSearch_8"))
        TransactionDeal = TransactionDeal.Where(Function(f) f.DealNumber.Contains(DealNumberSearch))
        TransactionDeal = TransactionDeal.Where(Function(f) f.AccNum.Contains(AccNumSearch))
        TransactionDeal = TransactionDeal.Where(Function(f) f.AccName.Contains(AccNameSearch))
        TransactionDeal = TransactionDeal.Where(Function(f) If(f.BranchId Is Nothing, "", f.BranchId).Contains(BranchSearch) Or If(f.Branch.BranchAbbr Is Nothing, "", f.Branch.BranchAbbr).Contains(BranchSearch))
        TransactionDeal = TransactionDeal.Where(Function(f) f.TransactionType.Contains(TransactionTypeSearch) Or f.DealType.Contains(TransactionTypeSearch))
        TransactionDeal = TransactionDeal.Where(Function(f) f.CurrencyDeal.Contains(CurrencyDealSearch) Or f.CurrencyCustomer.Contains(CurrencyDealSearch))
        'TransactionDeal = TransactionDeal.Where(Function(f) f.AmountCustomer.Contains(AmountCustomerSearch))
        If DealDateSearch.Trim.Length > 0 And AppHelper.checkDate(DealDateSearch) Then
            Dim dTDate As Date
            dTDate = AppHelper.dateConvert(DealDateSearch)
            TransactionDeal = TransactionDeal.Where(Function(f) f.DealDate.Day = dTDate.Day And f.DealDate.Month = dTDate.Month And f.DealDate.Year = dTDate.Year)
            'Else
            '    If StatusSearch <> "ALL" Then
            '        If StatusSearch = "ACTIVE" Or StatusSearch = "" Then
            '            If mode <> "History" Then
            '                TransactionDeal = TransactionDeal.Where(Function(f) f.DealDate.Day = Now.Day And f.DealDate.Month = Now.Month And f.DealDate.Year = Now.Year)
            '            End If
            '        End If
            '    End If
        End If
        If StatusSearch = "" Then
            If mode = "Index" Then
                TransactionDeal = TransactionDeal.Where(Function(f) f.Status.Contains("ACTIVE"))
            ElseIf mode = "Approval" Then
                TransactionDeal = TransactionDeal.Where(Function(f) f.Status.Contains("PENDING"))
            End If
        Else
            If StatusSearch <> "ALL" Then
                TransactionDeal = TransactionDeal.Where(Function(f) f.Status = StatusSearch)
            End If
        End If

        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    TransactionDeal = TransactionDeal.OrderBy(Function(f) f.DealNumber)
                Else
                    TransactionDeal = TransactionDeal.OrderByDescending(Function(f) f.DealNumber)
                End If
            Case 1
                If sortOrder = "asc" Then
                    TransactionDeal = TransactionDeal.OrderBy(Function(f) f.AccNum)
                Else
                    TransactionDeal = TransactionDeal.OrderByDescending(Function(f) f.AccNum)
                End If
            Case 2
                If sortOrder = "asc" Then
                    TransactionDeal = TransactionDeal.OrderBy(Function(f) f.AccName)
                Else
                    TransactionDeal = TransactionDeal.OrderByDescending(Function(f) f.AccName)
                End If
            Case 3
                If sortOrder = "asc" Then
                    TransactionDeal = TransactionDeal.OrderBy(Function(f) f.BranchId)
                Else
                    TransactionDeal = TransactionDeal.OrderByDescending(Function(f) f.BranchId)
                End If
            Case 4
                If sortOrder = "asc" Then
                    TransactionDeal = TransactionDeal.OrderBy(Function(f) f.TransactionType)
                Else
                    TransactionDeal = TransactionDeal.OrderByDescending(Function(f) f.TransactionType)
                End If
            Case 5
                If sortOrder = "asc" Then
                    TransactionDeal = TransactionDeal.OrderBy(Function(f) f.CurrencyDeal)
                Else
                    TransactionDeal = TransactionDeal.OrderByDescending(Function(f) f.CurrencyDeal)
                End If
            Case 6
                If sortOrder = "asc" Then
                    TransactionDeal = TransactionDeal.OrderBy(Function(f) f.AmountCustomer)
                Else
                    TransactionDeal = TransactionDeal.OrderByDescending(Function(f) f.AmountCustomer)
                End If
            Case 7
                If sortOrder = "asc" Then
                    TransactionDeal = TransactionDeal.OrderBy(Function(f) f.DealDate)
                Else
                    TransactionDeal = TransactionDeal.OrderByDescending(Function(f) f.DealDate)
                End If
            Case 8
                If sortOrder = "asc" Then
                    TransactionDeal = TransactionDeal.OrderBy(Function(f) f.Status)
                Else
                    TransactionDeal = TransactionDeal.OrderByDescending(Function(f) f.Status)
                End If
        End Select

        TransactionDeal = TransactionDeal.OrderBy(Function(f) f.DealNumber)

        Dim result As New List(Of String())
        For Each data As TransactionDeal In TransactionDeal
            result.Add({data.ID, data.DealNumber, data.AccNum, data.AccName, If(data.Branch Is Nothing, "", data.BranchId & "-" & data.Branch.BranchAbbr), data.TransactionType + " - " + data.DealType, _
                        data.CurrencyDeal + " - " + data.CurrencyCustomer + " - " + CStr(CDec(data.DealRate).ToString("N2")), _
                        CStr(CDec(data.AmountCustomer).ToString("N2")) + " - " + CStr(CDec(data.AmountDeal).ToString("N2")), _
                        data.DealPeriod & "-" & data.DealDate.ToString("dd-MM-yyyy"), data.Status, "", _
                        data.BranchId, data.TransactionType, data.DealType, data.CurrencyDeal, CDec(data.DealRate).ToString("N2"), _
                        CDec(data.AmountDeal).ToString("N2"), data.CurrencyCustomer, CDec(data.AmountCustomer).ToString("N2"), _
                        CDec(data.RateCustomer).ToString("N2"), data.DealPeriod, data.DealPeriod & "-" & (data.DealDate).ToString("dd/MM/yyyy"), data.Status, ""}) 'Hidden Field
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

    Private Class AllAccount
        Public AccNo As String
        Public AccName As String
        Public TODLimitCurrency As String
        Public TODLimit As Nullable(Of Decimal)
        Public TOMLimitCurrency As String
        Public TOMLimit As Nullable(Of Decimal)
        Public SPOTLimitCurrency As String
        Public SPOTLimit As Nullable(Of Decimal)
        Public AllLimitCurrency As String
        Public AllLimit As Nullable(Of Decimal)
        Public Status As String
    End Class

    <Authorize> _
    <HttpGet> _
    Public Function DTAccountAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        If Request("sSearch_0") <> "" Or Request("sSearch_1") <> "" Then
            Dim accounts = ((From s In db.Accounts
                       Where s.Status.Contains("PENDING") = False
                       Select New AllAccount With {.AccNo = s.AccNo, .AccName = s.Name, .TOMLimit = s.TOMLimit, .SPOTLimit = s.SPOTLimit, .Status = s.Status}).Union(
                       From t In db.OtherAccounts
                       Where t.Status.Contains("PENDING") = False
                       Select New AllAccount With {.AccNo = t.AccNo, .AccName = t.Name, .TOMLimit = t.TOMLimit, .SPOTLimit = t.SPOTLimit, .Status = t.Status}))

            Return ReturnAccountDataTable(param, accounts)
        Else
            Return ReturnAccountDataTable(param, Nothing)
        End If
    End Function

    Private Function ReturnAccountDataTable(param As jQueryDataTableParamModel, accounts As IQueryable(Of AllAccount)) As JsonResult
        Dim totalRecords As Integer = 0
        Dim displayRecord As Integer = 0

        If accounts IsNot Nothing Then
            totalRecords = accounts.Count
            displayRecord = accounts.Count
        End If

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim accnoSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim NameSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim statusSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))

        Dim showed As New List(Of AllAccount)

        If accounts IsNot Nothing Then
            accounts = accounts.Where(Function(f) f.AccNo.Contains(accnoSearch))
            accounts = accounts.Where(Function(f) f.AccName.Contains(NameSearch))
            accounts = accounts.Where(Function(f) If(String.IsNullOrEmpty(f.Status), "RAW", f.Status).Contains(statusSearch))

            displayRecord = accounts.Count

            'Detection of sorted column
            Dim sortOrder As String = Request("sSortDir_0")
            Select Case Request("iSortCol_0")
                Case 0
                    If sortOrder = "asc" Then
                        accounts = accounts.OrderBy(Function(f) f.AccNo)
                    Else
                        accounts = accounts.OrderByDescending(Function(f) f.AccNo)
                    End If
                Case 1
                    If sortOrder = "asc" Then
                        accounts = accounts.OrderBy(Function(f) f.AccName)
                    Else
                        accounts = accounts.OrderByDescending(Function(f) f.AccName)
                    End If
                Case 2
                    If sortOrder = "asc" Then
                        accounts = accounts.OrderBy(Function(f) f.Status)
                    Else
                        accounts = accounts.OrderByDescending(Function(f) f.Status)
                    End If
            End Select

            showed = (From acc In accounts
                       Select acc).ToList
        End If
        
        Dim result As New List(Of String())

        For Each acc As AllAccount In showed
            result.Add({acc.AccNo, acc.AccName, IIf(IsNothing(acc.Status), "RAW", acc.Status), acc.TOMLimit, acc.SPOTLimit, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

    ' GET: /TransactionDeal/BrowseAccNum
    <Authorize> _
    Function BrowseAccNum() As ActionResult
        Return PartialView("_AccNumBrowser")
    End Function

    ' GET: /TransactionDeal/BrowseDeal
    <Authorize> _
    Function BrowseDeal() As ActionResult
        Return PartialView("_DealBrowser")
    End Function

    ' GET: /TransactionDeal/ViewHistory
    <Authorize> _
    Function ViewHistory(ByVal id As String) As ActionResult
        Dim model As New TransactionDeal
        Dim db As New FectorEntities
        Session("BranchHO") = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        If IsNothing(model.DealNumber) Then
            model = TransactionDeal.GetTransactionDeal(id, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        Dim ListCurrNoIDR As New List(Of GetCurrency_View)
        ListCurrNoIDR.Add(New GetCurrency_View With {.CurrId = model.CurrencyDeal, .CurrDisplay = model.CurrencyDeal})

        Dim ListCurr As New List(Of GetCurrency_View)
        ListCurr.Add(New GetCurrency_View With {.CurrId = model.CurrencyCustomer, .CurrDisplay = model.CurrencyCustomer})

        ViewData("Title") = "View Transaction Deal"
        If model.DealNumber = "" Then
            model.DealNumber = "(AUTO)"
        End If
        ViewBag.Breadcrumb = String.Format("Home > Transaction Deal > View")
        ViewBag.DealNumber = New SelectList(db.TransactionDeal, "DealNumber", "DealNumber")
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(ListCurrNoIDR, "CurrId", "CurrDisplay")
        ViewBag.Currency = New SelectList(ListCurr, "CurrId", "CurrDisplay")
        ViewBag.RateTypeOption = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption

        Return View("Detail", model)
    End Function
End Class