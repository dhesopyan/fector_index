Imports System.Net
Imports System.Data.Objects.SqlClient

Public Class DocumentLHBUController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /DocumentLHBU

    Public Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > Document LHBU"
        Return View("Index")
    End Function

    ' GET: /DocumentLHBU/Approval
    <Authorize> _
    Public Function Approval() As ActionResult
        ViewBag.Breadcrumb = "Home > Document LHBU > Approval"
        Return View("Index")
    End Function

    '
    ' POST: /DocumentLHBU/Delete
    <Authorize> _
    <HttpPost> _
    Public Function Delete() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedDocumentLHBU = db.DocumentLHBU.Find(id)

        If IsNothing(editedDocumentLHBU) Then
            Return New HttpNotFoundResult
        End If

        editedDocumentLHBU.Status = "DELETE - PENDING"
        editedDocumentLHBU.EditBy = User.Identity.Name
        editedDocumentLHBU.EditDate = Now
        db.Entry(editedDocumentLHBU).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "DELETE DOC LHBU", editedDocumentLHBU.DocumentId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /DocumentLHBU/Inactive
    <Authorize> _
    <HttpPost> _
    Public Function Inactive() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedDocumentLHBU As MsDocumentLHBU = MsDocumentLHBU.GetAllDocumentLHBU(id, db)

        If IsNothing(editedDocumentLHBU) Then
            Return New HttpNotFoundResult
        End If

        editedDocumentLHBU.Status = "INACTIVE - PENDING"
        editedDocumentLHBU.EditBy = User.Identity.Name
        editedDocumentLHBU.EditDate = Now
        db.Entry(editedDocumentLHBU).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "INACTIVE DOC LHBU", editedDocumentLHBU.DocumentId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /DocumentLHBU/Active
    <Authorize> _
    <HttpPost> _
    Public Function Active() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedDocumentLHBU As MsDocumentLHBU = MsDocumentLHBU.GetAllDocumentLHBU(id, db)

        If IsNothing(editedDocumentLHBU) Then
            Return New HttpNotFoundResult
        End If

        editedDocumentLHBU.Status = "ACTIVE - PENDING"
        editedDocumentLHBU.EditBy = User.Identity.Name
        editedDocumentLHBU.EditDate = Now
        db.Entry(editedDocumentLHBU).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "ACTIVE DOC LHBU", editedDocumentLHBU.DocumentId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /DocumentLHBU/Approve
    <Authorize> _
    <HttpPost> _
    Public Function Approve() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedDocumentLHBU = db.DocumentLHBU.Find(id)

        If IsNothing(editedDocumentLHBU) Then
            Return New HttpNotFoundResult
        End If

        Dim flagDelete As Boolean = False
        Select Case editedDocumentLHBU.Status
            Case "ACTIVE - PENDING", "EDIT - PENDING", "CREATE - PENDING"
                editedDocumentLHBU.Status = "ACTIVE"
            Case "INACTIVE - PENDING"
                editedDocumentLHBU.Status = "INACTIVE"
            Case "DELETE - PENDING"
                flagDelete = True
        End Select

        If flagDelete Then
            db.DocumentLHBU.Remove(editedDocumentLHBU)
            db.SaveChanges()
        Else
            editedDocumentLHBU.ApproveBy = User.Identity.Name
            editedDocumentLHBU.ApproveDate = Now
            db.Entry(editedDocumentLHBU).State = EntityState.Modified
            db.SaveChanges()
        End If

        LogTransaction.WriteLog(User.Identity.Name, "APPROVE DOC LHBU", editedDocumentLHBU.DocumentId, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' POST: /DocumentLHBU/Reject
    <Authorize> _
    <HttpPost> _
    Public Function Reject() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedDocumentLHBU = db.DocumentLHBU.Find(id)

        If IsNothing(editedDocumentLHBU) Then
            Return New HttpNotFoundResult
        End If

        editedDocumentLHBU.Status = "REJECTED"
        editedDocumentLHBU.ApproveBy = User.Identity.Name
        editedDocumentLHBU.ApproveDate = Now
        db.Entry(editedDocumentLHBU).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "REJECT DOC LHBU", editedDocumentLHBU.DocumentId, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' Get: /DocumentLHBU/Edit
    <Authorize> _
    Public Function Edit(ByVal id As String) As ActionResult
        Dim model As New MsDocumentLHBU
        Dim db As New FectorEntities

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return RedirectToAction("Index", "DocumentLHBU")
        End If

        If IsNothing(model.DocumentId) Then
            model = MsDocumentLHBU.GetAllDocumentLHBU(id, db)
        End If

        ViewData("Title") = "Edit DOCUMENT LHBU"
        ViewBag.Breadcrumb = "Home > Document LHBU > Edit"

        Return View("Detail", model)
    End Function

    '
    ' Post: /DocumentLHBU/Edit
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Edit(model As MsDocumentLHBU) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Edit Document LHBU"
        ViewBag.Breadcrumb = "Home > Document LHBU > Edit"

        If ModelState.IsValid Then
            Dim DocumentId As String = model.DocumentId
            Dim Description As String = model.Description

            Dim editedDocumentLHBU As MsDocumentLHBU = MsDocumentLHBU.GetAllDocumentLHBU(model.DocumentId, db)
            With editedDocumentLHBU
                .Description = Description
                .EditBy = User.Identity.Name
                .EditDate = DateTime.Now
                .ApproveBy = ""
                .ApproveDate = New DateTime(1900, 1, 1)
                .Status = "EDIT - PENDING"
            End With
            db.Entry(editedDocumentLHBU).State = EntityState.Modified
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "EDIT DOC LHBU", model.DocumentId, db)

            Return RedirectToAction("Index")
        Else
            Return View("Detail", model)
        End If
    End Function

    '
    ' Get: /DocumentLHBU/Create
    <Authorize> _
    Public Function Create() As ActionResult
        Dim model As New MsDocumentLHBU
        Dim db As New FectorEntities

        ViewData("Title") = "Create Document LHBU"
        ViewBag.Breadcrumb = "Home > Document LHBU > Create"

        Return View("Detail", model)
    End Function

    '
    ' Post: /DocumentLHBU/Create
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Create(model As MsDocumentLHBU) As ActionResult
        Dim db As New FectorEntities()

        ViewData("Title") = "Create Document LHBU"
        ViewBag.Breadcrumb = "Home > Document LHBU > Create"
        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim DocumentId As String = model.DocumentId
            Dim Description As String = model.Description

            Dim existingDocumentLHBU = MsDocumentLHBU.GetAllDocumentLHBU(DocumentId, db)

            If IsNothing(existingDocumentLHBU) Then
                Dim newDocumentLHBU As New MsDocumentLHBU
                With newDocumentLHBU
                    .DocumentId = DocumentId
                    .Description = Description
                    .EditBy = User.Identity.Name
                    .EditDate = DateTime.Now
                    .ApproveBy = ""
                    .ApproveDate = New DateTime(1900, 1, 1)
                    .Status = "CREATE - PENDING"
                End With
                db.DocumentLHBU.Add(newDocumentLHBU)
                db.SaveChanges()

                LogTransaction.WriteLog(User.Identity.Name, "CREATE DOC LHBU", DocumentId, db)

                Return RedirectToAction("Index", "DocumentLHBU")
            Else
                ModelState.AddModelError("DocumentId", "Document ID has already registered")
            End If
        End If

        Return View("Detail", model)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTDocumentLHBUAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim DocumentLHBU = From s In db.DocumentLHBU Select s
        Return ReturnDocumentLHBUDataTable(param, DocumentLHBU)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTDocumentLHBUApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim DocumentLHBU = From s In db.DocumentLHBU
                    Where s.Status.Contains("PENDING") And s.EditBy <> User.Identity.Name
                    Select s
        Return ReturnDocumentLHBUDataTable(param, DocumentLHBU)
    End Function

    Private Function ReturnDocumentLHBUDataTable(param As jQueryDataTableParamModel, DocumentLHBU As IQueryable(Of MsDocumentLHBU)) As JsonResult
        Dim totalRecords = DocumentLHBU.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim DocumentIdSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim DescriptionSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim StatusSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        DocumentLHBU = DocumentLHBU.Where(Function(f) f.DocumentId.Contains(DocumentIdSearch))
        DocumentLHBU = DocumentLHBU.Where(Function(f) f.Description.Contains(DescriptionSearch))
        If StatusSearch <> "" Then
            DocumentLHBU = DocumentLHBU.Where(Function(f) f.Status.StartsWith(StatusSearch))
        End If


        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    DocumentLHBU = DocumentLHBU.OrderBy(Function(f) f.DocumentId)
                Else
                    DocumentLHBU = DocumentLHBU.OrderByDescending(Function(f) f.DocumentId)
                End If
            Case 1
                If sortOrder = "asc" Then
                    DocumentLHBU = DocumentLHBU.OrderBy(Function(f) f.Description)
                Else
                    DocumentLHBU = DocumentLHBU.OrderByDescending(Function(f) f.Description)
                End If
            Case 2
                If sortOrder = "asc" Then
                    DocumentLHBU = DocumentLHBU.OrderBy(Function(f) f.Status)
                Else
                    DocumentLHBU = DocumentLHBU.OrderByDescending(Function(f) f.Status)
                End If
        End Select

        Dim result As New List(Of String())
        For Each data As MsDocumentLHBU In DocumentLHBU
            result.Add({data.DocumentId, data.Description, data.Status, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function


End Class