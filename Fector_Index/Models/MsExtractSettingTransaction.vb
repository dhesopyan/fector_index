Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class MsExtractSettingTransaction
    <Key> _
    <Column(order:=1)> _
    <DisplayName("COA")> _
    <StringLength(20)> _
    <Required(ErrorMessage:="Please fill COA")> _
    Public Property GLCode As String

    <Key> _
    <Column(order:=2)> _
    <DisplayName("Transaction Code")> _
    <StringLength(100)> _
    <Required(ErrorMessage:="Please fill transaction code")> _
    Public Property TransCode As String

    Public Property EditBy As String
    Public Property EditDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property Status As String

    Public Shared Function GetExtractSettingTransaction(ByVal GLCode As String, ByVal db As FectorEntities) As MsExtractSettingTransaction
        Return db.ExtractSettingTransaction.Where(Function(f) f.GLCode = GLCode And f.Status = "ACTIVE").SingleOrDefault
    End Function

    Public Shared Function GetExtractSettingTransaction2(ByVal TransCode As String, ByVal db As FectorEntities) As MsExtractSettingTransaction
        Return db.ExtractSettingTransaction.Where(Function(f) f.TransCode = TransCode And f.Status = "ACTIVE").SingleOrDefault
    End Function

    Public Shared Function GetAllExtractSettingTransaction(ByVal GLCode As String, ByVal TransCode As String, ByVal db As FectorEntities) As MsExtractSettingTransaction
        Return db.ExtractSettingTransaction.Where(Function(f) f.GLCode = GLCode And f.TransCode = TransCode).SingleOrDefault
    End Function
End Class
