Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class CloseDealView
    <Key> _
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property DealNumber As String

    <DisplayName("Close Deal Reason")> _
    <StringLength(100)> _
    <Required(ErrorMessage:="Please fill reason")> _
    Public Property CloseDealReason As String
    Public Property ProcessName As String

    Public Shared Function GetCloseDealReason(ByVal DealNumber As String, ByVal db As FectorEntities) As CloseDealView
        Dim TransactionDeal = db.TransactionDeal.Where(Function(f) f.DealNumber = DealNumber).SingleOrDefault

        Dim mdl As New CloseDealView
        With mdl
            .DealNumber = TransactionDeal.DealNumber
            .CloseDealReason = TransactionDeal.CloseReason
        End With

        Return mdl
    End Function
End Class
