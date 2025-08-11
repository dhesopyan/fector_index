Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class TransactionDeal
    <Key> _
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property ID As Decimal

    <DisplayName("Deal Number")> _
    Public Property DealNumber As String

    <StringLength(100)> _
    <DisplayName("Account Number")> _
    <Required(ErrorMessage:="Please fill account number")> _
    Public Property AccNum As String

    <StringLength(100)> _
    <DisplayName("Account Name")> _
    <Required(ErrorMessage:="Please fill account name")> _
    Public Property AccName As String

    <DisplayName("Branch")> _
    <ForeignKey("Branch")> _
    Public Property BranchId As String
    Public Overridable Property Branch As MsBranch

    <StringLength(100)> _
    <DisplayName("Transaction Type")> _
    <Required(ErrorMessage:="Please select transaction type")> _
    Public Property TransactionType As String

    <StringLength(100)> _
    <DisplayName("Bank Position")> _
    <Required(ErrorMessage:="Please select deal type")> _
    Public Property DealType As String

    <DisplayName("Base Currency")> _
    <Required(ErrorMessage:="Please select base currency")> _
    Public Property CurrencyDeal As String

    <DisplayName("Base Rate")> _
    <Required(ErrorMessage:="Please fill base rate")> _
    <DisplayFormat(DataFormatString:="{0:N4}", ApplyFormatInEditMode:=True)> _
    Public Property DealRate As Nullable(Of Decimal)

    <DisplayName("Base Amount")> _
    <Required(ErrorMessage:="Please fill amount deal")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property AmountDeal As Nullable(Of Decimal)

    <StringLength(100)> _
    <DisplayName("Counter Currency")> _
    <Required(ErrorMessage:="Please select counter currency")> _
    Public Property CurrencyCustomer As String

    <DisplayName("Counter Rate")> _
    <Required(ErrorMessage:="Please fill counter rate")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property RateCustomer As Nullable(Of Decimal)

    <DisplayName("Counter Amount")> _
    <Required(ErrorMessage:="Please fill counter amount")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property AmountCustomer As Nullable(Of Decimal)

    <StringLength(100)> _
    <DisplayName("Value Period")> _
    <Required(ErrorMessage:="Please select deal period")> _
    Public Property DealPeriod As String

    <DisplayName("Value Date")> _
    <Required(ErrorMessage:="Please fill deal date")> _
    <DisplayFormat(DataFormatString:="{0:yyyy/MM/dd}", ApplyFormatInEditMode:=True)> _
    Public Property DealDate As DateTime

    Public Property CloseReason As String
    Public Property EditBy As String
    Public Property EditDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property Status As String

    Public Shared Function GetTransactionDeal(ByVal ID As String, ByVal db As FectorEntities) As TransactionDeal
        'Return db.TransactionDeal.Where(Function(f) f.DealNumber = DealNum And f.Status = "UNFINISHED").SingleOrDefault
        Return db.TransactionDeal.Where(Function(f) f.ID = ID).SingleOrDefault
    End Function

    Public Shared Function GetTransDealWithDealNumber(ByVal DealNumber As String, ByVal db As FectorEntities) As TransactionDeal
        'Return db.TransactionDeal.Where(Function(f) f.DealNumber = DealNum And f.Status = "UNFINISHED").SingleOrDefault
        Return db.TransactionDeal.Where(Function(f) f.DealNumber = DealNumber).SingleOrDefault
    End Function

    Public ReadOnly Property TypeDisplay As String
        Get
            Return String.Format("{0} - {1}", TransactionType, DealType)
        End Get
    End Property

    Public ReadOnly Property BaseAmountDisplay As String
        Get
            Return String.Format("{0} - {1}", CurrencyDeal, CDec(AmountDeal).ToString("N2"))
        End Get
    End Property

    Public ReadOnly Property CounterAmountDisplay As String
        Get
            Return String.Format("{0} - {1}", CurrencyCustomer, CDec(AmountCustomer).ToString("N2"))
        End Get
    End Property

    Public ReadOnly Property ValuePeriodDisplay As String
        Get
            Return String.Format("{0} - {1}", DealPeriod, CDate(DealDate).ToString("dd-MM-yyyy"))
        End Get
    End Property
End Class
