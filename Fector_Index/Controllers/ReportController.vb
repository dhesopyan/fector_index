Imports System.IO
Imports CrystalDecisions.CrystalReports.Engine
Imports System.Data.Objects
Imports CrystalDecisions.Shared
Imports System.Data.Objects.SqlClient
Imports Microsoft.VisualBasic
Imports OfficeOpenXml
Imports Ionic.Zip

Public Class ReportController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /ReportLHBU

    Function IndexLHBU() As ActionResult
        Return View("IndexLHBU")
    End Function

    '
    ' GET: /ReportNonLHBU

    Function IndexNonLHBU() As ActionResult
        Return View("IndexNonLHBU")
    End Function


    ' GET: /ReportDeal

    Function IndexDeal() As ActionResult
        Return View("IndexDeal")
    End Function


    ' GET: /ReportDataKurs

    Function IndexDataKurs() As ActionResult
        Dim db As New FectorEntities
        ViewBag.Currency = New SelectList(db.Currencies, "CurrId", "CurrencyDisplay")
        Return View("IndexDataKurs")
    End Function

    ' GET: /ReportTransRecap

    Function IndexTransRecap() As ActionResult
        Return View("IndexTransRecap")
    End Function

    ' GET: /ReportAuditTrailTransaction

    Function IndexAuditTrailTransaction() As ActionResult
        Return View("IndexAuditTrailTransaction")
    End Function

    ' GET: /ReportAuditTrailUser

    Function IndexAuditTrailUser() As ActionResult
        Return View("IndexAuditTrailUser")
    End Function

    ' GET: /ReportExchangeTransaction

    Function IndexExchangeTransaction() As ActionResult
        Return View("IndexExchangeTransaction")
    End Function

    ' GET: /ReportExchangeTransaction

    Function IndexDailyProfit() As ActionResult
        Return View("IndexDailyProfit")
    End Function

    ' GET: /ReportExportTransaction

    Function IndexExportTransaction() As ActionResult
        Return View("IndexExportTransaction")
    End Function

    'GET : /ReportTransactionLimit
    Private Class GetBranch
        Public Property BranchId As String
        Public Property BranchName As String
    End Class

    Function IndexTransactionLimit() As ActionResult
        Dim db As New FectorEntities

        Dim tl = (From b In db.Branches _
                 Select New GetBranch With {.BranchId = b.BranchId, .BranchName = b.Name}).ToList

        Dim ListBranch As New List(Of GetBranch)

        ListBranch.Add(New GetBranch With {.BranchId = "-1", .BranchName = "ALL"})
        For i As Integer = 0 To tl.Count - 1
            ListBranch.Add(New GetBranch With {.BranchId = tl.Item(i).BranchId, .BranchName = tl.Item(i).BranchName})
        Next

        ViewBag.branch = New SelectList(ListBranch, "BranchId", "BranchName")

        Return View("IndexTransactionLimit")
    End Function


    Private Class RecordLHBU
        Public IDOperational As String
        Public TDate As DateTime
        Public RefNum As String
        Public AccName As String
        Public BaseCurrency As String
        Public CounterCurrency As String
        Public TransactionRate As String
        Public Volume As String
        Public BSubjectStatus As String
        Public TransactionType As String
        Public BBankCode As String
        Public BName As String
        Public BNonBankCode As String
        Public SSubjectStatus As String
        Public SBankCode As String
        Public SName As String
        Public SNonBankCode As String
        Public ValueDate As String
        Public DueDate As String
        Public ValuePeriod As String
        Public TransactionTime As String
        Public Purpose As String
        Public BBusinessType As String
        Public SBusinessType As String
        Public JoinTransAmount As String
        Public BCountry As String
        Public SCountry As String
        Public BNPWP As String
        Public DocumentTransId As String
        Public DocumentLHBUId As String
        Public DocumentType As String
        Public DocumentNotes As String
        Public LastTransNum As String
        Public LastTDate As String
        Public IsNetting As String
        Public DerivativeType As String
        Public NettingType As String
        Public NettingVolume As String
        Public EditBy As String
        Public Status As String
    End Class

    '
    ' POST : /Report/RptLHBU

    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function RptLHBU(model As RptLHBUView) As ActionResult
        Dim db As New FectorEntities


        If ModelState.IsValid Then
            Dim MemoryStream As New MemoryStream
            Dim Writer As TextWriter
            Dim sb As New StringBuilder
            Dim dTDate As Date = AppHelper.dateConvert(model.Period)

            'LINQ Header
            Dim RHeader = (From h In db.Settings
                           Where h.Key1 = "Bank"
                           Select h).ToList

            'Header
            Dim ReportHeader As String = ""
            Dim BankCode As String = ""
            Dim BankBusinessType As String = ""
            Dim BankNPWP As String = ""
            Dim ReportDate As String = ""
            Dim FormNumber As String = ""
            Dim RecordCount As String = ""

            BankCode = RHeader.Item(2).Value1
            BankBusinessType = RHeader.Item(3).Value1
            BankNPWP = RHeader.Item(6).Value1
            ReportDate = Now.ToString("ddMMyyy")

            Dim acc = (From a In db.Accounts
                       Where a.flagNonLHBU = False
                      Select New With {.AccNo = a.AccNo, .NPWP = a.NPWP}).Union(
                      From b In db.OtherAccounts
                      Where b.flagNonLHBU = False
                      Select New With {.AccNo = b.AccNo, .NPWP = ""})

            Dim LastTrans = From a In db.ExchangeTransactionHead
                            Where a.IsNetting = True
                            Group a By a.AccNum Into g = Group
                            Select New With {.TransNum = g.Max(Function(f) f.TransNum), _
                                            .AccNUm = AccNum, _
                                            .MaxDate = g.Max(Function(f) f.TDate)}
            'LINQ Record
            Dim tmp = (From r In db.ExchangeTransactionDetail
                          Join h In db.ExchangeTransactionHead On r.TransNum Equals h.TransNum
                          Group Join b In db.ExchangeTransactionDoc On r.TransNum Equals b.TransNum Into nb = Group
                          Join a In acc On h.AccNum Equals a.AccNo
                          Join c In db.TransactionDeal On r.DealNumber Equals c.DealNumber
                          Group Join m In LastTrans On h.TransNum Equals m.TransNum Into Group From m In Group.DefaultIfEmpty
                          Where h.RateType = "TT" And ((h.AccNum <> "1000000000" Or h.TransactionType = "Buy")) And h.TDate.Day = dTDate.Day And h.TDate.Month = dTDate.Month And h.TDate.Year = dTDate.Year And h.Status.Contains("ACTIVE") And r.ValuePeriod <> "FWD"
                          Select New RecordLHBU With {.IDOperational = "", _
                                                      .TDate = h.TDate, _
                                                      .AccName = h.AccName, _
                                                      .RefNum = h.TransNum, _
                                                      .BaseCurrency = r.TransactionCurrency, _
                                                      .CounterCurrency = r.CustomerCurrency, _
                                                      .TransactionRate = r.TransactionRate, _
                                                      .Volume = r.TransactionNominal, _
                                                      .BSubjectStatus = h.BSubjectStatusId, _
                                                      .BBankCode = h.BBankCode, _
                                                      .TransactionType = h.TransactionType, _
                                                      .BName = "", _
                                                      .BNonBankCode = "", _
                                                      .SSubjectStatus = h.SSubjectStatusId, _
                                                      .SBankCode = h.SBankCode, _
                                                      .SName = "", _
                                                      .SNonBankCode = "", _
                                                      .ValueDate = r.ValueDate, _
                                                      .DueDate = r.ValueDate, _
                                                      .ValuePeriod = r.ValuePeriod, _
                                                      .TransactionTime = h.TDate, _
                                                      .Purpose = nb.DefaultIfEmpty.FirstOrDefault.PurposeId, _
                                                      .BBusinessType = h.BBusinessTypeId, _
                                                      .SBusinessType = h.SBusinessTypeId, _
                                                      .JoinTransAmount = "0", _
                                                      .BCountry = h.BCountryId, _
                                                      .SCountry = h.SCountryId, _
                                                      .BNPWP = a.NPWP, _
                                                      .DocumentTransId = nb.DefaultIfEmpty.FirstOrDefault.DocumentTransId, _
                                                      .DocumentLHBUId = nb.DefaultIfEmpty.FirstOrDefault.DocumentLHBUId, _
                                                      .DocumentType = "", _
                                                      .DocumentNotes = "", _
                                                      .LastTransNum = m.TransNum, _
                                                      .LastTDate = m.MaxDate, _
                                                      .IsNetting = h.IsNetting, _
                                                      .DerivativeType = h.DerivativeType, _
                                                      .NettingType = h.NettingType, _
                                                      .NettingVolume = r.TransactionNominal})

            Dim RRecord = ((From r In db.ExchangeTransactionDetail
                          Join h In db.ExchangeTransactionHead On r.TransNum Equals h.TransNum
                          Group Join b In db.ExchangeTransactionDoc On r.TransNum Equals b.TransNum Into nb = Group
                          Join a In acc On h.AccNum Equals a.AccNo
                          Join c In db.TransactionDeal On r.DealNumber Equals c.DealNumber
                          Group Join m In LastTrans On h.TransNum Equals m.TransNum Into Group From m In Group.DefaultIfEmpty
                          Where h.RateType = "TT" And ((h.AccNum <> "1000000000" Or h.TransactionType = "Buy")) And h.TDate.Day = dTDate.Day And h.TDate.Month = dTDate.Month And h.TDate.Year = dTDate.Year And h.Status.Contains("ACTIVE") And r.ValuePeriod <> "FWD"
                          Select New RecordLHBU With {.IDOperational = "", _
                                                      .TDate = h.TDate, _
                                                      .AccName = h.AccName, _
                                                      .RefNum = h.TransNum, _
                                                      .BaseCurrency = r.TransactionCurrency, _
                                                      .CounterCurrency = r.CustomerCurrency, _
                                                      .TransactionRate = r.TransactionRate, _
                                                      .Volume = r.TransactionNominal, _
                                                      .BSubjectStatus = h.BSubjectStatusId, _
                                                      .BBankCode = h.BBankCode, _
                                                      .TransactionType = h.TransactionType, _
                                                      .BName = "", _
                                                      .BNonBankCode = "", _
                                                      .SSubjectStatus = h.SSubjectStatusId, _
                                                      .SBankCode = h.SBankCode, _
                                                      .SName = "", _
                                                      .SNonBankCode = "", _
                                                      .ValueDate = r.ValueDate, _
                                                      .DueDate = r.ValueDate, _
                                                      .ValuePeriod = r.ValuePeriod, _
                                                      .TransactionTime = h.TDate, _
                                                      .Purpose = nb.DefaultIfEmpty.FirstOrDefault.PurposeId, _
                                                      .BBusinessType = h.BBusinessTypeId, _
                                                      .SBusinessType = h.SBusinessTypeId, _
                                                      .JoinTransAmount = "0", _
                                                      .BCountry = h.BCountryId, _
                                                      .SCountry = h.SCountryId, _
                                                      .BNPWP = a.NPWP, _
                                                      .DocumentTransId = nb.DefaultIfEmpty.FirstOrDefault.DocumentTransId, _
                                                      .DocumentLHBUId = nb.DefaultIfEmpty.FirstOrDefault.DocumentLHBUId, _
                                                      .DocumentType = "", _
                                                      .DocumentNotes = "", _
                                                      .LastTransNum = m.TransNum, _
                                                      .LastTDate = m.MaxDate, _
                                                      .IsNetting = h.IsNetting, _
                                                      .DerivativeType = h.DerivativeType, _
                                                      .NettingType = h.NettingType, _
                                                      .NettingVolume = r.TransactionNominal})).ToList

            If RRecord.Count <> 0 Then
                'Record
                Dim ReportRecord As String = ""

                Dim IDOperational As String = ""
                Dim RefNum As String = ""
                Dim BaseCurrency As String = ""
                Dim CounterCurrency As String = ""
                Dim TransactionRate As String = ""
                Dim Volume As String = ""
                Dim BSubjectStatus As String = ""
                Dim BBankCode As String = ""
                Dim BName As String = ""
                Dim BNonBankCode As String = ""
                Dim SSubjectStatus As String = ""
                Dim SBankCode As String = ""
                Dim SName As String = ""
                Dim SNonBankCode As String = ""
                Dim ValueDate As String = ""
                Dim DueDate As String = ""
                Dim ValuePeriod As String = ""
                Dim TransactionTime As String = ""
                Dim Purpose As String = ""
                Dim BBusinessType As String = ""
                Dim SBusinessType As String = ""
                Dim JoinTransAmount As String = ""
                Dim BCountry As String = ""
                Dim SCountry As String = ""
                Dim BNPWP As String = ""
                Dim DocumentType As String = ""
                Dim DocumentNotes As String = ""
                Dim LastTransNum As String = ""
                Dim LastTDate As String = ""
                Dim IsNetting As String = ""
                Dim DerivativeType As String = ""
                Dim NettingType As String = ""
                Dim NettingVolume As String = ""

                Writer = New StreamWriter(MemoryStream)

                'Fill Report Header
                RecordCount = RRecord.Count.ToString.PadLeft(8, "0")

                ReportHeader = BankCode + "" + BankBusinessType + "" + ReportDate + "" + "201" + RecordCount

                sb.AppendLine(ReportHeader.TrimEnd)

                'Fill Report Record
                For i As Integer = 0 To RRecord.Count - 1
                    'ID Operasional
                    IDOperational = "1"

                    'No. Referensi
                    'dari core banking
                    Dim GetDate As String = CStr(Now.Date.ToString("yyyyMMdd"))
                    Dim count As String = CStr(i + 1).ToString.PadLeft(15 - GetDate.Length, "0")
                    RefNum = "L" & GetDate & count

                    'Mata Uang Dasar
                    BaseCurrency = RRecord.Item(i).BaseCurrency

                    'Mata Uang Lawan
                    CounterCurrency = RRecord.Item(i).CounterCurrency

                    'Kurs Transaksi
                    Dim array As Array = RRecord.Item(i).TransactionRate.ToString.Split(CChar("."))
                    Dim NumericTransactionRate As String = array(0).ToString.PadLeft(5, "0")
                    Dim DecimalTransactionRate As String = array(1).ToString.PadRight(4, "0")

                    TransactionRate = NumericTransactionRate & DecimalTransactionRate

                    'Volume (mata uang dasar)
                    Volume = CInt(RRecord.Item(i).Volume).ToString.PadLeft(16, "0")

                    'Status Pembeli
                    If RRecord.Item(i).BSubjectStatus <> "110" Or RRecord.Item(i).BSubjectStatus <> "120" Or RRecord.Item(i).BSubjectStatus <> "130" Or RRecord.Item(i).BSubjectStatus <> "140" Or RRecord.Item(i).BSubjectStatus <> "150" Then
                        BSubjectStatus = RRecord.Item(i).BSubjectStatus
                    End If

                    Dim TransactionType As String = RRecord.Item(i).TransactionType

                    'Sandi Pembeli
                    If BSubjectStatus = "130" Or BSubjectStatus = "140" Then
                        BBankCode = "".ToString.PadRight(3, " ")
                    Else
                        If TransactionType = "Sell" Then
                            BBankCode = RRecord.Item(i).BBankCode
                        Else
                            BBankCode = RHeader.Item(2).Value1
                        End If
                    End If

                    'Nama Pembeli
                    If TransactionType = "Sell" Then
                        BName = RRecord.Item(i).AccName.ToString.PadRight(35, " ").Substring(0, 35)
                    Else
                        BName = RHeader.Item(4).Value1.ToString.PadRight(35, " ").Substring(0, 35)
                    End If

                    'Sandi Pembeli Non Bank
                    If BSubjectStatus = "110" Or BSubjectStatus = "120" Or BSubjectStatus = "150" Then
                        BNonBankCode = "".ToString.PadRight(15, " ")
                    Else
                        BNonBankCode = RRecord.Item(i).BBankCode.PadRight(15, " ")
                    End If

                    'Status Penjual
                    If RRecord.Item(i).SSubjectStatus <> "110" Or RRecord.Item(i).SSubjectStatus <> "120" Or RRecord.Item(i).SSubjectStatus <> "130" Or RRecord.Item(i).SSubjectStatus <> "140" Or RRecord.Item(i).SSubjectStatus <> "150" Then
                        SSubjectStatus = RRecord.Item(i).SSubjectStatus
                    End If

                    'Sandi Penjual
                    If SSubjectStatus = "130" Or SSubjectStatus = "140" Then
                        SBankCode = "".ToString.PadRight(3, " ")
                    Else
                        If TransactionType = "Sell" Then
                            SBankCode = RHeader.Item(2).Value1
                        Else
                            SBankCode = RRecord.Item(i).SBankCode
                        End If
                    End If

                    'Nama Penjual
                    If TransactionType = "Sell" Then
                        SName = RHeader.Item(4).Value1.ToString.PadRight(35, " ").Substring(0, 35)
                    Else
                        SName = RRecord.Item(i).AccName.ToString.PadRight(35, " ").Substring(0, 35)
                    End If

                    'Sandi Penjual Non Bank
                    If SSubjectStatus = "110" Or SSubjectStatus = "120" Or SSubjectStatus = "150" Then
                        SNonBankCode = "".ToString.PadRight(15, " ")
                    Else
                        SNonBankCode = RRecord.Item(i).SBankCode.PadRight(15, " ")
                    End If

                    'Tanggal Valuta
                    ValueDate = CStr(CDate(RRecord.Item(i).ValueDate).ToString("ddMMyyyy"))

                    'Tanggal Jatuh Tempo
                    DueDate = CStr(CDate(RRecord.Item(i).DueDate).ToString("ddMMyyyy"))

                    'Jangka Waktu
                    If RRecord.Item(i).ValuePeriod = "TOD" Then
                        ValuePeriod = "00"
                    ElseIf RRecord.Item(i).ValuePeriod = "TOM" Then
                        ValuePeriod = "01"
                    Else
                        ValuePeriod = "02"
                    End If

                    'Jam Transaksi
                    TransactionTime = CStr(Now.Hour.ToString.PadLeft(2, "0")) + CStr(Now.Minute.ToString.PadLeft(2, "0"))

                    'Tujuan
                    Purpose = RRecord.Item(i).Purpose

                    'Jenis Kegiatan Usaha Pembeli
                    If BSubjectStatus = "110" Then
                        If RRecord.Item(i).BBusinessType <> "01" And RRecord.Item(i).BBusinessType <> "08" Then
                            BBusinessType = "00"
                        Else
                            BBusinessType = RRecord.Item(i).BBusinessType
                        End If
                    Else
                        BBusinessType = "00"
                    End If

                    'Jenis Kegiatan Usaha Penjual
                    If SSubjectStatus = "110" Then
                        If RRecord.Item(i).SBusinessType <> "01" And RRecord.Item(i).SBusinessType <> "08" Then
                            SBusinessType = "00"
                        Else
                            SBusinessType = RRecord.Item(i).SBusinessType
                        End If
                    Else
                        SBusinessType = "00"
                    End If

                    'Jumlah Transaksi Yang Digabung
                    JoinTransAmount = RRecord.Item(i).JoinTransAmount.PadRight(4, "0")

                    'Sandi Negara Pembeli
                    If BSubjectStatus = "110" Or BSubjectStatus = "130" Or BSubjectStatus = "140" Then
                        BCountry = "ID"
                    ElseIf BSubjectStatus = "120" Or BSubjectStatus = "150" Then
                        BCountry = RRecord.Item(i).BCountry
                    Else
                        BCountry = "".ToString.PadRight(2, " ")
                    End If

                    'Sandi Negara Penjual
                    If SSubjectStatus = "110" Or SSubjectStatus = "130" Or SSubjectStatus = "140" Then
                        SCountry = "ID"
                    ElseIf SSubjectStatus = "120" Or SSubjectStatus = "150" Then
                        SCountry = RRecord.Item(i).SCountry
                    Else
                        SCountry = "".ToString.PadRight(2, " ")
                    End If

                    'NPWP Pembeli
                    If TransactionType = "Sell" Then
                        BNPWP = RRecord.Item(i).BNPWP.PadLeft(15, " ")
                    Else
                        BNPWP = RHeader.Item(6).Value1.PadLeft(15, " ")
                    End If

                    'Jenis Dokumen
                    If IIf(IsDBNull(RRecord.Item(i).DocumentLHBUId), "", RRecord.Item(i).DocumentLHBUId) = "" Then
                        DocumentType = "998"
                    Else
                        DocumentType = RRecord.Item(i).DocumentLHBUId
                    End If

                    'Dim DocumentTransID As String = RRecord.Item(i).DocumentTransId
                    'Dim DocumentLHBU = (From lhbu In db.MappingDocument
                    '                   Where lhbu.DocumentTransId = DocumentTransID
                    '                   Select lhbu).ToList

                    'For l As Integer = 0 To DocumentLHBU.Count - 1
                    '    DocumentType += DocumentLHBU.Item(l).DocumentLHBUId
                    'Next

                    'Keterangan Jenis Dokumen
                    If DocumentType = "999" Then
                        DocumentNotes = RRecord.Item(i).DocumentNotes.ToString.PadRight(100, " ")
                    Else
                        DocumentNotes = "".ToString.PadRight(100, " ")
                    End If

                    IsNetting = RRecord.Item(i).IsNetting

                    If IsNetting = True Then
                        LastTransNum = RRecord.Item(i).LastTransNum.ToString.PadLeft(16, "0")
                        LastTDate = CStr(CDate(RRecord.Item(i).LastTDate).ToString("ddMMyyyy"))
                        DerivativeType = RRecord.Item(i).DerivativeType.ToString
                        NettingType = RRecord.Item(i).NettingType.ToString
                        NettingVolume = CInt(RRecord.Item(i).NettingVolume).ToString.PadLeft(16, "0")
                    Else
                        LastTransNum = "".ToString.PadLeft(16, "")
                        LastTDate = CStr("").ToString.PadLeft(8, "0")
                        DerivativeType = "".ToString.PadLeft(2, " ")
                        NettingType = "".ToString.PadLeft(2, " ")
                        NettingVolume = "".ToString.PadLeft(16, "0")
                    End If


                    ReportRecord = IDOperational + RefNum + BaseCurrency + CounterCurrency + CStr(TransactionRate) + CStr(Volume) _
                                    + BSubjectStatus + BBankCode + BName + BNonBankCode + SSubjectStatus + SBankCode _
                                    + SName + SNonBankCode + ValueDate + DueDate + ValuePeriod + TransactionTime _
                                    + Purpose + BBusinessType + SBusinessType + CStr(JoinTransAmount) + BCountry + SCountry _
                                    + BNPWP + DocumentType + DocumentNotes + LastTransNum + LastTDate + DerivativeType + NettingType + NettingVolume

                    sb.AppendLine(ReportRecord.TrimEnd)
                Next

                Writer.WriteLine(sb.ToString)
                Writer.Flush()
                Writer.Close()

                Return File(MemoryStream.GetBuffer(), "text/plain", "LHBU201-" & CStr(Now.Date.ToString("ddMMyy")) & ".txt")
            Else
                ModelState.AddModelError("Period", "There is no data in this period")

                Return View("IndexLHBU", model)
            End If
        Else
            Return View("IndexLHBU", model)
        End If
    End Function

    '
    ' POST : /Report/RptNonLHBU
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function RptNonLHBU(model As RptLHBUView) As ActionResult
        Dim db As New FectorEntities

        If ModelState.IsValid Then
            Dim MemoryStream As New MemoryStream
            Dim Writer As TextWriter
            Dim sb As New StringBuilder
            Dim dTDate As Date = AppHelper.dateConvert(model.Period)

            'LINQ Header
            Dim CoreNonTrx = (From a In db.Settings
                              Where a.Key1 = "CoreNonTrx"
                              Select a).ToList

            Dim CoreNonTrx_CounterCurrency As String = CoreNonTrx.Item(0).Value1
            Dim CoreNonTrx_SSubjectStatus As String = CoreNonTrx.Item(1).Value1
            Dim CoreNonTrx_SName As String = CoreNonTrx.Item(2).Value1
            Dim CoreNonTrx_SBusinessType As String = CoreNonTrx.Item(3).Value1
            Dim CoreNonTrx_SCountry As String = CoreNonTrx.Item(4).Value1
            Dim CoreNonTrx_DocumentLHBU As String = CoreNonTrx.Item(5).Value1
            Dim CoreNonTrx_ValuePeriod As String = CoreNonTrx.Item(6).Value1


            Dim RHeader = (From h In db.Settings
                           Where h.Key1 = "Bank"
                           Select h).ToList

            Dim CoreNonTrx_BCountry As String = RHeader.Item(0).Value1
            Dim CoreNonTrx_BSubjectStatus As String = RHeader.Item(1).Value1
            Dim CoreNonTrx_BBankCode As String = RHeader.Item(2).Value1
            Dim CoreNonTrx_BBusinessType As String = RHeader.Item(3).Value1
            Dim CoreNonTrx_BBankName As String = RHeader.Item(4).Value1

            'LINQ Record
            Dim CountNonTrx = (From a In db.CoreNonTrx
                               Where a.TDate.Day = dTDate.Day And a.TDate.Month = dTDate.Month And a.TDate.Year = dTDate.Year And a.FlagLHBU = 1
                               Select a).Count

            If CountNonTrx <> 0 Then
                Dim AVGAmount = Aggregate a In db.CoreNonTrx
                            Where a.TDate.Day = dTDate.Day And a.TDate.Month = dTDate.Month And a.TDate.Year = dTDate.Year And a.FlagLHBU = 1
                            Into Sum(a.Amount)

                Dim tmpAVGAmount As Decimal = AVGAmount
                AVGAmount = CInt(Math.Round(tmpAVGAmount, 1, MidpointRounding.AwayFromZero))

                Dim RRecord = (From a In db.CoreNonTrx
                             Join b In db.Branches On a.BranchId Equals b.BranchId
                             Join c In db.Accounts On a.AccNo Equals c.AccNo
                             Join d In db.Currencies On a.CoreCurrId Equals d.CoreCurrId
                             Where a.TDate.Day = dTDate.Day And a.TDate.Month = dTDate.Month And a.TDate.Year = dTDate.Year And a.FlagLHBU = 1
                             Group By a.TDate, d.CurrId, a.Rate, a.Time Into g = Group
                            Select New RecordLHBU With {.IDOperational = "", _
                                                          .TDate = TDate, _
                                                          .AccName = "GABUNGAN", _
                                                          .RefNum = "", _
                                                          .BaseCurrency = CurrId, _
                                                          .CounterCurrency = CoreNonTrx_CounterCurrency, _
                                                          .TransactionRate = Rate, _
                                                          .Volume = AVGAmount, _
                                                          .BSubjectStatus = CoreNonTrx_BSubjectStatus, _
                                                          .BBankCode = CoreNonTrx_BBankCode, _
                                                          .TransactionType = "GABUNGAN", _
                                                          .BName = CoreNonTrx_BBankName, _
                                                          .BNonBankCode = "", _
                                                          .SSubjectStatus = CoreNonTrx_SSubjectStatus, _
                                                          .SBankCode = "", _
                                                          .SName = CoreNonTrx_SName, _
                                                          .SNonBankCode = "999999", _
                                                          .ValueDate = TDate, _
                                                          .DueDate = TDate, _
                                                          .ValuePeriod = CoreNonTrx_ValuePeriod, _
                                                          .TransactionTime = Time, _
                                                          .Purpose = "33", _
                                                          .BBusinessType = CoreNonTrx_BBusinessType, _
                                                          .SBusinessType = CoreNonTrx_SBusinessType, _
                                                          .JoinTransAmount = CountNonTrx, _
                                                          .BCountry = CoreNonTrx_BCountry, _
                                                          .SCountry = CoreNonTrx_SCountry, _
                                                          .BNPWP = "", _
                                                          .DocumentTransId = "1", _
                                                          .DocumentType = CoreNonTrx_DocumentLHBU, _
                                                          .DocumentNotes = "-", _
                                                          .LastTransNum = "", _
                                                          .LastTDate = "", _
                                                          .IsNetting = False, _
                                                          .DerivativeType = "", _
                                                          .NettingType = "", _
                                                          .NettingVolume = ""}).ToList

                'Header
                Dim ReportHeader As String = ""

                Dim BankCode As String = ""
                Dim BankBusinessType As String = ""
                Dim ReportDate As String = ""
                Dim FormNumber As String = ""
                Dim RecordCount As String = ""

                'Record
                Dim ReportRecord As String = ""

                Dim IDOperational As String = ""
                Dim RefNum As String = ""
                Dim BaseCurrency As String = ""
                Dim CounterCurrency As String = ""
                Dim TransactionRate As Decimal = 0
                Dim Volume As String = ""
                Dim BSubjectStatus As String = ""
                Dim BBankCode As String = ""
                Dim BName As String = ""
                Dim BNonBankCode As String = ""
                Dim SSubjectStatus As String = ""
                Dim SBankCode As String = ""
                Dim SName As String = ""
                Dim SNonBankCode As String = ""
                Dim ValueDate As String = ""
                Dim DueDate As String = ""
                Dim ValuePeriod As String = ""
                Dim TransactionTime As String = ""
                Dim Purpose As String = ""
                Dim BBusinessType As String = ""
                Dim SBusinessType As String = ""
                Dim JoinTransAmount As String = ""
                Dim BCountry As String = ""
                Dim SCountry As String = ""
                Dim BNPWP As String = ""
                Dim DocumentType As String = ""
                Dim DocumentNotes As String = ""
                Dim LastTransNum As String = ""
                Dim LastTDate As String = ""
                Dim IsNetting As String = ""
                Dim DerivativeType As String = ""
                Dim NettingType As String = ""
                Dim NettingVolume As String = ""

                Writer = New StreamWriter(MemoryStream)

                'Fill Report Header
                BankCode = RHeader.Item(2).Value1
                BankBusinessType = RHeader.Item(3).Value1
                ReportDate = Now.ToString("ddMMyyy")
                RecordCount = RRecord.Count.ToString.PadLeft(8, "0")

                ReportHeader = BankCode + "" + BankBusinessType + "" + ReportDate + "" + "201" + RecordCount

                sb.AppendLine(ReportHeader)

                'Fill Report Record
                For i As Integer = 0 To RRecord.Count - 1
                    'ID Operasional
                    IDOperational = "1"

                    'No. Referensi
                    If RRecord.Item(i).ValuePeriod = "TOD" Then
                        'dari core banking
                        Dim GetDate As String = CStr(Now.Date.ToString("yyyyMMdd"))
                        Dim count As String = CStr(i + 1).ToString.PadLeft(15 - GetDate.Length, "0")
                        RefNum = "N" & GetDate & count
                    Else
                        RefNum = "N" & RRecord.Item(i).RefNum.ToString.PadLeft(15, "0")
                    End If

                    'Mata Uang Dasar
                    BaseCurrency = RRecord.Item(i).BaseCurrency

                    'Mata Uang Lawan
                    CounterCurrency = RRecord.Item(i).CounterCurrency

                    'Kurs Transaksi
                    Dim array As Array = RRecord.Item(i).TransactionRate.ToString.Split(CChar("."))
                    Dim NumericTransactionRate As String = array(0).ToString.PadLeft(5, "0")
                    Dim DecimalTransactionRate As String = array(1).ToString.PadRight(4, "0")

                    TransactionRate = NumericTransactionRate & DecimalTransactionRate

                    'Volume (mata uang dasar)
                    Volume = CInt(RRecord.Item(i).Volume).ToString.PadLeft(16, "0")

                    'Status Pembeli
                    If RRecord.Item(i).BSubjectStatus <> "110" Or RRecord.Item(i).BSubjectStatus <> "120" Or RRecord.Item(i).BSubjectStatus <> "130" Or RRecord.Item(i).BSubjectStatus <> "140" Or RRecord.Item(i).BSubjectStatus <> "150" Then
                        BSubjectStatus = RRecord.Item(i).BSubjectStatus
                    End If

                    Dim TransactionType As String = RRecord.Item(i).TransactionType

                    'Sandi Pembeli
                    BBankCode = RHeader.Item(2).Value1

                    'Nama Pembeli
                    BName = RHeader.Item(4).Value1.ToString.PadRight(35, " ")

                    'Sandi Pembeli Non Bank
                    BNonBankCode = RRecord.Item(i).BNonBankCode.PadRight(15, " ")

                    'Status Penjual
                    If RRecord.Item(i).SSubjectStatus <> "110" Or RRecord.Item(i).SSubjectStatus <> "120" Or RRecord.Item(i).SSubjectStatus <> "130" Or RRecord.Item(i).SSubjectStatus <> "140" Or RRecord.Item(i).SSubjectStatus <> "150" Then
                        SSubjectStatus = RRecord.Item(i).SSubjectStatus
                    End If

                    'Sandi Penjual
                    SBankCode = RRecord.Item(i).SBankCode.PadLeft(3, " ")

                    'Nama Penjual
                    SName = RRecord.Item(i).AccName.ToString.PadRight(35, " ")

                    'Sandi Penjual Non Bank
                    SNonBankCode = RRecord.Item(i).SNonBankCode.PadLeft(15, "0")

                    'Tanggal Valuta
                    ValueDate = CStr(CDate(RRecord.Item(i).ValueDate).ToString("ddMMyyyy"))

                    'Tanggal Jatuh Tempo
                    DueDate = CStr(CDate(RRecord.Item(i).DueDate).ToString("ddMMyyyy"))

                    'Jangka Waktu
                    ValuePeriod = "00"

                    'Jam Transaksi
                    If RRecord.Item(i).TransactionTime.Length = 5 Then
                        RRecord.Item(i).TransactionTime = "0" & RRecord.Item(i).TransactionTime
                    End If
                    TransactionTime = CStr(RRecord.Item(i).TransactionTime.Substring(0, 2)) + CStr(RRecord.Item(i).TransactionTime.Substring(3, 2))

                    'Tujuan
                    Purpose = RRecord.Item(i).Purpose

                    'Jenis Kegiatan Usaha Pembeli
                    BBusinessType = RRecord.Item(i).BBusinessType

                    'Jenis Kegiatan Usaha Penjual
                    SBusinessType = RRecord.Item(i).SBusinessType

                    'Jumlah Transaksi Yang Digabung
                    JoinTransAmount = RRecord.Item(i).JoinTransAmount.PadLeft(4, "0")

                    'Sandi Negara Pembeli
                    BCountry = RRecord.Item(i).BCountry

                    'Sandi Negara Penjual
                    SCountry = "".PadLeft(2, " ")

                    'NPWP Pembeli
                    BNPWP = RRecord.Item(i).BNPWP.PadLeft(15, " ")

                    'Jenis Dokumen
                    DocumentType += RRecord.Item(i).DocumentType

                    'Keterangan Jenis Dokumen
                    If DocumentType = "999" Then
                        DocumentNotes = RRecord.Item(i).DocumentNotes.ToString.PadRight(100, " ")
                    Else
                        DocumentNotes = "".ToString.PadRight(100, " ")
                    End If

                    IsNetting = RRecord.Item(i).IsNetting

                    If IsNetting = True Then
                        LastTransNum = RRecord.Item(i).LastTransNum.ToString.PadLeft(16, "0")
                        LastTDate = CStr(CDate(RRecord.Item(i).LastTDate).ToString("ddMMyyyy"))
                        DerivativeType = RRecord.Item(i).DerivativeType.ToString
                        NettingType = RRecord.Item(i).NettingType.ToString
                        NettingVolume = CInt(RRecord.Item(i).NettingVolume).ToString.PadLeft(16, "0")
                    Else
                        LastTransNum = "".ToString.PadLeft(16, "")
                        LastTDate = CStr("").ToString.PadLeft(8, "0")
                        DerivativeType = "".ToString.PadLeft(2, " ")
                        NettingType = "".ToString.PadLeft(2, " ")
                        NettingVolume = "".ToString.PadLeft(16, "0")
                    End If

                    ReportRecord = IDOperational + RefNum + BaseCurrency + CounterCurrency + CStr(TransactionRate) + CStr(Volume) _
                                    + BSubjectStatus + BBankCode + BName + BNonBankCode + SSubjectStatus + SBankCode _
                                    + SName + SNonBankCode + ValueDate + DueDate + ValuePeriod + TransactionTime _
                                    + Purpose + BBusinessType + SBusinessType + CStr(JoinTransAmount) + BCountry + SCountry _
                                    + BNPWP + DocumentType + DocumentNotes + LastTransNum + LastTDate + DerivativeType + NettingType + NettingVolume

                    sb.AppendLine(ReportRecord)
                Next

                Writer.WriteLine(sb.ToString)
                Writer.Flush()
                Writer.Close()

                Return File(MemoryStream.GetBuffer(), "text/plain", "NonLHBU-" & CStr(Now.Date.ToString("ddMMyy")) & ".txt")
            Else
                ModelState.AddModelError("Period", "There is no data in this period")

                Return View("IndexNonLHBU", model)
            End If
        Else
            Return View("IndexNonLHBU", model)
        End If
    End Function

    Private Class DataDeal
        Public Property Period As String
        Public Property DealNumber As String
        Public Property AccNum As String
        Public Property AccName As String
        Public Property BranchId As String
        Public Property Branch As String
        Public Property TransactionType As String
        Public Property DealType As String
        Public Property CurrencyDeal As String
        Public Property DealRate As Decimal
        Public Property AmountDeal As Decimal
        Public Property CurrencyCustomer As String
        Public Property AmountCustomer As Decimal
        Public Property DealPeriod As String
        Public Property DealDate As DateTime
    End Class

    ' POST : /Report/RptDeal
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function RptDeal(model As RptDealView) As ActionResult
        Dim db As New FectorEntities

        Dim Startdate = AppHelper.dateConvert(model.StartPeriod)
        Dim EndDate = AppHelper.dateConvert(model.EndPeriod)
        Dim Period As String = model.StartPeriod & " - " & model.EndPeriod

        Dim Deal = (From a In db.TransactionDeal
                    Join b In db.Branches On a.BranchId Equals b.BranchId
                    Where EntityFunctions.TruncateTime(a.DealDate) >= Startdate And EntityFunctions.TruncateTime(a.DealDate) <= EndDate
                   Select New DataDeal With {.Period = Period, _
                                             .DealNumber = a.DealNumber, _
                                            .AccNum = a.AccNum, _
                                            .AccName = a.AccName, _
                                            .BranchId = a.BranchId, _
                                            .Branch = b.Name, _
                                            .TransactionType = a.TransactionType, _
                                            .DealType = a.DealType, _
                                            .CurrencyDeal = a.CurrencyDeal, _
                                            .DealRate = a.DealRate, _
                                            .AmountDeal = a.AmountDeal, _
                                            .CurrencyCustomer = a.CurrencyCustomer, _
                                            .AmountCustomer = a.AmountCustomer, _
                                            .DealPeriod = a.DealPeriod, _
                                            .DealDate = a.DealDate}).ToList

        Dim rd As New ReportDocument
        rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RptDeal.rpt"))
        rd.SetDataSource(Deal)

        Response.Buffer = False
        Response.ClearContent()
        Response.ClearHeaders()

        Try
            Dim stream As Stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat)
            stream.Seek(0, SeekOrigin.Begin)

            rd.Close()
            rd.Dispose()
            Return File(stream, "application/pdf", "RptDeal - " & Period & ".pdf")
        Catch ex As Exception
            Throw
        End Try
    End Function

    Private Class DataTransRecap
        Public Property TransNum As Decimal
        Public Property TDate As DateTime
        Public Property AccName As String
        Public Property AccNum As String
        Public Property BranchId As String
        Public Property BranchName As String
        Public Property RateType As String
        Public Property TransactionType As String
        Public Property BSubjectStatusId As String
        Public Property BBankCode As String
        Public Property BCountryId As String
        Public Property BBusinessTypeId As String
        Public Property SSubjectStatusId As String
        Public Property SBankCode As String
        Public Property SBusinessTypeId As String
        Public Property SourceFunds As String
        Public Property SourceAccNum As String
        Public Property DocumentLHBUId As String
        Public Property DocumentLHBUDescription As String
        Public Property SourceNominal As Decimal
        Public Property DocumentTransId As String
        Public Property DocumentTransDescription As String
        Public Property DocUnderlyingNum As String
        Public Property SourceAccName As String
        Public Property PurposeId As String
        Public Property PurposeDescription As String
        Public Property DocumentTransactionLink As String
        Public Property DocumentStatementUnderlimitLink As String
        Public Property DocumentStatementOverlimitLink As String
        Public Property TransFormulirLink As String
        Public Property EditBy As String
        Public Property EditDate As DateTime
        Public Property ApproveBy As String
        Public Property ApproveDate As DateTime
        Public Property Status As String
        Public Property Period As String
        Public Property AmountTransaction As Decimal
    End Class

    ' POST : /Report/RptTransRecap
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function RptTransRecap(model As RptTransRecapView) As ActionResult
        Dim db As New FectorEntities

        Dim Startdate = AppHelper.dateConvert(model.StartPeriod)
        Dim EndDate = AppHelper.dateConvert(model.EndPeriod)
        Dim Period As String = model.StartPeriod & " - " & model.EndPeriod

        Dim Purpose = From a In db.Purposes
                      Select New With {.PurposeId = a.PurposeId, _
                                      .PurposeDesc = a.Description}
        Dim DocTransaction = From b In db.DocumentTransaction
                             Select New With {.DocTransactionID = b.DocumentId, _
                                              .DocTransDesc = b.Description}


        Dim TransRecap = (From a In db.ExchangeTransactionHead
                    Join b In db.ExchangeTransactionDetail On a.TransNum Equals b.TransNum
                    Join c In db.Branches On a.BranchId Equals c.BranchId
                    Join d In db.ExchangeTransactionDoc On a.TransNum Equals d.TransNum
                    Join e In Purpose On d.PurposeId Equals e.PurposeId
                    Join f In DocTransaction On d.DocumentTransId Equals f.DocTransactionID
                    Join g In db.DocumentLHBU On d.DocumentLHBUId Equals g.DocumentId
                    Group By a.TransNum, a.AccName, a.AccNum, a.TDate, a.BranchId, a.RateType, a.TransactionType, a.BSubjectStatusId, a.BBankCode,
                    a.BCountryId, a.BBusinessTypeId, a.SSubjectStatusId, a.SBankCode, a.SBusinessTypeId, a.SourceFunds, a.SourceAccNum,
                    d.DocumentLHBUId, a.SourceNominal, d.DocumentTransId, d.DocUnderlyingNum, a.SourceAccName, d.PurposeId,
                    d.DocumentTransactionLink, a.DocumentStatementUnderlimitLink, a.DocumentStatementOverlimitLink, d.TransFormulirLink,
                    a.EditBy, a.EditDate, a.ApproveBy, a.ApproveDate, a.Status, c.Name, g.Description, e.PurposeDesc, f.DocTransDesc
                    Into Group = Sum(b.TransactionNominal)
                    Where EntityFunctions.TruncateTime(TDate) >= Startdate And EntityFunctions.TruncateTime(TDate) <= EndDate
                    Select New DataTransRecap With {.Period = Period, _
                                             .TransNum = TransNum, _
                                             .AccName = AccName, _
                                             .AccNum = AccNum, _
                                             .TDate = TDate, _
                                             .BranchId = BranchId, _
                                             .BranchName = Name, _
                                             .RateType = RateType, _
                                             .TransactionType = TransactionType, _
                                             .BSubjectStatusId = BSubjectStatusId, _
                                             .BBankCode = BBankCode, _
                                             .BCountryId = BCountryId, _
                                             .BBusinessTypeId = BBusinessTypeId, _
                                             .SSubjectStatusId = SSubjectStatusId, _
                                             .SBankCode = SBankCode, _
                                             .SBusinessTypeId = SBusinessTypeId, _
                                             .SourceFunds = SourceFunds, _
                                             .SourceAccNum = SourceAccNum, _
                                             .DocumentLHBUId = DocumentLHBUId, _
                                             .DocumentLHBUDescription = Description, _
                                             .SourceNominal = SourceNominal, _
                                             .DocumentTransId = DocumentTransId, _
                                             .DocumentTransDescription = DocTransDesc, _
                                             .DocUnderlyingNum = DocUnderlyingNum, _
                                             .SourceAccName = SourceAccName, _
                                             .PurposeId = PurposeId, _
                                             .PurposeDescription = PurposeDesc, _
                                             .DocumentTransactionLink = DocumentTransactionLink, _
                                             .DocumentStatementUnderlimitLink = DocumentStatementUnderlimitLink, _
                                             .DocumentStatementOverlimitLink = DocumentStatementOverlimitLink, _
                                             .TransFormulirLink = TransFormulirLink, _
                                             .EditBy = EditBy, _
                                             .EditDate = EditDate, _
                                             .ApproveBy = ApproveBy, _
                                             .ApproveDate = ApproveDate, _
                                             .Status = Status, _
                                             .AmountTransaction = Group}).ToList

        Dim rtr As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        rtr.Load(Path.Combine(Server.MapPath("~/Reports"), "RptTransRecap.rpt"))
        rtr.SetDataSource(TransRecap)
        rtr.SetParameterValue(0, Period)

        Response.Buffer = False
        Response.ClearContent()
        Response.ClearHeaders()

        Try
            Dim stream As Stream = rtr.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat)
            stream.Seek(0, SeekOrigin.Begin)
            rtr.Close()
            rtr.Dispose()
            Return File(stream, "application/pdf", "RptTransRecap- " & Period & ".pdf")
        Catch ex As Exception
            Throw
        End Try
    End Function

    Private Class DataAuditTrailTransaction
        Public Property LogId As Decimal
        Public Property UserId As String
        Public Property Tdate As DateTime
        Public Property Action As String
        Public Property RefNo As String
        Public Property Period As String
    End Class

    ' POST : /Report/RptAuditTrailTransaction
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function RptAuditTrailTransaction(model As RptAuditTrailTransactionView) As ActionResult
        Dim db As New FectorEntities

        Dim Startdate = AppHelper.dateConvert(model.StartPeriod)
        Dim EndDate = AppHelper.dateConvert(model.EndPeriod)
        Dim Period As String = model.StartPeriod & " - " & model.EndPeriod

        Dim AuditTrailTransaction = (From a In db.LogTransactions
                         Where EntityFunctions.TruncateTime(a.Tdate) >= Startdate And EntityFunctions.TruncateTime(a.Tdate) <= EndDate
                         Select New DataAuditTrailTransaction With {.Period = Period, _
                                                         .LogId = a.LogId, _
                                                         .RefNo = a.RefNo, _
                                                         .Tdate = a.Tdate, _
                                                         .UserId = a.UserId, _
                                                         .Action = a.Action}).ToList
        Dim ratt As New ReportDocument
        ratt.Load(Path.Combine(Server.MapPath("~/Reports"), "RptAuditTrailTransaction.rpt"))
        ratt.SetDataSource(AuditTrailTransaction)

        Response.Buffer = False
        Response.ClearContent()
        Response.ClearHeaders()

        Try
            Dim stream As Stream = ratt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat)
            stream.Seek(0, SeekOrigin.Begin)
            ratt.Close()
            ratt.Dispose()
            Return File(stream, "application/pdf", "RptAuditTrailTransaction- " & Period & ".pdf")
        Catch ex As Exception
            Throw
        End Try
    End Function

    Private Class DataAuditTrailUser
        Public Property LogId As String
        Public Property UserId As String
        Public Property LoginTime As DateTime
        Public Property ExpiredTime As DateTime
        Public Property LogoutTime As DateTime
        Public Property Period As String
    End Class

    ' POST : /Report/RptAuditTrailUser
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function RptAuditTrailUser(model As RptAuditTrailUserView) As ActionResult
        Dim db As New FectorEntities

        Dim Startdate = AppHelper.dateConvert(model.StartPeriod)
        Dim EndDate = AppHelper.dateConvert(model.EndPeriod)
        Dim Period As String = model.StartPeriod & " - " & model.EndPeriod

        Dim AuditTrailUser = (From a In db.LogUsers
                         Where EntityFunctions.TruncateTime(a.LoginTime) >= Startdate And EntityFunctions.TruncateTime(a.LoginTime) <= EndDate
                         Select New DataAuditTrailUser With {.Period = Period, _
                                                         .LogId = a.LogId, _
                                                         .LoginTime = a.LoginTime, _
                                                         .ExpiredTime = a.ExpiredTime, _
                                                         .UserId = a.UserId, _
                                                         .LogoutTime = a.LogoutTime}).ToList
        Dim ratu As New ReportDocument
        ratu.Load(Path.Combine(Server.MapPath("~/Reports"), "RptAuditTrailUser.rpt"))
        ratu.SetDataSource(AuditTrailUser)

        Response.Buffer = False
        Response.ClearContent()
        Response.ClearHeaders()

        Try
            Dim stream As Stream = ratu.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat)
            stream.Seek(0, SeekOrigin.Begin)
            ratu.Close()
            ratu.Dispose()
            Return File(stream, "application/pdf", "RptAuditTrailUser- " & Period & ".pdf")
        Catch ex As Exception
            Throw
        End Try
    End Function

    Private Class DataKurs
        Public Property RateId As String
        Public Property TTime As DateTime
        Public Property CurrId As String
        Public Property CoreCurrId As String
        Public Property TTBuyRate As Decimal
        Public Property TTSellRate As Decimal
        Public Property BNBuyRate As Decimal
        Public Property BNSellRate As Decimal
        Public Property Period As String

    End Class

    ' POST : /Report/RptDataKurs
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function RptDataKurs(model As RptDataKursView) As ActionResult
        Dim db As New FectorEntities
        Dim Startdate = AppHelper.dateConvert(model.StartPeriod)
        Dim EndDate = AppHelper.dateConvert(model.EndPeriod)
        Dim Period As String = model.StartPeriod & " - " & model.EndPeriod
        Dim Currency As String = model.Currency

        Dim DataKurs = (From a In db.ExchangeRateMaster
                        Where (EntityFunctions.TruncateTime(a.TTime) >= Startdate And EntityFunctions.TruncateTime(a.TTime) <= EndDate) _
                        And a.CurrId = Currency
        Select New DataKurs With {.Period = Period, _
                                  .TTime = a.TTime, _
                                  .RateId = a.RateId, _
                                  .CurrId = a.CurrId, _
                                  .CoreCurrId = a.CoreCurrId, _
                                  .TTBuyRate = a.TTBuyRate, _
                                  .TTSellRate = a.TTSellRate, _
                                  .BNBuyRate = a.BNBuyRate, _
                                  .BNSellRate = a.BNSellRate
                                   }).ToList

        Dim rdk As New ReportDocument
        rdk.Load(Path.Combine(Server.MapPath("~/Reports"), "RptDataKurs.rpt"))
        rdk.SetDataSource(DataKurs)

        Response.Buffer = False
        Response.ClearContent()
        Response.ClearHeaders()

        Try
            Dim stream As Stream = rdk.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat)
            stream.Seek(0, SeekOrigin.Begin)

            rdk.Close()
            rdk.Dispose()
            Return File(stream, "application/pdf", "RptDataKurs- " & Period & ".pdf")
        Catch ex As Exception
            Throw
        End Try
    End Function

    Private Class DataExchangeTransaction
        Public Property TDate As DateTime
        Public Property Accname As String
        Public Property AccNum As String
        Public Property TransNum As Decimal
        Public Property DealNumber As String
        Public Property TransactionCurrency As String
        Public Property TransactionRate As Decimal
        Public Property TransactionNominal As Decimal
        Public Property CustomerCurrency As String
        Public Property CustomerNominal As Decimal
        Public Property ValuePeriod As String
        Public Property ValueDate As DateTime
        Public Property Period As String
    End Class

    Private Class DataExchangeTransactionHead
        Public Property TransNum As Decimal
        Public Property TDate As DateTime
        Public Property AccName As String
        Public Property AccNum As String
        Public Property BranchId As String
        Public Property BranchName As String
        Public Property RateType As String
        Public Property TransactionType As String
        Public Property SourceFunds As String
        Public Property SourceNominal As Decimal
    End Class

    ' POST : /Report/RptExchangeTransaction
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function RptExchangeTransactionWithDetail(model As RptExchangeTransactionView) As ActionResult
        Dim db As New FectorEntities

        Dim Startdate = AppHelper.dateConvert(model.StartPeriod)
        Dim EndDate = AppHelper.dateConvert(model.EndPeriod)
        Dim Period As String = model.StartPeriod & " - " & model.EndPeriod

        Dim ExchangeTransactionHead = (From a In db.ExchangeTransactionHead
                                       Join b In db.Branches On a.BranchId Equals b.BranchId
                                       Where EntityFunctions.TruncateTime(a.TDate) >= Startdate And EntityFunctions.TruncateTime(a.TDate) <= EndDate
                                       Select New DataExchangeTransactionHead With {
                                           .AccName = a.AccName, _
                                           .AccNum = a.AccNum, _
                                           .BranchId = a.BranchId, _
                                           .BranchName = b.Name, _
                                           .RateType = a.RateType, _
                                           .SourceFunds = a.SourceFunds, _
                                           .SourceNominal = a.SourceNominal, _
                                           .TDate = a.TDate, _
                                           .TransactionType = a.TransactionType, _
                                           .TransNum = a.TransNum
                                           }).ToList

        Dim ExchangeTransactionDetail = (From a In db.ExchangeTransactionDetail
                                   Join b In db.ExchangeTransactionHead On a.TransNum Equals b.TransNum
                                   Where EntityFunctions.TruncateTime(b.TDate) >= Startdate And EntityFunctions.TruncateTime(b.TDate) <= EndDate
                                   Select New DataExchangeTransaction With {.Period = Period, _
                                                                             .Accname = b.AccName, _
                                                                             .AccNum = b.AccNum, _
                                                                             .TransNum = a.TransNum, _
                                                                             .DealNumber = a.DealNumber, _
                                                                             .TransactionCurrency = a.TransactionCurrency, _
                                                                             .TransactionRate = a.TransactionRate, _
                                                                             .TransactionNominal = a.TransactionNominal, _
                                                                             .CustomerCurrency = a.CustomerCurrency, _
                                                                             .CustomerNominal = a.CustomerNominal, _
                                                                             .ValuePeriod = a.ValuePeriod, _
                                                                             .ValueDate = a.ValueDate
                                                                             }).ToList

        Dim ret As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Dim param As String = model.StartPeriod & " - " & model.EndPeriod
        ret.Load(Path.Combine(Server.MapPath("~/Reports"), "RptExchangeTransactionHead.rpt"))
        ret.SetDataSource(ExchangeTransactionHead)
        ret.Subreports(0).SetDataSource(ExchangeTransactionDetail)
        ret.SetParameterValue(0, param)
        Response.Buffer = False
        Response.ClearContent()
        Response.ClearHeaders()

        Try
            Dim stream As Stream = ret.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat)
            stream.Seek(0, SeekOrigin.Begin)

            ret.Close()
            ret.Dispose()
            Return File(stream, "application/pdf", "RptExchangeTransactionHead- " & Period & ".pdf")
        Catch ex As Exception
            Throw
        End Try
    End Function

    Private Class LimitTransaction
        Public AccNo As String
        Public AccName As String
        Public Branch As String
        Public TODLimitCurrency As String
        Public TODLimit As Decimal
        Public TODTrans As Decimal
        Public TOMLimitCurrency As String
        Public TOMLimit As Decimal
        Public TOMTrans As Decimal
        Public SPOTLimitCurrency As String
        Public SPOTLimit As Decimal
        Public SPOTTrans As Decimal
        Public AllLimitCurrency As String
        Public AllLimit As Decimal
        Public AllTrans As Decimal
    End Class

    Private Class DataDealTrans
        Public AccNo As String
        Public AccName As String
        Public TransactionType As String
        Public DealType As String
        Public BaseCurrency As String
        Public DealPeriod As String
        Public AmountDeal As Nullable(Of Decimal)
        Public UsedRate As Nullable(Of Decimal)
    End Class

    Public Class SUMDeal
        Public AccNo As String
        Public DealPeriod As String
        Public TotalDeal As Nullable(Of Decimal)
    End Class

    Private Class SUMAll
        Public AccNo As String
        Public TotalAllDeal As Nullable(Of Decimal)
    End Class

    'POST : /Report/RptTransactionLimit

    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function RptTransactionLimit(model As RptTransactionLimitView) As ActionResult
        Dim db As New FectorEntities()
        Dim BranchName As String = ""

        If model.Branch = "-1" Then
            BranchName = "ALL"
        Else
            Dim Branch = (From a In db.Branches
                         Where a.BranchId = model.Branch
                         Select New With {.BranchName = a.Name}).FirstOrDefault

            BranchName = Branch.BranchName
        End If

        Dim tmp = From c In db.Accounts
                  Join a In db.TransactionDeal On a.AccNum Equals c.AccNo
                  Join b In db.ExchangeRate On a.CurrencyDeal Equals b.CurrId
                  Join d In db.ExchangeRate On c.TODLimitcurrency Equals d.CurrId
                  Join e In db.ExchangeRate On c.TOMLimitcurrency Equals e.CurrId
                  Join f In db.ExchangeRate On c.SPOTLimitcurrency Equals f.CurrId
                  Where a.Status <> "REJECTED" And a.BranchId = model.Branch
                  Select New DataDealTrans With {.AccNo = a.AccNum, _
                                            .AccName = a.AccName, _
                                            .TransactionType = a.TransactionType, _
                                            .DealType = a.DealType, _
                                            .BaseCurrency = a.CurrencyDeal, _
                                            .DealPeriod = a.DealPeriod, _
                                            .AmountDeal = If(a.DealPeriod = "TOD", _
                                                           (a.AmountDeal * b.ClosingRate / d.ClosingRate), _
                                                            If(a.DealPeriod = "TOM", _
                                                               (a.AmountDeal * b.ClosingRate / e.ClosingRate), _
                                                            If(a.DealPeriod = "SPOT", _
                                                               (a.AmountDeal * b.ClosingRate / f.ClosingRate), _
                                                            0)))}

        Dim tmp2 = From a In tmp
                   Group By a.AccNo, a.DealPeriod Into Total = Sum(a.AmountDeal)
                   Select New SUMDeal With {.AccNo = AccNo, _
                                            .DealPeriod = DealPeriod, _
                                            .TotalDeal = Total}

        Dim tmp3 = From a In tmp2
                   Group By a.AccNo Into TotalAll = Sum(a.TotalDeal)
                   Select New SUMAll With {.AccNo = AccNo, _
                                           .TotalAllDeal = TotalAll}

        Dim tmp4 = (From a In db.Accounts
                     Group Join b In tmp2 On a.AccNo Equals b.AccNo Into ab = Group
                    Group Join c In tmp3 On a.AccNo Equals c.AccNo Into ac = Group
                   Where a.Status = "ACTIVE"
                   Select New LimitTransaction() With {.AccNo = a.AccNo, _
                                                     .AccName = a.Name, _
                                                     .TODLimitCurrency = If(a.TODLimitcurrency Is Nothing, "", a.TODLimitcurrency), _
                                                     .TODLimit = If(a.TODLimit Is Nothing, 0, a.TODLimit), _
                                                     .TODTrans = If(tmp2.Where(Function(f) f.DealPeriod = "TOD" And f.AccNo = a.AccNo).DefaultIfEmpty.FirstOrDefault.TotalDeal Is Nothing, 0, tmp2.Where(Function(f) f.DealPeriod = "TOD" And f.AccNo = a.AccNo).DefaultIfEmpty.FirstOrDefault.TotalDeal), _
                                                     .TOMLimitCurrency = If(a.TOMLimitcurrency Is Nothing, "", a.TOMLimitcurrency), _
                                                     .TOMLimit = If(a.TOMLimit Is Nothing, 0, a.TOMLimit), _
                                                     .TOMTrans = If(tmp2.Where(Function(f) f.DealPeriod = "TOM" And f.AccNo = a.AccNo).DefaultIfEmpty.FirstOrDefault.TotalDeal Is Nothing, 0, tmp2.Where(Function(f) f.DealPeriod = "TOM" And f.AccNo = a.AccNo).DefaultIfEmpty.FirstOrDefault.TotalDeal), _
                                                     .SPOTLimitCurrency = If(a.SPOTLimitcurrency Is Nothing, "", a.SPOTLimitcurrency), _
                                                     .SPOTLimit = If(a.SPOTLimit Is Nothing, 0, a.SPOTLimit), _
                                                     .SPOTTrans = If(tmp2.Where(Function(f) f.DealPeriod = "SPOT" And f.AccNo = a.AccNo).DefaultIfEmpty.FirstOrDefault.TotalDeal Is Nothing, 0, tmp2.Where(Function(f) f.DealPeriod = "SPOT" And f.AccNo = a.AccNo).DefaultIfEmpty.FirstOrDefault.TotalDeal), _
                                                     .AllLimitCurrency = If(a.ALLLimitcurrency Is Nothing, "", a.ALLLimitcurrency), _
                                                     .AllLimit = If(a.ALLLimit Is Nothing, 0, a.ALLLimit), _
                                                     .AllTrans = If(tmp3.Where(Function(f) f.AccNo = a.AccNo).DefaultIfEmpty.FirstOrDefault.TotalAllDeal Is Nothing, 0, tmp3.Where(Function(f) f.AccNo = a.AccNo).DefaultIfEmpty.FirstOrDefault.TotalAllDeal)}).ToList

        Dim rtl As New CrystalDecisions.CrystalReports.Engine.ReportDocument

        rtl.Load(Path.Combine(Server.MapPath("~/Reports"), "RptTransactionLimit.rpt"))
        rtl.SetDataSource(tmp4)
        rtl.SetParameterValue(0, BranchName)

        Response.Buffer = False
        Response.ClearContent()
        Response.ClearHeaders()

        Try
            Dim stream As Stream = rtl.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat)
            stream.Seek(0, SeekOrigin.Begin)

            rtl.Close()
            rtl.Dispose()
            Return File(stream, "application/pdf", "RptTransactionLimit - " & BranchName & ".pdf")
        Catch ex As Exception
            Throw
        End Try
    End Function

    Private Class tmp
        Public Property ID As Decimal
        Public Property BranchId As String
        Public Property DealNumber As String
        Public Property DealRate As Decimal
        Public Property AmountDeal As Decimal
        Public Property DealDate As Date
    End Class

    ' POST : /Report/RptDailyProfit
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function RptDailyProfit(model As RptDailyProfit) As ActionResult
        Dim db As New FectorEntities

        Dim Period = AppHelper.dateConvert("01-" + model.Period)

        Dim TransactionDeal = From a In db.TransactionDeal
                              Where a.DealDate.Month <= Period.Month And a.DealDate.Year = Period.Year
                              Select New tmp With {.ID = a.ID, _
                                              .BranchId = a.BranchId, _
                                              .DealNumber = a.DealNumber, _
                                              .DealRate = a.DealRate, _
                                              .AmountDeal = a.AmountDeal, _
                                              .DealDate = EntityFunctions.TruncateTime(a.DealDate)}

        Dim AllDealDate = ((From a In db.TransactionDeal
                            Where a.DealDate.Month = Period.Month And a.DealDate.Year = Period.Year
                            Order By a.DealDate
                            Select New With {.DealDate = EntityFunctions.TruncateTime(a.DealDate)}).Distinct).ToList


        ' SUM Total Profit In Current Period
        Dim CalculateProfitCurrentPeriod = From a In TransactionDeal
                                              Group Join b In db.DailyProfit.Distinct On a.ID Equals b.ID Into Group From b In Group.DefaultIfEmpty
                                              Group By a.BranchId, a.DealDate Into TotalProfit = Sum(If(If(b.BEP Is Nothing, 0, b.BEP) = 0, _
                                                                                                      If(If(b.BranchProfit Is Nothing, 0, b.BranchProfit) = 0, _
                                                                                                        0, _
                                                                                                        b.BranchProfit * a.AmountDeal),
                                                                                                    (((b.BEP - a.DealRate) * -1) * a.AmountDeal) / 2))
                                              Where DealDate.Month = Period.Month And DealDate.Year = Period.Year
                                              Select New With {.BranchId = BranchId, _
                                                              .DealDate = DealDate, _
                                                              .TotalProfit = TotalProfit}

        'SUM Total Profit Before Current Period
        Dim Deal = From a In db.TransactionDeal
                   Select New With {.ID = a.ID, _
                                    .DealNumber = a.DealNumber, _
                                    .BranchId = a.BranchId, _
                                    .DealRate = a.DealRate, _
                                    .AmountDeal = a.AmountDeal, _
                                    .MonthDeal = a.DealDate.Month, _
                                    .YearDeal = a.DealDate.Year}

        Dim CalculateProfitBeforeCurrentPeriod = From a In Deal
                                                  Group Join b In db.DailyProfit.Distinct On a.ID Equals b.ID Into Group From b In Group.DefaultIfEmpty
                                                  Group By a.BranchId, a.MonthDeal, a.YearDeal Into TotalProfit = Sum(If(If(b.BEP Is Nothing, 0, b.BEP) = 0, _
                                                                                                          If(If(b.BranchProfit Is Nothing, 0, b.BranchProfit) = 0, _
                                                                                                            0, _
                                                                                                            b.BranchProfit * a.AmountDeal),
                                                                                                        (((b.BEP - a.DealRate) * -1) * a.AmountDeal) / 2))
                                                  Where MonthDeal < Period.Month And YearDeal = Period.Year
                                                  Select New With {.BranchId = BranchId, _
                                                                    .TotalProfit = TotalProfit}

        Dim SUMProfitBefore = From a In CalculateProfitBeforeCurrentPeriod
                              Group By a.BranchId Into TotalProfit = Sum(a.TotalProfit)
                              Select New With {.BranchId = BranchId, _
                                              .TotalProfit = TotalProfit}


        Dim tmpDailyProfit = (From a In db.Branches
                          From b In db.TransactionDeal
                          Group Join c In CalculateProfitCurrentPeriod On a.BranchId Equals c.BranchId And b.DealDate.Year Equals c.DealDate.Year And b.DealDate.Month Equals c.DealDate.Month And c.DealDate.Day Equals b.DealDate.Day Into Group From c In Group.DefaultIfEmpty
                          Where b.DealDate.Month = Period.Month And b.DealDate.Year = Period.Year
                          Order By b.DealDate
                          Select New With {.BranchId = a.BranchId, _
                                           .BranchName = a.Name, _
                                           .DealDate = EntityFunctions.TruncateTime(b.DealDate), _
                                           .TotalProfit = If(c.TotalProfit Is Nothing, 0, c.TotalProfit)}).Distinct

        Dim DailyProfit = ((From a In tmpDailyProfit
                          Group Join b In SUMProfitBefore On a.BranchId Equals b.BranchId Into Group From b In Group.DefaultIfEmpty
                          Order By a.DealDate
                          Select New With {.BranchId = a.BranchId, _
                                           .BranchName = a.BranchName, _
                                           .DealDate = EntityFunctions.TruncateTime(a.DealDate), _
                                           .TotalProfit = If(a.TotalProfit Is Nothing, 0, a.TotalProfit), _
                                           .TotalProfitBefore = If(b.TotalProfit Is Nothing, 0, b.TotalProfit)}).Distinct).ToList




        Dim ms As New MemoryStream
        Dim XlsPack As New ExcelPackage(ms)
        Dim XlsWs As ExcelWorksheet
        Dim CurrenctRow As Integer = 0
        Dim LastColumn As Integer = 0

        Dim LastID As Integer = 1
        Dim LastBranchId As String = ""
        Dim LastBranchName As String = ""
        Dim LastSUMTotalProfit As Decimal = 0
        Dim LastSUMTotalProfitBeforeCurrentPeriod As Decimal = 0

        Try
            XlsWs = XlsPack.Workbook.Worksheets.Add("Daily Recapitulation")

            With XlsWs
                CurrenctRow = 2

                .Column(1).Width = 20
                .Cells(CurrenctRow, 1).Value = "No."

                .Column(2).Width = 20
                .Cells(CurrenctRow, 2).Value = "KODE"

                .Column(3).Width = 20
                .Cells(CurrenctRow, 3).Value = "CBG"

                LastColumn = 4
                For i As Integer = 0 To AllDealDate.Count - 1
                    .Column(LastColumn).Width = 20
                    .Cells(CurrenctRow, LastColumn).Value = AllDealDate(i).DealDate.Value.ToString("dd-MM-yyyy")
                    LastColumn += 1
                Next
                .Column(LastColumn).Width = 30
                .Cells(CurrenctRow, LastColumn).Value = "MTD Profit"

                .Column(LastColumn + 1).Width = 30
                .Cells(CurrenctRow, LastColumn + 1).Value = "MTD Profit(Rounding-Up)"

                .Column(LastColumn + 2).Width = 30
                .Cells(CurrenctRow, LastColumn + 2).Value = "YTD Profit"

                CurrenctRow = 3
                For i As Integer = 0 To DailyProfit.Count - 1
                    If LastBranchId <> DailyProfit(i).BranchId Then
                        .Cells(CurrenctRow, 1).Value = LastID

                        .Cells(CurrenctRow, 2).Value = DailyProfit(i).BranchId
                        .Cells(CurrenctRow, 3).Value = DailyProfit(i).BranchName

                        LastColumn = 4
                        For j As Integer = 0 To AllDealDate.Count - 1
                            .Cells(CurrenctRow, LastColumn).Value = DailyProfit(i + j).TotalProfit
                            LastColumn += 1
                        Next

                        .Cells(CurrenctRow, LastColumn + 2).Value = DailyProfit(i).TotalProfitBefore

                        LastSUMTotalProfit = 0
                        LastSUMTotalProfitBeforeCurrentPeriod = 0

                        LastBranchId = DailyProfit(i).BranchId
                        CurrenctRow += 1
                        LastID += 1
                    End If
                    LastSUMTotalProfit += DailyProfit(i).TotalProfit

                    .Cells(CurrenctRow - 1, LastColumn).Value = LastSUMTotalProfit
                    .Cells(CurrenctRow - 1, LastColumn + 1).Value = Math.Ceiling(LastSUMTotalProfit)
                Next

                XlsPack.Save()
                XlsPack.Dispose()

                Dim filename As String = "DailyProfit-" & model.Period & ".xlsx"

                XlsWs.View.FreezePanes(3, 2)
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                Response.AddHeader("content-disposition", "attachment;  filename=" + filename)
                Response.BinaryWrite(ms.ToArray)
                Response.End()

                Return Json(New With {.success = True})
            End With
        Catch ex As Exception
            'alert("No Data for Report", Me)
            Throw
        End Try
    End Function

    Private Class ExportTransaction
        Public Property ID As String
        Public Property TransNum As String
        Public Property BICode As String
        Public Property Contract As String
        Public Property BaseVariable As String
        Public Property BuyName As String
        Public Property TransTime As String
        Public Property EffectiveDate As String
        Public Property StartForwardDate As String
        Public Property ValueDate As String
        Public Property DueDate As String
        Public Property BaseCurrency As String
        Public Property CounterCurrency As String
        Public Property Amount As Decimal
        Public Property Rate As Decimal
        Public Property StrikePrice As String
        Public Property BaseRate As Decimal
        Public Property SwapPremium As String
        Public Property OptionPremium As String
        Public Property StyleOption As String
        Public Property RatePaymentPeriod As String
        Public Property BaseInterestType As String
        Public Property BaseFloatInterestType As String
        Public Property BaseTenorInterestType As String
        Public Property BasePremiumInterestType As String
        Public Property BaseFixRate As String
        Public Property CounterInterestType As String
        Public Property CounterFloatInterestType As String
        Public Property CounterTenorInterestType As String
        Public Property CounterPremiumInterestType As String
        Public Property CounterFixRate As String
        Public Property FuturesPrice As String
        Public Property TransactionNotes As String
        Public Property ForeignTransaction As String
        Public Property PartnerCountry As String
        Public Property NettingLastTransNum As String
        Public Property NettingPurpose As String
        Public Property NettingVolume As String
        Public Property DynamicHedginLastTransNum As String

        Public Property TransactionType As String
    End Class

    Private Class DocTransaction
        Public Property TransNum As String
        Public Property DocUnderlyingType As String
        Public Property DocUnderlyingName As String
        Public Property LHBUDocument As String
        Public Property LHBUPurpose As String
        Public Property NominalDoc As Decimal
        Public Property DocUnderlyingFile As String
        Public Property DocTransFormFile As String
    End Class

    ' POST : /Report/RptExportTransaction
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function RptExportTransaction(model As RptExportTransaction) As ActionResult
        Try
            Dim db As New FectorEntities

            'Bank Index Info
            Dim BankIndexInfo = (From h In db.Settings
                                   Where h.Key1 = "Bank"
                                   Select h).ToList

            Dim Startdate = AppHelper.dateConvert(model.StartPeriod)
            Dim EndDate = AppHelper.dateConvert(model.EndPeriod)
            Dim Period As String = model.StartPeriod & " - " & model.EndPeriod

            Dim ExportTransaction = ((From a In db.ExchangeTransactionHead
                                    Join b In db.ExchangeTransactionDetail On a.TransNum Equals b.TransNum
                                    Where a.Status = "ACTIVE" And EntityFunctions.TruncateTime(a.TDate) >= Startdate And EntityFunctions.TruncateTime(a.TDate) <= EndDate
                                    Select New ExportTransaction() With {
                                        .ID = "",
                                        .TransNum = a.TransNum,
                                        .BICode = "555001",
                                        .Contract = If(b.ValuePeriod = "TOD", "00", If(b.ValuePeriod = "TOM", "01", "02")),
                                        .BaseVariable = "C",
                                        .BuyName = a.AccName,
                                        .TransTime = a.TDate,
                                        .EffectiveDate = "",
                                        .StartForwardDate = "",
                                        .ValueDate = b.ValueDate,
                                        .DueDate = "",
                                        .BaseCurrency = b.TransactionCurrency,
                                        .CounterCurrency = b.CustomerCurrency,
                                        .Amount = b.TransactionNominal,
                                        .Rate = b.TransactionRate,
                                        .StrikePrice = "",
                                        .BaseRate = 0,
                                        .SwapPremium = "",
                                        .OptionPremium = "",
                                        .StyleOption = "",
                                        .RatePaymentPeriod = "",
                                        .BaseInterestType = "",
                                        .BaseFloatInterestType = "",
                                        .BaseTenorInterestType = "",
                                        .BasePremiumInterestType = "",
                                        .BaseFixRate = "",
                                        .CounterInterestType = "",
                                        .CounterFloatInterestType = "",
                                        .CounterTenorInterestType = "",
                                        .CounterPremiumInterestType = "",
                                        .CounterFixRate = "",
                                        .FuturesPrice = "",
                                        .TransactionNotes = "",
                                        .ForeignTransaction = "",
                                        .PartnerCountry = a.SCountryId,
                                        .NettingLastTransNum = "0",
                                        .NettingPurpose = "0",
                                        .NettingVolume = "0",
                                        .DynamicHedginLastTransNum = "",
                                        .TransactionType = a.TransactionType
                                    }).Union(
                                    From a In db.CoreNonTrx
                                    Join b In db.Currencies On a.CoreCurrId Equals b.CoreCurrId
                                    Where EntityFunctions.TruncateTime(a.TDate) >= Startdate And EntityFunctions.TruncateTime(a.TDate) <= EndDate
                                    Select New ExportTransaction() With {
                                        .ID = "",
                                        .TransNum = a.Refno,
                                        .BICode = "555001",
                                        .Contract = "",
                                        .BaseVariable = "C",
                                        .BuyName = "",
                                        .TransTime = a.TDate,
                                        .EffectiveDate = "",
                                        .StartForwardDate = "",
                                        .ValueDate = "",
                                        .DueDate = "",
                                        .BaseCurrency = b.CurrId,
                                        .CounterCurrency = "",
                                        .Amount = a.Amount,
                                        .Rate = a.Rate,
                                        .StrikePrice = "",
                                        .BaseRate = 0,
                                        .SwapPremium = "",
                                        .OptionPremium = "",
                                        .StyleOption = "",
                                        .RatePaymentPeriod = "",
                                        .BaseInterestType = "",
                                        .BaseFloatInterestType = "",
                                        .BaseTenorInterestType = "",
                                        .BasePremiumInterestType = "",
                                        .BaseFixRate = "",
                                        .CounterInterestType = "",
                                        .CounterFloatInterestType = "",
                                        .CounterTenorInterestType = "",
                                        .CounterPremiumInterestType = "",
                                        .CounterFixRate = "",
                                        .FuturesPrice = "",
                                        .TransactionNotes = "",
                                        .ForeignTransaction = "",
                                        .PartnerCountry = "",
                                        .NettingLastTransNum = "0",
                                        .NettingPurpose = "0",
                                        .NettingVolume = "0",
                                        .DynamicHedginLastTransNum = "",
                                        .TransactionType = ""})).ToList

            db = New FectorEntities
            Dim DocTransaction = (From a In db.ExchangeTransactionDoc
                                  Join b In db.ExchangeTransactionHead On a.TransNum Equals b.TransNum
                                  Group Join c In db.DocumentTransaction On a.DocumentTransId Equals c.DocumentId Into ac = Group
                                  Group Join d In db.DocumentLHBU On a.DocumentLHBUId Equals d.DocumentId Into ad = Group
                                  Group Join e In db.Purposes On a.PurposeId Equals e.PurposeId Into ae = Group
                                  Where b.Status = "ACTIVE" And EntityFunctions.TruncateTime(b.TDate) >= Startdate And EntityFunctions.TruncateTime(b.TDate) <= EndDate
                                  Select New DocTransaction With {
                                            .TransNum = a.TransNum,
                                            .DocUnderlyingType = ac.DefaultIfEmpty.FirstOrDefault.DocumentId,
                                            .DocUnderlyingName = a.DocUnderlyingNum,
                                            .LHBUDocument = ad.DefaultIfEmpty.FirstOrDefault.Description,
                                            .LHBUPurpose = ae.DefaultIfEmpty.FirstOrDefault.PurposeId,
                                            .NominalDoc = a.NominalDoc,
                                            .DocUnderlyingFile = a.DocumentTransactionLink,
                                            .DocTransFormFile = a.TransFormulirLink
                                        }).ToList

            Dim ms As New MemoryStream
            Dim XlsPack As New ExcelPackage(ms)
            Dim XlsWs As ExcelWorksheet
            Dim CurrentRow As Integer = 0
            Dim LastColumn As Integer = 0
            Dim LastTransNUm As Decimal = 0

            Try
                XlsWs = XlsPack.Workbook.Worksheets.Add("Export Transaction")

                With XlsWs
                    CurrentRow = 2

                    .Column(1).Width = 50
                    .Cells(CurrentRow, 1).Value = "ID Operasional"

                    .Column(2).Width = 50
                    .Cells(CurrentRow, 2).Value = "Nomor Referensi Transaksi"

                    .Column(3).Width = 50
                    .Cells(CurrentRow, 3).Value = "ID Pihak Lawan"

                    .Column(4).Width = 50
                    .Cells(CurrentRow, 4).Value = "Kontrak"

                    .Column(5).Width = 50
                    .Cells(CurrentRow, 5).Value = "Variabel Yang Mendasari"

                    .Column(6).Width = 50
                    .Cells(CurrentRow, 6).Value = "Peran Pelapor"

                    .Column(7).Width = 50
                    .Cells(CurrentRow, 7).Value = "Jam Transaksi"

                    .Column(8).Width = 50
                    .Cells(CurrentRow, 8).Value = "Tanggal Efektif"

                    .Column(9).Width = 50
                    .Cells(CurrentRow, 9).Value = "Tanggal Awal Forward"

                    .Column(10).Width = 50
                    .Cells(CurrentRow, 10).Value = "Tanggal Valuta"

                    .Column(11).Width = 50
                    .Cells(CurrentRow, 11).Value = "Tanggal Jatuh Tempo"

                    .Column(12).Width = 50
                    .Cells(CurrentRow, 12).Value = "Valuta Dasar"

                    .Column(13).Width = 50
                    .Cells(CurrentRow, 13).Value = "Valuta Lawan"

                    .Column(14).Width = 50
                    .Cells(CurrentRow, 14).Value = "Nominal Dalam Valuta Dasar"

                    .Column(15).Width = 50
                    .Cells(CurrentRow, 15).Value = "Kurs Transaksi/Forward Rate/Strike Price"

                    .Column(16).Width = 50
                    .Cells(CurrentRow, 16).Value = "Strike Price 2"

                    .Column(17).Width = 50
                    .Cells(CurrentRow, 17).Value = "Base Rate"

                    .Column(18).Width = 50
                    .Cells(CurrentRow, 18).Value = "Premi Swap"

                    .Column(19).Width = 50
                    .Cells(CurrentRow, 19).Value = "Premi Option"

                    .Column(20).Width = 50
                    .Cells(CurrentRow, 20).Value = "Style Option"

                    .Column(21).Width = 50
                    .Cells(CurrentRow, 21).Value = "Periode Pembayaran Bunga"

                    .Column(22).Width = 50
                    .Cells(CurrentRow, 22).Value = "Valuta Dasar - Jenis Suku Bunga"

                    .Column(23).Width = 50
                    .Cells(CurrentRow, 23).Value = "Valuta Dasar - Jenis Suku Bunga Acuan Mengambang"

                    .Column(24).Width = 50
                    .Cells(CurrentRow, 24).Value = "Valuta Dasar - Tenor Suku Bunga Acuan Mengambang"

                    .Column(25).Width = 50
                    .Cells(CurrentRow, 25).Value = "Valuta Dasar - Premium Suku Bunga Acuan Mengambang"

                    .Column(26).Width = 50
                    .Cells(CurrentRow, 26).Value = "Valuta Dasar - Suku Bunga Tetap"

                    .Column(27).Width = 50
                    .Cells(CurrentRow, 27).Value = "Valuta Lawan - Jenis Suku Bunga"

                    .Column(28).Width = 50
                    .Cells(CurrentRow, 28).Value = "Valuta Lawan - Jenis Suku Bunga Acuan Mengambang"

                    .Column(29).Width = 50
                    .Cells(CurrentRow, 29).Value = "Valuta Lawan - Tenor Suku Bunga Acuan Mengambang"

                    .Column(30).Width = 50
                    .Cells(CurrentRow, 30).Value = "Valuta Lawan - Premium Suku Bunga Acuan Mengambang"

                    .Column(31).Width = 50
                    .Cells(CurrentRow, 31).Value = "Valuta Lawan - Suku Bunga Tetap"

                    .Column(32).Width = 50
                    .Cells(CurrentRow, 32).Value = "Harga Futures"

                    .Column(33).Width = 50
                    .Cells(CurrentRow, 33).Value = "Keterangan Transaksi"

                    .Column(34).Width = 50
                    .Cells(CurrentRow, 34).Value = "Transaksi Dengan Pihak Asing"

                    .Column(35).Width = 50
                    .Cells(CurrentRow, 35).Value = "LCS - Negera Mitra"

                    .Column(36).Width = 50
                    .Cells(CurrentRow, 36).Value = "Netting - Nomor Referensi Transaksi Terakhir"

                    .Column(37).Width = 50
                    .Cells(CurrentRow, 37).Value = "Netting - Tujuan"

                    .Column(38).Width = 50
                    .Cells(CurrentRow, 38).Value = "Netting - Volume"

                    .Column(39).Width = 50
                    .Cells(CurrentRow, 39).Value = "Dynamic Hedging - Nomor Referensi Transaksi Terakhir"

                    CurrentRow = 3
                    For i As Integer = 0 To ExportTransaction.Count - 1
                        .Cells(CurrentRow, 1).Value = "1"
                        .Cells(CurrentRow, 2).Value = ExportTransaction(i).TransNum
                        .Cells(CurrentRow, 3).Value = "555001"
                        .Cells(CurrentRow, 4).Value = ExportTransaction(i).Contract
                        .Cells(CurrentRow, 5).Value = ExportTransaction(i).BaseVariable
                        If ExportTransaction(i).TransactionType = "Sell" Then
                            .Cells(CurrentRow, 6).Value = ExportTransaction(i).BuyName
                        Else
                            .Cells(CurrentRow, 6).Value = BankIndexInfo.Item(4).Value1
                        End If
                        .Cells(CurrentRow, 7).Value = CDate(ExportTransaction(i).TransTime).ToString("HH:mm")
                        .Cells(CurrentRow, 8).Value = ExportTransaction(i).EffectiveDate
                        .Cells(CurrentRow, 9).Value = ExportTransaction(i).StartForwardDate
                        If ExportTransaction(i).ValueDate <> "" Then
                            .Cells(CurrentRow, 10).Value = CDate(ExportTransaction(i).ValueDate).ToString("dd-MM-yyyy")
                        Else
                            .Cells(CurrentRow, 10).Value = ""
                        End If
                        .Cells(CurrentRow, 11).Value = ExportTransaction(i).DueDate
                        .Cells(CurrentRow, 12).Value = ExportTransaction(i).BaseCurrency
                        .Cells(CurrentRow, 13).Value = ExportTransaction(i).CounterCurrency
                        .Cells(CurrentRow, 14).Value = CDec(ExportTransaction(i).Amount).ToString("N2")
                        .Cells(CurrentRow, 15).Value = CDec(ExportTransaction(i).Rate).ToString("N2")
                        .Cells(CurrentRow, 16).Value = ExportTransaction(i).StrikePrice
                        .Cells(CurrentRow, 17).Value = ExportTransaction(i).BaseRate
                        .Cells(CurrentRow, 18).Value = ExportTransaction(i).SwapPremium
                        .Cells(CurrentRow, 19).Value = ExportTransaction(i).OptionPremium
                        .Cells(CurrentRow, 20).Value = ExportTransaction(i).StyleOption
                        .Cells(CurrentRow, 21).Value = ExportTransaction(i).RatePaymentPeriod
                        .Cells(CurrentRow, 22).Value = ExportTransaction(i).BaseInterestType
                        .Cells(CurrentRow, 23).Value = ExportTransaction(i).BaseFloatInterestType
                        .Cells(CurrentRow, 24).Value = ExportTransaction(i).BaseTenorInterestType
                        .Cells(CurrentRow, 25).Value = ExportTransaction(i).BasePremiumInterestType
                        .Cells(CurrentRow, 26).Value = ExportTransaction(i).BaseFixRate
                        .Cells(CurrentRow, 27).Value = ExportTransaction(i).CounterInterestType
                        .Cells(CurrentRow, 28).Value = ExportTransaction(i).CounterFloatInterestType
                        .Cells(CurrentRow, 29).Value = ExportTransaction(i).CounterTenorInterestType
                        .Cells(CurrentRow, 30).Value = ExportTransaction(i).CounterPremiumInterestType
                        .Cells(CurrentRow, 31).Value = ExportTransaction(i).CounterFixRate
                        .Cells(CurrentRow, 32).Value = ExportTransaction(i).FuturesPrice
                        .Cells(CurrentRow, 33).Value = ExportTransaction(i).TransactionNotes
                        .Cells(CurrentRow, 34).Value = ExportTransaction(i).ForeignTransaction
                        .Cells(CurrentRow, 35).Value = ExportTransaction(i).PartnerCountry
                        .Cells(CurrentRow, 36).Value = ExportTransaction(i).NettingLastTransNum
                        .Cells(CurrentRow, 37).Value = ExportTransaction(i).NettingPurpose
                        .Cells(CurrentRow, 38).Value = ExportTransaction(i).NettingVolume
                        .Cells(CurrentRow, 39).Value = ExportTransaction(i).DynamicHedginLastTransNum
                        CurrentRow += 1
                    Next

                    XlsWs = XlsPack.Workbook.Worksheets.Add("Doc. Transaction")

                    Dim zip As New ZipFile
                    zip.AlternateEncodingUsage = ZipOption.AsNecessary

                    With XlsWs
                        CurrentRow = 2

                        .Column(1).Width = 50
                        .Cells(CurrentRow, 1).Value = "Nomor Referensi Transaksi"

                        .Column(2).Width = 50
                        .Cells(CurrentRow, 2).Value = "Doc. Underlying Type"

                        .Column(3).Width = 50
                        .Cells(CurrentRow, 3).Value = "Doc. Underlying Name"

                        .Column(4).Width = 50
                        .Cells(CurrentRow, 4).Value = "LHBU Document"

                        .Column(5).Width = 50
                        .Cells(CurrentRow, 5).Value = "LHBU Purpose"

                        .Column(6).Width = 50
                        .Cells(CurrentRow, 6).Value = "Nominal Doc."

                        CurrentRow = 3
                        For i As Integer = 0 To DocTransaction.Count - 1
                            LastTransNUm = DocTransaction(i).TransNum
                            .Cells(CurrentRow, 1).Value = DocTransaction(i).TransNum
                            .Cells(CurrentRow, 2).Value = DocTransaction(i).DocUnderlyingType
                            .Cells(CurrentRow, 3).Value = DocTransaction(i).DocUnderlyingName
                            .Cells(CurrentRow, 4).Value = DocTransaction(i).LHBUDocument
                            .Cells(CurrentRow, 5).Value = DocTransaction(i).LHBUPurpose
                            .Cells(CurrentRow, 6).Value = DocTransaction(i).NominalDoc

                            If Not zip.EntryFileNames.Contains(DocTransaction(i).TransNum & "/") Then
                                zip.AddDirectoryByName(DocTransaction(i).TransNum)
                            End If

                            If LastTransNUm = 14855 Then
                                Dim tmp As String = ""
                            End If

                            If DocTransaction(i).DocUnderlyingFile <> "" And Not IsDBNull(DocTransaction(i).DocUnderlyingFile) And DocTransaction(i).DocUnderlyingFile IsNot Nothing Then
                                Dim tmp_Filename As String = DocTransaction(i).DocUnderlyingFile

                                tmp_Filename = tmp_Filename.Substring(tmp_Filename.LastIndexOf("/"), tmp_Filename.Length - tmp_Filename.LastIndexOf("/"))
                                If Not zip.EntryFileNames.Contains(DocTransaction(i).TransNum & tmp_Filename) And DocTransaction(i).DocUnderlyingFile.Contains("~/Uploads/Transaction/") Then
                                    If System.IO.File.Exists(Server.MapPath(DocTransaction(i).DocUnderlyingFile)) Then
                                        zip.AddFile(Server.MapPath(DocTransaction(i).DocUnderlyingFile), DocTransaction(i).TransNum)
                                    End If
                                End If

                            End If

                            If DocTransaction(i).DocTransFormFile <> "" And Not IsDBNull(DocTransaction(i).DocTransFormFile) And DocTransaction(i).DocTransFormFile IsNot Nothing Then
                                Dim tmp_Filename As String = DocTransaction(i).DocTransFormFile

                                tmp_Filename = tmp_Filename.Substring(tmp_Filename.LastIndexOf("/"), tmp_Filename.Length - tmp_Filename.LastIndexOf("/"))

                                If Not zip.EntryFileNames.Contains(DocTransaction(i).DocTransFormFile & tmp_Filename) And DocTransaction(i).DocTransFormFile.Contains("~/Uploads/Transaction/") Then
                                    If System.IO.File.Exists(Server.MapPath(DocTransaction(i).DocTransFormFile)) Then
                                        zip.AddFile(Server.MapPath(DocTransaction(i).DocTransFormFile), DocTransaction(i).TransNum)
                                    End If

                                End If
                            End If

                            CurrentRow += 1
                        Next
                    End With
                    
                    XlsPack.Save()

                    Dim filename As String = "FectorTransaction_" & Period & ".xlsx"
                    zip.AddEntry(filename, XlsPack.GetAsByteArray)

                    Response.Clear()
                    Response.BufferOutput = False
                    Dim zipName As String = [String].Format("FectorTransaction_{0}.zip", Period)
                    Response.ContentType = "application/zip"
                    Response.AddHeader("content-disposition", String.Format("attachment;filename=""{0}""", zipName))
                    zip.Save(Response.OutputStream)
                    Response.End()

                    Return Json(New With {.success = True})
                End With
            Catch ex As Exception
                Throw
            End Try
        Catch ex As Exception
            Throw
        End Try
    End Function
End Class

