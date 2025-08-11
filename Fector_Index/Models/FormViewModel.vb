Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel

Public Class LoginViewModel
    <Required(ErrorMessage:="Please fill username")> _
    <StringLength(20)> _
    Public Property Username As String

    <Required(ErrorMessage:="Please fill password")> _
    <StringLength(20)> _
    Public Property Password As String

    Public Property ReturnUrl As String
End Class

Public Class ChangePasswordViewModel
    <Required(ErrorMessage:="Please fill old password")> _
    <StringLength(20)> _
    <DisplayName("Old Password")> _
    Public Property OldPassword As String

    <Required(ErrorMessage:="Please fill new password")> _
    <StringLength(20)> _
    <DisplayName("New Password")> _
    Public Property NewPassword As String

    <Required(ErrorMessage:="Please fill confirmation of new password")> _
    <StringLength(20)> _
    <DisplayName("Confirm New Password")> _
    Public Property ConfirmNewPassword As String
End Class

Public Class DashboardViewModel
    <Required(ErrorMessage:="Please choose the currency for Exchange Rate Movements")> _
    <StringLength(20)> _
    <DisplayName("Currency")> _
    Public Property ExRateCurr As String

    <Required(ErrorMessage:="Please choose the currency for Total Deal")> _
    <StringLength(20)> _
    <DisplayName("Currency")> _
    Public Property ExRateDeal As String

    <Required(ErrorMessage:="Please choose the currency for Total Transaction")> _
    <StringLength(20)> _
    <DisplayName("Currency")> _
    Public Property ExRateTrans As String

    <Required(ErrorMessage:="Please choose the Branch")> _
    <StringLength(20)> _
    <DisplayName("Branch")> _
    Public Property DealBranch As String

    <Required(ErrorMessage:="Please choose the Branch")> _
    <StringLength(20)> _
    <DisplayName("Branch")> _
    Public Property TransactionBranch As String

    <Required(ErrorMessage:="Please choose the period for Total Deal")> _
    <StringLength(20)> _
    <DisplayName("Period")> _
    Public Property StartPeriod As DateTime
    Public Property EndPeriod As DateTime
End Class
