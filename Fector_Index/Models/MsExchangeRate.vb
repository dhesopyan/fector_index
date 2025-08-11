Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class MsExchangeRate
    <Key> _
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property RateId As Decimal

    <DisplayName("Period")> _
    Public Property TTime As DateTime

    <ForeignKey("Currency")> _
    <DisplayName("Currency")> _
    <Required(ErrorMessage:="Please select currency")> _
    Public Property CurrId As String
    Public Overridable Property Currency As MsCurrency

    Public Property CoreCurrId As String

    <DisplayName("TT Buy Rate")> _
    <Required(ErrorMessage:="Please fill TT Buy Rate")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property TTBuyRate As Nullable(Of Decimal)

    <DisplayName("TT Sell Rate")> _
    <Required(ErrorMessage:="Please fill TT Sell Rate")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property TTSellRate As Nullable(Of Decimal)

    <DisplayName("BN Buy Rate")> _
    <Required(ErrorMessage:="Please fill BN Buy Rate")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property BNBuyRate As Nullable(Of Decimal)

    <DisplayName("BN Sell Rate")> _
    <Required(ErrorMessage:="Please fill BN Sell Rate")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property BNSellRate As Nullable(Of Decimal)

    <DisplayName("Closing Rate")> _
    <Required(ErrorMessage:="Please fill Closing Rate")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property ClosingRate As Nullable(Of Decimal)
    
    Public Property EditBy As String
    Public Property EditDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property Status As String
End Class
