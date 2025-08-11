Imports System.Net
Imports System.Data.Objects.SqlClient

Public Class MappingDocumentController
    Inherits System.Web.Mvc.Controller

    Public Shared tempID As String
    '
    ' GET: /MappingDocument

    Public Function Index() As ActionResult
        ViewBag.Breadcrumb = String.Format("<a href='{0}'>Home</a> > Mapping Document", Url.Content("~"))
        Return View("Index")
    End Function

    ' GET: /MappingDocument/Approval
    <Authorize> _
    Public Function Approval() As ActionResult
        ViewBag.Breadcrumb = String.Format("<a href='{0}'>Home</a> > <a href='{1}'> Mapping Document</a> > Approval", Url.Content("~"), Url.Content("~/MappingDocument/"))
        Return View("Index")
    End Function

    '
    ' POST: /MappingDocument/Delete
    <Authorize> _
    <HttpPost> _
    Public Function Delete() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedmappingdocument = db.MappingDocument.Find(id)

        If IsNothing(editedmappingdocument) Then
            Return New HttpNotFoundResult
        End If

        editedmappingdocument.Status = "DELETE - PENDING"
        db.MappingDocument.Remove(editedmappingdocument)
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "DELETE MAPPING DOCUMENT", editedmappingdocument.DocumentTransId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /MappingDocument/Inactivate
    <Authorize> _
    <HttpPost> _
    Public Function Inactivate() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedmappingdocument As MsMappingDocument = MsMappingDocument.GetDocumentTransaction(id, db)

        If IsNothing(editedmappingdocument) Then
            Return New HttpNotFoundResult
        End If

        editedmappingdocument.Status = "INACTIVE - PENDING"
        db.Entry(editedmappingdocument).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "INACTIVATE MAPPING DOCUMENT", editedmappingdocument.DocumentTransId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /MappingDocument/Activate
    <Authorize> _
    <HttpPost> _
    Public Function Activate() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedmappingdocument As MsMappingDocument = MsMappingDocument.GetDocumentTransaction(id, db)

        If IsNothing(editedmappingdocument) Then
            Return New HttpNotFoundResult
        End If

        editedmappingdocument.Status = "ACTIVE - PENDING"
        db.Entry(editedmappingdocument).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "INACTIVATE MAPPING DOCUMENT", editedmappingdocument.DocumentTransId, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /MappingDocument/Approve
    <Authorize> _
    <HttpPost> _
    Public Function Approve() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedmappingdocument = db.MappingDocument.Where(Function(f) f.DocumentTransId = id).ToList

        If IsNothing(editedmappingdocument) Then
            Return New HttpNotFoundResult
        End If

        For i As Integer = 0 To editedmappingdocument.Count - 1
            Dim flagDelete As Boolean = False
            Select Case editedmappingdocument(i).Status
                Case "ACTIVATE - PENDING", "EDIT - PENDING", "CREATE - PENDING"
                    editedmappingdocument(i).Status = "ACTIVE"
                Case "INACTIVATE - PENDING"
                    editedmappingdocument(i).Status = "INACTIVE"
                Case "DELETE - PENDING"
                    flagDelete = True
            End Select

            If flagDelete Then
                db.MappingDocument.Remove(editedmappingdocument(i))
                db.SaveChanges()
            Else
                editedmappingdocument(i).ApproveBy = User.Identity.Name
                editedmappingdocument(i).ApproveDate = Now
                db.Entry(editedmappingdocument(i)).State = EntityState.Modified
                db.SaveChanges()
            End If
        Next

        LogTransaction.WriteLog(User.Identity.Name, "APPROVE MAPPING DOCUMENT", id, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' POST: /MappingDocument/Reject
    <Authorize> _
    <HttpPost> _
    Public Function Reject() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedmappingdocument = db.MappingDocument.Where(Function(f) f.DocumentTransId = id).ToList

        If IsNothing(editedmappingdocument) Then
            Return New HttpNotFoundResult
        End If

        For i As Integer = 0 To editedmappingdocument.Count - 1
            editedmappingdocument(i).Status = "REJECTED"
            editedmappingdocument(i).ApproveBy = User.Identity.Name
            editedmappingdocument(i).ApproveDate = Now
            db.Entry(editedmappingdocument(i)).State = EntityState.Modified
            db.SaveChanges()
        Next

        LogTransaction.WriteLog(User.Identity.Name, "REJECT MAPPING DOCUMENT", id, db)

        Return RedirectToAction("Approval")
    End Function

    '
    ' Get: /MappingDocument/Edit
    <Authorize> _
    Public Function Edit(ByVal id As String) As ActionResult
        Dim model As New MsMappingDocument
        Dim db As New FectorEntities

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return RedirectToAction("Index", "MappingDocument")
        End If

        If IsNothing(model.DocumentTransId) Then
            model = MsMappingDocument.GetDocumentTransaction(id, db)
        End If

        ViewData("Title") = "Edit Mapping Document"
        ViewBag.Breadcrumb = String.Format("<a href='{0}'>Home</a> > <a href='{1}'> Mapping Document</a> > Edit", Url.Content("~"), Url.Content("~/MappingDocument/Index"))
        ViewBag.DocumentTransId = New SelectList(db.MappingDocument, "DocumentTransId", "DocumentTransId")
        tempID = id
        Return View("Detail", model)
    End Function

    '
    ' Post: /MappingDocument/Edit
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Edit(model As MsMappingDocument, ByVal Collection As FormCollection) As ActionResult
        Dim db As New FectorEntities()

        ViewData("Title") = "Edit Mapping Document"
        ViewBag.Breadcrumb = String.Format("<a href='{0}'>Home</a> > <a href='{1}'> Mapping Document</a> > Edit", Url.Content("~"), Url.Content("~/MappingDocument/Index"))
        ViewBag.DocumentId = New SelectList(db.MappingDocument, "DocumentTransId", "DocumentTransId")

        Dim db2 As New FectorEntities()
        Dim DocumentLHBU = From s In db2.DocumentLHBU Select s

        Dim temp As String
        Dim tempList As New List(Of String())
        For Each Data As MsDocumentLHBU In DocumentLHBU
            temp = Collection.Item("documentid[" & Data.DocumentId & "]")
            If temp = "on" Then
                tempList.Add({Data.DocumentId})
            Else
                tempList.Remove({Data.DocumentId})
            End If
        Next

        db = New FectorEntities()

        If ModelState.IsValid Then
            ViewData("Title") = "Edit Mapping Document"
            ViewBag.Breadcrumb = String.Format("<a href='{0}'>Home</a> > <a href='{1}'> Mapping Document</a> > Edit", Url.Content("~"), Url.Content("~/MappingDocument/Index"))

            Dim editedmappingdocument As List(Of MsMappingDocument) = MsMappingDocument.GetAllDocumentTransaction(tempID, db)

            'delete first
            If editedmappingdocument IsNot Nothing Then
                For Each Data As MsMappingDocument In editedmappingdocument
                    db.MappingDocument.Remove(Data)
                    db.SaveChanges()
                Next
            End If

            'insert again
            Dim newMappingDocument As New MsMappingDocument
            For i As Integer = 0 To tempList.Count - 1
                newMappingDocument = New MsMappingDocument
                With newMappingDocument
                    .DocumentTransId = tempID
                    .DocumentLHBUId = tempList(i)(0)
                    .EditBy = User.Identity.Name
                    .EditDate = DateTime.Now
                    .ApproveBy = ""
                    .ApproveDate = New DateTime(1900, 1, 1)
                    .Status = "EDIT - PENDING"
                End With

                db.MappingDocument.Add(newMappingDocument)
                db.SaveChanges()
            Next

            LogTransaction.WriteLog(User.Identity.Name, "EDIT MAPPING DOCUMENT", tempID, db)

            Return RedirectToAction("Index")
        Else
            Return View("Detail", model)
        End If
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTMappingDocumentAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim DocumentTransaction = From s In db.DocumentTransaction.Include("Purpose") Select s
        Return ReturnMappingDocumentDataTable(param, DocumentTransaction)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTMappingDocumentDetailAjaxHandler(param As jQueryDataTableParamModel, ByVal Collection As FormCollection) As JsonResult
        Dim db As New FectorEntities()
        Dim db2 As New FectorEntities()
        Dim DocumentLHBU = From s In db.DocumentLHBU Where s.Status.Contains("PENDING") = False Select s
        Dim MappingDocument = From s In db2.MappingDocument Where s.Status = "ACTIVE" Select s
        Return ReturnMappingDocumentDetailDataTable(param, DocumentLHBU, MappingDocument)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTMappingDocumentApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim MappingDocument = (From s In db.DocumentTransaction.Include("Purpose")
                              Join t In db.MappingDocument On s.DocumentId Equals t.DocumentTransId
        Where t.Status.Contains("PENDING") And t.EditBy <> User.Identity.Name
                    Select s).Distinct
        Return ReturnMappingDocumentDataTable(param, MappingDocument)
    End Function

    Private Function ReturnMappingDocumentDataTable(param As jQueryDataTableParamModel, MappingDocument As IQueryable(Of MsDocumentTransaction)) As JsonResult
        Dim totalRecords = MappingDocument.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim DocumentIdSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim DescriptionSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim DocumentTypeSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        Dim CustomerTypeSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))
        Dim PurposeIdSearch As String = If(IsNothing(Request("sSearch_4")), "", Request("sSearch_4"))
        MappingDocument = MappingDocument.Where(Function(f) f.DocumentId.Contains(DocumentIdSearch))
        MappingDocument = MappingDocument.Where(Function(f) f.Description.Contains(DescriptionSearch))
        MappingDocument = MappingDocument.Where(Function(f) f.DocumentType.Contains(DocumentTypeSearch))
        MappingDocument = MappingDocument.Where(Function(f) f.CustomerType.Contains(CustomerTypeSearch))
        MappingDocument = MappingDocument.Where(Function(f) f.PurposeId.Contains(PurposeIdSearch))

        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    MappingDocument = MappingDocument.OrderBy(Function(f) f.DocumentId)
                Else
                    MappingDocument = MappingDocument.OrderByDescending(Function(f) f.DocumentId)
                End If
            Case 1
                If sortOrder = "asc" Then
                    MappingDocument = MappingDocument.OrderBy(Function(f) f.Description)
                Else
                    MappingDocument = MappingDocument.OrderByDescending(Function(f) f.Description)
                End If
            Case 2
                If sortOrder = "asc" Then
                    MappingDocument = MappingDocument.OrderBy(Function(f) f.DocumentType)
                Else
                    MappingDocument = MappingDocument.OrderByDescending(Function(f) f.DocumentType)
                End If
            Case 3
                If sortOrder = "asc" Then
                    MappingDocument = MappingDocument.OrderBy(Function(f) f.CustomerType)
                Else
                    MappingDocument = MappingDocument.OrderByDescending(Function(f) f.CustomerType)
                End If
            Case 4
                If sortOrder = "asc" Then
                    MappingDocument = MappingDocument.OrderBy(Function(f) f.PurposeId)
                Else
                    MappingDocument = MappingDocument.OrderByDescending(Function(f) f.PurposeId)
                End If
            Case Else
                If sortOrder = "asc" Then
                    MappingDocument = MappingDocument.OrderBy(Function(f) f.DocumentId)
                Else
                    MappingDocument = MappingDocument.OrderByDescending(Function(f) f.DocumentId)
                End If
        End Select

        Dim result As New List(Of String())
        For Each data As MsDocumentTransaction In MappingDocument
            result.Add({data.DocumentId, data.Description, data.DocumentType, data.CustomerType, "", data.Status, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

    Private Function ReturnMappingDocumentDetailDataTable(param As jQueryDataTableParamModel, DocumentLHBU As IQueryable(Of MsDocumentLHBU), MappingDocument As IQueryable(Of MsMappingDocument)) As JsonResult
        Dim totalRecords = DocumentLHBU.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim DocumentIdSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim DescriptionSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        DocumentLHBU = DocumentLHBU.Where(Function(f) f.DocumentId.Contains(DocumentIdSearch))
        DocumentLHBU = DocumentLHBU.Where(Function(f) f.Description.Contains(DescriptionSearch))
        MappingDocument = MappingDocument.Where(Function(f) f.DocumentTransId = tempID)

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
            Case Else
                If sortOrder = "asc" Then
                    DocumentLHBU = DocumentLHBU.OrderBy(Function(f) f.DocumentId)
                Else
                    DocumentLHBU = DocumentLHBU.OrderByDescending(Function(f) f.DocumentId)
                End If
        End Select

        Dim result As New List(Of String())

        Dim db2 As New FectorEntities()
        Dim tempDocumentLHBU = From s In db2.DocumentLHBU Where s.Status.Contains("PENDING") = False Select s
        Dim tempList As New List(Of String())
        If MappingDocument.Count <> 0 Then
            For Each Data As MsMappingDocument In MappingDocument.ToList
                For Each data2 As MsDocumentLHBU In DocumentLHBU
                    If Data.DocumentLHBUId = data2.DocumentId Then
                        result.Add({data2.DocumentId, data2.Description, "<div style='text-align:center;'><input type='checkbox' name='documentid[" & data2.DocumentId & "]' checked></div>", ""})
                    End If
                Next
            Next

            Dim tempInt As Integer = 0
            Dim tempResultWithCheck As Integer = result.Count
            For Each Data As MsDocumentLHBU In DocumentLHBU
                For i As Integer = tempInt To tempDocumentLHBU.Count - 1
                    If tempInt <= tempResultWithCheck - 1 Then
                        If result(i).Contains(Data.DocumentId) = True Then
                            tempInt += 1
                            Exit For
                        Else
                            result.Add({Data.DocumentId, Data.Description, "<div style='text-align:center;'><input type='checkbox' name='documentid[" & Data.DocumentId & "]' ></div>", ""})
                            Exit For
                        End If
                    Else
                        result.Add({Data.DocumentId, Data.Description, "<div style='text-align:center;'><input type='checkbox' name='documentid[" & Data.DocumentId & "]'></div>", ""})
                        Exit For
                    End If
                Next
            Next
        Else
            For Each data As MsDocumentLHBU In DocumentLHBU
                result.Add({data.DocumentId, data.Description, "<div style='text-align:center;'><input type='checkbox' name='documentid[" & data.DocumentId & "]'></div>", ""})
            Next
        End If

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function
End Class