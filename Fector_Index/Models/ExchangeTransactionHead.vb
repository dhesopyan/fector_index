Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.IO

Public Class ExchangeTransactionHead
    <Key> _
    <DisplayName("Transaction Number")> _
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property TransNum As Decimal

    <DisplayName("Date")> _
    <Required(ErrorMessage:="Please fill the date")> _
    <DisplayFormat(DataFormatString:="{0:yyyy/MM/dd}", ApplyFormatInEditMode:=True)> _
    Public Property TDate As DateTime

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
    <StringLength(10)> _
    <Required(ErrorMessage:="Please select source of funds")> _
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

    Public Property DocumentStatementUnderlimitLink As String
    Public Property DocumentStatementOverlimitLink As String

    <DisplayName("Netting Transaction")> _
    Public Property IsNetting As Boolean

    <DisplayName("Derivative Type")> _
    Public Property DerivativeType As Integer

    <DisplayName("Netting Type")> _
    Public Property NettingType As Integer

    Public Property EditBy As String
    Public Property EditDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property Status As String
    Public Property CIF As Decimal

    Public Shared Function GetExchangeTransHead(ByVal TransNum As String, ByVal db As FectorEntities) As ExchangeTransactionHead
        Return db.ExchangeTransactionHead.Where(Function(f) f.TransNum = TransNum).SingleOrDefault
    End Function
End Class
