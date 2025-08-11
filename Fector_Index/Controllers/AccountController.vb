Imports System.Data.Objects.SqlClient
Imports System.Net

Public Class AccountController
    Inherits System.Web.Mvc.Controller

    Dim LHBUOptionItem As IEnumerable(Of SelectListItem) = {New SelectListItem With {.Text = "NO", .Value = False}, _
                                                            New SelectListItem With {.Text = "YES", .Value = True}}

    ' GET: /Account/Approval
    <Authorize> _
    Public Function Approval() As ActionResult
        ViewBag.Breadcrumb = "Home > Account > Approval"
        Return View("Index")
    End Function

    ' GET: /Account/Index
    <Authorize> _
    Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > Account"
        Return View()
    End Function

    ' GET: /Account/Process
    <Authorize> _
    Function Process(ByVal id As String, ByVal cif As String) As ActionResult
        Dim model As New MsAccount
        Dim db As New FectorEntities

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        If IsNothing(model.AccNo) Then
            model = MsAccount.GetAccount(id, cif, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        ViewData("Title") = "Process Account"
        ViewBag.Breadcrumb = "Home > Account > Approval > Process"
        ViewBag.BType = New SelectList(db.BusinessType, "BusinessTypeId", "DisplayValue")
        ViewBag.Status = New SelectList(db.SubjectStatus, "StatusId", "Description")
        ViewBag.CurrencyList = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")
        ViewBag.LHBUOption = LHBUOptionItem

        Return View("Detail", model)
    End Function

    ' POST: /Account/Process
    <Authorize> _
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    Function Process(model As MsAccount, approvalButton As String) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Process Account"
        ViewBag.Breadcrumb = "Home > Account > Approval > Process"
        ViewBag.BType = New SelectList(db.BusinessType, "BusinessTypeId", "DisplayValue")
        ViewBag.Status = New SelectList(db.SubjectStatus, "StatusId", "DisplayValue")
        ViewBag.CurrencyList = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")
        ViewBag.LHBUOption = LHBUOptionItem

        Dim accextension = MsAccountExtension.GetAccountExtension(model.CIF, db)

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

        LogTransaction.WriteLog(User.Identity.Name, Status & " ACCOUNT", model.AccNo & " / " & model.CIF, db)

        Return RedirectToAction("Approval")
    End Function

    

    ' Get: /Account/Edit
    <Authorize> _
    Public Function Edit(ByVal id As String, ByVal cif As String) As ActionResult
        Dim model As New MsAccount
        Dim db As New FectorEntities

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        If IsNothing(model.AccNo) Then
            model = MsAccount.GetAccount(id, cif, db)

            Dim account = (From a In db.Accounts
                            Where a.AccNo = id And a.CIF = cif
                            Select a).SingleOrDefault

            If account IsNot Nothing Then
                Dim MsKatPelakuDesc As String = IIf(account.MsKategoriLLD Is Nothing, "", account.MsKategoriLLD)

                If MsKatPelakuDesc.ToUpper = "PERUSAHAAN" Then
                    model.BICode = "000000000999979"
                Else
                    model.BICode = "000000000999989"
                End If
            End If
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        ViewData("Title") = "Edit Account"
        ViewBag.Breadcrumb = "Home > Account > Edit"
        ViewBag.BType = New SelectList(db.BusinessType.Where(Function(f) f.Status = "ACTIVE"), "BusinessTypeId", "DisplayValue")
        ViewBag.Status = New SelectList(db.SubjectStatus, "StatusId", "DisplayValue")
        ViewBag.CurrencyList = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")
        ViewBag.LHBUOption = LHBUOptionItem

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
    Public Function Edit(model As MsAccount) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Edit Account"
        ViewBag.Breadcrumb = "Home > Account > Edit"
        ViewBag.BType = New SelectList(db.BusinessType.Where(Function(f) f.Status = "ACTIVE"), "BusinessTypeId", "DisplayValue")
        ViewBag.Status = New SelectList(db.SubjectStatus, "StatusId", "DisplayValue")
        ViewBag.CurrencyList = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")
        ViewBag.LHBUOption = LHBUOptionItem

        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim CIFNumber As String = model.CIF
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

            Dim accountextension = MsAccountExtension.GetAccountExtension(CIFNumber, db)
            If Not IsNothing(accountextension) Then
                db.AccountsExtension.Remove(accountextension)
                db.SaveChanges()
            End If

            Dim newaccountextension As New MsAccountExtension
            With newaccountextension
                .AccNo = CIFNumber
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
            db.AccountsExtension.Add(newaccountextension)
            db.SaveChanges()

            Dim accountlimit = MsAccountLimit.GetAccountLimit(CIFNumber, db)
            If Not IsNothing(accountlimit) Then
                db.AccountsLimit.Remove(accountlimit)
                db.SaveChanges()
            End If

            Dim newaccountlimit As New MsAccountLimit
            With newaccountlimit
                .AccNo = CIFNumber
                .TODLimitcurrency = TODLimitCurrency
                .TOMLimitcurrency = TOMLimitCurrency
                .SPOTLimitcurrency = SPOTLimitCurrency
                .ALLLimitcurrency = ALLLimitCurrency
                .TODLimit = TODLimit
                .TOMLimit = TOMLimit
                .SPOTLimit = SPOTLimit
                .ALLLimit = ALLLimit
            End With
            db.AccountsLimit.Add(newaccountlimit)
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "EDIT ACCOUNT", AccNumber & " / " & CIFNumber, db)

            Return RedirectToAction("Index")
        Else
            Return View("Detail", model)
        End If
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTAccountAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        If Request("sSearch_0") <> "" Or Request("sSearch_1") <> "" Or Request("sSearch_2") <> "" Or Request("sSearch_3") <> "" Then
            Dim accounts = From s In db.Accounts Select s

            Return ReturnAccountDataTable(param, accounts)
        Else
            Return ReturnAccountDataTable(param, Nothing)
        End If
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTAccountApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim accounts = From s In db.Accounts
                       Where s.Status.Contains("PENDING") And s.EditBy <> User.Identity.Name
                       Select s
        Return ReturnAccountDataTable(param, accounts)
    End Function

    Private Function ReturnAccountDataTable(param As jQueryDataTableParamModel, accounts As IQueryable(Of MsAccount)) As JsonResult
        Dim totalRecords As Integer = 0

        If accounts IsNot Nothing Then
            totalRecords = accounts.Count
        End If

        Dim displayRecord As Integer = 0

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim accnoSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim CIFSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim NameSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        Dim statusSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))

        Dim showed As New List(Of MsAccount)

        If accounts IsNot Nothing Then
            accounts = accounts.Where(Function(f) f.AccNo.Contains(accnoSearch))
            accounts = accounts.Where(Function(f) If(f.CIF.HasValue, SqlFunctions.StringConvert(f.CIF), "").Contains(CIFSearch))
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
                        accounts = accounts.OrderBy(Function(f) f.CIF)
                    Else
                        accounts = accounts.OrderByDescending(Function(f) f.CIF)
                    End If
                Case 2
                    If sortOrder = "asc" Then
                        accounts = accounts.OrderBy(Function(f) f.Name)
                    Else
                        accounts = accounts.OrderByDescending(Function(f) f.Name)
                    End If
                Case 3
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

        For Each acc As MsAccount In showed
            result.Add({acc.AccNo, IIf(acc.CIF.HasValue, acc.CIF, ""), acc.Name, IIf(IsNothing(acc.Status), "RAW", acc.Status), ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = displayRecord,
                      .aaData = result},
                      JsonRequestBehavior.AllowGet)
    End Function

    ' GET: /Account/BrowseBICode
    <Authorize> _
    Function BrowseBICode(ByVal name As String, ByVal selectedstatus As String) As ActionResult
        ViewBag.Name = name
        ViewBag.SelectedStatus = selectedstatus
        Return PartialView("_BICodeBrowser")
    End Function

End Class