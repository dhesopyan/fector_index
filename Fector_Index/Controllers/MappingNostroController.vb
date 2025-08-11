Imports System.Net

Public Class MappingNostroController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /MappingNostro
    Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > Mapping Nostro"
        Return View("Index")
    End Function

    ' GET: /MappingNostro/Approval
    <Authorize> _
    Public Function Approval() As ActionResult
        ViewBag.Breadcrumb = "Home > Mapping Nostro > Approval"
        Return View("Index")
    End Function

    '
    ' Get: /MappingNostro/Create
    <Authorize> _
    Public Function Create() As ActionResult
        Dim model As New MsMappingNostro
        Dim db As New FectorEntities

        ViewData("Title") = "Create Mapping Nostro"
        ViewBag.Breadcrumb = "Home > Mapping Nostro > Create"
        ViewBag.strBIC = New SelectList(db.Nostro, "BIC", "nostro")
        ViewBag.Currency = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")

        Return View("Detail", model)
    End Function

    '
    ' Post: /MappingNostro/Create
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Create(model As MsMappingNostro) As ActionResult
        Dim db As New FectorEntities()

        ViewData("Title") = "Create Mapping Nostro"
        ViewBag.Breadcrumb = "Home > Mapping Nostro > Create"
        ViewBag.strBIC = New SelectList(db.Nostro, "BIC", "nostro")
        ViewBag.Currency = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")
        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim BIC As String = model.BIC
            Dim CurrId As String = model.CurrID

            Dim existingmappingnostro = MsMappingNostro.GetExistingMappingNostro(BIC, CurrId, db, "CREATE")

            If IsNothing(existingmappingnostro) Then
                Dim newMappingNostro As New MsMappingNostro
                With newMappingNostro
                    .BIC = BIC
                    .CurrID = CurrId
                    .EditBy = User.Identity.Name
                    .EditDate = DateTime.Now
                    .ApproveBy = ""
                    .ApproveDate = New DateTime(1900, 1, 1)
                    .Status = "CREATE - PENDING"
                End With
                db.MappingNostro.Add(newMappingNostro)
                db.SaveChanges()

                LogTransaction.WriteLog(User.Identity.Name, "CREATE MAPPING NOSTRO", BIC & " - " & CurrId, db)

                Return RedirectToAction("Index")
            Else
                ModelState.AddModelError("MappingNostro", "Mapping Nostro has already registered")
            End If
        End If

        Return View("Detail", model)
    End Function

    '
    ' Get: /MappingNostro/Edit
    <Authorize> _
    Public Function Edit(ByVal id As String) As ActionResult
        Dim model As New MsMappingNostro
        Dim db As New FectorEntities

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return RedirectToAction("Index", "MappingNostro")
        End If

        If IsNothing(model.BIC) Or IsNothing(model.CurrID) Then
            model = MsMappingNostro.GetMappingNostro(id, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        ViewData("Title") = "Edit Mapping Nostro"
        ViewBag.Breadcrumb = "Home > Mapping Nostro > Edit"
        ViewBag.strBIC = New SelectList(db.Nostro, "BIC", "Nostro")
        ViewBag.Currency = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")

        Return View("Detail", model)
    End Function

    '
    ' Post: /MappingNostro/Edit
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Edit(model As MsMappingNostro) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Edit Mapping Nostro"
        ViewBag.Breadcrumb = "Home > Mapping Nostro > Edit"
        ViewBag.strBIC = New SelectList(db.Nostro, "BIC", "nostro")
        ViewBag.Currency = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")
        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim BIC As String = model.BIC
            Dim CurrID As String = model.CurrID

            Dim existingmappingnostro = MsMappingNostro.GetExistingMappingNostro(BIC, CurrID, db, "EDIT", model.ID)

            If IsNothing(existingmappingnostro) Then
                Dim editedmappingnostro As MsMappingNostro = MsMappingNostro.GetMappingNostro(model.ID, db)
                With editedmappingnostro
                    .BIC = BIC
                    .CurrID = CurrID
                    .EditBy = User.Identity.Name
                    .EditDate = DateTime.Now
                    .ApproveBy = ""
                    .ApproveDate = New DateTime(1900, 1, 1)
                    .Status = "EDIT - PENDING"
                End With
                db.Entry(editedmappingnostro).State = EntityState.Modified
                db.SaveChanges()

                LogTransaction.WriteLog(User.Identity.Name, "EDIT MAPPING NOSTRO", model.BIC & " - " & model.CurrID, db)
            Else
                ModelState.AddModelError("MappingNostro", "Mapping Nostro has already registered")
            End If

            Return RedirectToAction("Index")
        Else
            Return View("Detail", model)
        End If
    End Function

    '
    ' POST: /MappingNostro/Delete
    <Authorize> _
    <HttpPost> _
    Public Function Delete() As ActionResult
        Dim db As New FectorEntities()
        Dim id As Decimal = Request.Form("id")

        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedmappingnostro = db.MappingNostro.Find(id)

        If IsNothing(editedmappingnostro) Then
            Return New HttpNotFoundResult
        End If

        editedmappingnostro.Status = "DELETE - PENDING"
        editedmappingnostro.EditBy = User.Identity.Name
        editedmappingnostro.EditDate = Now
        db.Entry(editedmappingnostro).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "DELETE MAPPING NOSTRO", editedmappingnostro.BIC & " - " & editedmappingnostro.CurrID, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /MappingNostro/Approve
    <Authorize> _
    <HttpPost> _
    Public Function Approve() As ActionResult
        Dim db As New FectorEntities()
        Dim id As Decimal = Request.Form("id")

        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedmappingnostro = db.MappingNostro.Find(id)

        If IsNothing(editedmappingnostro) Then
            Return New HttpNotFoundResult
        End If

        Dim flagDelete As Boolean = False
        Select Case editedmappingnostro.Status
            Case "ACTIVE - PENDING", "EDIT - PENDING", "CREATE - PENDING"
                editedmappingnostro.Status = "ACTIVE"
            Case "INACTIVE - PENDING"
                editedmappingnostro.Status = "INACTIVE"
            Case "DELETE - PENDING"
                flagDelete = True
        End Select

        If flagDelete Then
            db.MappingNostro.Remove(editedmappingnostro)
            db.SaveChanges()
        Else
            editedmappingnostro.ApproveBy = User.Identity.Name
            editedmappingnostro.ApproveDate = Now
            db.Entry(editedmappingnostro).State = EntityState.Modified
            db.SaveChanges()
        End If

        LogTransaction.WriteLog(User.Identity.Name, "APPROVE MAPPING NOSTRO", editedmappingnostro.BIC & " - " & editedmappingnostro.CurrID, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' POST: /MappingNostro/Reject
    <Authorize> _
    <HttpPost> _
    Public Function Reject() As ActionResult
        Dim db As New FectorEntities()

        Dim id As Decimal = Request.Form("id")

        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedmappingnostro = db.MappingNostro.Find(id)

        If IsNothing(editedmappingnostro) Then
            Return New HttpNotFoundResult
        End If

        editedmappingnostro.Status = "REJECTED"
        editedmappingnostro.ApproveBy = User.Identity.Name
        editedmappingnostro.ApproveDate = Now
        db.Entry(editedmappingnostro).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "REJECT MAPPING NOSTRO", editedmappingnostro.BIC & " - " & editedmappingnostro.CurrID, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' POST: /MappingNostro/Inactive
    <Authorize> _
    <HttpPost> _
    Public Function Inactive() As ActionResult
        Dim db As New FectorEntities()

        Dim id As Decimal = Request.Form("id")

        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedmappingnostro = db.MappingNostro.Find(id)

        If IsNothing(editedmappingnostro) Then
            Return New HttpNotFoundResult
        End If

        editedmappingnostro.Status = "INACTIVE - PENDING"
        editedmappingnostro.EditBy = User.Identity.Name
        editedmappingnostro.EditDate = Now
        db.Entry(editedmappingnostro).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "INACTIVE MAPPING NOSTRO", editedmappingnostro.BIC & " - " & editedmappingnostro.CurrID, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /MappingNostro/Active
    <Authorize> _
    <HttpPost> _
    Public Function Active() As ActionResult
        Dim db As New FectorEntities()

        Dim id As Decimal = Request.Form("id")

        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedmappingnostro = db.MappingNostro.Find(id)

        If IsNothing(editedmappingnostro) Then
            Return New HttpNotFoundResult
        End If

        editedmappingnostro.Status = "ACTIVE - PENDING"
        editedmappingnostro.EditBy = User.Identity.Name
        editedmappingnostro.EditDate = Now
        db.Entry(editedmappingnostro).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "ACTIVE MAPPING NOSTRO", editedmappingnostro.BIC & " - " & editedmappingnostro.CurrID, db)

        Return RedirectToAction("Index")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTMappingNostroAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()

        Dim MappingNostro = From s In db.MappingNostro.Include("MsCurrency")
                            Select s

        Return ReturnMappingNostroDataTable(param, MappingNostro, "Index")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTMappingNostroApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()

        Dim MappingNostro = From s In db.MappingNostro.Include("MsCurrency")
                            Where s.Status.Contains("PENDING") And s.EditBy <> User.Identity.Name
                            Select s

        Return ReturnMappingNostroDataTable(param, MappingNostro, "Approval")
    End Function

    Private Function ReturnMappingNostroDataTable(param As jQueryDataTableParamModel, MappingNostro As IQueryable(Of MsMappingNostro), mode As String) As JsonResult
        Dim totalRecords = MappingNostro.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim BICSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim CurrIDSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim StatusSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))

        MappingNostro = MappingNostro.Where(Function(f) f.BIC.Contains(BICSearch))
        MappingNostro = MappingNostro.Where(Function(f) f.CurrID.Contains(CurrIDSearch))

        If StatusSearch <> "" Then
            MappingNostro = MappingNostro.Where(Function(f) f.Status = StatusSearch)
        End If

        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    MappingNostro = MappingNostro.OrderBy(Function(f) f.BIC)
                Else
                    MappingNostro = MappingNostro.OrderByDescending(Function(f) f.BIC)
                End If
            Case 1
                If sortOrder = "asc" Then
                    MappingNostro = MappingNostro.OrderBy(Function(f) f.CurrID)
                Else
                    MappingNostro = MappingNostro.OrderByDescending(Function(f) f.CurrID)
                End If
            Case 3
                If sortOrder = "asc" Then
                    MappingNostro = MappingNostro.OrderBy(Function(f) f.Status)
                Else
                    MappingNostro = MappingNostro.OrderByDescending(Function(f) f.Status)
                End If
        End Select

        Dim result As New List(Of String())

        For Each data As MsMappingNostro In MappingNostro
            result.Add({data.ID, data.BIC, data.MsCurrency.CurrencyDisplay, data.Status, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function
End Class