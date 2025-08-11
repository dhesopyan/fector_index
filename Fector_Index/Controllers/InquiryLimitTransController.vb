Imports CrystalDecisions.CrystalReports.Engine
Imports System.IO

Public Class InquiryLimitTransController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /InquiryLimitTrans

    Function Index() As ActionResult
        Return View()
    End Function

    Private Class LimitTransaction
        Public CIF As String
        Public AccName As String
    End Class

    Private Class DataDeal
        Public CIF As String
        Public AccName As String
        Public BaseCurrency As String
        Public DealPeriod As String
        Public AmountDeal As Nullable(Of Decimal)
    End Class

    Public Class SUMDeal
        Public CIF As String
        Public DealPeriod As String
        Public TotalDeal As Nullable(Of Decimal)
    End Class

    Private Class SUMAll
        Public CIF As String
        Public TotalAllDeal As Nullable(Of Decimal)
    End Class

    <Authorize> _
    <HttpGet> _
    Public Function DTLimitTransactionAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()

        'Dim tmp = From c In db.Accounts
        '          Join a In db.TransactionDeal On a.AccNum Equals c.AccNo
        '          Join b In db.ExchangeRate On a.CurrencyDeal Equals b.CurrId
        '          Join d In db.ExchangeRate On c.TODLimitcurrency Equals d.CurrId
        '          Join e In db.ExchangeRate On c.TOMLimitcurrency Equals e.CurrId
        '          Join f In db.ExchangeRate On c.SPOTLimitcurrency Equals f.CurrId
        '          Where a.Status <> "REJECTED" And a.DealDate.Year = Now.Year And a.DealDate.Month = Now.Month And a.TransactionType = "Sell"
        '          Select New DataDeal With {.AccNo = a.AccNum, _
        '                                    .AccName = a.AccName, _
        '                                    .TransactionType = a.TransactionType, _
        '                                    .DealType = a.DealType, _
        '                                    .BaseCurrency = a.CurrencyDeal, _
        '                                    .DealPeriod = a.DealPeriod, _
        '                                    .AmountDeal = If(a.DealPeriod = "TOD", _
        '                                                   (a.AmountDeal * b.ClosingRate / d.ClosingRate), _
        '                                                If(a.DealPeriod = "TOM", _
        '                                                   (a.AmountDeal * b.ClosingRate / e.ClosingRate), _
        '                                                If(a.DealPeriod = "SPOT", _
        '                                                   (a.AmountDeal * b.ClosingRate / f.ClosingRate), _
        '                                                0)))}

        Dim tmp = From a In db.Accounts
                   Join b In db.TransactionDeal On a.AccNo Equals b.AccNum
                   Join c In db.ExchangeRate On b.CurrencyDeal Equals c.CurrId
                   Where b.Status <> "REJECTED" And b.TransactionType = "Sell"
                   Select New DataDeal With {.CIF = a.CIF, _
                                            .AccName = a.Name, _
                                            .BaseCurrency = b.CurrencyDeal, _
                                            .DealPeriod = b.DealPeriod, _
                                            .AmountDeal = (b.AmountDeal * c.ClosingRate) / db.ExchangeRate.Where(Function(f) f.CurrId = "USD").FirstOrDefault.ClosingRate}

        Dim tmp2 = From a In tmp
                    Group By a.CIF, a.DealPeriod Into Total = Sum(a.AmountDeal)
                    Select New SUMDeal With {.CIF = CIF, _
                                             .DealPeriod = DealPeriod, _
                                             .TotalDeal = Total}

        Dim tmp3 = From a In tmp2
                    Group By a.CIF Into TotalAll = Sum(a.TotalDeal)
                    Select New SUMAll With {.CIF = CIF, _
                                            .TotalAllDeal = TotalAll}

        Dim tmp4 = From a In db.Accounts
                    Group Join b In tmp2 On a.CIF Equals b.CIF Into ab = Group
                    Group Join c In tmp3 On a.CIF Equals c.CIF Into ac = Group
                    Where a.Status = "ACTIVE"
                    Group By a.CIF, a.Name Into TotalEqUSD = Sum(If(ac.FirstOrDefault.TotalAllDeal Is Nothing, 0, ac.FirstOrDefault.TotalAllDeal))
                    Select New LimitTransaction With {.CIF = CIF, _
                                                     .AccName = Name}


        If Request("sSearch_0") <> "" Or Request("sSearch_1") <> "" Then
            Return ReturnLimitTransactionDataTable(param, tmp4)
        Else
            Return ReturnLimitTransactionDataTable(param, Nothing)
        End If

    End Function

    Private Function ReturnLimitTransactionDataTable(param As jQueryDataTableParamModel, LimitTransaction As IQueryable(Of LimitTransaction)) As JsonResult
        Dim totalRecords = 0

        If LimitTransaction IsNot Nothing Then
            totalRecords = LimitTransaction.Count
        End If

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim CIFSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim AccNameSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))

        Dim result As New List(Of String())

        If LimitTransaction IsNot Nothing Then
            LimitTransaction = LimitTransaction.Where(Function(f) f.CIF.Contains(CIFSearch))
            LimitTransaction = LimitTransaction.Where(Function(f) f.AccName.Contains(AccNameSearch))

            'Detection of sorted column
            Dim sortOrder As String = Request("sSortDir_0")
            Select Case Request("iSortCol_0")
                Case 0
                    If sortOrder = "asc" Then
                        LimitTransaction = LimitTransaction.OrderBy(Function(f) f.CIF)
                    Else
                        LimitTransaction = LimitTransaction.OrderByDescending(Function(f) f.CIF)
                    End If
                Case 1
                    If sortOrder = "asc" Then
                        LimitTransaction = LimitTransaction.OrderBy(Function(f) f.AccName)
                    Else
                        LimitTransaction = LimitTransaction.OrderByDescending(Function(f) f.AccName)
                    End If
            End Select


            For Each data As LimitTransaction In LimitTransaction
                result.Add({data.CIF, data.AccName, ""})
            Next
        End If


        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

    ' GET: /InquiryLimitTrans/BrowseDetailDeal
    <Authorize> _
    Public Function BrowseDetailDeal(ByVal cif As String, ByVal accname As String) As ActionResult
        ViewBag.cif = cif
        ViewBag.accname = accname.Replace("%22", "'")

        Return PartialView("_DetailDealBrowser")
    End Function

    Private Class Detail
        Public Property DealDate As Date
        Public Property DealNumber As String
        Public Property AccNo As String
        Public Property DealPeriod As String
        Public Property CurrencyDeal As String
        Public Property CurrencyCustomer As String
        Public Property Amount As Decimal
        Public Property EqUSD As Decimal
    End Class

    <Authorize> _
    <HttpGet> _
    Public Function DTDetailDealAjaxHandler(param As jQueryDataTableParamModel, cif As String, accname As String) As JsonResult
        Dim db As New FectorEntities()

        If IsNothing(cif) Then
            cif = ""
        End If
        If IsNothing(accname) Then
            accname = ""
        End If

        Dim TDateSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim DealNumberSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim AccNoSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        'Dim CurrDealSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))

        Dim TransDeal = From a In db.TransactionDeal
                        Join b In db.ExchangeRate On a.CurrencyDeal Equals b.CurrId
                        Join c In db.Accounts On a.AccNum Equals c.AccNo
                        Where (a.Status <> "REJECTED" And a.Status <> "INACTIVE") And c.CIF = cif And a.TransactionType = "Sell"
                        Order By a.DealDate Descending
                        Select New Detail With {.DealDate = a.DealDate, _
                                                .DealNumber = a.DealNumber, _
                                                .AccNo = c.AccNo, _
                                                .DealPeriod = a.DealPeriod, _
                                                .CurrencyDeal = a.CurrencyDeal, _
                                                .CurrencyCustomer = a.CurrencyCustomer, _
                                                .Amount = a.AmountDeal, _
                                                .EqUSD = If(a.TransactionType = "Sell",
                                                            If(a.DealType = "TT",
                                                               (a.AmountDeal * b.TTSellRate) / db.ExchangeRate.Where(Function(f) f.CurrId = "USD").FirstOrDefault.TTSellRate,
                                                               (a.AmountDeal * b.BNSellRate) / db.ExchangeRate.Where(Function(f) f.CurrId = "USD").FirstOrDefault.BNSellRate),
                                                            If(a.DealType = "TT",
                                                               (a.AmountDeal * b.TTBuyRate) / db.ExchangeRate.Where(Function(f) f.CurrId = "USD").FirstOrDefault.TTBuyRate,
                                                               (a.AmountDeal * b.BNBuyRate) / db.ExchangeRate.Where(Function(f) f.CurrId = "USD").FirstOrDefault.BNBuyRate))}

        If TDateSearch.Trim.Length > 0 Then
            ViewBag.period = TDateSearch

            Dim filterMonth As String = TDateSearch.Split("-")(0)
            Dim filterYear As String = TDateSearch.Split("-")(1)

            TransDeal = TransDeal.Where(Function(f) f.DealDate.Month = filterMonth And f.DealDate.Year = filterYear)
        End If
        TransDeal = TransDeal.Where(Function(f) f.DealNumber.Contains(DealNumberSearch))
        TransDeal = TransDeal.Where(Function(f) f.AccNo.Contains(AccNoSearch))

        Dim ListDetailDeal As New List(Of Detail)

        For Each row As Detail In TransDeal
            ListDetailDeal.Add(New Detail With {.DealDate = row.DealDate, .DealNumber = row.DealNumber, .AccNo = row.AccNo, .DealPeriod = row.DealPeriod, .CurrencyDeal = row.CurrencyDeal & " " & CDec(row.Amount).ToString("N2"), .CurrencyCustomer = row.CurrencyCustomer, .Amount = row.Amount, .EqUSD = row.EqUSD})
        Next

        Return ReturnDetailDealDataTable(param, ListDetailDeal)
    End Function

    Private Function ReturnDetailDealDataTable(param As jQueryDataTableParamModel, SelectDetailDeal As List(Of Detail)) As JsonResult
        Dim totalRecords As Integer = SelectDetailDeal.Count
        Dim displayRecord As Integer = SelectDetailDeal.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim showed = (From acc In SelectDetailDeal.Skip(iDisplayStart).Take(iDisplayLength)
                      Select acc).ToList

        Dim result As New List(Of String())
        For Each code As Detail In showed
            result.Add({code.DealPeriod + " " + (code.DealDate).ToString("dd-MM-yyyy"), code.DealNumber, code.AccNo, code.CurrencyDeal, CDec(code.EqUSD).ToString("N2"), ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = displayRecord,
                      .aaData = result},
                      JsonRequestBehavior.AllowGet)
    End Function

    ' POST : /InquiryLimit/RptDetailInquiryLimit
    <Authorize> _
    Public Function RptDetailInquiryLimit(ByVal cif As String, ByVal accname As String, ByVal period As String) As ActionResult
        Dim db As New FectorEntities()

        Dim TransDeal = From a In db.TransactionDeal
                        Join b In db.ExchangeRate On a.CurrencyDeal Equals b.CurrId
                        Join c In db.Accounts On a.AccNum Equals c.AccNo
                        Where a.Status <> "REJECTED" And c.CIF = cif And a.TransactionType = "Sell"
                        Order By a.DealDate
                        Select New Detail With {.DealDate = a.DealDate, _
                                                .DealNumber = a.DealNumber, _
                                                .AccNo = c.AccNo, _
                                                .DealPeriod = a.DealPeriod, _
                                                .CurrencyDeal = a.CurrencyDeal, _
                                                .CurrencyCustomer = a.CurrencyCustomer, _
                                                .Amount = a.AmountDeal, _
                                                .EqUSD = If(a.TransactionType = "Sell",
                                                            If(a.DealType = "TT",
                                                               (a.AmountDeal * b.TTSellRate) / db.ExchangeRate.Where(Function(f) f.CurrId = "USD").FirstOrDefault.TTSellRate,
                                                               (a.AmountDeal * b.BNSellRate) / db.ExchangeRate.Where(Function(f) f.CurrId = "USD").FirstOrDefault.BNSellRate),
                                                            If(a.DealType = "TT",
                                                               (a.AmountDeal * b.TTBuyRate) / db.ExchangeRate.Where(Function(f) f.CurrId = "USD").FirstOrDefault.TTBuyRate,
                                                               (a.AmountDeal * b.BNBuyRate) / db.ExchangeRate.Where(Function(f) f.CurrId = "USD").FirstOrDefault.BNBuyRate))}

        If period <> "" And period IsNot Nothing And period <> "undefined" Then
            Dim filterMonth As String = period.Split("-")(0)
            Dim filterYear As String = period.Split("-")(1)

            TransDeal = TransDeal.Where(Function(f) f.DealDate.Month = filterMonth And f.DealDate.Year = filterYear)
        End If

        Dim ListDetailDeal As New List(Of Detail)

        For Each row As Detail In TransDeal
            ListDetailDeal.Add(New Detail With {.DealDate = row.DealDate, .DealNumber = row.DealNumber, .AccNo = row.AccNo, .DealPeriod = row.DealPeriod, .CurrencyDeal = row.CurrencyDeal, .CurrencyCustomer = row.CurrencyCustomer, .Amount = row.Amount, .EqUSD = row.EqUSD})
        Next

        Dim rptDetailInquriryLimit As New ReportDocument
        rptDetailInquriryLimit.Load(Path.Combine(Server.MapPath("~/Reports"), "RptDetailInquiryLimit.rpt"))
        rptDetailInquriryLimit.SetDataSource(ListDetailDeal)
        rptDetailInquriryLimit.SetParameterValue(0, cif)
        rptDetailInquriryLimit.SetParameterValue(1, accname)

        Response.Buffer = False
        Response.ClearContent()
        Response.ClearHeaders()

        Try
            Dim stream As Stream = rptDetailInquriryLimit.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat)
            stream.Seek(0, SeekOrigin.Begin)
            Return File(stream, "application/pdf", "RptDetailInquiryLimit- " & cif & ".pdf")
        Catch ex As Exception
            Throw
        End Try
    End Function
End Class