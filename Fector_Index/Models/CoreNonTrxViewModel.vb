Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class CoreNonTrxViewModel
    <Key> _
    <DisplayName("Transaction Number")> _
    Public Property Refno As Nullable(Of Decimal)

    <DisplayName("Date")> _
    Public Property TDate As DateTime

    <DisplayName("Account Number")> _
    Public Property AccNo As String

    <DisplayName("Currency")> _
    Public Property CoreCurrId As String

    <DisplayName("Branch")> _
    Public Property BranchId As Nullable(Of Decimal)

    <DisplayName("Amount")> _
    Public Property Amount As Nullable(Of Decimal)

    <DisplayName("Time")> _
    Public Property Time As Integer

    <DisplayName("Rate")> _
    Public Property Rate As Nullable(Of Decimal)

    Public Property FlagLHBU As Integer

    Public Shared Function GetCoreNonTrx(ByVal RefNo As String, ByVal db As FectorEntities) As CoreNonTransaction
        Return db.CoreNonTrx.Where(Function(f) f.Refno = RefNo).SingleOrDefault
    End Function
End Class
