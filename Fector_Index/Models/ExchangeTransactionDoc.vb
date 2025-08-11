Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel
Imports System.IO

Public Class ExchangeTransactionDoc
    <Key> _
    <Column(order:=1)> _
    <DisplayName("ID")> _
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property ID As Decimal

    <Key> _
    <Column(order:=2)> _
    <ForeignKey("ExchangeTransactionHead")> _
    <DisplayName("Transaction Number")> _
    Public Property TransNum As Decimal
    Public Overridable Property ExchangeTransactionHead As ExchangeTransactionHead

    Public Property DocumentTransId As String
    Public Property DocumentLHBUId As String
    Public Property PurposeId As String

    <DisplayName("Document Underlying Number")> _
    Public Property DocUnderlyingNum As String

    Public Property NominalDoc As Decimal

    Public Property DocumentTransactionLink As String
    Public Property TransFormulirLink As String
End Class
