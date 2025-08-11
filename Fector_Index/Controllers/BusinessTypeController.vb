Imports System.Net
Imports System.Data.Objects.SqlClient

Public Class BusinessTypeController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /BusinessType

    Public Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > Business Type"
        Return View("Index")
    End Function

    ' GET: /BusinessTpye/Approval
    <Authorize> _
    Public Function Approval() As ActionResult
        ViewBag.Breadcrumb = "Home > Business Type > Approval"
        Return View("Index")
    End Function

    '
    ' POST: /BusinessType/Delete
    <Authorize> _
    <HttpPost> _
    Public Function Delete() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedbusinesstype = db.BusinessType.Find(id)

        If IsNothing(editedbusinesstype) Then
            Return New HttpNotFoundResult
        End If

        editedbusinesstype.Status = "DELETE - PENDING"
        editedbusinesstype.EditBy = User.Identity.Name
        editedbusinesstype.EditDate = Now
        db.Entry(editedbusinesstype).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "DELETE BUSINESS TYPE", editedbusinesstype.BusinessTypeId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /BusinessType/Inactive
    <Authorize> _
    <HttpPost> _
    Public Function Inactive() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedbusinesstype As MsBusinessType = MsBusinessType.GetAllBusinessType(id, db)

        If IsNothing(editedbusinesstype) Then
            Return New HttpNotFoundResult
        End If

        editedbusinesstype.Status = "INACTIVE - PENDING"
        editedbusinesstype.EditBy = User.Identity.Name
        editedbusinesstype.EditDate = Now
        db.Entry(editedbusinesstype).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "INACTIVE BUSINESS TYPE", editedbusinesstype.BusinessTypeId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /BusinessType/Active
    <Authorize> _
    <HttpPost> _
    Public Function Active() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedbusinesstype As MsBusinessType = MsBusinessType.GetAllBusinessType(id, db)

        If IsNothing(editedbusinesstype) Then
            Return New HttpNotFoundResult
        End If

        editedbusinesstype.Status = "ACTIVE - PENDING"
        editedbusinesstype.EditBy = User.Identity.Name
        editedbusinesstype.EditDate = Now
        db.Entry(editedbusinesstype).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "ACTIVE BUSINESS TYPE", editedbusinesstype.BusinessTypeId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /BusinessType/Approve
    <Authorize> _
    <HttpPost> _
    Public Function Approve() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedbusinesstype = db.BusinessType.Find(id)

        If IsNothing(editedbusinesstype) Then
            Return New HttpNotFoundResult
        End If

        Dim flagDelete As Boolean = False
        Select Case editedbusinesstype.Status
            Case "ACTIVE - PENDING", "EDIT - PENDING", "CREATE - PENDING"
                editedbusinesstype.Status = "ACTIVE"
            Case "INACTIVE - PENDING"
                editedbusinesstype.Status = "INACTIVE"
            Case "DELETE - PENDING"
                flagDelete = True
        End Select

        If flagDelete Then
            db.BusinessType.Remove(editedbusinesstype)
            db.SaveChanges()
        Else
            editedbusinesstype.ApproveBy = User.Identity.Name
            editedbusinesstype.ApproveDate = Now
            db.Entry(editedbusinesstype).State = EntityState.Modified
            db.SaveChanges()
        End If

        LogTransaction.WriteLog(User.Identity.Name, "APPROVE BUSINESS TYPE", editedbusinesstype.BusinessTypeId, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' POST: /BusinessType/Reject
    <Authorize> _
    <HttpPost> _
    Public Function Reject() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedbusinesstype = db.BusinessType.Find(id)

        If IsNothing(editedbusinesstype) Then
            Return New HttpNotFoundResult
        End If

        editedbusinesstype.Status = "REJECTED"
        editedbusinesstype.ApproveBy = User.Identity.Name
        editedbusinesstype.ApproveDate = Now
        db.Entry(editedbusinesstype).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "REJECT BUSINESS TYPE", editedbusinesstype.BusinessTypeId, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' Get: /BusinessType/Edit
    <Authorize> _
    Public Function Edit(ByVal id As String) As ActionResult
        Dim model As New MsBusinessType
        Dim db As New FectorEntities

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return RedirectToAction("Index", "BusinessType")
        End If

        If IsNothing(model.BusinessTypeId) Then
            model = MsBusinessType.GetAllBusinessType(id, db)
        End If

        ViewData("Title") = "Edit Business Type"
        ViewBag.Breadcrumb = "Home > Business Type > Edit"
        ViewBag.BusinessTypeId = New SelectList(db.BusinessType, "BusinessTypeId", "BusinessTypeId")
        ViewBag.Description = New SelectList(db.BusinessType, "Description", "Description")

        Return View("Detail", model)
    End Function

    '
    ' Post: /BusinessType/Edit
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Edit(model As MsBusinessType) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Edit Business Type"
        ViewBag.Breadcrumb = "Home > Business Type > Edit"
        ViewBag.BusinessTypeId = New SelectList(db.BusinessType, "BusinessTypeId", "BusinessTypeId")
        ViewBag.Description = New SelectList(db.BusinessType, "Description", "Description")

        If ModelState.IsValid Then
            Dim BusinessTypeId As String = model.BusinessTypeId
            Dim Description As String = model.Description

            Dim editedbusinesstype As MsBusinessType = MsBusinessType.GetAllBusinessType(model.BusinessTypeId, db)
            With editedbusinesstype
                .Description = Description
                .EditBy = User.Identity.Name
                .EditDate = DateTime.Now
                .ApproveBy = ""
                .ApproveDate = New DateTime(1900, 1, 1)
                .Status = "EDIT - PENDING"
            End With
            db.Entry(editedbusinesstype).State = EntityState.Modified
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "EDIT BUSINESS TYPE", model.BusinessTypeId, db)

            Return RedirectToAction("Index")
        Else
            Return View("Detail", model)
        End If
    End Function

    '
    ' Get: /BusinessType/Create
    <Authorize> _
    Public Function Create() As ActionResult
        Dim model As New MsBusinessType
        Dim db As New FectorEntities

        ViewData("Title") = "Create Business Type"
        ViewBag.Breadcrumb = "Home > Business Type > Create"
        ViewBag.BusinessTypeId = New SelectList(db.BusinessType, "BusinessTypeId", "BusinessTypeId")
        ViewBag.Description = New SelectList(db.BusinessType, "Description", "Description")

        Return View("Detail", model)
    End Function

    '
    ' Post: /BusinessType/Create
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Create(model As MsBusinessType) As ActionResult
        Dim db As New FectorEntities()

        ViewData("Title") = "Create Business Type"
        ViewBag.Breadcrumb = "Home > Business Type > Create"
        ViewBag.BusinessTypeId = New SelectList(db.BusinessType, "BusinessTypeId", "BusinessTypeId")
        ViewBag.Description = New SelectList(db.BusinessType, "Description", "Description")
        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim BusinessTypeId As String = model.BusinessTypeId
            Dim Description As String = model.Description
            Dim existingBusinessType = MsBusinessType.GetAllBusinessType(BusinessTypeId, db)

            If IsNothing(existingBusinessType) Then
                Dim newBusinessType As New MsBusinessType
                With newBusinessType
                    .BusinessTypeId = BusinessTypeId
                    .Description = Description
                    .EditBy = User.Identity.Name
                    .EditDate = DateTime.Now
                    .ApproveBy = ""
                    .ApproveDate = New DateTime(1900, 1, 1)
                    .Status = "CREATE - PENDING"
                End With
                db.BusinessType.Add(newBusinessType)
                db.SaveChanges()

                LogTransaction.WriteLog(User.Identity.Name, "CREATE BUSINESS TYPE", BusinessTypeId, db)

                Return RedirectToAction("Index", "BusinessType")
            Else
                ModelState.AddModelError("BusinessTypeId", "Business Type ID has already registered")
            End If
        End If

        Return View("Detail", model)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTBusinessTypeAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim BusinessType = From s In db.BusinessType Select s
        Return ReturnBusinessTypeDataTable(param, BusinessType)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTBusinessTypeApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim BusinessType = From s In db.BusinessType
                    Where s.Status.Contains("PENDING") And s.EditBy <> User.Identity.Name
                    Select s
        Return ReturnBusinessTypeDataTable(param, BusinessType)
    End Function

    Private Function ReturnBusinessTypeDataTable(param As jQueryDataTableParamModel, BusinessType As IQueryable(Of MsBusinessType)) As JsonResult
        Dim totalRecords = BusinessType.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim BusinessTypeIdSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim DescriptionSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim StatusSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        BusinessType = BusinessType.Where(Function(f) f.BusinessTypeId.Contains(BusinessTypeIdSearch))
        BusinessType = BusinessType.Where(Function(f) f.Description.Contains(DescriptionSearch))
        If StatusSearch <> "" Then
            BusinessType = BusinessType.Where(Function(f) f.Status.StartsWith(StatusSearch))
        End If


        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    BusinessType = BusinessType.OrderBy(Function(f) f.BusinessTypeId)
                Else
                    BusinessType = BusinessType.OrderByDescending(Function(f) f.BusinessTypeId)
                End If
            Case 1
                If sortOrder = "asc" Then
                    BusinessType = BusinessType.OrderBy(Function(f) f.Description)
                Else
                    BusinessType = BusinessType.OrderByDescending(Function(f) f.Description)
                End If
            Case 2
                If sortOrder = "asc" Then
                    BusinessType = BusinessType.OrderBy(Function(f) f.Status)
                Else
                    BusinessType = BusinessType.OrderByDescending(Function(f) f.Status)
                End If
        End Select

        Dim result As New List(Of String())
        For Each data As MsBusinessType In BusinessType
            result.Add({data.BusinessTypeId, data.Description, data.Status, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

End Class