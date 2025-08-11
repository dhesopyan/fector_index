Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Security.Cryptography
Imports System.Data.Entity.Validation

Public Class LogTransaction
    <Key> _
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property LogId As Long

    <ForeignKey("User")> _
    Public Property UserId As String
    Public Property User As MsUser

    Public Property Tdate As Date
    Public Property Action As String
    Public Property RefNo As String

    Public Shared Sub WriteLog(ByVal UserId As String, ByVal Action As String, ByVal RefNo As String, ByVal db As FectorEntities)
        Dim newlog As New LogTransaction() With {.UserId = UserId, .Tdate = Now, .Action = Action, .RefNo = RefNo}
        db.LogTransactions.Add(newlog)
        db.SaveChanges()
    End Sub

End Class
