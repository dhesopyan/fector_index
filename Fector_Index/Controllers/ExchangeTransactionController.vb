Imports System.Net
Imports System.Data.Objects.SqlClient
Imports System.IO
Imports System.Data.Entity.Validation
Imports CrystalDecisions.CrystalReports.Engine

Public Class ExchangeTransactionController
    Inherits System.Web.Mvc.Controller

    Dim tempDealNumber As String = ""

    Dim RateTypeOption As IEnumerable(Of SelectListItem) = {New SelectListItem With {.Text = "TT", .Value = "TT"}, _
                                                            New SelectListItem With {.Text = "Bank Notes", .Value = "BN"}}

    Dim TransactionTypeOption As IEnumerable(Of SelectListItem) = {New SelectListItem With {.Text = "Sell", .Value = "Sell"}, _
                                                            New SelectListItem With {.Text = "Buy", .Value = "Buy"}}

    Dim DealPeriodOption As IEnumerable(Of SelectListItem) = {New SelectListItem With {.Text = "TOD", .Value = "TOD"}, _
                                                              New SelectListItem With {.Text = "TOM", .Value = "TOM"}, _
                                                              New SelectListItem With {.Text = "SPOT", .Value = "SPOT"}, _
                                                              New SelectListItem With {.Text = "FORWARD", .Value = "FWD"}}

    Dim SourceFundsOption As IEnumerable(Of SelectListItem) = {New SelectListItem With {.Text = "Cash", .Value = "Cash"}, _
                                                            New SelectListItem With {.Text = "Debit", .Value = "Debit"}, _
                                                            New SelectListItem With {.Text = "Other", .Value = "Others"}}

    Dim initDealNumber As String = ""

    ' GET: /ExchangeTransaction/Index
    <Authorize> _
    Function Index() As ActionResult
        ViewBag.Breadcrumb = String.Format("Home > Exchange Transaction")
        Return View("Index")
    End Function

    ' GET: /ExchangeTransaction/Approval
    <Authorize> _
    Function Approval() As ActionResult
        ViewBag.Breadcrumb = String.Format("Home > Exchange Transaction Approval")
        Return View("Index")
    End Function

    ' GET: /ExchangeTransaction/ViewHistory
    <Authorize> _
    Function History() As ActionResult
        ViewBag.Breadcrumb = String.Format("Home > Exchange Transaction History")
        Return View("Index")
    End Function

    ' GET: /ExchangeTransaction/Review
    <Authorize> _
    Function Review() As ActionResult
        ViewBag.Breadcrumb = String.Format("Home > Exchange Transaction Review")
        Return View("Index")
    End Function

    ' GET: /ExchangeTransaction/Reactive
    <Authorize> _
    Function Reactive() As ActionResult
        ViewBag.Breadcrumb = String.Format("Home > Exchange Transaction Reactived")
        Return View("Index")
    End Function

    ' GET: /ExchangeTransaction/CloseTransaction
    <Authorize> _
    Function CloseTransaction() As ActionResult
        ViewBag.Breadcrumb = String.Format("Home > Exchange Transaction Closed")
        Return View("CloseTransaction")
    End Function

    '
    ' Get: /ExchangeTransaction/Deal
    <Authorize> _
    Public Function Deal() As ActionResult
        Dim model As New ExchangeTransactionViewModel
        Dim db As New FectorEntities

        model.TransNum = "(AUTO)"
        ViewData("Title") = "Create Transaction"
        ViewBag.Breadcrumb = "Home > Exchange Transaction"
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
        ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(db.ExchangeRate.Where(Function(f) f.CurrId <> "IDR"), "CurrId", "CurrencyDisplay")
        ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
        ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
        ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
        ViewBag.SourceFundsOption = SourceFundsOption
        ViewBag.RateType = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption
        ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")
        ViewBag.AddAnotherTransaction = False

        model.UseDeal = False
        model.TransactionNominal = 0
        model.BranchId = Session("BranchId")
        Session("dealNumber") = ""
        Session("LastDealNominal") = 0
        Session("CountDocUnderlying") = 0
        Session("CountTransForm") = 0

        Return View("DetailUseDeal", model)
    End Function

    '
    ' Post: /ExchangeTransaction/Deal
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Deal(model As ExchangeTransactionViewModel) As ActionResult
        Dim db As New FectorEntities
        Dim file As HttpPostedFileBase = model.FileDocumentTransaction

        model.TransNum = "(AUTO)"
        ViewData("Title") = "Create Transaction"
        ViewBag.Breadcrumb = "Home > Exchange Transaction > Create"
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
        ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(db.ExchangeRate.Where(Function(f) f.CurrId <> "IDR"), "CurrId", "CurrencyDisplay")
        ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
        ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
        ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
        ViewBag.SourceFundsOption = SourceFundsOption
        ViewBag.RateType = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption
        ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        If ModelState.IsValid Then
            Dim transnum As Decimal = 0
            Dim tdate As DateTime = Now
            Dim branchid As String = Session("BranchId")
            Dim accno As String = model.AccNum
            Dim accname As String = model.AccName
            Dim trantype As String = model.TransactionType
            Dim ratetype As String = model.RateType
            Dim basecurr As String = model.TransactionCurrency
            Dim countercurr As String = model.CustomerCurrency
            Dim bsubjectstatusid As String = ""
            Dim bbankcode As String = ""
            Dim bcountryid As String = ""
            Dim bbusinesstypeid As String = ""
            Dim ssubjectstatusid As String = ""
            Dim sbankcode As String = ""
            Dim scountryid As String = ""
            Dim sbusinesstypeid As String = ""
            Dim sourcefund As String = model.SourceFunds
            Dim sourceaccnum As String = model.SourceAccNum
            Dim sourceaccname As String = model.SourceAccName
            Dim sourcenominal As Decimal = model.SourceNominal
            Dim doctransid As String = model.DocumentTransId
            Dim docunderlyingnumber As String = model.DocUnderlyingNum
            Dim docunderlyingnominal As Decimal = model.DocUnderlyingNominal
            Dim doclhbuid As String = model.DocumentLHBUId
            Dim purposeid As String = model.PurposeId
            Dim doctranslink As String = ""
            Dim docstatementunderlimitlink As String = ""
            Dim docstatementoverlimitlink As String = ""
            Dim transformlink As String = ""
            Dim editby As String = User.Identity.Name
            Dim editdate As DateTime = Now
            Dim flagreconcile As Integer = 0
            Dim dealnumber As String = ""
            Dim transrate As Decimal = model.TransactionRate
            Dim transamount As Decimal = model.TransactionNominal
            Dim customeramount As Decimal = model.CustomerNominal
            Dim valperiod As String = model.ValuePeriod
            Dim valdate As DateTime = AppHelper.dateConvert(model.ValueDate)
            Dim isnetting As Boolean = model.IsNetting
            Dim derivativetype As Integer = model.DerivativeType
            Dim nettingtype As Integer = model.NettingType
            Dim dealFinished As Boolean
            Dim CountDocUnderlying As Integer = 0
            Dim CountTransForm As Integer = 0
            Dim i As Integer = 1
            Dim StatementAttachmentSize As Decimal = 0
            Dim TransformAttachmentSize As Decimal = 0
            Dim UnderlyingAttachmentSize As Decimal = 0
            Dim MaxUploadSize As String = MsSetting.GetSetting("UploadFile", "MaxSize", "", 1)

            CountDocUnderlying = Session("CountDocUnderlying")
            CountTransForm = Session("CountTransForm")

            If model.ListOfDeal.Count = 1 Then
                dealnumber = model.ListOfDeal(0).DealNumber
                Dim dealamount As Decimal = model.ListOfDeal(0).BaseNominal
                Dim prevtransamount = (From det In db.ExchangeTransactionDetail
                                       Where det.DealNumber = dealnumber
                                       Select det).ToList.Sum(Function(f) f.TransactionNominal)

                If Not IsNothing(prevtransamount) Then
                    If prevtransamount + model.TransactionNominal > dealamount Then
                        ModelState.AddModelError("TransactionNominal", "Transaction amount surpasses deal amount")
                        Return View("DetailUseDeal", model)
                    ElseIf prevtransamount + model.TransactionNominal = dealamount Then
                        dealFinished = True
                    Else
                        dealFinished = False
                    End If
                End If
            Else
                For Each de As TransactionDetail In model.ListOfDeal
                    Dim tempdealnumber As String = de.DealNumber
                    Dim dealreference = db.TransactionDeal.Where(Function(f) f.DealNumber = tempdealnumber).FirstOrDefault
                    If dealreference.Status <> "ACTIVE" Then
                        ModelState.AddModelError("DealNumber", "Deal " & de.DealNumber & " can not be used in multi deal transaction because it is already used partially")
                        Return View("DetailUseDeal", model)
                    End If
                Next
            End If

            Dim custinfo = ((From accext In db.AccountsExtension
                               Join acc In db.Accounts On accext.AccNo Equals acc.CIF
                               Where acc.AccNo = accno
                               Select New With {.ssid = acc.SubjectStatusId, .bc = acc.BICode, .cid = acc.CountryId, .btid = acc.BusinessTypeId}).Union(
                               From otheraccext In db.OtherAccountsExtension
                               Join otheracc In db.OtherAccounts On otheraccext.AccNo Equals otheracc.AccNo
                               Where otheracc.AccNo = accno
                               Select New With {.ssid = otheracc.SubjectStatusId, .bc = otheracc.BICode, .cid = otheracc.CountryId, .btid = otheracc.BusinessTypeId})
                           ).FirstOrDefault

            If custinfo.cid Is Nothing Then
                ModelState.AddModelError("AccNum", "Country not found for this customer, please complete the country in LLD")

                Return View("DetailUseDeal", model)
            End If

            If trantype = "Buy" Then
                'Fill Buyer with Bank Information
                bsubjectstatusid = MsSetting.GetSetting("Bank", "SubjectStatusId", "", 1)
                bbankcode = MsSetting.GetSetting("Bank", "BICode", "", 1)
                bcountryid = MsSetting.GetSetting("Bank", "CountryID", "", 1)
                bbusinesstypeid = MsSetting.GetSetting("Bank", "BusinessTypeID", "", 1)

                ssubjectstatusid = custinfo.ssid
                sbankcode = custinfo.bc
                scountryid = custinfo.cid
                sbusinesstypeid = custinfo.btid
            Else
                'Fill Seller with Bank Information
                ssubjectstatusid = MsSetting.GetSetting("Bank", "SubjectStatusId", "", 1)
                sbankcode = MsSetting.GetSetting("Bank", "BICode", "", 1)
                scountryid = MsSetting.GetSetting("Bank", "CountryID", "", 1)
                sbusinesstypeid = MsSetting.GetSetting("Bank", "BusinessTypeID", "", 1)

                bsubjectstatusid = custinfo.ssid
                bbankcode = custinfo.bc
                bcountryid = custinfo.cid
                bbusinesstypeid = custinfo.btid
            End If

            Dim CIF As Decimal = 0
            Dim CheckOtherAcc = db.OtherAccounts.Where(Function(f) f.AccNo = model.AccNum).Count
            If CheckOtherAcc = 0 Then
                Dim msAccount As MsAccount = db.Accounts.FirstOrDefault(Function(x) x.AccNo = model.AccNum)
                If (msAccount Is Nothing) Then
                    ModelState.AddModelError("AccNum", "This Account has no CIF")
                    Return View("DetailUseDeal", model)
                Else
                    CIF = msAccount.CIF
                End If
            End If

            If Not IsNothing(model.FileDocumentStatementUnderlimit) Then
                If model.FileDocumentStatementUnderlimit.ContentLength > 0 Then
                    If model.FileDocumentStatementUnderlimit.ContentLength / 1000000 > CDec(MaxUploadSize) Then
                        ModelState.AddModelError("FileDocumentStatementUnderlimit", "This file size is more than 2 MB, cannot upload")
                        Return View("DetailUseDeal", model)
                    End If
                End If
            End If

            If Not IsNothing(model.FileDocumentStatementOverlimit) Then
                If model.FileDocumentStatementOverlimit.ContentLength > 0 Then
                    If model.FileDocumentStatementOverlimit.ContentLength / 1000000 > CDec(MaxUploadSize) Then
                        ModelState.AddModelError("FileDocumentStatementOverlimit", "This file size is more than 2 MB, cannot upload")
                        Return View("DetailUseDeal", model)
                    End If
                End If
            End If

            If model.ListOfDoc.Count = 0 Then
                If Not IsNothing(model.FileTransFormulir) Then
                    If model.FileTransFormulir.ContentLength > 0 Then
                        If model.FileTransFormulir.ContentLength / 1000000 > CDec(MaxUploadSize) Then
                            ModelState.AddModelError("FileTransFormulir", "This file size is more than 2 MB, cannot upload")
                            Return View("DetailUseDeal", model)
                        End If
                    End If
                End If
            End If
            
            Dim th As New ExchangeTransactionHead
            th.AccName = accname
            th.AccNum = accno
            th.ApproveBy = ""
            th.ApproveDate = New DateTime(1900, 1, 1)
            th.BBankCode = bbankcode
            th.BBusinessTypeId = bbusinesstypeid
            th.BCountryId = bcountryid
            th.BranchId = branchid
            th.BSubjectStatusId = bsubjectstatusid
            th.DocumentStatementUnderlimitLink = docstatementunderlimitlink
            th.DocumentStatementOverlimitLink = docstatementoverlimitlink
            'th.DocumentTransactionLink = doctranslink
            'th.DocumentTransId = doctransid
            'th.DocUnderlyingNum = docunderlyingnumber
            th.EditBy = editby
            th.EditDate = editdate
            'th.DocUnderlyingNum = docunderlyingnumber
            If doclhbuid Is Nothing Or doclhbuid = "" Then
                'th.DocumentLHBUId = "998"
            Else
                'th.DocumentLHBUId = doclhbuid
            End If
            'th.PurposeId = purposeid
            th.RateType = ratetype
            th.SBankCode = sbankcode
            th.SBusinessTypeId = sbusinesstypeid
            th.SCountryId = scountryid
            th.SourceFunds = sourcefund
            th.SourceAccNum = sourceaccnum
            th.SourceAccName = sourceaccname
            th.SourceNominal = sourcenominal
            th.SSubjectStatusId = ssubjectstatusid
            th.IsNetting = isnetting
            th.DerivativeType = derivativetype
            th.NettingType = nettingtype
            If model.ListOfDeal.Count = 1 Then
                If model.PassLimitThreshold Or model.PassLimitUser Then
                    'Dim AnotherTrans = (From ex In db.ExchangeTransactionHead
                    '                  Where ex.AccNum = accno And ex.AccName = ex.AccName And ex.TDate.Date = tdate.Date
                    '                  Select ex).ToList

                    'If AnotherTrans.Count > 0 Then
                    '    For i As Integer = 0 To AnotherTrans.Count - 1
                    '        Dim editedtrans As ExchangeTransactionHead = ExchangeTransactionHead.GetExchangeTransHead(AnotherTrans.Item(i).TransNum, db)
                    '        editedtrans.Status = "REJECTED"
                    '        db.Entry(editedtrans).State = EntityState.Modified
                    '        db.SaveChanges()
                    '    Next
                    'Else
                    '    th.Status = "COUNTER - PENDING"
                    'End If
                    th.Status = "DEAL - PENDING"
                Else
                    th.Status = "ACTIVE"
                End If
            Else
                th.Status = "ACTIVE"
            End If
            th.TDate = tdate
            th.TransactionType = trantype
            th.CIF = CIF
            'th.TransFormulirLink = transformlink

            db.ExchangeTransactionHead.Add(th)
            db.SaveChanges()

            transnum = th.TransNum

            If Not IsNothing(model.FileDocumentStatementUnderlimit) Then
                If model.FileDocumentStatementUnderlimit.ContentLength > 0 Then
                    Dim filenamesplit As String() = model.FileDocumentStatementUnderlimit.FileName.Split(".")
                    Dim filename As String = "DocStatement(Underlimit)." & filenamesplit(filenamesplit.Length - 1)
                    Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                    If Not IO.Directory.Exists(Path) Then
                        IO.Directory.CreateDirectory(Path)
                    End If

                    th.DocumentStatementUnderlimitLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                    model.FileDocumentStatementUnderlimit.SaveAs(Path & filename)
                End If
            ElseIf model.PrevRefDocumentStatementUnderlimitName <> "" Then
                th.DocumentStatementUnderlimitLink = model.PrevRefDocumentStatementUnderlimitName
            End If

            If Not IsNothing(model.FileDocumentStatementOverlimit) Then
                If model.FileDocumentStatementOverlimit.ContentLength > 0 Then
                    Dim filenamesplit As String() = model.FileDocumentStatementOverlimit.FileName.Split(".")
                    Dim filename As String = "DocStatement(Overlimit)." & filenamesplit(filenamesplit.Length - 1)
                    Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                    If Not IO.Directory.Exists(Path) Then
                        IO.Directory.CreateDirectory(Path)
                    End If

                    th.DocumentStatementOverlimitLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                    model.FileDocumentStatementOverlimit.SaveAs(Path & filename)
                End If
            ElseIf model.PrevRefDocumentStatementOverlimitName <> "" Then
                th.DocumentStatementOverlimitLink = model.PrevRefDocumentStatementOverlimitName
            End If

            db.Entry(th).State = EntityState.Modified
            db.SaveChanges()

            Dim td As New ExchangeTransactionDetail
            If model.ListOfDeal.Count = 1 Then
                td = New ExchangeTransactionDetail

                td.CustomerCurrency = countercurr
                td.CustomerNominal = customeramount
                td.DealNumber = model.ListOfDeal(0).DealNumber
                td.TransactionCurrency = basecurr
                td.TransactionNominal = transamount
                td.TransactionRate = transrate
                td.TransNum = transnum
                td.ValueDate = valdate
                td.ValuePeriod = valperiod
                td.FlagReconcile = flagreconcile

                db.ExchangeTransactionDetail.Add(td)
            Else
                For Each dtl As TransactionDetail In model.ListOfDeal
                    td = New ExchangeTransactionDetail

                    td.CustomerCurrency = dtl.CounterCurrency
                    td.CustomerNominal = dtl.CounterNominal
                    td.DealNumber = dtl.DealNumber
                    td.TransactionCurrency = dtl.BaseCurrency
                    td.TransactionNominal = dtl.BaseNominal
                    td.TransactionRate = dtl.TransRate
                    td.TransNum = transnum
                    td.ValueDate = valdate
                    td.ValuePeriod = valperiod
                    td.FlagReconcile = flagreconcile

                    db.ExchangeTransactionDetail.Add(td)
                Next
            End If
            db.SaveChanges()

            If model.ListOfDeal.Count = 1 Then
                Dim tempdealnumber As String = model.ListOfDeal(0).DealNumber
                Dim editeddeal = db.TransactionDeal.Where(Function(f) f.DealNumber = tempdealnumber).First
                If dealFinished Then
                    editeddeal.Status = "FINISHED"
                Else
                    editeddeal.Status = "UNFINISHED"
                End If
                db.Entry(editeddeal).State = EntityState.Modified
            Else
                For Each dtl As TransactionDetail In model.ListOfDeal
                    Dim tempdealnumber As String = dtl.DealNumber
                    Dim editeddeal = db.TransactionDeal.Where(Function(f) f.DealNumber = tempdealnumber).FirstOrDefault
                    editeddeal.Status = "FINISHED"
                    db.Entry(editeddeal).State = EntityState.Modified
                Next
            End If
            db.SaveChanges()

            If model.ListOfDoc.Count = 0 Then
                Dim doc As New ExchangeTransactionDoc
                doc.TransNum = transnum
                doc.DocumentTransId = model.DocumentTransId
                doc.DocUnderlyingNum = model.DocUnderlyingNum
                doc.NominalDoc = model.DocUnderlyingNominal
                doc.DocumentLHBUId = "998"
                doc.PurposeId = model.PurposeId
                doc.DocumentTransactionLink = Nothing

                If Not IsNothing(model.FileTransFormulir) Then
                    If model.FileTransFormulir.ContentLength > 0 Then
                        Dim filenamesplit As String() = model.FileTransFormulir.FileName.Split(".")
                        Dim filename As String = "TransForm." & filenamesplit(filenamesplit.Length - 1)
                        Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                        If Not IO.Directory.Exists(Path) Then
                            IO.Directory.CreateDirectory(Path)
                        End If

                        doc.TransFormulirLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                        model.FileTransFormulir.SaveAs(Path & filename)
                    End If
                ElseIf model.PrevRefTransactionFormName <> "" Then
                    doc.TransFormulirLink = model.PrevRefTransactionFormName
                End If

                If Not IsNothing(model.FileDocumentTransaction) Then
                    If model.FileDocumentTransaction.ContentLength > 0 Then
                        Dim filenamesplit As String() = model.FileDocumentTransaction.FileName.Split(".")
                        Dim filename As String = "DocTrans." & filenamesplit(filenamesplit.Length - 1)
                        Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                        If Not IO.Directory.Exists(Path) Then
                            IO.Directory.CreateDirectory(Path)
                        End If

                        doc.DocumentTransactionLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                        model.FileDocumentTransaction.SaveAs(Path & filename)
                    End If
                ElseIf model.PrevRefDocumentTransactionName <> "" Then
                    doc.DocumentTransactionLink = model.PrevRefDocumentTransactionName
                End If

                db.ExchangeTransactionDoc.Add(doc)
            Else
                For Each row As ExchangeTransactionDoc In model.ListOfDoc
                    'If row.DocumentTransId IsNot Nothing Then
                    '    If model.FlagUploadUnderlying Then
                    '        Dim DocUnderlying As New MsDocUnderlying
                    '        DocUnderlying.DocNum = model.DocUnderlyingNum
                    '        DocUnderlying.DocType = db.DocumentTransaction.Where(Function(f) f.DocumentId = row.DocumentTransId).FirstOrDefault.DocumentType
                    '        DocUnderlying.TransNum = transnum
                    '        DocUnderlying.NominalDoc = model.DocUnderlyingNominal
                    '        db.DocUnderlying.Add(DocUnderlying)
                    '        db.SaveChanges()
                    '    End If
                    'End If

                    Dim doc As New ExchangeTransactionDoc
                    doc.TransNum = transnum
                    doc.DocumentTransId = row.DocumentTransId
                    doc.DocUnderlyingNum = row.DocUnderlyingNum
                    doc.NominalDoc = row.NominalDoc
                    doc.DocumentLHBUId = row.DocumentLHBUId
                    doc.PurposeId = row.PurposeId

                    For j As Integer = i To CountDocUnderlying
                        Dim FileDocUnderlying As HttpPostedFileBase = Session("FUDocUnderlying" & j)
                        If Not IsNothing(FileDocUnderlying) Then
                            If FileDocUnderlying.ContentLength > 0 Then
                                Dim filenamesplit As String() = FileDocUnderlying.FileName.Split(".")
                                Dim filename As String = "DocTrans" & j & "." & filenamesplit(filenamesplit.Length - 1)
                                Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                                If Not IO.Directory.Exists(Path) Then
                                    IO.Directory.CreateDirectory(Path)
                                End If

                                FileDocUnderlying.SaveAs(Path & filename)
                                doc.DocumentTransactionLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                                Exit For
                            End If
                        Else
                            If Session("PrevDocUnderlying" & j) IsNot Nothing Then
                                doc.DocumentTransactionLink = Session("PrevDocUnderlying" & j)
                            Else
                                doc.DocumentTransactionLink = ""
                            End If
                            Exit For
                        End If
                    Next

                    For j As Integer = i To CountTransForm
                        Dim FileTransForm As HttpPostedFileBase = Session("FUTransForm" & j)
                        If Not IsNothing(FileTransForm) Then
                            If FileTransForm.ContentLength > 0 Then
                                Dim filenamesplit As String() = FileTransForm.FileName.Split(".")
                                Dim filename As String = "TransForm" & j & "." & filenamesplit(filenamesplit.Length - 1)
                                Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                                If Not IO.Directory.Exists(Path) Then
                                    IO.Directory.CreateDirectory(Path)
                                End If

                                FileTransForm.SaveAs(Path & filename)
                                doc.TransFormulirLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                                Exit For
                            End If
                        Else
                            If Session("PrevTransForm" & j) IsNot Nothing Then
                                doc.TransFormulirLink = Session("PrevTransForm" & j)
                            Else
                                doc.TransFormulirLink = ""
                            End If
                            Exit For
                        End If
                    Next

                    i += 1

                    db.ExchangeTransactionDoc.Add(doc)
                Next
            End If

            Dim review As New ExchangeTransactionReview
            review.TransNum = transnum
            review.FlagReview = 0

            db.ExchangeTransactionReview.Add(review)

            db.SaveChanges()

            If model.ListOfDeal.Count = 1 Then
                LogTransaction.WriteLog(User.Identity.Name, "CREATE TRANSACTION", model.ListOfDeal.Item(0).DealNumber, db)
            Else
                For j As Integer = 0 To model.ListOfDeal.Count - 1
                    LogTransaction.WriteLog(User.Identity.Name, "CREATE TRANSACTION", model.ListOfDeal.Item(j).DealNumber, db)
                Next
            End If

            If model.AddAnotherTransaction Then
                Session("DealNumForNextTrans") = model.ListOfDeal(0).DealNumber
                Session("LastDealNominal") += model.TransactionNominal
                model = New ExchangeTransactionViewModel
                db = New FectorEntities

                model.TransNum = "(AUTO)"
                ViewData("Title") = "Create Deal Transaction"
                ViewBag.Breadcrumb = "Home > Exchange Transaction > Deal"
                ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
                ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
                ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
                ViewBag.CurrencyNoIDR = New SelectList(db.ExchangeRate.Where(Function(f) f.CurrId <> "IDR"), "CurrId", "CurrencyDisplay")
                ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
                ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
                ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
                ViewBag.SourceFundsOption = SourceFundsOption
                ViewBag.RateType = RateTypeOption
                ViewBag.TransactionTypeOption = TransactionTypeOption
                ViewBag.DealPeriodOption = DealPeriodOption
                ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")
                ViewBag.AddAnotherTransaction = "yes"

                Dim tmp As String = Session("DealNumForNextTrans")
                Dim tmpLastDealNominal As Decimal = CDec(Session("LastDealNominal"))
                Dim transdetail = (From d In db.TransactionDeal
                                    Where d.DealNumber = tmp
                                    Select New TransactionDetail With {.DealNumber = d.DealNumber, _
                                                                        .AccNumber = d.AccNum, _
                                                                        .AccName = d.AccName, _
                                                                        .BaseCurrency = d.CurrencyDeal, _
                                                                        .TransRate = d.DealRate, _
                                                                        .BaseNominal = d.AmountDeal, _
                                                                        .CounterCurrency = d.CurrencyCustomer, _
                                                                        .CounterNominal = d.AmountCustomer}).FirstOrDefault

                Dim selectdeal = (From d In db.TransactionDeal
                                    Where d.DealNumber = tmp
                                    Select New selectdeal With {.DealNumber = d.DealNumber, _
                                                                .AccNumber = d.AccNum, _
                                                                .AccName = d.AccName, _
                                                                .BaseCurrency = d.CurrencyDeal, _
                                                                .TransRate = d.DealRate, _
                                                                .BaseNominal = d.AmountDeal, _
                                                                .CounterCurrency = d.CurrencyCustomer, _
                                                                .CounterNominal = d.AmountCustomer, _
                                                                .BranchId = d.BranchId, _
                                                                .TransactionType = d.TransactionType, _
                                                                .RateType = d.DealType, _
                                                                .ValuePeriod = d.DealPeriod, _
                                                                .ValueDate = d.DealDate}).ToList

                With model
                    .AccNum = selectdeal.Item(0).AccNumber
                    .AccName = selectdeal.Item(0).AccName
                    .BranchId = selectdeal.Item(0).BranchId
                    .TransactionType = selectdeal.Item(0).TransactionType
                    .RateType = selectdeal.Item(0).RateType
                    .TransactionCurrency = selectdeal.Item(0).BaseCurrency
                    .CustomerCurrency = selectdeal.Item(0).CounterCurrency
                    .ValuePeriod = selectdeal.Item(0).ValuePeriod
                    .ValueDate = selectdeal.Item(0).ValueDate
                    .ListOfDeal.Add(transdetail)
                    .TransactionNominal = selectdeal.Item(0).BaseNominal - Session("LastDealNominal")
                    .TransactionRate = selectdeal.Item(0).TransRate
                    .CustomerNominal = selectdeal.Item(0).CounterNominal
                    .AddAnotherTransaction = False
                End With

                Session("dealNumber") = Session("DealNumForNextTrans")
                Session("accNumber") = selectdeal.Item(0).AccNumber
                Session("accName") = selectdeal.Item(0).AccName
                Session("transType") = selectdeal.Item(0).TransactionType
                Session("fromCurrency") = selectdeal.Item(0).BaseCurrency
                Session("toCurrency") = selectdeal.Item(0).CounterCurrency
                Session("valueDate") = selectdeal.Item(0).ValueDate.Date

                model.UseDeal = True

                model.BranchId = Session("BranchId")

                Return View("DetailUseDeal", model)
            Else
                Return Redirect("~/ExchangeTransaction")
            End If

            ' Remove session for uploading file
            For j As Integer = 0 To CountDocUnderlying - 1
                Session.Remove("FUDocUnderlying" & j)
                Session.Remove("PrevDocUnderlying" & j)
            Next

            For k As Integer = 0 To CountTransForm - 1
                Session.Remove("FUTransForm" & k)
            Next

            Session("docunderlyingid") = 0
            Session("transformid") = 0
            Session("CountDocUnderlying") = 0
            Session("CountTransForm") = 0
        End If

        Return View("DetailUseDeal", model)
    End Function

    Public Class selectdeal
        Public Property DealNumber As String
        Public Property AccNumber As String
        Public Property AccName As String
        Public Property BaseCurrency As String
        Public Property TransRate As Decimal
        Public Property BaseNominal As Decimal
        Public Property CounterCurrency As String
        Public Property CounterNominal As Decimal
        Public Property BranchId As String
        Public Property TransactionType As String
        Public Property RateType As String
        Public Property ValuePeriod As String
        Public Property ValueDate As Date
    End Class

    '
    ' Get: /ExchangeTransaction/Edit
    <Authorize> _
    Public Function Edit(ByVal id As String) As ActionResult
        Dim model As New ExchangeTransactionViewModel
        Dim db As New FectorEntities

        If IsNothing(id) OrElse id.Trim.Length = 0 Then
            Return RedirectToAction("Index", "ExchangeTransaction")
        End If

        Session("CountDocUnderlying") = 0
        Session("CountTransForm") = 0
        If IsNothing(model.TransNum) Then
            model = ExchangeTransactionViewModel.GetExchangeTransaction(id, db)

            If model.ListOfDoc IsNot Nothing Then
                For j As Integer = 0 To model.ListOfDoc.Count - 1
                    'If model.ListOfDoc.Item(j).DocumentTransactionLink IsNot Nothing Then
                    '    If model.ListOfDoc.Item(j).DocumentTransactionLink.Length > 0 Then
                    '        Session("CountDocUnderlying") += 1
                    '    End If
                    'End If

                    'If model.ListOfDoc.Item(j).TransFormulirLink IsNot Nothing Then
                    '    If model.ListOfDoc.Item(j).TransFormulirLink.Length > 0 Then
                    '        Session("CountTransForm") += 1
                    '    End If
                    'End If
                    Session("CountDocUnderlying") += 1

                    Session("CountTransForm") += 1
                Next
            End If
            

            Dim CountDocUnderlying As Integer = 0
            Dim CountTransForm As Integer = 0
            CountDocUnderlying = Session("CountDocUnderlying")
            CountTransForm = Session("CountTransForm")

            For i As Integer = 0 To CountDocUnderlying - 1
                Session.Add("PrevDocUnderlying" & i + 1, model.ListOfDoc(i).DocumentTransactionLink)
            Next

            For i As Integer = 0 To CountTransForm - 1
                Session.Add("PrevTransForm" & i + 1, model.ListOfDoc(i).TransFormulirLink)
            Next
        End If

        Dim ExchRateNoIDR = (From a In db.ExchangeRate
                             Where a.CoreCurrId <> "IDR" And a.CurrId <> model.TransactionCurrency
                             Select a).ToList

        Dim ExchRate = (From a In db.ExchangeRate
                        Where a.CurrId <> model.CustomerCurrency
                        Select a).ToList

        Dim ListCurrNoIDR As New List(Of GetCurrency_View)
        ListCurrNoIDR.Add(New GetCurrency_View With {.CurrId = model.TransactionCurrency, .CurrDisplay = model.TransactionCurrency})
        For i As Integer = 0 To ExchRateNoIDR.Count - 1
            ListCurrNoIDR.Add(New GetCurrency_View With {.CurrId = ExchRateNoIDR.Item(i).CurrId, .CurrDisplay = ExchRateNoIDR.Item(i).CurrencyDisplay})
        Next

        Dim ListCurr As New List(Of GetCurrency_View)
        ListCurr.Add(New GetCurrency_View With {.CurrId = model.CustomerCurrency, .CurrDisplay = model.CustomerCurrency})

        For i As Integer = 0 To ExchRate.Count - 1
            ListCurr.Add(New GetCurrency_View With {.CurrId = ExchRate.Item(i).CurrId, .CurrDisplay = ExchRate.Item(i).CurrencyDisplay})
        Next

        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
        ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(ListCurrNoIDR, "CurrId", "CurrDisplay")
        ViewBag.Currency = New SelectList(ListCurr, "CurrId", "CurrDisplay")
        ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
        ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
        ViewBag.SourceFundsOption = SourceFundsOption
        ViewBag.RateType = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption
        ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        If model.ListOfDeal Is Nothing Then
            ViewData("Title") = "Edit Transaction"
            ViewBag.Breadcrumb = "Home > Exchange Transaction > Edit"
        Else
            ViewData("Title") = "Edit Transaction"
            ViewBag.Breadcrumb = "Home > Exchange Transaction > Edit"
        End If

        If model.ListOfDeal Is Nothing Then
            Return View("Detail", model)
        Else
            Return View("DetailUseDeal", model)
        End If

    End Function

    '
    ' POST: /ExchangeTransaction/DeleteDeal
    <Authorize> _
    <HttpPost> _
    Public Function DeleteDeal() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("id")
        Dim model As New ExchangeTransactionViewModel
        model.TransNum = "(AUTO)"
        ViewData("Title") = "Create Exchange Transaction"
        ViewBag.Breadcrumb = String.Format("<a href='#'>Home</a> > <a href='#'>Exchange Transaction</a> > Create")
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction, "DocumentId", "DocumentTransactionDisplay")
        ViewBag.PurposeIdOption = New SelectList(db.Purposes, "PurposeId", "PurposeDisplay")
        ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
        ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
        ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
        ViewBag.SourceFundsOption = SourceFundsOption
        ViewBag.RateType = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption

        Dim ListDeal As String = Session("tmpDealNum")
        Dim List As New List(Of String)
        For i As Integer = 0 To ListDeal.Split(CChar(",")).Length - 1
            List.Add(ListDeal.Split(CChar(","))(i))
        Next

        If List.Contains(id) = True Then
            List.Remove(id)
        End If
        Dim GetDealNumNow As String = ""

        If List.Count > 0 Then
            For i As Integer = 0 To List.Count - 1
                GetDealNumNow &= List(i) + ","
            Next
            GetDealNumNow = GetDealNumNow.Substring(0, GetDealNumNow.Length - 1)
            Session("tmpDealNum") = GetDealNumNow
        Else
            Session("tmpDealNum") = ""
        End If

        'Return View("Detail", model)
        Return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString())
    End Function

    Private Class DeletedDocument
        Public Deleted As Boolean
    End Class

    '
    ' POST: /ExchangeTransaction/DeleteDocument
    Public Function DeleteDocument(FilePath As String, ButtonValue As String) As ActionResult
        Dim deleted As New DeletedDocument
        Try
            FilePath = UrlHelper.GenerateContentUrl(FilePath, Me.HttpContext)
            Dim GetFile As New FileInfo(Server.MapPath(FilePath))
            GetFile.Delete()
        Catch ex As Exception
            Return Json(New With {.Deleted = False})
        End Try
        Return Json(New With {.Deleted = True})
    End Function

    '
    ' Post: /ExchangeTransaction/Edit
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Edit(model As ExchangeTransactionViewModel) As ActionResult
        Dim db As New FectorEntities
        Dim file As HttpPostedFileBase = model.FileDocumentTransaction

        If model.ListOfDeal Is Nothing Then
            ViewData("Title") = "Edit Transaction"
            ViewBag.Breadcrumb = "Home > Exchange Transaction > Edit"
        Else
            ViewData("Title") = "Edit Transaction"
            ViewBag.Breadcrumb = "Home > Exchange Transaction > Edit"
        End If

        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
        ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(db.ExchangeRate.Where(Function(f) f.CurrId <> "IDR"), "CurrId", "CurrencyDisplay")
        ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
        ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
        ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
        ViewBag.SourceFundsOption = SourceFundsOption
        ViewBag.RateType = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption
        ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        If ModelState.IsValid Then
            Dim transnum As Decimal = model.TransNum
            Dim tdate As DateTime = Now
            Dim branchid As String = Session("BranchId")
            Dim accno As String = model.AccNum
            Dim accname As String = model.AccName
            Dim trantype As String = model.TransactionType
            Dim ratetype As String = model.RateType
            Dim basecurr As String = model.TransactionCurrency
            Dim countercurr As String = model.CustomerCurrency
            Dim bsubjectstatusid As String = ""
            Dim bbankcode As String = ""
            Dim bcountryid As String = ""
            Dim bbusinesstypeid As String = ""
            Dim ssubjectstatusid As String = ""
            Dim sbankcode As String = ""
            Dim scountryid As String = ""
            Dim sbusinesstypeid As String = ""
            Dim sourcefund As String = model.SourceFunds
            Dim sourceaccnum As String = model.SourceAccNum
            Dim sourceaccname As String = model.SourceAccName
            Dim sourcenominal As Decimal = model.SourceNominal
            Dim docunderlyingnum As String = model.DocUnderlyingNum
            Dim docunderlyingnominal As Decimal = model.DocUnderlyingNominal
            Dim doctransid As String = model.DocumentTransId
            Dim doclhbuid As String = model.DocumentLHBUId
            Dim purposeid As String = model.PurposeId
            Dim doctranslink As String = ""
            Dim docstatementunderlimitlink As String = ""
            Dim docstatementoverlimitlink As String = ""
            Dim transformlink As String = ""
            Dim editby As String = User.Identity.Name
            Dim editdate As DateTime = Now
            Dim status As String = ""
            Dim flagreconcile As Integer = 0
            Dim dealnumber As String = ""
            Dim transcurr As String = model.TransactionCurrency
            Dim transrate As Decimal = model.TransactionRate
            Dim transamount As Decimal = model.TransactionNominal
            Dim customercurr As String = model.CustomerCurrency
            Dim customeramount As Decimal = model.CustomerNominal
            Dim valperiod As String = model.ValuePeriod
            Dim valdate As DateTime = AppHelper.dateConvert(model.ValueDate)
            Dim isnetting As Boolean = model.IsNetting
            Dim derivativetype As Integer = model.DerivativeType
            Dim nettingtype As Integer = model.NettingType
            Dim dealFinished As Boolean
            Dim StatementAttachmentSize As Decimal = 0
            Dim TransformAttachmentSize As Decimal = 0
            Dim UnderlyingAttachmentSize As Decimal = 0
            Dim MaxUploadSize As String = MsSetting.GetSetting("UploadFile", "MaxSize", "", 1)

            Dim CountDocUnderlying As Integer = 0
            Dim CountTransForm As Integer = 0

            CountDocUnderlying = Session("CountDocUnderlying")
            CountTransForm = Session("CountTransForm")

            If model.ListOfDeal.Count = 1 Then
                dealnumber = model.ListOfDeal(0).DealNumber
                Dim dealamount As Decimal = model.ListOfDeal(0).BaseNominal
                Dim prevtransamount = (From det In db.ExchangeTransactionDetail
                                       Where det.DealNumber = dealnumber And det.TransNum <> transnum
                                       Select det).ToList.Sum(Function(f) f.TransactionNominal)

                If Not IsNothing(prevtransamount) Then
                    If prevtransamount + model.TransactionNominal > dealamount Then
                        ModelState.AddModelError("TransactionNominal", "Transaction amount surpasses deal amount")
                        Return View("DetailUseDeal", model)
                    ElseIf prevtransamount + model.TransactionNominal = dealamount Then
                        dealFinished = True
                    Else
                        dealFinished = False
                    End If
                End If
            Else
                For Each de As TransactionDetail In model.ListOfDeal
                    Dim detail = db.ExchangeTransactionDetail.Where(Function(f) f.TransNum = transnum)
                    Dim tempdealnumber As String = de.DealNumber
                    Dim dealreference = db.TransactionDeal.Where(Function(f) f.DealNumber = tempdealnumber).FirstOrDefault
                    If dealreference.Status <> "ACTIVE" Then
                        ModelState.AddModelError("DealNumber", "Deal " & de.DealNumber & " can not be used in multi deal transaction because it is already used partially")
                        Return View("DetailUseDeal", model)
                    End If
                Next
            End If

            Dim custinfo = (From accext In db.AccountsExtension
                               Join acc In db.Accounts On accext.AccNo Equals acc.CIF
                               Where acc.AccNo = accno
                               Select New With {.ssid = acc.SubjectStatusId, .bc = acc.BICode, .cid = acc.CountryId, .btid = acc.BusinessTypeId}).FirstOrDefault

            If custinfo.cid Is Nothing Then
                ModelState.AddModelError("AccNum", "Country not found for this customer, please complete the country in LLD")

                Return View("DetailUseDeal", model)
            End If

            If trantype = "Buy" Then
                'Fill Buyer with Bank Information
                bsubjectstatusid = MsSetting.GetSetting("Bank", "SubjectStatusId", "", 1)
                bbankcode = MsSetting.GetSetting("Bank", "BICode", "", 1)
                bcountryid = MsSetting.GetSetting("Bank", "CountryID", "", 1)
                bbusinesstypeid = MsSetting.GetSetting("Bank", "BusinessTypeID", "", 1)

                ssubjectstatusid = custinfo.ssid
                sbankcode = custinfo.bc
                scountryid = custinfo.cid
                sbusinesstypeid = custinfo.btid
            Else
                'Fill Seller with Bank Information
                ssubjectstatusid = MsSetting.GetSetting("Bank", "SubjectStatusId", "", 1)
                sbankcode = MsSetting.GetSetting("Bank", "BICode", "", 1)
                scountryid = MsSetting.GetSetting("Bank", "CountryID", "", 1)
                sbusinesstypeid = MsSetting.GetSetting("Bank", "BusinessTypeID", "", 1)

                bsubjectstatusid = custinfo.ssid
                bbankcode = custinfo.bc
                bcountryid = custinfo.cid
                bbusinesstypeid = custinfo.btid
            End If

            If Not IsNothing(model.FileDocumentStatementUnderlimit) Then
                If model.FileDocumentStatementUnderlimit.ContentLength > 0 Then
                    If model.FileDocumentStatementUnderlimit.ContentLength / 1000000 > CDec(MaxUploadSize) Then
                        ModelState.AddModelError("FileDocumentStatementUnderlimit", "This file size is more than 2 MB, cannot upload")
                        Return View("DetailUseDeal", model)
                    End If
                End If
            End If

            If Not IsNothing(model.FileDocumentStatementOverlimit) Then
                If model.FileDocumentStatementOverlimit.ContentLength > 0 Then
                    If model.FileDocumentStatementOverlimit.ContentLength / 1000000 > CDec(MaxUploadSize) Then
                        ModelState.AddModelError("FileDocumentStatementOverlimit", "This file size is more than 2 MB, cannot upload")
                        Return View("DetailUseDeal", model)
                    End If
                End If
            End If

            If model.ListOfDoc.Count = 0 Then
                If Not IsNothing(model.FileTransFormulir) Then
                    If model.FileTransFormulir.ContentLength > 0 Then
                        If model.FileTransFormulir.ContentLength / 1000000 > CDec(MaxUploadSize) Then
                            ModelState.AddModelError("FileTransFormulir", "This file size is more than 2 MB, cannot upload")
                            Return View("DetailUseDeal", model)
                        End If
                    End If
                End If
            End If

            Dim editedExchangeTransaction As ExchangeTransactionHead = ExchangeTransactionHead.GetExchangeTransHead(transnum, db)
            With editedExchangeTransaction
                .AccName = accname
                .AccNum = accno
                '.BranchId = branchid
                .ApproveBy = ""
                .ApproveDate = New DateTime(1900, 1, 1)
                .BBankCode = bbankcode
                .BBusinessTypeId = bbusinesstypeid
                .BCountryId = bcountryid
                .BSubjectStatusId = bsubjectstatusid
                .DocumentStatementUnderlimitLink = docstatementunderlimitlink
                '.DocumentTransactionLink = doctranslink
                '.DocumentTransId = doctransid
                .EditBy = editby
                .EditDate = editdate
                '.DocUnderlyingNum = docunderlyingnum
                If doclhbuid Is Nothing Or doclhbuid = "" Then
                    '.DocumentLHBUId = "998"
                Else
                    '.DocumentLHBUId = doclhbuid
                End If
                '.PurposeId = purposeid
                .RateType = ratetype
                .SBankCode = sbankcode
                .SBusinessTypeId = sbusinesstypeid
                .SCountryId = scountryid
                .SourceFunds = sourcefund
                .SourceAccNum = sourceaccnum
                .SourceAccName = sourceaccname
                .SourceNominal = sourcenominal
                .SSubjectStatusId = ssubjectstatusid
                .IsNetting = isnetting
                .DerivativeType = derivativetype
                .NettingType = nettingtype
                If model.PassLimitThreshold Or model.PassLimitUser Then
                    'Dim AnotherTrans = (From ex In db.ExchangeTransactionHead
                    '                  Where ex.AccNum = accno And ex.AccName = ex.AccName And ex.TDate.Date = tdate.Date
                    '                  Select ex).ToList

                    'If AnotherTrans.Count > 0 Then
                    '    For i As Integer = 0 To AnotherTrans.Count - 1
                    '        Dim editedtrans As ExchangeTransactionHead = ExchangeTransactionHead.GetExchangeTransHead(AnotherTrans.Item(i).TransNum, db)
                    '        editedtrans.Status = "REJECTED"
                    '        db.Entry(editedtrans).State = EntityState.Modified
                    '        db.SaveChanges()
                    '    Next
                    'Else
                    '    th.Status = "COUNTER - PENDING"
                    'End If
                    .Status = "EDIT DEAL - PENDING"
                Else
                    .Status = "ACTIVE"
                End If
                'If CheckLimit(accno, accname, trantype, ratetype, basecurr, transamount, valperiod, "EDIT", transnum) = False Then
                '    If model.ListOfDeal Is Nothing Then
                '        .Status = "EDIT COUNTER - PENDING"
                '    Else
                '        .Status = "EDIT DEAL - PENDING"
                '    End If
                'Else
                '    .Status = "ACTIVE"
                'End If
                .TransactionType = trantype
                '.TransFormulirLink = transformlink
            End With

            transnum = editedExchangeTransaction.TransNum
            If Not IsNothing(model.FileDocumentStatementUnderlimit) Then
                If model.FileDocumentStatementUnderlimit.ContentLength > 0 Then
                    Dim filenamesplit As String() = model.FileDocumentStatementUnderlimit.FileName.Split(".")
                    Dim filename As String = "DocStatement(Underlimit)." & filenamesplit(filenamesplit.Length - 1)
                    Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                    If Not IO.Directory.Exists(Path) Then
                        IO.Directory.CreateDirectory(Path)
                    End If

                    editedExchangeTransaction.DocumentStatementUnderlimitLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                    model.FileDocumentStatementUnderlimit.SaveAs(Path & filename)
                End If
            ElseIf model.PrevRefDocumentStatementUnderlimitName <> "" Then
                editedExchangeTransaction.DocumentStatementUnderlimitLink = model.PrevRefDocumentStatementUnderlimitName
            End If

            If Not IsNothing(model.FileDocumentStatementOverlimit) Then
                If model.FileDocumentStatementOverlimit.ContentLength > 0 Then
                    Dim filenamesplit As String() = model.FileDocumentStatementOverlimit.FileName.Split(".")
                    Dim filename As String = "DocStatement(Overlimit)." & filenamesplit(filenamesplit.Length - 1)
                    Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                    If Not IO.Directory.Exists(Path) Then
                        IO.Directory.CreateDirectory(Path)
                    End If

                    editedExchangeTransaction.DocumentStatementOverlimitLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                    model.FileDocumentStatementOverlimit.SaveAs(Path & filename)
                End If
            ElseIf model.PrevRefDocumentStatementOverlimitName <> "" Then
                editedExchangeTransaction.DocumentStatementOverlimitLink = model.PrevRefDocumentStatementOverlimitName
            End If

            'If Not IsNothing(model.FileTransFormulir) Then
            '    If model.FileTransFormulir.ContentLength > 0 Then
            '        Dim filenamesplit As String() = model.FileTransFormulir.FileName.Split(".")
            '        Dim filename As String = "TransForm." & filenamesplit(filenamesplit.Length - 1)
            '        Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

            '        If Not IO.Directory.Exists(Path) Then
            '            IO.Directory.CreateDirectory(Path)
            '        End If

            '        'editedExchangeTransaction.TransFormulirLink = "~/Uploads/Transaction/" & transnum & "/" & filename
            '        model.FileTransFormulir.SaveAs(Path & filename)
            '    End If
            'End If

            db.Entry(editedExchangeTransaction).State = EntityState.Modified

            Try
                db.SaveChanges()
            Catch ex As DbEntityValidationException
                For Each eve In ex.EntityValidationErrors
                    Dim errmessage = eve.ValidationErrors.Count
                Next
            End Try

            If model.ListOfDeal IsNot Nothing Then
                For i As Integer = 0 To model.ListOfDeal.Count - 1
                    Dim deletedTransNum = db.ExchangeTransactionDetail.Find(transnum, model.ListOfDeal(i).DealNumber)
                    db.ExchangeTransactionDetail.Remove(deletedTransNum)
                Next
            Else
                Dim deletedTransNum = db.ExchangeTransactionDetail.Where(Function(f) f.TransNum = transnum And f.DealNumber = "").ToList
                db.SaveChanges()
            End If

            If model.ListOfDoc.Count = 0 Then
                Dim doc As New ExchangeTransactionDoc
                doc.TransNum = transnum
                doc.DocumentTransId = model.DocumentTransId
                doc.DocUnderlyingNum = model.DocUnderlyingNum
                doc.NominalDoc = model.DocUnderlyingNominal
                doc.DocumentLHBUId = "998"
                doc.PurposeId = model.PurposeId
                doc.DocumentTransactionLink = Nothing

                If Not IsNothing(model.FileTransFormulir) Then
                    If model.FileTransFormulir.ContentLength > 0 Then
                        For Each row As ExchangeTransactionDoc In db.ExchangeTransactionDoc.Where(Function(f) f.TransNum = transnum)
                            DeleteDocument(row.TransFormulirLink, "")
                        Next

                        Dim filenamesplit As String() = model.FileTransFormulir.FileName.Split(".")
                        Dim filename As String = "TransForm." & filenamesplit(filenamesplit.Length - 1)
                        Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                        If Not IO.Directory.Exists(Path) Then
                            IO.Directory.CreateDirectory(Path)
                        End If

                        doc.TransFormulirLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                        model.FileTransFormulir.SaveAs(Path & filename)
                    End If
                ElseIf model.PrevRefDocumentTransactionName <> "" Then
                    doc.TransFormulirLink = model.PrevRefTransactionFormName
                End If

                If Not IsNothing(model.FileDocumentTransaction) Then
                    If model.FileDocumentTransaction.ContentLength > 0 Then
                        Dim filenamesplit As String() = model.FileDocumentTransaction.FileName.Split(".")
                        Dim filename As String = "DocTrans." & filenamesplit(filenamesplit.Length - 1)
                        Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                        If Not IO.Directory.Exists(Path) Then
                            IO.Directory.CreateDirectory(Path)
                        End If

                        doc.DocumentTransactionLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                        model.FileDocumentTransaction.SaveAs(Path & filename)
                    End If
                ElseIf model.PrevRefDocumentTransactionName <> "" Then
                    doc.DocumentTransactionLink = model.PrevRefDocumentTransactionName
                End If

                db.ExchangeTransactionDoc.Add(doc)
            Else
                Dim i As Integer = 1
                For Each row As ExchangeTransactionDoc In model.ListOfDoc
                    Dim doc As New ExchangeTransactionDoc
                    doc.TransNum = transnum
                    doc.DocumentTransId = row.DocumentTransId
                    doc.DocUnderlyingNum = row.DocUnderlyingNum
                    doc.NominalDoc = row.NominalDoc
                    If row.DocumentTransId IsNot Nothing Then
                        If row.DocumentLHBUId Is Nothing Then
                            doc.DocumentLHBUId = "998"
                        Else
                            doc.DocumentLHBUId = row.DocumentLHBUId
                        End If
                    Else
                        doc.DocumentLHBUId = Nothing
                    End If
                    doc.PurposeId = row.PurposeId

                    For j As Integer = i To CountDocUnderlying
                        Dim FileDocUnderlying As HttpPostedFileBase = Session("FUDocUnderlying" & j)
                        If Not IsNothing(FileDocUnderlying) Then
                            If FileDocUnderlying.ContentLength > 0 Then
                                For Each row2 As ExchangeTransactionDoc In db.ExchangeTransactionDoc.Where(Function(f) f.TransNum = transnum)
                                    DeleteDocument(row2.DocumentTransactionLink, "")
                                Next

                                Dim filenamesplit As String() = FileDocUnderlying.FileName.Split(".")
                                Dim filename As String = "DocTrans" & j & "." & filenamesplit(filenamesplit.Length - 1)
                                Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                                If Not IO.Directory.Exists(Path) Then
                                    IO.Directory.CreateDirectory(Path)
                                End If

                                FileDocUnderlying.SaveAs(Path & filename)
                                doc.DocumentTransactionLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                                Exit For
                            End If
                        Else
                            'If Session("PrevDocUnderlying" & j) IsNot Nothing Then
                            '    Dim filenamesplit As String() = Session("PrevDocUnderlying" & j).Split(".")
                            '    Dim filename As String = "DocTrans" & j & "." & filenamesplit(filenamesplit.Length - 1)
                            '    Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                            '    If Not IO.Directory.Exists(Path) Then
                            '        IO.Directory.CreateDirectory(Path)
                            '    End If

                            '    FileDocUnderlying.SaveAs(Path & filename)
                            '    doc.DocumentTransactionLink = Session("PrevDocUnderlying" & j)
                            'Else
                            '    doc.DocumentTransactionLink = ""
                            'End If
                            'Exit For
                            doc.DocumentTransactionLink = row.DocumentTransactionLink
                        End If
                    Next

                    For j As Integer = i To CountTransForm
                        Dim FileTransForm As HttpPostedFileBase = Session("FUTransForm" & j)
                        If Not IsNothing(FileTransForm) Then
                            If FileTransForm.ContentLength > 0 Then
                                For Each row2 As ExchangeTransactionDoc In db.ExchangeTransactionDoc.Where(Function(f) f.TransNum = transnum)
                                    DeleteDocument(row2.TransFormulirLink, "")
                                Next

                                Dim filenamesplit As String() = FileTransForm.FileName.Split(".")
                                Dim filename As String = "TransForm" & j & "." & filenamesplit(filenamesplit.Length - 1)
                                Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                                If Not IO.Directory.Exists(Path) Then
                                    IO.Directory.CreateDirectory(Path)
                                End If

                                FileTransForm.SaveAs(Path & filename)
                                doc.TransFormulirLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                                Exit For
                            End If
                        Else
                            'If Session("PrevTransForm" & j) IsNot Nothing Then
                            '    Dim filenamesplit As String() = Session("PrevTransForm" & j).Split(".")
                            '    Dim filename As String = "TransForm" & j & "." & filenamesplit(filenamesplit.Length - 1)
                            '    Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                            '    If Not IO.Directory.Exists(Path) Then
                            '        IO.Directory.CreateDirectory(Path)
                            '    End If

                            '    FileTransForm.SaveAs(Path & filename)
                            '    doc.TransFormulirLink = Session("PrevTransForm" & j)
                            'Else
                            '    doc.DocumentTransactionLink = ""
                            'End If
                            'Exit For
                            doc.TransFormulirLink = row.TransFormulirLink
                        End If
                    Next

                    i += 1
                    db.ExchangeTransactionDoc.Add(doc)
                Next
            End If

            For Each row As ExchangeTransactionDoc In db.ExchangeTransactionDoc.Where(Function(f) f.TransNum = transnum)
                db.ExchangeTransactionDoc.Remove(row)
            Next

            db.SaveChanges()

            Dim td As New ExchangeTransactionDetail
            If model.ListOfDeal.Count = 1 Then
                td = New ExchangeTransactionDetail

                td.CustomerCurrency = countercurr
                td.CustomerNominal = customeramount
                td.DealNumber = model.ListOfDeal(0).DealNumber
                td.TransactionCurrency = basecurr
                td.TransactionNominal = transamount
                td.TransactionRate = transrate
                td.TransNum = transnum
                td.ValueDate = valdate
                td.ValuePeriod = valperiod
                td.FlagReconcile = flagreconcile

                db.ExchangeTransactionDetail.Add(td)
            Else
                For Each dtl As TransactionDetail In model.ListOfDeal
                    td = New ExchangeTransactionDetail

                    td.CustomerCurrency = dtl.CounterCurrency
                    td.CustomerNominal = dtl.CounterNominal
                    td.DealNumber = dtl.DealNumber
                    td.TransactionCurrency = dtl.BaseCurrency
                    td.TransactionNominal = dtl.BaseNominal
                    td.TransactionRate = dtl.TransRate
                    td.TransNum = transnum
                    td.ValueDate = valdate
                    td.ValuePeriod = valperiod
                    td.FlagReconcile = flagreconcile

                    db.ExchangeTransactionDetail.Add(td)
                Next
            End If

            Try
                db.SaveChanges()
            Catch ex As DbEntityValidationException
                For Each eve In ex.EntityValidationErrors
                    Dim errmessage = eve.ValidationErrors.Count
                Next
            End Try

            If model.ListOfDeal.Count = 1 Then
                Dim tempdealnumber As String = model.ListOfDeal(0).DealNumber
                Dim editeddeal = db.TransactionDeal.Where(Function(f) f.DealNumber = tempdealnumber).FirstOrDefault
                If dealFinished Then
                    editeddeal.Status = "FINISHED"
                Else
                    editeddeal.Status = "UNFINISHED"
                End If
                db.Entry(editeddeal).State = EntityState.Modified
            Else
                For Each dtl As TransactionDetail In model.ListOfDeal
                    Dim tempdealnumber As String = dtl.DealNumber
                    Dim editeddeal = db.TransactionDeal.Where(Function(f) f.DealNumber = tempdealnumber).FirstOrDefault
                    editeddeal.Status = "FINISHED"
                    db.Entry(editeddeal).State = EntityState.Modified
                Next
            End If

            Dim deleteReview = db.ExchangeTransactionReview.Find(transnum)
            If deleteReview IsNot Nothing Then
                db.ExchangeTransactionReview.Remove(deleteReview)

                Dim review As New ExchangeTransactionReview
                review.TransNum = transnum
                review.FlagReview = 0

                db.ExchangeTransactionReview.Add(review)
            End If

            Try
                db.SaveChanges()
            Catch ex As DbEntityValidationException
                For Each eve In ex.EntityValidationErrors
                    Dim errmessage = eve.ValidationErrors.Count
                Next
            End Try

            'If model.FlagUploadUnderlying Then
            '    Dim DocUnderlying As New MsDocUnderlying
            '    DocUnderlying.DocNum = model.DocUnderlyingNum
            '    DocUnderlying.DocType = db.DocumentTransaction.Where(Function(f) f.DocumentId = model.DocumentTransId).FirstOrDefault.DocumentType
            '    DocUnderlying.TransNum = transnum
            '    DocUnderlying.NominalDoc = model.DocUnderlyingNominal
            '    db.DocUnderlying.Add(DocUnderlying)
            '    db.SaveChanges()
            'End If

            If model.ListOfDeal.Count = 1 Then
                LogTransaction.WriteLog(User.Identity.Name, "EDIT TRANSACTION", model.ListOfDeal.Item(0).DealNumber, db)
            Else
                For j As Integer = 0 To model.ListOfDeal.Count - 1
                    LogTransaction.WriteLog(User.Identity.Name, "EDIT TRANSACTION", model.ListOfDeal.Item(j).DealNumber, db)
                Next
            End If

            ' Remove session for uploading file
            For j As Integer = 0 To CountDocUnderlying - 1
                Session.Remove("FUDocUnderlying" & j)
                Session.Remove("PrevDocUnderlying" & j)
            Next

            For k As Integer = 0 To CountTransForm - 1
                Session.Remove("FUTransForm" & k)
            Next

            Session("docunderlyingid") = 0
            Session("transformid") = 0
            Session("CountDocUnderlying") = 0
            Session("CountTransForm") = 0


            Return Redirect("~/ExchangeTransaction")
        End If

        Return View("Detail", model)
    End Function

    Private Class GetCurrency_View
        Public Property CurrId As String
        Public Property CurrDisplay As String
    End Class

    '
    ' GET: /ExchangeTransaction/Viewed
    <Authorize> _
    Public Function Viewed(ByVal id As Integer) As ActionResult
        Dim model As New ExchangeTransactionViewModel
        Dim db As New FectorEntities

        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        If IsNothing(model.TransNum) Then
            model = ExchangeTransactionViewModel.GetExchangeTransaction(id, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        Dim ListCurrNoIDR As New List(Of GetCurrency_View)
        ListCurrNoIDR.Add(New GetCurrency_View With {.CurrId = model.TransactionCurrency, .CurrDisplay = model.TransactionCurrency})

        Dim ListCurr As New List(Of GetCurrency_View)
        ListCurr.Add(New GetCurrency_View With {.CurrId = model.CustomerCurrency, .CurrDisplay = model.CustomerCurrency})

        ViewData("Title") = "View Transaction"
        ViewBag.Breadcrumb = "Home > Exchange Transaction > View "
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
        ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(ListCurrNoIDR, "CurrId", "CurrDisplay")
        ViewBag.Currency = New SelectList(ListCurr, "CurrId", "CurrDisplay")
        ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
        ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
        ViewBag.SourceFundsOption = SourceFundsOption
        ViewBag.RateType = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption
        ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        If model.ListOfDeal Is Nothing Then
            Return View("Detail", model)
        Else
            Return View("DetailUseDeal", model)
        End If
    End Function

    '
    ' GET: /ExchangeTransaction/ViewDoc
    <Authorize> _
    Public Function ViewDoc(ByVal id As Integer) As ActionResult
        Dim model As New ExchangeTransactionViewModel
        Dim db As New FectorEntities

        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        If IsNothing(model.TransNum) Then
            model = ExchangeTransactionViewModel.GetExchangeTransaction(id, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        Dim ListCurrNoIDR As New List(Of GetCurrency_View)
        ListCurrNoIDR.Add(New GetCurrency_View With {.CurrId = model.TransactionCurrency, .CurrDisplay = model.TransactionCurrency})

        Dim ListCurr As New List(Of GetCurrency_View)
        ListCurr.Add(New GetCurrency_View With {.CurrId = model.CustomerCurrency, .CurrDisplay = model.CustomerCurrency})

        ViewData("Title") = "View Transaction"
        ViewBag.Breadcrumb = "Home > Exchange Transaction > View Doc. "
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
        ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(ListCurrNoIDR, "CurrId", "CurrDisplay")
        ViewBag.Currency = New SelectList(ListCurr, "CurrId", "CurrDisplay")
        ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
        ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
        ViewBag.SourceFundsOption = SourceFundsOption
        ViewBag.RateType = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption
        ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        If model.ListOfDeal Is Nothing Then
            Return View("Detail", model)
        Else
            Return View("DetailUseDeal", model)
        End If
    End Function

    '
    ' GET: /ExchangeTransaction/Reactive
    <Authorize> _
    Public Function ReactiveTrans(ByVal id As Integer) As ActionResult
        Dim model As New ExchangeTransactionViewModel
        Dim db As New FectorEntities

        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        If IsNothing(model.TransNum) Then
            model = ExchangeTransactionViewModel.GetExchangeTransaction(id, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        Dim ListCurrNoIDR As New List(Of GetCurrency_View)
        ListCurrNoIDR.Add(New GetCurrency_View With {.CurrId = model.TransactionCurrency, .CurrDisplay = model.TransactionCurrency})

        Dim ListCurr As New List(Of GetCurrency_View)
        ListCurr.Add(New GetCurrency_View With {.CurrId = model.CustomerCurrency, .CurrDisplay = model.CustomerCurrency})

        ViewData("Title") = "Reactive Transaction"
        ViewBag.Breadcrumb = "Home > Exchange Transaction > Reactive "
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
        ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(ListCurrNoIDR, "CurrId", "CurrDisplay")
        ViewBag.Currency = New SelectList(ListCurr, "CurrId", "CurrDisplay")
        ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
        ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
        ViewBag.SourceFundsOption = SourceFundsOption
        ViewBag.RateType = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption
        ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        If model.ListOfDeal Is Nothing Then
            Return View("Detail", model)
        Else
            Return View("DetailUseDeal", model)
        End If
    End Function

    '
    ' POST: /ExchangeTransaction/Process
    <Authorize> _
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    Public Function ReactiveTrans(model As ExchangeTransactionViewModel) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Reactive Transaction"
        ViewBag.Breadcrumb = "Home > Exchange Transaction > Reactive "
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
        ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(db.ExchangeRate.Where(Function(f) f.CurrId <> "IDR"), "CurrId", "CurrencyDisplay")
        ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
        ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
        ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
        ViewBag.SourceFundsOption = SourceFundsOption
        ViewBag.RateType = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption
        ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        Dim TransHead = ExchangeTransactionHead.GetExchangeTransHead(model.TransNum, db)

        TransHead.Status = "ACTIVE"
        TransHead.ApproveBy = User.Identity.Name
        TransHead.ApproveDate = Now

        db.Entry(TransHead).State = EntityState.Modified
        db.SaveChanges()

        For i As Integer = 0 To model.ListOfDeal.Count - 1
            LogTransaction.WriteLog(User.Identity.Name, "REACTIVED EXCHANGE TRANSACTION", model.ListOfDeal.Item(i).DealNumber, db)
        Next

        Return Redirect("~/ExchangeTransaction")
    End Function

    '
    ' GET: /ExchangeTransaction/Review
    <Authorize> _
    Public Function ReviewDoc(ByVal id As Integer) As ActionResult
        Dim model As New ExchangeTransactionViewModel
        Dim db As New FectorEntities

        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Session("CountDocUnderlying") = 0
        Session("CountTransForm") = 0

        If IsNothing(model.TransNum) Then
            model = ExchangeTransactionViewModel.GetExchangeTransaction(id, db)

            If model.ListOfDoc IsNot Nothing Then
                For j As Integer = 0 To model.ListOfDoc.Count - 1
                    'If model.ListOfDoc.Item(j).DocumentTransactionLink IsNot Nothing Then
                    '    If model.ListOfDoc.Item(j).DocumentTransactionLink.Length > 0 Then
                    '        Session("CountDocUnderlying") += 1
                    '    End If
                    'End If

                    'If model.ListOfDoc.Item(j).TransFormulirLink IsNot Nothing Then
                    '    If model.ListOfDoc.Item(j).TransFormulirLink.Length > 0 Then
                    '        Session("CountTransForm") += 1
                    '    End If
                    'End If
                    Session("CountDocUnderlying") += 1

                    Session("CountTransForm") += 1
                Next
            End If


            Dim CountDocUnderlying As Integer = 0
            Dim CountTransForm As Integer = 0
            CountDocUnderlying = Session("CountDocUnderlying")
            CountTransForm = Session("CountTransForm")

            For i As Integer = 0 To CountDocUnderlying - 1
                Session.Add("PrevDocUnderlying" & i + 1, model.ListOfDoc(i).DocumentTransactionLink)
            Next

            For i As Integer = 0 To CountTransForm - 1
                Session.Add("PrevTransForm" & i + 1, model.ListOfDoc(i).TransFormulirLink)
            Next
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        Dim ListCurrNoIDR As New List(Of GetCurrency_View)
        ListCurrNoIDR.Add(New GetCurrency_View With {.CurrId = model.TransactionCurrency, .CurrDisplay = model.TransactionCurrency})

        Dim ListCurr As New List(Of GetCurrency_View)
        ListCurr.Add(New GetCurrency_View With {.CurrId = model.CustomerCurrency, .CurrDisplay = model.CustomerCurrency})

        ViewData("Title") = "Review Transaction"
        ViewBag.Breadcrumb = "Home > Exchange Transaction > Review "
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
        ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(ListCurrNoIDR, "CurrId", "CurrDisplay")
        ViewBag.Currency = New SelectList(ListCurr, "CurrId", "CurrDisplay")
        ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
        ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
        ViewBag.SourceFundsOption = SourceFundsOption
        ViewBag.RateType = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption
        ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        If model.ListOfDeal Is Nothing Then
            Return View("Detail", model)
        Else
            Return View("DetailUseDeal", model)
        End If
    End Function

    '
    ' POST: /ExchangeTransaction/Review
    <Authorize> _
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    Public Function ReviewDoc(model As ExchangeTransactionViewModel) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Review Transaction"
        ViewBag.Breadcrumb = "Home > Exchange Transaction > Review "
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
        ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(db.ExchangeRate.Where(Function(f) f.CurrId <> "IDR"), "CurrId", "CurrencyDisplay")
        ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
        ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
        ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
        ViewBag.SourceFundsOption = SourceFundsOption
        ViewBag.RateType = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption
        ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        Dim transnum As String = model.TransNum

        Dim editedExchangeTransaction As ExchangeTransactionHead = ExchangeTransactionHead.GetExchangeTransHead(transnum, db)

        transnum = editedExchangeTransaction.TransNum

        Dim CountDocUnderlying As Integer = 0
        Dim CountTransForm As Integer = 0

        CountDocUnderlying = Session("CountDocUnderlying")
        CountTransForm = Session("CountTransForm")

        If model.TransactionType = "Sell" Then
            Dim StatementAttachmentSize As Decimal = 0
            Dim TransformAttachmentSize As Decimal = 0
            Dim UnderlyingAttachmentSize As Decimal = 0
            Dim MaxUploadSize As String = MsSetting.GetSetting("UploadFile", "MaxSize", "", 1)

            If Not IsNothing(model.FileDocumentStatementUnderlimit) Then
                If model.FileDocumentStatementUnderlimit.ContentLength > 0 Then
                    If model.FileDocumentStatementUnderlimit.ContentLength / 1000000 > CDec(MaxUploadSize) Then
                        ModelState.AddModelError("FileDocumentStatementUnderlimit", "This file size is more than 2 MB, cannot upload")
                        Return View("DetailUseDeal", model)
                    End If
                End If
            End If

            If Not IsNothing(model.FileDocumentStatementOverlimit) Then
                If model.FileDocumentStatementOverlimit.ContentLength > 0 Then
                    If model.FileDocumentStatementOverlimit.ContentLength / 1000000 > CDec(MaxUploadSize) Then
                        ModelState.AddModelError("FileDocumentStatementOverlimit", "This file size is more than 2 MB, cannot upload")
                        Return View("DetailUseDeal", model)
                    End If
                End If
            End If

            If model.ListOfDoc.Count = 0 Then
                If Not IsNothing(model.FileTransFormulir) Then
                    If model.FileTransFormulir.ContentLength > 0 Then
                        If model.FileTransFormulir.ContentLength / 1000000 > CDec(MaxUploadSize) Then
                            ModelState.AddModelError("FileTransFormulir", "This file size is more than 2 MB, cannot upload")
                            Return View("DetailUseDeal", model)
                        End If
                    End If
                End If
            End If

            If Not IsNothing(model.FileDocumentStatementUnderlimit) Then
                If model.FileDocumentStatementUnderlimit.ContentLength > 0 Then
                    Dim filenamesplit As String() = model.FileDocumentStatementUnderlimit.FileName.Split(".")
                    Dim filename As String = "DocStatement(Underlimit)." & filenamesplit(filenamesplit.Length - 1)
                    Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                    If Not IO.Directory.Exists(Path) Then
                        IO.Directory.CreateDirectory(Path)
                    End If

                    editedExchangeTransaction.DocumentStatementUnderlimitLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                    model.FileDocumentStatementUnderlimit.SaveAs(Path & filename)
                End If
            ElseIf model.PrevRefDocumentStatementUnderlimitName <> "" Then
                editedExchangeTransaction.DocumentStatementUnderlimitLink = model.PrevRefDocumentStatementUnderlimitName
            End If

            If Not IsNothing(model.FileDocumentStatementOverlimit) Then
                If model.FileDocumentStatementOverlimit.ContentLength > 0 Then
                    Dim filenamesplit As String() = model.FileDocumentStatementOverlimit.FileName.Split(".")
                    Dim filename As String = "DocStatement(Overlimit)." & filenamesplit(filenamesplit.Length - 1)
                    Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                    If Not IO.Directory.Exists(Path) Then
                        IO.Directory.CreateDirectory(Path)
                    End If

                    editedExchangeTransaction.DocumentStatementOverlimitLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                    model.FileDocumentStatementOverlimit.SaveAs(Path & filename)
                End If
            ElseIf model.PrevRefDocumentStatementOverlimitName <> "" Then
                editedExchangeTransaction.DocumentStatementOverlimitLink = model.PrevRefDocumentStatementOverlimitName
            End If

            db.Entry(editedExchangeTransaction).State = EntityState.Modified

            Try
                db.SaveChanges()
            Catch ex As DbEntityValidationException
                For Each eve In ex.EntityValidationErrors
                    Dim errmessage = eve.ValidationErrors.Count
                Next
            End Try

            If model.ListOfDoc.Count = 0 Then
                Dim doc As New ExchangeTransactionDoc
                doc.TransNum = transnum
                doc.DocumentTransId = Nothing
                doc.DocUnderlyingNum = Nothing
                doc.NominalDoc = model.DocUnderlyingNominal
                doc.DocumentLHBUId = "998"
                doc.PurposeId = model.PurposeId
                doc.DocumentTransactionLink = Nothing

                If Not IsNothing(model.FileTransFormulir) Then
                    If model.FileTransFormulir.ContentLength > 0 Then
                        For Each row As ExchangeTransactionDoc In db.ExchangeTransactionDoc.Where(Function(f) f.TransNum = transnum)
                            DeleteDocument(row.TransFormulirLink, "")
                        Next

                        Dim filenamesplit As String() = model.FileTransFormulir.FileName.Split(".")
                        Dim filename As String = "TransForm." & filenamesplit(filenamesplit.Length - 1)
                        Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                        If Not IO.Directory.Exists(Path) Then
                            IO.Directory.CreateDirectory(Path)
                        End If

                        doc.TransFormulirLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                        model.FileTransFormulir.SaveAs(Path & filename)
                    End If
                ElseIf model.PrevRefDocumentTransactionName <> "" Then
                    doc.TransFormulirLink = model.PrevRefTransactionFormName
                End If

                db.ExchangeTransactionDoc.Add(doc)
            Else
                Dim i As Integer = 1
                For Each row As ExchangeTransactionDoc In model.ListOfDoc
                    Dim doc As New ExchangeTransactionDoc
                    doc.TransNum = transnum
                    doc.DocumentTransId = row.DocumentTransId
                    doc.DocUnderlyingNum = row.DocUnderlyingNum
                    doc.NominalDoc = row.NominalDoc
                    If row.DocumentTransId IsNot Nothing Then
                        If row.DocumentLHBUId Is Nothing Then
                            doc.DocumentLHBUId = "998"
                        Else
                            doc.DocumentLHBUId = row.DocumentLHBUId
                        End If
                    Else
                        doc.DocumentLHBUId = Nothing
                    End If
                    doc.PurposeId = row.PurposeId

                    For j As Integer = i To CountDocUnderlying
                        Dim FileDocUnderlying As HttpPostedFileBase = Session("FUDocUnderlying" & j)
                        If Not IsNothing(FileDocUnderlying) Then
                            If FileDocUnderlying.ContentLength > 0 Then
                                For Each row2 As ExchangeTransactionDoc In db.ExchangeTransactionDoc.Where(Function(f) f.TransNum = transnum)
                                    DeleteDocument(row2.DocumentTransactionLink, "")
                                Next

                                Dim filenamesplit As String() = FileDocUnderlying.FileName.Split(".")
                                Dim filename As String = "DocTrans" & j & "." & filenamesplit(filenamesplit.Length - 1)
                                Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                                If Not IO.Directory.Exists(Path) Then
                                    IO.Directory.CreateDirectory(Path)
                                End If

                                FileDocUnderlying.SaveAs(Path & filename)
                                doc.DocumentTransactionLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                                Exit For
                            End If
                        Else
                            'If Session("PrevDocUnderlying" & j) IsNot Nothing Then
                            '    Dim filenamesplit As String() = Session("PrevDocUnderlying" & j).Split(".")
                            '    Dim filename As String = "DocTrans" & j & "." & filenamesplit(filenamesplit.Length - 1)
                            '    Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                            '    If Not IO.Directory.Exists(Path) Then
                            '        IO.Directory.CreateDirectory(Path)
                            '    End If

                            '    FileDocUnderlying.SaveAs(Path & filename)
                            '    doc.DocumentTransactionLink = Session("PrevDocUnderlying" & j)
                            'Else
                            '    doc.DocumentTransactionLink = ""
                            'End If
                            'Exit For
                            doc.DocumentTransactionLink = row.DocumentTransactionLink
                        End If
                    Next

                    For j As Integer = i To CountTransForm
                        Dim FileTransForm As HttpPostedFileBase = Session("FUTransForm" & j)
                        If Not IsNothing(FileTransForm) Then
                            If FileTransForm.ContentLength > 0 Then
                                For Each row2 As ExchangeTransactionDoc In db.ExchangeTransactionDoc.Where(Function(f) f.TransNum = transnum)
                                    DeleteDocument(row2.TransFormulirLink, "")
                                Next

                                Dim filenamesplit As String() = FileTransForm.FileName.Split(".")
                                Dim filename As String = "TransForm" & j & "." & filenamesplit(filenamesplit.Length - 1)
                                Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                                If Not IO.Directory.Exists(Path) Then
                                    IO.Directory.CreateDirectory(Path)
                                End If

                                FileTransForm.SaveAs(Path & filename)
                                doc.TransFormulirLink = "~/Uploads/Transaction/" & transnum & "/" & filename
                                Exit For
                            End If
                        Else
                            'If Session("PrevTransForm" & j) IsNot Nothing Then
                            '    Dim filenamesplit As String() = Session("PrevTransForm" & j).Split(".")
                            '    Dim filename As String = "TransForm" & j & "." & filenamesplit(filenamesplit.Length - 1)
                            '    Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

                            '    If Not IO.Directory.Exists(Path) Then
                            '        IO.Directory.CreateDirectory(Path)
                            '    End If

                            '    FileTransForm.SaveAs(Path & filename)
                            '    doc.TransFormulirLink = Session("PrevTransForm" & j)
                            'Else
                            '    doc.DocumentTransactionLink = ""
                            'End If
                            'Exit For
                            doc.TransFormulirLink = row.TransFormulirLink
                        End If
                    Next

                    i += 1
                    db.ExchangeTransactionDoc.Add(doc)
                Next
            End If

            For Each row As ExchangeTransactionDoc In db.ExchangeTransactionDoc.Where(Function(f) f.TransNum = transnum)
                db.ExchangeTransactionDoc.Remove(row)
            Next

            ' Remove session for uploading file
            For j As Integer = 0 To CountDocUnderlying - 1
                Session.Remove("FUDocUnderlying" & j)
                Session.Remove("PrevDocUnderlying" & j)
            Next

            For k As Integer = 0 To CountTransForm - 1
                Session.Remove("FUTransForm" & k)
            Next

            Session("docunderlyingid") = 0
            Session("transformid") = 0
            Session("CountDocUnderlying") = 0
            Session("CountTransForm") = 0

            db.SaveChanges()
        End If

        Dim Reviewed = ExchangeTransactionReview.GetExchangeTransReview(transnum, db)

        If Reviewed IsNot Nothing Then
            Reviewed.FlagReview = 1
            Reviewed.ReviewBy = User.Identity.Name
            Reviewed.ReviewDate = Now

            db.Entry(Reviewed).State = EntityState.Modified
        Else
            Dim NewReview As New ExchangeTransactionReview
            NewReview.TransNum = model.TransNum
            NewReview.FlagReview = 1
            NewReview.ReviewBy = User.Identity.Name
            NewReview.ReviewDate = Now

            db.ExchangeTransactionReview.Add(NewReview)
        End If
        
        db.SaveChanges()
        For i As Integer = 0 To model.ListOfDeal.Count - 1
            LogTransaction.WriteLog(User.Identity.Name, "REVIEW EXCHANGE TRANSACTION", model.ListOfDeal.Item(i).DealNumber, db)
        Next

        Return Redirect("~/ExchangeTransaction/Review")
    End Function

    '
    ' GET: /ExchangeTransaction/ViewHistory
    <Authorize> _
    Public Function ViewHistory(ByVal id As Integer) As ActionResult
        Dim model As New ExchangeTransactionViewModel
        Dim db As New FectorEntities

        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        If IsNothing(model.TransNum) Then
            model = ExchangeTransactionViewModel.GetExchangeTransaction(id, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        Dim ListCurrNoIDR As New List(Of GetCurrency_View)
        ListCurrNoIDR.Add(New GetCurrency_View With {.CurrId = model.TransactionCurrency, .CurrDisplay = model.TransactionCurrency})

        Dim ListCurr As New List(Of GetCurrency_View)
        ListCurr.Add(New GetCurrency_View With {.CurrId = model.CustomerCurrency, .CurrDisplay = model.CustomerCurrency})

        ViewData("Title") = "View History Transaction"
        ViewBag.Breadcrumb = "Home > Exchange Transaction > History "
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
        ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(ListCurrNoIDR, "CurrId", "CurrDisplay")
        ViewBag.Currency = New SelectList(ListCurr, "CurrId", "CurrDisplay")
        ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
        ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
        ViewBag.SourceFundsOption = SourceFundsOption
        ViewBag.RateType = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption
        ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        If model.ListOfDeal Is Nothing Then
            Return View("Detail", model)
        Else
            Return View("DetailUseDeal", model)
        End If
    End Function

    '
    ' POST: /ExchangeTransaction/Process
    <Authorize> _
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    Public Function Viewed(model As ExchangeTransactionViewModel) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Process Transaction"
        ViewBag.Breadcrumb = "Home > Exchange Transaction > Process "
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
        ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(db.ExchangeRate.Where(Function(f) f.CurrId <> "IDR"), "CurrId", "CurrencyDisplay")
        ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
        ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
        ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
        ViewBag.SourceFundsOption = SourceFundsOption
        ViewBag.RateType = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption
        ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        Dim TransHead = ExchangeTransactionHead.GetExchangeTransHead(model.TransNum, db)

        TransHead.Status = "DELETE - PENDING"
        TransHead.ApproveBy = User.Identity.Name
        TransHead.ApproveDate = Now

        db.Entry(TransHead).State = EntityState.Modified
        db.SaveChanges()

        For i As Integer = 0 To model.ListOfDeal.Count - 1
            LogTransaction.WriteLog(User.Identity.Name, "DELETED EXCHANGE TRANSACTION", model.ListOfDeal.Item(i).DealNumber, db)
        Next

        Return Redirect("~/ExchangeTransaction")
    End Function

    '
    ' GET: /ExchangeTransaction/Process
    <Authorize> _
    Public Function Process(ByVal id As Integer) As ActionResult
        Dim model As New ExchangeTransactionViewModel
        Dim db As New FectorEntities

        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        If IsNothing(model.TransNum) Then
            model = ExchangeTransactionViewModel.GetExchangeTransaction(id, db)
        End If

        If IsNothing(model) Then
            Return New HttpNotFoundResult
        End If

        Dim ExchRateNoIDR = (From a In db.ExchangeRate
                             Where a.CoreCurrId <> "IDR" And a.CurrId <> model.TransactionCurrency
                             Select a).ToList

        Dim ExchRate = (From a In db.ExchangeRate
                        Where a.CurrId <> model.CustomerCurrency
                        Select a).ToList

        Dim ListCurrNoIDR As New List(Of GetCurrency_View)
        ListCurrNoIDR.Add(New GetCurrency_View With {.CurrId = model.TransactionCurrency, .CurrDisplay = model.TransactionCurrency})
        For i As Integer = 0 To ExchRateNoIDR.Count - 1
            ListCurrNoIDR.Add(New GetCurrency_View With {.CurrId = ExchRateNoIDR.Item(i).CurrId, .CurrDisplay = ExchRateNoIDR.Item(i).CurrencyDisplay})
        Next

        Dim ListCurr As New List(Of GetCurrency_View)
        ListCurr.Add(New GetCurrency_View With {.CurrId = model.CustomerCurrency, .CurrDisplay = model.CustomerCurrency})

        For i As Integer = 0 To ExchRate.Count - 1
            ListCurr.Add(New GetCurrency_View With {.CurrId = ExchRate.Item(i).CurrId, .CurrDisplay = ExchRate.Item(i).CurrencyDisplay})
        Next

        ViewData("Title") = "Process Counter Transaction"
        ViewBag.Breadcrumb = "Home > Exchange Transaction > Process "
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
        ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(ListCurrNoIDR, "CurrId", "CurrDisplay")
        ViewBag.Currency = New SelectList(ListCurr, "CurrId", "CurrDisplay")
        ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
        ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
        ViewBag.SourceFundsOption = SourceFundsOption
        ViewBag.RateType = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption
        ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        If model.ListOfDeal Is Nothing Then
            Return View("Detail", model)
        Else
            Return View("DetailUseDeal", model)
        End If
    End Function

    '
    ' POST: /ExchangeTransaction/Process
    <Authorize> _
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    Public Function Process(model As ExchangeTransactionViewModel, approvalButton As String) As ActionResult
        Dim db As New FectorEntities

        ViewData("Title") = "Process Counter Transaction"
        ViewBag.Breadcrumb = "Home > Exchange Transaction > Process "
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
        ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
        ViewBag.CurrencyNoIDR = New SelectList(db.ExchangeRate.Where(Function(f) f.CurrId <> "IDR"), "CurrId", "CurrencyDisplay")
        ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
        ViewBag.DerivativeType = New SelectList(db.LKDerivativeType, "ID", "Description")
        ViewBag.NettingType = New SelectList(db.LKNettingType, "ID", "Description")
        ViewBag.SourceFundsOption = SourceFundsOption
        ViewBag.RateType = RateTypeOption
        ViewBag.TransactionTypeOption = TransactionTypeOption
        ViewBag.DealPeriodOption = DealPeriodOption
        ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

        Dim TransHead = ExchangeTransactionHead.GetExchangeTransHead(model.TransNum, db)

        TransHead.ApproveBy = User.Identity.Name
        TransHead.ApproveDate = Now
        Dim Status As String = ""
        Select Case TransHead.Status
            Case "COUNTER - PENDING", "DEAL - PENDING", "EDIT COUNTER - PENDING", "EDIT DEAL - PENDING"
                If approvalButton = "Approve" Then
                    TransHead.Status = "ACTIVE"
                    Status = "APPROVE"
                Else
                    TransHead.Status = "REJECTED"
                    Status = "REJECT"
                End If
            Case "DELETE - PENDING"
                If approvalButton = "Approve" Then
                    TransHead.Status = "INACTIVE"
                    Status = "DELETE"
                Else
                    TransHead.Status = "ACTIVE"
                    Status = "ACTIVE"
                End If
        End Select

        db.Entry(TransHead).State = EntityState.Modified
        db.SaveChanges()

        For i As Integer = 0 To model.ListOfDeal.Count - 1
            LogTransaction.WriteLog(User.Identity.Name, Status & " EXCHANGE TRANSACTION", model.ListOfDeal.Item(i).DealNumber, db)
        Next

        Return Redirect("~/ExchangeTransaction/Approval")
    End Function

    '
    ' POST : /ExchangeTransaction/CloseTransaction
    <HttpPost> _
    Function CloseTransaction(model As ExchangeTransactionClose) As ActionResult
        Dim db As New FectorEntities

        Dim CloseTrans As New ExchangeTransactionClose

        CloseTrans.TransNum = model.TransNum
        CloseTrans.DealNumber = model.DealNumber
        CloseTrans.CloseTransactionReason = model.CloseTransactionReason
        CloseTrans.CloseBy = User.Identity.Name
        CloseTrans.CloseDate = Now
        db.ExchangeTransactionClose.Add(CloseTrans)
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "CLOSE TRANSACTION", model.DealNumber & " / " & model.DealNumber, db)

        Return RedirectToAction("CloseTransaction")
    End Function

    Private Class AllAccount
        Public AccNo As String
        Public AccName As String
        Public TODLimitCurrency As String
        Public TODLimit As Nullable(Of Decimal)
        Public TOMLimitCurrency As String
        Public TOMLimit As Nullable(Of Decimal)
        Public SPOTLimitCurrency As String
        Public SPOTLimit As Nullable(Of Decimal)
        Public AllLimitCurrency As String
        Public AllLimit As Nullable(Of Decimal)
        Public Status As String
    End Class

    Public Function CheckAnotherTrans(AccNo As String, AccName As String, TDate As Date) As Boolean
        Dim db As New FectorEntities
        Dim AnotherTrans = (From ex In db.ExchangeTransactionHead
                          Where ex.AccNum = AccNo And ex.AccName = ex.AccName And ex.TDate.Date = TDate.Date
                          Select ex).ToList

        If AnotherTrans.Count > 0 Then
            For i As Integer = 0 To AnotherTrans.Count - 1
                Dim editedtrans As ExchangeTransactionHead = ExchangeTransactionHead.GetExchangeTransHead(AnotherTrans.Item(i).TransNum, db)
                editedtrans.Status = "REJECTED"
                db.Entry(editedtrans).State = EntityState.Modified
                db.SaveChanges()
            Next
            Return True
        Else
            Return False
        End If
    End Function

    Private Function getWorkingDate(ByVal dtInitial As DateTime, ByVal BaseCurrency As String, Optional ValueType As String = "") As DateTime
        If isHoliday(dtInitial, BaseCurrency) Then
            If ValueType = "SPOT" Then
                Return getWorkingDate(dtInitial.AddDays(2), BaseCurrency)
            Else
                Return getWorkingDate(dtInitial.AddDays(1), BaseCurrency)
            End If

        Else
            Return dtInitial
        End If
    End Function

    Private Function isHoliday(ByVal dtCheck As DateTime, ByVal BaseCurrency As String) As Boolean
        If dtCheck.DayOfWeek = DayOfWeek.Sunday Or dtCheck.DayOfWeek = DayOfWeek.Saturday Then
            Return True
        End If

        Dim db As New FectorEntities
        Dim temp = From hl In db.Holidays
                    Group Join no In db.MappingNostro On hl.Nostro Equals no.BIC Into Group From p In Group.DefaultIfEmpty
                    Where dtCheck.Date <= hl.EndDate And dtCheck.Date >= hl.StartDate
                    Select New With {hl.HolidayDesc}

        Dim holiday = (From hl In db.Holidays
                       Group Join no In db.MappingNostro On hl.Nostro Equals no.BIC Into Group From p In Group.DefaultIfEmpty
                        Where dtCheck.Date <= hl.EndDate And dtCheck.Date >= hl.StartDate And p.CurrID = BaseCurrency
                        Select New With {hl.HolidayDesc}).FirstOrDefault

        If IsNothing(holiday) Then
            Return False
        Else
            Return True
        End If
    End Function

    Class DocumentReqOutput
        Public Property PassNominalUnderlying As Boolean = True
        Public Property DocTransLinkName As String
        Public Property DocTransLinkSave As String
    End Class

    Class EditDocumentReqOutput
        Public Property UnderLimit As Boolean

        Public Property HaveDocTransLink As Boolean
        Public Property DocTransLinkName As String
        Public Property DocTransLinkSave As String
        Public Property NominalUnderlying As Decimal
        Public Property RemainingUnderlying As Decimal

        Public Property HaveDocStatementUnderlimitLink As Boolean
        Public Property DocStatementUnderlimitLinkName As String
        Public Property DocStatementUnderlimitLinkSave As String

        Public Property HaveDocStatementOverlimitLink As Boolean
        Public Property DocStatementOverlimitLinkName As String
        Public Property DocStatementOverlimitLinkSave As String

        Public Property HaveTransFormulirLink As Boolean
        Public Property TransFormulirLinkName As String
        Public Property TransFormulirLinkSave As String

        Public Property IsOtherAccount As Boolean
    End Class

    Private Class TransDetail
        Public TransNum As Decimal
        Public DealNumber As String
        Public TransactionCurrency As String
        Public TransactionRate As Decimal
        Public TransactionNominal As Decimal
        Public CustomerCurrency As String
        Public CustomerNominal As Decimal
        Public ValuePeriod As String
        Public ValueDate As DateTime
        Public FlagReconsile As Integer
    End Class

    Class dealInfo
        Public Property dealNumber As String
        Public Property accNumber As String
        Public Property accName As String
        Public Property tranType As String
        Public Property dealType As String
        Public Property fromCurrency As String
        Public Property toCurrency As String
        Public Property exchRate As Decimal
        Public Property strexchRate As String
        Public Property valuePeriod As String
        Public Property valueDate As Date
        Public Property strvalueDate As String
        Public Property counterAmount As Decimal
        Public Property strcounterAmount As String
        Public Property dealAmount As Decimal
        Public Property strdealAmount As String

        Public Property ResultCheck As String
    End Class

    <HttpPost> _
    Public Function AddDeal(model As ExchangeTransactionViewModel, DealNumber As String) As ActionResult
        Dim db As New FectorEntities
        Dim dealintran As New TransactionDetail

        Dim deal = (From dl In db.TransactionDeal
                   Where dl.DealNumber = DealNumber
                   Select New TransactionDetail With {.AccName = dl.AccName, .AccNumber = dl.AccNum, _
                                                     .BaseCurrency = dl.CurrencyDeal, .BaseNominal = dl.AmountDeal, _
                                                     .CounterCurrency = dl.CurrencyCustomer, _
                                                     .CounterNominal = dl.AmountCustomer, .DealNumber = dl.DealNumber, _
                                                     .TransRate = dl.DealRate}).FirstOrDefault

        model.ListOfDeal.Add(deal)

        Return View("DetailUseDeal", model)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTDealAjaxHandler() As JsonResult
        Dim db As New FectorEntities()
        tempDealNumber = Session("tmpDealNum")
        Dim TransactionDeal = From a In db.TransactionDeal
                        Where tempDealNumber.Contains(a.DealNumber)
                        Order By a.DealNumber
                        Select a
        Return ReturnDealDataTable(TransactionDeal)
    End Function

    Private Function ReturnDealDataTable(TransactionDeal As IQueryable(Of TransactionDeal)) As JsonResult
        Dim totalRecords As Integer = TransactionDeal.Count
        Dim displayRecord As Integer = 0

        displayRecord = TransactionDeal.Count

        Dim result As New List(Of String())
        For Each d As TransactionDeal In TransactionDeal
            result.Add({d.DealNumber, d.AccNum, d.AccName, d.BranchId, d.TransactionType, d.DealType, d.CurrencyDeal, CDec(d.DealRate).ToString("N2"), CDec(d.AmountDeal).ToString("N2"), d.CurrencyCustomer, CDec(d.RateCustomer).ToString("N2"), CDec(d.AmountCustomer).ToString("N2"), d.DealPeriod, CDate(d.DealDate).ToString("dd/MM/yyyy"), d.Status, ""})
        Next

        Return Json(New With {.iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = displayRecord,
                      .aaData = result},
                      JsonRequestBehavior.AllowGet)
    End Function

    Private Class ExchangeTransaction
        Public TransNum As String
        Public DealNumber As String
        Public TDate As DateTime
        Public AccNum As String
        Public AccName As String
        Public TransactionType As String
        Public RateType As String
        Public SourceFunds As String
        Public FlagReconsile As String
        Public Status As String
        Public BranchId As String
    End Class

    <Authorize> _
    <HttpGet> _
    Public Function DTExchangeTransactionReactiveAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim HeadBranch = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1
        Dim UserBranchID As Decimal = Session("BranchId")
        Dim ExchangeTransaction As IQueryable(Of ExchangeTransaction)

        If UserBranchID = HeadBranch Then
            ExchangeTransaction = From s In db.ExchangeTransactionDetail
                                  Group Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum Into Group From t In Group.DefaultIfEmpty
                                  Where s.FlagReconcile = 0 And t.Status = "INACTIVE"
                                  Select New ExchangeTransaction With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = t.TDate, _
                                                                       .AccNum = t.AccNum, .AccName = t.AccName, _
                                                                       .TransactionType = t.TransactionType, .RateType = t.RateType, .SourceFunds = t.SourceFunds, .Status = t.Status, .BranchId = UserBranchID}
        Else
            ExchangeTransaction = From s In db.ExchangeTransactionDetail
                                  Group Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum Into Group From t In Group.DefaultIfEmpty
                                  Where t.BranchId = UserBranchID And s.FlagReconcile = 0 And t.Status = "INACTIVE"
                                  Select New ExchangeTransaction With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = t.TDate, _
                                                                       .AccNum = t.AccNum, .AccName = t.AccName, _
                                                                       .TransactionType = t.TransactionType, .RateType = t.RateType, .SourceFunds = t.SourceFunds, .Status = t.Status, .BranchId = UserBranchID}
        End If

        Return ReturnExchangeTransactionDataTable(param, ExchangeTransaction, "Reactive")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTExchangeTransactionReviewAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim HeadBranch = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1
        Dim UserBranchID As Decimal = Session("BranchId")
        Dim ExchangeTransaction As IQueryable(Of ExchangeTransaction)

        'If UserBranchID = HeadBranch Then

        'Else
        '    ExchangeTransaction = From a In db.ExchangeTransactionReview
        '                          Join s In db.ExchangeTransactionDetail On a.TransNum Equals s.TransNum
        '                          Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum
        '                          Where a.FlagReview = 0 And t.BranchId = UserBranchID
        '                          Order By s.DealNumber
        '                          Select New ExchangeTransaction With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = t.TDate, _
        '                                                               .AccNum = t.AccNum, .AccName = t.AccName, _
        '                                                               .TransactionType = t.TransactionType, .RateType = t.RateType, .SourceFunds = t.SourceFunds, .FlagReconsile = s.FlagReconcile, .Status = a.FlagReview, .BranchId = UserBranchID}
        'End If
        ExchangeTransaction = From s In db.ExchangeTransactionDetail
                              Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum
                              Group Join v In db.ExchangeTransactionReview On s.TransNum Equals v.TransNum Into sv = Group
                              Group Join u In db.ExchangeTransactionClose On s.TransNum Equals u.TransNum Into su = Group
                                Where su.FirstOrDefault.DealNumber Is Nothing And t.Status = "ACTIVE"
                                Order By s.DealNumber
                                Select New ExchangeTransaction With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = t.TDate, _
                                                                       .AccNum = t.AccNum, .AccName = t.AccName, _
                                                                       .TransactionType = t.TransactionType, .RateType = t.RateType, .SourceFunds = t.SourceFunds, .FlagReconsile = s.FlagReconcile, .Status = If(CStr(sv.FirstOrDefault.FlagReview) Is Nothing, 0, sv.FirstOrDefault.FlagReview), .BranchId = UserBranchID}
                                  

        Return ReturnExchangeTransactionDataTable(param, ExchangeTransaction, "Review")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTExchangeTransactionClosedAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim HeadBranch = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1
        Dim UserBranchID As Decimal = Session("BranchId")
        Dim ExchangeTransaction As IQueryable(Of ExchangeTransaction)

        'If UserBranchID = HeadBranch Then

        'Else
        '    ExchangeTransaction = From a In db.ExchangeTransactionReview
        '                          Join s In db.ExchangeTransactionDetail On a.TransNum Equals s.TransNum
        '                          Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum
        '                          Where a.FlagReview = 0 And t.BranchId = UserBranchID
        '                          Order By s.DealNumber
        '                          Select New ExchangeTransaction With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = t.TDate, _
        '                                                               .AccNum = t.AccNum, .AccName = t.AccName, _
        '                                                               .TransactionType = t.TransactionType, .RateType = t.RateType, .SourceFunds = t.SourceFunds, .FlagReconsile = s.FlagReconcile, .Status = a.FlagReview, .BranchId = UserBranchID}
        'End If
        ExchangeTransaction = From s In db.ExchangeTransactionDetail
                              Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum
                              Group Join c In db.ExchangeTransactionClose On s.TransNum Equals c.TransNum Into Group From sc In Group.DefaultIfEmpty
                                Where s.FlagReconcile = 1 And t.Status = "ACTIVE"
                                Select New ExchangeTransaction With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = t.TDate, _
                                                                       .AccNum = t.AccNum, .AccName = t.AccName, _
                                                                       .TransactionType = t.TransactionType, .RateType = t.RateType, .SourceFunds = t.SourceFunds, .FlagReconsile = s.FlagReconcile, .Status = If(CStr(sc.TransNum) Is Nothing, t.Status, "CLOSED"), .BranchId = If(sc.CloseTransactionReason Is Nothing, "", sc.CloseTransactionReason)}

        Return ReturnExchangeTransactionDataTable(param, ExchangeTransaction, "CloseTrans")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTExchangeTransactionHistoryAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim HeadBranch = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1
        Dim UserBranchID As Decimal = Session("BranchId")
        Dim ExchangeTransaction As IQueryable(Of ExchangeTransaction)

        If UserBranchID = HeadBranch Then
            ExchangeTransaction = From s In db.ExchangeTransactionDetail
                                  Group Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum Into Group From t In Group.DefaultIfEmpty
                                Order By s.DealNumber
                                  Select New ExchangeTransaction With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = t.TDate, _
                                                                       .AccNum = t.AccNum, .AccName = t.AccName, _
                                                                       .TransactionType = t.TransactionType, .RateType = t.RateType, .SourceFunds = t.SourceFunds, .FlagReconsile = s.FlagReconcile, .Status = t.Status, .BranchId = UserBranchID}
        Else
            ExchangeTransaction = From s In db.ExchangeTransactionDetail
                                  Group Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum Into Group From t In Group.DefaultIfEmpty
                                  Where t.BranchId = UserBranchID
                                  Order By s.DealNumber
                                  Select New ExchangeTransaction With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = t.TDate, _
                                                                       .AccNum = t.AccNum, .AccName = t.AccName, _
                                                                       .TransactionType = t.TransactionType, .RateType = t.RateType, .SourceFunds = t.SourceFunds, .FlagReconsile = s.FlagReconcile, .Status = t.Status, .BranchId = UserBranchID}
        End If

        Return ReturnExchangeTransactionDataTable(param, ExchangeTransaction, "History")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTExchangeTransactionCounterAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim HeadBranch = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1
        Dim UserBranchID As Decimal = Session("BranchId")
        Dim ExchangeTransaction As IQueryable(Of ExchangeTransaction)

        If UserBranchID = HeadBranch Then
            ExchangeTransaction = From s In db.ExchangeTransactionDetail
                                  Group Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum Into st = Group
                                  Group Join u In db.ExchangeTransactionClose On s.TransNum Equals u.TransNum Into su = Group
                                  Where s.FlagReconcile = 0 And su.FirstOrDefault.DealNumber Is Nothing
                                  Select New ExchangeTransaction With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = st.FirstOrDefault.TDate, _
                                                                       .AccNum = st.FirstOrDefault.AccNum, .AccName = st.FirstOrDefault.AccName, _
                                                                       .TransactionType = st.FirstOrDefault.TransactionType, .RateType = st.FirstOrDefault.RateType, .SourceFunds = st.FirstOrDefault.SourceFunds, .Status = st.FirstOrDefault.Status, .BranchId = UserBranchID}
        Else
            ExchangeTransaction = From s In db.ExchangeTransactionDetail
                                  Group Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum Into st = Group
                                  Group Join u In db.ExchangeTransactionClose On s.TransNum Equals u.TransNum Into su = Group
                                  Where st.FirstOrDefault.BranchId = UserBranchID And s.FlagReconcile = 0 And su.FirstOrDefault.DealNumber Is Nothing
                                  Select New ExchangeTransaction With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = st.FirstOrDefault.TDate, _
                                                                       .AccNum = st.FirstOrDefault.AccNum, .AccName = st.FirstOrDefault.AccName, _
                                                                       .TransactionType = st.FirstOrDefault.TransactionType, .RateType = st.FirstOrDefault.RateType, .SourceFunds = st.FirstOrDefault.SourceFunds, .Status = st.FirstOrDefault.Status, .BranchId = UserBranchID}
        End If

        Return ReturnExchangeTransactionDataTable(param, ExchangeTransaction, "Index")
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function DTExchangeTransactionCounterApprovalAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()
        Dim HeadBranch = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1
        Dim UserBranchID As Decimal = Session("BranchId")
        Dim ExchangeTransaction As IQueryable(Of ExchangeTransaction)

        If UserBranchID = HeadBranch Then
            ExchangeTransaction = From s In db.ExchangeTransactionDetail
                                 Group Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum Into Group From t In Group.DefaultIfEmpty
                                 Where t.Status.Contains("PENDING") And t.EditBy <> User.Identity.Name And s.FlagReconcile = 0
                                 Select New ExchangeTransaction With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = t.TDate, _
                                                                      .AccNum = t.AccNum, .AccName = t.AccName, _
                                                                      .TransactionType = t.TransactionType, .RateType = t.RateType, .SourceFunds = t.SourceFunds, .Status = t.Status, .BranchId = UserBranchID}
        Else
            ExchangeTransaction = From s In db.ExchangeTransactionDetail
                                 Group Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum Into Group From t In Group.DefaultIfEmpty
                                 Where t.Status.Contains("PENDING") And t.EditBy <> User.Identity.Name And t.BranchId = UserBranchID And s.FlagReconcile = 0
                                 Select New ExchangeTransaction With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = t.TDate, _
                                                                      .AccNum = t.AccNum, .AccName = t.AccName, _
                                                                      .TransactionType = t.TransactionType, .RateType = t.RateType, .SourceFunds = t.SourceFunds, .Status = t.Status, .BranchId = UserBranchID}
        End If

        Return ReturnExchangeTransactionDataTable(param, ExchangeTransaction, "Approval")
    End Function

    Private Function ReturnExchangeTransactionDataTable(param As jQueryDataTableParamModel, ExchangeTransaction As IQueryable(Of ExchangeTransaction), mode As String) As JsonResult
        Dim totalRecords = ExchangeTransaction.Count

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim DealNumSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim TDateSearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim AccNumSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        Dim AccNameSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))
        Dim TransactionTypeSearch As String = If(IsNothing(Request("sSearch_4")), "", Request("sSearch_4"))
        Dim SourceFundsSearch As String = If(IsNothing(Request("sSearch_5")), "", Request("sSearch_5"))
        Dim ReconsileSearch As String = ""
        Dim StatusSearch As String = ""

        If mode <> "History" Then
            StatusSearch = If(IsNothing(Request("sSearch_6")), "", Request("sSearch_6"))
        Else
            ReconsileSearch = If(IsNothing(Request("sSearch_6")), "", Request("sSearch_6"))
            StatusSearch = If(IsNothing(Request("sSearch_7")), "", Request("sSearch_7"))
        End If

        ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.DealNumber.Contains(DealNumSearch))
        If TDateSearch.Trim.Length > 0 And AppHelper.checkDate(TDateSearch) Then
            Dim dTDate As Date
            dTDate = AppHelper.dateConvert(TDateSearch)
            ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.TDate.Day = dTDate.Day And f.TDate.Month = dTDate.Month And f.TDate.Year = dTDate.Year)
        End If
        ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.AccNum.Contains(AccNumSearch))
        ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.AccName.Contains(AccNameSearch))
        ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.TransactionType.Contains(TransactionTypeSearch) Or f.RateType.Contains(TransactionTypeSearch))
        ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.SourceFunds.Contains(SourceFundsSearch))

        If StatusSearch = "" Then
            If mode = "Index" Then
                ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.Status.Contains("ACTIVE") And f.Status <> "INACTIVE")
            ElseIf mode = "Approval" Then
                ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.Status.Contains("PENDING") And f.Status <> "INACTIVE")
            ElseIf mode = "Review" Then
                ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.Status = 0)
            End If
        Else
            If StatusSearch <> "ALL" Then
                ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.Status = StatusSearch)
            End If
        End If

        If mode = "History" Then
            If ReconsileSearch = "" Then
                ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.FlagReconsile = 0)
            Else
                ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.FlagReconsile = ReconsileSearch)
            End If
        End If

        'If StatusSearch <> "" Then
        '    If StatusSearch <> "ALL" Then
        '        ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.Status = StatusSearch)
        '    End If
        'Else
        '    If mode = "Index" Then
        '        ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.Status = "ACTIVE")
        '    ElseIf mode = "Approval" Then
        '        ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.Status.Contains("PENDING"))
        '    Else
        '        If StatusSearch <> "" Then
        '            ExchangeTransaction = ExchangeTransaction.Where(Function(f) f.Status = StatusSearch)
        '        End If
        '    End If
        'End If


        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    ExchangeTransaction = ExchangeTransaction.OrderBy(Function(f) f.TransNum)
                Else
                    ExchangeTransaction = ExchangeTransaction.OrderByDescending(Function(f) f.TransNum)
                End If
            Case 1
                If sortOrder = "asc" Then
                    ExchangeTransaction = ExchangeTransaction.OrderBy(Function(f) f.DealNumber)
                Else
                    ExchangeTransaction = ExchangeTransaction.OrderByDescending(Function(f) f.DealNumber)
                End If
            Case 2
                If sortOrder = "asc" Then
                    ExchangeTransaction = ExchangeTransaction.OrderBy(Function(f) f.TDate)
                Else
                    ExchangeTransaction = ExchangeTransaction.OrderByDescending(Function(f) f.TDate)
                End If
            Case 3
                If sortOrder = "asc" Then
                    ExchangeTransaction = ExchangeTransaction.OrderBy(Function(f) f.AccNum)
                Else
                    ExchangeTransaction = ExchangeTransaction.OrderByDescending(Function(f) f.AccNum)
                End If
            Case 4
                If sortOrder = "asc" Then
                    ExchangeTransaction = ExchangeTransaction.OrderBy(Function(f) f.AccName)
                Else
                    ExchangeTransaction = ExchangeTransaction.OrderByDescending(Function(f) f.AccName)
                End If
            Case 5
                If sortOrder = "asc" Then
                    ExchangeTransaction = ExchangeTransaction.OrderBy(Function(f) f.TransactionType)
                Else
                    ExchangeTransaction = ExchangeTransaction.OrderByDescending(Function(f) f.TransactionType)
                End If
            Case 6
                If sortOrder = "asc" Then
                    ExchangeTransaction = ExchangeTransaction.OrderBy(Function(f) f.SourceFunds)
                Else
                    ExchangeTransaction = ExchangeTransaction.OrderByDescending(Function(f) f.SourceFunds)
                End If
            Case 7
                If sortOrder = "asc" Then
                    ExchangeTransaction = ExchangeTransaction.OrderBy(Function(f) f.Status)
                Else
                    ExchangeTransaction = ExchangeTransaction.OrderByDescending(Function(f) f.Status)
                End If
        End Select

        'ExchangeTransaction = ExchangeTransaction.OrderBy(Function(f) f.DealNumber)

        Dim result As New List(Of String())

        If mode <> "Review" Then
            For Each data As ExchangeTransaction In ExchangeTransaction
                result.Add({data.TransNum, data.DealNumber, CDate(data.TDate).ToString("dd/MM/yyyy"), data.AccNum, data.AccName, data.TransactionType + " - " + data.RateType, data.RateType, data.SourceFunds, IIf(data.FlagReconsile = "0", "No", "Yes"), data.Status, "", data.BranchId})
            Next
        Else
            For Each data As ExchangeTransaction In ExchangeTransaction
                result.Add({data.TransNum, data.DealNumber, CDate(data.TDate).ToString("dd/MM/yyyy"), data.AccNum, data.AccName, data.TransactionType + " - " + data.RateType, data.RateType, data.SourceFunds, IIf(data.FlagReconsile = "0", "No", "Yes"), IIf(If(data.Status Is Nothing, 0, data.Status) = "0", "No", "Yes"), "", data.BranchId})
            Next
        End If

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

    <Authorize> _
    <HttpGet> _
    Public Function SearchDeal(DealNumber As String, JoinStr As String) As String
        JoinStr = Session("tmpDealNum")
        Dim db As New FectorEntities()
        Dim TransType1 As String = ""
        Dim RateType1 As String = ""
        Dim TransCurr1 As String = ""
        Dim CustomerCurr1 As String = ""
        Dim ValuePeriod1 As String = ""
        Dim ValueDate1 As String = ""

        Dim TransType2 As String = ""
        Dim RateType2 As String = ""
        Dim TransCurr2 As String = ""
        Dim CustomerCurr2 As String = ""
        Dim ValuePeriod2 As String = ""
        Dim ValueDate2 As String = ""

        Dim count As Integer = 0
        If JoinStr IsNot Nothing Then
            If JoinStr.Length > 0 Then
                count = 1
            End If
        End If

        Dim deal = (From d In db.TransactionDeal
                   Where d.DealNumber = DealNumber And d.Status = "ACTIVE").ToList

        If deal.Count > 0 Then
            If count = 0 Then
                Session("TransType1") = deal.Item(0).TransactionType
                Session("RateType1") = deal.Item(0).DealType
                Session("TransCurr1") = deal.Item(0).CurrencyDeal
                Session("CustomerCurr1") = deal.Item(0).CurrencyCustomer
                Session("ValuePeriod1") = deal.Item(0).DealPeriod
                Session("ValueDate1") = CDate(deal.Item(0).DealDate).ToString("yyyy/MM/dd")

                If JoinStr IsNot Nothing Then
                    If JoinStr.Length > 0 Then
                        JoinStr = JoinStr & ","
                    End If
                End If
                Session("tmpDealNum") = JoinStr & deal.Item(0).DealNumber
                Return JoinStr & deal.Item(0).DealNumber
            Else
                TransType1 = Session("TransType1")
                RateType1 = Session("RateType1")
                TransCurr1 = Session("TransCurr1")
                CustomerCurr1 = Session("CustomerCurr1")
                ValuePeriod1 = Session("ValuePeriod1")
                ValueDate1 = Session("ValueDate1")

                TransType2 = deal.Item(0).TransactionType
                RateType2 = deal.Item(0).DealType
                TransCurr2 = deal.Item(0).CurrencyDeal
                CustomerCurr2 = deal.Item(0).CurrencyCustomer
                ValuePeriod2 = deal.Item(0).DealPeriod
                ValueDate2 = CDate(deal.Item(0).DealDate).ToString("yyyy/MM/dd")

                If TransType1 = TransType2 And RateType1 = RateType2 And TransCurr1 = TransCurr2 And CustomerCurr1 = CustomerCurr2 And ValuePeriod1 = ValuePeriod2 And ValueDate1 = ValueDate2 Then
                    If JoinStr IsNot Nothing Then
                        If JoinStr.Length > 0 Then
                            JoinStr = JoinStr & ","
                        End If
                    End If
                    Session("tmpDealNum") = JoinStr & deal.Item(0).DealNumber
                    Return JoinStr & deal.Item(0).DealNumber
                Else
                    ViewBag.ErrorMessage = "This deal number is not same with another deal"
                End If
            End If
        End If

        ViewBag.ErrorMessage = "The deal number is not found"
        Return ""
    End Function

    <HttpPost> _
    Public Function UploadFile(model As ExchangeTransactionHead) As ActionResult
        'Dim temp As Integer = model.FileDocumentStatement.ContentLength
        'Dim File As HttpPostedFileBase = Request.Files("File")
        'If File.ContentLength > 0 Then
        '    Dim FileName As String = Path.GetFileName(File.FileName)
        '    Dim FilePath As String = Path.Combine(Server.MapPath("~/UploadResult"), FileName)
        '    File.SaveAs(FilePath)

        'End If
        Return View("Index")
    End Function

    <HttpGet> _
    <Authorize>
    Public Function ManualReconcile() As ActionResult
        InitViewBagManualReconcile()
        ViewBag.Init = "true"

        Dim model As New ManualReconcileViewModel()
        Return View(model)
    End Function

    <HttpPost>
    Public Function ManualReconcile(model As ManualReconcileViewModel) As ActionResult
        If (Not ModelState.IsValid) Then
            InitViewBagManualReconcile()
            ModelState.AddModelError("", "Something wrong")
            Return View(model)
        End If

        'Check for Deal Number availability
        Dim db As New FectorEntities()
        Dim exchangeTrxDetail = db.ExchangeTransactionDetail.SingleOrDefault(Function(m) m.DealNumber = model.DealNumber)
        If (exchangeTrxDetail Is Nothing) Then
            InitViewBagManualReconcile()
            ModelState.AddModelError("", "Deal Number not found, please make a deal first")
            Return View(model)
        End If

        exchangeTrxDetail.FlagReconcile = 1
        db.Entry(exchangeTrxDetail).State = EntityState.Modified
        db.SaveChanges()

        'Actuall database write operation
        Dim coreTrx = db.CoreTrx.SingleOrDefault(Function(m) m.Refno = model.CoreReferenceNumber)
        If (Not coreTrx Is Nothing) Then
            'Already available, upadte
            db.Entry(coreTrx).State = EntityState.Modified
            coreTrx.DealNumber = model.DealNumber
            coreTrx.ExchangeTransactionNumber = exchangeTrxDetail.TransNum
            coreTrx.Status = 1

            db.SaveChanges()
        Else
            'Not available, insert
            Dim reconcile = New CoreTransaction()
            reconcile.Refno = model.CoreReferenceNumber
            reconcile.TDate = model.TDate
            reconcile.AccNo = model.AccountNumber
            reconcile.CoreCurrId = model.Currency
            reconcile.BranchId = model.BranchId
            reconcile.Amount = model.Amount
            reconcile.Time = model.Time.Replace(":", "")
            reconcile.ExchangeTransactionNumber = exchangeTrxDetail.TransNum
            reconcile.DealNumber = model.DealNumber
            reconcile.Status = 1

            db.CoreTrx.Add(reconcile)
            db.SaveChanges()
        End If

        Return RedirectToAction("ManualReconcile")
    End Function

    Sub InitViewBagManualReconcile()
        Dim db As New FectorEntities
        ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
        ViewBag.Currencies = New SelectList(db.Currencies, "CoreCurrId", "CurrencyDisplay")
    End Sub

    ' GET: /ExchangeTransaction/BrowseAccNum
    <Authorize> _
    Function BrowseAccNum() As ActionResult
        Return PartialView("_AccNumBrowser")
    End Function

    ' GET: /ExchangeTransaction/BrowseSourceAccNum
    <Authorize> _
    Function BrowseSourceAccNum() As ActionResult
        Return PartialView("_SourceAccNumBrowser")
    End Function

    ' GET: /ExchangeTransaction/BrowseDeal
    <Authorize> _
    Function BrowseDeal() As ActionResult
        Return PartialView("_DealInTransaction")
    End Function



    Private Class FormTransaction
        Public TDate As Date
        Public DealNumber As String
        Public DealCreator As String
        Public TransCreator As String
        Public RateType As String
        Public TransactionType As String
        Public AccNum As String
        Public AccName As String
        Public BaseCurrency As String
        Public CounterCurrency As String
        Public BaseNominal As Decimal
        Public BaseRate As Decimal
        Public ConvertAmount As Decimal
        Public DealApproval As String
        Public TransApproval As String
        Public ValuePeriod As String
        Public ValueDate As Date
    End Class

    <HttpPost> _
    Public Function PrintReport(ByVal id As String, ByVal dealnumber As String, model As ExchangeTransactionHead) As ActionResult
        Dim db As New FectorEntities

        Dim TransDetail = (From td In db.ExchangeTransactionDetail
                            Join th In db.ExchangeTransactionHead On td.TransNum Equals th.TransNum
                            Join cr1 In db.Currencies On td.TransactionCurrency Equals cr1.CurrId
                            Join cr2 In db.Currencies On td.CustomerCurrency Equals cr2.CurrId
                            Join cr3 In db.TransactionDeal On td.DealNumber Equals cr3.DealNumber
                            Where td.TransNum = id And td.DealNumber = dealnumber
                            Select New FormTransaction With {.TDate = th.TDate, _
                                                             .DealNumber = td.DealNumber, _
                                                             .DealCreator = cr3.EditBy, _
                                                             .TransCreator = th.EditBy, _
                                                             .RateType = th.RateType, _
                                                             .TransactionType = th.TransactionType, _
                                                             .AccNum = th.AccNum, _
                                                             .AccName = th.AccName, _
                                                             .BaseCurrency = cr1.CurrId, _
                                                             .CounterCurrency = cr2.CurrId, _
                                                             .BaseNominal = td.TransactionNominal, _
                                                             .BaseRate = td.TransactionRate, _
                                                             .ConvertAmount = td.CustomerNominal, _
                                                             .DealApproval = cr3.ApproveBy, _
                                                             .TransApproval = th.ApproveBy, _
                                                             .ValuePeriod = td.ValuePeriod, _
                                                             .ValueDate = td.ValueDate}).ToList()

        Dim rd As New ReportDocument
        rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RptFormTransaction.rpt"))
        rd.SetDataSource(TransDetail)

        Response.Buffer = False
        Response.ClearContent()
        Response.ClearHeaders()

        Try
            Dim stream As Stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat)
            stream.Seek(0, SeekOrigin.Begin)
            Return File(stream, "application/pdf", "FOREIGN-EXCHANGE-DEAL-SLIP " & id & ".pdf")
        Catch ex As Exception
            Throw
        End Try
    End Function

    Private Function CheckLimit(AccountNumber As String, AccountName As String, TransactionType As String, RateType As String, BaseCurrency As String, BaseAmount As Decimal, DealPeriod As String, Mode As String, Optional Transnum As String = "") As Boolean
        Dim db As New FectorEntities
        Dim out As New CheckNominalNPWP

        'Start Limit User Who Login and Convert it into IDR
        Dim DataUser = (From u In db.Users
                    Where u.UserId = User.Identity.Name
                    Select u).ToList

        Dim LimitCurrencyUser As String = DataUser.Item(0).TransactionLimitCurrency
        Dim LimitUser As Decimal = DataUser.Item(0).TransactionLimit

        Dim DataRate = (From e In db.ExchangeRate
                        Where e.CurrId = LimitCurrencyUser
                        Select e).ToList

        If DataRate.Count > 0 Then
            'If TransactionType = "Sell" Then
            '    If RateType = "TT" Then
            '        LimitUser = CDec(LimitUser * DataRate.Item(0).TTSellRate).ToString("N2")
            '    Else
            '        LimitUser = CDec(LimitUser * DataRate.Item(0).BNSellRate).ToString("N2")
            '    End If
            'Else
            '    If RateType = "TT" Then
            '        LimitUser = CDec(LimitUser * DataRate.Item(0).TTBuyRate).ToString("N2")
            '    Else
            '        LimitUser = CDec(LimitUser * DataRate.Item(0).BNBuyRate).ToString("N2")
            '    End If
            'End If
            LimitUser = CDec(LimitUser * DataRate.Item(0).ClosingRate).ToString("N2")
        End If
        'End Limit User Who Login and Convert it into IDR

        'Start Get Limit Threshold and Convert it into IDR
        Dim LimitCurrencyThreshold As String
        Dim LimitThreshold As Nullable(Of Decimal) = 0
        Dim Threshold = (From a In db.Settings
                         Where a.Key1 = "Transaction" And a.Key2 = "Limit"
                         Select a).ToList

        LimitCurrencyThreshold = Threshold.Item(0).Value2
        LimitThreshold = Threshold.Item(0).Value1

        DataRate = (From e In db.ExchangeRate
                     Where e.CurrId = LimitCurrencyThreshold
                     Select e).ToList

        If DataRate.Count > 0 Then
            'If TransactionType = "Sell" Then
            '    If RateType = "TT" Then
            '        LimitThreshold = CDec(LimitThreshold * DataRate.Item(0).TTSellRate).ToString("N2")
            '    Else
            '        LimitThreshold = CDec(LimitThreshold * DataRate.Item(0).BNSellRate).ToString("N2")
            '    End If
            'Else
            '    If RateType = "TT" Then
            '        LimitThreshold = CDec(LimitThreshold * DataRate.Item(0).TTBuyRate).ToString("N2")
            '    Else
            '        LimitThreshold = CDec(LimitThreshold * DataRate.Item(0).BNBuyRate).ToString("N2")
            '    End If
            'End If
            LimitThreshold = CDec(LimitThreshold * DataRate.Item(0).ClosingRate).ToString("N2")
        End If
        'End Get Limit Threshold and Convert it into IDR

        'Start SUM Deal Transaction Today (IDR) with That Customer, but status not in 'REJECTED', 'INACTIVE'
        Dim SUMTransactionCustomerToday As Nullable(Of Decimal) = 0

        If Mode = "CREATE" Then
            SUMTransactionCustomerToday = Aggregate s In db.ExchangeTransactionHead
                                         Join t In db.ExchangeTransactionDetail On s.TransNum Equals t.TransNum
                                         Where t.FlagReconcile = 1 And (s.Status <> "REJECTED" Or s.Status <> "CLOSED") _
                                         And s.EditBy = User.Identity.Name _
                                         And s.EditDate.Year = Now.Year And s.EditDate.Month = Now.Month _
                                         Into Sum(t.TransactionRate * t.TransactionNominal)
        ElseIf Mode = "EDIT" Then
            SUMTransactionCustomerToday = Aggregate s In db.ExchangeTransactionHead
                                         Join t In db.ExchangeTransactionDetail On s.TransNum Equals t.TransNum
                                         Where t.FlagReconcile = 1 And (s.Status <> "REJECTED" Or s.Status <> "CLOSED") _
                                         And s.EditBy = User.Identity.Name _
                                         And s.EditDate.Year = Now.Year And s.EditDate.Month = Now.Month _
                                         And s.TransNum <> Transnum
                                         Into Sum(t.TransactionRate * t.TransactionNominal)
        End If

        If SUMTransactionCustomerToday Is Nothing Then
            SUMTransactionCustomerToday = 0
        End If
        'End SUM Deal Transaction Today (IDR) with That Customer, but status not in 'REJECTED', 'INACTIVE', 'CLOSED'

        'Start Calculate Transaction Amount Which Customer Used
        DataRate = (From e In db.ExchangeRate
                    Where e.CurrId = BaseCurrency
                    Select e).ToList

        If DataRate.Count > 0 Then
            'If TransactionType = "Sell" Then
            '    If RateType = "TT" Then
            '        BaseAmount = CDec(BaseAmount * DataRate.Item(0).TTSellRate).ToString("N2")
            '    Else
            '        BaseAmount = CDec(BaseAmount * DataRate.Item(0).BNSellRate).ToString("N2")
            '    End If
            'Else
            '    If RateType = "TT" Then
            '        BaseAmount = CDec(BaseAmount * DataRate.Item(0).TTBuyRate).ToString("N2")
            '    Else
            '        BaseAmount = CDec(BaseAmount * DataRate.Item(0).BNBuyRate).ToString("N2")
            '    End If
            'End If
            BaseAmount = CDec(BaseAmount * DataRate.Item(0).ClosingRate).ToString("N2")
        End If
        'End Calculate Transaction Amount Which Customer Used

        'Start Check Account has NPWP or not
        Dim cif As Decimal = db.Accounts.Where(Function(f) f.AccNo = AccountNumber).FirstOrDefault.CIF

        Dim npwpCount = db.Accounts.Where(Function(f) f.CIF = cif And f.NPWP.Length > 0).Count
        'End Check Account has NPWP or not

        If SUMTransactionCustomerToday + BaseAmount > LimitThreshold And LimitThreshold <> 0 Then
            Return False
        End If
        If BaseAmount > LimitUser And LimitUser <> 0 Then
            Return False
        End If

        Return True
    End Function

    <HttpPost> _
    Public Function JsOnCheckDealNumber(ByVal dealnum As String, ByVal model As ExchangeTransactionViewModel) As ActionResult
        Dim dealnumber As String = ""
        Dim Action As String = Request.UrlReferrer.AbsolutePath
        Dim db As New FectorEntities

        If Not IsNothing(dealnum) Then
            dealnumber = dealnum
        End If

        Dim deal As New dealInfo

        Dim dealUsedInTrans = (Aggregate ed In db.ExchangeTransactionDetail
                                Where ed.DealNumber = dealnum
                                Into Sum(ed.TransactionNominal))

        If dealUsedInTrans Is Nothing Then
            dealUsedInTrans = 0
        End If

        deal = (From de In db.TransactionDeal
                Where de.DealNumber.StartsWith(dealnum) And de.DealDate.Year = Now.Year And (de.Status = "ACTIVE" Or de.Status = "UNFINISHED")
                Select New dealInfo With {.dealNumber = de.DealNumber, .accNumber = de.AccNum, _
                                        .accName = de.AccName, .tranType = de.TransactionType, .dealType = de.DealType, _
                                        .fromCurrency = de.CurrencyDeal, .toCurrency = de.CurrencyCustomer, _
                                        .exchRate = de.DealRate, .dealAmount = de.AmountDeal, _
                                        .valueDate = de.DealDate, .valuePeriod = de.DealPeriod, .counterAmount = de.AmountCustomer}).FirstOrDefault

        If Action.Contains("Edit") Then
            Dim transdetail = (From de In db.ExchangeTransactionDetail
                              Join he In db.ExchangeTransactionHead On de.TransNum Equals he.TransNum
                              Where de.TransNum = Request.UrlReferrer.Query.Replace("?id=", "")
                              Select New dealInfo With {.dealNumber = de.DealNumber, .accNumber = he.AccNum, _
                                                .accName = he.AccName, .tranType = he.TransactionType, .dealType = he.RateType, _
                                                .fromCurrency = de.TransactionCurrency, .toCurrency = de.CustomerCurrency, _
                                                .exchRate = de.TransactionRate, .dealAmount = de.TransactionNominal, _
                                                .valueDate = de.ValueDate, .valuePeriod = de.ValuePeriod, .counterAmount = de.CustomerNominal}).ToList

            If transdetail.Count <> 0 Then
                For i As Integer = 0 To transdetail.Count - 1
                    If dealnum = transdetail(i).dealNumber Then
                        transdetail(i).strvalueDate = transdetail(i).valueDate.ToString("dd-MM-yyyy")
                        transdetail(i).strexchRate = CDec(transdetail(i).exchRate).ToString("N2")
                        transdetail(i).strdealAmount = CDec(transdetail(i).dealAmount).ToString("N2")
                        transdetail(i).strcounterAmount = CDec(transdetail(i).counterAmount).ToString("N2")

                        Session("dealNumber") = transdetail(i).dealNumber
                        Session("accNumber") = transdetail(i).accNumber
                        Session("accName") = transdetail(i).accName
                        Session("transType") = transdetail(i).tranType
                        Session("dealType") = transdetail(i).dealType
                        Session("fromCurrency") = transdetail(i).fromCurrency
                        Session("toCurrency") = transdetail(i).toCurrency
                        Session("valueDate") = transdetail(i).valueDate.Date

                        Return Json(transdetail(i))
                    End If
                Next

                If IsNothing(deal) Then
                    Return Json(New With {.errMessage = "Deal not found"})
                Else
                    If deal.valueDate.Date < Now.Date Then
                        Return Json(New With {.errMessage = "Deal already expired"})
                    End If

                    If Session("dealNumber") <> "" Then
                        If deal.accNumber <> Session("accNumber") Then
                            Return Json(New With {.errMessage = "This deal has different account number with previously added deal"})
                        ElseIf deal.accName <> Session("accName") Then
                            Return Json(New With {.errMessage = "This deal has different account name with previously added deal"})
                        ElseIf deal.tranType <> Session("transType") Then
                            Return Json(New With {.errMessage = "This deal has different transaction type with previously added deal"})
                        ElseIf deal.dealType <> Session("dealType") Then
                            Return Json(New With {.errMessage = "This deal has different deal type with previously added deal"})
                        ElseIf deal.fromCurrency <> Session("fromCurrency") Or deal.toCurrency <> Session("toCurrency") Then
                            Return Json(New With {.errMessage = "This deal has different currency with previously added deal"})
                        ElseIf deal.valueDate.Date <> Session("valueDate") Then
                            Return Json(New With {.errMessage = "This deal has different value date with previously added deal"})
                        End If
                    Else
                        Session("dealNumber") = deal.dealNumber
                        Session("accNumber") = deal.accNumber
                        Session("accName") = deal.accName
                        Session("transType") = deal.tranType
                        Session("dealType") = deal.dealType
                        Session("fromCurrency") = deal.fromCurrency
                        Session("toCurrency") = deal.toCurrency
                        Session("valueDate") = deal.valueDate.Date
                    End If
                End If
            End If

        Else
            If IsNothing(deal) Then
                Return Json(New With {.errMessage = "Deal not found"})
            Else
                If deal.valueDate.Date < Now.Date Then
                    Return Json(New With {.errMessage = "Deal already expired"})
                End If

                If Session("dealNumber") <> "" Then
                    If deal.accNumber <> Session("accNumber") Then
                        Return Json(New With {.errMessage = "This deal has different account number with previously added deal"})
                    ElseIf deal.accName <> Session("accName") Then
                        Return Json(New With {.errMessage = "This deal has different account name with previously added deal"})
                    ElseIf deal.tranType <> Session("transType") Then
                        Return Json(New With {.errMessage = "This deal has different transaction type with previously added deal"})
                    ElseIf deal.dealType <> Session("dealType") Then
                        Return Json(New With {.errMessage = "This deal has different deal type with previously added deal"})
                    ElseIf deal.fromCurrency <> Session("fromCurrency") Or deal.toCurrency <> Session("toCurrency") Then
                        Return Json(New With {.errMessage = "This deal has different currency with previously added deal"})
                    ElseIf deal.valueDate.Date <> Session("valueDate") Then
                        Return Json(New With {.errMessage = "This deal has different value date with previously added deal"})
                    End If
                Else
                    Session("dealNumber") = deal.dealNumber
                    Session("accNumber") = deal.accNumber
                    Session("accName") = deal.accName
                    Session("transType") = deal.tranType
                    Session("dealType") = deal.dealType
                    Session("fromCurrency") = deal.fromCurrency
                    Session("toCurrency") = deal.toCurrency
                    Session("valueDate") = deal.valueDate.Date
                End If
            End If
        End If

        deal.strvalueDate = deal.valueDate.ToString("dd-MM-yyyy")
        deal.strexchRate = CDec(deal.exchRate).ToString("N2")
        deal.strdealAmount = CDec(deal.dealAmount - dealUsedInTrans).ToString("N2")
        deal.strcounterAmount = CDec(deal.counterAmount).ToString("N2")

        Return Json(deal)
    End Function



    Class StatementOutput
        Public Property UnderLimit As Boolean = False
        Public Property IsOtherAccount As Boolean = False
        Public Property HaveStatementUnderLimit As Boolean = False
        Public Property HaveStatementOverLimit As Boolean = False
        Public Property StatementUnderLimitName As String = ""
        Public Property StatementOverLimitName As String = ""
        Public Property StatementUnderLimitToSave As String = ""
        Public Property StatementOverLimitToSave As String = ""
        Public Property IsExcludeLHBU As Boolean = False
    End Class

    '
    ' Post:  /ExchangeTransaction/JsOnCheckDocStatement
    Public Function JsOnCheckDocStatement(accno As String, convertedamount As Nullable(Of Decimal), basecurrency As String, trantype As String, ratetype As String, modeTransnum As String, Optional edited As Boolean = False)
        Dim db As New FectorEntities
        Dim out As New StatementOutput
        Dim AccNum As String = accno

        Dim TransLimit As Decimal = CDec(MsSetting.GetSetting("Transaction", "Limit", "", 1))
        Dim TransLimitCurr As String = MsSetting.GetSetting("Transaction", "Limit", "", 2)

        Dim tmp = modeTransnum.Split(",")

        Dim mode As String = tmp(0)
        Dim transnum As String = tmp(1)

        If mode <> "ReviewDoc" Then
            '#EDITED
            'CIF GET FROM EXCHANGETRANSACTIONHEAD FOR MODE ReviewDoc, Edit, And 
            Dim CIF As Decimal
            Dim isExcludeLHBU As Boolean = db.Accounts.Where(Function(f) f.AccNo = AccNum).FirstOrDefault.flagNonLHBU

            Dim Usage As Decimal = 0
            If edited Then
                CIF = db.Accounts.FirstOrDefault(Function(f) f.AccNo = AccNum).CIF
            Else
                CIF = db.ExchangeTransactionHead.FirstOrDefault(Function(f) f.TransNum = transnum).CIF
            End If


            If mode = "Edit" Then
                Dim data = From ac In db.Accounts
                            Join th In db.ExchangeTransactionHead On ac.AccNo Equals th.AccNum
                            Join td In db.ExchangeTransactionDetail On th.TransNum Equals td.TransNum
                            Join ex In db.ExchangeRate On td.TransactionCurrency Equals ex.CurrId
                            Where ac.CIF = CIF And td.ValueDate.Value.Month = Now.Month And td.ValueDate.Value.Year = Now.Year And th.Status = "ACTIVE" And th.TransNum <> transnum
                            Select New With {.TransactionNominal = td.TransactionNominal, .ClosingRate = ex.ClosingRate}

                If data.Count > 0 Then
                    Usage = (From a In data
                            Select a.TransactionNominal * a.ClosingRate).Sum()
                End If
            Else
                Dim data = From ac In db.Accounts
                            Join th In db.ExchangeTransactionHead On ac.AccNo Equals th.AccNum
                            Join td In db.ExchangeTransactionDetail On th.TransNum Equals td.TransNum
                            Join ex In db.ExchangeRate On td.TransactionCurrency Equals ex.CurrId
                            Where ac.CIF = CIF And td.ValueDate.Value.Month = Now.Month And td.ValueDate.Value.Year = Now.Year And th.Status = "ACTIVE"
                            Select New With {.TransactionNominal = td.TransactionNominal, .ClosingRate = ex.ClosingRate}

                If data.Count > 0 Then
                    Usage = (From a In data
                            Select a.TransactionNominal * a.ClosingRate).Sum()
                End If
            End If

            Dim CurrentUsage = (From ex In db.ExchangeRate
                                Where ex.CurrId = basecurrency
                                Select convertedamount * CDec(ex.ClosingRate)).Sum()

            If IsNothing(Usage) Then
                Usage = 0
            End If
            Usage += CurrentUsage

            TransLimit = TransLimit * CDec(db.ExchangeRate.Where(Function(f) f.CurrId = TransLimitCurr).Select(Function(f) f.ClosingRate).FirstOrDefault)

            If CDec(Usage) <= CDec(TransLimit) Then
                out.UnderLimit = True
            Else
                out.UnderLimit = False
            End If

            Dim lastTran = (From a In db.ExchangeTransactionHead
                            Join c In db.ExchangeTransactionDetail On a.TransNum Equals c.TransNum
                            Join b In db.Accounts On a.AccNum Equals b.AccNo
                            Order By c.ValueDate, c.TransNum Descending
                            Where c.ValueDate.Value.Month = Now.Month And c.ValueDate.Value.Year = Now.Year And b.CIF = CIF And a.Status = "ACTIVE"
                            Select New With {.statementunderlimit = a.DocumentStatementUnderlimitLink, .statementoverlimit = a.DocumentStatementOverlimitLink}).FirstOrDefault

            If IsNothing(lastTran) Then
                out.HaveStatementUnderLimit = False
                out.HaveStatementOverLimit = False
            Else
                If lastTran.statementunderlimit.Length > 0 Then
                    out.HaveStatementUnderLimit = True
                    out.StatementUnderLimitToSave = lastTran.statementunderlimit
                    out.StatementUnderLimitName = UrlHelper.GenerateContentUrl(lastTran.statementunderlimit, Me.HttpContext)
                Else
                    out.HaveStatementUnderLimit = False
                End If

                If lastTran.statementoverlimit.Length > 0 Then
                    out.HaveStatementOverLimit = True
                    out.StatementOverLimitToSave = lastTran.statementoverlimit
                    out.StatementOverLimitName = UrlHelper.GenerateContentUrl(lastTran.statementoverlimit, Me.HttpContext)
                Else
                    out.HaveStatementOverLimit = False
                End If
            End If

            If isExcludeLHBU Then
                out.IsExcludeLHBU = True
            Else
                out.IsExcludeLHBU = False
            End If
        Else
            Dim account = db.Accounts.Where(Function(f) f.AccNo = AccNum).FirstOrDefault
            Dim isExcludeLHBU As Boolean = If(account.flagNonLHBU IsNot Nothing, account.flagNonLHBU, False)

            Dim DocStatement = (From a In db.ExchangeTransactionHead
                               Where a.TransNum = transnum
                               Select a).FirstOrDefault

            If DocStatement.DocumentStatementUnderlimitLink IsNot Nothing And DocStatement.DocumentStatementUnderlimitLink <> "" Then
                out.UnderLimit = True
                out.HaveStatementUnderLimit = True

                out.StatementUnderLimitToSave = DocStatement.DocumentStatementUnderlimitLink
                out.StatementUnderLimitName = UrlHelper.GenerateContentUrl(DocStatement.DocumentStatementUnderlimitLink, Me.HttpContext)
            ElseIf DocStatement.DocumentStatementOverlimitLink IsNot Nothing And DocStatement.DocumentStatementOverlimitLink <> "" Then
                out.UnderLimit = False
                out.HaveStatementOverLimit = True

                out.StatementOverLimitToSave = DocStatement.DocumentStatementOverlimitLink
                out.StatementOverLimitName = UrlHelper.GenerateContentUrl(DocStatement.DocumentStatementOverlimitLink, Me.HttpContext)
            Else
                Dim CIF As Decimal = db.ExchangeTransactionHead.FirstOrDefault(Function(f) f.AccNum = AccNum).CIF
                Dim usage As Decimal = 0

                Dim data = From ac In db.Accounts
                                Join th In db.ExchangeTransactionHead On ac.AccNo Equals th.AccNum
                                Join td In db.ExchangeTransactionDetail On th.TransNum Equals td.TransNum
                                Join ex In db.ExchangeRate On td.TransactionCurrency Equals ex.CurrId
                                Where ac.CIF = CIF And td.ValueDate.Value.Month = Now.Month And td.ValueDate.Value.Year = Now.Year And th.TransNum <> transnum And th.Status = "ACTIVE"
                                Select New With {.TransactionNominal = td.TransactionNominal, .ClosingRate = ex.ClosingRate}

                If data.Count > 0 Then
                    usage = (From a In data
                            Select a.TransactionNominal * a.ClosingRate).Sum()
                End If

                TransLimit = TransLimit * CDec(db.ExchangeRate.Where(Function(f) f.CurrId = TransLimitCurr).Select(Function(f) f.ClosingRate).FirstOrDefault)

                If CDec(usage) <= CDec(TransLimit) Then
                    out.UnderLimit = True
                Else
                    out.UnderLimit = False
                End If

                out.HaveStatementUnderLimit = False
                out.HaveStatementOverLimit = False
            End If

            If isExcludeLHBU Then
                out.IsExcludeLHBU = True
            Else
                out.IsExcludeLHBU = False
            End If
        End If


        Dim OtherAcc = db.OtherAccounts.Where(Function(f) f.AccNo = AccNum And f.Status = "ACTIVE").Count
        If OtherAcc > 0 Then
            out.IsOtherAccount = True
        Else
            out.IsOtherAccount = False
        End If

        Return Json(out)
    End Function

    '
    ' Post:  /ExchangeTransaction/JsOnCheckDocumentRequirement
    Public Function JsOnCheckDocumentRequirement(accno As String, convertedamount As Nullable(Of Decimal)) As ActionResult
        Dim Action As String = Request.UrlReferrer.AbsolutePath
        Dim out As New DocumentReqOutput

        If Not Action.Contains("Viewed") Then
            Dim db As New FectorEntities
            Dim accnum As String = accno

            Dim Trans = (From a In db.ExchangeTransactionHead
                        Where a.AccNum = accnum
                        Order By a.TDate Descending
                        Select a).FirstOrDefault

            If Trans IsNot Nothing Then
                Dim Doc = (From a In db.ExchangeTransactionDoc
                        Where a.TransNum = Trans.TransNum
                        Order By a.ID Descending
                        Select a).FirstOrDefault

                Dim TransDetail = (From a In db.ExchangeTransactionDetail
                                  Where a.TransNum = Trans.TransNum
                                  Select a).ToList

                If Doc IsNot Nothing And TransDetail IsNot Nothing Then
                    Dim SUMTransDetail As Decimal = 0
                    If TransDetail IsNot Nothing Then
                        For i As Integer = 0 To TransDetail.Count - 1
                            SUMTransDetail += TransDetail(i).TransactionNominal
                        Next
                    End If

                    Dim DocNominalDetail = (From a In db.ExchangeTransactionDoc
                                           Where a.TransNum = Trans.TransNum
                                           Select a).ToList

                    Dim SUMDocNominal As Decimal = 0
                    If DocNominalDetail IsNot Nothing Then
                        For i As Integer = 0 To DocNominalDetail.Count - 1
                            SUMDocNominal += DocNominalDetail(i).NominalDoc
                        Next
                    End If

                    If SUMTransDetail + convertedamount > SUMDocNominal Then
                        out.PassNominalUnderlying = True
                        out.DocTransLinkName = Nothing
                        out.DocTransLinkSave = Nothing
                    Else
                        out.PassNominalUnderlying = False

                        If Doc.DocumentTransactionLink IsNot Nothing Then
                            out.DocTransLinkName = Doc.DocumentTransactionLink
                            out.DocTransLinkSave = UrlHelper.GenerateContentUrl(Doc.DocumentTransactionLink, Me.HttpContext)
                        End If

                    End If
                End If
            End If
        End If
        Return Json(out)
    End Function

    '
    ' Get:  /ExchangeTransaction/JsOnGetLHBUDocument
    <HttpPost> _
    Public Function JsOnGetLHBUDocument(DocTrans As String) As ActionResult
        Dim db As New FectorEntities
        Dim listLHBUDoc As List(Of MsDocumentLHBU)

        listLHBUDoc = (From dt In db.MappingDocument
                      Join dl In db.DocumentLHBU On dt.DocumentLHBUId Equals dl.DocumentId
                      Where dt.DocumentTransId = DocTrans
                      Select dl).ToList

        Return Json(listLHBUDoc)
    End Function

    '
    ' Get:  /ExchangeTransaction/JsOnGetLHBUPurpose
    <HttpPost> _
    Public Function JsOnGetLHBUPurpose(DocTrans As String) As ActionResult
        Dim db As New FectorEntities
        Dim listLHBUPur As List(Of MsPurpose)

        If DocTrans = "" Then
            listLHBUPur = (From dt In db.Purposes
                              Where dt.Status = "ACTIVE"
                              Select dt).ToList
        Else
            listLHBUPur = (From dt In db.MappingDocumentPurpose
                              Join pu In db.Purposes On dt.PurposeLHBUId Equals pu.PurposeId
                              Where dt.DocumentTransId = DocTrans
                              Select pu).ToList
        End If


        Return Json(listLHBUPur)
    End Function

    '
    ' Get:  /ExchangeTransaction/JsOnFillAllLHBU
    <HttpPost> _
    Public Function JsOnFillAllLHBU() As ActionResult
        Dim db As New FectorEntities
        Dim listLHBUDoc As List(Of MsDocumentLHBU)

        listLHBUDoc = (From a In db.DocumentLHBU
                       Where a.Status = "ACTIVE"
                      Select a).ToList

        Return Json(listLHBUDoc)
    End Function

    '
    ' Get:  /ExchangeTransaction/JsOnFillAllPurpose
    <HttpPost> _
    Public Function JsOnFillAllPurpose() As ActionResult
        Dim db As New FectorEntities
        Dim listLHBUPur As List(Of MsPurpose)

        listLHBUPur = (From dt In db.Purposes
                        Where dt.Status = "ACTIVE"
                        Select dt).ToList


        Return Json(listLHBUPur)
    End Function

    '
    ' Post:  /ExchangeTransaction/JsOnGetRate
    <HttpPost> _
    Public Function JsOnGetRate(tranType As String, rateType As String, fromCurr As String, toCurr As String) As ActionResult
        Dim db As New FectorEntities
        Dim exchangeRate As Decimal = 0
        Dim baserate = (From ex In db.ExchangeRate
                       Where ex.CurrId = fromCurr
                       Select New With {.BNBuyRate = ex.BNBuyRate, .BNSellRate = ex.BNSellRate, _
                                       .TTSellRate = ex.TTSellRate, .TTBuyRate = ex.TTBuyRate}).FirstOrDefault

        If IsNothing(baserate) Then
            Return Json(New With {.exchangeRate = exchangeRate})
        End If

        Dim counterrate = (From ex In db.ExchangeRate
                           Where ex.CurrId = toCurr
                           Select New With {.BNBuyRate = ex.BNBuyRate, .BNSellRate = ex.BNSellRate, _
                                       .TTSellRate = ex.TTSellRate, .TTBuyRate = ex.TTBuyRate}).FirstOrDefault

        If IsNothing(baserate) Then
            Return Json(New With {.exchangeRate = exchangeRate})
        End If

        If tranType = "Sell" Then
            If rateType = "TT" Then
                exchangeRate = CDec(baserate.TTSellRate / counterrate.TTBuyRate).ToString("N2")
            Else
                exchangeRate = CDec(baserate.BNSellRate / counterrate.BNBuyRate).ToString("N2")
            End If
        Else
            If rateType = "TT" Then
                exchangeRate = CDec(baserate.TTBuyRate / counterrate.TTSellRate).ToString("N2")
            Else
                exchangeRate = CDec(baserate.BNBuyRate / counterrate.BNSellRate).ToString("N2")
            End If
        End If

        Return Json(New With {.exchangeRate = exchangeRate})
    End Function

    '
    ' Post:  /ExchangeTransaction/JsOnGetValueDate
    <HttpPost> _
    Public Function JsOnGetValueDate(valuetype As String, basecurrency As String) As ActionResult
        Dim db As New FectorEntities
        Dim exchangeRate As Decimal = 0
        Dim valDate As Date

        If valuetype = "TOD" Then
            Dim tempDate As DateTime = Now
            valDate = getWorkingDate(tempDate, basecurrency)
        ElseIf valuetype = "TOM" Then
            Dim tempDate As DateTime = Now.AddDays(1)
            valDate = getWorkingDate(tempDate, basecurrency)
        ElseIf valuetype = "SPOT" Then
            Dim tempDate As DateTime = Now.AddDays(2)
            valDate = getWorkingDate(tempDate, basecurrency, "SPOT")
        End If

        Return Json(New With {.valueDate = valDate.ToString("dd-MM-yyyy")})
    End Function

    '
    ' Post:  /ExchangeTransaction/JsOnFirstLoadEdit
    Public Function JsOnFirstLoadEdit(TransNum As String) As ActionResult
        Dim Action As String = Request.UrlReferrer.AbsolutePath
        Dim db As New FectorEntities

        Dim Head = (From h In db.ExchangeTransactionHead
                    Where h.TransNum = TransNum
                    Select h).FirstOrDefault

        Dim TransHead = (From a In db.ExchangeTransactionHead
                        Where a.AccNum = Head.AccNum And a.SourceNominal <> 0 And a.TDate.Month = Now.Month And a.TDate.Year = Now.Year
                        Order By a.TDate Descending
                        Select a).FirstOrDefault

        Dim TransDetail = (From a In db.ExchangeTransactionDetail
                           Where a.TransNum = TransNum
                           Select a).ToList

        Dim NominalUnderlying As Decimal = 0
        Dim SUMTransactionNominal As Decimal = 0
        Dim De As List(Of TransDetail)

        If TransHead IsNot Nothing Then
            NominalUnderlying = TransHead.SourceNominal

            If Action.Contains("Edit") Then
                'a.DocumentTransactionLink = TransHead.DocumentTransactionLink And
                De = (From a In db.ExchangeTransactionHead
                    Join b In db.ExchangeTransactionDetail On a.TransNum Equals b.TransNum
                    Where a.AccNum = Head.AccNum And a.SourceNominal IsNot Nothing And a.TDate.Month = Now.Month And a.TDate.Year = Now.Year And a.TransNum <> Request.UrlReferrer.Query.Replace("?id=", "")
                    Select New TransDetail With {.TransNum = b.TransNum, .DealNumber = b.DealNumber, .TransactionCurrency = b.TransactionCurrency, _
                                                .TransactionRate = b.TransactionRate, .TransactionNominal = b.TransactionNominal, .CustomerCurrency = b.CustomerCurrency, _
                                                .CustomerNominal = b.CustomerNominal, .ValuePeriod = b.ValuePeriod, .ValueDate = b.ValueDate, .FlagReconsile = b.FlagReconcile}).ToList
            Else
                'And a.DocumentTransactionLink = TransHead.DocumentTransactionLink
                De = (From a In db.ExchangeTransactionHead
                    Join b In db.ExchangeTransactionDetail On a.TransNum Equals b.TransNum
                    Where a.AccNum = Head.AccNum And a.SourceNominal IsNot Nothing And a.TDate.Month = Now.Month And a.TDate.Year = Now.Year
                    Select New TransDetail With {.TransNum = b.TransNum, .DealNumber = b.DealNumber, .TransactionCurrency = b.TransactionCurrency, _
                                                .TransactionRate = b.TransactionRate, .TransactionNominal = b.TransactionNominal, .CustomerCurrency = b.CustomerCurrency, _
                                                .CustomerNominal = b.CustomerNominal, .ValuePeriod = b.ValuePeriod, .ValueDate = b.ValueDate, .FlagReconsile = b.FlagReconcile}).ToList
            End If

            For i As Integer = 0 To De.Count - 1
                SUMTransactionNominal += De.Item(i).TransactionNominal
            Next
        End If

        Dim out As New EditDocumentReqOutput

        'If Head.DocumentTransId IsNot Nothing Then
        '    out.UnderLimit = False
        'Else
        '    out.UnderLimit = True
        'End If

        'If Head.DocumentTransactionLink <> "" Then
        '    out.HaveDocTransLink = True
        '    out.DocTransLinkName = Head.DocumentTransactionLink
        '    out.DocTransLinkSave = UrlHelper.GenerateContentUrl(Head.DocumentTransactionLink, Me.HttpContext)

        '    out.NominalUnderlying = NominalUnderlying
        '    out.RemainingUnderlying = NominalUnderlying - (SUMTransactionNominal + TransDetail.Item(0).TransactionNominal)
        'Else
        '    out.HaveDocTransLink = False
        'End If

        If Head.DocumentStatementUnderlimitLink <> "" Then
            out.HaveDocStatementUnderlimitLink = True
            out.DocStatementUnderlimitLinkName = Head.DocumentStatementUnderlimitLink
            out.DocStatementUnderlimitLinkSave = UrlHelper.GenerateContentUrl(Head.DocumentStatementUnderlimitLink, Me.HttpContext)
        Else
            out.HaveDocStatementUnderlimitLink = False
        End If

        If Head.DocumentStatementOverlimitLink <> "" Then
            out.HaveDocStatementOverlimitLink = True
            out.DocStatementOverlimitLinkName = Head.DocumentStatementOverlimitLink
            out.DocStatementOverlimitLinkSave = UrlHelper.GenerateContentUrl(Head.DocumentStatementOverlimitLink, Me.HttpContext)
        Else
            out.HaveDocStatementOverlimitLink = False
        End If

        'If Head.TransFormulirLink <> "" Then
        '    out.HaveTransFormulirLink = True
        '    out.TransFormulirLinkName = Head.TransFormulirLink
        '    out.TransFormulirLinkSave = UrlHelper.GenerateContentUrl(Head.TransFormulirLink, Me.HttpContext)
        'Else
        '    out.HaveTransFormulirLink = False
        'End If

        Dim otheracc = db.OtherAccounts.Where(Function(f) f.AccNo = Head.AccNum).Count
        If otheracc = 0 Then
            out.IsOtherAccount = False
        Else
            out.IsOtherAccount = True
        End If

        Return Json(out)
    End Function

    Class CheckNominalNPWP
        Public Property UnderLimitThreshold As Boolean = True
        Public Property UnderLimitUser As Boolean = True
        Public Property HaveNPWP As Boolean
    End Class

    '
    ' Post:  /ExchangeTransaction/JsOnGetRate
    <HttpPost> _
    Public Function JsOnCheckLimit(AccountNumber As String, AccountName As String, TransactionType As String, RateType As String, BaseCurrency As String, BaseAmount As Decimal, DealPeriod As String, Mode As String, Optional Transnum As String = "") As ActionResult
        Dim db As New FectorEntities
        Dim out As New CheckNominalNPWP

        'Start Limit User Who Login and Convert it into IDR
        Dim DataUser = (From u In db.Users
                    Where u.UserId = User.Identity.Name
                    Select u).ToList

        Dim LimitCurrencyUser As String = DataUser.Item(0).TransactionLimitCurrency
        Dim LimitUser As Decimal = DataUser.Item(0).TransactionLimit

        Dim DataRate = (From e In db.ExchangeRate
                        Where e.CurrId = LimitCurrencyUser
                        Select e).ToList

        If DataRate.Count > 0 Then
            'If TransactionType = "Sell" Then
            '    If RateType = "TT" Then
            '        LimitUser = CDec(LimitUser * DataRate.Item(0).TTSellRate).ToString("N2")
            '    Else
            '        LimitUser = CDec(LimitUser * DataRate.Item(0).BNSellRate).ToString("N2")
            '    End If
            'Else
            '    If RateType = "TT" Then
            '        LimitUser = CDec(LimitUser * DataRate.Item(0).TTBuyRate).ToString("N2")
            '    Else
            '        LimitUser = CDec(LimitUser * DataRate.Item(0).BNBuyRate).ToString("N2")
            '    End If
            'End If
            LimitUser = CDec(LimitUser * DataRate.Item(0).ClosingRate).ToString("N2")
        End If
        'End Limit User Who Login and Convert it into IDR

        'Start SUM Deal Transaction Today (IDR) with That User, but status not in 'REJECTED', 'INACTIVE'
        Dim SUMTransactionUserToday As Nullable(Of Decimal) = 0

        If Mode = "CREATE" Then
            SUMTransactionUserToday = Aggregate s In db.ExchangeTransactionHead
                                        Join t In db.ExchangeTransactionDetail On s.TransNum Equals t.TransNum
                                        Join u In db.ExchangeRate On t.TransactionCurrency Equals u.CurrId
                                        Where t.FlagReconcile = 1 And (s.Status <> "REJECTED" And s.Status <> "CLOSED") _
                                        And s.EditBy = User.Identity.Name _
                                        And s.EditDate.Year = Now.Year And s.EditDate.Month = Now.Month _
                                        Into Sum(u.ClosingRate * t.TransactionNominal)
        ElseIf Mode = "EDIT" Then
            SUMTransactionUserToday = Aggregate s In db.ExchangeTransactionHead
                                         Join t In db.ExchangeTransactionDetail On s.TransNum Equals t.TransNum
                                         Join u In db.ExchangeRate On t.TransactionCurrency Equals u.CurrId
                                         Where t.FlagReconcile = 1 And (s.Status <> "REJECTED" And s.Status <> "CLOSED") _
                                         And s.EditBy = User.Identity.Name _
                                         And s.EditDate.Year = Now.Year And s.EditDate.Month = Now.Month _
                                         And s.TransNum <> Transnum
                                         Into Sum(u.ClosingRate * t.TransactionNominal)
        End If

        If SUMTransactionUserToday Is Nothing Then
            SUMTransactionUserToday = 0
        End If
        'End SUM Deal Transaction Today (IDR) with That User but status not in 'REJECTED', 'INACTIVE'

        'Start Get Limit Threshold and Convert it into IDR
        Dim LimitCurrencyThreshold As String
        Dim LimitThreshold As Nullable(Of Decimal) = 0
        Dim Threshold = (From a In db.Settings
                         Where a.Key1 = "Transaction" And a.Key2 = "Limit"
                         Select a).ToList

        LimitCurrencyThreshold = Threshold.Item(0).Value2
        LimitThreshold = Threshold.Item(0).Value1

        DataRate = (From e In db.ExchangeRate
                     Where e.CurrId = LimitCurrencyThreshold
                     Select e).ToList

        If DataRate.Count > 0 Then
            'If TransactionType = "Sell" Then
            '    If RateType = "TT" Then
            '        LimitThreshold = CDec(LimitThreshold * DataRate.Item(0).TTSellRate).ToString("N2")
            '    Else
            '        LimitThreshold = CDec(LimitThreshold * DataRate.Item(0).BNSellRate).ToString("N2")
            '    End If
            'Else
            '    If RateType = "TT" Then
            '        LimitThreshold = CDec(LimitThreshold * DataRate.Item(0).TTBuyRate).ToString("N2")
            '    Else
            '        LimitThreshold = CDec(LimitThreshold * DataRate.Item(0).BNBuyRate).ToString("N2")
            '    End If
            'End If
            LimitThreshold = CDec(LimitThreshold * DataRate.Item(0).ClosingRate).ToString("N2")
        End If
        'End Get Limit Threshold and Convert it into IDR

        'Start SUM Deal Transaction Today (IDR) with That Customer, but status not in 'REJECTED', 'INACTIVE'
        Dim SUMTransactionCustomerToday As Nullable(Of Decimal) = 0

        If Mode = "CREATE" Then
            SUMTransactionCustomerToday = Aggregate s In db.ExchangeTransactionHead
                                         Join t In db.ExchangeTransactionDetail On s.TransNum Equals t.TransNum
                                         Join u In db.ExchangeRate On t.TransactionCurrency Equals u.CurrId
                                         Where t.FlagReconcile = 1 And (s.Status <> "REJECTED" And s.Status <> "CLOSED") And s.AccName = AccountName _
                                         And s.EditDate.Year = Now.Year And s.EditDate.Month = Now.Month _
                                         Into Sum(u.ClosingRate * t.TransactionNominal)
        ElseIf Mode = "EDIT" Then
            SUMTransactionCustomerToday = Aggregate s In db.ExchangeTransactionHead
                                         Join t In db.ExchangeTransactionDetail On s.TransNum Equals t.TransNum
                                         Join u In db.ExchangeRate On t.TransactionCurrency Equals u.CurrId
                                         Where t.FlagReconcile = 1 And (s.Status <> "REJECTED" And s.Status <> "CLOSED") And s.AccName = AccountName _
                                         And s.EditDate.Year = Now.Year And s.EditDate.Month = Now.Month _
                                         And s.TransNum <> Transnum
            Into Sum(u.ClosingRate * t.TransactionNominal)
        End If

        If SUMTransactionCustomerToday Is Nothing Then
            SUMTransactionCustomerToday = 0
        End If
        'End SUM Deal Transaction Today (IDR) with That Customer, but status not in 'REJECTED', 'INACTIVE', 'CLOSED'

        'Start Calculate Transaction Amount Which Customer Used
        DataRate = (From e In db.ExchangeRate
                    Where e.CurrId = BaseCurrency
                    Select e).ToList

        If DataRate.Count > 0 Then
            'If TransactionType = "Sell" Then
            '    If RateType = "TT" Then
            '        BaseAmount = CDec(BaseAmount * DataRate.Item(0).TTSellRate).ToString("N2")
            '    Else
            '        BaseAmount = CDec(BaseAmount * DataRate.Item(0).BNSellRate).ToString("N2")
            '    End If
            'Else
            '    If RateType = "TT" Then
            '        BaseAmount = CDec(BaseAmount * DataRate.Item(0).TTBuyRate).ToString("N2")
            '    Else
            '        BaseAmount = CDec(BaseAmount * DataRate.Item(0).BNBuyRate).ToString("N2")
            '    End If
            'End If
            BaseAmount = CDec(BaseAmount * DataRate.Item(0).ClosingRate).ToString("N2")
        End If
        'End Calculate Transaction Amount Which Customer Used

        'Start Check Account has NPWP or not
        Dim otheracc = db.OtherAccounts.Where(Function(f) f.AccNo = AccountNumber).Count
        If otheracc = 0 Then
            Dim notNPWP As String = ("0").ToString.PadLeft(15, "0")

            Dim cif As Decimal = db.Accounts.Where(Function(f) f.AccNo = AccountNumber).FirstOrDefault.CIF

            Dim npwpCount = db.Accounts.Where(Function(f) f.CIF = cif And (f.NPWP.Length > 0 Or f.NPWP = notNPWP)).Count

            If SUMTransactionCustomerToday + BaseAmount > LimitThreshold And LimitThreshold <> 0 Then
                out.UnderLimitThreshold = False

                If npwpCount > 0 Then
                    out.HaveNPWP = True
                Else
                    out.HaveNPWP = False
                End If
            End If

            If SUMTransactionUserToday + BaseAmount > LimitUser And LimitUser <> 0 Then
                out.UnderLimitUser = False
            End If
        Else
            out.UnderLimitThreshold = True
            out.UnderLimitUser = True
            out.HaveNPWP = True
        End If

        'End Check Account has NPWP or not


        Return Json(out)
    End Function

    '
    ' Get:  /ExchangeTransaction/JsOnFillDocUnderlyingType
    Public Function JsOnFillDocUnderlyingType(AccNumber As Decimal) As ActionResult
        Dim db As New FectorEntities
        Dim listDocTrans As List(Of MsDocumentTransaction)

        Dim countryCustomer = db.Accounts.Where(Function(f) f.AccNo = AccNumber).FirstOrDefault.CountryId

        If countryCustomer = "ID" Then
            listDocTrans = (From dt In db.DocumentTransaction
                            Where dt.CustomerType = "Domestic" And dt.Status = "ACTIVE"
                            Select dt).ToList
        Else
            listDocTrans = (From dt In db.DocumentTransaction
                            Where dt.CustomerType = "Foreigner" And dt.Status = "ACTIVE"
                            Select dt).ToList
        End If

        If countryCustomer Is Nothing Then
            listDocTrans = (From dt In db.DocumentTransaction
                            Where dt.Status = "ACTIVE"
                            Select dt).ToList
        End If

        Return Json(listDocTrans)
    End Function

    Class DocUnderlying
        Public Property DocNumFound As Boolean = True
        Public Property IsNewDoc As Boolean = True
        Public Property IsOwnerUnderlying As Boolean = True
        Public Property NominalUnderlying As Decimal
        Public Property DocStatus As String
        Public Property RemainingUnderlying As Decimal
    End Class

    '
    ' POST:  /ExchangeTransaction/JsOnCheckDocUnderlying
    Public Function JsOnCheckDocUnderlying(DocUnderlyingType As String, AccNum As String, DocNum As String, TransNominal As Decimal, Mode As String, Transnum As Decimal) As ActionResult
        Dim db As New FectorEntities
        Dim out As New DocUnderlying

        Dim DocUnderlying = (From a In db.ExchangeTransactionDoc
                             Join b In db.DocumentTransaction On a.DocumentTransId Equals b.DocumentId
                             Where a.DocUnderlyingNum = DocNum And a.DocumentTransId = DocUnderlyingType
                             Select New With {.NominalDoc = a.NominalDoc, _
                                              .DocType = b.DocumentType}).ToList

        Dim ExchangeDoc = (From a In db.ExchangeTransactionDoc
                          Where a.DocUnderlyingNum = DocNum
                          Select a).FirstOrDefault

        Dim tmpTransNum As Integer = 0

        If ExchangeDoc IsNot Nothing Then
            tmpTransNum = ExchangeDoc.TransNum
        End If

        Dim OwnerUnderlying = (From a In db.ExchangeTransactionHead
                                Where a.TransNum = tmpTransNum
                                Select a).ToList

        If OwnerUnderlying.Count <> 0 Then
            Dim accno As String = OwnerUnderlying(0).AccNum
            Dim cif As Decimal
            Dim usedCIF As Decimal

            If (Mode <> "ReviewDoc") Then
                cif = db.Accounts.Where(Function(f) f.AccNo = accno).FirstOrDefault.CIF
                usedCIF = db.Accounts.Where(Function(f) f.AccNo = AccNum).FirstOrDefault.CIF
            Else
                cif = db.ExchangeTransactionHead.Where(Function(f) f.TransNum = tmpTransNum).FirstOrDefault.CIF
                usedCIF = db.ExchangeTransactionHead.Where(Function(f) f.AccNum = AccNum).FirstOrDefault.CIF
            End If

            
            If cif = usedCIF Then
                out.IsOwnerUnderlying = True
            Else
                out.IsOwnerUnderlying = False
            End If
        End If

            Dim UnderlyingUsedInTrans As Nullable(Of Decimal)

            If Mode = "CREATE" Then
                UnderlyingUsedInTrans = Aggregate s In db.ExchangeTransactionHead
                                        Join t In db.ExchangeTransactionDetail On s.TransNum Equals t.TransNum
                                        Join u In db.ExchangeTransactionDoc On s.TransNum Equals u.TransNum
                                        Where (s.Status <> "REJECTED" Or s.Status <> "CLOSED") _
                                        And u.DocUnderlyingNum = DocNum _
                                        Into Sum(t.TransactionNominal)
            Else
                UnderlyingUsedInTrans = Aggregate s In db.ExchangeTransactionHead
                                        Join t In db.ExchangeTransactionDetail On s.TransNum Equals t.TransNum
                                        Join u In db.ExchangeTransactionDoc On s.TransNum Equals u.TransNum
                                        Where (s.Status <> "REJECTED" Or s.Status <> "CLOSED") _
                                        And u.DocUnderlyingNum = DocNum _
                                        And s.TransNum <> Transnum _
                                        Into Sum(t.TransactionNominal)
            End If


            If DocUnderlying.Count = 0 Then
                out.DocNumFound = False
                out.IsNewDoc = True
                out.NominalUnderlying = 0
                out.DocStatus = ""
                out.RemainingUnderlying = 0
            Else
                out.DocNumFound = True
                out.IsNewDoc = False
                out.NominalUnderlying = DocUnderlying.Item(0).NominalDoc
                If DocUnderlying.Item(0).DocType = "Estimation" Then
                    out.DocStatus = "Estimation"
                Else
                    out.DocStatus = "Final"
                End If

                out.RemainingUnderlying = DocUnderlying.Item(0).NominalDoc - UnderlyingUsedInTrans

                If DocUnderlying.Item(0).NominalDoc - UnderlyingUsedInTrans = 0 Then
                    out.DocNumFound = False
                    out.IsNewDoc = True
                    out.NominalUnderlying = 0
                    out.DocStatus = ""
                    out.RemainingUnderlying = 0
                End If
            End If
            Return Json(out)
    End Function

    <HttpPost> _
    Public Function JsOnSubmitCheck(ByVal DealNum As String, ByVal TransactionNominal As Decimal) As ActionResult
        Dim model As New ExchangeTransactionViewModel

        Dim Action As String = Request.UrlReferrer.AbsolutePath
        Dim db As New FectorEntities

        Dim deal As New dealInfo

        deal = (From de In db.TransactionDeal
                Where de.DealNumber = DealNum
                Select New dealInfo With {.dealNumber = de.DealNumber, .accNumber = de.AccNum, _
                                        .accName = de.AccName, .tranType = de.TransactionType, .dealType = de.DealType, _
                                        .fromCurrency = de.CurrencyDeal, .toCurrency = de.CurrencyCustomer, _
                                        .exchRate = de.DealRate, .dealAmount = de.AmountDeal, _
                                        .valueDate = de.DealDate, .valuePeriod = de.DealPeriod, .counterAmount = de.AmountCustomer}).FirstOrDefault

        If TransactionNominal <> deal.dealAmount Then
            Return Json(New With {.errMessage = "Transaction nominal which used in this transaction can't be more than amount deal"})
        Else
            deal.strvalueDate = deal.valueDate.ToString("dd-MM-yyyy")
            deal.strexchRate = CDec(deal.exchRate).ToString("N2")
            deal.strdealAmount = CDec(deal.dealAmount).ToString("N2")
            deal.strcounterAmount = CDec(deal.counterAmount).ToString("N2")

            Return View("DetailUseDeal", model)
        End If

        Return Json(deal)
    End Function

    '
    ' Post:  /ExchangeTransaction/JsOnCheckTransAmount
    <HttpPost> _
    Public Function JsOnCheckTransAmount(Amount As String, TotalBaseAmount As String) As ActionResult
        If CDec(Amount).ToString("N2") - CDec(TotalBaseAmount).ToString("N2") > 0 Then
            Return Json(New With {.MoreThanBaseAmount = True})
        Else
            Return Json(New With {.MoreThanBaseAmount = False})
        End If
    End Function

    '
    ' Post:  /ExchangeTransaction/JsOnCheckTransAmount
    <HttpPost> _
    Public Function JsOnCheckNominalUnderlying(Amount As Decimal, RemainingNominalUnderlying As Decimal) As ActionResult
        If CDec(Amount).ToString("N2") > CDec(RemainingNominalUnderlying).ToString("N2") Then
            Return Json(New With {.MoreThanRemainNominalUnderlying = True})
        Else
            Return Json(New With {.MoreThanRemainNominalUnderlying = False})
        End If
    End Function

    '
    ' Post:  /ExchangeTransaction/JsOnKeepDocDetail
    <HttpPost> _
    Public Function JsOnKeepDocDetail(ByVal model As ExchangeTransactionViewModel) As ActionResult
        If Not IsNothing(model.FileDocumentTransaction) Then
            If model.FileDocumentTransaction.ContentLength > 0 Then
                Session("CountDocUnderlying") += 1

                Session.Add("FUDocUnderlying" & Session("CountDocUnderlying"), model.FileDocumentTransaction)
            End If
        Else
            If model.PrevRefDocumentTransactionName IsNot Nothing Then
                Session("CountDocUnderlying") += 1

                Session.Add("PrevDocUnderlying" & Session("CountDocUnderlying"), model.PrevRefDocumentTransactionName)
            End If
        End If

        If Not IsNothing(model.FileTransFormulir) Then
            If model.FileTransFormulir.ContentLength > 0 Then
                Session("CountTransForm") += 1

                Session.Add("FUTransForm" & Session("CountTransForm"), model.FileTransFormulir)
            End If
        End If

        Return Json(New With {.SuccessUpload = True})
    End Function

    '
    ' Post:  /ExchangeTransaction/JsOnDeleteDoc
    <HttpPost> _
    Public Function JsOnDeleteDoc(ByVal model As ExchangeTransactionViewModel, docid As String) As ActionResult
        Dim tmp As HttpPostedFileBase = Session("FUDocUnderlying" & docid.Replace("doc", ""))

        'Session("CountDocUnderlying") -= 1
        If Not IsNothing(tmp) Then
            Session.Remove("FUDocUnderlying" & docid.Replace("doc", ""))
        Else
            If Session("PrevDocUnderlying" & docid.Replace("doc", "")) IsNot Nothing Then
                Session.Remove("PrevDocUnderlying" & docid.Replace("doc", ""))
            End If
        End If

        Dim tmp2 As HttpPostedFileBase = Session("FUTransForm" & docid.Replace("doc", ""))

        'Session("CountTransForm") -= 1
        If Not IsNothing(tmp2) Then
            Session.Remove("FUTransForm" & docid.Replace("doc", ""))
        Else
            If Session("PrevTransForm" & docid.Replace("doc", "")) IsNot Nothing Then
                Session.Remove("PrevTransForm" & docid.Replace("doc", ""))
            End If
        End If

        Return Json(New With {.SuccessDelete = True})
    End Function

    '
    ' Post:  /ExchangeTransaction/JsOnCheckDocSize
    <HttpPost> _
    Public Function JsOnCheckDocSize(ByVal model As ExchangeTransactionViewModel) As ActionResult
        Dim StatementAttachmentSize As Decimal = 0
        Dim TransformAttachmentSize As Decimal = 0
        Dim UnderlyingAttachmentSize As Decimal = 0
        Dim MaxUploadSize As String = MsSetting.GetSetting("UploadFile", "MaxSize", "", 1)

        If Not IsNothing(model.FileTransFormulir) Then
            If model.FileTransFormulir.ContentLength > 0 Then
                If model.FileTransFormulir.ContentLength / 1000000 > CDec(MaxUploadSize) Then
                    Return Json(New With {.FailUpload = True, .DocTransForm = True})
                End If
            End If
        End If

        If Not IsNothing(model.FileDocumentTransaction) Then
            If model.FileDocumentTransaction.ContentLength > 0 Then
                If model.FileDocumentTransaction.ContentLength / 1000000 > CDec(MaxUploadSize) Then
                    Return Json(New With {.FailUpload = True, .DocTrans = True})
                End If
            End If
        End If

        ModelState.Remove("FileDocumentStatementUnderlimit")
        ModelState.Remove("FileDocumentStatementOverlimit")
        Return Json(New With {.FailUpload = False})
    End Function

    ''
    '' Get: /ExchangeTransaction/Counter
    '<Authorize> _
    'Public Function Counter() As ActionResult
    '    Dim model As New ExchangeTransactionViewModel
    '    Dim db As New FectorEntities

    '    model.TransNum = "(AUTO)"
    '    ViewData("Title") = "Create Counter Transaction"
    '    ViewBag.Breadcrumb = "Home > Exchange Transaction > Counter"
    '    ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
    '    ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
    '    ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
    '    ViewBag.CurrencyNoIDR = New SelectList(db.ExchangeRate.Where(Function(f) f.CurrId <> "IDR"), "CurrId", "CurrencyDisplay")
    '    ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
    '    ViewBag.SourceFundsOption = SourceFundsOption
    '    ViewBag.RateType = RateTypeOption
    '    ViewBag.TransactionTypeOption = TransactionTypeOption
    '    ViewBag.DealPeriodOption = DealPeriodOption
    '    ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

    '    model.UseDeal = False
    '    model.TransactionNominal = 0
    '    model.BranchId = Session("BranchId")

    '    Return View("Detail", model)
    'End Function

    ''
    '' Post: /ExchangeTransaction/Counter
    '<HttpPost> _
    '<ValidateAntiForgeryToken> _
    '<Authorize> _
    'Public Function Counter(model As ExchangeTransactionViewModel) As ActionResult
    '    Dim db As New FectorEntities
    '    Dim file As HttpPostedFileBase = model.FileDocumentTransaction
    '    model.TransNum = "(AUTO)"
    '    ViewData("Title") = "Create Counter Transaction"
    '    ViewBag.Breadcrumb = "Home > Exchange Transaction > Counter"
    '    ViewBag.Branch = New SelectList(db.Branches, "BranchId", "BranchDisplay")
    '    ViewBag.DocumentTransOption = New SelectList(db.DocumentTransaction.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "Description")
    '    ViewBag.PurposeIdOption = New SelectList(db.Purposes.Where(Function(f) f.Status = "ACTIVE"), "PurposeId", "PurposeDisplay")
    '    ViewBag.CurrencyNoIDR = New SelectList(db.ExchangeRate.Where(Function(f) f.CurrId <> "IDR"), "CurrId", "CurrencyDisplay")
    '    ViewBag.Currency = New SelectList(db.ExchangeRate, "CurrId", "CurrencyDisplay")
    '    ViewBag.SourceFundsOption = SourceFundsOption
    '    ViewBag.RateType = RateTypeOption
    '    ViewBag.TransactionTypeOption = TransactionTypeOption
    '    ViewBag.DealPeriodOption = DealPeriodOption
    '    ViewBag.LHBUDocumentOption = New SelectList(db.DocumentLHBU.Where(Function(f) f.Status = "ACTIVE"), "DocumentId", "DocumentLHBUDisplay")

    '    If ModelState.IsValid Then
    '        Dim transnum As Decimal = 0
    '        Dim tdate As DateTime = Now
    '        Dim branchid As String = Session("BranchId")
    '        Dim accno As String = model.AccNum
    '        Dim accname As String = model.AccName
    '        Dim trantype As String = model.TransactionType
    '        Dim ratetype As String = model.RateType
    '        Dim basecurr As String = model.TransactionCurrency
    '        Dim countercurr As String = model.CustomerCurrency
    '        Dim bsubjectstatusid As String = ""
    '        Dim bbankcode As String = ""
    '        Dim bcountryid As String = ""
    '        Dim bbusinesstypeid As String = ""
    '        Dim ssubjectstatusid As String = ""
    '        Dim sbankcode As String = ""
    '        Dim scountryid As String = ""
    '        Dim sbusinesstypeid As String = ""
    '        Dim sourcefund As String = model.SourceFunds
    '        Dim sourceaccnum As String = model.SourceAccNum
    '        Dim sourceaccname As String = model.SourceAccName
    '        Dim nominalunderlying As Decimal = model.NominalUnderlying
    '        Dim doctransid As String = model.DocumentTransId
    '        Dim doclhbuid As String = model.DocumentLHBUId
    '        Dim purposeid As String = model.PurposeId
    '        Dim doctranslink As String = ""
    '        Dim docstatementlink As String = ""
    '        Dim transformlink As String = ""
    '        Dim editby As String = User.Identity.Name
    '        Dim editdate As DateTime = Now
    '        Dim status As String = ""
    '        Dim flagreconcile As Integer = 0
    '        Dim dealnumber As String = ""
    '        Dim transcurr As String = model.TransactionCurrency
    '        Dim transrate As Decimal = model.TransactionRate
    '        Dim transamount As Decimal = model.TransactionNominal
    '        Dim customercurr As String = model.CustomerCurrency
    '        Dim customeramount As Decimal = model.CustomerNominal
    '        Dim valperiod As String = model.ValuePeriod
    '        Dim valdate As DateTime = AppHelper.dateConvert(model.ValueDate)

    '        Dim custinfo = (From accext In db.AccountsExtension
    '                           Join acc In db.Accounts On accext.AccNo Equals acc.CIF
    '                           Where acc.AccNo = accno
    '                           Select New With {.ssid = acc.SubjectStatusId, .bc = acc.BICode, .cid = acc.CountryId, .btid = acc.BusinessTypeId}).FirstOrDefault

    '        If custinfo.cid Is Nothing Then
    '            ModelState.AddModelError("AccNum", "Country not found for this customer, please complete the country in LLD")

    '            Return View("Detail", model)
    '        End If

    '        If trantype = "Buy" Then
    '            'Fill Buyer with Bank Information
    '            bsubjectstatusid = MsSetting.GetSetting("Bank", "SubjectStatusId", "", 1)
    '            bbankcode = MsSetting.GetSetting("Bank", "BICode", "", 1)
    '            bcountryid = MsSetting.GetSetting("Bank", "CountryID", "", 1)
    '            bbusinesstypeid = MsSetting.GetSetting("Bank", "BusinessTypeID", "", 1)

    '            ssubjectstatusid = custinfo.ssid
    '            sbankcode = custinfo.bc
    '            scountryid = custinfo.cid
    '            sbusinesstypeid = custinfo.btid
    '        Else
    '            'Fill Seller with Bank Information
    '            ssubjectstatusid = MsSetting.GetSetting("Bank", "SubjectStatusId", "", 1)
    '            sbankcode = MsSetting.GetSetting("Bank", "BICode", "", 1)
    '            scountryid = MsSetting.GetSetting("Bank", "CountryID", "", 1)
    '            sbusinesstypeid = MsSetting.GetSetting("Bank", "BusinessTypeID", "", 1)

    '            bsubjectstatusid = custinfo.ssid
    '            bbankcode = custinfo.bc
    '            bcountryid = custinfo.cid
    '            bbusinesstypeid = custinfo.btid
    '        End If

    '        Dim th As New ExchangeTransactionHead
    '        th.AccName = accname
    '        th.AccNum = accno
    '        th.ApproveBy = ""
    '        th.ApproveDate = New DateTime(1900, 1, 1)
    '        th.BBankCode = bbankcode
    '        th.BBusinessTypeId = bbusinesstypeid
    '        th.BCountryId = bcountryid
    '        th.BranchId = branchid
    '        th.BSubjectStatusId = bsubjectstatusid
    '        th.DocumentStatementUnderlimitLink = docstatementlink
    '        th.DocumentTransactionLink = doctranslink
    '        th.DocumentTransId = doctransid
    '        th.EditBy = editby
    '        th.EditDate = editdate
    '        th.DocumentLHBUId = doclhbuid
    '        th.PurposeId = purposeid
    '        th.RateType = ratetype
    '        th.SBankCode = sbankcode
    '        th.SBusinessTypeId = sbusinesstypeid
    '        th.SCountryId = scountryid
    '        th.SourceFunds = sourcefund
    '        th.SourceAccNum = sourceaccnum
    '        th.SourceAccName = sourceaccname
    '        th.NominalUnderlying = nominalunderlying
    '        th.SSubjectStatusId = ssubjectstatusid
    '        If CheckLimit(accno, accname, trantype, ratetype, basecurr, transamount, valperiod, "CREATE") = False Then
    '            th.Status = "COUNTER - PENDING"
    '        Else
    '            th.Status = "ACTIVE"
    '        End If
    '        th.TDate = tdate
    '        th.TransactionType = trantype
    '        th.TransFormulirLink = transformlink

    '        db.ExchangeTransactionHead.Add(th)
    '        db.SaveChanges()

    '        transnum = th.TransNum
    '        If Not IsNothing(model.FileDocumentStatement) Then
    '            If model.FileDocumentStatement.ContentLength > 0 Then
    '                Dim filenamesplit As String() = model.FileDocumentStatement.FileName.Split(".")
    '                Dim filename As String = "DocStatement." & filenamesplit(filenamesplit.Length - 1)
    '                Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

    '                If Not IO.Directory.Exists(Path) Then
    '                    IO.Directory.CreateDirectory(Path)
    '                End If

    '                th.DocumentStatementUnderlimitLink = "~/Uploads/Transaction/" & transnum & "/" & filename
    '                model.FileDocumentStatement.SaveAs(Path & filename)
    '            End If
    '        ElseIf model.PrevRefDocumentStatementUnderlimitName <> "" Then
    '            th.DocumentStatementUnderlimitLink = model.PrevRefDocumentStatementUnderlimitName
    '        End If

    '        If Not IsNothing(model.FileDocumentTransaction) Then
    '            If model.FileDocumentTransaction.ContentLength > 0 Then
    '                Dim filenamesplit As String() = model.FileDocumentTransaction.FileName.Split(".")
    '                Dim filename As String = "DocTrans." & filenamesplit(filenamesplit.Length - 1)
    '                Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

    '                If Not IO.Directory.Exists(Path) Then
    '                    IO.Directory.CreateDirectory(Path)
    '                End If

    '                th.DocumentTransactionLink = "~/Uploads/Transaction/" & transnum & "/" & filename
    '                model.FileDocumentTransaction.SaveAs(Path & filename)
    '            End If
    '        ElseIf model.PrevRefDocumentTransactionName <> "" Then
    '            th.DocumentTransactionLink = model.PrevRefDocumentTransactionName
    '        End If

    '        If Not IsNothing(model.FileTransFormulir) Then
    '            If model.FileTransFormulir.ContentLength > 0 Then
    '                Dim filenamesplit As String() = model.FileTransFormulir.FileName.Split(".")
    '                Dim filename As String = "TransForm." & filenamesplit(filenamesplit.Length - 1)
    '                Dim Path As String = Server.MapPath("~/Uploads/Transaction/" & transnum & "/")

    '                If Not IO.Directory.Exists(Path) Then
    '                    IO.Directory.CreateDirectory(Path)
    '                End If

    '                th.TransFormulirLink = "~/Uploads/Transaction/" & transnum & "/" & filename
    '                model.FileTransFormulir.SaveAs(Path & filename)
    '            End If
    '        End If

    '        db.Entry(th).State = EntityState.Modified
    '        db.SaveChanges()

    '        Dim td As New ExchangeTransactionDetail
    '        td.CustomerCurrency = customercurr
    '        td.CustomerNominal = customeramount
    '        td.DealNumber = dealnumber
    '        td.TransactionCurrency = transcurr
    '        td.TransactionNominal = transamount
    '        td.TransactionRate = transrate
    '        td.TransNum = transnum
    '        td.ValueDate = valdate
    '        td.ValuePeriod = valperiod
    '        td.FlagReconcile = flagreconcile

    '        db.ExchangeTransactionDetail.Add(td)
    '        Try
    '            db.SaveChanges()
    '        Catch ex As DbEntityValidationException
    '            For Each eve In ex.EntityValidationErrors
    '                Dim errmessage = eve.ValidationErrors.Count
    '            Next
    '        End Try

    '        LogTransaction.WriteLog(User.Identity.Name, "COUNTER TRANSACTION", transnum, db)

    '        Return Redirect("~/ExchangeTransaction")
    '    End If

    '    Return View("Detail", model)
    'End Function
End Class