Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class MsBusinessType
    <Key> _
    <DisplayName("Business Type ID")> _
    <StringLength(2)> _
    <Required(ErrorMessage:="Please fill business type id")> _
    Public Property BusinessTypeId As String

    <DisplayName("Description")> _
    <StringLength(100)> _
    <Required(ErrorMessage:="Please fill description")> _
    Public Property Description As String


    Public Property EditBy As String
    Public Property EditDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property Status As String

    Public ReadOnly Property DisplayValue As String
        Get
            Return BusinessTypeId & " - " & Description
        End Get
    End Property

    Public Shared Function GetBusinessType(ByVal BusinessTypeId As String, ByVal db As FectorEntities) As MsBusinessType
        Return db.BusinessType.Where(Function(f) f.BusinessTypeId = BusinessTypeId And f.Status = "ACTIVE").SingleOrDefault
    End Function

    Public Shared Function GetAllBusinessType(ByVal BusinessTypeId As String, ByVal db As FectorEntities) As MsBusinessType
        Return db.BusinessType.Where(Function(f) f.BusinessTypeId = BusinessTypeId).SingleOrDefault
    End Function
End Class
