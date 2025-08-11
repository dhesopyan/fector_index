Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class MsSettingSecurityViewModel
    <DisplayName("Login Timeout")> _
    <Required(ErrorMessage:="Please fill Login Timeout")> _
    Public Property LoginTimeout As Integer

    <DisplayName("Minimum Length Password")> _
    <Required(ErrorMessage:="Please fill Minimum Length Password")> _
    Public Property MinPassword As Integer

    <DisplayName("Maximum Length Password")> _
    <Required(ErrorMessage:="Please fill Maximum Length Password")> _
    Public Property MaxPassword As Integer

    <DisplayName("Maximum Incorrect Password")> _
    <Required(ErrorMessage:="Please fill Maximum Incorrect Password")> _
    Public Property MaxIncorrect As Integer

    <DisplayName("Password Expired (Days)")> _
    <Required(ErrorMessage:="Please fill Password Expired")> _
    Public Property PasswordExpired As String
End Class
