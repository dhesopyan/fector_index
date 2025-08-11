Public Class LogUserController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /LogUsers

    Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > Audit Trail User"
        Return View("Index")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTLogUserAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim audittrail = From s In db.LogUsers
                         Order By s.LoginTime Descending
                         Select s
        Return ReturnLogUserDataTable(param, audittrail)
    End Function

    Private Function ReturnLogUserDataTable(param As jQueryDataTableParamModel, audittrail As IQueryable(Of LogUser)) As JsonResult
        Dim totalRecords = audittrail.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)
        Dim iSort = param.iSortingCols

        Dim LogIdSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim UserIdSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))

        Dim LoginTimeSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        If LoginTimeSearch.Trim.Length > 0 And AppHelper.checkDate(LoginTimeSearch) Then
            Dim dTDate As Date
            dTDate = AppHelper.dateTimeConvert(LoginTimeSearch)
            audittrail = audittrail.Where(Function(f) f.LoginTime.Day = dTDate.Day And f.LoginTime.Month = dTDate.Month And f.LoginTime.Year = dTDate.Year And f.LoginTime.Hour = dTDate.Hour And f.LoginTime.Minute = dTDate.Minute And f.LoginTime.Second = dTDate.Second)
        End If

        Dim ExpiredTimeSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))
        If ExpiredTimeSearch.Trim.Length > 0 And AppHelper.checkDate(ExpiredTimeSearch) Then
            Dim dTDate As Date
            dTDate = AppHelper.dateTimeConvert(ExpiredTimeSearch)
            audittrail = audittrail.Where(Function(f) f.ExpiredTime.Day = dTDate.Day And f.ExpiredTime.Month = dTDate.Month And f.ExpiredTime.Year = dTDate.Year And f.ExpiredTime.Hour = dTDate.Hour And f.ExpiredTime.Minute = dTDate.Minute And f.ExpiredTime.Second = dTDate.Second)
        End If

        Dim LogoutTimeSearch As String = If(IsNothing(Request("sSearch_4")), "", Request("sSearch_4"))
        If LogoutTimeSearch.Trim.Length > 0 And AppHelper.checkDate(LogoutTimeSearch) Then
            Dim dTDate As Date
            dTDate = AppHelper.dateTimeConvert(LogoutTimeSearch)
            audittrail = audittrail.Where(Function(f) f.LogoutTime.Day = dTDate.Day And f.LogoutTime.Month = dTDate.Month And f.LogoutTime.Year = dTDate.Year And f.LogoutTime.Hour = dTDate.Hour And f.LogoutTime.Minute = dTDate.Minute And f.LogoutTime.Second = dTDate.Second)
        End If

        audittrail = audittrail.Where(Function(f) f.UserId.Contains(UserIdSearch))

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
                    audittrail = audittrail.OrderBy(Function(f) f.LoginTime)
                Else
                    audittrail = audittrail.OrderByDescending(Function(f) f.LoginTime)
                End If
            Case 3
                If sortOrder = "asc" Then
                    audittrail = audittrail.OrderBy(Function(f) f.ExpiredTime)
                Else
                    audittrail = audittrail.OrderByDescending(Function(f) f.ExpiredTime)
                End If
            Case 4
                If sortOrder = "asc" Then
                    audittrail = audittrail.OrderBy(Function(f) f.LogoutTime)
                Else
                    audittrail = audittrail.OrderByDescending(Function(f) f.LogoutTime)
                End If
        End Select

        Dim result As New List(Of String())
        For Each atu As LogUser In audittrail.OrderByDescending(Function(f) f.LoginTime)
            result.Add({atu.LogId, atu.UserId, atu.LoginTime.ToString("dd-MM-yyyy HH:mm:ss"), atu.ExpiredTime.ToString("dd-MM-yyyy HH:mm:ss"), IIf(atu.LogoutTime.ToString("dd-MM-yyyy hh:mm:ss") = "01-01-1900 12:00:00", "-", atu.LogoutTime.ToString("dd-MM-yyyy HH:mm:ss")), atu.Status, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

End Class