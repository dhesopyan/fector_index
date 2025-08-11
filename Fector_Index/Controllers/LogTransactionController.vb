Public Class LogTransactionController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /LogTransaction

    Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > Audit Trail Transaction"
        Return View()
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTLogTransactionAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim audittrail = From s In db.LogTransactions
                         Select s
        Return ReturnLogUserDataTable(param, audittrail)
    End Function

    Private Function ReturnLogUserDataTable(param As jQueryDataTableParamModel, audittrail As IQueryable(Of LogTransaction)) As JsonResult
        Dim totalRecords = audittrail.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        'Dim LogIdSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim UserIdSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))

        Dim TDateSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        If TDateSearch.Trim.Length > 0 And AppHelper.checkDate(TDateSearch) Then
            Dim dTDate As Date
            dTDate = AppHelper.dateTimeConvert(TDateSearch)
            audittrail = audittrail.Where(Function(f) f.Tdate.Day = dTDate.Day And f.Tdate.Month = dTDate.Month And f.Tdate.Year = dTDate.Year And f.Tdate.Hour = dTDate.Hour And f.Tdate.Minute = dTDate.Minute And f.Tdate.Second = dTDate.Second)
        End If

        Dim ActionSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))
        Dim RefNoSearch As String = If(IsNothing(Request("sSearch_4")), "", Request("sSearch_4"))
        'Dim StatusSearch As String = If(IsNothing(Request("sSearch_5")), "", Request("sSearch_5"))
        audittrail = audittrail.Where(Function(f) f.UserId.Contains(UserIdSearch))
        audittrail = audittrail.Where(Function(f) f.Action.Contains(ActionSearch))
        audittrail = audittrail.Where(Function(f) f.RefNo.Contains(RefNoSearch))

        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    audittrail = audittrail.OrderBy(Function(f) f.LogId)
                Else
                    audittrail = audittrail.OrderByDescending(Function(f) f.LogId)
                End If
            Case 1
                If sortOrder = "asc" Then
                    audittrail = audittrail.OrderBy(Function(f) f.UserId)
                Else
                    audittrail = audittrail.OrderByDescending(Function(f) f.UserId)
                End If
            Case 2
                If sortOrder = "asc" Then
                    audittrail = audittrail.OrderBy(Function(f) f.Tdate)
                Else
                    audittrail = audittrail.OrderByDescending(Function(f) f.Tdate)
                End If
            Case 3
                If sortOrder = "asc" Then
                    audittrail = audittrail.OrderBy(Function(f) f.Action)
                Else
                    audittrail = audittrail.OrderByDescending(Function(f) f.Action)
                End If
            Case 4
                If sortOrder = "asc" Then
                    audittrail = audittrail.OrderBy(Function(f) f.RefNo)
                Else
                    audittrail = audittrail.OrderByDescending(Function(f) f.RefNo)
                End If
        End Select

        Dim result As New List(Of String())
        For Each atu As LogTransaction In audittrail.OrderByDescending(Function(f) f.Tdate)
            result.Add({atu.LogId, atu.UserId, atu.Tdate.ToString("dd-MM-yyyy HH:mm:ss"), atu.Action, atu.RefNo, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function
End Class