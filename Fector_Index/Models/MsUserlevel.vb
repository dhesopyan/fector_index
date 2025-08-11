Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class MsUserlevel
    <Key> _
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property UserlevelId As Integer

    Public Property Description As String
    Public Property FlagUseLimit As Boolean
    Public Property EditBy As String
    Public Property EditDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property Status As String

    Public Shared Function GetAllUserLevel(ByVal UserLevelId As Integer, ByVal db As FectorEntities) As MsUserlevel
        Return db.UserLevels.Where(Function(f) f.UserlevelId = UserLevelId).SingleOrDefault
    End Function
End Class