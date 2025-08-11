Imports System.Data.Objects.SqlClient
Imports System.Net

Public Class OtherAccountController
    Inherits System.Web.Mvc.Controller

    ' GET: /OtherAccount/Approval
    <Authorize> _
    Public Function Approval() As ActionResult
        ViewBag.Breadcrumb = "Home > Other Account > Approval"
        Return View("Index")
    End Function

    ' GET: /OtherAccount/Process
    <Authorize> _
    Function Process(ByVal id As String) As ActionResult
        Dim model As New MsOtherAccount
        Dim db As New FectorEntities

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        If IsNothing(model.AccNo) Then
            model = MsOtherAccount.GetOtherAccount(id, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        ViewData("Title") = "Process Other Account"
        ViewBag.Breadcrumb = "Home > Other Account > Approval > Process"
        ViewBag.BType = New SelectList(db.BusinessType, "BusinessTypeId", "DisplayValue")
        ViewBag.Status = New SelectList(db.SubjectStatus, "StatusId", "DisplayValue")
        ViewBag.CurrencyList = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")
        Dim item As IEnumerable(Of SelectListItem) = {New SelectListItem With {.Text = "NO", .Value = False}, _
                                                      New SelectListItem With {.Text = "YES", .Value = True}}
        ViewBag.LHBUOption = item

        Return View("Detail", model)
    End Function

    ' POST: /OtherAccount/Process
    <Authorize> _
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    Function Process(model As MsOtherAccount, approvalButton As String) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Process Other Account"
        ViewBag.Breadcrumb = "Home > Other Account > Approval > Process"
        ViewBag.BType = New SelectList(db.BusinessType, "BusinessTypeId", "DisplayValue")
        ViewBag.Status = New SelectList(db.SubjectStatus, "StatusId", "DisplayValue")
        ViewBag.CurrencyList = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")
        Dim item As IEnumerable(Of SelectListItem) = {New SelectListItem With {.Text = "NO", .Value = False}, _
                                                      New SelectListItem With {.Text = "YES", .Value = True}}
        ViewBag.LHBUOption = item

        Dim accextension = MsOtherAccountExtension.GetOtherAccountExtension(model.AccNo, db)

        accextension.ApproveBy = User.Identity.Name
        accextension.ApproveDate = Now
        Dim Status As String = ""
        If approvalButton = "Approve" Then
            accextension.Status = "ACTIVE"
            Status = "APPROVE"
        Else
            accextension.Status = "REJECTED"
            Status = "REJECT"
        End If

        db.Entry(accextension).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, Status & " OTH ACC", model.AccNo, db)

        Return RedirectToAction("Approval")
    End Function

    ' GET: /Account/Index
    <Authorize> _
    Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > Other Account"
        Return View()
    End Function

    ' Get: /Account/Edit
    <Authorize> _
    Public Function Edit(ByVal id As String) As ActionResult
        Dim model As New MsOtherAccount
        Dim db As New FectorEntities

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        If IsNothing(model.AccNo) Then
            model = MsOtherAccount.GetOtherAccount(id, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        ViewData("Title") = "Edit Account"
        ViewBag.Breadcrumb = "Home</a> > Other Account > Edit"
        ViewBag.BType = New SelectList(db.BusinessType, "BusinessTypeId", "DisplayValue")
        ViewBag.Status = New SelectList(db.SubjectStatus, "StatusId", "DisplayValue")
        ViewBag.CurrencyList = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")
        Dim item As IEnumerable(Of SelectListItem) = {New SelectListItem With {.Text = "NO", .Value = False}, _
                                                      New SelectListItem With {.Text = "YES", .Value = True}}
        ViewBag.LHBUOption = item

        If IsNothing(model.Status) Then
            model.flagNonLHBU = False
            model.ALLLimit = 0
            model.SPOTLimit = 0
            model.TOMLimit = 0
            model.TODLimit = 0
            model.ALLLimitcurrency = "USD"
            model.SPOTLimitcurrency = "USD"
            model.TOMLimitcurrency = "USD"
            model.TODLimitcurrency = "USD"
        End If

        Return View("Detail", model)
    End Function

    '
    ' Post: /Account/Edit
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Edit(model As MsOtherAccount) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Edit Other Account"
        ViewBag.Breadcrumb = "Home > Other Account > Edit"
        ViewBag.BType = New SelectList(db.BusinessType, "BusinessTypeId", "DisplayValue")
        ViewBag.Status = New SelectList(db.SubjectStatus, "StatusId", "DisplayValue")
        ViewBag.CurrencyList = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")
        Dim item As IEnumerable(Of SelectListItem) = {New SelectListItem With {.Text = "NO", .Value = False}, _
                                                      New SelectListItem With {.Text = "YES", .Value = True}}
        ViewBag.LHBUOption = item

        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim AccNumber As String = model.AccNo
            Dim BusinessTypeId As String = model.BusinessTypeId
            Dim FlagNonLHBU As Boolean = model.flagNonLHBU
            Dim StatusId As String = model.SubjectStatusId
            Dim BICode As String = model.BICode

            Dim TODLimitCurrency As String = model.TODLimitcurrency
            Dim TOMLimitCurrency As String = model.TOMLimitcurrency
            Dim SPOTLimitCurrency As String = model.SPOTLimitcurrency
            Dim ALLLimitCurrency As String = model.ALLLimitcurrency
            Dim TODLimit As Decimal = model.TODLimit
            Dim TOMLimit As Decimal = model.TOMLimit
            Dim SPOTLimit As Decimal = model.SPOTLimit
            Dim ALLLimit As Decimal = model.ALLLimit

            Dim accountextension = MsOtherAccountExtension.GetOtherAccountExtension(AccNumber, db)
            If Not IsNothing(accountextension) Then
                db.OtherAccountsExtension.Remove(accountextension)
                db.SaveChanges()
            End If

            Dim newaccountextension As New MsOtherAccountExtension
            With newaccountextension
                .AccNo = AccNumber
                .BICode = BICode
                .BusinessTypeId = BusinessTypeId
                .flagNonLHBU = FlagNonLHBU
                .SubjectStatusId = StatusId
                .EditBy = User.Identity.Name
                .EditDate = Now
                .ApproveBy = ""
                .ApproveDate = New Date(1900, 1, 1)
                .Status = "EDIT - PENDING"
            End With
            db.OtherAccountsExtension.Add(newaccountextension)
            db.SaveChanges()

            Dim accountlimit = MsOtherAccountLimit.GetOtherAccountLimit(AccNumber, db)
            If Not IsNothing(accountlimit) Then
                db.OtherAccountsLimit.Remove(accountlimit)
                db.SaveChanges()
            End If

            Dim newaccountlimit As New MsOtherAccountLimit
            With newaccountlimit
                .AccNo = AccNumber
                .TODLimitcurrency = TODLimitCurrency
                .TOMLimitcurrency = TOMLimitCurrency
                .SPOTLimitcurrency = SPOTLimitCurrency
                .ALLLimitcurrency = ALLLimitCurrency
                .TODLimit = TODLimit
                .TOMLimit = TOMLimit
                .SPOTLimit = SPOTLimit
                .ALLLimit = ALLLimit
            End With
            db.OtherAccountsLimit.Add(newaccountlimit)
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "EDIT OTH ACC", AccNumber, db)

            Return RedirectToAction("Index")
        Else
            Return View("Detail", model)
        End If
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTOtherAccountAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        If Request("sSearch_0") <> "" Or Request("sSearch_1") <> "" Or Request("sSearch_2") <> "" Or Request("sSearch_3") <> "" Then
            Dim accounts = From s In db.OtherAccounts Select s

            Return ReturnOtherAccountDataTable(param, accounts)
        Else
            Return ReturnOtherAccountDataTable(param, Nothing)
        End If
        
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTOtherAccountApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim accounts = From s In db.OtherAccounts
                       Where s.Status.Contains("PENDING") And s.EditBy <> User.Identity.Name
                       Select s
        Return ReturnOtherAccountDataTable(param, accounts)
    End Function

    Private Function ReturnOtherAccountDataTable(param As jQueryDataTableParamModel, accounts As IQueryable(Of MsOtherAccount)) As JsonResult
        Dim totalRecords As Integer = 0

        If accounts IsNot Nothing Then
            totalRecords = accounts.Count
        End If

        Dim displayRecord As Integer = 0

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim accnoSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim NameSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim statusSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))

        Dim showed As New List(Of MsOtherAccount)

        If accounts IsNot Nothing Then
            accounts = accounts.Where(Function(f) f.AccNo.Contains(accnoSearch))
            accounts = accounts.Where(Function(f) f.Name.Contains(NameSearch))
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
                        accounts = accounts.OrderBy(Function(f) f.Name)
                    Else
                        accounts = accounts.OrderByDescending(Function(f) f.Name)
                    End If
                Case 2
                    If sortOrder = "asc" Then
                        accounts = accounts.OrderBy(Function(f) f.Status)
                    Else
                        accounts = accounts.OrderByDescending(Function(f) f.Status)
                    End If
            End Select

            showed = (From acc In accounts.Skip(iDisplayStart).Take(iDisplayLength)
                       Select acc).ToList
        End If

        Dim result As New List(Of String())

        For Each acc As MsOtherAccount In showed
            result.Add({acc.AccNo, acc.Name, IIf(IsNothing(acc.Status), "RAW", acc.Status), ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = displayRecord,
                      .aaData = result},
                      JsonRequestBehavior.AllowGet)
    End Function

    ' GET: /OtherAccount/BrowseBICode
    <Authorize> _
    Function BrowseBICode(ByVal name As String, ByVal selectedstatus As String) As ActionResult
        ViewBag.Name = name
        ViewBag.SelectedStatus = selectedstatus
        Return PartialView("_BICodeBrowser")
    End Function
End Class