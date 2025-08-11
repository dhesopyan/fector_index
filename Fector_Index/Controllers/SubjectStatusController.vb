Imports System.Net
Imports System.Data.Objects.SqlClient

Public Class SubjectStatusController
    Inherits System.Web.Mvc.Controller

    ' GET: /SubjectStatus/Index
    <Authorize> _
    Public Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > Subject Status"
        Return View("Index")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTSubjectStatusAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim subjectstats = From s In db.SubjectStatus Select s
        Return ReturnSubjectStatusDataTable(param, subjectstats)
    End Function

    Private Function ReturnSubjectStatusDataTable(param As jQueryDataTableParamModel, subjectstats As IQueryable(Of MsSubjectStatus)) As JsonResult
        Dim totalRecords = subjectstats.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim statuscodesearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim descriptionsearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        subjectstats = subjectstats.Where(Function(f) f.StatusId.Contains(statuscodesearch))
        subjectstats = subjectstats.Where(Function(f) f.Description.Contains(descriptionsearch))

        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    subjectstats = subjectstats.OrderBy(Function(f) f.StatusId)
                Else
                    subjectstats = subjectstats.OrderByDescending(Function(f) f.StatusId)
                End If
            Case 1
                If sortOrder = "asc" Then
                    subjectstats = subjectstats.OrderBy(Function(f) f.Description)
                Else
                    subjectstats = subjectstats.OrderByDescending(Function(f) f.Description)
                End If
        End Select

        Dim result As New List(Of String())
        For Each stat As MsSubjectStatus In subjectstats
            result.Add({stat.StatusId, stat.Description, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

End Class