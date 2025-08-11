Imports System.Net
Imports System.Data.Objects.SqlClient

Public Class UserController
    Inherits System.Web.Mvc.Controller

    ' GET: /User/Login
    Public Function Login(returnUrl As String) As ActionResult
        Dim model As New LoginViewModel
        model.ReturnUrl = returnUrl
        Return View("Login", model)
    End Function

    ' POST: /User/Login
    <HttpPost> _
    <AllowAnonymous> _
    <ValidateAntiForgeryToken> _
    Public Function Login(model As LoginViewModel, returnUrl As String) As ActionResult
        ViewBag.ErrorMessage = ""
        returnUrl = model.ReturnUrl

        If ModelState.IsValid Then
            Dim username As String = model.Username
            Dim password As String = model.Password

            Dim db As New FectorEntities()
            If Not MsUser.isUserBlock(username) Then
                If MsUser.Authenticate(username, password) Then
                    If IsNothing(Session("SessionId")) OrElse Session("SessionId") = "" Then
                        Session("SessionId") = LogUser.createSessionId
                    End If

                    If LogUser.isLoginOK(username, Session("SessionId")) Then
                        Call LogUser.LoginUser(username, Session("SessionId"))
                        FormsAuthentication.SetAuthCookie(username, False)

                        Session("BranchId") = db.Users.Find(username).BranchId

                        If MsUser.isPasswordExpired(username) Then
                            Session("HasToChangePassword") = True
                            Return RedirectToAction("ChangePassword", "User")
                        Else
                            If Url.IsLocalUrl(returnUrl) AndAlso Not returnUrl.ToLower.Contains("/user/logout") AndAlso returnUrl.Length > 1 AndAlso returnUrl.StartsWith("/") AndAlso Not returnUrl.StartsWith("//") AndAlso Not returnUrl.StartsWith("/\") Then
                                Return Redirect(returnUrl)
                            Else
                                Return RedirectToAction("Index", "Home")
                            End If
                        End If
                    Else
                        ViewBag.ErrorMessage = "User has already login in another workstation"
                    End If
                Else
                    Call MsUser.CaptureIncorrectPassword(username)
                    ViewBag.ErrorMessage = "Invalid username or password"
                End If
            Else
                ViewBag.ErrorMessage = "User has been blocked due to maximum incorrect password login attempt"
            End If
        Else
            ViewBag.ErrorMessage = "Invalid username or password"
        End If

        Return View("Login", model)
    End Function

    ' GET: /User/Logout
    <Authorize> _
    Public Function Logout(Optional returnUrl As String = "") As ActionResult
        Dim db As New FectorEntities()
        Call LogUser.LogoutUser(User.Identity.Name)

        FormsAuthentication.SignOut()

        'If (Not String.IsNullOrEmpty(returnUrl) And Not Me.Request.UrlReferrer Is Nothing And Not String.IsNullOrEmpty(Me.Request.UrlReferrer.PathAndQuery)) Then
        '    returnUrl = Me.Request.UrlReferrer.PathAndQuery
        'End If
        Return RedirectToAction("Login", "User", New With {.returnUrl = returnUrl})
    End Function

    ' GET: /User/ChangePassword
    <Authorize> _
    Public Function ChangePassword() As ActionResult
        Dim model As New ChangePasswordViewModel
        ViewBag.Breadcrumb = "Home > Change Password"
        Return View("ChangePassword", model)
    End Function

    ' POST: /User/ChangePassword
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function ChangePassword(model As ChangePasswordViewModel) As ActionResult
        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim oldpwd As String = model.OldPassword
            Dim newpwd As String = model.NewPassword
            Dim confpwd As String = model.ConfirmNewPassword
            Dim username As String = User.Identity.Name

            Dim db As New FectorEntities()
            If MsUser.Authenticate(username, oldpwd) Then
                If oldpwd = newpwd Then
                    'ModelState.AddModelError("NewPassword", "Please input different new password")
                    ViewBag.ErrorMessage = "Please input different new password"
                Else
                    If newpwd <> confpwd Then
                        'ModelState.AddModelError("ConfirmNewPassword", "New password and confirmation has to be the same")
                        ViewBag.ErrorMessage = "New password and confirmation has to be the same"
                    Else
                        Dim errMsg As String = ""
                        If Not MsUser.isPasswordPolicyOk(newpwd, errMsg) Then
                            ViewBag.ErrorMessage = errMsg
                        Else
                            Call MsUser.ChangePassword(User.Identity.Name, newpwd)
                            Session("HasToChangePassword") = False

                            LogTransaction.WriteLog(User.Identity.Name, "CHANGE USER PASSWORD", User.Identity.Name, db)

                            Return RedirectToAction("Index", "Home")
                        End If
                    End If
                End If
            Else
                'ModelState.AddModelError("OldPassword", "Invalid old password")
                ViewBag.ErrorMessage = "Invalid old password"
            End If
        End If

        Return View("ChangePassword", model)
    End Function

    ' GET: /User/Index
    <Authorize> _
    Public Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > User"
        Return View("Index")
    End Function

    ' GET: /User/Approval
    <Authorize> _
    Public Function Approval() As ActionResult
        ViewBag.Breadcrumb = "Home > User > Approval"
        Return View("Index")
    End Function

    ' GET: /User/Unblock
    <Authorize> _
    Public Function Unblock() As ActionResult
        ViewBag.Breadcrumb = "Home> Unblock User"
        Return View("Unblock")
    End Function

    '
    ' POST: /User/UnblockUser
    <Authorize> _
    <HttpPost> _
    Public Function UnblockUser() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editeduser = db.Users.Find(id)

        If IsNothing(editeduser) Then
            Return New HttpNotFoundResult
        End If

        editeduser.PasswordTry = 0
        db.Entry(editeduser).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "UNBLOCK USER", editeduser.UserId, db)

        Return RedirectToAction("Unblock")
    End Function

    ' GET: /User/Unlock
    <Authorize> _
    Public Function Unlock() As ActionResult
        ViewBag.Breadcrumb = "Home > Unlock User"
        Return View("Unlock")
    End Function

    '
    ' POST: /User/UnlockUser
    <Authorize> _
    <HttpPost> _
    Public Function UnlockUser() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedlog = db.LogUsers.Find(CDec(id))

        If IsNothing(editedlog) Then
            Return New HttpNotFoundResult
        End If

        editedlog.LogoutTime = Now
        editedlog.Status = False
        db.Entry(editedlog).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "UNLOCK USER", editedlog.UserId, db)

        Return RedirectToAction("Unlock")
    End Function

    '
    ' POST: /User/Delete
    <Authorize> _
    <HttpPost> _
    Public Function Delete() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editeduser = db.Users.Find(id)

        If IsNothing(editeduser) Then
            Return New HttpNotFoundResult
        End If

        editeduser.Status = "DELETE - PENDING"
        editeduser.EditBy = User.Identity.Name
        editeduser.EditDate = Now
        db.Entry(editeduser).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "DELETE USER", editeduser.UserId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /User/ResetPassword
    <Authorize> _
    <HttpPost> _
    Public Function ResetPassword() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editeduser = db.Users.Find(id)

        If IsNothing(editeduser) Then
            Return New HttpNotFoundResult
        End If

        Dim defaultpassword As String = MsSetting.GetSetting("Password", "Default", "", 1)
        editeduser.Password = MsUser.getMD5Hash(defaultpassword + editeduser.Salt)
        editeduser.PasswordLastChange = New DateTime(1900, 1, 1)
        editeduser.Status = "ACTIVE"
        editeduser.ApproveBy = User.Identity.Name
        editeduser.ApproveDate = Now

        db.Entry(editeduser).State = EntityState.Modified
        db.SaveChanges()

        'LogTransaction.WriteLog(User.Identity.Name, "APPROVE USER", editeduser.UserId, db)

        'editeduser.Status = "RESET PWD - PENDING"
        'editeduser.EditBy = User.Identity.Name
        'editeduser.EditDate = Now
        'db.Entry(editeduser).State = EntityState.Modified
        'db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "RESET USER PASSWORD", editeduser.UserId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /User/Inactive
    <Authorize> _
    <HttpPost> _
    Public Function Inactive() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editeduser = db.Users.Find(id)

        If IsNothing(editeduser) Then
            Return New HttpNotFoundResult
        End If

        editeduser.Status = "INACTIVE"
        editeduser.EditBy = User.Identity.Name
        editeduser.EditDate = Now
        db.Entry(editeduser).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "INACTIVE USER", editeduser.UserId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /User/Active
    <Authorize> _
    <HttpPost> _
    Public Function Active() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editeduser = db.Users.Find(id)

        If IsNothing(editeduser) Then
            Return New HttpNotFoundResult
        End If

        editeduser.Status = "ACTIVE"
        editeduser.EditBy = User.Identity.Name
        editeduser.EditDate = Now
        db.Entry(editeduser).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "ACTIVE USER", editeduser.UserId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /User/Approve
    <Authorize> _
    <HttpPost> _
    Public Function Approve() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editeduser = MsUser.GetAllUser(id, db)

        If IsNothing(editeduser) Then
            Return New HttpNotFoundResult
        End If

        Dim flagDelete As Boolean = False
        Dim flagResetPassword As Boolean = False
        Select Case editeduser.Status
            Case "ACTIVE - PENDING", "EDIT - PENDING", "CREATE - PENDING"
                editeduser.Status = "ACTIVE"
            Case "INACTIVE - PENDING"
                editeduser.Status = "INACTIVE"
            Case "DELETE - PENDING"
                'flagDelete = True
                editeduser.Status = "INACTIVE"
            Case "RESET PWD - PENDING"
                flagResetPassword = True
        End Select

        'If flagDelete Then
        '    db.Users.Remove(editeduser)
        '    Try
        '        db.SaveChanges()
        '    Catch ex As Exception
        '        TempData("DeleteUser") = "This data is used"
        '        Return RedirectToAction("Approval")
        '    End Try
        'ElseIf flagResetPassword Then
        If flagResetPassword Then
            Dim defaultpassword As String = MsSetting.GetSetting("Password", "Default", "", 1)
            editeduser.Password = MsUser.getMD5Hash(defaultpassword + editeduser.Salt)
            editeduser.PasswordLastChange = New DateTime(1900, 1, 1)
            editeduser.Status = "ACTIVE"
            editeduser.ApproveBy = User.Identity.Name
            editeduser.ApproveDate = Now

            db.Entry(editeduser).State = EntityState.Modified
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "APPROVE USER", editeduser.UserId, db)
        Else
            editeduser.ApproveBy = User.Identity.Name
            editeduser.ApproveDate = Now
            db.Entry(editeduser).State = EntityState.Modified
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "APPROVE USER", editeduser.UserId, db)
        End If


        Return RedirectToAction("Approval")
    End Function

    '
    ' POST: /User/Reject
    <Authorize> _
    <HttpPost> _
    Public Function Reject() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editeduser = db.Users.Find(id)

        If IsNothing(editeduser) Then
            Return New HttpNotFoundResult
        End If

        editeduser.Status = "REJECTED"
        editeduser.ApproveBy = User.Identity.Name
        editeduser.ApproveDate = Now
        db.Entry(editeduser).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "REJECT USER", editeduser.UserId, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' Get: /User/Edit
    <Authorize> _
    Public Function Edit(ByVal id As String) As ActionResult
        Dim model As New MsUser
        Dim db As New FectorEntities

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        If IsNothing(model.UserId) Then
            model = MsUser.GetAllUser(id, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        ViewData("Title") = "Edit User"
        ViewBag.Breadcrumb = "Home > User > Edit"
        ViewBag.UserLevel = New SelectList(db.UserLevels.Where(Function(f) f.Status = "ACTIVE"), "UserlevelId", "Description")
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.Currency = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")

        Return View("Detail", model)
    End Function

    '
    ' Post: /User/Edit
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Edit(model As MsUser) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Edit User"
        ViewBag.Breadcrumb = "Home > User > Edit"
        ViewBag.UserLevel = New SelectList(db.UserLevels.Where(Function(f) f.Status = "ACTIVE"), "UserlevelId", "Description")
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.Currency = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")
        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim fullname As String = model.Fullname
            Dim branch As String = model.BranchId
            Dim userlevel As String = model.UserlevelId
            Dim transactionlimitcurrency As String = model.TransactionLimitCurrency
            Dim transactionlimit As Decimal = model.TransactionLimit

            Dim editeduser As MsUser = MsUser.GetAllUser(model.UserId, db)
            With editeduser
                .Fullname = fullname
                .BranchId = branch
                .UserlevelId = userlevel
                .TransactionLimitCurrency = transactionlimitcurrency
                .TransactionLimit = transactionlimit
                .EditBy = User.Identity.Name
                .EditDate = DateTime.Now
                .ApproveBy = ""
                .ApproveDate = New DateTime(1900, 1, 1)
                .Status = "EDIT - PENDING"
            End With
            db.Entry(editeduser).State = EntityState.Modified
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "EDIT USER", model.UserId, db)

            Return RedirectToAction("Index")
        Else
            Return View("Detail", model)
        End If
    End Function

    '
    ' Get: /User/Create
    <Authorize> _
    Public Function Create() As ActionResult
        Dim model As New MsUser
        Dim db As New FectorEntities

        Dim DefaultPass = (From a In db.Settings
                           Where a.Key1 = "Password" And a.Key2 = "Default"
                           Select a).ToList

        ViewData("Title") = "Create User"
        ViewBag.Breadcrumb = "Home > User > Create"
        ViewBag.UserLevel = New SelectList(db.UserLevels.Where(Function(f) f.Status = "ACTIVE"), "UserlevelId", "Description")
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.Currency = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")
        ViewBag.DefaultPass = DefaultPass.Item(0).Value1

        model.TransactionLimitCurrency = "USD"

        Return View("Detail", model)
    End Function

    '
    ' Post: /User/Create
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Create(model As MsUser) As ActionResult
        Dim db As New FectorEntities()

        ViewData("Title") = "Create User"
        ViewBag.Breadcrumb = "Home > User > Create"
        ViewBag.UserLevel = New SelectList(db.UserLevels.Where(Function(f) f.Status = "ACTIVE"), "UserlevelId", "Description")
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.Currency = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")
        ViewBag.ErrorMessage = ""

        If IsNothing(model.TransactionLimit) Then
            model.TransactionLimit = 0.0
        End If

        If ModelState.IsValid Then
            Dim username As String = model.UserId
            Dim fullname As String = model.Fullname
            Dim branch As String = model.BranchId
            Dim userlevel As String = model.UserlevelId
            Dim transactionlimitcurrency As String = model.TransactionLimitCurrency
            Dim transactionlimit As Decimal = model.TransactionLimit
            Dim defaultpassword As String = MsSetting.GetSetting("Password", "Default", "", 1)
            Dim salt As String = MsUser.getMD5Hash(DateTime.Now.Ticks.ToString)
            Dim existingUser = MsUser.GetAllUser(username, db)

            If IsNothing(existingUser) Then
                Dim newUser As New MsUser
                With newUser
                    .UserId = username
                    .Password = MsUser.getMD5Hash(defaultpassword + salt)
                    .Salt = salt
                    .Fullname = fullname
                    .BranchId = branch
                    .UserlevelId = userlevel
                    .TransactionLimitCurrency = transactionlimitcurrency
                    .TransactionLimit = transactionlimit
                    .PasswordLastChange = New DateTime(1900, 1, 1)
                    .PasswordTry = 0
                    .EditBy = User.Identity.Name
                    .EditDate = DateTime.Now
                    .ApproveBy = ""
                    .ApproveDate = New DateTime(1900, 1, 1)
                    .Status = "CREATE - PENDING"
                End With
                db.Users.Add(newUser)
                db.SaveChanges()

                LogTransaction.WriteLog(User.Identity.Name, "CREATE USER", username, db)

                Return RedirectToAction("Index", "User")
            Else
                ModelState.AddModelError("Userid", "Username has already registered")
            End If
        End If

        Return View("Detail", model)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTUserAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim users = From s In db.Users.Include("UserLevel").Include("Branch") Select s
        Return ReturnUserDataTable(param, users, "Index")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTUserApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim users = From s In db.Users.Include("UserLevel").Include("Branch")
                    Where s.Status.Contains("PENDING") And s.EditBy <> User.Identity.Name
                    Select s
        Return ReturnUserDataTable(param, users, "Approval")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTUserUnblockAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim maxAttemp As Integer = MsSetting.GetSetting("Password", "Incorrect", "", 1)
        Dim users = From s In db.Users.Include("UserLevel").Include("Branch")
                    Where s.PasswordTry >= maxAttemp
                    Select s
        Return ReturnUserUnblockDataTable(param, users)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTUserUnlockAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Return ReturnUserUnlockDataTable(param)
    End Function


    Private Function ReturnUserDataTable(param As jQueryDataTableParamModel, users As IQueryable(Of MsUser), mode As String) As JsonResult
        Dim totalRecords = users.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim usernameSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim fullNameSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim levelSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        Dim branchSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))
        Dim limitSearch As String = If(IsNothing(Request("sSearch_4")), "", Request("sSearch_4"))
        Dim statusSearch As String = If(IsNothing(Request("sSearch_5")), "", Request("sSearch_5"))
        users = users.Where(Function(f) f.UserId.Contains(usernameSearch))
        users = users.Where(Function(f) f.Fullname.Contains(fullNameSearch))
        users = users.Where(Function(f) f.Userlevel.Description.Contains(levelSearch))
        users = users.Where(Function(f) f.Branch.Name.Contains(branchSearch) Or f.Branch.BranchId.Contains(branchSearch))
        users = users.Where(Function(f) SqlFunctions.StringConvert(f.TransactionLimit).Contains(limitSearch) Or f.TransactionLimitCurrency.Contains(limitSearch))

        If statusSearch = "" Then
            If mode = "Index" Then
                users = users.Where(Function(f) f.Status = "ACTIVE")
            End If
        Else
            If statusSearch <> "ALL" Then
                users = users.Where(Function(f) f.Status = statusSearch)
            End If
        End If

        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.UserId)
                Else
                    users = users.OrderByDescending(Function(f) f.UserId)
                End If
            Case 1
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.Fullname)
                Else
                    users = users.OrderByDescending(Function(f) f.Fullname)
                End If
            Case 2
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.Userlevel.Description)
                Else
                    users = users.OrderByDescending(Function(f) f.Userlevel.Description)
                End If
            Case 3
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.Branch.BranchId)
                Else
                    users = users.OrderByDescending(Function(f) f.Branch.BranchId)
                End If
            Case 4
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.TransactionLimit)
                Else
                    users = users.OrderByDescending(Function(f) f.TransactionLimit)
                End If
            Case 5
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.Status)
                Else
                    users = users.OrderByDescending(Function(f) f.Status)
                End If
            Case Else
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.UserId)
                Else
                    users = users.OrderByDescending(Function(f) f.UserId)
                End If
        End Select

        Dim result As New List(Of String())
        For Each user As MsUser In users
            result.Add({user.UserId, user.Fullname, user.Userlevel.Description, user.Branch.BranchDisplay, user.LimitDisplay, user.Status, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

    Private Function ReturnUserUnblockDataTable(param As jQueryDataTableParamModel, users As IQueryable(Of MsUser)) As JsonResult
        Dim totalRecords = users.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim usernameSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim fullNameSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim levelSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        Dim branchSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))
        Dim attemptSearch As String = If(IsNothing(Request("sSearch_4")), "", Request("sSearch_4"))
        users = users.Where(Function(f) f.UserId.Contains(usernameSearch))
        users = users.Where(Function(f) f.Fullname.Contains(fullNameSearch))
        users = users.Where(Function(f) f.Userlevel.Description.Contains(levelSearch))
        users = users.Where(Function(f) f.Branch.Name.Contains(branchSearch) Or f.Branch.BranchId.Contains(branchSearch))
        users = users.Where(Function(f) SqlFunctions.StringConvert(f.PasswordTry).Contains(attemptSearch))

        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.UserId)
                Else
                    users = users.OrderByDescending(Function(f) f.UserId)
                End If
            Case 1
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.Fullname)
                Else
                    users = users.OrderByDescending(Function(f) f.Fullname)
                End If
            Case 2
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.Userlevel.Description)
                Else
                    users = users.OrderByDescending(Function(f) f.Userlevel.Description)
                End If
            Case 3
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.Branch.BranchId)
                Else
                    users = users.OrderByDescending(Function(f) f.Branch.BranchId)
                End If
            Case 4
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.PasswordTry)
                Else
                    users = users.OrderByDescending(Function(f) f.PasswordTry)
                End If
            Case Else
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.UserId)
                Else
                    users = users.OrderByDescending(Function(f) f.UserId)
                End If
        End Select

        Dim result As New List(Of String())
        For Each user As MsUser In users
            result.Add({user.UserId, user.Fullname, user.Userlevel.Description, user.Branch.BranchDisplay, user.PasswordTry, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

    Class UserLogData
        Public LogId As String
        Public UserId As String
        Public Fullname As String
        Public Userlevel As String
        Public Branch As String
        Public LoginTime As DateTime
    End Class

    Private Function ReturnUserUnlockDataTable(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()

        Dim users = From lu In db.LogUsers.Include("User").Include("User.Branch").Include("User.Userlevel")
                    Where lu.Status = True
                    Select New UserLogData With {.LogId = lu.LogId, .UserId = lu.UserId,
                                                  .Fullname = lu.User.Fullname, .Userlevel = lu.User.Userlevel.Description,
                                                  .Branch = lu.User.Branch.BranchId & " - " & lu.User.Branch.name, .LoginTime = lu.LoginTime
                                                 }

        Dim totalRecords = users.Count
        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim usernameSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim fullNameSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        Dim levelSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))
        Dim branchSearch As String = If(IsNothing(Request("sSearch_4")), "", Request("sSearch_4"))
        Dim loginTimeSearch As String = If(IsNothing(Request("sSearch_5")), "", Request("sSearch_5"))
        users = users.Where(Function(f) f.UserId.Contains(usernameSearch))
        users = users.Where(Function(f) f.Fullname.Contains(fullNameSearch))
        users = users.Where(Function(f) f.Userlevel.Contains(levelSearch))
        users = users.Where(Function(f) f.Branch.Contains(branchSearch))
        If loginTimeSearch <> "" Then
            Dim dTDate As Date
            dTDate = AppHelper.dateTimeConvert(loginTimeSearch)
            users = users.Where(Function(f) f.LoginTime.Day = dTDate.Day And f.LoginTime.Month = dTDate.Month And f.LoginTime.Year = dTDate.Year And f.LoginTime.Hour = dTDate.Hour And f.LoginTime.Minute = dTDate.Minute And f.LoginTime.Second = dTDate.Second)

            'users = users.Where(Function(f) f.LoginTime = loginTimeSearch)
            'users = users.Where(Function(f) f.LoginTime.Day = loginTimeSearch Or f.LoginTime.Month = loginTimeSearch Or f.LoginTime.Year = loginTimeSearch)
        End If
        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.UserId)
                Else
                    users = users.OrderByDescending(Function(f) f.UserId)
                End If
            Case 1
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.Fullname)
                Else
                    users = users.OrderByDescending(Function(f) f.Fullname)
                End If
            Case 2
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.Userlevel)
                Else
                    users = users.OrderByDescending(Function(f) f.Userlevel)
                End If
            Case 3
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.Branch)
                Else
                    users = users.OrderByDescending(Function(f) f.Branch)
                End If
            Case 4
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.LoginTime)
                Else
                    users = users.OrderByDescending(Function(f) f.LoginTime)
                End If
            Case Else
                If sortOrder = "asc" Then
                    users = users.OrderBy(Function(f) f.UserId)
                Else
                    users = users.OrderByDescending(Function(f) f.UserId)
                End If
        End Select

        Dim result As New List(Of String())
        For Each user As UserLogData In users.OrderByDescending(Function(f) f.LoginTime)
            result.Add({user.LogId, user.UserId, user.Fullname, user.Userlevel, user.Branch, user.LoginTime.ToString("dd/MM/yyyy HH:mm:ss"), ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

    <HttpPost> _
    Public Function JSONCheckUserLevelLimit(ByVal Id As Integer) As ActionResult
        Return Json(New With {.useLimit = CheckUserLevelLimit(Id)})
    End Function

    Private Function CheckUserLevelLimit(ByVal Id As Integer) As Boolean
        Dim db As New FectorEntities
        If Id = -1 Then
            Return False
        End If

        Dim useLimit As Boolean = db.UserLevels.Find(Id).FlagUseLimit

        If IsNothing(useLimit) Then
            useLimit = False
        End If

        Return useLimit
    End Function

End Class