Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class CoreTransaction
    <Key> _
    Public Property Refno As String
    Public Property TDate As DateTime
    Public Property AccNo As String
    Public Property CoreCurrId As String
    Public Property BranchId As Decimal
    Public Property Amount As Nullable(Of Decimal)
    Public Property Time As Integer
    Public Property ExchangeTransactionNumber As Nullable(Of Decimal)
    Public Property DealNumber As String
    Public Property Status As Integer

    <NotMapped> _
    Public ReadOnly Property TimeStr As String
        Get
            Dim tempStr As String = Right("000000" & Time, 6)
            tempStr = Left(tempStr, 2) & ":" & Mid(tempStr, 3, 2) & ":" & Right(tempStr, 2)
            Return tempStr
        End Get
    End Property

    Public Shared Function GetAccount(ByVal AccNo As String, ByVal db As FectorEntities) As MsAccount
        Return db.Accounts.Where(Function(f) f.AccNo = AccNo).SingleOrDefault
    End Function
End Class


Public Class CoreNonTransaction
    <Key> _
    Public Property Refno As Nullable(Of Decimal)
    Public Property TDate As DateTime
    Public Property AccNo As String
    Public Property CoreCurrId As String
    Public Property BranchId As Nullable(Of Decimal)
    Public Property Amount As Nullable(Of Decimal)
    Public Property Time As Integer
    Public Property Rate As Nullable(Of Decimal)
    Public Property FlagLHBU As Integer

    Public Shared Function GetAccount(ByVal AccNo As String, ByVal db As FectorEntities) As MsAccount
        Return db.Accounts.Where(Function(f) f.AccNo = AccNo).SingleOrDefault
    End Function
End Class
