Imports System.Net
Imports System.Data.Objects.SqlClient

Public Class ExtractSettingNonTransactionController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /ExtractSettingNonTransaction

    Public Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > Extract Setting Non Transaction"
        Return View("Index")
    End Function

    ' GET: /ExtractSettingNonTransaction/Approval
    <Authorize> _
    Public Function Approval() As ActionResult
        ViewBag.Breadcrumb = "Home > Extract Setting Non Transaction > Approval"
        Return View("Index")
    End Function

    '
    ' POST: /ExtractSettingNonTransaction/Delete
    <Authorize> _
    <HttpPost> _
    Public Function Delete() As ActionResult
        Dim db As New FectorEntities()
        Dim glcode As String = Request.Form("glcode")
        Dim trancode As String = Request.Form("trancode")
        If IsNothing(glcode) Or IsNothing(trancode) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedExtractSettingTransaction = MsExtractSettingNonTransaction.GetAllExtractSettingNonTransaction(glcode, trancode, db)

        If IsNothing(editedExtractSettingTransaction) Then
            Return New HttpNotFoundResult
        End If

        editedExtractSettingTransaction.Status = "DELETE - PENDING"
        editedExtractSettingTransaction.EditBy = User.Identity.Name
        editedExtractSettingTransaction.EditDate = Now
        db.Entry(editedExtractSettingTransaction).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "DELETE NONTRAN CODE", editedExtractSettingTransaction.GLCode + "-" + editedExtractSettingTransaction.TransCode, db)


        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /ExtractSettingNonTransaction/Inactive
    <Authorize> _
    <HttpPost> _
    Public Function Inactive() As ActionResult
        Dim db As New FectorEntities()
        Dim glcode As String = Request.Form("glcode")
        Dim trancode As String = Request.Form("trancode")
        If IsNothing(glcode) Or IsNothing(trancode) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedExtractSettingTransaction As MsExtractSettingNonTransaction = MsExtractSettingNonTransaction.GetAllExtractSettingNonTransaction(glcode, trancode, db)

        If IsNothing(editedExtractSettingTransaction) Then
            Return New HttpNotFoundResult
        End If

        editedExtractSettingTransaction.Status = "INACTIVE - PENDING"
        editedExtractSettingTransaction.EditBy = User.Identity.Name
        editedExtractSettingTransaction.EditDate = Now
        db.Entry(editedExtractSettingTransaction).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "INACTIVE NONTRAN CODE", editedExtractSettingTransaction.GLCode + "-" + editedExtractSettingTransaction.TransCode, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /ExtractSettingNonTransaction/Active
    <Authorize> _
    <HttpPost> _
    Public Function Active() As ActionResult
        Dim db As New FectorEntities()
        Dim glcode As String = Request.Form("glcode")
        Dim trancode As String = Request.Form("trancode")
        If IsNothing(glcode) Or IsNothing(trancode) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedExtractSettingTransaction As MsExtractSettingNonTransaction = MsExtractSettingNonTransaction.GetAllExtractSettingNonTransaction(glcode, trancode, db)

        If IsNothing(editedExtractSettingTransaction) Then
            Return New HttpNotFoundResult
        End If

        editedExtractSettingTransaction.Status = "ACTIVE - PENDING"
        editedExtractSettingTransaction.EditBy = User.Identity.Name
        editedExtractSettingTransaction.EditDate = Now
        db.Entry(editedExtractSettingTransaction).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "ACTIVE NONTRAN CODE", editedExtractSettingTransaction.GLCode + "-" + editedExtractSettingTransaction.TransCode, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /ExtractSettingNonTransaction/Approve
    <Authorize> _
    <HttpPost> _
    Public Function Approve() As ActionResult
        Dim db As New FectorEntities()
        Dim glcode As String = Request.Form("glcode")
        Dim trancode As String = Request.Form("trancode")
        If IsNothing(glcode) Or IsNothing(trancode) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedExtractSettingTransaction As MsExtractSettingNonTransaction = MsExtractSettingNonTransaction.GetAllExtractSettingNonTransaction(glcode, trancode, db)

        If IsNothing(editedExtractSettingTransaction) Then
            Return New HttpNotFoundResult
        End If

        Dim flagDelete As Boolean = False
        Select Case editedExtractSettingTransaction.Status
            Case "ACTIVE - PENDING", "CREATE - PENDING"
                editedExtractSettingTransaction.Status = "ACTIVE"
            Case "INACTIVE - PENDING"
                editedExtractSettingTransaction.Status = "INACTIVE"
            Case "DELETE - PENDING"
                flagDelete = True
        End Select

        If flagDelete Then
            db.ExtractSettingNonTransaction.Remove(editedExtractSettingTransaction)
            db.SaveChanges()
        Else
            editedExtractSettingTransaction.ApproveBy = User.Identity.Name
            editedExtractSettingTransaction.ApproveDate = Now
            db.Entry(editedExtractSettingTransaction).State = EntityState.Modified
            db.SaveChanges()
        End If

        LogTransaction.WriteLog(User.Identity.Name, "APPROVE NONTRAN CODE", editedExtractSettingTransaction.GLCode + "-" + editedExtractSettingTransaction.TransCode, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' POST: /ExtractSettingNonTransaction/Reject
    <Authorize> _
    <HttpPost> _
    Public Function Reject() As ActionResult
        Dim db As New FectorEntities()
        Dim glcode As String = Request.Form("glcode")
        Dim trancode As String = Request.Form("trancode")
        If IsNothing(glcode) Or IsNothing(trancode) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedExtractSettingTransaction As MsExtractSettingNonTransaction = MsExtractSettingNonTransaction.GetAllExtractSettingNonTransaction(glcode, trancode, db)

        If IsNothing(editedExtractSettingTransaction) Then
            Return New HttpNotFoundResult
        End If

        editedExtractSettingTransaction.Status = "REJECTED"
        editedExtractSettingTransaction.ApproveBy = User.Identity.Name
        editedExtractSettingTransaction.ApproveDate = Now
        db.Entry(editedExtractSettingTransaction).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "REJECT NONTRAN CODE", editedExtractSettingTransaction.GLCode + "-" + editedExtractSettingTransaction.TransCode, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' Get: /ExtractSettingNonTransaction/Create
    <Authorize> _
    Public Function Create() As ActionResult
        Dim model As New MsExtractSettingNonTransaction
        Dim db As New FectorEntities

        ViewData("Title") = "Create Extract Setting Non Transaction"
        ViewBag.Breadcrumb = "Home > Extract Setting Non Transaction > Create"

        Return View("Detail", model)
    End Function

    '
    ' Post: /ExtractSettingNonTransaction/Create
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Create(model As MsExtractSettingNonTransaction) As ActionResult
        Dim db As New FectorEntities()

        ViewData("Title") = "Create Extract Setting Non Transaction"
        ViewBag.Breadcrumb = "Home > Extract Setting Non Transaction > Create"
        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim GLCode As String = model.GLCode
            Dim TransCode As String = model.TransCode

            Dim existingSetting = MsExtractSettingNonTransaction.GetAllExtractSettingNonTransaction(GLCode, TransCode, db)

            If IsNothing(existingSetting) Then
                Dim newExtractSettingTransaction As New MsExtractSettingNonTransaction
                With newExtractSettingTransaction
                    .GLCode = GLCode
                    .TransCode = TransCode
                    .EditBy = User.Identity.Name
                    .EditDate = DateTime.Now
                    .ApproveBy = ""
                    .ApproveDate = New DateTime(1900, 1, 1)
                    .Status = "CREATE - PENDING"
                End With
                db.ExtractSettingNonTransaction.Add(newExtractSettingTransaction)
                db.SaveChanges()

                LogTransaction.WriteLog(User.Identity.Name, "CREATE NONTRAN CODE", GLCode + "-" + TransCode, db)

                Return RedirectToAction("Index", "ExtractSettingNonTransaction")
            Else
                ModelState.AddModelError("TransCode", "COA and Transaction Code has already registered")
                ModelState.AddModelError("GLCode", "COA and Transaction Code has already registered")
            End If
        End If

        Return View("Detail", model)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTExtractSettingNonTransactionAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim ExtractSettingTransaction = From s In db.ExtractSettingNonTransaction Select s
        Return ReturnExtractSettingNonTransactionDataTable(param, ExtractSettingTransaction)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTExtractSettingNonTransactionApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim ExtractSettingTransaction = From s In db.ExtractSettingNonTransaction
                    Where s.Status.Contains("PENDING") And s.EditBy <> User.Identity.Name
                    Select s
        Return ReturnExtractSettingNonTransactionDataTable(param, ExtractSettingTransaction)
    End Function

    Private Function ReturnExtractSettingNonTransactionDataTable(param As jQueryDataTableParamModel, ExtractSettingTransaction As IQueryable(Of MsExtractSettingNonTransaction)) As JsonResult
        Dim totalRecords = ExtractSettingTransaction.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim GLCodeSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim TransCodeSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim StatusSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        ExtractSettingTransaction = ExtractSettingTransaction.Where(Function(f) f.GLCode.Contains(GLCodeSearch))
        ExtractSettingTransaction = ExtractSettingTransaction.Where(Function(f) f.TransCode.Contains(TransCodeSearch))
        If StatusSearch <> "" Then
            ExtractSettingTransaction = ExtractSettingTransaction.Where(Function(f) f.Status.StartsWith(StatusSearch))
        End If


        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    ExtractSettingTransaction = ExtractSettingTransaction.OrderBy(Function(f) f.GLCode)
                Else
                    ExtractSettingTransaction = ExtractSettingTransaction.OrderByDescending(Function(f) f.GLCode)
                End If
            Case 1
                If sortOrder = "asc" Then
                    ExtractSettingTransaction = ExtractSettingTransaction.OrderBy(Function(f) f.TransCode)
                Else
                    ExtractSettingTransaction = ExtractSettingTransaction.OrderByDescending(Function(f) f.TransCode)
                End If
            Case 2
                If sortOrder = "asc" Then
                    ExtractSettingTransaction = ExtractSettingTransaction.OrderBy(Function(f) f.Status)
                Else
                    ExtractSettingTransaction = ExtractSettingTransaction.OrderByDescending(Function(f) f.Status)
                End If
        End Select

        Dim result As New List(Of String())
        For Each data As MsExtractSettingNonTransaction In ExtractSettingTransaction
            result.Add({data.GLCode, data.TransCode, data.Status, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

End Class