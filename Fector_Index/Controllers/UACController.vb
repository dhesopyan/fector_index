Imports System.Data.Objects.SqlClient
Imports System.Net

Public Class UACController
    Inherits System.Web.Mvc.Controller

    Dim UseLimitOption As IEnumerable(Of SelectListItem) = {New SelectListItem With {.Text = "NO", .Value = False}, _
                                                        New SelectListItem With {.Text = "YES", .Value = True}}

    ' GET: /UAC/Approval
    <Authorize> _
    Public Function Approval() As ActionResult
        ViewBag.Breadcrumb = "Home > UAC > Approval"
        Return View("Index")
    End Function

    '
    ' GET: /UAC/Index
    <Authorize> _
    Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > UAC"
        Return View()
    End Function

    '
    ' Get: /UAC/Create
    <Authorize> _
    Public Function Create() As ActionResult
        Dim model As New MsUACViewModel
        Dim db As New FectorEntities

        ViewData("Title") = "Create UAC"
        ViewBag.Breadcrumb = "Home > UAC > Create"
        model.AvailableUACLists = db.SubMenus.OrderBy(Function(f) f.Ordering).Select(Function(f) SqlFunctions.StringConvert(f.SubMenuId)).ToArray

        Return View("Detail", model)
    End Function

    '
    ' Post: /UAC/Create
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Create(model As MsUACViewModel) As ActionResult
        Dim db As New FectorEntities()

        ViewData("Title") = "Create UAC"
        ViewBag.Breadcrumb = "Home > UAC > Create"
        ViewBag.ErrorMessage = ""

        If IsNothing(model.SelectedUACLists) OrElse model.SelectedUACLists.Length = 0 Then
            ModelState.AddModelError("UAC", "Please fill at least one access")
        End If

        If ModelState.IsValid Then
            Dim userlevelName As String = model.Description.ToUpper
            Dim useLimit As Boolean = model.FlagUseLimit
            Dim existingUserlevel = db.UserLevels.Where(Function(f) f.Description = userlevelName).SingleOrDefault

            If IsNothing(existingUserlevel) Then
                Dim newUserlevel As New MsUserlevel
                With newUserlevel
                    .Description = userlevelName
                    .FlagUseLimit = useLimit
                    .EditBy = User.Identity.Name
                    .EditDate = DateTime.Now
                    .ApproveBy = ""
                    .ApproveDate = New DateTime(1900, 1, 1)
                    .Status = "CREATE - PENDING"
                End With
                db.UserLevels.Add(newUserlevel)
                db.SaveChanges()

                Dim newId As Integer = newUserlevel.UserlevelId

                For i As Integer = 0 To model.SelectedUACLists.Length - 1
                    Dim newMapping As New MsUserAccess
                    With newMapping
                        .UserLevelId = newId
                        .SubMenuId = model.SelectedUACLists(i)
                    End With
                    db.UserAccesses.Add(newMapping)
                Next
                db.SaveChanges()

                LogTransaction.WriteLog(User.Identity.Name, "CREATE UAC", userlevelName, db)

                Return RedirectToAction("Index", "UAC")
            Else
                ModelState.AddModelError("Description", "User Level has already registered")
            End If
        End If

        Return View("Detail", model)
    End Function

    '
    ' Post: /DocumentTransaction/Edit
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Edit(model As MsUACViewModel) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Edit User Level"
        ViewBag.Breadcrumb = "Home > Document Transaction > Edit"

        If ModelState.IsValid Then
            Dim userlevelName As String = model.Description.ToUpper
            Dim useLimit As Boolean = model.FlagUseLimit
            Dim SelectedUAC As String() = model.SelectedUACLists

            Dim editedUAC As MsUserlevel = MsUserlevel.GetAllUserLevel(model.UserlevelId, db)
            With editedUAC
                .Description = userlevelName
                .FlagUseLimit = useLimit
                .EditBy = User.Identity.Name
                .EditDate = DateTime.Now
                .ApproveBy = ""
                .ApproveDate = New DateTime(1900, 1, 1)
                .Status = "EDIT - PENDING"
            End With
            db.Entry(editedUAC).State = EntityState.Modified
            db.SaveChanges()

            For Each ms As MsUserAccess In MsUserAccess.GetUAC(model.UserlevelId, db)
                db.UserAccesses.Remove(ms)
            Next
            db.SaveChanges()

            For i As Integer = 0 To SelectedUAC.Length - 1
                Dim newUAC As New MsUserAccess
                With newUAC
                    .UserLevelId = model.UserlevelId
                    .SubMenuId = SelectedUAC(i)
                End With
                db.UserAccesses.Add(newUAC)
            Next
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "EDIT UAC", model.UserlevelId, db)

            Return RedirectToAction("Index")
        Else
            Return View("Detail", model)
        End If
    End Function

    '
    ' POST: /UAC/Delete
    <Authorize> _
    <HttpPost> _
    Public Function Delete() As ActionResult
        Dim db As New FectorEntities()
        Dim id As Integer = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedUserLevel = MsUserlevel.GetAllUserLevel(id, db)

        If IsNothing(editedUserLevel) Then
            Return New HttpNotFoundResult
        End If

        editedUserLevel.Status = "DELETE - PENDING"
        editedUserLevel.EditBy = User.Identity.Name
        editedUserLevel.EditDate = Now
        db.Entry(editedUserLevel).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "DELETE UAC", editedUserLevel.UserlevelId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /UAC/Inactive
    <Authorize> _
    <HttpPost> _
    Public Function Inactive() As ActionResult
        Dim db As New FectorEntities()
        Dim id As Integer = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedUserLevel As MsUserlevel = MsUserlevel.GetAllUserLevel(id, db)

        If IsNothing(editedUserLevel) Then
            Return New HttpNotFoundResult
        End If

        editedUserLevel.Status = "INACTIVE - PENDING"
        editedUserLevel.EditBy = User.Identity.Name
        editedUserLevel.EditDate = Now
        db.Entry(editedUserLevel).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "INACTIVE UAC", editedUserLevel.UserlevelId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /DocumentTransaction/Active
    <Authorize> _
    <HttpPost> _
    Public Function Active() As ActionResult
        Dim db As New FectorEntities()
        Dim id As Integer = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedUserLevel As MsUserlevel = MsUserlevel.GetAllUserLevel(id, db)

        If IsNothing(editedUserLevel) Then
            Return New HttpNotFoundResult
        End If

        editedUserLevel.Status = "ACTIVE - PENDING"
        editedUserLevel.EditBy = User.Identity.Name
        editedUserLevel.EditDate = Now
        db.Entry(editedUserLevel).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "ACTIVE UAC", editedUserLevel.UserlevelId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' Get: /UAC/Edit
    <Authorize> _
    Public Function Edit(ByVal id As Integer) As ActionResult
        Dim model As New MsUACViewModel
        Dim db As New FectorEntities

        If IsNothing(id) OrElse id = 0 Then
            Return RedirectToAction("Index", "UAC")
        End If

        If IsNothing(model.UserlevelId) Or model.UserlevelId = 0 Then
            model = MsUACViewModel.GetUAC(id, db)
        End If

        ViewData("Title") = "Edit UAC"
        ViewBag.Breadcrumb = "Home > UAC > Edit"
        model.AvailableUACLists = db.SubMenus.OrderBy(Function(f) f.Ordering).Select(Function(f) SqlFunctions.StringConvert(f.SubMenuId)).ToArray
        model.SelectedUACLists = db.UserAccesses.Where(Function(f) f.UserLevelId = id).Select(Function(f) SqlFunctions.StringConvert(f.SubMenuId)).ToArray

        Return View("Detail", model)
    End Function

    ' GET: /UAC/Process
    <Authorize> _
    Function Process(ByVal id As Integer) As ActionResult
        Dim model As New MsUACViewModel
        Dim db As New FectorEntities

        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        If IsNothing(model.UserlevelId) OrElse model.UserlevelId = 0 Then
            model = MsUACViewModel.GetUAC(id, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        ViewData("Title") = "Edit UAC"
        ViewBag.Breadcrumb = "Home > UAC > Approval > Process"
        model.AvailableUACLists = db.SubMenus.OrderBy(Function(f) f.Ordering).Select(Function(f) SqlFunctions.StringConvert(f.SubMenuId)).ToArray
        model.SelectedUACLists = db.UserAccesses.Where(Function(f) f.UserLevelId = id).Select(Function(f) SqlFunctions.StringConvert(f.SubMenuId)).ToArray

        Return View("Detail", model)
    End Function

    ' POST: /UAC/Process
    <Authorize> _
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    Function Process(model As MsUACViewModel, approvalButton As String) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Process UAC"
        ViewBag.Breadcrumb = "Home > UAC > Approval > Process"

        Dim uac = MsUserlevel.GetAllUserLevel(model.UserlevelId, db)
        Dim temp = MsUserAccess.GetUAC(model.UserlevelId, db)

        uac.ApproveBy = User.Identity.Name
        uac.ApproveDate = Now
        Dim Status As String = ""
        If approvalButton = "Approve" Then
            Dim flagDelete As Boolean = False
            Select Case uac.Status
                Case "ACTIVE - PENDING", "EDIT - PENDING", "CREATE - PENDING"
                    uac.Status = "ACTIVE"
                Case "INACTIVE - PENDING"
                    uac.Status = "INACTIVE"
                Case "DELETE - PENDING"
                    uac.Status = "INACTIVE"
                    'flagDelete = True
            End Select

            'If flagDelete Then
            '    For i As Integer = 0 To temp.Count - 1
            '        db.UserAccesses.Remove(temp(i))
            '    Next
            '    db.UserLevels.Remove(uac)
            'Else
            '    uac.ApproveBy = User.Identity.Name
            '    uac.ApproveDate = Now
            '    db.Entry(uac).State = EntityState.Modified
            'End If

            uac.ApproveBy = User.Identity.Name
            uac.ApproveDate = Now
            db.Entry(uac).State = EntityState.Modified
            Status = "APPROVE"
        Else
            uac.Status = "REJECTED"
            uac.ApproveBy = User.Identity.Name
            uac.ApproveDate = Now
            db.Entry(uac).State = EntityState.Modified
            Status = "REJECT"
        End If

        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, Status & " UAC", model.UserlevelId, db)

        Return RedirectToAction("Approval")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTUACAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim uacs = From s In db.UserLevels Select s
        Return ReturnUACDataTable(param, uacs)
    End Function

    <Authorize> _
   <HttpGet> _
    Public Function DTUACApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim uacs = From s In db.UserLevels
                   Where s.Status.Contains("PENDING") And s.EditBy <> User.Identity.Name
                   Select s
        Return ReturnUACDataTable(param, uacs)
    End Function

    Private Function ReturnUACDataTable(param As jQueryDataTableParamModel, uacs As IQueryable(Of MsUserlevel)) As JsonResult
        Dim totalRecords As Integer = uacs.Count
        Dim displayRecord As Integer = 0

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim userLevelSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim statusSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))
        uacs = uacs.Where(Function(f) f.Description.Contains(userLevelSearch))
        If statusSearch <> "" Then
            uacs = uacs.Where(Function(f) f.Status.Contains(statusSearch))
        End If

        
        displayRecord = uacs.Count

        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0, 1
                If sortOrder = "asc" Then
                    uacs = uacs.OrderBy(Function(f) f.Description)
                Else
                    uacs = uacs.OrderByDescending(Function(f) f.Description)
                End If
            Case 2
                If sortOrder = "asc" Then
                    uacs = uacs.OrderBy(Function(f) f.FlagUseLimit)
                Else
                    uacs = uacs.OrderByDescending(Function(f) f.FlagUseLimit)
                End If
            Case 3
                If sortOrder = "asc" Then
                    uacs = uacs.OrderBy(Function(f) f.Status)
                Else
                    uacs = uacs.OrderByDescending(Function(f) f.Status)
                End If
        End Select

        Dim showed = (From uac In uacs.Skip(iDisplayStart).Take(iDisplayLength)
                       Select uac).ToList
        Dim result As New List(Of String())
        For Each uac As MsUserlevel In showed
            result.Add({uac.UserlevelId, uac.Description, uac.FlagUseLimit, uac.Status, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = displayRecord,
                      .aaData = result},
                      JsonRequestBehavior.AllowGet)
    End Function

End Class