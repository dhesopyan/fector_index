Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class MsSettingViewModel
    <DisplayName("Country")> _
    <Required(ErrorMessage:="Please choose Country")> _
    Public Property BankCountryID As String

    <DisplayName("Subject Status")> _
    <Required(ErrorMessage:="Please choose Subject Status")> _
    Public Property BankSubjectStatusID As String

    <DisplayName("BI Code")> _
    <Required(ErrorMessage:="Please choose BI Code")> _
    Public Property BankBICode As String

    <DisplayName("Business Type")> _
    <Required(ErrorMessage:="Please choose Business Type")> _
    Public Property BankBusinessTypeID As String

    <DisplayName("Limit Transaction")> _
    <Required(ErrorMessage:="Please fill Limit Transaction")> _
    Public Property TransactionLimit As String

    <Required(ErrorMessage:="Please choose Limit Currency")> _
    Public Property LimitCurrency As String

    <DisplayName("Bank Name")> _
    <Required(ErrorMessage:="Please fill Bank Name")> _
    Public Property BankName As String

    Public Shared Function GetMsSetting(key1 As String, key2 As String, key3 As String, valueindex As Integer, db As FectorEntities) As MsSetting
        Return db.Settings.Where(Function(s) s.Key1 = key1 And s.Key2 = key2 And s.Key3 = key3).SingleOrDefault
    End Function
End Class
