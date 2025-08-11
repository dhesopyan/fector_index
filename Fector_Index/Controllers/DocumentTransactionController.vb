Imports System.Net
Imports System.Data.Objects.SqlClient

Public Class DocumentTransactionController
    Inherits System.Web.Mvc.Controller

    Dim CustomerTypesItem As IEnumerable(Of SelectListItem) = {New SelectListItem With {.Text = "Domestic", .Value = "Domestic"}, _
                                                               New SelectListItem With {.Text = "Foreigner", .Value = "Foreigner"}}

    Dim DocumentTypesItem As IEnumerable(Of SelectListItem) = {New SelectListItem With {.Text = "Final", .Value = "Final"}, _
                                                               New SelectListItem With {.Text = "Estimation", .Value = "Estimation"}}

    '
    ' GET: /DocumentTransaction

    Public Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > Document Underlying"
        Return View("Index")
    End Function

    ' GET: /DocumentTransaction/Approval
    <Authorize> _
    Public Function Approval() As ActionResult
        ViewBag.Breadcrumb = "Home > Document Underlying > Approval"
        Return View("Index")
    End Function

    ' GET: /DocumentTransaction/Process
    <Authorize> _
    Function Process(ByVal id As Integer) As ActionResult
        Dim model As New MsDocumentTransactionViewModel
        Dim db As New FectorEntities

        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        If IsNothing(model.DocumentId) OrElse model.DocumentId = 0 Then
            model = MsDocumentTransactionViewModel.GetDocumentTransaction(id, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        ViewData("Title") = "Process Document Underlying"
        ViewBag.Breadcrumb = "Home > Document Underlying > Approval > Process"
        ViewBag.DocumentTypes = DocumentTypesItem
        ViewBag.CustomerTypes = CustomerTypesItem
        ViewBag.LHBUPurposes = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.LHBUDocuments = New MultiSelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        Return View("Detail", model)
    End Function

    ' POST: /Account/Process
    <Authorize> _
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    Function Process(model As MsDocumentTransactionViewModel, approvalButton As String) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Process Document Underlying"
        ViewBag.Breadcrumb = "Home > Document Underlying > Approval > Process"
        ViewBag.DocumentTypes = DocumentTypesItem
        ViewBag.CustomerTypes = CustomerTypesItem
        ViewBag.LHBUPurposes = New MultiSelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.LHBUDocuments = New MultiSelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        Dim doctrans = MsDocumentTransaction.GetAllDocumentTransaction(model.DocumentId, db)

        doctrans.ApproveBy = User.Identity.Name
        doctrans.ApproveDate = Now
        Dim Status As String = ""
        If approvalButton = "Approve" Then
            Dim flagDelete As Boolean = False
            Select Case doctrans.Status
                Case "ACTIVE - PENDING", "EDIT - PENDING", "CREATE - PENDING"
                    doctrans.Status = "ACTIVE"
                Case "INACTIVE - PENDING"
                    doctrans.Status = "INACTIVE"
                Case "DELETE - PENDING"
                    flagDelete = True
            End Select

            If flagDelete Then
                db.DocumentTransaction.Remove(doctrans)
            Else
                doctrans.ApproveBy = User.Identity.Name
                doctrans.ApproveDate = Now
                db.Entry(doctrans).State = EntityState.Modified
            End If
            Status = "APPROVE"
        Else
            doctrans.Status = "REJECTED"
            doctrans.ApproveBy = User.Identity.Name
            doctrans.ApproveDate = Now
            db.Entry(doctrans).State = EntityState.Modified
            Status = "REJECT"
        End If

        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, Status & " DOC TRAN", model.DocumentId, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' POST: /DocumentTransaction/Delete
    <Authorize> _
    <HttpPost> _
    Public Function Delete() As ActionResult
        Dim db As New FectorEntities()
        Dim id As Integer = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editeddocumenttransaction = MsDocumentTransaction.GetAllDocumentTransaction(id, db)

        If IsNothing(editeddocumenttransaction) Then
            Return New HttpNotFoundResult
        End If

        editeddocumenttransaction.Status = "DELETE - PENDING"
        editeddocumenttransaction.EditBy = User.Identity.Name
        editeddocumenttransaction.EditDate = Now
        db.Entry(editeddocumenttransaction).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "DELETE DOC TRANS", editeddocumenttransaction.DocumentId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /DocumentTransaction/Inactive
    <Authorize> _
    <HttpPost> _
    Public Function Inactive() As ActionResult
        Dim db As New FectorEntities()
        Dim id As Integer = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editeddocumenttransaction As MsDocumentTransaction = MsDocumentTransaction.GetAllDocumentTransaction(id, db)

        If IsNothing(editeddocumenttransaction) Then
            Return New HttpNotFoundResult
        End If

        editeddocumenttransaction.Status = "INACTIVE - PENDING"
        editeddocumenttransaction.EditBy = User.Identity.Name
        editeddocumenttransaction.EditDate = Now
        db.Entry(editeddocumenttransaction).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "INACTIVE DOC TRAN", editeddocumenttransaction.DocumentId, db)

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

        Dim editeddocumenttransaction As MsDocumentTransaction = MsDocumentTransaction.GetAllDocumentTransaction(id, db)

        If IsNothing(editeddocumenttransaction) Then
            Return New HttpNotFoundResult
        End If

        editeddocumenttransaction.Status = "ACTIVE - PENDING"
        editeddocumenttransaction.EditBy = User.Identity.Name
        editeddocumenttransaction.EditDate = Now
        db.Entry(editeddocumenttransaction).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "ACTIVE DOC TRAN", editeddocumenttransaction.DocumentId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' Get: /DocumentTransaction/Edit
    <Authorize> _
    Public Function Edit(ByVal id As Integer) As ActionResult
        Dim model As New MsDocumentTransactionViewModel
        Dim db As New FectorEntities

        If IsNothing(id) OrElse id = 0 Then
            Return RedirectToAction("Index", "DocumentTransaction")
        End If

        If IsNothing(model.DocumentId) Or model.DocumentId = 0 Then
            model = MsDocumentTransactionViewModel.GetDocumentTransaction(id, db)
        End If

        ViewData("Title") = "Edit Document Underlying"
        ViewBag.Breadcrumb = "Home > Document Underlying > Edit"
        ViewBag.DocumentTypes = DocumentTypesItem
        ViewBag.CustomerTypes = CustomerTypesItem
        ViewBag.LHBUPurposes = New MultiSelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.LHBUDocuments = New MultiSelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        Return View("Detail", model)
    End Function

    '
    ' Post: /DocumentTransaction/Edit
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Edit(model As MsDocumentTransactionViewModel) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Edit Document Underlying"
        ViewBag.Breadcrumb = "Home > Document Underlying > Edit"
        ViewBag.DocumentTypes = DocumentTypesItem
        ViewBag.CustomerTypes = CustomerTypesItem
        ViewBag.LHBUPurposes = New MultiSelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.LHBUDocuments = New MultiSelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        If ModelState.IsValid Then
            Dim DocumentId As String = model.DocumentId
            Dim Description As String = model.Description
            Dim DocumentType As String = model.DocumentType
            Dim CustomerType As String = model.CustomerType
            Dim SelectedPurpose As String() = model.SelectedLHBUPurposes
            Dim SelectedLHBU As String() = model.SelectedLHBUDocuments

            Dim editeddocumenttransaction As MsDocumentTransaction = MsDocumentTransaction.GetAllDocumentTransaction(model.DocumentId, db)
            With editeddocumenttransaction
                .Description = Description
                .DocumentType = DocumentType
                .CustomerType = CustomerType
                .EditBy = User.Identity.Name
                .EditDate = DateTime.Now
                .ApproveBy = ""
                .ApproveDate = New DateTime(1900, 1, 1)
                .Status = "EDIT - PENDING"
            End With
            db.Entry(editeddocumenttransaction).State = EntityState.Modified
            db.SaveChanges()

            For Each ms As MsMappingDocument In MsMappingDocument.GetMapping(model.DocumentId, db)
                db.MappingDocument.Remove(ms)
            Next

            For Each ms As MsMappingDocumentPurpose In MsMappingDocumentPurpose.GetMapping(model.DocumentId, db)
                db.MappingDocumentPurpose.Remove(ms)
            Next
            db.SaveChanges()

            For i As Integer = 0 To SelectedLHBU.Length - 1
                Dim newMapping As New MsMappingDocument
                With newMapping
                    .DocumentTransId = DocumentId
                    .DocumentLHBUId = SelectedLHBU(i)
                End With
                db.MappingDocument.Add(newMapping)
            Next

            For i As Integer = 0 To SelectedPurpose.Length - 1
                Dim newMapping As New MsMappingDocumentPurpose
                With newMapping
                    .DocumentTransId = DocumentId
                    .PurposeLHBUId = SelectedPurpose(i)
                End With
                db.MappingDocumentPurpose.Add(newMapping)
            Next
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "EDIT DOC TRAN", model.DocumentId, db)

            Return RedirectToAction("Index")
        Else
            Return View("Detail", model)
        End If
    End Function

    '
    ' Get: /DocumentTransaction/Create
    <Authorize> _
    Public Function Create() As ActionResult
        Dim model As New MsDocumentTransactionViewModel
        Dim db As New FectorEntities

        ViewData("Title") = "Create Document Underlying"
        ViewBag.Breadcrumb = "Home > Document Underlying > Create"
        ViewBag.DocumentTypes = DocumentTypesItem
        ViewBag.CustomerTypes = CustomerTypesItem
        ViewBag.LHBUPurposes = New MultiSelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.LHBUDocuments = New MultiSelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        Return View("Detail", model)
    End Function

    '
    ' Post: /DocumentTransaction/Create
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Create(model As MsDocumentTransactionViewModel) As ActionResult
        Dim db As New FectorEntities()

        ViewData("Title") = "Create Document Underlying"
        ViewBag.Breadcrumb = "Home > Document Underlying > Create"
        ViewBag.DocumentTypes = DocumentTypesItem
        ViewBag.CustomerTypes = CustomerTypesItem
        ViewBag.LHBUPurposes = New MultiSelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.LHBUDocuments = New MultiSelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim Description As String = model.Description
            Dim DocumentType As String = model.DocumentType
            Dim CustomerType As String = model.CustomerType
            Dim SelectedPurpose As String() = model.SelectedLHBUPurposes
            Dim SelectedLHBU As String() = model.SelectedLHBUDocuments

            Dim newDocumentTransaction As New MsDocumentTransaction
            With newDocumentTransaction
                .DocumentType = DocumentType
                .CustomerType = CustomerType
                .Description = Description
                .EditBy = User.Identity.Name
                .EditDate = DateTime.Now
                .ApproveBy = ""
                .ApproveDate = New DateTime(1900, 1, 1)
                .Status = "CREATE - PENDING"
            End With
            db.DocumentTransaction.Add(newDocumentTransaction)
            db.SaveChanges()

            Dim pk As Integer = newDocumentTransaction.DocumentId

            If SelectedLHBU IsNot Nothing Then
                For i As Integer = 0 To SelectedLHBU.Length - 1
                    Dim newMapping As New MsMappingDocument
                    With newMapping
                        .DocumentTransId = pk
                        .DocumentLHBUId = SelectedLHBU(i)
                    End With
                    db.MappingDocument.Add(newMapping)
                Next
            Else
                Return View("Detail", model)
            End If
            
            If SelectedPurpose IsNot Nothing Then
                For i As Integer = 0 To SelectedPurpose.Length - 1
                    Dim newMapping As New MsMappingDocumentPurpose
                    With newMapping
                        .DocumentTransId = pk
                        .PurposeLHBUId = SelectedPurpose(i)
                    End With
                    db.MappingDocumentPurpose.Add(newMapping)
                Next
            Else
                Return View("Detail", model)
            End If
           
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "CREATE DOC TRAN", pk, db)

            Return RedirectToAction("Index", "DocumentTransaction")
        End If

        Return View("Detail", model)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTDocumentTransactionAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim DocumentTransaction = From s In db.DocumentTransaction Select s
        Return ReturnDocumentTransactionDataTable(param, DocumentTransaction)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTDocumentTransactionApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim DocumentTransaction = From s In db.DocumentTransaction
                    Where s.Status.Contains("PENDING") And s.EditBy <> User.Identity.Name
                    Select s
        Return ReturnDocumentTransactionDataTable(param, DocumentTransaction)
    End Function

    Private Function ReturnDocumentTransactionDataTable(param As jQueryDataTableParamModel, DocumentTransaction As IQueryable(Of MsDocumentTransaction)) As JsonResult
        Dim totalRecords = DocumentTransaction.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim DescriptionSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim DocumentTypeSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim CustomerTypeSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        Dim StatusSearch As String = If(IsNothing(Request("sSearch_4")), "", Request("sSearch_4"))
        DocumentTransaction = DocumentTransaction.Where(Function(f) f.Description.Contains(DescriptionSearch))
        DocumentTransaction = DocumentTransaction.Where(Function(f) f.DocumentType.Contains(DocumentTypeSearch))
        DocumentTransaction = DocumentTransaction.Where(Function(f) f.CustomerType.Contains(CustomerTypeSearch))

        If StatusSearch <> "" Then
            DocumentTransaction = DocumentTransaction.Where(Function(f) f.Status.StartsWith(StatusSearch))
        End If

        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    DocumentTransaction = DocumentTransaction.OrderBy(Function(f) f.DocumentId)
                Else
                    DocumentTransaction = DocumentTransaction.OrderByDescending(Function(f) f.DocumentId)
                End If
            Case 1
                If sortOrder = "asc" Then
                    DocumentTransaction = DocumentTransaction.OrderBy(Function(f) f.Description)
                Else
                    DocumentTransaction = DocumentTransaction.OrderByDescending(Function(f) f.Description)
                End If
            Case 2
                If sortOrder = "asc" Then
                    DocumentTransaction = DocumentTransaction.OrderBy(Function(f) f.DocumentType)
                Else
                    DocumentTransaction = DocumentTransaction.OrderByDescending(Function(f) f.DocumentType)
                End If
            Case 3
                If sortOrder = "asc" Then
                    DocumentTransaction = DocumentTransaction.OrderBy(Function(f) f.CustomerType)
                Else
                    DocumentTransaction = DocumentTransaction.OrderByDescending(Function(f) f.CustomerType)
                End If
            Case 4
                If sortOrder = "asc" Then
                    DocumentTransaction = DocumentTransaction.OrderBy(Function(f) f.Status)
                Else
                    DocumentTransaction = DocumentTransaction.OrderByDescending(Function(f) f.Status)
                End If
            Case Else
                If sortOrder = "asc" Then
                    DocumentTransaction = DocumentTransaction.OrderBy(Function(f) f.DocumentId)
                Else
                    DocumentTransaction = DocumentTransaction.OrderByDescending(Function(f) f.DocumentId)
                End If
        End Select

        Dim result As New List(Of String())
        For Each data As MsDocumentTransaction In DocumentTransaction
            result.Add({data.DocumentId, data.Description, data.DocumentType, data.CustomerType, data.Status, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

End Class