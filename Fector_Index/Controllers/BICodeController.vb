Imports System.Net
Imports System.Data.Objects.SqlClient
Imports System.Linq

Public Class BICodeController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /BICode/Index

    Public Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > BI Code"
        Return View("Index")
    End Function

    ' GET: /BICode/Approval
    <Authorize> _
    Public Function Approval() As ActionResult
        ViewBag.Breadcrumb = "Home > BI Code > Approval"
        Return View("Index")
    End Function

    '
    ' POST: /BICode/Delete
    <Authorize> _
    <HttpPost> _
    Public Function Delete() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedbicode = db.BICodes.Find(id)

        If IsNothing(editedbicode) Then
            Return New HttpNotFoundResult
        End If

        editedbicode.Status = "DELETE - PENDING"
        editedbicode.EditBy = User.Identity.Name
        editedbicode.EditDate = Now
        db.Entry(editedbicode).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "DELETE BI CODE", editedbicode.BICode, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /BICode/Inactive
    <Authorize> _
    <HttpPost> _
    Public Function Inactive() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedbicode = db.BICodes.Find(id)

        If IsNothing(editedbicode) Then
            Return New HttpNotFoundResult
        End If

        editedbicode.Status = "INACTIVE - PENDING"
        editedbicode.EditBy = User.Identity.Name
        editedbicode.EditDate = Now
        db.Entry(editedbicode).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "INACTIVE BI CODE", editedbicode.BICode, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /BICode/Active
    <Authorize> _
    <HttpPost> _
    Public Function Active() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedbicode = db.BICodes.Find(id)

        If IsNothing(editedbicode) Then
            Return New HttpNotFoundResult
        End If

        editedbicode.Status = "ACTIVE - PENDING"
        editedbicode.EditBy = User.Identity.Name
        editedbicode.EditDate = Now
        db.Entry(editedbicode).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "ACTIVE BI CODE", editedbicode.BICode, db)

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

        Dim editedbicode = db.BICodes.Find(id)

        If IsNothing(editedbicode) Then
            Return New HttpNotFoundResult
        End If

        Dim flagDelete As Boolean = False
        Select Case editedbicode.Status
            Case "ACTIVE - PENDING", "EDIT - PENDING", "CREATE - PENDING"
                editedbicode.Status = "ACTIVE"
            Case "INACTIVE - PENDING"
                editedbicode.Status = "INACTIVE"
            Case "DELETE - PENDING"
                flagDelete = True
        End Select

        If flagDelete Then
            db.BICodes.Remove(editedbicode)
            db.SaveChanges()
        Else
            editedbicode.ApproveBy = User.Identity.Name
            editedbicode.ApproveDate = Now
            db.Entry(editedbicode).State = EntityState.Modified
            db.SaveChanges()
        End If

        LogTransaction.WriteLog(User.Identity.Name, "APPROVE BI CODE", editedbicode.BICode, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' POST: /BICode/Reject
    <Authorize> _
    <HttpPost> _
    Public Function Reject() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedbicode = db.BICodes.Find(id)

        If IsNothing(editedbicode) Then
            Return New HttpNotFoundResult
        End If

        editedbicode.Status = "REJECTED"
        editedbicode.ApproveBy = User.Identity.Name
        editedbicode.ApproveDate = Now
        db.Entry(editedbicode).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "REJECT BI CODE", editedbicode.BICode, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' Get: /Purpose/Edit
    <Authorize> _
    Public Function Edit(ByVal id As String) As ActionResult
        Dim model As New MsBICode
        Dim db As New FectorEntities

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return RedirectToAction("Index", "BICode")
        End If

        If IsNothing(model.BICode) Then
            model = MsBICode.GetBICode(id, db)
        End If

        If IsNothing(model.BICode) Then
            model = MsBICode.GetBICode(id, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        ViewData("Title") = "Edit BI Code"
        ViewBag.Breadcrumb = "Home > BI Code > Edit"
        ViewBag.Status = New SelectList(db.SubjectStatus, "StatusId", "Description")

        Return View("Detail", model)
    End Function

    '
    ' Post: /BICode/Edit
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Edit(model As MsBICode) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Edit BI Code"
        ViewBag.Breadcrumb = "Home > BI Code > Edit"
        ViewBag.Status = New SelectList(db.SubjectStatus, "StatusId", "Description")
        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim SubjectStatusId As String = model.SubjectStatusId
            Dim SubjectName As String = model.SubjectName

            Dim editedbicode As MsBICode = MsBICode.GetBICode(model.BICode, db)
            With editedbicode
                .SubjectStatusId = SubjectStatusId
                .SubjectName = SubjectName
                .EditBy = User.Identity.Name
                .EditDate = DateTime.Now
                .ApproveBy = ""
                .ApproveDate = New DateTime(1900, 1, 1)
                .Status = "EDIT - PENDING"
            End With
            db.Entry(editedbicode).State = EntityState.Modified
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "EDIT BI CODE", model.BICode, db)

            Return RedirectToAction("Index")
        Else
            Return View("Detail", model)
        End If
    End Function

    '
    ' Get: /BICode/Create
    <Authorize> _
    Public Function Create() As ActionResult
        Dim model As New MsBICode
        Dim db As New FectorEntities

        ViewData("Title") = "Create BI Code"
        ViewBag.Breadcrumb = "Home > BI Code > Create"
        ViewBag.Status = New SelectList(db.SubjectStatus, "StatusId", "Description")

        Return View("Detail", model)
    End Function

    '
    ' Post: /BICode/Create
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Create(model As MsBICode) As ActionResult
        Dim db As New FectorEntities()

        ViewData("Title") = "Create BI Code"
        ViewBag.Breadcrumb = "Home > BI Code > Create"
        ViewBag.Status = New SelectList(db.SubjectStatus, "StatusId", "Description")
        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim id As String = model.BICode
            Dim SubjectStatusId As String = model.SubjectStatusId
            Dim SubjectStatusName As String = model.SubjectName

            Dim existingbicode = MsBICode.GetBICode(id, db)

            If IsNothing(existingbicode) Then
                Dim newBICode As New MsBICode
                With newBICode
                    .BICode = id
                    .SubjectStatusId = SubjectStatusId
                    .SubjectName = SubjectStatusName
                    .EditBy = User.Identity.Name
                    .EditDate = DateTime.Now
                    .ApproveBy = ""
                    .ApproveDate = New DateTime(1900, 1, 1)
                    .Status = "CREATE - PENDING"
                End With
                db.BICodes.Add(newBICode)
                db.SaveChanges()

                LogTransaction.WriteLog(User.Identity.Name, "CREATE BI CODE", id, db)

                Return RedirectToAction("Index")
            Else
                ModelState.AddModelError("BICode", "BI code has already registered")
            End If
        End If

        Return View("Detail", model)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTBICodeAjaxHandler(param As jQueryDataTableParamModel, name As String, selectedstatus As String) As JsonResult
        Dim db As New FectorEntities()
        If IsNothing(name) Then
            name = ""
        End If
        If IsNothing(selectedstatus) Then
            selectedstatus = ""
        End If

        Dim biCodeSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim statusSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim NameSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))

        Dim bicode = db.BICodes.Where(Function(f) f.Status = "ACTIVE" And f.SubjectStatusId = selectedstatus)

        bicode = bicode.Where(Function(f) f.BICode.Contains(biCodeSearch))
        bicode = bicode.Where(Function(f) f.Status.Contains(statusSearch))
        bicode = bicode.Where(Function(f) f.SubjectName.Contains(NameSearch))

        Dim bicodematching As New List(Of MsBICodeMatching)

        For Each code As MsBICode In bicode
            bicodematching.Add(New MsBICodeMatching With {.bicode = code, .NametoMatch = name})
        Next

        Dim bicodematchingfiltered = (From a In bicodematching
                                      Order By a.MatchPercentage Descending
                                     Select a).ToList

        Return ReturnBICodeDataTable(param, bicodematchingfiltered)
    End Function


    Private Function ReturnBICodeDataTable(param As jQueryDataTableParamModel, bicodes As List(Of MsBICodeMatching)) As JsonResult
        Dim totalRecords As Integer = bicodes.Count
        Dim displayRecord As Integer = bicodes.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim showed = (From acc In bicodes.Skip(iDisplayStart).Take(iDisplayLength)
                      Select acc).ToList

        Dim result As New List(Of String())
        For Each code As MsBICodeMatching In showed
            result.Add({code.bicode.BICode, code.bicode.SubjectStatus.Description, code.bicode.SubjectName, code.MatchPercentage.ToString("N2"), ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = displayRecord,
                      .aaData = result},
                      JsonRequestBehavior.AllowGet)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTMsBICodeAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim bicode = From s In db.BICodes
                     Select s
        Return ReturnMsBICodeDataTable(param, bicode)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTMsBICodeApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim bicode = From s In db.BICodes
                    Where s.Status.Contains("PENDING") And s.EditBy <> User.Identity.Name
                    Select s
        Return ReturnMsBICodeDataTable(param, bicode)
    End Function

    Private Function ReturnMsBICodeDataTable(param As jQueryDataTableParamModel, bicode As IQueryable(Of MsBICode)) As JsonResult
        Dim totalRecords = bicode.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim BICodeSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim SubjectStatusIdSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim SubjectNameSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        Dim StatusSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))
        bicode = bicode.Where(Function(f) f.BICode.Contains(BICodeSearch))
        bicode = bicode.Where(Function(f) f.SubjectStatusId.Contains(SubjectStatusIdSearch))
        bicode = bicode.Where(Function(f) f.SubjectName.Contains(SubjectNameSearch))
        If StatusSearch <> "" Then
            bicode = bicode.Where(Function(f) f.Status.StartsWith(StatusSearch))
        End If


        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    bicode = bicode.OrderBy(Function(f) f.BICode)
                Else
                    bicode = bicode.OrderByDescending(Function(f) f.BICode)
                End If
            Case 1
                If sortOrder = "asc" Then
                    bicode = bicode.OrderBy(Function(f) f.SubjectStatusId)
                Else
                    bicode = bicode.OrderByDescending(Function(f) f.SubjectStatusId)
                End If
            Case 2
                If sortOrder = "asc" Then
                    bicode = bicode.OrderBy(Function(f) f.SubjectName)
                Else
                    bicode = bicode.OrderByDescending(Function(f) f.SubjectName)
                End If
        End Select

        Dim result As New List(Of String())
        For Each data As MsBICode In bicode
            result.Add({data.BICode, data.SubjectStatusId, data.SubjectName, data.Status, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function


End Class