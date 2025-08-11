Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel

Public Class MsRTExchangeRate
    <Key> _
    <StringLength(3)> _
    Public Property CurrId As String

    <StringLength(3)> _
    Public Property CoreCurrId As String

    Public Property Description As String

    Public Property TTBuyRate As Nullable(Of Decimal)
    Public Property TTSellRate As Nullable(Of Decimal)
    Public Property BNBuyRate As Nullable(Of Decimal)
    Public Property BNSellRate As Nullable(Of Decimal)
    Public Property ClosingRate As Nullable(Of Decimal)

    Public ReadOnly Property CurrencyDisplay As String
        Get
            Return String.Format("{0} - {1}", CurrId, Description)
        End Get
    End Property
End Class
