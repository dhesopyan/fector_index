Public Class CoreNonTrxController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /CoreNonTrx
    <Authorize> _
    Function Index() As ActionResult
        Return View()
    End Function

    ' POST: /CoreNonTrx/Index
    <Authorize> _
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    Function Index(chkSelect As List(Of String), button As String) As ActionResult
        Dim db As New FectorEntities

        For i As Integer = 0 To chkSelect.Count - 1
            Dim CoreNonTrx = CoreNonTrxViewModel.GetCoreNonTrx(chkSelect(i), db)

            If button = "BtnProcess" Then
                CoreNonTrx.FlagLHBU = "1"
            ElseIf button = "BtnDelete" Then
                CoreNonTrx.FlagLHBU = "-1"
            ElseIf button = "BtnActive" Then
                CoreNonTrx.FlagLHBU = "0"
            ElseIf button = "BtnInactive" Then
                CoreNonTrx.FlagLHBU = "2"
            End If


            db.Entry(CoreNonTrx).State = EntityState.Modified
            db.SaveChanges()

            If button = "BtnProcess" Then
                LogTransaction.WriteLog(User.Identity.Name, "ACTIVE CORE NON TRX", chkSelect(i), db)
            ElseIf button = "BtnDelete" Then
                LogTransaction.WriteLog(User.Identity.Name, "DELETE CORE NON TRX", chkSelect(i), db)
            ElseIf button = "BtnActive" Then
                LogTransaction.WriteLog(User.Identity.Name, "ACTIVE CORE NON TRX", chkSelect(i), db)
            ElseIf button = "BtnInactive" Then
                LogTransaction.WriteLog(User.Identity.Name, "INACTIVE CORE NON TRX", chkSelect(i), db)
            End If

        Next
        Return View()
    End Function

    Private Class SelectNonTrx
        Public Property RefNo As Nullable(Of Decimal)
        Public Property TDate As DateTime
        Public Property Time As String
        Public Property AccNo As String
        Public Property AccName As String
        Public Property BranchId As String
        Public Property Branch As String
        Public Property CoreCurrId As String
        Public Property Currency As String
        Public Property Rate As Nullable(Of Decimal)
        Public Property Amount As Nullable(Of Decimal)
        Public Property FlagLHBU As Integer
    End Class

    <Authorize> _
    <HttpGet> _
    Public Function DTCoreNonTrxAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim UserBranchID As Decimal = Session("BranchId")
        Dim db As New FectorEntities()
        Dim CoreNonTrx = (From a In db.CoreNonTrx
                         Join b In db.Branches On a.BranchId Equals b.BranchId
                         Join c In db.Accounts On a.AccNo Equals c.AccNo
                         Join d In db.Currencies On a.CoreCurrId Equals d.CoreCurrId
                         Where a.TDate.Year = Now.Year And a.TDate.Month = Now.Month And a.TDate.Day = Now.Day
                         Select New SelectNonTrx With {.RefNo = a.Refno, _
                                                       .TDate = a.TDate, _
                                                       .Time = a.Time, _
                                                       .AccNo = a.AccNo, _
                                                       .AccName = c.Name, _
                                                       .BranchId = a.BranchId, _
                                                       .Branch = b.Name, _
                                                       .CoreCurrId = a.CoreCurrId, _
                                                       .Currency = d.CurrId, _
                                                       .Rate = a.Rate, _
                                                       .Amount = a.Amount, _
                                                       .FlagLHBU = a.FlagLHBU}).Distinct

        Return ReturnCoreNonTrxDataTable(param, CoreNonTrx)
    End Function

    Private Function ReturnCoreNonTrxDataTable(param As jQueryDataTableParamModel, CoreNonTrx As IQueryable(Of SelectNonTrx)) As JsonResult
        Dim totalRecords As Integer = CoreNonTrx.Count
        Dim displayRecord As Integer = 0

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim AccNoSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim AccNameSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        Dim CoreCurrSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))
        Dim BranchSearch As String = If(IsNothing(Request("sSearch_4")), "", Request("sSearch_4"))
        Dim TimeSearch As String = If(IsNothing(Request("sSearch_5")), "", Request("sSearch_5"))
        Dim StatusSearch As String = If(IsNothing(Request("sSearch_6")), "", Request("sSearch_6"))

        CoreNonTrx = CoreNonTrx.Where(Function(f) f.AccNo.Contains(AccNoSearch))
        CoreNonTrx = CoreNonTrx.Where(Function(f) f.AccName.Contains(AccNameSearch))
        CoreNonTrx = CoreNonTrx.Where(Function(f) f.CoreCurrId.Contains(CoreCurrSearch) Or f.Currency.Contains(CoreCurrSearch))
        CoreNonTrx = CoreNonTrx.Where(Function(f) f.Branch.Contains(BranchSearch))
        CoreNonTrx = CoreNonTrx.Where(Function(f) f.Branch.Contains(BranchSearch))
        CoreNonTrx = CoreNonTrx.Where(Function(f) f.Branch.Contains(BranchSearch))

        If StatusSearch <> "" Then
            CoreNonTrx = CoreNonTrx.Where(Function(f) f.FlagLHBU = StatusSearch)
        End If

        displayRecord = CoreNonTrx.Count

        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    CoreNonTrx = CoreNonTrx.OrderBy(Function(f) f.TDate)
                Else
                    CoreNonTrx = CoreNonTrx.OrderByDescending(Function(f) f.TDate)
                End If
            Case 1
                If sortOrder = "asc" Then
                    CoreNonTrx = CoreNonTrx.OrderBy(Function(f) f.AccNo)
                Else
                    CoreNonTrx = CoreNonTrx.OrderByDescending(Function(f) f.AccNo)
                End If
            Case 2
                If sortOrder = "asc" Then
                    CoreNonTrx = CoreNonTrx.OrderBy(Function(f) f.AccName)
                Else
                    CoreNonTrx = CoreNonTrx.OrderByDescending(Function(f) f.AccName)
                End If
            Case 3
                If sortOrder = "asc" Then
                    CoreNonTrx = CoreNonTrx.OrderBy(Function(f) f.CoreCurrId)
                Else
                    CoreNonTrx = CoreNonTrx.OrderByDescending(Function(f) f.CoreCurrId)
                End If
            Case 4
                If sortOrder = "asc" Then
                    CoreNonTrx = CoreNonTrx.OrderBy(Function(f) f.BranchId)
                Else
                    CoreNonTrx = CoreNonTrx.OrderByDescending(Function(f) f.BranchId)
                End If
            Case 5
                If sortOrder = "asc" Then
                    CoreNonTrx = CoreNonTrx.OrderBy(Function(f) f.Amount)
                Else
                    CoreNonTrx = CoreNonTrx.OrderByDescending(Function(f) f.Amount)
                End If
            Case 6
                If sortOrder = "asc" Then
                    CoreNonTrx = CoreNonTrx.OrderBy(Function(f) f.Time)
                Else
                    CoreNonTrx = CoreNonTrx.OrderByDescending(Function(f) f.Time)
                End If
            Case 7
                If sortOrder = "asc" Then
                    CoreNonTrx = CoreNonTrx.OrderBy(Function(f) f.Rate)
                Else
                    CoreNonTrx = CoreNonTrx.OrderByDescending(Function(f) f.Rate)
                End If
        End Select

        'Public Property RefNo As Nullable(Of Decimal)
        'Public Property TDate As DateTime
        'Public Property Time As String
        'Public Property AccNo As String
        'Public Property AccName As String
        'Public Property BranchId As String
        'Public Property Branch As String
        'Public Property CoreCurrId As String
        'Public Property Currency As String
        'Public Property Rate As Nullable(Of Decimal)
        'Public Property Amount As Nullable(Of Decimal)
        'Public Property FlagLHBU As Integer

        Dim result As New List(Of String())
        For Each data As SelectNonTrx In CoreNonTrx
            Dim tmpStatus As String = ""
            If data.FlagLHBU = "-1" Then
                tmpStatus = "DELETED"
            ElseIf data.FlagLHBU = "0" Then
                tmpStatus = "ACTIVE"
            ElseIf data.FlagLHBU = "1" Then
                tmpStatus = "PROCESS"
            Else
                tmpStatus = "INACTIVE"
            End If
            If data.Time.Length = 5 Then
                data.Time = "0" & data.Time
            End If
            result.Add({data.RefNo, _
                    data.TDate.ToString("dd-MM-yyyy"), _
                    data.AccNo, _
                    data.AccName, _
                    data.CoreCurrId & " - " & data.Currency, _
                    data.BranchId & " - " & data.Branch, _
                    CDec(data.Amount).ToString("N2"), _
                    data.Time.Substring(0, 2) & ":" & data.Time.Substring(2, 2) & ":" & data.Time.Substring(4, 2), _
                    CDec(data.Rate).ToString("N2"), _
                    tmpStatus, ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = displayRecord,
                      .aaData = result},
                      JsonRequestBehavior.AllowGet)
    End Function

End Class