Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel
Imports System.Data.Objects

Public Class ReconcileViewModel
    Public Property PendingTransaction As List(Of ReconsileTrx)
    Public Property CoreUnmapped As List(Of CoreTransaction)

    Public Property SelectedTransaction As String
    Public Property SelectedCoreTrx As String

    Public Shared Function GetTodayPendingTransaction(ByRef db As FectorEntities, ByVal BranchId As String) As List(Of ReconsileTrx)
        Dim DateNow As Date = Now.Date
        Dim trx = (From a In db.ExchangeTransactionDetail
                   Join b In db.ExchangeTransactionHead On a.TransNum Equals b.TransNum
                   Where a.FlagReconcile = 0 And b.BranchId = BranchId And b.Status = "ACTIVE"
                   Select New ReconsileTrx With {.TransNum = a.TransNum, _
                                                 .DealNumber = a.DealNumber, _
                                                 .AccNum = b.AccNum, _
                                                 .AccName = b.AccName, _
                                                 .RateType = b.RateType, _
                                                 .TransactionType = b.TransactionType, _
                                                 .TransactionCurrency = a.TransactionCurrency, _
                                                 .TransactionRate = a.TransactionRate, _
                                                 .TransactionNominal = a.TransactionNominal, _
                                                 .CustomerCurrency = a.CustomerCurrency, _
                                                 .CustomerNominal = a.CustomerNominal, _
                                                 .ValuePeriod = a.ValuePeriod, _
                                                 .ValueDate = a.ValueDate, _
                                                 .FlagReconcile = a.FlagReconcile, _
                                                 .SourceFunds = b.SourceFunds, _
                                                 .SourceAccNum = b.SourceAccNum, _
                                                 .SourceAccName = b.SourceAccName, _
                                                 .SourceNominal = b.SourceNominal}).ToList

        Return trx.ToList
    End Function

    Public Shared Function GetCoreTransaction(ByRef db As FectorEntities, ByVal BranchId As String) As List(Of CoreTransaction)
        Dim trx = db.CoreTrx.Where(Function(f) f.TDate = EntityFunctions.TruncateTime(DateTime.Now) And Not f.ExchangeTransactionNumber.HasValue And f.BranchId = BranchId And f.Status = 1)
        Return trx.ToList
    End Function

    Public Class ReconsileTrx
        Public TransNum As Decimal
        Public DealNumber As String
        Public AccNum As String
        Public AccName As String
        Public RateType As String
        Public TransactionType As String
        Public TransactionCurrency As String
        Public TransactionRate As Nullable(Of Decimal)
        Public TransactionNominal As Nullable(Of Decimal)
        Public CustomerCurrency As String
        Public CustomerNominal As Nullable(Of Decimal)
        Public ValuePeriod As String
        Public ValueDate As Nullable(Of Date)
        Public FlagReconcile As Nullable(Of Integer)
        Public SourceFunds As String
        Public SourceAccNum As String
        Public SourceAccName As String
        Public SourceNominal As Nullable(Of Decimal)
    End Class
End Class
