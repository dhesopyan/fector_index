Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class ExchangeTransactionViewModel
    <Key> _
    <DisplayName("Transaction Number")> _
    Public Property TransNum As String

    Public Property UseDeal As Boolean

    <StringLength(20)> _
    <DisplayName("Deal Number")> _
    Public Property DealNumber As String

    <DisplayName("Account Number")> _
    <StringLength(20)> _
    <Required(ErrorMessage:="Please select the account number")> _
    Public Property AccNum As String

    <DisplayName("Account Name")> _
    <StringLength(50)> _
    Public Property AccName As String

    <DisplayName("Branch")> _
    <ForeignKey("Branch")> _
    Public Property BranchId As String
    Public Overridable Property Branch As MsBranch

    <DisplayName("Transaction Type")> _
    <StringLength(2)> _
    <Required(ErrorMessage:="Please select the transaction type")> _
    Public Property RateType As String

    <DisplayName("Bank Position")> _
    <StringLength(5)> _
    <Required(ErrorMessage:="Please select the bank position")> _
    Public Property TransactionType As String

    <DisplayName("Source of Funds")> _
    <StringLength(10)> _
    Public Property SourceFunds As String

    <DisplayName("Source Account Number")> _
    <StringLength(20)> _
    Public Property SourceAccNum As String

    <DisplayName("Source Account Name")> _
    <StringLength(50)> _
    Public Property SourceAccName As String

    <DisplayName("Source Nominal")> _
    <Required(ErrorMessage:="Please fill source nominal")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property SourceNominal As Nullable(Of Decimal)

    <ForeignKey("DocumentTransaction")> _
    <DisplayName("Document Underlying Type")> _
    Public Property DocumentTransId As String
    Public Overridable Property DocumentTransaction As MsDocumentTransaction

    <DisplayName("Document Underlying Number")> _
    Public Property DocUnderlyingNum As String

    <DisplayName("Document Underlying Nominal")> _
    Public Property DocUnderlyingNominal As Nullable(Of Decimal)

    <DisplayName("Document Underlying Remaining")> _
    Public Property DocUnderlyingRemaining As Nullable(Of Decimal)

    <ForeignKey("DocumentLHBU")> _
    <DisplayName("LHBU Document")> _
    Public Property DocumentLHBUId As String
    Public Overridable Property DocumentLHBU As MsDocumentLHBU

    <ForeignKey("Purpose")> _
    <DisplayName("LHBU Purpose")> _
    Public Property PurposeId As String
    Public Overridable Property Purpose As MsPurpose

    <DisplayName("Transaction Document")> _
    Public Property FileDocumentTransaction As HttpPostedFileBase

    <DisplayName("Previous Transaction Document")> _
    Public Property PrevRefDocumentTransactionName As String

    <DisplayName("Letter of Statement(Underlimit)")> _
    Public Property FileDocumentStatementUnderlimit As HttpPostedFileBase

    <DisplayName("Previous Letter of Statement(Underlimit)")> _
    Public Property PrevRefDocumentStatementUnderlimitName As String

    <DisplayName("Letter of Statement(Overlimit)")> _
    Public Property FileDocumentStatementOverlimit As HttpPostedFileBase

    <DisplayName("Previous Letter of Statement(Overlimit)")> _
    Public Property PrevRefDocumentStatementOverlimitName As String

    <DisplayName("Transaction Form")> _
    Public Property FileTransFormulir As HttpPostedFileBase

    <DisplayName("Previous Transaction Form")> _
    Public Property PrevRefTransactionFormName As String

    <DisplayName("Base Currency")> _
    <Required(ErrorMessage:="Please select the transaction currency")> _
    Public Property TransactionCurrency As String

    <DisplayName("Base Rate")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property TransactionRate As Nullable(Of Decimal)

    <DisplayName("Amount")> _
    <Required(ErrorMessage:="Please fill amount")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property TransactionNominal As Nullable(Of Decimal)

    <DisplayName("Counter Currency")> _
    <StringLength(3)> _
    <Required(ErrorMessage:="Please select the counter currency")> _
    Public Property CustomerCurrency As String

    <DisplayName("Converted Amount")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property CustomerNominal As Nullable(Of Decimal)

    <DisplayName("Value Period")> _
    <StringLength(4)> _
    <Required(ErrorMessage:="Please select the value period")> _
    Public Property ValuePeriod As String

    <DisplayName("Value Date")> _
    Public Property ValueDate As String

    <DisplayName("Netting Transaction")> _
    Public Property IsNetting As Boolean = False

    <DisplayName("Derivative Type")> _
    Public Property DerivativeType As String

    <DisplayName("Netting Type")> _
    Public Property NettingType As String

    Public Property ListOfDeal As New List(Of TransactionDetail)
    Public Property ListOfDoc As New List(Of ExchangeTransactionDoc)
    Public Property PassLimitThreshold As Boolean = False
    Public Property PassLimitUser As Boolean = False
    Public Property FlagUploadUnderlying As Boolean = False
    Public Property AddAnotherTransaction As Boolean = False

    Public Shared Function GetExchangeTransaction(ByVal TransNum As Integer, ByVal db As FectorEntities) As ExchangeTransactionViewModel
        Dim ExchangeTransactionHead = db.ExchangeTransactionHead.Where(Function(f) f.TransNum = TransNum).SingleOrDefault

        Dim ExchangeTransactionDetail = db.ExchangeTransactionDetail.Where(Function(f) f.TransNum = TransNum).ToList

        Dim ExchangeTransactionDoc = db.ExchangeTransactionDoc.Where(Function(f) f.TransNum = TransNum).ToList

        Dim ListDetail = (From d In db.ExchangeTransactionDetail
                          Join h In db.ExchangeTransactionHead On d.TransNum Equals h.TransNum
                          Where d.TransNum = TransNum And d.DealNumber <> ""
                          Select New TransactionDetail With {.DealNumber = d.DealNumber, _
                                                             .AccNumber = h.AccNum, _
                                                             .AccName = h.AccName, _
                                                             .BaseCurrency = d.TransactionCurrency, _
                                                             .TransRate = d.TransactionRate, _
                                                             .BaseNominal = d.TransactionNominal, _
                                                             .CounterCurrency = d.CustomerCurrency, _
                                                             .CounterNominal = d.CustomerNominal}).ToList

        'Dim Underlying = db.DocUnderlying.Where(Function(f) f.DocNum = ExchangeTransactionDoc.DocUnderlyingNum).SingleOrDefault

        Dim TotalNominal As Decimal = 0
        For i As Integer = 0 To ListDetail.Count - 1
            TotalNominal += ListDetail(i).BaseNominal
        Next

        Dim mdl As New ExchangeTransactionViewModel
        With mdl
            .TransNum = ExchangeTransactionHead.TransNum
            .AccNum = ExchangeTransactionHead.AccNum
            .AccName = ExchangeTransactionHead.AccName
            .BranchId = ExchangeTransactionHead.BranchId
            .TransactionType = ExchangeTransactionHead.TransactionType
            .RateType = ExchangeTransactionHead.RateType
            .TransactionCurrency = ExchangeTransactionDetail(0).TransactionCurrency
            .CustomerCurrency = ExchangeTransactionDetail(0).CustomerCurrency
            .ValuePeriod = ExchangeTransactionDetail(0).ValuePeriod
            .ValueDate = CDate(ExchangeTransactionDetail(0).ValueDate).ToString("dd-MM-yyyy")
            .SourceFunds = ExchangeTransactionHead.SourceFunds
            .SourceAccNum = ExchangeTransactionHead.SourceAccNum
            .SourceAccName = ExchangeTransactionHead.SourceAccName
            .SourceNominal = ExchangeTransactionHead.SourceNominal
            '.DocUnderlyingNum = ExchangeTransactionHead.DocUnderlyingNum
            'If Underlying IsNot Nothing Then
            '    .DocUnderlyingNominal = Underlying.NominalDoc
            '    .DocUnderlyingRemaining = Underlying.NominalDoc - TotalNominal
            'Else
            '    .DocUnderlyingNominal = 0
            '    .DocUnderlyingRemaining = 0
            'End If
            '.DocumentTransId = ExchangeTransactionHead.DocumentTransId
            '.DocumentLHBUId = ExchangeTransactionHead.DocumentLHBUId
            '.PurposeId = ExchangeTransactionHead.PurposeId
            If ListDetail.Count <> 0 Then
                .ListOfDeal = ListDetail
                '.TransactionNominal = "0.00"
            Else
                .ListOfDeal = Nothing
                .TransactionNominal = ExchangeTransactionDetail(0).TransactionNominal
                .TransactionRate = CDec(ExchangeTransactionDetail(0).TransactionRate).ToString("N2")
                .CustomerNominal = ExchangeTransactionDetail(0).CustomerNominal
            End If

            If ExchangeTransactionDoc.Count <> 0 Then
                .ListOfDoc = ExchangeTransactionDoc
            Else
                .ListOfDoc = Nothing
            End If
        End With

        Return mdl
    End Function
End Class

Public Class TransactionDetail
    Public Property DealNumber As String
    Public Property AccNumber As String
    Public Property AccName As String
    Public Property BaseCurrency As String
    Public Property TransRate As Decimal
    Public Property BaseNominal As Decimal
    Public Property CounterCurrency As String
    Public Property CounterNominal As Decimal
End Class

