Imports System.Net
Imports System.Data.Objects.SqlClient

Public Class PurposeController
    Inherits System.Web.Mvc.Controller

    ' GET: /Purpose/Index
    <Authorize> _
    Public Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > Purpose"
        Return View("Index")
    End Function

    ' GET: /Purpose/Approval
    <Authorize> _
    Public Function Approval() As ActionResult
        ViewBag.Breadcrumb = "Home > Purpose > Approval"
        Return View("Index")
    End Function

    '
    ' POST: /Purpose/Delete
    <Authorize> _
    <HttpPost> _
    Public Function Delete() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedpurpose = db.Purposes.Find(id)

        If IsNothing(editedpurpose) Then
            Return New HttpNotFoundResult
        End If

        editedpurpose.Status = "DELETE - PENDING"
        editedpurpose.EditBy = User.Identity.Name
        editedpurpose.EditDate = Now
        db.Entry(editedpurpose).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "DELETE PURPOSE", editedpurpose.PurposeId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /Purpose/Inactive
    <Authorize> _
    <HttpPost> _
    Public Function Inactive() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedpurpose = db.Purposes.Find(id)

        If IsNothing(editedpurpose) Then
            Return New HttpNotFoundResult
        End If

        editedpurpose.Status = "INACTIVE - PENDING"
        editedpurpose.EditBy = User.Identity.Name
        editedpurpose.EditDate = Now
        db.Entry(editedpurpose).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "INACTIVE PURPOSE", editedpurpose.PurposeId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /Purpose/Active
    <Authorize> _
    <HttpPost> _
    Public Function Active() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedpurpose = db.Purposes.Find(id)

        If IsNothing(editedpurpose) Then
            Return New HttpNotFoundResult
        End If

        editedpurpose.Status = "ACTIVE - PENDING"
        editedpurpose.EditBy = User.Identity.Name
        editedpurpose.EditDate = Now
        db.Entry(editedpurpose).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "ACTIVE PURPOSE", editedpurpose.PurposeId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /Purpose/Approve
    <Authorize> _
    <HttpPost> _
    Public Function Approve() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedpurpose = db.Purposes.Find(id)

        If IsNothing(editedpurpose) Then
            Return New HttpNotFoundResult
        End If

        Dim flagDelete As Boolean = False
        Dim status As String = ""

        Select Case editedpurpose.Status
            Case "ACTIVE - PENDING", "EDIT - PENDING", "CREATE - PENDING"
                editedpurpose.Status = "ACTIVE"
            Case "INACTIVE - PENDING"
                editedpurpose.Status = "INACTIVE"
            Case "DELETE - PENDING"
                flagDelete = True
        End Select

        If flagDelete Then
            db.Purposes.Remove(editedpurpose)
            db.SaveChanges()
        Else
            editedpurpose.ApproveBy = User.Identity.Name
            editedpurpose.ApproveDate = Now
            db.Entry(editedpurpose).State = EntityState.Modified
            db.SaveChanges()
        End If

        LogTransaction.WriteLog(User.Identity.Name, "APPROVE PURPOSE", editedpurpose.PurposeId, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' POST: /Purpose/Reject
    <Authorize> _
    <HttpPost> _
    Public Function Reject() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedpurpose = db.Purposes.Find(id)

        If IsNothing(editedpurpose) Then
            Return New HttpNotFoundResult
        End If

        editedpurpose.Status = "REJECTED"
        editedpurpose.ApproveBy = User.Identity.Name
        editedpurpose.ApproveDate = Now
        db.Entry(editedpurpose).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "REJECT PURPOSE", editedpurpose.PurposeId, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' Get: /Purpose/Edit
    <Authorize> _
    Public Function Edit(ByVal id As String) As ActionResult
        Dim model As New MsPurpose
        Dim db As New FectorEntities

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return RedirectToAction("Index", "Purpose")
        End If

        If IsNothing(model.PurposeId) Then
            model = MsPurpose.GetAllPurpose(id, db)
        End If

        ViewData("Title") = "Edit Purpose"
        ViewBag.Breadcrumb = "Home > Purpose > Edit"
        
        Return View("Detail", model)
    End Function

    '
    ' Post: /Purpose/Edit
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Edit(model As MsPurpose) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Edit Purpose"
        ViewBag.Breadcrumb = "Home > Purpose > Edit"
        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim description As String = model.Description

            Dim editedpurpose As MsPurpose = MsPurpose.GetAllPurpose(model.PurposeId, db)
            With editedpurpose
                .Description = description
                .EditBy = User.Identity.Name
                .EditDate = DateTime.Now
                .ApproveBy = ""
                .ApproveDate = New DateTime(1900, 1, 1)
                .Status = "EDIT - PENDING"
            End With
            db.Entry(editedpurpose).State = EntityState.Modified
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "EDIT PURPOSE", model.PurposeId, db)

            Return RedirectToAction("Index")
        Else
            Return View("Detail", model)
        End If
    End Function

    '
    ' Get: /Purpose/Create
    <Authorize> _
    Public Function Create() As ActionResult
        Dim model As New MsPurpose
        Dim db As New FectorEntities

        ViewData("Title") = "Create Purpose"
        ViewBag.Breadcrumb = "Home > Purpose > Create"
        
        Return View("Detail", model)
    End Function

    '
    ' Post: /Purpose/Create
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Create(model As MsPurpose) As ActionResult
        Dim db As New FectorEntities()

        ViewData("Title") = "Create Purpose"
        ViewBag.Breadcrumb = "Home > Purpose > Create"
        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim id As String = model.PurposeId
            Dim description As String = model.Description

            Dim existingpurpose = MsPurpose.GetAllPurpose(id, db)

            If IsNothing(existingpurpose) Then
                Dim newPurpose As New MsPurpose
                With newPurpose
                    .PurposeId = id
                    .Description = description
                    .EditBy = User.Identity.Name
                    .EditDate = DateTime.Now
                    .ApproveBy = ""
                    .ApproveDate = New DateTime(1900, 1, 1)
                    .Status = "CREATE - PENDING"
                End With
                db.Purposes.Add(newPurpose)
                db.SaveChanges()

                LogTransaction.WriteLog(User.Identity.Name, "CREATE PURPOSE", id, db)

                Return RedirectToAction("Index")
            Else
                ModelState.AddModelError("PurposeId", "Purpose code has already registered")
            End If
        Else
            Return View("Detail", model)
        End If

        Return View("Detail", model)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTPurposeAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim purposes = From s In db.Purposes Select s
        Return ReturnPurposeDataTable(param, purposes)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTPurposeApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim purposes = From s In db.Purposes
                    Where s.Status.Contains("PENDING") And s.EditBy <> User.Identity.Name
                    Select s
        Return ReturnPurposeDataTable(param, purposes)
    End Function

    Private Function ReturnPurposeDataTable(param As jQueryDataTableParamModel, purposes As IQueryable(Of MsPurpose)) As JsonResult
        Dim totalRecords = purposes.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim IdSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim DescriptionSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim StatusSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))

        purposes = purposes.Where(Function(f) f.PurposeId.Contains(IdSearch))
        purposes = purposes.Where(Function(f) f.Description.Contains(DescriptionSearch))
        If StatusSearch <> "" Then
            purposes = purposes.Where(Function(f) f.Status.StartsWith(StatusSearch))
        End If


        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    purposes = purposes.OrderBy(Function(f) f.PurposeId)
                Else
                    purposes = purposes.OrderByDescending(Function(f) f.PurposeId)
                End If
            Case 1
                If sortOrder = "asc" Then
                    purposes = purposes.OrderBy(Function(f) f.Description)
                Else
                    purposes = purposes.OrderByDescending(Function(f) f.Description)
                End If
            Case 2
                If sortOrder = "asc" Then
                    purposes = purposes.OrderBy(Function(f) f.Status)
                Else
                    purposes = purposes.OrderByDescending(Function(f) f.Status)
                End If
        End Select

        Dim result As New List(Of String())
        For Each purpose As MsPurpose In purposes
            result.Add({purpose.PurposeId, purpose.Description, purpose.Status, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

End Class