Imports System.Net
Imports System.Data.Objects.SqlClient

Public Class CountryController
    Inherits System.Web.Mvc.Controller

    ' GET: /Country/Index
    <Authorize> _
    Public Function Index() As ActionResult
        ViewBag.Breadcrumb = "Home > Country"
        Return View("Index")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTCountryAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim countries = From s In db.Countries Select s
        Return ReturnCountryDataTable(param, countries)
    End Function

    Private Function ReturnCountryDataTable(param As jQueryDataTableParamModel, countries As IQueryable(Of MsCountry)) As JsonResult
        Dim totalRecords = countries.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim statuscodesearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim descriptionsearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        countries = countries.Where(Function(f) f.CountryId.Contains(statuscodesearch))
        countries = countries.Where(Function(f) f.Description.Contains(descriptionsearch))

        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    countries = countries.OrderBy(Function(f) f.CountryId)
                Else
                    countries = countries.OrderByDescending(Function(f) f.CountryId)
                End If
            Case 1
                If sortOrder = "asc" Then
                    countries = countries.OrderBy(Function(f) f.Description)
                Else
                    countries = countries.OrderByDescending(Function(f) f.Description)
                End If
        End Select

        Dim result As New List(Of String())
        For Each stat As MsCountry In countries
            result.Add({stat.CountryId, stat.Description, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

End Class