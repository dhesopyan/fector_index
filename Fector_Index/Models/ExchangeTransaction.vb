Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class ExchangeTransaction
    'Head
    <Key> _
    <DisplayName("Transaction Number")> _
    <StringLength(20)> _
    <Required(ErrorMessage:="Please fill transaction number")> _
    Public Property TransNum As String

    <DisplayName("Deal Number")> _
    <StringLength(20)> _
    <Required(ErrorMessage:="Please fill transaction number")> _
    Public Property DealNumber As String

    <DisplayName("Date")> _
    <Required(ErrorMessage:="Please fill the date")> _
    <DisplayFormat(DataFormatString:="{0:yyyy/MM/dd}", ApplyFormatInEditMode:=True)> _
    Public Property TDate As Nullable(Of Date)

    <DisplayName("Account Number")> _
    <StringLength(20)> _
    Public Property AccNum As String

    <DisplayName("Account Name")> _
    <StringLength(50)> _
    Public Property AccName As String

    <DisplayName("Branch")> _
    <ForeignKey("Branch")> _
    <Required(ErrorMessage:="Please select branch")> _
    Public Property BranchId As String
    Public Overridable Property Branch As MsBranch

    <DisplayName("Rate Type")> _
    <StringLength(2)> _
    <Required(ErrorMessage:="Please choose the rate type")> _
    Public Property RateType As String

    <DisplayName("Transaction Type")> _
    <StringLength(5)> _
    <Required(ErrorMessage:="Please choose the transaction type")> _
    Public Property TransactionType As String

    <DisplayName("Customer Status")> _
    <Required(ErrorMessage:="Please select customer status")> _
    Public Property BSubjectStatusId As String

    Public Property SSubjectStatusId As String

    <DisplayName("BI Customer")> _
    <Required(ErrorMessage:="Please fill the BI customer")> _
    Public Property BBankCode As String

    Public Property SBankCode As String

    <DisplayName("Customer Country")> _
    <Required(ErrorMessage:="Please select customer country")> _
    Public Property BCountryId As String

    Public Property SCountryId As String

    <DisplayName("Customer Business Type")> _
    <Required(ErrorMessage:="Please select customer business type")> _
    Public Property BBusinessTypeId As String

    Public Property SBusinessTypeId As String

    <DisplayName("Source of Funds")> _
    <StringLength(5)> _
    <Required(ErrorMessage:="Please select source of funds")> _
    Public Property SourceFunds As String

    <DisplayName("Document Transaction")> _
    <StringLength(10)> _
    <Required(ErrorMessage:="Please select document transaction")> _
    Public Property DocumentTransId As String

    <DisplayName("Purpose")> _
    <StringLength(2)> _
    <Required(ErrorMessage:="Please select the purpose")> _
    Public Property PurposeId As String

    <DisplayName("File Document Transaction")> _
    <StringLength(2)> _
    <Required(ErrorMessage:="Please upload file document transaction")> _
    Public Property FileDocumentTransaction As String

    <DisplayName("File Document Statement")> _
    <StringLength(2)> _
    <Required(ErrorMessage:="Please upload file document statement")> _
    Public Property FileDocumentStatement As String

    <DisplayName("File Transaction Formulir")> _
    <StringLength(2)> _
    <Required(ErrorMessage:="Please upload file transaction formulir")> _
    Public Property FileTransFormulir As String

    'Detail
    Public Property Detail As String()
    Public Property TransactionDetail As List(Of String())

    <DisplayName("Transaction Currency")> _
    <StringLength(3)> _
    <Required(ErrorMessage:="Please select the transaction currency")> _
    Public Property TransactionCurrency As String

    <DisplayName("Transaction Rate")> _
    <Required(ErrorMessage:="Please fill transaction rate")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property TransactionRate As Nullable(Of Decimal)

    <DisplayName("Transaction Nominal")> _
    <Required(ErrorMessage:="Please fill transaction nominal")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property TransactionNominal As Nullable(Of Decimal)

    <DisplayName("Customer Currency")> _
    <StringLength(3)> _
    <Required(ErrorMessage:="Please select the customer currency")> _
    Public Property CustomerCurrency As String

    <DisplayName("Customer Rate")> _
    <Required(ErrorMessage:="Please fill customer rate")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property CustomerRate As Nullable(Of Decimal)

    <DisplayName("Customer Nominal")> _
    <Required(ErrorMessage:="Please fill customer nominal")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property CustomerNominal As Nullable(Of Decimal)

    <DisplayName("Value Period")> _
    <StringLength(4)> _
    <Required(ErrorMessage:="Please select the value period")> _
    Public Property ValuePeriod As String

    <DisplayName("Value Date")> _
    <Required(ErrorMessage:="Please fill the value date")> _
    <DisplayFormat(DataFormatString:="{0:yyyy/MM/dd}", ApplyFormatInEditMode:=True)> _
    Public Property ValueDate As Nullable(Of Date)

    Public Property EditBy As String
    Public Property EditDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property Status As String
    Public Property FlagReconsile As String

    Public Shared Function GetExchangeTransaction(ByVal TransNum As String, ByVal db As FectorEntities) As ExchangeTransaction
        Return db.ExchangeTransaction.Where(Function(f) f.TransNum = TransNum).SingleOrDefault
    End Function


End Class
