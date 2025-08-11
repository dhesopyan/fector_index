Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class MsMappingNostro
    <Key> _
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property ID As Decimal

    <DisplayName("Nostro")> _
    <ForeignKey("Nostro")> _
    <Required(ErrorMessage:="Please choose the Nostro")> _
    Public Property BIC As String
    Public Overridable Property Nostro As MsNostro

    <DisplayName("Currency")> _
    <ForeignKey("MsCurrency")> _
    <Required(ErrorMessage:="Please choose the Currency")> _
    Public Property CurrID As String
    Public Overridable Property MsCurrency As MsCurrency

    Public Property EditBy As String
    Public Property EditDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property Status As String

    Public Shared Function GetMappingNostro(ByVal id As String, ByVal db As FectorEntities) As MsMappingNostro
        Return db.MappingNostro.Where(Function(f) f.ID = id).FirstOrDefault
    End Function

    Public Shared Function GetExistingMappingNostro(ByVal BIC As String, ByVal CurrID As String, ByVal db As FectorEntities, ByVal Mode As String, Optional ID As Integer = 0) As MsMappingNostro
        If Mode = "EDIT" Then
            Return db.MappingNostro.Where(Function(f) f.BIC = BIC And f.CurrID = CurrID And ID <> ID).FirstOrDefault
        End If
        Return db.MappingNostro.Where(Function(f) f.BIC = BIC And f.CurrID = CurrID).FirstOrDefault
    End Function
End Class
