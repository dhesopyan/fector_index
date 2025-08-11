Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel

Public Class MsPurpose
    <Key> _
    <StringLength(2)> _
    <DisplayName("Purpose Code")> _
    <Required(ErrorMessage:="Please fill purpose code")> _
    Public Property PurposeId As String

    <StringLength(100)> _
    <DisplayName("Description")> _
    <Required(ErrorMessage:="Please fill the description")> _
    Public Property Description As String

    Public Property EditBy As String
    Public Property EditDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property Status As String

    Public Shared Function GetAllPurpose(ByVal PurposeId As String, ByVal db As FectorEntities) As MsPurpose
        Return db.Purposes.Where(Function(f) f.PurposeId = PurposeId).SingleOrDefault
    End Function

    Public Shared Function GetActivePurpose(ByVal PurposeId As String, ByVal db As FectorEntities) As MsPurpose
        Return db.Purposes.Where(Function(f) f.PurposeId = PurposeId And f.Status = "ACTIVE").SingleOrDefault
    End Function

    Public ReadOnly Property PurposeDisplay As String
        Get
            Return String.Format("{0} - {1}", PurposeId, Description)
        End Get
    End Property

End Class

