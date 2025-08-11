Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class MsAvailableMenu
    <Key> _
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property ID As Long

    <ForeignKey("SubMenu")> _
    Public Property SubMenuId As Integer
    Public Overridable Property SubMenu As MsMenu

    Public Property Link As String


End Class
