Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class MsOtherAccount
    <Key> _
    <DisplayName("Account Number")> _
    Public Property AccNo As String

    <DisplayName("Name")> _
    Public Property Name As String

    <ForeignKey("Country")> _
    <Required(ErrorMessage:="Please fill country in LLD Application")> _
    Public Property CountryId As String
    Public Overridable Property Country As MsCountry

    <ForeignKey("SubjectStatus")> _
    <DisplayName("Status")> _
    <Required(ErrorMessage:="Please select subject status")> _
    Public Property SubjectStatusId As String
    Public Overridable Property SubjectStatus As MsSubjectStatus

    <DisplayName("BI Code")> _
    <Required(ErrorMessage:="Please select BI code")> _
    Public Property BICode As String

    <ForeignKey("BusinessType")> _
    <DisplayName("Business Type")> _
    <Required(ErrorMessage:="Please select business type")> _
    Public Property BusinessTypeId As String
    Public Overridable Property BusinessType As MsBusinessType

    <DisplayName("LHBU Exclusion")> _
    Public Property flagNonLHBU As Nullable(Of Boolean)

    <ForeignKey("TODCurrency")> _
    <DisplayName("TOD Limit Currency")> _
    Public Property TODLimitcurrency As String
    Public Overridable Property TODCurrency As MsCurrency

    <DisplayName("TOD Limit")> _
    <Required(ErrorMessage:="Invalid TOD Limit")> _
    Public Property TODLimit As Nullable(Of Decimal)

    <ForeignKey("TOMCurrency")> _
    <DisplayName("TOM Limit Currency")> _
    Public Property TOMLimitcurrency As String
    Public Overridable Property TOMCurrency As MsCurrency

    <DisplayName("TOM Limit")> _
    <Required(ErrorMessage:="Invalid TOM Limit")> _
    Public Property TOMLimit As Nullable(Of Decimal)

    <ForeignKey("SPOTCurrency")> _
    <DisplayName("SPOT Limit Currency")> _
    Public Property SPOTLimitcurrency As String
    Public Overridable Property SPOTCurrency As MsCurrency

    <DisplayName("SPOT Limit")> _
    <Required(ErrorMessage:="Invalid SPOT Limit")> _
    Public Property SPOTLimit As Nullable(Of Decimal)

    <ForeignKey("ALLCurrency")> _
    <DisplayName("Combination Limit Currency")> _
    Public Property ALLLimitcurrency As String
    Public Overridable Property ALLCurrency As MsCurrency

    <DisplayName("Combination Limit")> _
    <Required(ErrorMessage:="Invalid Combination Limit")> _
    Public Property ALLLimit As Nullable(Of Decimal)
    Public Property EditBy As String
    Public Property EditDate As Nullable(Of DateTime)
    Public Property ApproveBy As String
    Public Property ApproveDate As Nullable(Of DateTime)
    Public Property Status As String

    Public Shared Function GetOtherAccount(ByVal AccNo As String, ByVal db As FectorEntities) As MsOtherAccount
        Return db.OtherAccounts.Where(Function(f) f.AccNo = AccNo).SingleOrDefault
    End Function
End Class

Public Class MsOtherAccountExtension
    <Key> _
    Public Property AccNo As String

    <ForeignKey("SubjectStatus")> _
    Public Property SubjectStatusId As String
    Public Overridable Property SubjectStatus As MsSubjectStatus

    <DisplayName("BI Code")> _
    <Required(ErrorMessage:="Please select BI code")> _
    Public Property BICode As String

    <ForeignKey("BusinessType")> _
    Public Property BusinessTypeId As String
    Public Overridable Property BusinessType As MsBusinessType

    Public Property flagNonLHBU As Boolean
    Public Property EditBy As String
    Public Property EditDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property Status As String

    Public Shared Function GetOtherAccountExtension(ByVal AccNo As String, ByVal db As FectorEntities) As MsOtherAccountExtension
        Return db.OtherAccountsExtension.Where(Function(f) f.AccNo = AccNo).SingleOrDefault
    End Function
End Class

Public Class MsOtherAccountLimit
    <Key> _
    Public Property AccNo As String

    <ForeignKey("TODCurrency")> _
    Public Property TODLimitcurrency As String
    Public Overridable Property TODCurrency As MsCurrency

    Public Property TODLimit As Nullable(Of Decimal)

    <ForeignKey("TOMCurrency")> _
    Public Property TOMLimitcurrency As String
    Public Overridable Property TOMCurrency As MsCurrency

    Public Property TOMLimit As Nullable(Of Decimal)

    <ForeignKey("SPOTCurrency")> _
    Public Property SPOTLimitcurrency As String
    Public Overridable Property SPOTCurrency As MsCurrency

    Public Property SPOTLimit As Nullable(Of Decimal)

    <ForeignKey("ALLCurrency")> _
    Public Property ALLLimitcurrency As String
    Public Overridable Property ALLCurrency As MsCurrency

    Public Property ALLLimit As Nullable(Of Decimal)

    Public Shared Function GetOtherAccountLimit(ByVal AccNo As String, ByVal db As FectorEntities) As MsOtherAccountLimit
        Return db.OtherAccountsLimit.Where(Function(f) f.AccNo = AccNo).SingleOrDefault
    End Function
End Class
