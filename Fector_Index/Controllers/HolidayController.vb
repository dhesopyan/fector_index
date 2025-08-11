Public Class HolidayController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /Holiday

    Function Index() As ActionResult
        Return View()
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTHolidayAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim Holiday = From h In db.Holidays
                      Select h

        Return ReturnHolidayDataTable(param, Holiday)
    End Function

    Private Function ReturnHolidayDataTable(param As jQueryDataTableParamModel, Holiday As IQueryable(Of MsHoliday)) As JsonResult
        Dim totalRecords = Holiday.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim HolidayIdSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim HolidayDescSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim StartDateSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        Dim EndDateSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))
        Holiday = Holiday.Where(Function(f) f.HolidayDesc.Contains(HolidayDescSearch))
        If StartDateSearch.Trim.Length > 0 And AppHelper.checkDate(StartDateSearch) Then
            Dim dTDate As Date
            dTDate = AppHelper.dateConvert(StartDateSearch)
            Holiday = Holiday.Where(Function(f) f.StartDate.Day = dTDate.Day And f.StartDate.Month = dTDate.Month And f.StartDate.Year = dTDate.Year)
        End If
        If EndDateSearch.Trim.Length > 0 And AppHelper.checkDate(EndDateSearch) Then
            Dim dTDate As Date
            dTDate = AppHelper.dateConvert(EndDateSearch)
            Holiday = Holiday.Where(Function(f) f.EndDate.Day = dTDate.Day And f.EndDate.Month = dTDate.Month And f.EndDate.Year = dTDate.Year)
        End If

        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    Holiday = Holiday.OrderBy(Function(f) f.HolidayId)
                Else
                    Holiday = Holiday.OrderByDescending(Function(f) f.HolidayId)
                End If
            Case 1
                If sortOrder = "asc" Then
                    Holiday = Holiday.OrderBy(Function(f) f.HolidayDesc)
                Else
                    Holiday = Holiday.OrderByDescending(Function(f) f.HolidayDesc)
                End If
            Case 2
                If sortOrder = "asc" Then
                    Holiday = Holiday.OrderBy(Function(f) f.StartDate)
                Else
                    Holiday = Holiday.OrderByDescending(Function(f) f.StartDate)
                End If
            Case 3
                If sortOrder = "asc" Then
                    Holiday = Holiday.OrderBy(Function(f) f.EndDate)
                Else
                    Holiday = Holiday.OrderByDescending(Function(f) f.EndDate)
                End If
        End Select

        Dim result As New List(Of String())
        For Each data As MsHoliday In Holiday
            result.Add({data.HolidayId, data.HolidayDesc, CDate(data.StartDate).ToString("dd-MM-yyy"), CDate(data.EndDate).ToString("dd-MM-yyyy"), data.Nostro, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

End Class